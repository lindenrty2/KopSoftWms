using IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiClient;
using YL.Core.Dto;
using YL.Core.Entity;
using YL.Utils.Extensions;
using YL.Utils.Pub;

namespace WMSCore.Outside
{
    [Route("/hook/wms")]
    public class WMSHookController : Controller
    {

        private readonly IWms_stockinServices _stockinServices;
        private readonly IWms_stockindetailServices _stockindetailServices;
        private readonly IWms_stockindetailboxServices _stockindetailboxServices;
        private readonly IWms_stockoutServices _stockoutServices;
        private readonly IWms_inventoryBoxServices _inventoryBoxServices;
        private readonly IWms_inventoryBoxTaskServices _inventoryBoxTaskServices;
        private readonly SqlSugarClient _client;
        private readonly ILogger<WMSHookController> _logger;
        public WMSHookController(
            SqlSugarClient client,
            IWms_stockinServices stockinServices,
            IWms_stockindetailServices stockindetailServices,
            IWms_stockindetailboxServices stockindetailboxServices,
            IWms_stockoutServices stockoutServices,
            IWms_inventoryBoxServices inventoryBoxServices,
            IWms_inventoryBoxTaskServices inventoryBoxTaskServices,
            ILogger<WMSHookController> logger)
        {
            _client = client;
            _logger = logger;
            _stockinServices = stockinServices;
            _stockindetailServices = stockindetailServices;
            _stockindetailboxServices = stockindetailboxServices;
            _stockoutServices = stockoutServices;
            _inventoryBoxServices = inventoryBoxServices;
            _inventoryBoxTaskServices = inventoryBoxTaskServices;
        }

        [HttpGet("ping")]
        public string Ping(string s)
        {
            return "OK";
        }

        /// <summary>
        /// 入库任务状态变化通知
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        [HttpPost("/StockIn/{stockInId}/Status")]
        public async Task<RouteData> StockInReport(long stockInId,[FromBody]OutsideStockInReportDto result)
        {
            try
            {
                result.StockInId = stockInId;
                Wms_stockin stockIn = await _client.Queryable<Wms_stockin>()
                     .FirstAsync(x => x.StockInId == result.StockInId);
                if (stockIn == null)
                {
                    return YL.Core.Dto.RouteData.From(PubMessages.E2013_STOCKIN_NOTFOUND);
                }
                if (stockIn.StockInStatus == StockInStatus.task_finish.ToInt32())
                {
                    return YL.Core.Dto.RouteData.From(PubMessages.E2014_STOCKIN_ALLOW_FINISHED);
                }

                Wms_mestask mesTask = await _client.Queryable<Wms_mestask>()
                    .FirstAsync(x => x.MesTaskId == stockIn.MesTaskId);
                if (mesTask == null)
                {
                    return YL.Core.Dto.RouteData.From(PubMessages.E3000_MES_STOCKINTASK_NOTFOUND);
                }

                Wms_stockindetail[] details = _client.Queryable<Wms_stockindetail>()
                    .Where(x => x.StockInId == result.StockInId).ToArray();

                _client.BeginTran();
                foreach (OutsideStockInReportDetail detail in result.Details)
                {
                    Wms_stockindetail localDetail = details.FirstOrDefault(x => x.MaterialNo == detail.MaterialNo && x.MaterialOnlyId == detail.MaterialOnlyId);
                    if (localDetail == null)
                    {
                        _client.RollbackTran();
                        return YL.Core.Dto.RouteData.From(PubMessages.E2015_STOCKIN_HASNOT_MATERIAL, $"MaterialId={detail.MaterialId}");
                    }
                    localDetail.PlanInQty = detail.PlanInQty;
                    localDetail.ActInQty = detail.ActInQty;
                    localDetail.Status = detail.Status.ToInt32();
                    localDetail.ModifiedBy = PubConst.InterfaceUserId;
                    localDetail.ModifiedUser = detail.ModifiedBy;
                    localDetail.ModifiedDate = Convert.ToDateTime(detail.ModifiedDate);
                    localDetail.Remark = detail.Remark;
                }

                if (_client.Updateable(details).ExecuteCommand() == 0)
                {
                    _client.RollbackTran();
                    return YL.Core.Dto.RouteData.From(PubMessages.E0002_UPDATE_COUNT_FAIL);
                }

                stockIn.StockInStatus = result.StockInStatus.ToInt32();
                stockIn.ModifiedBy = PubConst.InterfaceUserId;
                stockIn.ModifiedUser = PubConst.InterfaceUserName;

                if (_client.Updateable(stockIn).ExecuteCommand() == 0)
                {
                    _client.RollbackTran();
                    return YL.Core.Dto.RouteData.From(PubMessages.E0002_UPDATE_COUNT_FAIL);
                }

                Wms_stockin[] stockins = _client.Queryable<Wms_stockin>()
                    .Where(x => x.MesTaskId == mesTask.MesTaskId).ToArray();

                if (!stockins.Any(x => x.StockInStatus != StockInStatus.task_finish.ToInt32() && x.StockInStatus != StockInStatus.task_canceled.ToInt32()))
                {
                    await NofityStockIn(mesTask);
                }

                _client.CommitTran();
                return new RouteData();
            }
            catch (Exception ex)
            {
                _client.RollbackTran();
                return new RouteData() { Code = -1, Message = ex.Message };
            }
        }

        /// <summary>
        /// 通知MES入库完成
        /// </summary>
        /// <param name="stockOutId"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private async Task<RouteData> NofityStockIn(Wms_mestask mesTask)
        {
         

            mesTask.ModifiedDate = DateTime.Now;
            mesTask.WorkStatus = MESTaskWorkStatus.WorkComplated;
            mesTask.NotifyStatus = MESTaskNotifyStatus.WaitResponse;

            bool notifyComplate = false;
            try
            {
                List<Wms_stockin> stockIns = await _client.Queryable<Wms_stockin>().Where(x => x.MesTaskId == mesTask.MesTaskId).ToListAsync();

                List<OutsideStockInResponseWarehouse> warehouseList = new List<OutsideStockInResponseWarehouse>();
                foreach (Wms_stockin stockIn in stockIns)
                {
                    OutsideStockInResponseWarehouse warehouse = warehouseList.FirstOrDefault(x => x.WarehouseId == stockIn.WarehouseId.ToString());
                    if(warehouse == null)
                    {

                        warehouse = new OutsideStockInResponseWarehouse()
                        {
                            WarehouseId = stockIn.WarehouseId.ToString(),
                            WarehouseName = "", //TODO
                            WarehousePosition = "",
                            WarehousingFinishTime = stockIn.ModifiedDate.Value.ToString("yyyy-MM-dd HH:mm:ss"), 
                        };
                        warehouseList.Add(warehouse);
                    }
                    List<Wms_stockindetail> stockInDetails = await _client.Queryable<Wms_stockindetail>().Where(x => x.StockInId == stockIn.StockInId).ToListAsync();
                    foreach (Wms_stockindetail stockInDetail in stockInDetails)
                    {
                        OutsideMaterialResult material = new OutsideMaterialResult()
                        {
                            SuppliesId = stockInDetail.MaterialNo.ToString(),
                            SuppliesName = stockInDetail.MaterialName,
                            SuppliesNumber = stockInDetail.ActInQty.ToString(),
                            RefreshStock = stockInDetail.ActInQty.ToString(),
                            ErrorId = "", //TODO  stockInDetail.ErrorId,
                            ErrorInfo = "", //TODO stockInDetail.ErrorInfo

                        };
                        warehouse.SuppliesInfoList.Add(material);
                        warehouse.SuppliesKinds = warehouse.SuppliesInfoList.Count;
                    }
                }

                OutsideStockInResponse response = new OutsideStockInResponse()
                {
                    WarehousingId = mesTask.WarehousingId,
                    WarehousingEntryNumber = warehouseList.Count,
                    WarehousingEntryFinishList = JsonConvert.SerializeObject(warehouseList)
                };

                await MESApiAccessor.Instance.WarehousingFinish(response);
                notifyComplate = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "入库完成通知时发生异常"); 
                //逻辑继续,寻找其它时机重新通知
            }
            mesTask.NotifyStatus = notifyComplate ? MESTaskNotifyStatus.Responsed : MESTaskNotifyStatus.Failed;
            if(_client.Updateable(mesTask).ExecuteCommand() == 0)
            {
                return YL.Core.Dto.RouteData.From(PubMessages.E0002_UPDATE_COUNT_FAIL);

            }

            if (notifyComplate)
            {
                return new RouteData();
            }
            else
            {
                return YL.Core.Dto.RouteData.From(PubMessages.E3100_MES_STOCKOUTTASK_NOTFOUND);

            }
        }

        /// <summary>
        /// 出库任务状态变化通知
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        [HttpPost("/StockOut/{stockOutId}/Status")]
        public async Task<RouteData> StockOutReport(long stockOutId, [FromBody]OutsideStockOutReportDto result)
        {
            try
            {
                result.StockOutId = stockOutId;
                Wms_stockout stockOut = await _client.Queryable<Wms_stockout>()
                     .FirstAsync(x => x.StockOutId == result.StockOutId);
                if (stockOut == null)
                {
                    return YL.Core.Dto.RouteData.From(PubMessages.E2113_STOCKOUT_NOTFOUND);
                }
                if (stockOut.StockOutStatus == StockOutStatus.task_finish.ToInt32())
                {
                    return YL.Core.Dto.RouteData.From(PubMessages.E2114_STOCKOUT_ALLOW_FINISHED);
                }
                Wms_mestask mesTask = await _client.Queryable<Wms_mestask>()
                    .FirstAsync(x => x.MesTaskId == stockOut.MesTaskId);
                if (mesTask == null)
                {
                    return YL.Core.Dto.RouteData.From(PubMessages.E3000_MES_STOCKINTASK_NOTFOUND);
                }


                Wms_stockoutdetail[] details = _client.Queryable<Wms_stockoutdetail>()
                    .Where(x => x.StockOutId == result.StockOutId).ToArray();

                _client.BeginTran();
                foreach (OutsideStockOutReportDetail detail in result.Details)
                {
                    Wms_stockoutdetail localDetail = details.FirstOrDefault(x => x.MaterialNo == detail.MaterialNo && x.MaterialOnlyId == detail.MaterialOnlyId);
                    localDetail.PlanOutQty = detail.PlanOutQty;
                    localDetail.ActOutQty = detail.ActOutQty;
                    localDetail.Status = detail.Status.ToInt32();
                    localDetail.ModifiedBy = PubConst.InterfaceUserId;
                    localDetail.ModifiedUser = detail.ModifiedBy;
                    localDetail.ModifiedDate = Convert.ToDateTime(detail.ModifiedDate); 
                    localDetail.Remark = detail.Remark;
                }

                if (_client.Updateable(details).ExecuteCommand() == 0)
                {
                    _client.RollbackTran();
                    return YL.Core.Dto.RouteData.From(PubMessages.E0002_UPDATE_COUNT_FAIL);
                }

                stockOut.StockOutStatus = result.StockOutStatus.ToInt32();
                stockOut.ModifiedBy = PubConst.InterfaceUserId;
                stockOut.ModifiedUser = PubConst.InterfaceUserName;

                if (_client.Updateable(stockOut).ExecuteCommand() == 0)
                {
                    _client.RollbackTran();
                    return YL.Core.Dto.RouteData.From(PubMessages.E0002_UPDATE_COUNT_FAIL);
                }
                 
                Wms_stockout[] stockouts = _client.Queryable<Wms_stockout>()
                    .Where(x => x.MesTaskId == mesTask.MesTaskId).ToArray();

                if (!stockouts.Any(x => x.StockOutStatus != StockOutStatus.task_finish.ToInt32() && x.StockOutStatus != StockOutStatus.task_canceled.ToInt32()))
                {
                    await NofityStockIn(mesTask);
                }

                _client.CommitTran();
                return new RouteData();
            }
            catch (Exception ex)
            {
                _client.RollbackTran();
                return new RouteData() { Code = -1, Message = ex.Message };
            }
        }


        /// <summary>
        /// 通知MES出库完成
        /// </summary>
        /// <param name="stockOutId"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private async Task<RouteData> NofityStockOut(Wms_mestask mesTask)
        {


            mesTask.ModifiedDate = DateTime.Now;
            mesTask.WorkStatus = MESTaskWorkStatus.WorkComplated;
            mesTask.NotifyStatus = MESTaskNotifyStatus.WaitResponse;

            bool notifyComplate = false;
            try
            {
                List<Wms_stockout> stockOuts = await _client.Queryable<Wms_stockout>().Where(x => x.MesTaskId == mesTask.MesTaskId).ToListAsync();

                List<OutsideStockOutResponseWarehouse> warehouseList = new List<OutsideStockOutResponseWarehouse>();
                foreach (Wms_stockout stockOut in stockOuts)
                {
                    OutsideStockOutResponseWarehouse warehouse = warehouseList.FirstOrDefault(x => x.WarehouseId == stockOut.WarehouseId.ToString());
                    if (warehouse == null)
                    {

                        warehouse = new OutsideStockOutResponseWarehouse()
                        {
                            WarehouseId = stockOut.WarehouseId.ToString(),
                            WarehouseName = "", //TODO
                            WorkAreaName = mesTask.WorkAreaName, 
                            WarehouseEntryFinishTime = stockOut.ModifiedDate.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                        };
                        warehouseList.Add(warehouse);
                    }
                    List<Wms_stockoutdetail> stockOutDetails = await _client.Queryable<Wms_stockoutdetail>().Where(x => x.StockOutId == stockOut.StockOutId).ToListAsync();
                    foreach (Wms_stockoutdetail stockOutDetail in stockOutDetails)
                    {
                        OutsideMaterialResult material = new OutsideMaterialResult()
                        {
                            SuppliesId = stockOutDetail.MaterialNo.ToString(),
                            SuppliesName = stockOutDetail.MaterialName,
                            SuppliesNumber = stockOutDetail.ActOutQty.ToString(),
                            RefreshStock = stockOutDetail.ActOutQty.ToString(),
                            ErrorId = "", //TODO  stockInDetail.ErrorId,
                            ErrorInfo = "", //TODO stockInDetail.ErrorInfo

                        };
                        warehouse.SuppliesInfoList.Add(material);
                        warehouse.SuppliesKinds = warehouse.SuppliesInfoList.Count;
                    }
                }

                OutsideStockOutResponse response = new OutsideStockOutResponse()
                {
                    WarehouseEntryId = mesTask.WarehousingId,
                    WarehouseEntryFinishCount = warehouseList.Count,
                    WarehouseEntryFinishList = JsonConvert.SerializeObject(warehouseList)
                };

                await MESApiAccessor.Instance.WarehouseEntryFinish(response);
                notifyComplate = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "出库完成通知时发生异常");
                //逻辑继续,寻找其它时机重新通知
            }
            mesTask.NotifyStatus = notifyComplate ? MESTaskNotifyStatus.Responsed : MESTaskNotifyStatus.Failed;
            if (_client.Updateable(mesTask).ExecuteCommand() == 0)
            {
                return YL.Core.Dto.RouteData.From(PubMessages.E0002_UPDATE_COUNT_FAIL);

            }

            if (notifyComplate)
            {
                return new RouteData();
            }
            else
            {
                return YL.Core.Dto.RouteData.From(PubMessages.E3100_MES_STOCKOUTTASK_NOTFOUND);

            }
        }

    }


}
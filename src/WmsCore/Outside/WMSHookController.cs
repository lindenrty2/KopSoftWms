using IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Services.Outside;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiClient;
using YL.Core.Dto;
using YL.Core.Entity;
using YL.NetCore.Attributes;
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
        [OutsideLog]
        [HttpPost("StockIn/{stockInId}/Status")]
        public async Task<RouteData> StockInReport(long stockInId, [FromBody]OutsideStockInReportDto result)
        {
            try
            {
                _logger.LogInformation($"[入库任务状态变化通知]收到通知,StockInId={stockInId},data={JsonConvert.SerializeObject(result)}");
                result.StockInId = stockInId;
                Wms_stockin stockIn = await _client.Queryable<Wms_stockin>()
                     .FirstAsync(x => x.StockInId == result.StockInId);
                if (stockIn == null)
                {
                    _logger.LogError($"[入库任务状态变化通知]E2013-没有找到入库单,StockInId={stockInId}");
                    return YL.Core.Dto.RouteData.From(PubMessages.E2013_STOCKIN_NOTFOUND);
                }
                if (stockIn.StockInStatus == StockInStatus.task_finish.ToInt32())
                {
                    _logger.LogError($"[入库任务状态变化通知]E2014-入库单状态已标记为完成,本次操作中断,StockInId={stockInId}, StockInNo={stockIn.StockInNo}");
                    return YL.Core.Dto.RouteData.From(PubMessages.E2014_STOCKIN_ALLOW_FINISHED);
                }

                Wms_mestask mesTask = await _client.Queryable<Wms_mestask>()
                    .FirstAsync(x => x.MesTaskId == stockIn.MesTaskId);
                if (mesTask == null)
                {
                    _logger.LogError($"[入库任务状态变化通知]E3000-没有找到相应的Mes任务,StockInId={stockInId}, StockInNo={stockIn.StockInNo}, MesTaskId={stockIn.MesTaskId}");
                    return YL.Core.Dto.RouteData.From(PubMessages.E3000_MES_STOCKINTASK_NOTFOUND);
                }

                Wms_stockindetail[] details = _client.Queryable<Wms_stockindetail>()
                    .Where(x => x.StockInId == result.StockInId).ToArray();

                _client.BeginTran();
                foreach (OutsideStockInReportDetail detail in result.Details)
                {
                    Wms_stockindetail localDetail = details.FirstOrDefault(
                        x => x.UniqueIndex == detail.UniqueIndex);
                    if (localDetail == null)
                    {
                        _client.RollbackTran();
                        _logger.LogError($"[入库任务状态变化通知]E2015-没有找到相应的物料,StockInId={stockInId}, StockInNo={stockIn.StockInNo} ,UniqueIndex ={ detail.UniqueIndex}");
                        return YL.Core.Dto.RouteData.From(PubMessages.E2015_STOCKIN_HASNOT_MATERIAL, $"MaterialId={detail.MaterialId}");
                    }
                    localDetail.PlanInQty = detail.PlanInQty;
                    localDetail.ActInQty = detail.ActInQty;
                    localDetail.Status = detail.Status.ToInt32();
                    localDetail.ModifiedBy = PubConst.InterfaceUserId;
                    localDetail.ModifiedUser = detail.ModifiedBy;
                    localDetail.ModifiedDate = Convert.ToDateTime(detail.ModifiedDate);
                    localDetail.Remark = detail.Remark;
                    localDetail.ErrorId = detail.ErrorId;
                    localDetail.ErrorInfo = detail.ErrorInfo;
                }

                if (_client.Updateable(details).ExecuteCommand() == 0)
                {
                    _client.RollbackTran();
                    _logger.LogError($"[入库任务状态变化通知]E0002-任务明细更新失败,StockInId={stockInId}, StockInNo={stockIn.StockInNo}");
                    return YL.Core.Dto.RouteData.From(PubMessages.E0002_UPDATE_COUNT_FAIL);
                }

                stockIn.StockInStatus = result.StockInStatus.ToInt32();
                stockIn.ModifiedBy = PubConst.InterfaceUserId;
                stockIn.ModifiedUser = PubConst.InterfaceUserName;

                if (_client.Updateable(stockIn).ExecuteCommand() == 0)
                {
                    _client.RollbackTran();
                    _logger.LogError($"[入库任务状态变化通知]E0002-任务更新失败,StockInId={stockInId}, StockInNo={stockIn.StockInNo}");
                    return YL.Core.Dto.RouteData.From(PubMessages.E0002_UPDATE_COUNT_FAIL);
                }

                var anyWorking = await _client.Queryable<Wms_stockin>()
                       .AnyAsync(x => x.MesTaskId == stockIn.MesTaskId
                       && x.StockInStatus != (int)StockInStatus.task_finish
                       && x.StockInStatus != (int)StockInStatus.task_canceled);

                if (!anyWorking)
                {
                    _logger.LogInformation($"[入库任务状态变化通知]所有相关任务已完成,尝试通知MES MesTaskId={stockIn.MesTaskId}");
                    await _client.NofityStockIn(mesTask);
                }
                else { 
                    _logger.LogInformation($"[入库任务状态变化通知]尚有未完成任务,等待其余任务完成通知 MesTaskId={stockIn.MesTaskId}");
                }

                _logger.LogInformation($"[入库任务状态变化通知]通知处理正常完成,StockInId={stockInId}");
                _client.CommitTran();
                return new RouteData();
            }
            catch (Exception ex)
            {
                _client.RollbackTran();
                _logger.LogError($"[入库任务状态变化通知]E-1-发生异常,处理结束 ex={ex.ToString()}");
                return new RouteData() { Code = -1, Message = ex.Message };
            }
        }


        /// <summary>
        /// 出库任务状态变化通知
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        [OutsideLog]
        [HttpPost("StockOut/{stockOutId}/Status")]
        public async Task<RouteData> StockOutReport(long stockOutId, [FromBody]OutsideStockOutReportDto result)
        {
            try
            {
                _logger.LogInformation($"[出库任务状态变化通知]收到通知,StockOutId={stockOutId},data={JsonConvert.SerializeObject(result)}");
                result.StockOutId = stockOutId;
                Wms_stockout stockOut = await _client.Queryable<Wms_stockout>()
                     .FirstAsync(x => x.StockOutId == result.StockOutId);
                if (stockOut == null)
                {
                    _logger.LogError($"[出库任务状态变化通知]E2113-没有找到出库单,StockOutId={stockOutId}");
                    return YL.Core.Dto.RouteData.From(PubMessages.E2113_STOCKOUT_NOTFOUND);
                }
                if (stockOut.StockOutStatus == StockOutStatus.task_finish.ToInt32())
                {
                    _logger.LogError($"[出库任务状态变化通知]E2114-出库单状态已标记为完成,本次操作中断,StockOutId={stockOutId}, StockOutNo={stockOut.StockOutNo}");
                    return YL.Core.Dto.RouteData.From(PubMessages.E2114_STOCKOUT_ALLOW_FINISHED);
                }
                Wms_mestask mesTask = await _client.Queryable<Wms_mestask>()
                    .FirstAsync(x => x.MesTaskId == stockOut.MesTaskId);
                if (mesTask == null)
                {
                    _logger.LogError($"[出库任务状态变化通知]E3000-没有找到相应的Mes任务,StockOutId={stockOutId}, StockOutNo={stockOut.StockOutNo}, MesTaskId={stockOut.MesTaskId}");
                    return YL.Core.Dto.RouteData.From(PubMessages.E3000_MES_STOCKINTASK_NOTFOUND);
                }


                Wms_stockoutdetail[] details = _client.Queryable<Wms_stockoutdetail>()
                    .Where(x => x.StockOutId == result.StockOutId).ToArray();

                _client.BeginTran();
                foreach (OutsideStockOutReportDetail detail in result.Details)
                {
                    Wms_stockoutdetail localDetail = details.FirstOrDefault(
                        x => x.UniqueIndex == detail.UniqueIndex);
                    if (localDetail == null)
                    {
                        _client.RollbackTran();
                        _logger.LogError($"[出库任务状态变化通知]E2115-没有找到相应的物料,StockOutId={stockOutId}, StockOutNo={stockOut.StockOutNo} ,UniqueOutdex ={ detail.UniqueOutdex}");
                        return YL.Core.Dto.RouteData.From(PubMessages.E2115_STOCKOUT_HASNOT_MATERIAL, $"MaterialId={detail.MaterialId}");
                    }
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
                    _logger.LogError($"[出库任务状态变化通知]E0002-任务明细更新失败,StockOutId={stockOutId}, StockOutNo={stockOut.StockOutNo}");
                    return YL.Core.Dto.RouteData.From(PubMessages.E0002_UPDATE_COUNT_FAIL);
                }

                stockOut.StockOutStatus = result.StockOutStatus.ToInt32();
                stockOut.ModifiedBy = PubConst.InterfaceUserId;
                stockOut.ModifiedUser = PubConst.InterfaceUserName;

                if (_client.Updateable(stockOut).ExecuteCommand() == 0)
                {
                    _client.RollbackTran();
                    _logger.LogError($"[出库任务状态变化通知]E0002-任务更新失败,StockOutId={stockOutId}, StockOutNo={stockOut.StockOutNo}");
                    return YL.Core.Dto.RouteData.From(PubMessages.E0002_UPDATE_COUNT_FAIL);
                }

                bool anyWorking = await _client.Queryable<Wms_stockout>()
                    .AnyAsync(x => x.MesTaskId == mesTask.MesTaskId 
                    && x.StockOutStatus != (int)StockOutStatus.task_finish 
                    && x.StockOutStatus != (int)StockOutStatus.task_canceled);

                if (!anyWorking)
                {
                    _logger.LogInformation($"[出库任务状态变化通知]所有相关任务已完成,尝试通知MES MesTaskId={stockOut.MesTaskId}");
                    await _client.NofityStockOut(mesTask);
                }
                else
                {
                    _logger.LogInformation($"[出库任务状态变化通知]尚有未完成任务,等待其余任务完成通知 MesTaskId={stockOut.MesTaskId}");
                }

                _logger.LogInformation($"[出库任务状态变化通知]通知处理正常完成,StockOutId={stockOutId}");
                _client.CommitTran();
                return new RouteData();
            }
            catch (Exception ex)
            {
                _client.RollbackTran();
                _logger.LogError($"[出库任务状态变化通知]E-1-发生异常,处理结束 ex={ex.ToString()}");
                return new RouteData() { Code = -1, Message = ex.Message };
            }
        }


        /// <summary>
        /// 盘库任务完成通知
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        [OutsideLog]
        [HttpPost("StockCount")]
        public async Task<RouteData> StockCountComplete([FromBody]OutsideStockCountReportDto report)
        {
            try
            {
                _logger.LogInformation($"[盘库任务完成通知]收到通知,report={JsonConvert.SerializeObject(report)}");
                DateTime completeDate = Convert.ToDateTime(report.CompleteDate);
                OutsideStockCountMaterialDto_MES[] materials = report.MaterialList.Select(x => {
                    return new OutsideStockCountMaterialDto_MES()
                    {
                        SuppliesId = x.MaterialNo,
                        SuppliesOnlyId = x.MaterialOnlyId,
                        SuppliesName = x.MaterialName,
                        SuppliesType = x.MaterialType,
                        PrevNumber = x.PrevNumber.ToString(),
                        InventoryNumber = x.StockCount.ToString(),
                        StoreId = x.InventoryBoxNo,
                        StoreName = x.InventoryBoxName,
                        Unit = x.Unit,
                        StoreMan = x.StockCountUser,
                        StockCountDate = x.StockCountDate.Value.ToString("yyyy/MM/dd HH:mm:ss"),

                    };
                }).ToArray();
                _logger.LogInformation($"[盘库任务完成通知]尝试通知MES");
                MESService.StockInventoryFinishRequest request = new MESService.StockInventoryFinishRequest()
                {
                    arg0 = report.StockCountNo,
                    arg1 = completeDate.Year.ToString(),
                    arg2 = completeDate.Month.ToString(),
                    arg3 = report.WarehouseId,
                    arg4 = JsonConvert.SerializeObject(materials)
                };
                _logger.LogInformation($"[盘库任务完成通知]通知MES成功");
                RouteData result = await MESApiAccessor.Instance.StockCount(request); 
                _logger.LogInformation($"[盘库任务完成通知]处理完成,request={JsonConvert.SerializeObject(request)}, result={JsonConvert.SerializeObject(result)}");

                return result;
            }
            catch(Exception ex)
            {
                _logger.LogInformation($"[盘库任务完成通知]处理失败,ex={ex.ToString()}");
                return new RouteData() { Code = -1, Message = ex.Message };
            }
        }
    }


}
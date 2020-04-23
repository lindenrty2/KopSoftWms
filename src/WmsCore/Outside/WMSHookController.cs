using IServices;
using Microsoft.AspNetCore.Mvc;
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
        public WMSHookController(
            SqlSugarClient client,
            IWms_stockinServices stockinServices,
            IWms_stockindetailServices stockindetailServices,
            IWms_stockindetailboxServices stockindetailboxServices,
            IWms_stockoutServices stockoutServices,
            IWms_inventoryBoxServices inventoryBoxServices,
            IWms_inventoryBoxTaskServices inventoryBoxTaskServices)
        {
            _client = client;
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
        public async Task<RouteData> StockInReport(int stockInId,[FromBody]OutsideStockInReportDto result)
        {
            try
            {
                Wms_stockin stockIn = await _client.Queryable<Wms_stockin>()
                     .FirstAsync(x => x.StockInId == result.StockInId);
                if (stockIn == null)
                {
                    return YL.Core.Dto.RouteData.From(PubMessages.E2013_STOCKIN_NOTFOUND);
                }
                if(stockIn.StockInStatus == StockInStatus.task_finish.ToInt32())
                {
                    return YL.Core.Dto.RouteData.From(PubMessages.E2014_STOCKIN_ALLOW_FINISHED);
                }

                Wms_stockindetail[] details = _client.Queryable<Wms_stockindetail>()
                    .Where(x => x.StockInId == result.StockInId).ToArray();

                _client.BeginTran();
                foreach (OutsideStockInReportDetail detail in result.Details)
                {
                    Wms_stockindetail localDetail = details.FirstOrDefault( x => x.MaterialId == detail.MaterialId);
                    localDetail.PlanInQty = detail.PlanInQty;
                    localDetail.ActInQty = detail.ActInQty;
                    localDetail.Status = detail.Status.ToInt32();
                    localDetail.ModifiedBy = PubConst.InterfaceUserId;
                    localDetail.ModifiedUser = PubConst.InterfaceUserName;
                    localDetail.Remark = detail.Remark;
                }

                if(_client.Updateable(details).ExecuteCommand() == 0)
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
                Wms_mestask mesTask = await _client.Queryable<Wms_mestask>()
                    .FirstAsync(x => x.MesTaskId == stockIn.MesTaskId);
                if(mesTask == null)
                {
                    return YL.Core.Dto.RouteData.From(PubMessages.E3000_MES_STOCKINTASK_NOTFOUND); 
                }

                Wms_stockin[] stockins = _client.Queryable<Wms_stockin>()
                 .Where(x => x.MesTaskId == mesTask.MesTaskId).ToArray();

                if(!stockins.Any(x => x.StockInStatus != StockInStatus.task_finish.ToInt32() && x.StockInStatus != StockInStatus.task_canceled.ToInt32()))
                {
                    mesTask.ModifiedDate = DateTime.Now;
                    mesTask.WorkStatus = MESTaskWorkStatus.WorkComplated;
                    mesTask.NotifyStatus = MESTaskNotifyStatus.WaitResponse;
                }

                bool notifyComplate = false;
                try
                {
                    List<OutsideMaterialResult> materialList = new List<OutsideMaterialResult>();
              

                    OutsideStockInResponse response = new OutsideStockInResponse()
                    {
                        WarehousingId = mesTask.WarehousingId,
                        WarehousingFinishTime = mesTask.ModifiedDate.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                        SuppliesInfoList = JsonConvert.SerializeObject(materialList)
                    };

                    await MESApiAccessor.Instance.WarehousingFinish(response);
                    notifyComplate = true;
                }
                catch(Exception)
                {

                }
                if (notifyComplate)
                { 
                    mesTask.NotifyStatus = MESTaskNotifyStatus.Responsed;
                    _client.Updateable(mesTask).ExecuteCommand();
                }
                _client.CommitTran();
                return new RouteData();
            }
            catch(Exception ex)
            {
                _client.RollbackTran();
                return new RouteData() { Code = -1, Message = ex.Message };
            }
        }

        /// <summary>
        /// 出库任务状态变化通知
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        [HttpPost("/StockOut/{stockOutId}/Status")]
        public async Task<RouteData> StockOutReport(int stockOutId, [FromBody]OutsideStockOutReportDto result)
        {
            try
            {
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

                Wms_stockoutdetail[] details = _client.Queryable<Wms_stockoutdetail>()
                    .Where(x => x.StockOutId == result.StockOutId).ToArray();

                _client.BeginTran();
                foreach (OutsideStockOutReportDetail detail in result.Details)
                {
                    Wms_stockoutdetail localDetail = details.FirstOrDefault(x => x.MaterialId == detail.MaterialId);
                    localDetail.PlanOutQty = detail.PlanOutQty;
                    localDetail.ActOutQty = detail.ActOutQty;
                    localDetail.Status = detail.Status.ToInt32();
                    localDetail.ModifiedBy = PubConst.InterfaceUserId;
                    localDetail.ModifiedUser = PubConst.InterfaceUserName;
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

                _client.CommitTran();
                return new RouteData();
            }
            catch (Exception ex)
            {
                _client.RollbackTran();
                return new RouteData() { Code = -1, Message = ex.Message };
            }
        }
    } 
}
using IServices;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
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
        public Task<RouteData> StockInReport(int stockInId,[FromBody]OutsideStockInReportDto result)
        {
            try
            {
                _client.BeginTran();
                _client.CommitTran();
                return Task.FromResult(new RouteData());
            }
            catch(Exception ex)
            {
                _client.RollbackTran();
                return Task.FromResult(new RouteData() {
                    Code = -1,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// 出库任务状态变化通知
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        [HttpPost("/StockOut/{stockOutId}/Status")]
        public Task<RouteData> StockOutReport(int stockOutId, [FromBody]OutsideStockOutReportDto result)
        {
            try
            {
                _client.BeginTran();
                _client.CommitTran();
                return Task.FromResult(new RouteData());
            }
            catch (Exception ex)
            {
                _client.RollbackTran();
                return Task.FromResult(new RouteData()
                {
                    Code = -1,
                    Message = ex.Message
                });
            }
        }
    } 
}
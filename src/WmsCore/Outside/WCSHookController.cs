using IServices;
using IServices.Outside;
using Microsoft.AspNetCore.Mvc;
using Services.Outside;
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
    [Route("/hook/wcs")]
    public class WCSHookController : Controller
    {

        private readonly IWms_stockinServices _stockinServices;
        private readonly IWms_stockindetailServices _stockindetailServices;
        private readonly IWms_stockindetailboxServices _stockindetailboxServices;
        private readonly IWms_inventoryBoxServices _inventoryBoxServices;
        private readonly IWms_inventoryBoxTaskServices _inventoryBoxTaskServices;
        private readonly SqlSugarClient _client;
        private readonly SysUserDto UserDto = new SysUserDto() {
            UserId = PubConst.InterfaceUserId,
            UserName = PubConst.InterfaceUserName,
            UserNickname = PubConst.InterfaceUserName
        };

        public WCSHookController(
            SqlSugarClient client,
            IWms_stockinServices stockinServices,
            IWms_stockindetailServices stockindetailServices,
            IWms_stockindetailboxServices stockindetailboxServices,
            IWms_inventoryBoxServices inventoryBoxServices,
            IWms_inventoryBoxTaskServices inventoryBoxTaskServices)
        {
            _client = client;
            _stockinServices = stockinServices;
            _stockindetailServices = stockindetailServices;
            _stockindetailboxServices = stockindetailboxServices;
            _inventoryBoxServices = inventoryBoxServices;
            _inventoryBoxTaskServices = inventoryBoxTaskServices;
        }

        [HttpGet("ping")]
        public string Ping(string s)
        {
            return "OK";
        }


        [HttpPost("getLocation")]
        public Task<GetLocationResult> GetLocation([FromBody]GetLocationArg result)
        {
            return Task.FromResult(new GetLocationResult());
        }


        /// <summary>
        /// 确认出库完成
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        [HttpPost("ConfirmStockOut")]
        public async Task<ConfirmOutStockResult> ConfirmStockOut([FromBody]WCSStockTaskCallBack result)
        {
            SelfWMSOperationApiAccessor accessor = new SelfWMSOperationApiAccessor(null, _client,this.UserDto);
            ConfirmOutStockResult apiResult = await accessor.ConfirmOutStock(result);
            return apiResult;
        }

        /// <summary>
        /// 确认归库完成
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        [HttpPost("ConfirmStockIn")]
        public async Task<ConfirmBackStockResult> ConfirmStockIn([FromBody]WCSStockTaskCallBack result)
        {
            SelfWMSOperationApiAccessor accessor = new SelfWMSOperationApiAccessor(null, _client, this.UserDto);
            ConfirmBackStockResult apiResult = await accessor.ConfirmBackStock(result);
            return apiResult;
        }

        [HttpPost("LogisticsFinish")]
        public async Task<OutsideLogisticsFinishResponseResult> LogisticsFinish([FromBody]OutsideLogisticsFinishResponse arg)
        {
            return await MESApiAccessor.Instance.LogisticsFinish(arg);
        }

    } 
}
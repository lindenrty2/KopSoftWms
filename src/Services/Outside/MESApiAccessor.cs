using IServices.Outside;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using WebApiClient;
using YL.Core.Dto;

namespace WMSCore.Outside
{
    public class MESApiAccessor
    {
        public static string Host { get; set; }
        public static MESApiAccessor _instance;
        public static MESApiAccessor Instance {
            get
            {
                if(_instance == null)
                {
                    _instance = new MESApiAccessor();
                }
                return _instance;
            }
        }

        private MESService.MyMethodImpl _apiProxy = null;
        private MESApiAccessor()
        {
            //HttpApiConfig config = new HttpApiConfig();
            //config.HttpHost = new Uri(Host); 
            //_apiProxy = HttpApi.Create<IMESApiProxy>(config);

            var binding = new BasicHttpBinding();
            binding.SendTimeout = new TimeSpan(1, 0, 0);
            binding.ReceiveTimeout = new TimeSpan(1, 0, 0);
            var factory = new ChannelFactory<MESService.MyMethodImpl>(binding, new EndpointAddress(Host));
            _apiProxy = factory.CreateChannel();
             
        }

        /// <summary>
        /// 入库完成
        /// </summary>
        /// <param name="inStockInfo"></param>
        /// <returns></returns>
        public async Task<OutsideStockInResponseResult> WarehousingFinish(OutsideStockInResponse inStockInfo)
        {
            MESService.WarehousingFinishResponse1 reuslt = await _apiProxy.WarehousingFinishAsync(
                new MESService.WarehousingFinishRequest() {
                    arg0 = inStockInfo.WarehousingId,
                    arg1 = inStockInfo.WarehousingEntryNumber.ToString(),
                    arg2 = inStockInfo.WarehousingEntryFinishList
                }
            );
            return JsonConvert.DeserializeObject<OutsideStockInResponseResult>(reuslt.@return);
        }

        /// <summary>
        /// 出库完成
        /// </summary>
        /// <param name="outStockInfo"></param>
        /// <returns></returns>
        public async Task<OutsideStockOutResponseResult> WarehouseEntryFinish(OutsideStockOutResponse outStockInfo)
        {
            MESService.WarehousingEntryFinishResponse result = await _apiProxy.WarehousingEntryFinishAsync(
                new MESService.WarehousingEntryFinishRequest()
                {
                    arg0 = outStockInfo.WarehouseEntryId,
                    arg1 = outStockInfo.WarehouseEntryFinishCount.ToString(),
                    arg2 = outStockInfo.WarehouseEntryFinishList
                });
            return JsonConvert.DeserializeObject<OutsideStockOutResponseResult>(result.@return);
        }

        /// <summary>
        /// 物流控制完成
        /// </summary>
        public async Task<OutsideLogisticsFinishResponseResult> LogisticsFinish(OutsideLogisticsFinishResponse arg)
        {
            MESService.LogisticsFinishResponse result = await _apiProxy.LogisticsFinishAsync(
                new MESService.LogisticsFinishRequest()
                {
                    arg0 = arg.LogisticsId,
                    arg1 = arg.LogisticsFinishTime,
                    arg2 = arg.WorkAreaName,
                    arg3 = arg.ErrorId,
                    arg4 = arg.ErrorInfo
                }
            );
            return JsonConvert.DeserializeObject<OutsideLogisticsFinishResponseResult>(result.@return);
        }

        /// <summary>
        /// 盘库完成
        /// </summary>
        public async Task<RouteData> StockCount(OutsideStockCountReportDto report)
        {
            //MESService.LogisticsFinishResponse result = await _apiProxy.StockCount(
            //    new MESService.OutsideStockCountReportDto()
            //    {
            //        arg0 = arg.LogisticsId,
            //        arg1 = arg.LogisticsFinishTime,
            //        arg2 = arg.WorkAreaName,
            //        arg3 = arg.ErrorId,
            //        arg4 = arg.ErrorInfo
            //    }
            //);
            //return JsonConvert.DeserializeObject<OutsideLogisticsFinishResponseResult>(result.@return);
            return new RouteData();
        }
    }
}

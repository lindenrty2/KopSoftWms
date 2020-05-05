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

        private MESService.IService _apiProxy = null;
        private MESApiAccessor()
        {
            //HttpApiConfig config = new HttpApiConfig();
            //config.HttpHost = new Uri(Host); 
            //_apiProxy = HttpApi.Create<IMESApiProxy>(config);

            var binding = new BasicHttpBinding();
            binding.SendTimeout = new TimeSpan(1, 0, 0);
            binding.ReceiveTimeout = new TimeSpan(1, 0, 0);
            var factory = new ChannelFactory<MESService.IService>(binding, new EndpointAddress(Host));
            _apiProxy = factory.CreateChannel();
             
        }

        /// <summary>
        /// 入库完成
        /// </summary>
        /// <param name="inStockInfo"></param>
        /// <returns></returns>
        public async Task<OutsideStockInResponseResult> WarehousingFinish(OutsideStockInResponse inStockInfo)
        {
            string reuslt = await _apiProxy.WarehousingFinishAsync(inStockInfo.WarehousingId, inStockInfo.WarehousingEntryNumber, inStockInfo.WarehousingEntryFinishList);
            return JsonConvert.DeserializeObject<OutsideStockInResponseResult>(reuslt);
        }

        /// <summary>
        /// 出库完成
        /// </summary>
        /// <param name="outStockInfo"></param>
        /// <returns></returns>
        public async Task<OutsideStockOutResponseResult> WarehouseEntryFinish(OutsideStockOutResponse outStockInfo)
        {
            string result = await _apiProxy.WarehouseEntryFinishAsync(outStockInfo.WarehouseEntryId, outStockInfo.WarehouseEntryFinishCount, outStockInfo.WarehouseEntryFinishList);
            return JsonConvert.DeserializeObject<OutsideStockOutResponseResult>(result);
        }

        /// <summary>
        /// 物流控制完成
        /// </summary>
        public async Task<OutsideLogisticsFinishResponseResult> LogisticsFinish(OutsideLogisticsFinishResponse arg)
        {
            string result = await _apiProxy.LogisticsFinishAsync(arg.LogisticsId, arg.LogisticsFinishTime, arg.WorkAreaName, arg.ErrorId, arg.ErrorInfo);
            return JsonConvert.DeserializeObject<OutsideLogisticsFinishResponseResult>(result);
        }
         
    }
}

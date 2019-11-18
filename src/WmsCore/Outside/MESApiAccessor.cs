using IServices.Outside;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private IMESApiProxy _apiProxy = null;
        private MESApiAccessor()
        {
            HttpApiConfig config = new HttpApiConfig();
            config.HttpHost = new Uri(Host);
            _apiProxy = HttpApi.Create<IMESApiProxy>(config);
        }

        public async Task<OutsideStockInResponseResult> ConfirmBalanceMES(OutsideStockInResponse inStockInfo)
        {
            return await _apiProxy.ConfirmBalanceMES(inStockInfo);
        }

        public async Task<OutsideStockOutResponseResult> ConfirmOutStockMES(OutsideStockOutResponse outStockInfo)
        {
            return await _apiProxy.ConfirmOutStockMES(outStockInfo);
        }

        /// <summary>
        /// 物流控制完成
        /// </summary>
        public async Task<OutsideLogisticsFinishResponseResult> LogisticsFinish(OutsideLogisticsFinishResponse arg)
        {
            return await _apiProxy.LogisticsFinish(arg);
        }
         
    }
}

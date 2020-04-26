using InterfaceMocker.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiClient;
using IServices.Outside;
using YL.Core.Dto;

namespace InterfaceMocker.WindowUI.WebApi
{
    public class WMSApiAccessor
    {
        public static WMSApiAccessor _instance;
        public static WMSApiAccessor Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new WMSApiAccessor();
                }
                return _instance;
            }
        }

        private IWMSApiProxy _apiProxy = null;
        private WMSApiAccessor()
        {
            HttpApiConfig config = new HttpApiConfig();
            config.HttpHost = new Uri("http://localhost:23456/hook/wcs/"); 
            _apiProxy = HttpApi.Create<IWMSApiProxy>(config);
        }

        public async Task<ConfirmOutStockResult> ConfirmOutStock(WCSTaskResult result)
        {
            return await _apiProxy.ConfirmOutStock(result);
        }

        public async Task<ConfirmBackStockResult> ConfirmBackStock(WCSTaskResult result)
        {
            return await _apiProxy.ConfirmBackStock(result);
        }

        public async Task<OutsideLogisticsFinishResponseResult> LogisticsFinish(OutsideLogisticsFinishResponse result)
        {
            if (WCSController._logistics.ContainsKey(result.LogisticsId))
            {
                WCSController._logistics[result.LogisticsId].Step = -1;
            }
            return await _apiProxy.LogisticsFinish(result);
        }
    }
}

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
            config.HttpHost = new Uri("http://localhost:16001/MES/");
            _apiProxy = HttpApi.Create<IMESApiProxy>(config);
        }

        public async Task<OutSideStockInResponseResult> ConfirmBalanceMES(OutSideStockInResponse inStockInfo)
        {
            return await _apiProxy.ConfirmBalanceMES(inStockInfo);
        }

        public async Task<OutSideStockOutResponseResult> ConfirmOutStockMES(OutSideStockOutResponse outStockInfo)
        {
            return await _apiProxy.ConfirmOutStockMES(outStockInfo);
        }
    }
}

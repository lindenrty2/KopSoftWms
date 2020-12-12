using IServices.Outside;
using System;
using System.Threading.Tasks;
using WebApiClient;
using WebApiClient.Attributes;
using YL.Core.Dto;

namespace WMSCore.Outside
{
    public class WCSApiAccessor
    {
        public static string Host { get; set; }

        public static WCSApiAccessor _instance;
        public static WCSApiAccessor Instance {
            get
            {
                if(_instance == null)
                {
                    _instance = new WCSApiAccessor();
                }
                return _instance;
            }
        }

        private IWCSApiProxy _apiProxy = null;
        private WCSApiAccessor()
        {
            if (!string.IsNullOrEmpty(Host))
            {
                HttpApiConfig config = new HttpApiConfig();
                config.HttpHost = new Uri(Host);
                _apiProxy = HttpApi.Create<IWCSApiProxy>(config); 
            }
        }

        /// <summary>
        /// 创建WCS出库任务
        /// </summary>
        /// <param name="stockOutTask"></param>
        /// <returns></returns>
        public async Task<CreateOutStockResult> CreateStockOutTask([JsonContent]StockOutTaskInfo stockOutTask)
        {
            if(_apiProxy == null)
            {
                return new CreateOutStockResult() { Successd = true };
            }
            CreateOutStockResult result =  await _apiProxy.CreateStockOut(stockOutTask);
            return result;
        }

        /// <summary>
        /// 创建WCS入库任务
        /// </summary>
        /// <param name="stockInTask"></param>
        /// <returns></returns>
        public async Task<StockInTaskResult> CreateStockInTask([JsonContent]StockInTaskInfo stockInTask)
        {
            if (_apiProxy == null)
            {
                return new StockInTaskResult() { Successd = true };
            }
            StockInTaskResult result = await _apiProxy.CreateStockIn(stockInTask);
            return result;
        }

        public async Task<OutsideLogisticsEnquiryResult> LogisticsEnquiry([JsonContent]OutsideLogisticsEnquiryArg arg)
        {
            try
            {
                return await _apiProxy.LogisticsEnquiry(arg);
            }
            catch (Exception ex) {
                return new OutsideLogisticsEnquiryResult()
                {
                    Status = ex.Message
                };
            }
        }

        public async Task<OutsideLogisticsControlResult> LogisticsControl([JsonContent]OutsideLogisticsControlArg arg)
        {
            try
            {
                return await _apiProxy.LogisticsControl(arg);
            }
            catch (Exception ex)
            {
                return new OutsideLogisticsControlResult()
                {
                    ErrorId = "-1",
                    ErrorInfo = ex.Message
                };
            }
        }
    }
}

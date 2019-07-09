﻿using System;
using System.Collections.Generic;
using System.Linq;
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
            HttpApiConfig config = new HttpApiConfig();
            config.HttpHost = new Uri(Host);
            _apiProxy = HttpApi.Create<IWCSApiProxy>(config);
        }

        public async Task<CreateOutStockResult> CreateOutStockTask([JsonContent]OutStockInfo outStockInfo)
        {
            return await _apiProxy.CreateOutStockTask(outStockInfo);
        }

        public async Task<CreateBackStockResult> CreateBackStockTask([JsonContent]BackStockInfo backStockInfo)
        {
            return await _apiProxy.CreateBackStockTask(backStockInfo);
        }

        public async Task<OutsideLogisticsEnquiryResult> LogisticsEnquiry([JsonContent]OutsideLogisticsEnquiryArg arg)
        {
            return await _apiProxy.LogisticsEnquiry(arg);
        }

        public async Task<OutsideLogisticsControlResult> LogisticsControl([JsonContent]OutsideLogisticsControlArg arg)
        {
            return await _apiProxy.LogisticsControl(arg);
        }
    }
}

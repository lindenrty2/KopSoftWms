using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiClient;
using WebApiClient.Attributes;
using YL.Core.Dto;

namespace WMSCore.Outside
{
    public interface IWCSApiProxy : IHttpApi
    {

        /// <summary>
        /// 出库指令
        /// </summary>
        /// <param name="outStockInfo"></param>
        /// <returns></returns>
        [HttpPost("createOutStock")]
        ITask<CreateOutStockResult> CreateOutStockTask([JsonContent] OutStockInfo outStockInfo);

        /// <summary>
        /// 归库指令
        /// </summary>
        /// <param name="backStockInfo"></param>
        /// <returns></returns>
        [HttpPost("createBackStock")]
        ITask<CreateBackStockResult> CreateBackStockTask([JsonContent] BackStockInfo backStockInfo);

        /// <summary>
        /// 物流控制
        /// </summary>
        [HttpPost("logisticsControlWCS")]
        ITask<OutsideLogisticsControlResult> LogisticsControl([JsonContent] OutsideLogisticsControlArg arg);

        /// <summary>
        /// 物流（状态）查询
        /// </summary>
        [HttpPost("logisticsEnquiryWCS")]
        ITask<OutsideLogisticsEnquiryResult> LogisticsEnquiry([JsonContent] OutsideLogisticsEnquiryArg arg);
         
    }
}

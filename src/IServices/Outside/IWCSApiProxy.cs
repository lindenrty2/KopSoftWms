using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiClient;
using WebApiClient.Attributes;
using YL.Core.Dto;

namespace IServices.Outside
{
    public interface IWCSApiProxy : IHttpApi
    {

        /// <summary>
        /// 出库指令
        /// </summary>
        /// <param name="outStockInfo"></param>
        /// <returns></returns>
        [HttpPost("CreateStockOut.ashx")]
        ITask<CreateOutStockResult> CreateStockOut([JsonContent] StockOutTaskInfo outStockInfo);

        /// <summary>
        /// 入库(归库)指令
        /// </summary>
        /// <param name="backStockInfo"></param>
        /// <returns></returns>
        [HttpPost("CreateStockIn.ashx")]
        ITask<StockInTaskResult> CreateStockIn([JsonContent] StockInTaskInfo backStockInfo);

        /// <summary>
        /// 物流控制
        /// </summary>
        [HttpPost("logisticsControl.ashx")]
        ITask<OutsideLogisticsControlResult> LogisticsControl([JsonContent] OutsideLogisticsControlArg arg);

        /// <summary>
        /// 物流（状态）查询
        /// </summary>
        [HttpPost("logisticsEnquiry.ashx")]
        ITask<OutsideLogisticsEnquiryResult> LogisticsEnquiry([JsonContent] OutsideLogisticsEnquiryArg arg);
         
    }
}

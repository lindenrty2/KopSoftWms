using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiClient;
using WebApiClient.Attributes;
using YL.Core.Dto;

namespace WMSCore.Outside
{
    public interface IMESApiProxy : IHttpApi
    {

        [HttpPost("ConfirmBalanceMES")]
        ITask<OutsideStockInResponseResult> ConfirmBalanceMES(OutsideStockInResponse inStockInfo);

        [HttpPost("ConfirmOutStockMES")]
        ITask<OutsideStockOutResponseResult> ConfirmOutStockMES(OutsideStockOutResponse outStockInfo);

        [HttpPost("LogisticsFinish")]
        ITask<OutsideLogisticsFinishResponseResult> LogisticsFinish(OutsideLogisticsFinishResponse arg);
    }
}

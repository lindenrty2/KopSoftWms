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
        ITask<OutSideStockInResponseResult> ConfirmBalanceMES(OutSideStockInResponse inStockInfo);

        [HttpPost("ConfirmOutStockMES")]
        ITask<OutSideStockOutResponseResult> ConfirmOutStockMES(OutSideStockOutResponse outStockInfo);

    }
}

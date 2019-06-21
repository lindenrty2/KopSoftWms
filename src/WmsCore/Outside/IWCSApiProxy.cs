using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiClient;
using WebApiClient.Attributes;

namespace WMSCore.Outside
{
    public interface IWCSApiProxy : IHttpApi
    {

        [HttpPost("createOutStock")]
        ITask<CreateOutStockResult> CreateOutStockTask(OutStockInfo outStockInfo);

        [HttpPost("createBackStock")]
        ITask<CreateBackStockResult> CreateBackStockTask(BackStockInfo backStockInfo);

    }
}

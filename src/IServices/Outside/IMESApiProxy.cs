using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiClient;
using WebApiClient.Attributes;
using YL.Core.Dto;

namespace IServices.Outside
{
    public interface IMESApiProxy : IHttpApi
    {

        [HttpPost("WarehousingFinish")]
        ITask<OutsideStockInResponseResult> WarehousingFinish(OutsideStockInResponse inStockInfo);

        [HttpPost("WarehouseEntryFinish")]
        ITask<OutsideStockOutResponseResult> WarehouseEntryFinish(OutsideStockOutResponse outStockInfo);

        [HttpPost("LogisticsFinish")]
        ITask<OutsideLogisticsFinishResponseResult> LogisticsFinish(OutsideLogisticsFinishResponse arg);
    }
}

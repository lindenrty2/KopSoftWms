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
        ITask<string> WarehousingFinish(string warehousingId, int warehousingEntryNumber, string warehousingEntryFinishList); 

        [HttpPost("WarehouseEntryFinish")]
        ITask<string> WarehouseEntryFinish(string warehouseEntryId,int warehouseEntryFinishCount,string warehouseEntryFinishList);
 
        [HttpPost("LogisticsFinish")]
        ITask<string> LogisticsFinish(string LogisticsId, string LogisticsFinishTime, string WorkAreaName, string ErrorId,string ErrorInfo);

        [HttpPost("StockInventoryFinish")]
        ITask<string> StockInventoryFinish(string StockInventoryId, string Year, string Month,string WarehouseID,string SuppliesInfoList);

    }
}

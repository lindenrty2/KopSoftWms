using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using YL.Core.Dto;
using YL.Core.Entity;

namespace IServices.Outside
{
    /// <summary>
    /// WMS管理API
    /// </summary>
    public interface IWMSManagementApiAccessor
    {
        Task<RouteData<Wms_inventorybox>> AddInventoryBox(Wms_inventorybox box);
        Task<RouteData<Wms_inventorybox>> UpdateInventoryBox(long id,Wms_inventorybox box);
        Task<RouteData> DeleteInventoryBox(long inventoryBoxId);

        Task<RouteData<Sys_dict[]>> GetMaterialTypes(string search);
        Task<RouteData<Wms_StockCountInventoryBoxDto[]>> GetStockCountInventoryBoxList(long? materialTypeId, string materialName);
    }
}

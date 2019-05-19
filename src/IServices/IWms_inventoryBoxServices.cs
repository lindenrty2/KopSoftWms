using YL.Core.Dto;
using YL.Core.Entity;

namespace IServices
{
    public interface IWms_inventoryBoxServices : IBaseServices<wms_inventorybox>
    {
        string PageList(PubParams.InventoryBoxBootstrapParams bootstrap);
        

    }
}
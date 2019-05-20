using YL.Core.Dto;
using YL.Core.Entity;

namespace IServices
{
    public interface IWms_inventoryBoxServices : IBaseServices<Wms_inventorybox>
    {
        string PageList(PubParams.InventoryBoxBootstrapParams bootstrap);
        

    }
}
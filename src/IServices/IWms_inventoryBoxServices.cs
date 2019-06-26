using System.Threading.Tasks;
using YL.Core.Dto;
using YL.Core.Entity;

namespace IServices
{
    public interface IWms_inventoryBoxServices : IBaseServices<Wms_inventorybox>
    {
        string PageList(PubParams.InventoryBoxBootstrapParams bootstrap);
        /// <summary>
        /// 自动选择料箱
        /// </summary>
        /// <param name="isFullBox">是否是完整料箱</param>
        /// <returns></returns>
        Task<Wms_inventorybox> AutoSelectBox(bool isFullBox);

    }
     
}
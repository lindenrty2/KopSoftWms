using System.Threading.Tasks;
using YL.Core.Dto;
using YL.Core.Entity;

namespace IServices
{
    public interface IWms_inventoryBoxServices : IBaseServices<Wms_inventorybox>
    {
        string PageList(PubParams.InventoryBoxBootstrapParams bootstrap);
        /// <summary>
        /// �Զ�ѡ������
        /// </summary>
        /// <param name="isFullBox">�Ƿ�����������</param>
        /// <returns></returns>
        Task<Wms_inventorybox> AutoSelectBox(bool isFullBox);

    }
     
}
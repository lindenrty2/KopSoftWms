using SqlSugar;
using System.Threading.Tasks;
using YL.Core.Dto;
using YL.Core.Entity;

namespace IServices
{
    public interface IWms_stockoutServices : IBaseServices<Wms_stockout>
    {
        string PageList(PubParams.StockOutBootstrapParams bootstrap);

        string PrintList(string stockInId);

        DbResult<bool> Auditin(long userId, long stockOutId);

        Task<RouteData> DoScanComplate(long stockOutId, long inventoryBoxId, Wms_StockMaterialDetailDto[] materials, string remark, SysUserDto userDto);
    }
}
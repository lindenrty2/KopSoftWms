using System;
using System.Threading.Tasks;
using SqlSugar;
using YL.Core.Dto;
using YL.Core.Entity;

namespace IServices
{
    public interface IWms_stockinServices : IBaseServices<Wms_stockin>
    {
        //string PageList(PubParams.StockInBootstrapParams bootstrap);

        string PrintList(string stockInId,long? detailId);

        bool Auditin(long UserId, long stockInId);

        //Task<RouteData> DoScanComplate(long stockInId, long inventoryBoxId, Wms_StockMaterialDetailDto[] materials, string remark,SysUserDto userDto);
    }
}
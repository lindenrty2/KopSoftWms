using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using YL.Core.Dto;
using YL.Core.Entity;
using YL.Utils.Table;

namespace IServices
{
    public interface IWms_materialServices : IBaseServices<Wms_material>
    {
        string PageList(Bootstrap.BootstrapParams bootstrap);

        byte[] ExportList(Bootstrap.BootstrapParams bootstrap);

        Task<RouteData> ImportList(IFormFile stream);
    }
}
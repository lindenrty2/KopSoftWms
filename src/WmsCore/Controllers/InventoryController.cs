using IServices.Outside;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Threading.Tasks;
using WMSCore.Outside;
using YL.Core.Dto;
using YL.NetCore.Attributes;
using YL.NetCore.NetCoreApp;
using YL.Utils.Json;
using YL.Utils.Pub;
using YL.Utils.Table;

namespace KopSoftWms.Controllers
{
    public class InventoryController : BaseController
    {
        private readonly SqlSugarClient _client;

        public InventoryController(
            SqlSugarClient client
            )
        {
            _client = client;
        }

        [HttpGet]
        [CheckMenu]
        public IActionResult Index()
        {
            //ViewBag.StorageRack = _storagerackServices.QueryableToList(c => c.IsDel == 1);
            return View();
        }

        [HttpPost]
        [OperationLog(LogType.select)]
        public async Task<string> List([FromForm]PubParams.InventoryBootstrapParams bootstrap)
        {
            //var sd = _inventoryServices.PageList(bootstrap);
            //return Content(sd);

            IWMSApiAccessor wmsAccessor = WMSApiManager.Get(bootstrap.storeId.ToString(), _client);
            RouteData<OutsideInventoryDto[]> result = (await wmsAccessor.QueryInventory(null,null,null,null,bootstrap.pageIndex, bootstrap.limit, bootstrap.search, bootstrap.order.Split(","), bootstrap.datemin, bootstrap.datemax));
            if (!result.IsSccuess)
            {
                return new PageGridData().JilToJson();
            }
            return result.ToGridJson();
        }
    }
}
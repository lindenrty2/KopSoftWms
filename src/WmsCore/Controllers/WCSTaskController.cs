using IServices.Outside;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Threading.Tasks;
using WMSCore.Outside;
using YL.Core.Dto;
using YL.Core.Entity;
using YL.NetCore.Attributes;
using YL.NetCore.NetCoreApp;
using YL.Utils.Pub;

namespace WMSCore.Controllers
{
    public class WcsTaskController : BaseController
    {
        private readonly SqlSugarClient _client;
        public WcsTaskController(SqlSugarClient client)
        {
            _client = client;
        }


        [HttpGet]
        [CheckMenu]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [OperationLog(LogType.select)]
        public async Task<string> List([FromForm]PubParams.WcsTaskBootstrapParams bootstrap)
        {
            IWMSOperationApiAccessor accessor = WMSApiManager.GetOperationApiAccessor(bootstrap.storeId.ToString(), _client, this.UserDto);
            if (accessor == null)
            {
                return "";
            }
            RouteData<Wms_wcstask[]> result = await accessor.GetWCSTaskList(bootstrap.FailOnly, bootstrap.pageIndex, bootstrap.limit, bootstrap.search, new string[] { bootstrap.sort + " " + bootstrap.order }, bootstrap.datemin, bootstrap.datemax);
            if (!result.IsSccuess)
            {
                return "";
            }
            return result.ToGridJson();
        }


        //[HttpGet]
        //public async Task<IActionResult> Detail(string id)
        //{
        //    long searchId = id.ToInt64();
        //    if (id.IsEmptyZero())
        //    {
        //        return Content("");
        //    }
        //    else
        //    {
        //        Wms_mestask mesTask = await _client.Queryable<Wms_mestask>().FirstAsync(x => x.MesTaskId == SqlFunc.ToInt64(id)); 
        //        return View(mesTask);
        //    }
        //}


        [HttpPost]
        public async Task<RouteData> SetWCSTaskStatus(long storeId, long wcsTaskId, bool isSccuess)
        {
            IWMSOperationApiAccessor accessor = WMSApiManager.GetOperationApiAccessor(storeId.ToString(), _client, this.UserDto);
            if (accessor == null)
            {
                return  YL.Core.Dto.RouteData.From(PubMessages.E0007_WAREHOUSE_NOTFOUND);
            }
            RouteData result = await accessor.SetWCSTaskStatus(wcsTaskId, isSccuess ? "ok" : "500");
            if (!result.IsSccuess)
            { 
            }
            return result;

        }

        [HttpPost]
        public async Task<RouteData> RepeatWCSTaskStatus(long storeId, long wcsTaskId)
        {
            IWMSOperationApiAccessor accessor = WMSApiManager.GetOperationApiAccessor(storeId.ToString(), _client, this.UserDto);
            if (accessor == null)
            {
                return YL.Core.Dto.RouteData.From(PubMessages.E0007_WAREHOUSE_NOTFOUND);
            }
            RouteData result = await accessor.RepeatWCSTaskStatus(wcsTaskId);
            if (!result.IsSccuess)
            {
            }
            return result;

        }
    }
}
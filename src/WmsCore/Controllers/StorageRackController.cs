using IServices;
using IServices.Outside;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Linq;
using System.Threading.Tasks;
using WMSCore.Outside;
using YL.Core.Dto;
using YL.Core.Entity;
using YL.Core.Entity.Fluent.Validation;
using YL.NetCore.Attributes;
using YL.NetCore.NetCoreApp;
using YL.Utils.Extensions;
using YL.Utils.Json;
using YL.Utils.Pub;
using YL.Utils.Table;

namespace KopSoftWms.Controllers
{
    public class StorageRackController : BaseController
    {
        private readonly IWms_warehouseServices _warehouseServices;
        private readonly IWms_reservoirareaServices _reservoirareaServices;
        private readonly IWms_storagerackServices _storagerackServices;
        private readonly IWms_materialServices _materialServices;
        private readonly IWms_inventoryBoxServices _inventoryBoxServices;

        private readonly SqlSugarClient _client;

        public StorageRackController(
            SqlSugarClient client,
            IWms_warehouseServices warehouseServices,
            IWms_storagerackServices storagerackServices,
            IWms_reservoirareaServices reservoirareaServices,
            IWms_materialServices materialServices,
            IWms_inventoryBoxServices inventoryBoxServices
            )
        {
            _client = client;
            _warehouseServices = warehouseServices;
            _storagerackServices = storagerackServices;
            _reservoirareaServices = reservoirareaServices;
            _materialServices = materialServices;
            _inventoryBoxServices = inventoryBoxServices;
        }

        [HttpGet]
        [CheckMenu]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取仓库下的所有未删除的库区
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<string> GetReservoirarea(string id)
        {
            //var json = _reservoirareaServices.Queryable().Where(c => c.IsDel == 1 && c.WarehouseId == SqlFunc.ToInt64(id))
            //    .Select(c => new { ReservoirAreaId = c.ReservoirAreaId.ToString(), c.ReservoirAreaName })
            //    .ToList();
            //return Content(json.JilToJson());

            IWMSApiAccessor wmsAccessor = WMSApiManager.Get(id.ToString(), _client);
            RouteData<Wms_reservoirarea[]> result = (await wmsAccessor.GetReservoirAreaList(1, 100, null, null, null, null));
            if (!result.IsSccuess)
            {
                return new PageGridData().JilToJson();
            }
            return result.ToGridJson();
        }

        [HttpGet]
        public async Task<string> GetReservoirarea2(string id)
        {
            //var json = _reservoirareaServices.Queryable().Where(c => c.IsDel == 1 && c.WarehouseId == SqlFunc.ToInt64(id))
            //    .Select(c => new { value = c.ReservoirAreaId.ToString(), name = c.ReservoirAreaName })
            //    .ToList();
            //return Content(json.JilToJson());

            IWMSApiAccessor wmsAccessor = WMSApiManager.Get(id.ToString(), _client);
            RouteData<Wms_reservoirarea[]> result = (await wmsAccessor.GetReservoirAreaList(1, 100, null, null, null, null));
            if (!result.IsSccuess)
            {
                return new PageGridData().JilToJson();
            }
            return result.ToGridJson();
        }

        [HttpGet]
        public async Task<string> GetStoragerackAsync(string id)
        {
            //var json = _storagerackServices.Queryable().Where(c => c.IsDel == 1 && c.ReservoirAreaId == SqlFunc.ToInt64(id))
            //    .Select(c => new { value = c.StorageRackId.ToString(), name = c.StorageRackName })
            //    .ToList();
            //return Content(json.JilToJson());

            IWMSApiAccessor wmsAccessor = WMSApiManager.Get(id.ToString(), _client);
            RouteData<Wms_storagerack[]> result = (await wmsAccessor.GetStorageRackList(SqlFunc.ToInt64(id), 1, 100, null, null, null, null));
            if (!result.IsSccuess)
            {
                return new PageGridData().JilToJson();
            }
            return result.ToGridJson();
        }

        [HttpPost]
        [OperationLog(LogType.select)]
        public async Task<string> List([FromForm]Bootstrap.BootstrapParams bootstrap)
        {
            //var sd = _storagerackServices.PageList(bootstrap);
            //return Content(sd);

            IWMSApiAccessor wmsAccessor = WMSApiManager.Get(bootstrap.storeId.ToString(), _client);
            RouteData<Wms_storagerack[]> result = await wmsAccessor.GetStorageRackList(null, bootstrap.pageIndex, bootstrap.limit, bootstrap.search, bootstrap.order.Split(","), bootstrap.datemin, bootstrap.datemax);
            if (!result.IsSccuess)
            {
                return new PageGridData().JilToJson();
            }
            return result.ToGridJson();
        }

        [HttpGet]
        public IActionResult Add(string id)
        {
            var model = new Wms_storagerack();
            if (id.IsEmpty())
            {
                return View(model);
            }
            else
            {
                model = _storagerackServices.QueryableToEntity(c => c.StorageRackId == SqlFunc.ToInt64(id));
                return View(model);
            }
        }

        [HttpPost]
        [FilterXss]
        [OperationLog(LogType.addOrUpdate)]
        public IActionResult AddOrUpdate([FromForm]Wms_storagerack model, [FromForm]string id)
        {
            var validator = new StorageRackFluent();
            var results = validator.Validate(model);
            var success = results.IsValid;
            if (!success)
            {
                string msg = results.Errors.Aggregate("", (current, item) => (current + item.ErrorMessage + "</br>"));
                return BootJsonH((PubEnum.Failed.ToInt32(), msg));
            }
            if (id.IsEmptyZero())
            {
                if (_storagerackServices.IsAny(c => c.StorageRackNo == model.StorageRackNo || c.StorageRackName == model.StorageRackNo))
                {
                    return BootJsonH((false, PubConst.Warehouse5));
                }
                model.StorageRackId = PubId.SnowflakeId;
                model.CreateBy = UserDtoCache.UserId;
                bool flag = _storagerackServices.Insert(model);
                return BootJsonH(flag ? (flag, PubConst.Add1) : (flag, PubConst.Add2));
            }
            else
            {
                model.StorageRackId = id.ToInt64();
                model.ModifiedBy = UserDtoCache.UserId;
                model.ModifiedDate = DateTimeExt.DateTime;
                var flag = _storagerackServices.Update(model);
                return BootJsonH(flag ? (flag, PubConst.Update1) : (flag, PubConst.Update2));
            }
        }

        [HttpGet]
        [OperationLog(LogType.delete)]
        public IActionResult Delete(long id)
        {
            //判断有没有料箱在该货架上
            var isExist = _inventoryBoxServices.IsAny(x => x.StorageRackId == id);
            if (isExist)
            {
                return BootJsonH((false, PubConst.Warehouse6));
            }
            else
            {
                var flag = _storagerackServices.Update(new Wms_storagerack { StorageRackId = SqlFunc.ToInt64(id), IsDel = 0, ModifiedBy = UserDtoCache.UserId, ModifiedDate = DateTimeExt.DateTime }, c => new { c.IsDel, c.ModifiedBy, c.ModifiedDate });
                return BootJsonH(flag ? (flag, PubConst.Delete1) : (flag, PubConst.Delete2));
            }
        }

        [HttpGet]
        public async Task<string> Search(string text)
        {
            //var bootstrap = new Bootstrap.BootstrapParams
            //{
            //    limit = 100,
            //    offset = 0,
            //    sort = "CreateDate",
            //    search = text,
            //    order = "desc"
            //};
            //var json = _storagerackServices.PageList(bootstrap);
            //return Content(json);

            var bootstrap = new Bootstrap.BootstrapParams
            {
                limit = 100,
                offset = 1,
                sort = "CreateDate",
                search = text,
                order = "desc"
            };
            IWMSApiAccessor wmsAccessor = WMSApiManager.Get(bootstrap.storeId.ToString(), _client);
            RouteData<Wms_storagerack[]> result = await wmsAccessor.GetStorageRackList(null, bootstrap.offset, bootstrap.limit, bootstrap.search, bootstrap.order.Split(","), bootstrap.datemin, bootstrap.datemax);
            if (!result.IsSccuess)
            {
                return new PageGridData().JilToJson();
            }
            return result.ToGridJson();
        }
    }
}
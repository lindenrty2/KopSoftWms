using IServices;
using IServices.Outside;
using Microsoft.AspNetCore.Http;
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
using YL.Utils.Excel;
using YL.Utils.Extensions;
using YL.Utils.Files;
using YL.Utils.Pub;
using YL.Utils.Security;
using YL.Utils.Table; 
using YL.Utils.Json;
using System;
using System.Collections.Generic;

namespace KopSoftWms.Controllers
{
    public class MaterialController : BaseController
    {
        private readonly IWms_materialServices _materialServices;
        private readonly IWms_inventoryServices _inventoryServices;
        private readonly SqlSugarClient _client;

        public MaterialController(
            SqlSugarClient client,
            IWms_materialServices materialServices,
            IWms_inventoryServices inventoryServices
            )
        {
            _client = client;
            _materialServices = materialServices;
            _inventoryServices = inventoryServices;
        }

        [HttpGet]
        [CheckMenu]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [OperationLog(LogType.select)]
        public async Task<string> List([FromForm]Bootstrap.BootstrapParams bootstrap)
        {
            //var sd = _materialServices.PageList(bootstrap);
            //return Content(sd);

            IWMSBaseApiAccessor wmsAccessor = WMSApiManager.GetBaseApiAccessor(bootstrap.storeId.ToString(), _client);
            RouteData<Wms_MaterialDto[]> result = (await wmsAccessor.GetMateralList(
                bootstrap.pageIndex, bootstrap.limit, bootstrap.search,
                new string[] { bootstrap.sort + " " + bootstrap.order },
                bootstrap.datemin, bootstrap.datemax));
            if (!result.IsSccuess)
            {
                return new PageGridData().JilToJson();
            }
            return result.ToGridJson();
        }

        [HttpGet]
        public IActionResult Add(string id)
        {
            var model = new Wms_material();
            if (id.IsEmpty())
            {
                return View(model);
            }
            else
            {
                model = _materialServices.QueryableToEntity(c => c.MaterialId == SqlFunc.ToInt64(id));
                return View(model);
            }
        }

        [HttpPost]
        [FilterXss]
        [OperationLog(LogType.addOrUpdate)]
        public IActionResult AddOrUpdate([FromForm]Wms_material model, [FromForm]string id)
        {
            var validator = new MaterialFluent();
            var results = validator.Validate(model);
            var success = results.IsValid;
            if (!success)
            {
                string msg = results.Errors.Aggregate("", (current, item) => (current + item.ErrorMessage + "</br>"));
                return BootJsonH((PubEnum.Failed.ToInt32(), msg));
            }


            Sys_dict unitDict = _client.Queryable<Sys_dict>().First(x => x.DictId == model.Unit && x.DictType == PubDictType.unit.ToInt32().ToString());
            if (unitDict == null)
            {
                return BootJsonH((false, PubConst.UnitNotFound));
            }
            Sys_dict typeDict = _client.Queryable<Sys_dict>().First(x => x.DictId == model.MaterialType && x.DictType == PubDictType.material.ToInt32().ToString());
            if (typeDict == null)
            {
                return BootJsonH((false, PubConst.MaterialTypeNotFound));
            }

            if (id.IsEmptyZero())
            {
                if (_materialServices.IsAny(c => c.MaterialNo == model.MaterialNo || c.MaterialName == model.MaterialName))
                {
                    return BootJsonH((false, PubConst.Material1));
                }

                model.MaterialId = PubId.SnowflakeId;
                model.UnitName = unitDict.DictName;
                model.MaterialTypeName = typeDict.DictName;
                model.CreateBy = UserDtoCache.UserId;
                model.CreateDate = DateTime.Now;
                model.CreateUser = UserDtoCache.UserName;
                model.ModifiedBy = UserDtoCache.UserId;
                model.ModifiedDate = DateTime.Now;
                model.ModifiedUser = UserDtoCache.UserName;

                bool flag = _materialServices.Insert(model);
                return BootJsonH(flag ? (flag, PubConst.Add1) : (flag, PubConst.Add2));
            }
            else
            {
                model.MaterialId = id.ToInt64();
                model.UnitName = unitDict.DictName;
                model.MaterialTypeName = typeDict.DictName;
                model.ModifiedBy = UserDtoCache.UserId;
                model.ModifiedDate = DateTimeExt.DateTime;
                var flag = _materialServices.Update(model);
                return BootJsonH(flag ? (flag, PubConst.Update1) : (flag, PubConst.Update2));
            }
        }

        [HttpGet]
        [OperationLog(LogType.delete)]
        public IActionResult Delete(string id)
        {
            //判断库存数量，库存数量小于等于0，才能删除
            var isExist = _inventoryServices.IsAny(c => c.MaterialId == SqlFunc.ToInt64(id));
            if (isExist)
            {
                return BootJsonH((false, PubConst.Material2));
            }
            else
            {
                var flag = _materialServices.Update(new Wms_material { MaterialId = SqlFunc.ToInt64(id), IsDel = 0, ModifiedBy = UserDtoCache.UserId, ModifiedDate = DateTimeExt.DateTime }, c => new { c.IsDel, c.ModifiedBy, c.ModifiedDate });
                return BootJsonH(flag ? (flag, PubConst.Delete1) : (flag, PubConst.Delete2));
            }
        }

        /// <summary>
        /// 入库选择物料，默认显示100条数据，如果没有在从服务端取数据
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<string> Search(long storeId,string text)
        {
            IWMSBaseApiAccessor wmsAccessor = WMSApiManager.GetBaseApiAccessor(storeId.ToString(), _client);
            RouteData<Wms_MaterialDto[]> result = (await wmsAccessor.GetMateralList(1, 100, text, new string[0], null,null));
            if (!result.IsSccuess)
            {
                return new PageGridData().JilToJson();
            }
            return result.ToGridJson();
        }

        /// <summary>
        /// 入库选择物料，默认显示100条数据，如果没有在从服务端取数据
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<RouteData<Wms_material>> SearchMaterialNo(long storeId, string materialNo)
        {
            Wms_material material = await _materialServices.Queryable().FirstAsync(x => x.WarehouseId == storeId && x.MaterialNo == materialNo);
            if(material == null)
            {
                return RouteData<Wms_material>.From(PubMessages.E1005_MATERIALNO_NOTFOUND);
            }
            return RouteData<Wms_material>.From(material); 
        }


        [HttpGet]
        [OperationLog(LogType.export)]
        public IActionResult Export([FromQuery]Bootstrap.BootstrapParams bootstrap)
        {
            var buffer = _materialServices.ExportList(bootstrap);
            if (buffer.IsNull())
            {
                return File(JsonL((false, PubConst.File8)).ToBytes(), ContentType.ContentTypeJson);
            }
            return File(buffer, ContentType.ContentTypeFile, DateTimeExt.GetDateTimeS(DateTimeExt.DateTimeFormatString) + "-" + EncoderUtil.UrlHttpUtilityEncoder("物料") + ".xlsx");
        }
        
        [HttpPost]
        public async Task<RouteData> Import([FromForm]IFormFile file)
        {
            if(file == null)
            {
                return YL.Core.Dto.RouteData.From(PubMessages.E0003_FILEUPLOAD_FAIL);
            }
            if(!file.FileName.EndsWith(".xls") && !file.FileName.EndsWith(".xlsx"))
            {
                return YL.Core.Dto.RouteData.From(PubMessages.E4101_MATERIAL_IMPORT_NOTSUPPORT);
            }
            return await _materialServices.ImportList(file);
        }

    }
}
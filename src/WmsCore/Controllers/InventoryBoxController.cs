﻿using IServices;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Linq;
using YL.Core.Dto;
using YL.Core.Entity;
using YL.Core.Entity.Fluent.Validation;
using YL.NetCore.Attributes;
using YL.NetCore.NetCoreApp;
using YL.Utils.Extensions;
using YL.Utils.Pub;
using YL.Utils.Table;
using static YL.Core.Dto.PubParams;

namespace KopSoftWms.Controllers
{
    public class InventoryBoxController : BaseController
    {
        private readonly IWms_inventoryBoxServices _inventoryBoxServices;
        private readonly IWms_inventoryServices _inventoryServices;
        private readonly IWms_storagerackServices _storagerackServices;

        public InventoryBoxController(
            IWms_storagerackServices storagerackServices,
            IWms_inventoryBoxServices inventoryBoxServices,
            IWms_inventoryServices inventoryServices
            )
        {
            _storagerackServices = storagerackServices;
            _inventoryBoxServices = inventoryBoxServices;
            _inventoryServices = inventoryServices;
        }

        [HttpGet]
        [CheckMenu]
        public IActionResult Index()
        {
            long currentStoreId = (long)ViewData["currentStoreId"];
            ViewBag.StorageRack = _storagerackServices.QueryableToList(c => c.WarehouseId == currentStoreId && c.IsDel == 1);
            return View();
        }

        [HttpPost]
        [OperationLog(LogType.select)]
        public ContentResult List([FromForm]PubParams.InventoryBoxBootstrapParams bootstrap)
        {
            var sd = _inventoryBoxServices.PageList(bootstrap);
            return Content(sd);
        }



        [HttpGet]
        public IActionResult Add(string id)
        {
            var model = new Wms_inventorybox();
            if (id.IsEmpty())
            {
                return View(model);
            }
            else
            {
                model = _inventoryBoxServices.QueryableToEntity(c => c.InventoryBoxId == SqlFunc.ToInt64(id));
                return View(model);
            }
        }

        [HttpPost]
        [FilterXss]
        [OperationLog(LogType.addOrUpdate)]
        public IActionResult AddOrUpdate([FromForm]Wms_inventorybox model, [FromForm]string id)
        {
            var validator = new InventoryBoxFluent();
            var results = validator.Validate(model);
            var success = results.IsValid;
            if (!success)
            {
                string msg = results.Errors.Aggregate("", (current, item) => (current + item.ErrorMessage + "</br>"));
                return BootJsonH((PubEnum.Failed.ToInt32(), msg));
            }
            if (id.IsEmptyZero())
            {
                if (_inventoryBoxServices.IsAny(c => c.InventoryBoxNo == model.InventoryBoxNo && c.IsDel == 1))
                {
                    return BootJsonH((false, PubConst.InventoryBox_Duplicate));
                }
                model.InventoryBoxId = PubId.SnowflakeId;
                model.CreateBy = UserDtoCache.UserId;
                bool flag = _inventoryBoxServices.Insert(model);
                return BootJsonH(flag ? (flag, PubConst.Add1) : (flag, PubConst.Add2));
            }
            else
            {
                model.InventoryBoxId = id.ToInt64();
                model.ModifiedBy = UserDtoCache.UserId;
                model.ModifiedDate = DateTimeExt.DateTime;
                var flag = _inventoryBoxServices.Update(model,null,false);
                return BootJsonH(flag ? (flag, PubConst.Update1) : (flag, PubConst.Update2));
            }
        }

        [HttpGet]
        [OperationLog(LogType.delete)]
        public IActionResult Delete(string id)
        {
            //判断库存数量，库存数量小于等于0，才能删除
            var isExist = _inventoryBoxServices.IsAny(c => c.InventoryBoxId == SqlFunc.ToInt64(id));
            if (isExist)
            {
                return BootJsonH((false, PubConst.InventoryBox_NonExistent));
            }
            bool hasItems = _inventoryServices.IsAny(c => c.InventoryBoxId == SqlFunc.ToInt64(id));
            if (hasItems)
            {
                return BootJsonH((false, PubConst.InventoryBox_CannotDeleteBecauseItems));
            }
            var flag = _inventoryBoxServices.Update(new Wms_inventorybox { InventoryBoxId = SqlFunc.ToInt64(id), IsDel = 0, ModifiedBy = UserDtoCache.UserId, ModifiedDate = DateTimeExt.DateTime }, c => new { c.IsDel, c.ModifiedBy, c.ModifiedDate });
            return BootJsonH(flag ? (flag, PubConst.Delete1) : (flag, PubConst.Delete2));
            
        }

        [HttpGet]
        public IActionResult Search(long storeId, string text)
        {
            var bootstrap = new InventoryBoxBootstrapParams()
            {
                limit = 100,
                offset = 0,
                sort = "CreateDate",
                search = text,
                order = "desc",
                storeId = storeId
            };
            var json = _inventoryBoxServices.PageList(bootstrap);
            return Content(json);
        }


        [HttpGet]
        public IActionResult Detail(string id)
        {
            long searchId = id.ToInt64();
            var model = new Wms_InventryBoxDto();
            if (id.IsEmptyZero())
            {
                return Content("");
            }
            else
            {
                Wms_inventorybox box = _inventoryBoxServices.QueryableToEntity(c => c.InventoryBoxId == searchId && c.IsDel == 1);
                model.InventoryBoxId = box.InventoryBoxId;
                model.InventoryBoxNo = box.InventoryBoxNo;
                model.InventoryBoxName = box.InventoryBoxName;
                model.Size = box.Size;
                model.WarehouseId = box.WarehouseId;
                model.ReservoirAreaId = box.ReservoirAreaId;
                model.StorageRackId = box.StorageRackId;
                model.Detail = _inventoryServices.QueryableToList(c => c.InventoryBoxId == searchId && c.IsDel == 1);
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult ListDetail(string id)
        {
            long searchId = id.ToInt64();
            var model = new Wms_inventory();
            if (id.IsEmptyZero())
            {
                return Content("");
            }
            else
            {
                model = _inventoryServices.QueryableToEntity(c => c.InventoryBoxId == searchId);
                return View(model);
            }
        }
    }
}
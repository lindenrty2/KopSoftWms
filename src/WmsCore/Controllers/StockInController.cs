using IServices;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WMSCore.Outside;
using YL.Core.Dto;
using YL.Core.Entity;
using YL.Core.Entity.Fluent.Validation;
using YL.NetCore.Attributes;
using YL.NetCore.NetCoreApp;
using YL.Utils.Extensions;
using YL.Utils.Pub;
using YL.Utils.Table;

namespace KopSoftWms.Controllers
{
    public class StockInController : BaseController
    {
        private readonly IWms_stockinServices _stockinServices;
        private readonly IWms_supplierServices _supplierServices;
        private readonly ISys_dictServices _dictServices;
        private readonly ISys_serialnumServices _serialnumServices;
        private readonly IWms_stockindetailServices _stockindetailServices;
        private readonly IWms_stockindetailboxServices _stockindetailboxServices;
        private readonly IWms_inventoryBoxServices _inventoryBoxServices;
        private readonly IWms_inventoryBoxTaskServices _inventoryBoxTaskServices;
        private readonly IWms_inventoryServices _inventoryServices;
        private readonly SqlSugarClient _client;

        public StockInController( 
            IWms_stockindetailServices stockindetailServices,
            IWms_stockindetailboxServices stockindetailboxServices,
            ISys_serialnumServices serialnumServices,
            ISys_dictServices dictServices,
            IWms_supplierServices supplierServices,
            IWms_stockinServices stockinServices,
            IWms_inventoryBoxTaskServices stockinTaskServices, 
            IWms_inventoryBoxServices inventoryBoxServices,
            IWms_inventoryServices inventoryServices,
            SqlSugarClient client
            )
        {
            _stockindetailServices = stockindetailServices;
            _stockindetailboxServices = stockindetailboxServices;
            _serialnumServices = serialnumServices;
            _dictServices = dictServices;
            _supplierServices = supplierServices;
            _stockinServices = stockinServices;
            _inventoryBoxTaskServices = stockinTaskServices;
            _inventoryBoxServices = inventoryBoxServices;
            _inventoryServices = inventoryServices;
            _client = client;
        }

        [HttpGet]
        [CheckMenu]
        public IActionResult Index()
        {
            var list = _dictServices.Queryable().Where(c => c.IsDel == 1 && c.DictType == PubDictType.stockin.ToString()).ToList();
            var stockInStatus = EnumExt.ToKVListLinq<StockInStatus>();
            ViewBag.StockInType = list;
            ViewBag.StockInStatus = stockInStatus;
            return View();
        }

        /// <summary>
        /// 主表
        /// </summary>
        /// <param name="bootstrap">参数</param>
        /// <returns></returns>
        [HttpPost]
        [OperationLog(LogType.select)]
        public ContentResult List([FromForm]PubParams.StockInBootstrapParams bootstrap)
        {
            var sd = _stockinServices.PageList(bootstrap);
            return Content(sd);
        }

        /// <summary>
        /// 明细
        /// </summary>
        /// <param name="id">主表id</param>
        /// <returns></returns>
        [HttpPost]
        [OperationLog(LogType.select)]
        public ContentResult ListDetail(string pid)
        {
            var sd = _stockindetailServices.PageList(pid);
            return Content(sd);
        }

        [HttpGet]
        public IActionResult Add(string id,long storeId)
        {
            var model = new Wms_stockin();
            if (id.IsEmpty())
            {
                model.WarehouseId = storeId;
                return View(model);
            }
            else
            {
                model = _stockinServices.QueryableToEntity(c => c.StockInId == SqlFunc.ToInt64(id) && c.IsDel == 1);
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Detail(string id, string pid, long storeId)
        {
            var model = new Wms_stockindetail();
            ViewData["currentStoreId"] = storeId;
            if (id.IsEmptyZero())
            {
                model.StockInId = pid.ToInt64();
                return View(model);
            }
            else
            {
                model = _stockindetailServices.QueryableToEntity(c => c.StockInDetailId == SqlFunc.ToInt64(id) && c.IsDel == 1);
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Work(string pid)
        {
            var model = _stockinServices.QueryableToEntity(c => c.StockInId == SqlFunc.ToInt64(pid) && c.IsDel == 1);
            ViewData["currentStoreId"] = model.WarehouseId;
            return View(model);
        }

        [HttpGet]
        public ContentResult WorkList(string pid)
        {
            var workList = _stockindetailServices.PageList(pid); 
            return Content(workList);
        }

        [HttpGet]
        public IActionResult InventoryBoxOut( long storeId)
        {
            ViewData["currentStoreId"] = storeId;
            return View(null);
        }

        [HttpPost]
        public async Task<RouteData> DoInventoryBoxOut(long inventoryBoxId)
        {
            try
            {
                _client.BeginTran(); 

                Wms_inventorybox inventoryBox = _inventoryBoxServices.QueryableToEntity(x => x.InventoryBoxId == inventoryBoxId);
                if (inventoryBox == null) { return YL.Core.Dto.RouteData.From(PubMessages.E2003_INVENTORYBOX_NOTFOUND); }
                if (inventoryBox.Status != InventoryBoxStatus.InPosition) { return YL.Core.Dto.RouteData.From(PubMessages.E2004_INVENTORYBOX_NOTINPOSITION); }

                Wms_inventoryboxTask task = new Wms_inventoryboxTask()
                {
                    InventoryBoxTaskId = PubId.SnowflakeId,
                    InventoryBoxId = inventoryBoxId,
                    ReservoirareaId = (long)inventoryBox.ReservoirAreaId,
                    StoragerackId = (long)inventoryBox.StorageRackId,
                    Data = null,
                    OperaterDate = DateTime.Now,
                    OperaterId = UserDto.UserId, 
                    Status = InventoryBoxTaskStatus.task_outing.ToByte()
                };
                if (! await SendWCSOutCommand(task))
                {
                    _client.RollbackTran();
                    return YL.Core.Dto.RouteData.From(PubMessages.E2100_WCS_OUTCOMMAND_FAIL);
                }

                _inventoryBoxTaskServices.Insert(task);

                inventoryBox.Status = InventoryBoxStatus.Outing;
                inventoryBox.ModifiedBy = UserDto.UserId;
                inventoryBox.ModifiedDate = DateTime.Now;
                _inventoryBoxServices.Update(inventoryBox); 

                _client.CommitTran();
                return new RouteData();
            }
            catch(Exception)
            {
                _client.RollbackTran();
                return YL.Core.Dto.RouteData.From(PubMessages.E2005_STOCKIN_BOXOUT_FAIL);
            }

        }
        private async Task<bool> SendWCSOutCommand(Wms_inventoryboxTask task )
        {
            try
            {
                Wms_storagerack storagerack = _client.Queryable<Wms_storagerack>().First(x => x.StorageRackId == task.StoragerackId);

                OutStockInfo outStockInfo = new OutStockInfo()
                {
                    TaskId = task.InventoryBoxTaskId.ToString(),
                    GetColumn = storagerack.Column.ToString(),
                    GetRow = storagerack.Row.ToString(),
                    GetFloor = storagerack.Floor.ToString()
                };
                CreateOutStockResult result = await WCSApiAccessor.Instance.CreateOutStockTask(outStockInfo);
                return result.Successd;
            }
            catch(Exception)
            {
                return false;
            }
        }

        [HttpGet]
        public IActionResult ScanPage(long storeId,long stockInId)
        {
            ViewData["currentStoreId"] = storeId;
            var model = _stockinServices.QueryableToEntity(c => c.StockInId == SqlFunc.ToInt64(stockInId) && c.IsDel == 1);
            return View(model);
        }

        [HttpPost]
        public RouteData DoScanComplate(long stockInId,long inventoryBoxId, Wms_StockInMaterialDetailDto[] materials,string remark)
        {
            try
            {
                _client.BeginTran();

                Wms_stockin stockin = _stockinServices.QueryableToEntity(x => x.StockInId == stockInId);
                if (stockin == null) { return YL.Core.Dto.RouteData.From(PubMessages.E2013_STOCKIN_NOTFOUND); }
                if (stockin.StockInStatus == StockInStatus.task_finish.ToByte()) { return YL.Core.Dto.RouteData.From(PubMessages.E2014_STOCKIN_ALLOW_FINISHED); }
               
                Wms_inventorybox inventoryBox = _inventoryBoxServices.QueryableToEntity(x => x.InventoryBoxId == inventoryBoxId);
                if (inventoryBox == null) { return YL.Core.Dto.RouteData.From(PubMessages.E2003_INVENTORYBOX_NOTFOUND); }
                if (inventoryBox.Status != InventoryBoxStatus.Outed) { return YL.Core.Dto.RouteData.From(PubMessages.E2011_INVENTORYBOX_NOTOUTED); }

                Wms_inventoryboxTask inventoryBoxTask = _inventoryBoxTaskServices.QueryableToEntity(x => x.InventoryBoxId == inventoryBoxId && x.Status == InventoryBoxTaskStatus.task_outed.ToByte());
                if (inventoryBoxTask == null) { return YL.Core.Dto.RouteData.From(PubMessages.E2011_INVENTORYBOX_NOTOUTED); } //数据异常 


                foreach (Wms_StockInMaterialDetailDto material in materials)
                {
                    Wms_stockindetail detail = _stockindetailServices.QueryableToEntity(x => x.StockInId == stockInId && x.MaterialId.ToString() == material.MaterialId);
                    if (detail == null) { return YL.Core.Dto.RouteData.From(PubMessages.E2001_STOCKINDETAIL_NOTFOUND,$"MaterialId='{material.MaterialId}'"); }
                    if (detail.Status == StockInStatus.task_finish.ToByte()) { return YL.Core.Dto.RouteData.From(PubMessages.E2002_STOCKINDETAIL_ALLOW_FINISHED); }

                    Wms_stockindetail_box box = null;
                    if(_stockindetailboxServices.IsAny(x => x.InventoryBoxTaskId == inventoryBoxTask.InventoryBoxTaskId && x.StockinDetailId == detail.StockInDetailId))
                    {
                        continue;
                    }
                    box = new Wms_stockindetail_box()
                    {
                        DetailBoxId = PubId.SnowflakeId,
                        InventoryBoxTaskId = inventoryBoxTask.InventoryBoxTaskId,
                        InventoryBoxId = inventoryBox.InventoryBoxId,
                        StockinDetailId = detail.StockInDetailId,
                        Remark = remark,
                        CreateDate = DateTime.Now,
                        CreateBy = UserDto.UserId
                    };
                    
                    _stockindetailboxServices.Insert(box);

                    if (detail.Status == StockInStatus.task_confirm.ToByte())
                    {
                        detail.Status = StockInStatus.task_working.ToByte();
                        _stockindetailServices.Update(detail);
                    }
                }
                
                if (stockin.StockInStatus == StockInStatus.task_confirm.ToByte())
                {
                    stockin.StockInStatus = StockInStatus.task_working.ToByte();
                    _stockinServices.Update(stockin);
                }

                _client.CommitTran();
                return new YL.Core.Dto.RouteData();
            }
            catch (Exception)
            {
                _client.RollbackTran();
                return YL.Core.Dto.RouteData.From(PubMessages.E2005_STOCKIN_BOXOUT_FAIL);
            }

        }

        [HttpGet]
        public RouteData<Wms_StockInMaterialDetailDto> SearchMaterial(long storeId,long stockInId,string materialNo)
        {
            Wms_material material = _client.Queryable<Wms_material>().First( x => x.MaterialNo == materialNo);
            if(material == null)
            {
                return RouteData<Wms_StockInMaterialDetailDto>.From(PubMessages.E1005_MaterialNo_NotFound);
            }
            Wms_stockindetail detail = _stockindetailServices.QueryableToEntity(x => x.StockInId == stockInId && x.MaterialId == material.MaterialId);
            if(detail == null)
            {
                return RouteData<Wms_StockInMaterialDetailDto>.From(PubMessages.E2015_STOCKIN_HASNOT_MATERIAL);
            }
            Wms_StockInMaterialDetailDto detailDto = new Wms_StockInMaterialDetailDto()
            {
                StockInId = detail.StockInId.ToString(),
                StockInDetailId = detail.StockInDetailId.ToString(),
                MaterialId = material.MaterialId.ToString(),
                MaterialNo = material.MaterialNo,
                MaterialName = material.MaterialName,
                PlanInQty = (int)detail.PlanInQty,
                ActInQty = (int)detail.ActInQty
            };
            return RouteData<Wms_StockInMaterialDetailDto>.From(detailDto);
        }

        [HttpGet]
        public IActionResult InventoryBoxBack(long stockinTaskId)
        {
            return View(null);
        }

        [HttpPost]
        public async Task<RouteData> DoInventoryBoxBack(long inventoryBoxTaskId, InventoryDetailDto[] details)
        {
            try
            {
                _client.BeginTran();

                Wms_inventoryboxTask task = _inventoryBoxTaskServices.QueryableToEntity(x => x.InventoryBoxTaskId == inventoryBoxTaskId);
                if (task == null) { return YL.Core.Dto.RouteData.From(PubMessages.E2007_STOCKINTASK_NOTFOUND); }
                if (task.Status <= InventoryBoxTaskStatus.task_outed.ToByte()) { return YL.Core.Dto.RouteData.From(PubMessages.E2008_STOCKINTASK_NOTOUT); }
                if (task.Status == InventoryBoxTaskStatus.task_backing.ToByte()) { return YL.Core.Dto.RouteData.From(PubMessages.E2009_STOCKINTASK_ALLOW_BACKING); }
                if (task.Status == InventoryBoxTaskStatus.task_backed.ToByte()) { return YL.Core.Dto.RouteData.From(PubMessages.E2010_STOCKINTASK_ALLOW_BACKED); }

                Wms_inventorybox inventoryBox = _inventoryBoxServices.QueryableToEntity(x => x.InventoryBoxId == task.InventoryBoxId);
                if (inventoryBox == null) { return YL.Core.Dto.RouteData.From(PubMessages.E2003_INVENTORYBOX_NOTFOUND); }
                if (inventoryBox.Status != InventoryBoxStatus.Outed) { return YL.Core.Dto.RouteData.From(PubMessages.E2011_INVENTORYBOX_NOTOUTED); }

                List<Wms_inventory> inventories = _inventoryServices.QueryableToList(x => x.InventoryBoxId == task.InventoryBoxId);
                List<Wms_inventory> updatedInventories = new List<Wms_inventory>();
                foreach(InventoryDetailDto detail in details)
                {
                    Wms_inventory inventory = inventories.FirstOrDefault(x => x.InventoryId == detail.InventoryId);
                    if(inventory.MaterialId != detail.MaterialId)
                    {
                        return YL.Core.Dto.RouteData.From(PubMessages.E2012_INVENTORYBOX_MATERIAL_NOTMATCH);
                    }
                    inventory.Qty = detail.Qty;
                    inventory.ModifiedDate = DateTime.Now;
                    inventory.ModifiedBy = this.UserDto.UserId;
                    
                    updatedInventories.Add(inventory);
                }
                if (! await SendWCSBackCommand(task))
                {
                    _client.RollbackTran();
                    return YL.Core.Dto.RouteData.From(PubMessages.E2110_WCS_BACKCOMMAND_FAIL);
                }

                foreach (Wms_inventory inventoriy in updatedInventories)
                {
                    _inventoryServices.Update(inventoriy);
                }

                task.Status = InventoryBoxTaskStatus.task_backing.ToByte();
                task.OperaterId = UserDto.UserId;
                task.OperaterDate = DateTime.Now;
                _inventoryBoxTaskServices.UpdateEntity(task);

                inventoryBox.Status = InventoryBoxStatus.Backing;
                inventoryBox.ModifiedBy = UserDto.UserId;
                inventoryBox.ModifiedDate = DateTime.Now;
                _inventoryBoxServices.UpdateEntity(inventoryBox);

                _client.CommitTran();
                return new YL.Core.Dto.RouteData();
            }
            catch (Exception)
            {
                _client.RollbackTran();
                return YL.Core.Dto.RouteData.From(PubMessages.E2005_STOCKIN_BOXOUT_FAIL);
            }
        }

        private async Task<bool> SendWCSBackCommand(Wms_inventoryboxTask task)
        {
            try
            {
                Wms_storagerack storagerack = _client.Queryable<Wms_storagerack>().First(x => x.StorageRackId == task.StoragerackId);

                BackStockInfo backStockInfo = new BackStockInfo()
                {
                    TaskId = task.InventoryBoxTaskId.ToString()
                };
                CreateBackStockResult result = await WCSApiAccessor.Instance.CreateBackStockTask(backStockInfo);
                return result.Successd;
            }
            catch(Exception)
            {
                return false;
            }
        }

        [HttpPost]
        [FilterXss]
        [OperationLog(LogType.addOrUpdate)]
        public IActionResult AddOrUpdate([FromForm]Wms_stockin model, [FromForm]string id)
        {
            var validator = new StockInFluent();
            var results = validator.Validate(model);
            var success = results.IsValid;
            if (!success)
            {
                string msg = results.Errors.Aggregate("", (current, item) => (current + item.ErrorMessage + "</br>"));
                return BootJsonH((PubEnum.Failed.ToInt32(), msg));
            }
            if (id.IsEmptyZero())
            {
                if (!model.OrderNo.IsEmpty())
                {
                    if (_stockinServices.IsAny(c => c.OrderNo == model.OrderNo))
                    {
                        return BootJsonH((false, PubConst.StockIn1));
                    }
                }
                model.StockInNo = _serialnumServices.GetSerialnum(UserDtoCache.UserId, "Wms_stockin");
                model.StockInId = PubId.SnowflakeId;
                model.StockInStatus = StockInStatus.initial.ToByte();
                model.CreateBy = UserDtoCache.UserId; 
                bool flag = _stockinServices.Insert(model);
                return BootJsonH(flag ? (flag, PubConst.Add1) : (flag, PubConst.Add2));
            }
            else
            {
                model.StockInId = id.ToInt64();
                model.ModifiedBy = UserDtoCache.UserId;
                model.ModifiedDate = DateTimeExt.DateTime;
                var flag = _stockinServices.Update(model);
                return BootJsonH(flag ? (flag, PubConst.Update1) : (flag, PubConst.Update2));
            }
        }

        [HttpPost]
        [FilterXss]
        [OperationLog(LogType.addOrUpdate)]
        public IActionResult AddOrUpdateD([FromForm]Wms_stockindetail model, [FromForm]string id)
        {
            var validator = new StockInDetailFluent();
            var results = validator.Validate(model);
            var success = results.IsValid;
            if (!success)
            {
                string msg = results.Errors.Aggregate("", (current, item) => (current + item.ErrorMessage + "</br>"));
                return BootJsonH((PubEnum.Failed.ToInt32(), msg));
            }
            if (id.IsEmptyZero())
            {
                model.StockInDetailId = PubId.SnowflakeId;
                model.Status = StockInStatus.initial.ToByte();
                model.CreateBy = UserDtoCache.UserId;
                bool flag = _stockindetailServices.Insert(model);
                return BootJsonH(flag ? (flag, PubConst.Add1) : (flag, PubConst.Add2));
            }
            else
            {
                model.StockInDetailId = id.ToInt64();
                model.ModifiedBy = UserDtoCache.UserId;
                model.ModifiedDate = DateTimeExt.DateTime;
                var flag = _stockindetailServices.Update(model);
                return BootJsonH(flag ? (flag, PubConst.Update1) : (flag, PubConst.Update2));
            }
        }

        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [OperationLog(LogType.update)]
        public IActionResult Auditin(string id)
        {
            var list = _stockindetailServices.QueryableToList(c => c.IsDel == 1 && c.StockInId == SqlFunc.ToInt64(id));
            if (!list.Any())
            {
                return BootJsonH((false, PubConst.StockIn4));
            }
            var flag = _stockinServices.Auditin(UserDtoCache.UserId, SqlFunc.ToInt64(id));
            return BootJsonH(flag ? (flag, PubConst.StockIn2) : (flag, PubConst.StockIn3));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主表id</param>
        /// <returns></returns>
        [HttpGet]
        [OperationLog(LogType.delete)]
        public IActionResult Delete(string id)
        {
            var flag = _client.Ado.UseTran(() =>
            {
                _client.Updateable(new Wms_stockindetail { IsDel = 0, ModifiedBy = UserDtoCache.UserId, ModifiedDate = DateTimeExt.DateTime })
                .UpdateColumns(c => new { c.IsDel, c.ModifiedBy, c.ModifiedDate })
                .Where(c => c.StockInId == SqlFunc.ToInt64(id) && c.IsDel == 1).ExecuteCommand();
                _client.Updateable(new Wms_stockin { StockInId = SqlFunc.ToInt64(id), IsDel = 0, ModifiedBy = UserDtoCache.UserId, ModifiedDate = DateTimeExt.DateTime })
               .UpdateColumns(c => new { c.IsDel, c.ModifiedBy, c.ModifiedDate })
               .ExecuteCommand();
            }).IsSuccess;
            return BootJsonH(flag ? (flag, PubConst.Delete1) : (flag, PubConst.Delete2));
        }

        /// <summary>
        /// 删除明细
        /// </summary>
        /// <param name="id">明细id</param>
        /// <returns></returns>
        [HttpGet]
        [OperationLog(LogType.delete)]
        public IActionResult DeleteDetail(string id)
        {
            var flag = _stockindetailServices.Update(
                 new Wms_stockindetail { IsDel = 0, ModifiedBy = UserDtoCache.UserId, ModifiedDate = DateTimeExt.DateTime },
                 c => new { c.IsDel, c.ModifiedBy, c.ModifiedDate },
                 c => c.StockInDetailId == SqlFunc.ToInt64(id)
                 );
            return BootJsonH(flag ? (flag, PubConst.Delete1) : (flag, PubConst.Delete2));
        }

        [HttpGet]
        public IActionResult PreviewJson(string id)
        {
            var str = _stockinServices.PrintList(id);
            return Content(str);
        }
    }
}
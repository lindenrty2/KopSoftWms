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
using YL.Utils.Json;
using static YL.Core.Dto.PubParams;

namespace KopSoftWms.Controllers
{
    public class InventoryBoxController : BaseController
    {
        private readonly IWms_inventoryBoxServices _inventoryBoxServices;
        private readonly IWms_inventoryBoxTaskServices _inventoryBoxTaskServices;
        private readonly IWms_inventoryServices _inventoryServices;
        private readonly IWms_storagerackServices _storagerackServices;
        private readonly SqlSugarClient _client;

        public InventoryBoxController(
            IWms_storagerackServices storagerackServices,
            IWms_inventoryBoxServices inventoryBoxServices,
            IWms_inventoryBoxTaskServices inventoryBoxTaskServices,
            IWms_inventoryServices inventoryServices,
            SqlSugarClient client
            )
        {
            _client = client;
            _storagerackServices = storagerackServices;
            _inventoryBoxServices = inventoryBoxServices;
            _inventoryBoxTaskServices = inventoryBoxTaskServices;
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
        public IActionResult Detail(long storeId, string id)
        {
            ViewData["currentStoreId"] = storeId;
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
        public async Task<string> DetailList(string id)
        {
            long searchId = id.ToInt64(); 
            if (id.IsEmptyZero())
            {
                return PubMessages.E1006_INVENTORYBOX_MISSING.Message;
            }
            else
            {
                var query = _client.Queryable<Wms_inventory, Wms_material,Sys_user>((i,m,u) => new object[] {
                   JoinType.Left,i.MaterialId==m.MaterialId,
                   JoinType.Left,i.ModifiedBy==u.UserId 
                 })
                .Where((i,m,u) => i.InventoryBoxId == searchId )
                .Select((i, m ,u) => new
                {
                    i.Position,
                    m.MaterialNo,
                    m.MaterialOnlyId,
                    m.MaterialName,
                    i.Qty, 
                    u.UserName,
                    i.ModifiedDate
                }).MergeTable();
                var list = await query.ToListAsync();
                return Bootstrap.GridData(list, list.Count).JilToJson();
            }
        }


        [HttpGet]
        public IActionResult InventoryBoxOut(long storeId)
        {
            ViewData["currentStoreId"] = storeId;
            return View(null);
        }

        [HttpPost]
        public async Task<RouteData> DoInventoryBoxOut(int size)
        {
            return null;
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

                Wms_inventoryboxTask task = _inventoryBoxTaskServices.QueryableToEntity(x => x.InventoryBoxId == inventoryBox.InventoryBoxId && x.Status == InventoryBoxStatus.Outed.ToByte());
                if(task != null)
                {
                    return YL.Core.Dto.RouteData.From(PubMessages.E2101_WCS_STOCKOUT_NOTCOMPLATE_EXIST); 
                }
                task = new Wms_inventoryboxTask()
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
                if (!await SendWCSOutCommand(task))
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
                return YL.Core.Dto.RouteData.From(PubMessages.I2100_WCS_OUTCOMMAND_SCCUESS);
            }
            catch (Exception)
            {
                _client.RollbackTran();
                return YL.Core.Dto.RouteData.From(PubMessages.E2005_STOCKIN_BOXOUT_FAIL);
            }

        }
        private async Task<bool> SendWCSOutCommand(Wms_inventoryboxTask task)
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
            catch (Exception)
            {
                return false;
            }
        }


        ///// <summary>
        ///// 归库处理
        ///// </summary>
        ///// <param name="inventoryBoxId"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public async Task<RouteData> DoInventoryBoxBack(long inventoryBoxId)
        //{
        //    try
        //    {

        //        Wms_inventorybox inventoryBox = _inventoryBoxServices.QueryableToEntity(x => x.InventoryBoxId == inventoryBoxId);
        //        if (inventoryBox == null) { return YL.Core.Dto.RouteData.From(PubMessages.E2003_INVENTORYBOX_NOTFOUND); }
        //        if (inventoryBox.Status != InventoryBoxStatus.Outed) { return YL.Core.Dto.RouteData.From(PubMessages.E2011_INVENTORYBOX_NOTOUTED); }

        //        Wms_inventoryboxTask task = _inventoryBoxTaskServices.QueryableToEntity(x => x.InventoryBoxId == inventoryBox.InventoryBoxId && x.Status == InventoryBoxStatus.Outed.ToByte());
        //        if(task == null)
        //        {
        //            return YL.Core.Dto.RouteData.From(PubMessages.E2111_WCS_STOCKBACKTASK_NOTFOUND);
        //        }
        //        if (!await SendWCSBackCommand(task))
        //        {
        //            return YL.Core.Dto.RouteData.From(PubMessages.E2100_WCS_OUTCOMMAND_FAIL);
        //        }

        //        _client.BeginTran();

        //        inventoryBox.Status = InventoryBoxStatus.Backing;
        //        inventoryBox.ModifiedBy = UserDto.UserId;
        //        inventoryBox.ModifiedDate = DateTime.Now;
        //        _inventoryBoxServices.Update(inventoryBox);

        //        _client.CommitTran();
        //        return YL.Core.Dto.RouteData.From(PubMessages.I2100_WCS_BACKCOMMAND_SCCUESS);
        //    }
        //    catch (Exception)
        //    {
        //        _client.RollbackTran();
        //        return YL.Core.Dto.RouteData.From(PubMessages.E2005_STOCKIN_BOXOUT_FAIL);
        //    }

        //}


        [HttpGet]
        public IActionResult InventoryBoxBack(long inventoryBoxTaskId)
        {
            Wms_inventoryboxTask task = _inventoryBoxTaskServices.QueryableToEntity(x => x.InventoryBoxTaskId == inventoryBoxTaskId);
            if (task == null)
            {
                return Json(YL.Core.Dto.RouteData.From(PubMessages.E2201_INVENTORYBOXTASK_NOTFOUND));
            }
            Wms_inventorybox box = _inventoryBoxServices.QueryableToEntity(x => x.InventoryBoxId == task.InventoryBoxId);
            if (box == null)
            {
                return Json(YL.Core.Dto.RouteData.From(PubMessages.E2003_INVENTORYBOX_NOTFOUND));
            }
            ViewData["InventoryBoxTaskId"] = inventoryBoxTaskId;
            return View(box);
        }

        [HttpGet]
        public async Task<RouteData<InventoryDetailDto[]>> InventoryInDetailList(long inventoryBoxTaskId)
        {
            var inventoryBoxTask = await _client.Queryable<Wms_inventoryboxTask,Wms_inventorybox>(
                (ibt, ib) => new object[] { 
                   JoinType.Left,ibt.InventoryBoxId==ib.InventoryBoxId ,
                }
                )
                .Where((ibt, ib) => ibt.InventoryBoxTaskId == inventoryBoxTaskId)
                .Select( (ibt,ib) => new {
                    ibt.InventoryBoxTaskId,
                    ibt.InventoryBoxId,
                    ib.InventoryBoxNo,
                    ib.InventoryBoxName,
                    ib.Size
                }).FirstAsync();

            if(inventoryBoxTask == null)
            {
                return YL.Core.Dto.RouteData<InventoryDetailDto[]>.From(PubMessages.E2201_INVENTORYBOXTASK_NOTFOUND);
            } 

            var query = _client.Queryable< Wms_stockindetail_box, Wms_stockindetail, Wms_material >(
                (sidb,sid,m) => new object[] { 
                   JoinType.Left,sidb.StockinDetailId==sid.StockInDetailId ,
                   JoinType.Left,sid.MaterialId==m.MaterialId && m.IsDel == DeleteFlag.Normal
                  })
                 .Where((sidb, sid, m) => sidb.InventoryBoxTaskId == inventoryBoxTaskId)
                 .Select((sidb, sid, m) => new
                 {
                     sid.StockInDetailId,
                     MaterialId = m.MaterialId,
                     m.MaterialNo,
                     m.MaterialName,
                     PlanInQty = sid.PlanInQty,
                     ActInQty = sid.ActInQty,
                     Qty = sidb.Qty
                 }).MergeTable();

            var rawList = await query.ToListAsync();
            var inventoryList = _client.Queryable<Wms_inventory>().Where(x => x.InventoryBoxId == inventoryBoxTask.InventoryBoxId).ToArray();

            List<InventoryDetailDto> resultList = new List<InventoryDetailDto>();
            foreach(var inventory in inventoryList)
            {
                if (inventory.MaterialId == null) continue;
                var raw = rawList.FirstOrDefault(x => x.MaterialId == inventory.MaterialId);
                if (raw == null) continue; 
                InventoryDetailDto newInventory = new InventoryDetailDto()
                {
                    InventoryPosition = inventory.Position, 
                    MaterialId = raw.MaterialId.ToString(),
                    MaterialNo = raw.MaterialNo,
                    MaterialName = raw.MaterialName,
                    InventoryBoxId = inventoryBoxTask.InventoryBoxId.ToString(),
                    InventoryId = inventory.InventoryId.ToString(),
                    StockInDetailId = raw.StockInDetailId.ToString(),
                    StockOutDetailId = null,
                    BeforeQty = (int)inventory.Qty,
                    PlanQty = (int)raw.PlanInQty,
                    ComplateQty = (int)raw.ActInQty,
                    Qty = raw.Qty
                };
                resultList.Add(newInventory);
                rawList.Remove(raw);
            }
            int inventoryCount = inventoryList.Length;
            foreach (var raw in rawList)
            {
                var inventory = inventoryList.FirstOrDefault(x => x.MaterialId == null);
                if(inventory == null)
                {
                    if(inventoryCount >= inventoryBoxTask.Size)
                    {
                        return YL.Core.Dto.RouteData<InventoryDetailDto[]>.From(PubMessages.E2018_INVENTORYBOX_BLOCK_OVERLOAD);
                    }
                }
                inventoryCount++;
                InventoryDetailDto newInventory = new InventoryDetailDto()
                {
                    InventoryPosition = inventoryCount,
                    MaterialId = raw.MaterialId.ToString(),
                    MaterialNo = raw.MaterialNo,
                    MaterialName = raw.MaterialName,
                    InventoryBoxId = inventoryBoxTask.InventoryBoxId.ToString(),
                    InventoryId = null,
                    StockInDetailId = raw.StockInDetailId.ToString(),
                    StockOutDetailId = null,
                    BeforeQty = 0,
                    PlanQty = (int)raw.PlanInQty,
                    ComplateQty = (int)raw.ActInQty,
                    Qty = raw.Qty
                };
                resultList.Add(newInventory);
            }


            return YL.Core.Dto.RouteData<InventoryDetailDto[]>.From(resultList.ToArray());
        }


        [HttpPost]
        public async Task<RouteData> DoInventoryBoxBack(int mode, long inventoryBoxTaskId,  InventoryDetailDto[] details)
        {
            try
            {
                Wms_inventoryboxTask task = _inventoryBoxTaskServices.QueryableToEntity(x => x.InventoryBoxTaskId == inventoryBoxTaskId);
                if (task == null) { return YL.Core.Dto.RouteData.From(PubMessages.E2007_STOCKINTASK_NOTFOUND); }
                if (task.Status < InventoryBoxTaskStatus.task_outed.ToByte()) { return YL.Core.Dto.RouteData.From(PubMessages.E2008_STOCKINTASK_NOTOUT); }
                if (task.Status == InventoryBoxTaskStatus.task_backing.ToByte()) { return YL.Core.Dto.RouteData.From(PubMessages.E2009_STOCKINTASK_ALLOW_BACKING); }
                if (task.Status == InventoryBoxTaskStatus.task_backed.ToByte()) { return YL.Core.Dto.RouteData.From(PubMessages.E2010_STOCKINTASK_ALLOW_BACKED); }

                Wms_inventorybox inventoryBox = _inventoryBoxServices.QueryableToEntity(x => x.InventoryBoxId == task.InventoryBoxId);
                if (inventoryBox == null) { return YL.Core.Dto.RouteData.From(PubMessages.E2003_INVENTORYBOX_NOTFOUND); }
                if (inventoryBox.Status != InventoryBoxStatus.Outed) { return YL.Core.Dto.RouteData.From(PubMessages.E2011_INVENTORYBOX_NOTOUTED); }

                List<Wms_inventory> inventories = _inventoryServices.QueryableToList(x => x.InventoryBoxId == task.InventoryBoxId);
                List<Wms_inventory> updatedInventories = new List<Wms_inventory>();

                Wms_stockindetail[] stockindetails = GetStockInDetails(inventoryBoxTaskId);
                List<Wms_stockindetail> updatedStockindetails = new List<Wms_stockindetail>();

                Wms_stockoutdetail[] stockoutdetails = GetStockOutDetails(inventoryBoxTaskId);
                List<Wms_stockoutdetail> updatedStockoutdetails = new List<Wms_stockoutdetail>();

                List<Wms_stockin> relationStockins = new List<Wms_stockin>();
                foreach (InventoryDetailDto detail in details)
                { 
                    Wms_stockindetail stockindetail = string.IsNullOrEmpty(detail.StockInDetailId) ? null : stockindetails.FirstOrDefault(x => x.StockInDetailId == detail.StockInDetailId.ToInt64());
                    Wms_stockoutdetail stockoutdetail = string.IsNullOrEmpty(detail.StockOutDetailId) ? null : stockoutdetails.FirstOrDefault(x => x.StockOutDetailId == detail.StockOutDetailId.ToInt64());
                    if (stockindetail == null && stockoutdetail == null)
                    {
                        return YL.Core.Dto.RouteData.From(PubMessages.E2019_INVENTORYBOXTASK_DETAILRELATION_NOTMATCH);
                    }

                    Wms_inventory inventory = string.IsNullOrEmpty(detail.InventoryId) ? null : inventories.FirstOrDefault(x => x.InventoryId == detail.InventoryId.ToInt64());
                    if (inventory == null)
                    {
                        inventory = new Wms_inventory()
                        {
                            Position = detail.InventoryPosition,
                            InventoryId = 0,
                            InventoryBoxId = inventoryBox.InventoryBoxId,
                            MaterialId = detail.MaterialId.ToInt64(),
                            Qty = 0,
                            IsDel = (byte)DeleteFlag.Normal, 
                            CreateBy = UserDto.UserId,
                            CreateDate = DateTime.Now
                        };
                    } 
                 
                    if (inventory.MaterialId.ToString() != detail.MaterialId)
                    {
                        _client.RollbackTran();
                        return YL.Core.Dto.RouteData.From(PubMessages.E2012_INVENTORYBOX_MATERIAL_NOTMATCH);
                    }
                    inventory.Qty += detail.Qty; //加上入库数量
                    inventory.ModifiedDate = DateTime.Now;
                    inventory.ModifiedBy = this.UserDto.UserId;

                    updatedInventories.Add(inventory);

                    if(stockindetail != null)
                    {
                        stockindetail.ActInQty += detail.Qty;
                        stockindetail.ModifiedDate = DateTime.Now;
                        stockindetail.ModifiedBy = this.UserDto.UserId;
                        if (stockindetail.ActInQty >= stockindetail.PlanInQty)
                        {
                            stockindetail.Status = StockInStatus.task_finish.ToByte();
                        }
                        updatedStockindetails.Add(stockindetail);
                        if(!relationStockins.Any(x => x.StockInId == stockindetail.StockInId))
                        {
                            Wms_stockin stockin = _client.Queryable<Wms_stockin>().First(x => x.StockInId == stockindetail.StockInId );
                            relationStockins.Add(stockin);
                        }
                    }

                    if (stockoutdetail != null)
                    {
                        stockoutdetail.ActOutQty += detail.Qty;
                        stockoutdetail.ModifiedDate = DateTime.Now;
                        stockoutdetail.ModifiedBy = this.UserDto.UserId;
                        if (stockoutdetail.ActOutQty >= stockoutdetail.PlanOutQty)
                        {
                            stockoutdetail.Status = StockOutStatus.task_finish.ToByte();
                        }
                        updatedStockoutdetails.Add(stockoutdetail);
                    }

                }
                if (!await SendWCSBackCommand(task))
                {
                    return YL.Core.Dto.RouteData.From(PubMessages.E2110_WCS_BACKCOMMAND_FAIL);
                }

                _client.BeginTran();

                foreach (Wms_inventory inventoriy in updatedInventories)
                {
                    if(inventoriy.InventoryId == 0)
                    {
                        inventoriy.InventoryId = PubId.SnowflakeId;
                        _inventoryServices.Insert(inventoriy); 
                    }
                    else
                    {
                        _inventoryServices.Update(inventoriy); 
                    }
                }
                foreach (Wms_stockindetail stockindetail in updatedStockindetails)
                {
                    if(_client.Updateable(stockindetail).ExecuteCommand() == 0)
                    {
                        _client.RollbackTran();
                        return YL.Core.Dto.RouteData.From(PubMessages.E0002_UPDATE_COUNT_FAIL);

                    }
                }
                foreach (Wms_stockoutdetail stockoutdetial in updatedStockoutdetails)
                {
                    if (_client.Updateable(stockoutdetial).ExecuteCommand() == 0)
                    {
                        _client.RollbackTran();
                        return YL.Core.Dto.RouteData.From(PubMessages.E0002_UPDATE_COUNT_FAIL);

                    }
                }
                task.Status = InventoryBoxTaskStatus.task_backing.ToByte();
                task.OperaterId = UserDto.UserId;
                task.OperaterDate = DateTime.Now; 
                _inventoryBoxTaskServices.UpdateEntity(task);

                inventoryBox.Status = InventoryBoxStatus.Backing;
                inventoryBox.ModifiedBy = UserDto.UserId;
                inventoryBox.ModifiedDate = DateTime.Now; 
                _inventoryBoxServices.UpdateEntity(inventoryBox);

                foreach(Wms_stockin stockin in relationStockins)
                {
                    if(_client.Queryable<Wms_stockindetail>().Any(x => x.Status != StockInStatus.task_finish.ToByte()))
                    {
                        //尚有未入库任务
                    }
                    else
                    {
                        stockin.StockInStatus = StockInStatus.task_finish.ToByte(); 
                        stockin.ModifiedBy = UserDto.UserId;
                        stockin.ModifiedDate = DateTime.Now;
                        if (_client.Updateable(stockin).ExecuteCommand() == 0)
                        {
                            _client.RollbackTran();
                            return YL.Core.Dto.RouteData.From(PubMessages.E0002_UPDATE_COUNT_FAIL);

                        }
                    }
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

        private Wms_stockindetail[] GetStockInDetails(long inventoryBoxTaskId)
        {
            return _client.Queryable<Wms_stockindetail,Wms_stockindetail_box>(
                (sid,sidb) => new object[] {
                    JoinType.Left,sid.StockInDetailId == sidb.StockinDetailId,
                })
                .Where((sid, sidb) => sidb.InventoryBoxTaskId == inventoryBoxTaskId)
                .ToArray();
        }
        private Wms_stockoutdetail[] GetStockOutDetails(long inventoryBoxTaskId)
        {
            return null;
        }


        private async Task<bool> SendWCSBackCommand(Wms_inventoryboxTask task)
        {
            try
            {
                Wms_storagerack storagerack = _client.Queryable<Wms_storagerack>().First(x => x.StorageRackId == task.StoragerackId);

                BackStockInfo backStockInfo = new BackStockInfo()
                {
                    TaskId = task.InventoryBoxTaskId.ToString(),
                    //GetColumn = storagerack.Column.ToString(),
                    //GetRow = storagerack.Row.ToString(),
                    //GetFloor = storagerack.Floor.ToString()
                };
                CreateBackStockResult result = await WCSApiAccessor.Instance.CreateBackStockTask(backStockInfo);
                return result.Successd;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
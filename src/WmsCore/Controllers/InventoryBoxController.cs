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
using Newtonsoft.Json;
using IServices.Outside;

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
        public async Task<IActionResult> Index()
        {
            long currentStoreId = (long)ViewData["currentStoreId"];
            IWMSApiProxy wmsAccessor = WMSApiManager.Get(currentStoreId.ToString(),_client);
            ViewBag.StorageRack = (await wmsAccessor.GetStorageRackList( null, 1, 100, null, null, null, null)).Data;
            //ViewBag.StorageRack = _storagerackServices.QueryableToList(c => c.WarehouseId == currentStoreId && c.IsDel == 1);
            return View();
        }

        [HttpPost]
        [OperationLog(LogType.select)]
        public async Task<string> List([FromForm]PubParams.InventoryBoxBootstrapParams bootstrap)
        {
            IWMSApiProxy wmsAccessor = WMSApiManager.Get(bootstrap.storeId.ToString(), _client);
            RouteData<Wms_inventorybox[]> result = (await wmsAccessor.GetInventoryBoxList(null,null,bootstrap.offset,bootstrap.limit,bootstrap.search,bootstrap.order.Split(","),bootstrap.datemin,bootstrap.datemax));
            if (!result.IsSccuess)
            {
                return new PageGridData().JilToJson();
            }
            return result.ToGridJson();
            //var sd = _inventoryBoxServices.PageList(bootstrap);
            //return Content(sd);
        }

        [HttpGet]
        public async Task<RouteData<Wms_InventoryBoxDto>> Get(string inventoryBoxNo)
        {
            throw new NotSupportedException();
            //Wms_inventorybox box = await _client.Queryable<Wms_inventorybox>().FirstAsync(x => x.InventoryBoxNo == inventoryBoxNo);
            //if(box == null)
            //{
            //    return RouteData<Wms_InventoryBoxDto>.From(PubMessages.E1011_INVENTORYBOX_NOTFOUND);
            //}
            //Wms_InventoryBoxDto dto = JsonConvert.DeserializeObject<Wms_InventoryBoxDto>( JsonConvert.SerializeObject(box));

            //var query = _client.Queryable<Wms_inventory, Wms_material, Sys_user>((i, m, u) => new object[] {
            //       JoinType.Left,i.MaterialId==m.MaterialId,
            //       JoinType.Left,i.ModifiedBy==u.UserId
            //     })
            //   .Where((i, m, u) => i.InventoryBoxId == box.InventoryBoxId)
            //   .Select((i, m, u) => new InventoryDetailDto
            //   {
            //       InventoryPosition = i.Position,
            //       MaterialId = m.MaterialId.ToString(),
            //       MaterialNo = m.MaterialNo,
            //       MaterialOnlyId = m.MaterialOnlyId,
            //       MaterialName = m.MaterialName,
            //       OrderNo = i.OrderNo,
            //       IsLocked = i.IsLocked,
            //       Qty = i.Qty,
            //   }).MergeTable();
            //dto.Id = box.InventoryBoxId.ToString();
            //dto.Details = (await query.ToListAsync()).ToArray();
            //return RouteData<Wms_InventoryBoxDto>.From(dto);
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
        public async Task<string> Search(long storeId, string text)
        { 
            IWMSApiProxy wmsAccessor = WMSApiManager.Get(storeId.ToString(), _client);
            RouteData<Wms_inventorybox[]> result = (await wmsAccessor.GetInventoryBoxList(null, null, 0, 20, text, null, null, null));
            if (!result.IsSccuess)
            {
                return new PageGridData().JilToJson();
            }
            return result.ToGridJson();
            //var bootstrap = new InventoryBoxBootstrapParams()
            //{
            //    limit = 100,
            //    offset = 0,
            //    sort = "CreateDate",
            //    search = text,
            //    order = "desc",
            //    storeId = storeId
            //};
            //var json = _inventoryBoxServices.PageList(bootstrap);
            //return Content(json);
        }


        [HttpGet]
        public IActionResult Detail(long storeId, string id)
        {
            ViewData["currentStoreId"] = storeId;
            long searchId = id.ToInt64(); 
            if (id.IsEmptyZero())
            {
                return Content("");
            }
            else
            {
                Wms_inventorybox box = _inventoryBoxServices.QueryableToEntity(c => c.InventoryBoxId == searchId && c.IsDel == 1);
                return View(box);
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
                    i.OrderNo,
                    i.IsLocked,
                    i.Qty, 
                    u.UserName,
                    i.ModifiedDate
                }).MergeTable();
                var list = await query.ToListAsync();
                return Bootstrap.GridData(list, list.Count).JilToJson();
            }
        } 

        [HttpGet]
        public async Task<RouteData<InventoryDetailDto[]>> BackDetailList(string id)
        {
            long searchId = id.ToInt64();
            if (id.IsEmptyZero())
            {
                return RouteData<InventoryDetailDto[]>.From( PubMessages.E1006_INVENTORYBOX_MISSING );
            }
            else
            {
                var query = _client.Queryable<Wms_inventory, Wms_material, Sys_user>((i, m, u) => new object[] {
                   JoinType.Left,i.MaterialId==m.MaterialId,
                   JoinType.Left,i.ModifiedBy==u.UserId
                 })
                .Where((i, m, u) => i.InventoryBoxId == searchId)
                .Select((i, m, u) => new InventoryDetailDto()
                {
                    InventoryPosition = i.Position,
                    MaterialId = m.MaterialId.ToString(),
                    MaterialNo = m.MaterialNo,
                    MaterialOnlyId = m.MaterialOnlyId,
                    MaterialName = m.MaterialName,
                    BeforeQty = i.Qty,
                    Qty = i.Qty,
                    InventoryBoxId = i.InventoryBoxId.ToString(),
                    InventoryId = i.InventoryId.ToString(),
                    OrderNo = i.OrderNo
                }).MergeTable();
                var list = await query.ToListAsync();
                return RouteData<InventoryDetailDto[]>.From(list.ToArray());
                //return Bootstrap.GridData(list, list.Count).JilToJson();
            }
        }
        

        [HttpGet]
        public IActionResult InventoryBoxOut(long storeId)
        {
            ViewData["currentStoreId"] = storeId;
            return View(null);
        }

        /// <summary>
        /// 自动选择料箱
        /// </summary>
        /// <param name="isfullbox"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<RouteData> DoAutoSelectBox(long storeId,bool isfullbox)
        {
            Wms_inventorybox inventoryBox = await _inventoryBoxServices.AutoSelectBox(isfullbox);
            if (inventoryBox == null && isfullbox == false)
            {
                inventoryBox = await _inventoryBoxServices.AutoSelectBox(true);
            }
            if (inventoryBox == null)
            {
                return YL.Core.Dto.RouteData.From(PubMessages.E1007_INVENTORYBOX_ALLOWUSED);
            }
            RouteData result = await DoInventoryBoxOut(inventoryBox.InventoryBoxId);
            return result;
        }


        /// <summary>
        /// 根据出库单自动出库料箱
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="stockOutId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<RouteData> DoStockOutBoxOut(long storeId, long stockOutId)
        {
            try
            {
                _client.BeginTran();
                RouteData result = await DoStockOutBoxOutCore(storeId, stockOutId);
                if (!result.IsSccuess)
                {
                    _client.RollbackTran();
                    return result;
                }
                _client.CommitTran();
                return result;
            }
            catch (Exception ex)
            {
                _client.RollbackTran();
                return YL.Core.Dto.RouteData.From(PubMessages.E2105_STOCKOUT_BOXOUT_FAIL, ex.Message);
            }

        }

        public async Task<RouteData> DoStockOutBoxOutCore(long storeId, long stockOutId)
        { 
            Wms_stockout stockout = await _client.Queryable<Wms_stockout>().FirstAsync(x => x.StockOutId == stockOutId);
            if (stockout == null) { return YL.Core.Dto.RouteData.From(PubMessages.E2113_STOCKOUT_NOTFOUND); }
            if (stockout.StockOutStatus == StockOutStatus.task_finish.ToByte()) { return YL.Core.Dto.RouteData.From(PubMessages.E2114_STOCKOUT_ALLOW_FINISHED); }

            if (stockout.StockOutStatus != StockOutStatus.task_working.ToByte())
            {
                stockout.StockOutStatus = StockOutStatus.task_working.ToByte();
                stockout.ModifiedBy = this.UserDto.UserId;
                stockout.ModifiedDate = DateTime.Now;
                if (_client.Updateable(stockout).ExecuteCommand() == 0)
                {
                    return YL.Core.Dto.RouteData.From(PubMessages.E2118_STOCKOUT_LOCK_FAIL, "出库单锁定状态更新失败");
                }
            }

            List<Wms_stockoutdetail> details = await _client.Queryable<Wms_stockoutdetail>().Where(x => x.StockOutId == stockout.StockOutId).ToListAsync();
            List<long> targetBoxIdList = new List<long>();
            foreach (Wms_stockoutdetail detail in details)
            {
                if (detail.Status == StockOutStatus.task_finish.ToByte()) continue;

                //TODO：尽量减少出库料箱数量
                //优先出库自身生产令号的物料
                Wms_inventory[] inventories = _client.Queryable<Wms_inventory>()
                    .Where(x => x.MaterialId == detail.MaterialId && (x.OrderNo == stockout.OrderNo || (string.IsNullOrEmpty(x.OrderNo) && !x.IsLocked)))
                    .OrderBy(x => x.OrderNo, OrderByType.Desc).ToArray();
                int outedQty = 0;
                int needQty = detail.PlanOutQty - detail.ActOutQty;
                foreach (Wms_inventory inventory in inventories)
                {
                    if (!targetBoxIdList.Contains(inventory.InventoryBoxId))
                    {
                        targetBoxIdList.Add(inventory.InventoryBoxId);
                    }
                    outedQty += inventory.Qty;
                    if (outedQty >= needQty)
                    {
                        //已锁定足够物料
                        break;
                    }
                }

            }
            int outCount = 0;
            foreach (long targetBoxId in targetBoxIdList)
            { 
                RouteData result = await DoInventoryBoxOutCore(targetBoxId);
                if (!result.IsSccuess )
                {
                    if (result.Code != PubMessages.E1012_INVENTORYBOX_NOTINPOSITION.Code)
                    {
                        return result;
                    }
                }
                else
                {
                    outCount++;
                }
            } 
            if(outCount == 0)
            {
                return YL.Core.Dto.RouteData.From(PubMessages.E2120_STOCKOUT_NOMORE_BOX);
            }
            return YL.Core.Dto.RouteData.From(PubMessages.I1001_BOXBACK_SCCUESS);

        }

        [HttpPost]
        public async Task<RouteData> DoInventoryBoxOut(long inventoryBoxId)
        {
            try
            {
                _client.BeginTran();

                RouteData result = await DoInventoryBoxOutCore(inventoryBoxId);
                if (!result.IsSccuess)
                { 
                    _client.RollbackTran();
                    return result;
                }
                _client.CommitTran();
                return result;
            }
            catch (Exception)
            {
                _client.RollbackTran();
                return YL.Core.Dto.RouteData.From(PubMessages.E2005_STOCKIN_BOXOUT_FAIL);
            }

        }

        private async Task<RouteData> DoInventoryBoxOutCore(long inventoryBoxId)
        {
            Wms_inventorybox inventoryBox = _inventoryBoxServices.QueryableToEntity(x => x.InventoryBoxId == inventoryBoxId);
            if (inventoryBox == null) { return YL.Core.Dto.RouteData.From(PubMessages.E1011_INVENTORYBOX_NOTFOUND); }
            if (inventoryBox.Status != InventoryBoxStatus.InPosition) { return YL.Core.Dto.RouteData.From(PubMessages.E1012_INVENTORYBOX_NOTINPOSITION); }

            Wms_inventoryboxTask task = _inventoryBoxTaskServices.QueryableToEntity(x => x.InventoryBoxId == inventoryBox.InventoryBoxId && x.Status == InventoryBoxStatus.Outed.ToByte());
            if (task != null)
            {
                return YL.Core.Dto.RouteData.From(PubMessages.E2301_WCS_STOCKOUT_NOTCOMPLATE_EXIST);
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
                return YL.Core.Dto.RouteData.From(PubMessages.E2300_WCS_OUTCOMMAND_FAIL);
            }

            _inventoryBoxTaskServices.Insert(task);

            inventoryBox.Status = InventoryBoxStatus.Outing;
            inventoryBox.ModifiedBy = UserDto.UserId;
            inventoryBox.ModifiedDate = DateTime.Now;
            _inventoryBoxServices.Update(inventoryBox);

            return YL.Core.Dto.RouteData.From(PubMessages.I2300_WCS_OUTCOMMAND_SCCUESS, "料箱:" + inventoryBox.InventoryBoxName);
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
        public async Task<IActionResult> BoxBack(long storeId, long inventoryBoxId)
        {
            Wms_inventorybox box = _inventoryBoxServices.QueryableToEntity(x => x.InventoryBoxId == inventoryBoxId);
            if (box == null)
            {
                return Json(YL.Core.Dto.RouteData.From(PubMessages.E1013_INVENTORYBOXTASK_NOTFOUND));
            }
            Wms_inventoryboxTask task = await _client.Queryable<Wms_inventoryboxTask>().FirstAsync(x => x.InventoryBoxId == inventoryBoxId
            && ( x.Status != (int)InventoryBoxTaskStatus.task_canceled && x.Status != (int)InventoryBoxTaskStatus.task_backed && x.Status != (int)InventoryBoxTaskStatus.task_leaved ));
            if (task == null)
            {
                return Json(YL.Core.Dto.RouteData.From(PubMessages.E1011_INVENTORYBOX_NOTFOUND)); 
            }
            if (await _client.Queryable<Wms_stockindetail_box>().AnyAsync(x => x.InventoryBoxId == task.InventoryBoxTaskId))
            {
                return Redirect($"/inventorybox/stockinboxback?storeId={storeId}&inventoryBoxTaskId=" + task.InventoryBoxTaskId);
            }
            else if (await _client.Queryable<Wms_stockindetail_box>().AnyAsync(x => x.InventoryBoxId == task.InventoryBoxTaskId))
            {
                return Redirect($"/inventorybox/stockoutboxback?storeId={storeId}&inventoryBoxTaskId=" + task.InventoryBoxTaskId);
            }
            else
            {
                ViewData["InventoryBoxTaskId"] = task.InventoryBoxTaskId;
                ViewData["currentStoreId"] = storeId;
                return View(box);

            } 
        }
         

        [HttpGet]
        public IActionResult StockInBoxBack(long storeId, long inventoryBoxTaskId)
        {
            Wms_inventoryboxTask task = _inventoryBoxTaskServices.QueryableToEntity(x => x.InventoryBoxTaskId == inventoryBoxTaskId);
            if (task == null)
            {
                return Json(YL.Core.Dto.RouteData.From(PubMessages.E1013_INVENTORYBOXTASK_NOTFOUND));
            }
            Wms_inventorybox box = _inventoryBoxServices.QueryableToEntity(x => x.InventoryBoxId == task.InventoryBoxId);
            if (box == null)
            {
                return Json(YL.Core.Dto.RouteData.From(PubMessages.E1011_INVENTORYBOX_NOTFOUND));
            }
            ViewData["currentStoreId"] = storeId;
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
                return YL.Core.Dto.RouteData<InventoryDetailDto[]>.From(PubMessages.E1013_INVENTORYBOXTASK_NOTFOUND);
            } 

            var query = _client.Queryable< Wms_stockindetail_box, Wms_stockindetail, Wms_stockin, Wms_material >(
                (sidb,sid, si,m) => new object[] { 
                   JoinType.Left,sidb.StockinDetailId==sid.StockInDetailId ,
                   JoinType.Left,sid.StockInId==si.StockInId ,
                   JoinType.Left,sid.MaterialId==m.MaterialId && m.IsDel == DeleteFlag.Normal
                  })
                 .Where((sidb, sid, si, m) => sidb.InventoryBoxTaskId == inventoryBoxTaskId)
                 .Select((sidb, sid, si, m) => new
                 {
                     sid.StockInDetailId,
                     MaterialId = m.MaterialId,
                     m.MaterialNo,
                     m.MaterialName,
                     si.OrderNo,
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
                    OrderNo = raw.OrderNo,
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
                var inventory = inventoryList.FirstOrDefault(x => x.MaterialId == null && string.IsNullOrEmpty( x.OrderNo) && !x.IsLocked);
                if(inventory == null)
                {
                    if(inventoryCount >= inventoryBoxTask.Size)
                    {
                        return YL.Core.Dto.RouteData<InventoryDetailDto[]>.From(PubMessages.E1010_INVENTORYBOX_BLOCK_OVERLOAD);
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

        [HttpGet]
        public IActionResult StockOutBoxBack(long inventoryBoxTaskId)
        {
            Wms_inventoryboxTask task = _inventoryBoxTaskServices.QueryableToEntity(x => x.InventoryBoxTaskId == inventoryBoxTaskId);
            if (task == null)
            {
                return Json(YL.Core.Dto.RouteData.From(PubMessages.E1013_INVENTORYBOXTASK_NOTFOUND));
            }
            Wms_inventorybox box = _inventoryBoxServices.QueryableToEntity(x => x.InventoryBoxId == task.InventoryBoxId);
            if (box == null)
            {
                return Json(YL.Core.Dto.RouteData.From(PubMessages.E1011_INVENTORYBOX_NOTFOUND));
            }
            ViewData["InventoryBoxTaskId"] = inventoryBoxTaskId;
            return View(box);
        }

        [HttpGet]
        public async Task<RouteData<InventoryDetailDto[]>> InventoryOutDetailList(long inventoryBoxTaskId)
        {
            var inventoryBoxTask = await _client.Queryable<Wms_inventoryboxTask, Wms_inventorybox>(
                (ibt, ib) => new object[] {
                   JoinType.Left,ibt.InventoryBoxId==ib.InventoryBoxId ,
                }
                )
                .Where((ibt, ib) => ibt.InventoryBoxTaskId == inventoryBoxTaskId)
                .Select((ibt, ib) => new {
                    ibt.InventoryBoxTaskId,
                    ibt.InventoryBoxId,
                    ib.InventoryBoxNo,
                    ib.InventoryBoxName,
                    ib.Size
                }).FirstAsync();

            if (inventoryBoxTask == null)
            {
                return YL.Core.Dto.RouteData<InventoryDetailDto[]>.From(PubMessages.E1013_INVENTORYBOXTASK_NOTFOUND);
            }

            var query = _client.Queryable<Wms_stockoutdetail_box, Wms_stockoutdetail, Wms_stockout, Wms_material>(
                (sidb, sid, so, m) => new object[] {
                   JoinType.Left,sidb.StockOutDetailId==sid.StockOutDetailId ,
                   JoinType.Left,sid.StockOutId==so.StockOutId ,
                   JoinType.Left,sid.MaterialId==m.MaterialId && m.IsDel == DeleteFlag.Normal
                  })
                 .Where((sidb, sid, so, m) => sidb.InventoryBoxTaskId == inventoryBoxTaskId)
                 .Select((sidb, sid, so, m) => new
                 {
                     sid.StockOutDetailId,
                     MaterialId = m.MaterialId,
                     m.MaterialNo,
                     m.MaterialName,
                     so.OrderNo,
                     PlanOutQty = sid.PlanOutQty,
                     ActOutQty = sid.ActOutQty,
                     Qty = sidb.Qty
                 }).MergeTable();

            var rawList = await query.ToListAsync();
            var inventoryList = _client.Queryable<Wms_inventory>().Where(x => x.InventoryBoxId == inventoryBoxTask.InventoryBoxId).ToArray();

            List<InventoryDetailDto> resultList = new List<InventoryDetailDto>();
            foreach (var inventory in inventoryList)
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
                    StockInDetailId = null,
                    StockOutDetailId = raw.StockOutDetailId.ToString(),
                    OrderNo = raw.OrderNo,
                    BeforeQty = (int)inventory.Qty,
                    PlanQty = (int)raw.PlanOutQty,
                    ComplateQty = (int)raw.ActOutQty,
                    Qty = raw.Qty
                };
                resultList.Add(newInventory);
                rawList.Remove(raw);
            }
            int inventoryCount = inventoryList.Length;
            foreach (var raw in rawList)
            {
                var inventory = inventoryList.FirstOrDefault(x => x.MaterialId == null);
                if (inventory == null)
                {
                    if (inventoryCount >= inventoryBoxTask.Size)
                    {
                        return YL.Core.Dto.RouteData<InventoryDetailDto[]>.From(PubMessages.E1010_INVENTORYBOX_BLOCK_OVERLOAD);
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
                    InventoryId = raw.StockOutDetailId.ToString(),
                    StockInDetailId = null,
                    StockOutDetailId = null,
                    BeforeQty = 0,
                    PlanQty = (int)raw.PlanOutQty,
                    ComplateQty = (int)raw.ActOutQty,
                    Qty = raw.Qty
                };
                resultList.Add(newInventory);
            }


            return YL.Core.Dto.RouteData<InventoryDetailDto[]>.From(resultList.ToArray());
        }

        /// <summary>
        /// 归库
        /// </summary>
        /// <param name="mode">1:基于入库单,2:基于出库单,3:手工</param>
        /// <param name="inventoryBoxTaskId"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<RouteData> DoInventoryBoxBack(int mode, long inventoryBoxTaskId,  InventoryDetailDto[] details)
        {
            try
            {
                Wms_inventoryboxTask task = _inventoryBoxTaskServices.QueryableToEntity(x => x.InventoryBoxTaskId == inventoryBoxTaskId);
                if (task == null) { return YL.Core.Dto.RouteData.From(PubMessages.E1013_INVENTORYBOXTASK_NOTFOUND); }
                if (task.Status < InventoryBoxTaskStatus.task_outed.ToByte()) { return YL.Core.Dto.RouteData.From(PubMessages.E1016_INVENTORYBOX_NOTOUTED); }
                if (task.Status == InventoryBoxTaskStatus.task_backing.ToByte()) { return YL.Core.Dto.RouteData.From(PubMessages.E1017_INVENTORYBOX_ALLOW_BACKING); }
                if (task.Status == InventoryBoxTaskStatus.task_backed.ToByte()) { return YL.Core.Dto.RouteData.From(PubMessages.E1018_INVENTORYBOX_ALLOW_BACKED); }

                Wms_inventorybox inventoryBox = _inventoryBoxServices.QueryableToEntity(x => x.InventoryBoxId == task.InventoryBoxId);
                if (inventoryBox == null) { return YL.Core.Dto.RouteData.From(PubMessages.E1011_INVENTORYBOX_NOTFOUND); }
                if (inventoryBox.Status != InventoryBoxStatus.Outed) { return YL.Core.Dto.RouteData.From(PubMessages.E1014_INVENTORYBOX_NOTOUTED); }

                List<Wms_inventory> inventories = _inventoryServices.QueryableToList(x => x.InventoryBoxId == task.InventoryBoxId);
                List<Wms_inventory> updatedInventories = new List<Wms_inventory>();

                Wms_stockindetail[] stockindetails = mode != 3 ? GetStockInDetails(inventoryBoxTaskId) : new Wms_stockindetail[0];
                List<Wms_stockindetail> updatedStockindetails = new List<Wms_stockindetail>();

                Wms_stockoutdetail[] stockoutdetails = mode != 3 ? GetStockOutDetails(inventoryBoxTaskId) : new Wms_stockoutdetail[0];
                List<Wms_stockoutdetail> updatedStockoutdetails = new List<Wms_stockoutdetail>();

                List<Wms_stockin> relationStockins = new List<Wms_stockin>(); 
                foreach (InventoryDetailDto detail in details)
                { 
                    Wms_stockindetail stockindetail = string.IsNullOrEmpty(detail.StockInDetailId) ? null : stockindetails.FirstOrDefault(x => x.StockInDetailId == detail.StockInDetailId.ToInt64());
                    Wms_stockoutdetail stockoutdetail = string.IsNullOrEmpty(detail.StockOutDetailId) ? null : stockoutdetails.FirstOrDefault(x => x.StockOutDetailId == detail.StockOutDetailId.ToInt64());
                    if (mode == 1 && stockindetail == null )
                    {
                        _client.RollbackTran();
                        return YL.Core.Dto.RouteData.From(PubMessages.E2016_STOCKINDETAIL_INVENTORYBOXTASK_NOTMATCH);
                    }
                    else if(mode == 2 && stockoutdetail == null)
                    {
                        _client.RollbackTran();
                        return YL.Core.Dto.RouteData.From(PubMessages.E2116_STOCKOUTDETAIL_INVENTORYBOXTASK_NOTMATCH);
                    }
                    int? beforeQty = null;
                    int qty = 0;
                    int afterQty = 0;
                    Wms_inventory inventory = string.IsNullOrEmpty(detail.InventoryId) ? null : inventories.FirstOrDefault(x => x.InventoryId == detail.InventoryId.ToInt64());
                    if (inventory == null)
                    {
                        inventory = new Wms_inventory()
                        {
                            Position = detail.InventoryPosition,
                            InventoryId = 0,
                            InventoryBoxId = inventoryBox.InventoryBoxId,
                            MaterialId = detail.MaterialId.ToInt64(),
                            OrderNo = detail.OrderNo,
                            Qty = 0,
                            IsLocked = false, 
                            IsDel = (byte)DeleteFlag.Normal, 
                            CreateBy = UserDto.UserId,
                            CreateDate = DateTime.Now
                        };
                        inventories.Add(inventory);
                    }
                    else
                    {
                        beforeQty = inventory.Qty;
                    }
                    if (inventory.MaterialId.ToString() != detail.MaterialId)
                    {
                        _client.RollbackTran();
                        return YL.Core.Dto.RouteData.From(PubMessages.E1015_INVENTORYBOX_MATERIAL_NOTMATCH);
                    }
                    if (inventory.IsLocked && detail.OrderNo != inventory.OrderNo)
                    {
                        _client.RollbackTran();
                        return YL.Core.Dto.RouteData.From(PubMessages.E1019_INVENTORY_LOCKED , $"料箱编号{inventoryBox.InventoryBoxNo},物料编号{detail.MaterialNo}");
                    }
                    if (mode == 1)
                    {
                        inventory.Qty += detail.Qty; //加上入库数量
                        qty = detail.Qty;
                    }
                    else if (mode == 2)
                    {
                        inventory.Qty -= detail.Qty; //减去出库数量
                        qty -= detail.Qty;
                    }
                    else
                    {
                        qty = detail.Qty - inventory.Qty;
                        inventory.Qty = detail.Qty; //手动出入库，直接填入最终数量
                    }
                    afterQty = inventory.Qty;
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

                    Wms_inventoryrecord record = new Wms_inventoryrecord()
                    {
                        InventoryrecordId = PubId.SnowflakeId,
                        StockInDetailId = stockindetail.StockInDetailId,
                        InventoryBoxId = inventoryBox.InventoryBoxId,
                        InventoryBoxNo = inventoryBox.InventoryBoxNo,
                        InventoryId = inventory.InventoryId,
                        InventoryPosition = inventory.Position,
                        MaterialId = detail.MaterialId.ToInt64(),
                        MaterialName = detail.MaterialName,
                        BeforeQty = beforeQty,
                        Qty = qty,
                        AfterQty = afterQty,
                        CreateBy = this.UserDto.UserId,
                        CreateDate = DateTime.Now,
                        IsDel = DeleteFlag.Normal
                    };
                    if(_client.Insertable(record).ExecuteCommand() == 0)
                    {
                        _client.RollbackTran();
                        return YL.Core.Dto.RouteData.From(PubMessages.E1021_INVENTORYRECORD_FAIL);
                    }



                }
                if (mode != 4)
                {
                    if (!await SendWCSBackCommand(task))
                    {
                        return YL.Core.Dto.RouteData.From(PubMessages.E2310_WCS_BACKCOMMAND_FAIL);
                    }
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
                task.Status = mode == 4 ? InventoryBoxTaskStatus.task_leaved.ToByte() : InventoryBoxTaskStatus.task_backing.ToByte();
                task.OperaterId = UserDto.UserId;
                task.OperaterDate = DateTime.Now; 
                _inventoryBoxTaskServices.UpdateEntity(task);

                inventoryBox.UsedSize = inventories.Count;
                inventoryBox.Status = mode == 4 ? InventoryBoxStatus.None : InventoryBoxStatus.Backing;
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
                return YL.Core.Dto.RouteData.From(PubMessages.I2000_STOCKOUT_SCAN_SCCUESS);
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
            return _client.Queryable<Wms_stockoutdetail, Wms_stockoutdetail_box>(
                (sid, sidb) => new object[] {
                JoinType.Left,sid.StockOutDetailId == sidb.StockOutDetailId,
                })
                .Where((sid, sidb) => sidb.InventoryBoxTaskId == inventoryBoxTaskId)
                .ToArray();
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
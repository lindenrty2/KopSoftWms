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
        //private readonly IWms_inventoryBoxServices _inventoryBoxServices;
        //private readonly IWms_inventoryBoxTaskServices _inventoryBoxTaskServices;
        //private readonly IWms_inventoryServices _inventoryServices;
        //private readonly IWms_storagerackServices _storagerackServices;

        private readonly SqlSugarClient _client; 
        public InventoryBoxController(
            //IWms_storagerackServices storagerackServices,
            //IWms_inventoryBoxServices inventoryBoxServices,
            //IWms_inventoryBoxTaskServices inventoryBoxTaskServices,
            //IWms_inventoryServices inventoryServices,
            SqlSugarClient client
            )
        {
            _client = client;
            //_storagerackServices = storagerackServices;
            //_inventoryBoxServices = inventoryBoxServices;
            //_inventoryBoxTaskServices = inventoryBoxTaskServices;
            //_inventoryServices = inventoryServices;
        }

        [HttpGet]
        [CheckMenu]
        public async Task<IActionResult> Index(string storeId)
        {
            long currentStoreId = (long)ViewData["currentStoreId"];
            IWMSBaseApiAccessor wmsAccessor = WMSApiManager.GetBaseApiAccessor(storeId, _client);
            RouteData<Wms_reservoirarea[]> result = (await wmsAccessor.GetReservoirAreaList(1, 100, null, null, null, null));
            ViewData["reservoirAreaList"] = result.Data;
            //ViewBag.StorageRack = _storagerackServices.QueryableToList(c => c.WarehouseId == currentStoreId && c.IsDel == 1);
            return View();
        }

        [HttpPost]
        [OperationLog(LogType.select)]
        public async Task<string> List([FromForm]PubParams.InventoryBoxBootstrapParams bootstrap)
        {
            IWMSBaseApiAccessor wmsAccessor = WMSApiManager.GetBaseApiAccessor(bootstrap.storeId.ToString(), _client);
            RouteData<Wms_inventorybox[]> result = (await wmsAccessor.GetInventoryBoxList(
                bootstrap.ReservoirAreaId, null, bootstrap.Status, bootstrap.pageIndex,bootstrap.limit,bootstrap.search,
                new string[] { bootstrap.sort + " " + bootstrap.order },
                bootstrap.datemin,bootstrap.datemax));
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
            Wms_inventorybox box = await _client.Queryable<Wms_inventorybox>().FirstAsync(x => x.InventoryBoxNo == inventoryBoxNo);
            if (box == null)
            {
                return RouteData<Wms_InventoryBoxDto>.From(PubMessages.E1011_INVENTORYBOX_NOTFOUND);
            }
            Wms_InventoryBoxDto dto = JsonConvert.DeserializeObject<Wms_InventoryBoxDto>(JsonConvert.SerializeObject(box));

            var query = _client.Queryable<Wms_inventory, Wms_material, Sys_user>((i, m, u) => new object[] {
                   JoinType.Left,i.MaterialId==m.MaterialId,
                   JoinType.Left,i.ModifiedBy==u.UserId
                 })
               .Where((i, m, u) => i.InventoryBoxId == box.InventoryBoxId)
               .Select((i, m, u) => new InventoryDetailDto
               {
                   InventoryPosition = i.Position,
                   MaterialId = m.MaterialId.ToString(),
                   MaterialNo = m.MaterialNo,
                   MaterialOnlyId = m.MaterialOnlyId,
                   MaterialName = m.MaterialName,
                   OrderNo = i.OrderNo,
                   IsLocked = i.IsLocked,
                   Qty = i.Qty,
               }).MergeTable();
            dto.Id = box.InventoryBoxId.ToString();
            dto.Details = (await query.ToListAsync()).ToArray();
            return RouteData<Wms_InventoryBoxDto>.From(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Add(long storeId,string id)
        {
            ViewData["currentStoreId"] = storeId;
            var model = new Wms_inventorybox();
            if (id.IsEmpty())
            {
                return View(model);
            }
            else
            {
                IWMSBaseApiAccessor wmsAccessor = WMSApiManager.GetBaseApiAccessor(storeId.ToString(), _client);
                RouteData<Wms_inventorybox> result = await wmsAccessor.GetInventoryBox(SqlFunc.ToInt64(id));
                return View(result.Data);
            }
        }

        [HttpPost]
        [FilterXss]
        [OperationLog(LogType.addOrUpdate)]
        public async Task <IActionResult> AddOrUpdate([FromForm]Wms_inventorybox model, [FromForm]string id,[FromForm]string storeId)
        {
            var validator = new InventoryBoxFluent();
            var results = validator.Validate(model);
            var success = results.IsValid;
            if (!success)
            {
                string msg = results.Errors.Aggregate("", (current, item) => (current + item.ErrorMessage + "</br>"));
                return BootJsonH((PubEnum.Failed.ToInt32(), msg));
            }

            IWMSManagementApiAccessor wmsAccessor = WMSApiManager.GetManagementApiAccessor(storeId.ToString(), _client,this.UserDto);
            RouteData routeData = null;
            if (id.IsEmptyZero())
            {
                routeData = await wmsAccessor.AddInventoryBox(model); 
            }
            else
            {
                routeData = await wmsAccessor.UpdateInventoryBox(SqlFunc.ToInt64(id), model);
            }

            return BootJsonH((routeData.IsSccuess, routeData.Message));
        }

        [HttpGet]
        [OperationLog(LogType.delete)]
        public async Task<IActionResult> Delete(int storeId,string id)
        {
            IWMSManagementApiAccessor wmsAccessor = WMSApiManager.GetManagementApiAccessor(storeId.ToString(), _client, this.UserDto);
            RouteData routeData = await wmsAccessor.DeleteInventoryBox(SqlFunc.ToInt64(id));

             
            return BootJsonH((routeData.IsSccuess,routeData.Message));
        }

        [HttpGet]
        public async Task<string> Search(long storeId, string text)
        {
            IWMSBaseApiAccessor wmsAccessor = WMSApiManager.GetBaseApiAccessor(storeId.ToString(), _client);
            RouteData<Wms_inventorybox[]> result = (await wmsAccessor.GetInventoryBoxList(
                null, null, null, 1, 20, text, null, null, null));
            if (!result.IsSccuess)
            {
                return new PageGridData().JilToJson();
            }
            return result.ToGridJson();
        }
        
        [HttpGet]
        public async Task<IActionResult> Detail(long storeId, long id)
        { 
            IWMSBaseApiAccessor wmsAccessor = WMSApiManager.GetBaseApiAccessor(storeId.ToString(), _client); 
            if (id == 0)
            {
                return Content("调用错误");
            }
            RouteData<Wms_inventorybox> result = (await wmsAccessor.GetInventoryBox(id));
            if (!result.IsSccuess)
            {
                return Content(result.Message); 
            }
            ViewData["currentStoreId"] = storeId;
            return View(result.Data);
        }

        [HttpGet]
        public async Task<string> DetailList(long storeId, long id)
        { 
            if (id == 0)
            {
                return PubMessages.E1006_INVENTORYBOX_MISSING.Message;
            }

            IWMSBaseApiAccessor wmsAccessor = WMSApiManager.GetBaseApiAccessor(storeId.ToString(), _client);
            RouteData<Wms_inventory[]> result = await wmsAccessor.GetInventoryBoxDetail(id);
            if (!result.IsSccuess)
            {
                return result.Message; 
            }
            return result.ToGridJson();
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
                    OrderNo = i.OrderNo, 
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
        /// <param name="requestSize">期望料箱的格数</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<RouteData> DoAutoSelectBox(long storeId,long? reservoirAreaId,int requestSize, PLCPosition pos)
        {
            IWMSOperationApiAccessor wmsAccessor = WMSApiManager.GetOperationApiAccessor(storeId.ToString(), _client, this.UserDto);
            RouteData result = await wmsAccessor.DoAutoSelectBoxOut(reservoirAreaId, requestSize, pos); 

            return result;
        }


        /// <summary>
        /// 根据出库单自动出库料箱
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="stockOutId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<RouteData> DoStockOutBoxOut(long storeId, long stockOutId, PLCPosition pos)
        {
            IWMSOperationApiAccessor wmsAccessor = WMSApiManager.GetOperationApiAccessor(storeId.ToString(), _client, this.UserDto);
            RouteData result = await wmsAccessor.DoStockOutBoxOut(stockOutId, pos);

            return result;
        }

        /// <summary>
        /// 出库指定料箱
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="inventoryBoxId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<RouteData> DoInventoryBoxOut(long storeId, long inventoryBoxId, PLCPosition pos)
        {
            IWMSOperationApiAccessor wmsAccessor = WMSApiManager.GetOperationApiAccessor(storeId.ToString(), _client, this.UserDto);
            RouteData result = await wmsAccessor.DoInventoryBoxOut(inventoryBoxId, pos);

            return result;

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
            IWMSBaseApiAccessor wmsAccessor = WMSApiManager.GetBaseApiAccessor(storeId.ToString(), _client, this.UserDto); 
            RouteData<Wms_inventorybox> result = await wmsAccessor.GetInventoryBox(inventoryBoxId);
            if (!result.IsSccuess)
            { 
                return Json(result);
            }
            Wms_inventorybox box = result.Data; 
            Wms_inventoryboxTask task = await _client.Queryable<Wms_inventoryboxTask>().FirstAsync(x => x.InventoryBoxId == inventoryBoxId
                && ( x.Status != (int)InventoryBoxTaskStatus.task_canceled 
                && x.Status != (int)InventoryBoxTaskStatus.task_backed  ));
            if (task == null) //新料箱入库
            {
                if (box.Status == InventoryBoxStatus.None)
                {
                    task = new Wms_inventoryboxTask() {
                        InventoryBoxTaskId = PubId.SnowflakeId,
                        InventoryBoxId = box.InventoryBoxId,
                        InventoryBoxNo = box.InventoryBoxNo,
                        ReservoirareaId = (long)box.ReservoirAreaId,
                        StoragerackId = (long)box.StorageRackId,
                        Data = null,
                        OperaterDate = DateTime.Now,
                        OperaterId = UserDto.UserId,
                        OperaterUser = UserDto.UserName,
                        Status = InventoryBoxTaskStatus.task_leaved.ToByte()
                    };

                    if(_client.Insertable(task).ExecuteCommand() == 0 ) { 
                        return Json(YL.Core.Dto.RouteData.From(PubMessages.E0005_DATABASE_INSERT_FAIL, "新料箱入库时任务记录生成失败"));
                    }
                }
                else {
                    return Json(YL.Core.Dto.RouteData.From(PubMessages.E1013_INVENTORYBOXTASK_NOTFOUND)); 
                }
            }
            else if (await _client.Queryable<Wms_stockindetail_box>().AnyAsync(x => x.InventoryBoxId == task.InventoryBoxTaskId))
            {
                return Redirect($"/inventorybox/stockinboxback?storeId={storeId}&inventoryBoxTaskId=" + task.InventoryBoxTaskId);
            }
            else if (await _client.Queryable<Wms_stockoutdetail_box>().AnyAsync(x => x.InventoryBoxId == task.InventoryBoxTaskId))
            {
                return Redirect($"/inventorybox/stockoutboxback?storeId={storeId}&inventoryBoxTaskId=" + task.InventoryBoxTaskId);
            }
            ViewData["InventoryBoxTaskId"] = task.InventoryBoxTaskId;
            ViewData["currentStoreId"] = storeId;
            return View(box);
        }
         

        [HttpGet]
        public async Task<IActionResult> StockInBoxBack(long storeId, long inventoryBoxTaskId)
        {
            IWMSOperationApiAccessor wmsAccessor = WMSApiManager.GetOperationApiAccessor(storeId.ToString(), _client, this.UserDto);
            RouteData<Wms_inventoryboxTask> result = await wmsAccessor.GetInventoryBoxkTask(inventoryBoxTaskId);
            if (!result.IsSccuess)
            {
                return Json(result.Message);
            }
            IWMSBaseApiAccessor wmsBaseAccessor = WMSApiManager.GetBaseApiAccessor(storeId.ToString(), _client, this.UserDto);
            RouteData<Wms_inventorybox> boxResult = await wmsBaseAccessor.GetInventoryBox(result.Data.InventoryBoxId);
            if (!boxResult.IsSccuess)
            {
                return Json(boxResult.Message);
            }

            ViewData["currentStoreId"] = storeId;
            ViewData["InventoryBoxTaskId"] = inventoryBoxTaskId;
            return View(boxResult.Data);
        }

        [HttpGet]
        public async Task<RouteData<InventoryDetailDto[]>> InventoryInDetailList(long storeId,long inventoryBoxTaskId)
        {
            IWMSOperationApiAccessor wmsAccessor = WMSApiManager.GetOperationApiAccessor(storeId.ToString(), _client, this.UserDto);
            RouteData<InventoryDetailDto[]> result = await wmsAccessor.InventoryInDetailList(inventoryBoxTaskId);

            return result;

        }

        [HttpGet]
        public async Task<IActionResult> StockOutBoxBack(long storeId, long inventoryBoxTaskId)
        {
            IWMSOperationApiAccessor wmsAccessor = WMSApiManager.GetOperationApiAccessor(storeId.ToString(), _client, this.UserDto);
            RouteData<Wms_inventoryboxTask> result = await wmsAccessor.GetInventoryBoxkTask(inventoryBoxTaskId);

            IWMSBaseApiAccessor wmsBaseAccessor = WMSApiManager.GetBaseApiAccessor(storeId.ToString(), _client, this.UserDto);
            RouteData<Wms_inventorybox> boxResult = await wmsBaseAccessor.GetInventoryBox(result.Data.InventoryBoxId);
            if (!boxResult.IsSccuess)
            {
                return Json(boxResult.Message);
            }

            ViewData["currentStoreId"] = storeId;
            ViewData["InventoryBoxTaskId"] = inventoryBoxTaskId;
            return View(boxResult.Data);
        }

        [HttpGet]
        public async Task<RouteData<InventoryDetailDto[]>> InventoryOutDetailList(long storeId, long inventoryBoxTaskId)
        {
            IWMSOperationApiAccessor wmsAccessor = WMSApiManager.GetOperationApiAccessor(storeId.ToString(), _client, this.UserDto);
            RouteData<InventoryDetailDto[]> result = await wmsAccessor.InventoryOutDetailList(inventoryBoxTaskId); 

            return result;
        }

        /// <summary>
        /// 归库
        /// </summary>
        /// <param name="mode">1:基于入库单,2:基于出库单,3:手工,4出库任务料箱离库</param>
        /// <param name="inventoryBoxTaskId"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<RouteData> DoInventoryBoxBack(long storeId,StockOperation mode, long inventoryBoxTaskId,  InventoryDetailDto[] details ,PLCPosition pos )
        {
            IWMSOperationApiAccessor wmsAccessor = WMSApiManager.GetOperationApiAccessor(storeId.ToString(), _client, this.UserDto);
            RouteData result = await wmsAccessor.DoInventoryBoxBack(mode, inventoryBoxTaskId, details, pos);

            return result;
        }

         
    }
}
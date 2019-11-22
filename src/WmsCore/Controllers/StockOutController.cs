using IServices;
using IServices.Outside;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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

namespace KopSoftWms.Controllers
{
    public class StockOutController : BaseController
    {
        private readonly ISys_dictServices _dictServices;
        private readonly IWms_CustomerServices _customerServices;
        private readonly IWms_stockoutServices _stockoutServices;
        private readonly ISys_serialnumServices _serialnumServices;
        private readonly IWms_stockoutdetailServices _stockoutdetailServices; 
        private readonly IWms_stockoutdetailboxServices _stockoutdetailboxServices;
        private readonly IWms_inventoryBoxServices _inventoryBoxServices;
        private readonly IWms_inventoryBoxTaskServices _inventoryBoxTaskServices;
        private readonly IWms_inventoryServices _inventoryServices; 
        private readonly SqlSugarClient _client;

        public StockOutController(
            ISys_dictServices dictServices,
            IWms_CustomerServices customerServices,
            IWms_stockoutServices stockoutServices,
            ISys_serialnumServices serialnumServices,
            IWms_stockoutdetailServices stockoutdetailServices, 
            IWms_stockoutdetailboxServices stockoutdetailboxServices,
            IWms_inventoryBoxServices inventoryBoxServices,
            IWms_inventoryBoxTaskServices inventoryBoxTaskServices,
            IWms_inventoryServices inventoryServices,
            SqlSugarClient client
            )
        {
            _dictServices = dictServices;
            _customerServices = customerServices;
            _stockoutServices = stockoutServices;
            _serialnumServices = serialnumServices;
            _stockoutdetailServices = stockoutdetailServices;
            _stockoutdetailboxServices = stockoutdetailboxServices;
            _inventoryBoxServices = inventoryBoxServices;
            _inventoryBoxTaskServices = inventoryBoxTaskServices;
            _inventoryServices = inventoryServices;
            _client = client;
        }

        [HttpGet]
        [CheckMenu]
        public IActionResult Index()
        {
            var list = _dictServices.Queryable().Where(c => c.IsDel == 1 && c.DictType == PubDictType.stockout.ToString()).ToList();
            var stockOutStatus = EnumExt.ToKVListLinq<StockOutStatus>();
            ViewBag.StockOutType = list;
            ViewBag.StockOutStatus = stockOutStatus;
            return View();
        }

        [HttpGet]
        public async Task<string> Search(string storeId,string text)
        {
            //var bootstrap = new Bootstrap.BootstrapParams
            //{
            //    limit = 100,
            //    offset = 0,
            //    sort = "CreateDate",
            //    search = text,
            //    order = "desc"
            //};
            //var json = _customerServices.PageList(bootstrap);
            //return Content(json);

            IWMSApiProxy wmsAccessor = WMSApiManager.Get(storeId.ToString(), _client);
            RouteData<OutsideStockOutQueryResult[]> result = await wmsAccessor.QueryStockOutList(null, null, 1, 20, text, new string[0], null, null);
            if (!result.IsSccuess)
            {
                return new PageGridData().JilToJson();
            }
            return result.ToGridJson();
        }

        [HttpGet]
        public IActionResult SearchInventory(string id, string storagerackId)
        {
            var bootstrap = new PubParams.InventoryBootstrapParams
            {
                limit = 100,
                offset = 0,
                sort = "CreateDate",
                search = id,
                order = "desc",
                StorageRackId = storagerackId,
            };
            var json = _inventoryServices.SearchInventory(bootstrap);
            return Content(json);
        }

        [HttpGet]
        public async Task<RouteData<Wms_StockOutDto>> Get(long storeId,string no)
        {
            Wms_stockout model = await _client.Queryable<Wms_stockout>().FirstAsync(c => c.StockOutNo == no && c.WarehouseId == storeId && c.IsDel == 1);
            if (model == null)
            {
                RouteData<Wms_stockin>.From(PubMessages.E2013_STOCKIN_NOTFOUND);
            }
            Wms_StockOutDto dto = JsonConvert.DeserializeObject<Wms_StockOutDto>(JsonConvert.SerializeObject(model));
            List<Wms_StockMaterialDetailDto> details = await _client.Queryable<Wms_stockoutdetail, Wms_material>
                ((sid, m) => new object[] {
                   JoinType.Left,sid.MaterialId==m.MaterialId,
                })
                .Where((sid, m) => sid.StockOutId == model.StockOutId)
                .Select((sid, m) => new Wms_StockMaterialDetailDto()
                {
                    StockId = sid.StockOutId.ToString(),
                    StockDetailId = sid.StockOutDetailId.ToString(),
                    MaterialId = m.MaterialId.ToString(),
                    MaterialNo = m.MaterialNo,
                    MaterialName = m.MaterialName,
                    PlanQty = (int)sid.PlanOutQty * -1,
                    ActQty = (int)sid.ActOutQty * -1,
                    Qty = 0
                })
                .MergeTable()
                .ToListAsync();

            dto.Details = details.ToArray();
            return RouteData<Wms_StockOutDto>.From(dto);
        }

        [HttpGet]
        public IActionResult Add(long id, long storeId)
        {
            var model = new Wms_stockout();
            if (id == 0)
            {
                model.WarehouseId = storeId;
                return View(model);
            }
            else
            {
                model = _stockoutServices.QueryableToEntity(c => c.StockOutId == SqlFunc.ToInt64(id) && c.IsDel == 1);
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Detail(string id, string pid, long storeId)
        {
            ViewData["currentStoreId"] = storeId;
            var model = new Wms_stockoutdetail();
            if (id.IsEmptyZero())
            {
                model.StockOutId = pid.ToInt64();
                return View(model);
            }

            model = _stockoutdetailServices.QueryableToEntity(c => c.StockOutDetailId == SqlFunc.ToInt64(id) && c.IsDel == 1);
            return View(model);
        }

        /// <summary>
        /// 主表
        /// </summary>
        /// <param name="bootstrap">参数</param>
        /// <returns></returns>
        [HttpPost]
        [OperationLog(LogType.select)]
        public async Task<string> List([FromForm]PubParams.StockOutBootstrapParams bootstrap)
        {
            //var sd = _stockoutServices.PageList(bootstrap);
            //return Content(sd);

            IWMSApiProxy wmsAccessor = WMSApiManager.Get(bootstrap.storeId.ToString(), _client);
            RouteData<OutsideStockOutQueryResult[]> result = await wmsAccessor.QueryStockOutList(null, null, bootstrap.pageIndex, bootstrap.limit, bootstrap.search, bootstrap.order.Split(","), bootstrap.datemin, bootstrap.datemax);
            if (!result.IsSccuess)
            {
                return new PageGridData().JilToJson();
            }
            return result.ToGridJson();
        }

        /// <summary>
        /// 明细
        /// </summary>
        /// <param name="id">主表id</param>
        /// <returns></returns>
        [HttpPost]
        [OperationLog(LogType.select)]
        public async Task<string> ListDetail(int storeId,string pid)
        {
            //var sd = _stockoutdetailServices.PageList(pid);
            //return Content(sd);

            IWMSApiProxy wmsAccessor = WMSApiManager.Get(storeId.ToString(), _client);
            RouteData<OutsideStockOutQueryResult> result = await wmsAccessor.QueryStockOut(SqlFunc.ToInt64(pid));
            if (!result.IsSccuess)
            {
                return null;
            }
            return new PageGridData(result.Data.Details, result.Data.Details.Length).JilToJson();
        }

        
        [HttpGet]
        public IActionResult Work(string pid)
        {
            var model = _stockoutServices.QueryableToEntity(c => c.StockOutId == SqlFunc.ToInt64(pid) && c.IsDel == 1);
            ViewData["currentStoreId"] = model.WarehouseId;
            return View(model);
        }

        [HttpGet]
        public ContentResult WorkList(string pid)
        {
            var workList = _stockoutdetailServices.PageList(pid); 
            return Content(workList);
        }


        [HttpGet]
        public IActionResult ScanPage(long storeId,long stockOutId)
        {
            ViewData["currentStoreId"] = storeId;
            var model = _stockoutServices.QueryableToEntity(c => c.StockOutId == SqlFunc.ToInt64(stockOutId) && c.IsDel == 1);
            return View(model);
        }

        [HttpPost]
        public async Task<RouteData> DoScanComplate(long stockOutId,long inventoryBoxId, Wms_StockMaterialDetailDto[] materials,string remark)
        {
            try
            {
                _client.BeginTran();
                RouteData result = await _stockoutServices.DoScanComplate(stockOutId, inventoryBoxId, materials, remark, this.UserDto);
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
                return YL.Core.Dto.RouteData.From(PubMessages.E2105_STOCKOUT_BOXOUT_FAIL);
            } 
        }

        [HttpPost]
        public async Task<RouteData> DoLock(long storeId,long stockOutId)
        {
            try
            {
                _client.BeginTran();

                Wms_stockout stockout = _stockoutServices.QueryableToEntity(x => x.StockOutId == stockOutId);
                if (stockout == null) { return YL.Core.Dto.RouteData.From(PubMessages.E2113_STOCKOUT_NOTFOUND); }
                if (stockout.StockOutStatus == StockOutStatus.task_finish.ToByte()) { return YL.Core.Dto.RouteData.From(PubMessages.E2114_STOCKOUT_ALLOW_FINISHED); }
                if (stockout.IsLocked) { return YL.Core.Dto.RouteData.From(PubMessages.E2121_STOCKOUT_ALLOW_LOCKED); }

                stockout.IsLocked = true;
                if (!_stockoutServices.Update(stockout))
                {
                    return YL.Core.Dto.RouteData.From(PubMessages.E2118_STOCKOUT_LOCK_FAIL, "出库单锁定状态更新失败");
                }

                List<Wms_stockoutdetail> details = _stockoutdetailServices.QueryableToList(x => x.StockOutId == stockout.StockOutId);
                foreach (Wms_stockoutdetail detail in details)
                {
                    if (detail.Status == StockOutStatus.task_finish.ToByte()) continue;

                    //优先锁定自身生产令号的物料
                    Wms_inventory[] inventories = _client.Queryable<Wms_inventory>()
                        .Where( x => x.MaterialId == detail.MaterialId && !x.IsLocked && (x.OrderNo == stockout.OrderNo || string.IsNullOrEmpty(x.OrderNo)))
                        .OrderBy(x => x.OrderNo , OrderByType.Desc).ToArray();
                    int lockedQty = 0;
                    int needQty = detail.PlanOutQty - detail.ActOutQty;
                    foreach (Wms_inventory inventory in inventories)
                    {
                        inventory.IsLocked = true;
                        if (_client.Updateable(inventory).ExecuteCommand() == 0)
                        {
                            _client.RollbackTran();
                            return YL.Core.Dto.RouteData.From(PubMessages.E2118_STOCKOUT_LOCK_FAIL, "料格锁定状态更新失败");
                        }
                        lockedQty += inventory.Qty;
                        if(lockedQty >= needQty)
                        {
                            //已锁定足够物料
                            break;
                        }
                    }
                    if (lockedQty < needQty)
                    {
                        _client.RollbackTran();
                        Wms_material material = await _client.Queryable<Wms_material>().FirstAsync(x => x.MaterialId == detail.MaterialId);
                        return YL.Core.Dto.RouteData.From(PubMessages.E2119_STOCKOUT_MATERIAL_ENOUGH, $"{material.MaterialNo}({material.MaterialName}) 需求/可锁定数：{lockedQty}/{needQty}"); 
                    }
                }

                _client.CommitTran();
                return YL.Core.Dto.RouteData.From(PubMessages.I2103_STOCKOUT_LOCK_SCCUESS);
            }
            catch (Exception ex)
            {
                _client.RollbackTran();
                return YL.Core.Dto.RouteData.From(PubMessages.E2118_STOCKOUT_LOCK_FAIL, ex.Message); 
            } 
        }

        [HttpPost]
        public RouteData DoComplate(long storeId,long stockOutId)
        {
            try
            {
                _client.BeginTran();

                Wms_stockout stockout = _stockoutServices.QueryableToEntity(x => x.StockOutId == stockOutId);
                if (stockout == null) { return YL.Core.Dto.RouteData.From(PubMessages.E2113_STOCKOUT_NOTFOUND); }
                if (stockout.StockOutStatus == StockOutStatus.task_finish.ToByte()) { return YL.Core.Dto.RouteData.From(PubMessages.E2114_STOCKOUT_ALLOW_FINISHED); }

                stockout.StockOutStatus = StockOutStatus.task_finish.ToByte();
                if (!_stockoutServices.Update(stockout))
                {
                    return YL.Core.Dto.RouteData.From(PubMessages.E2117_STOCKOUT_FAIL, "出库单详细状态更新失败");
                }
                List<Wms_stockoutdetail> details = _stockoutdetailServices.QueryableToList(x => x.StockOutId == stockout.StockOutId);
                foreach(Wms_stockoutdetail detail in details)
                {
                    if (detail.Status == StockOutStatus.task_finish.ToByte()) continue;
                    detail.Status = StockOutStatus.task_finish.ToByte();
                    detail.ModifiedBy = this.UserDto.UserId;
                    detail.ModifiedDate = DateTime.Now;
                    if (!_stockoutdetailServices.Update(detail))
                    {
                        return YL.Core.Dto.RouteData.From(PubMessages.E2117_STOCKOUT_FAIL, "出库单详细状态更新失败");
                    }
                }
                _client.CommitTran();
                return YL.Core.Dto.RouteData.From(PubMessages.I2102_STOCKOUT_SCCUESS);
            }
            catch(Exception ex)
            { 
                _client.RollbackTran();
                return YL.Core.Dto.RouteData.From(PubMessages.E2117_STOCKOUT_FAIL, ex.Message);
            } 
        }

        [HttpGet]
        public async Task<RouteData<Wms_StockOutMaterialDetailDto>> SearchMaterial(long storeId,long stockOutId,string materialNo)
        {
            Wms_material material = await _client.Queryable<Wms_material>().FirstAsync( x => x.MaterialNo == materialNo);
            if(material == null)
            {
                return RouteData<Wms_StockOutMaterialDetailDto>.From(PubMessages.E1005_MATERIALNO_NOTFOUND);
            }
            Wms_stockout stockout = await _client.Queryable<Wms_stockout>().FirstAsync(x => x.StockOutId == stockOutId);
            if (stockout == null)
            {
                return RouteData<Wms_StockOutMaterialDetailDto>.From(PubMessages.E2115_STOCKOUT_HASNOT_MATERIAL);
            }
            Wms_stockoutdetail detail = _stockoutdetailServices.QueryableToEntity(x => x.StockOutId == stockOutId && x.MaterialId == material.MaterialId);
            if(detail == null)
            {
                return RouteData<Wms_StockOutMaterialDetailDto>.From(PubMessages.E2115_STOCKOUT_HASNOT_MATERIAL);
            } 
            Wms_StockOutMaterialDetailDto detailDto = new Wms_StockOutMaterialDetailDto()
            {
                StockOutId = detail.StockOutId.ToString(),
                StockOutDetailId = detail.StockOutDetailId.ToString(),
                MaterialId = material.MaterialId.ToString(),
                MaterialNo = material.MaterialNo,
                MaterialName = material.MaterialName,
                OrderNo = stockout.OrderNo,
                PlanOutQty = (int)detail.PlanOutQty,
                ActOutQty = (int)detail.ActOutQty,
                Qty = 0

            };
            return RouteData<Wms_StockOutMaterialDetailDto>.From(detailDto);
        }

        [HttpPost]
        [FilterXss]
        [OperationLog(LogType.addOrUpdate)]
        public IActionResult AddOrUpdate([FromForm]Wms_stockout model, [FromForm]string id)
        {
            var validator = new StockOutFluent();
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
                    if (_stockoutServices.IsAny(c => c.OrderNo == model.OrderNo))
                    {
                        return BootJsonH((false, PubConst.StockOut1));
                    }
                }
                model.StockOutNo = _serialnumServices.GetSerialnum(UserDtoCache.UserId, "Wms_stockout");
                model.StockOutId = PubId.SnowflakeId;
                model.StockOutStatus = StockOutStatus.initial.ToByte();
                model.CreateBy = UserDtoCache.UserId;
                bool flag = _stockoutServices.Insert(model);
                return BootJsonH(flag ? (flag, PubConst.Add1) : (flag, PubConst.Add2));
            }
            else
            {
                model.StockOutId = id.ToInt64();
                model.ModifiedBy = UserDtoCache.UserId;
                model.ModifiedDate = DateTimeExt.DateTime;
                var flag = _stockoutServices.Update(model);
                return BootJsonH(flag ? (flag, PubConst.Update1) : (flag, PubConst.Update2));
            }
        }

        [HttpPost]
        [FilterXss]
        [OperationLog(LogType.addOrUpdate)]
        public IActionResult AddOrUpdateD([FromForm]Wms_stockoutdetail model, [FromForm]string id)
        {
            var validator = new StockOutDetailFluent();
            var results = validator.Validate(model);
            var success = results.IsValid;
            if (!success)
            {
                string msg = results.Errors.Aggregate("", (current, item) => (current + item.ErrorMessage + "</br>"));
                return BootJsonH((PubEnum.Failed.ToInt32(), msg));
            }
            if (id.IsEmptyZero())
            {
                model.StockOutDetailId = PubId.SnowflakeId;
                model.Status = StockOutStatus.initial.ToByte();
                model.CreateBy = UserDtoCache.UserId;
                bool flag = _stockoutdetailServices.Insert(model);
                return BootJsonH(flag ? (flag, PubConst.Add1) : (flag, PubConst.Add2));
            }
            else
            {
                model.StockOutDetailId = id.ToInt64();
                model.ModifiedBy = UserDtoCache.UserId;
                model.ModifiedDate = DateTimeExt.DateTime;
                var flag = _stockoutdetailServices.Update(model);
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
            var list = _stockoutdetailServices.QueryableToList(c => c.IsDel == 1 && c.StockOutId == SqlFunc.ToInt64(id));
            if (!list.Any())
            {
                return BootJsonH((false, PubConst.StockIn4));
            }
            var flag = _stockoutServices.Auditin(UserDtoCache.UserId, SqlFunc.ToInt64(id));
            return BootJsonH(flag.IsSuccess ? (flag.IsSuccess, PubConst.StockIn2) : (flag.IsSuccess, flag.ErrorMessage));
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
                 _client.Updateable(new Wms_stockoutdetail { IsDel = 0, ModifiedBy = UserDtoCache.UserId, ModifiedDate = DateTimeExt.DateTime })
                 .UpdateColumns(c => new { c.IsDel, c.ModifiedBy, c.ModifiedDate }).Where(c => c.StockOutId == SqlFunc.ToInt64(id) && c.IsDel == 1).ExecuteCommand();

                 _client.Updateable(new Wms_stockout { StockOutId = SqlFunc.ToInt64(id), IsDel = 0, ModifiedBy = UserDtoCache.UserId, ModifiedDate = DateTimeExt.DateTime }).UpdateColumns(c => new { c.IsDel, c.ModifiedBy, c.ModifiedDate }).ExecuteCommand();
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
            var flag = _stockoutdetailServices.Update(
                 new Wms_stockoutdetail { IsDel = 0, ModifiedBy = UserDtoCache.UserId, ModifiedDate = DateTimeExt.DateTime },
                 c => new { c.IsDel, c.ModifiedBy, c.ModifiedDate },
                 c => c.StockOutDetailId == SqlFunc.ToInt64(id)
                 );
            return BootJsonH(flag ? (flag, PubConst.Delete1) : (flag, PubConst.Delete2));
        }

        [HttpGet]
        public IActionResult PreviewJson(string id)
        {
            var str = _stockoutServices.PrintList(id);
            return Content(str);
        }
    }
}
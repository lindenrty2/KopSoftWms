using IServices;
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
        public async Task<RouteData<Wms_StockInDto>> Get(long id)
        {
            Wms_stockin model = await _client.Queryable<Wms_stockin>().FirstAsync(c => c.StockInId == id && c.IsDel == 1);
            if(model == null)
            {
                RouteData<Wms_stockin>.From(PubMessages.E2013_STOCKIN_NOTFOUND);
            }
            Wms_StockInDto dto = JsonConvert.DeserializeObject<Wms_StockInDto>(JsonConvert.SerializeObject(model));
            List<Wms_StockMaterialDetailDto> details = await _client.Queryable<Wms_stockindetail,Wms_material>
                ((sid,m) => new object[] {
                   JoinType.Left,sid.MaterialId==m.MaterialId,
                })
                .Where(x => x.StockInId == model.StockInId)
                .Select((sid,m) => new Wms_StockMaterialDetailDto()
                {
                    StockId = sid.StockInId.ToString(),
                    StockDetailId = sid.StockInDetailId.ToString(),
                    MaterialId = m.MaterialId.ToString(),
                    MaterialNo = m.MaterialNo,
                    MaterialName = m.MaterialName,
                    PlanQty = (int)sid.PlanInQty * -1,
                    ActQty = (int)sid.ActInQty * -1,
                    Qty = 0
                })
                .MergeTable()
                .ToListAsync();

            dto.Details = details.ToArray();
            return RouteData<Wms_StockInDto>.From(dto);
        }

        [HttpGet]
        public IActionResult Add(long id,long storeId)
        {
            var model = new Wms_stockin();
            if (id.IsEmpty())
            {
                model.WarehouseId = storeId;
                return View(model);
            }
            else
            {
                model = _stockinServices.QueryableToEntity(c => c.StockInId == id && c.IsDel == 1);
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
        public IActionResult ScanPage(long storeId,long stockInId)
        {
            ViewData["currentStoreId"] = storeId;
            var model = _stockinServices.QueryableToEntity(c => c.StockInId == SqlFunc.ToInt64(stockInId) && c.IsDel == 1);
            return View(model);
        }

        /// <summary>
        /// 扫描完成处理
        /// </summary>
        /// <param name="stockInId"></param>
        /// <param name="inventoryBoxId"></param>
        /// <param name="materials"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<RouteData> DoScanComplate(long stockInId,long inventoryBoxId, Wms_StockMaterialDetailDto[] materials,string remark)
        {
            try
            {
                _client.BeginTran();
                RouteData result = await _stockinServices.DoScanComplate(stockInId, inventoryBoxId, materials, remark, this.UserDto);
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
        
        [HttpPost]
        public RouteData DoComplate(long storeId,long stockInId)
        {
            try
            {
                _client.BeginTran();

                Wms_stockin stockin = _stockinServices.QueryableToEntity(x => x.StockInId == stockInId);
                if (stockin == null) { return YL.Core.Dto.RouteData.From(PubMessages.E2013_STOCKIN_NOTFOUND); }
                if (stockin.StockInStatus == StockInStatus.task_finish.ToByte()) { return YL.Core.Dto.RouteData.From(PubMessages.E2014_STOCKIN_ALLOW_FINISHED); }

                stockin.StockInStatus = StockInStatus.task_finish.ToByte();
                if (!_stockinServices.Update(stockin))
                {
                    return YL.Core.Dto.RouteData.From(PubMessages.E2017_STOCKIN_FAIL, "入库单详细状态更新失败");
                }
                List<Wms_stockindetail> details = _stockindetailServices.QueryableToList(x => x.StockInId == stockin.StockInId);
                foreach(Wms_stockindetail detail in details)
                {
                    if (detail.Status == StockInStatus.task_finish.ToByte()) continue;
                    detail.Status = StockInStatus.task_finish.ToByte();
                    detail.ModifiedBy = this.UserDto.UserId;
                    detail.ModifiedDate = DateTime.Now;
                    if (!_stockindetailServices.Update(detail))
                    {
                        return YL.Core.Dto.RouteData.From(PubMessages.E2017_STOCKIN_FAIL, "入库单详细状态更新失败");
                    }
                }
                _client.CommitTran();
                return YL.Core.Dto.RouteData.From(PubMessages.I2002_STOCKIN_SCCUESS);
            }
            catch(Exception ex)
            { 
                _client.RollbackTran();
                return YL.Core.Dto.RouteData.From(PubMessages.E2017_STOCKIN_FAIL,ex.Message);
            }
             
        }

        [HttpGet]
        public RouteData<Wms_StockInMaterialDetailDto> SearchMaterial(long storeId,long stockInId,string materialNo)
        {
            Wms_material material = _client.Queryable<Wms_material>().First( x => x.MaterialNo == materialNo);
            if(material == null)
            {
                return RouteData<Wms_StockInMaterialDetailDto>.From(PubMessages.E1005_MATERIALNO_NOTFOUND);
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
                ActInQty = (int)detail.ActInQty,
                Qty = 0

            };
            return RouteData<Wms_StockInMaterialDetailDto>.From(detailDto);
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
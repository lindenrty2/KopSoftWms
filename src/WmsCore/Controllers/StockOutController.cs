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
using QRCoder;
using System.Drawing.Imaging;
using System.Drawing;
using YL.Utils.Files;

namespace KopSoftWms.Controllers
{
    public class StockOutController : BaseController
    {
        //private readonly ISys_dictServices _dictServices;
        //private readonly IWms_CustomerServices _customerServices;
        private readonly IWms_stockoutServices _stockoutServices;
        private readonly ISys_serialnumServices _serialnumServices;
        private readonly IWms_stockoutdetailServices _stockoutdetailServices; 
        //private readonly IWms_stockoutdetailboxServices _stockoutdetailboxServices;
        //private readonly IWms_inventoryBoxServices _inventoryBoxServices;
        //private readonly IWms_inventoryBoxTaskServices _inventoryBoxTaskServices;
        private readonly IWms_inventoryServices _inventoryServices; 
        private readonly SqlSugarClient _client;

        public StockOutController(
            //ISys_dictServices dictServices,
            //IWms_CustomerServices customerServices,
            IWms_stockoutServices stockoutServices,
            ISys_serialnumServices serialnumServices,
            IWms_stockoutdetailServices stockoutdetailServices, 
            //IWms_stockoutdetailboxServices stockoutdetailboxServices,
            //IWms_inventoryBoxServices inventoryBoxServices,
            //IWms_inventoryBoxTaskServices inventoryBoxTaskServices,
            IWms_inventoryServices inventoryServices,
            SqlSugarClient client
            )
        {
            //_dictServices = dictServices;
            //_customerServices = customerServices;
            _stockoutServices = stockoutServices;
            _serialnumServices = serialnumServices;
            _stockoutdetailServices = stockoutdetailServices;
            //_stockoutdetailboxServices = stockoutdetailboxServices;
            //_inventoryBoxServices = inventoryBoxServices;
            //_inventoryBoxTaskServices = inventoryBoxTaskServices;
            _inventoryServices = inventoryServices;
            _client = client;
        }

        [HttpGet]
        [CheckMenu]
        public IActionResult Index()
        {
            var list = _client.Queryable<Sys_dict>().Where(c => c.IsDel == 1 && c.DictType == PubDictType.stockout.ToString()).ToList();
            var stockOutStatus = EnumExt.ToKVListLinq<StockOutStatus>();
            ViewBag.StockOutType = list;
            ViewBag.StockOutStatus = stockOutStatus;
            return View();
        }

        [HttpGet]
        public async Task<string> Search(string storeId,string text)
        {
            IWMSBaseApiAccessor wmsAccessor = WMSApiManager.GetBaseApiAccessor(storeId.ToString(), _client);
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
            List<Wms_StockMaterialDetailDto> details = await _client.Queryable< Wms_stockoutdetail_box, Wms_inventorybox, Wms_stockoutdetail, Wms_material>
                ((sidb, ib, sid, m) => new object[] {
                   JoinType.Left,sidb.InventoryBoxId==ib.InventoryBoxId,
                   JoinType.Left,sidb.StockOutDetailId==sid.StockOutDetailId,
                   JoinType.Left,sid.MaterialId==m.MaterialId,
                })
                .Where((sidb, ib, sid, m) => sid.StockOutId == model.StockOutId)
                .Select((sidb, ib, sid, m) => new Wms_StockMaterialDetailDto()
                {
                    InventoryBoxNo = ib.InventoryBoxNo,
                    StockId = sid.StockOutId.ToString(),
                    StockDetailId = sid.StockOutDetailId.ToString(),
                    StockInUniqueIndex = sid.UniqueIndex,
                    MaterialId = m.MaterialId.ToString(),
                    MaterialNo = m.MaterialNo,
                    MaterialName = m.MaterialName,
                    PlanQty = sidb.PlanQty * -1,
                    ActQty = sidb.Qty * -1,
                    Qty = 0
                })
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

            IWMSBaseApiAccessor wmsAccessor = WMSApiManager.GetBaseApiAccessor(bootstrap.storeId.ToString(), _client);
            RouteData<OutsideStockOutQueryResult[]> result = await wmsAccessor.QueryStockOutList(
                null, null, bootstrap.pageIndex, bootstrap.limit, bootstrap.search, 
                new string[] { bootstrap.sort + " " + bootstrap.order }, 
                bootstrap.datemin, bootstrap.datemax);
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

            IWMSBaseApiAccessor wmsAccessor = WMSApiManager.GetBaseApiAccessor(storeId.ToString(), _client);
            RouteData<OutsideStockOutQueryResult> result = await wmsAccessor.QueryStockOut(SqlFunc.ToInt64(pid));
            if (!result.IsSccuess)
            {
                return null;
            }
            return new PageGridData(result.Data.Details, result.Data.Details.Length).JilToJson();
        }

        
        [HttpGet]
        public async Task<IActionResult> Work(string pid)
        { 
            var model = await _client.Queryable<Wms_stockout>().FirstAsync(c => c.StockOutId == SqlFunc.ToInt64(pid) && c.IsDel == 1);
            ViewData["currentStoreId"] = model.WarehouseId;
            return View(model);
        }

        [HttpGet]
        public async Task<string> WorkList(long storeId,long pid)
        {
            Wms_stockout stockout = await _client.Queryable<Wms_stockout>().FirstAsync(x => x.StockOutId == pid);
            if(stockout == null)
            {
                return "";
            }
            IWMSOperationApiAccessor wmsAccessor = WMSApiManager.GetOperationApiAccessor(storeId.ToString(), _client, this.UserDto);
            Wms_StockOutWorkDetailDto[] result = await wmsAccessor.GetStockOutCurrentPlan(pid);

            return Bootstrap.GridData(result, result.Length).JilToJson();
        }


        [HttpGet]
        public async Task<IActionResult> ScanPage(long storeId,long stockOutId)
        {
            ViewData["currentStoreId"] = storeId;
            var model = await _client.Queryable<Wms_stockout>().FirstAsync(c => c.StockOutId == SqlFunc.ToInt64(stockOutId) && c.IsDel == 1);
            return View(model);
        }

        [HttpPost]
        [Obsolete]
        public async Task<RouteData> DoScanComplate(long storeId,long stockOutId,long inventoryBoxId, Wms_StockMaterialDetailDto[] materials,string remark)
        {
            IWMSOperationApiAccessor wmsAccessor = WMSApiManager.GetOperationApiAccessor(storeId.ToString(), _client, this.UserDto);
            if (wmsAccessor == null)
            {
                return YL.Core.Dto.RouteData.From(PubMessages.E0007_WAREHOUSE_NOTFOUND);
            }
            return await wmsAccessor.DoStockOutScanComplate(stockOutId, inventoryBoxId, materials, remark); 
        }

        [HttpPost]
        public async Task<RouteData> DoLock(long storeId,long stockOutId)
        {
            IWMSOperationApiAccessor wmsAccessor = WMSApiManager.GetOperationApiAccessor(storeId.ToString(), _client, this.UserDto);
            if(wmsAccessor == null)
            {
                return YL.Core.Dto.RouteData.From(PubMessages.E0007_WAREHOUSE_NOTFOUND);
            }
            return await wmsAccessor.DoStockOutLock(stockOutId);
        }

        [HttpPost]
        public async Task<RouteData> DoComplate(long storeId,long stockOutId)
        {
            IWMSOperationApiAccessor wmsAccessor = WMSApiManager.GetOperationApiAccessor(storeId.ToString(), _client, this.UserDto);
            if (wmsAccessor == null)
            {
                return YL.Core.Dto.RouteData.From(PubMessages.E0007_WAREHOUSE_NOTFOUND);
            }
            return await wmsAccessor.DoStockOutComplate(stockOutId); ;
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
            Wms_stockoutdetail detail = await _client.Queryable<Wms_stockoutdetail>().FirstAsync(x => x.StockOutId == stockOutId && x.MaterialId == material.MaterialId);
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
        public async Task<RouteData<bool>> HasNofity(long storeId)
        {
            IWMSOperationApiAccessor wmsAccessor = WMSApiManager.GetOperationApiAccessor(storeId.ToString(), _client, this.UserDto);
            if (wmsAccessor == null)
            {
                return RouteData<bool>.From(false);
            }
            RouteData<bool> result = await wmsAccessor.HasStockOutNofity();

            return result;
        }

        [HttpGet]
        public IActionResult NofityListPage(long storeId)
        {
            ViewData["currentStoreId"] = storeId; 
            return View();
        }

        [HttpPost]
        public async Task<Wms_StockOutDto[]> NofityList(long storeId)
        {
            IWMSOperationApiAccessor wmsAccessor = WMSApiManager.GetOperationApiAccessor(storeId.ToString(), _client, this.UserDto);
            if(wmsAccessor == null)
            {
                return null;
            }
            RouteData<Wms_StockOutDto[]> result = await wmsAccessor.QueryStockOutNofityList();
            if (!result.IsSccuess)
            {
                return new Wms_StockOutDto[0];
            }
            return result.Data;

        }

        [HttpPost]
        public async Task<RouteData> SetStockOutNofitied(long storeId)
        {
            IWMSOperationApiAccessor wmsAccessor = WMSApiManager.GetOperationApiAccessor(storeId.ToString(), _client, this.UserDto);
            if (wmsAccessor == null)
            {
                return new RouteData();
            }
            RouteData result = await wmsAccessor.SetStockOutNofitied();

            return result;
        }

        [HttpGet]
        public IActionResult Preview(long storeId, long pid,long? boxId)
        {
            var model = _stockoutServices.QueryableToEntity(
                c => c.WarehouseId == storeId && c.StockOutId == pid && c.IsDel == 1);

            ViewBag.StoreId = storeId;
            ViewBag.StockOutId = pid;
            ViewBag.BoxId = boxId;
            return View(model);
        }

        [HttpGet]
        public void StockOutNoQRCode(string stockOutNo)
        { 
            string strQR = stockOutNo;

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(
                strQR, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            qrCodeImage.Save(this.Response.Body, ImageFormat.Png);
        }

        [HttpGet]
        public async void MaterialQRCode(long storeId, long detailboxId)
        { 

            var detailBox = await _client.Queryable<Wms_stockoutdetail_box>()
                .Where(c => c.DetailBoxId == detailboxId)
                .FirstAsync();

            var detail = await _client.Queryable<Wms_stockoutdetail>()
                .Where(c => c.StockOutDetailId == detailBox.StockOutDetailId)
                .FirstAsync();

            string strQR = $"{detailBox.StockInUniqueIndex}@@{detail.UniqueIndex}";

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(
                strQR, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            qrCodeImage.Save(this.Response.Body, ImageFormat.Png);
        }

        [HttpGet]
        public IActionResult PreviewJson(long id,long? boxId)
        {
            var list1 = _client.Queryable<Wms_stockout>()
                 .Where((s) => s.StockOutId == id && s.IsDel == DeleteFlag.Normal )
                 .Select((s) => new
                 {
                     StockOutId = s.StockOutId.ToString(),
                     StockOutType = s.StockOutTypeName,
                     StockOutTypeId = s.StockOutType.ToString(),
                     s.StockOutStatus,
                     s.StockOutNo,
                     s.OrderNo,
                     s.WorkNo,
                     s.WorkStationId,
                     s.WorkAreaName,
                     s.IsDel,
                     s.Remark,
                     CName = s.CreateUser,
                     s.CreateDate,
                     UName = s.ModifiedUser,
                     s.ModifiedDate
                 }).ToList();
            bool flag1 = true;
            bool flag2 = true;
            if(boxId == null)
            {
                return Content((flag1, list1, false , new object[1] { new object()}).JilToJson()); 
            }
            var list2 = _client.Queryable<Wms_stockoutdetail_box, Wms_stockoutdetail>((sodb, sod) => new object[] {
                    JoinType.Left,sodb.StockOutDetailId==sod.StockOutDetailId,
                })
                .Where((sodb, sod) => sodb.DetailBoxId == boxId.Value && sod.IsDel == DeleteFlag.Normal)
                .Select((sodb, sod) => new
                {
                    BoxId = sodb.DetailBoxId.ToString(),
                    StockOutId = sod.StockOutId.ToString(),
                    StockOutDetailId = sod.StockOutDetailId.ToString(),
                    sod.SubWarehouseEntryId,
                    sod.UniqueIndex,
                    sodb.StockInUniqueIndex,
                    sod.MaterialNo,
                    sod.MaterialName, 
                    Status = SqlFunc.IF(sod.Status == 1).Return(StockOutStatus.initial.GetDescription())
                    .ElseIF(sod.Status == 2).Return(StockOutStatus.task_confirm.GetDescription())
                    .ElseIF(sod.Status == 3).Return(StockOutStatus.task_canceled.GetDescription())
                    .ElseIF(sod.Status == 3).Return(StockOutStatus.task_working.GetDescription())
                    .End(StockOutStatus.task_finish.GetDescription()),
                    sod.PlanOutQty,
                    sod.ActOutQty,
                    sod.IsDel,
                    sod.Remark,
                    CName = sod.CreateUser,
                    sod.CreateDate,
                    UName = sod.ModifiedUser,
                    sod.ModifiedDate
                })
                .ToList();
            if (!list1.Any())
            {
                flag1 = false;
            }
            if (!list2.Any())
            {
                flag2 = false;
            }
            return Content((flag1, list1, flag2, list2).JilToJson());
        }


        [HttpGet]
        public async Task<RouteData<MaterialCode>> QueryStockInMaterial(string no)
        {
            Wms_stockindetail targetDetail = await _client.Queryable<Wms_stockindetail>()
                .FirstAsync(x => x.UniqueIndex == no);
            if (targetDetail != null)
            {
                return RouteData<MaterialCode>.From(
                    new MaterialCode()
                    {
                        UniqueIndex = no,
                        MaterialId = targetDetail.MaterialId.ToString(),
                        MaterialNo = targetDetail.MaterialNo,
                        MaterialOnlyId = targetDetail.MaterialOnlyId,
                        MaterialName = targetDetail.MaterialName
                    }
                );
            } 
            return new RouteData<MaterialCode>() { Code = -1 };
        }

        public class MaterialCode
        {
            public string UniqueIndex { get; set; }
            public string MaterialId { get; set; }
            public string MaterialName { get; set; }
            public string MaterialNo { get; set; }
            public string MaterialOnlyId { get; set; }

        }
    }
}
using IServices;
using IServices.Outside;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using YL.Core.Dto;
using YL.Core.Entity;
using YL.Utils.Check;
using YL.Utils.Extensions;
using YL.Utils.Pub;

namespace WMSCore.Outside
{
    [Route("/hook/mes")]
    public class MESHookController : Controller, IMESHookController
    {
        private IWms_mastaskServices _mastaskServices;
        private IWms_materialServices _materialServices;
        private ISys_serialnumServices _serialnumServices;
        private ISys_dictServices _dictServices;
        private IWms_stockinServices _stockinServices;
        private IWms_stockindetailServices _stockinDetailServices;
        private IWms_stockoutServices _stockoutServices;
        private IWms_stockoutdetailServices _stockoutDetailServices;
        private SqlSugarClient _sqlClient;
        public MESHookController(
            SqlSugarClient sqlClient,
            IWms_mastaskServices mastaskServices,
            IWms_materialServices materialServices,
            ISys_serialnumServices serialnumServices,
            ISys_dictServices dictServices,
            IWms_stockinServices stockinService,
            IWms_stockindetailServices stockinDetailServices,
            IWms_stockoutServices stockoutServices,
            IWms_stockoutdetailServices stockoutDetailServices)
        {
            _sqlClient = sqlClient;
            _mastaskServices = mastaskServices;
            _materialServices = materialServices;
            _serialnumServices = serialnumServices;
            _dictServices = dictServices;
            _stockinServices = stockinService;
            _stockinDetailServices = stockinDetailServices;
            _stockoutServices = stockoutServices;
            _stockoutDetailServices = stockoutDetailServices;
        }

        [HttpGet("Ping")]
        public string Ping()
        {
            return "OK";
        }

        [HttpPost("Warehousing")]
        //public OutsideStockInResult Warehousing([FromBody]OutsideStockInDto data)
        public string Warehousing(String WarehousingId, String WarehousingType, String WarehousingTime, String ProductionPlanId, String BatchPlanId, String WorkAreaName, String SuppliesKinds, String SuppliesInfoList)
        {

            try
            {
                OutsideStockInDto data = OutsideStockInDto.Create(
                    Guard.GuardEmpty(() => WarehousingId),
                    Guard.GuardEmpty(() => WarehousingType),
                    Guard.GuardEmpty(() => WarehousingTime),
                    Guard.GuardEmpty(() => ProductionPlanId),
                    Guard.GuardEmpty(() => BatchPlanId),
                    Guard.GuardEmpty(() => WorkAreaName),
                    Guard.GuardInteger(() => SuppliesKinds),
                    Guard.GuardEmpty(() => SuppliesInfoList)
                );

                _sqlClient.BeginTran();
                string jsonSuppliesInfoStr = data.SuppliesInfoList;
                OutsideMaterialDto[] suppliesInfos = JsonConvert.DeserializeObject<OutsideMaterialDto[]>(jsonSuppliesInfoStr);
                Wms_mestask mesTask = new Wms_mestask()
                {
                    MesTaskId = PubId.SnowflakeId,
                    MesTaskType = MESTaskTypes.StockIn,
                    WarehousingId = data.WarehousingId, //入库单编号
                    WarehousingType = data.WarehousingType, //入库类型
                    WarehousingTime = data.WarehousingTime.SerialNumberToDateTime(),   //入库时间
                    ProductionPlanId = data.ProductionPlanId, //生产令号
                    BatchPlanId = data.BatchPlanId, //批次号
                    WorkAreaName = data.WorkAreaName, //作业区
                    SuppliesKinds = data.SuppliesKinds, //物料种类
                    SuppliesInfoJson = "", // jsonSuppliesInfoStr, //物料信息
                    WorkStatus = MESTaskWorkStatus.WaitPlan,      //等待计划
                    NotifyStatus = MESTaskNotifyStatus.Requested, //已接收
                    CreateDate = DateTime.Now
                };
                _mastaskServices.Insert(mesTask);

                RouteData routeData = CreateWMSStockin(mesTask, suppliesInfos);
                if (!routeData.IsSccuess)
                {
                    _sqlClient.RollbackTran();
                    OutsideStockInResult result = new OutsideStockInResult()
                    {
                        ErrorId = routeData.CodeString,
                        ErrorInfo = routeData.Message,
                        Success = false,
                        WarehousingId = data.WarehousingId,
                        WarehousingTime = data.WarehousingTime
                    };

                    return JsonConvert.SerializeObject(result);
                }
                else
                {
                    _sqlClient.CommitTran();
                    OutsideStockInResult result = new OutsideStockInResult()
                    {
                        Success = true,
                        ErrorId = string.Empty,
                        ErrorInfo = string.Empty,
                        WarehousingId = data.WarehousingId,
                        WarehousingTime = data.WarehousingTime
                    };
                    return JsonConvert.SerializeObject(result);
                }
            }
            catch (Exception ex)
            {
                _sqlClient.RollbackTran();
                OutsideStockInResult result = new OutsideStockInResult()
                {
                    Success = true,
                    ErrorId = "-1",
                    ErrorInfo = ex.ToString()
                    //  WarehousingId = Warehousingid,
                    //  WarehousingTime = Warehousingtime
                };
                return JsonConvert.SerializeObject(result);
            }

        }

        private RouteData CreateWMSStockin(Wms_mestask mesTask, OutsideMaterialDto[] suppliesInfoList)
        {
            Dictionary<long, List<Wms_MaterialInventoryDto>> map = new Dictionary<long, List<Wms_MaterialInventoryDto>>();
            Sys_dict[] typeDicts = _sqlClient.Queryable<Sys_dict>()
                     .Where(x => x.DictType == PubDictType.material.ToByte().ToString())
                     .ToArray();

            foreach (OutsideMaterialDto materialDto in suppliesInfoList)
            {

                Sys_dict typeDict = typeDicts.FirstOrDefault(x => x.DictName == materialDto.SuppliesType);
                if (typeDict == null)
                {
                    return RouteData<Wms_material>.From(PubMessages.E1001_SUPPLIESTYPE_NOTFOUND, $"SuppliesType = {materialDto.SuppliesType}");
                }
                else if (typeDict.WarehouseId == null)
                {
                    return RouteData<Wms_material>.From(PubMessages.E1002_SUPPLIESTYPE_WAREHOUSEID_NOTSET, $"SuppliesType = {materialDto.SuppliesType}");
                }
                long warehouseId = typeDict.WarehouseId.Value;
                List<Wms_MaterialInventoryDto> warehouseMaterialList = null;
                if (map.ContainsKey(warehouseId))
                {
                    warehouseMaterialList = map[warehouseId];
                }
                else
                {
                    warehouseMaterialList = new List<Wms_MaterialInventoryDto>();
                    map.Add(warehouseId, warehouseMaterialList);
                }
                warehouseMaterialList.Add(new Wms_MaterialInventoryDto()
                {
                    MaterialId = "-1",
                    MaterialOnlyId = materialDto.SuppliesOnlyId,
                    MaterialNo = materialDto.SuppliesId,
                    MaterialName = materialDto.SuppliesName,
                    MaterialType = materialDto.SuppliesType,
                    Qty = materialDto.SuppliesNumber,
                    Unit = materialDto.Unit
                });
            }

            List<RouteData<OutsideStockInRequestResult[]>> result = new List<RouteData<OutsideStockInRequestResult[]>>();
            foreach (KeyValuePair<long, List<Wms_MaterialInventoryDto>> keyValue in map)
            {
                try
                {
                    IWMSBaseApiAccessor proxy = WMSApiManager.GetBaseApiAccessor(keyValue.Key.ToString(), _sqlClient);
                    OutsideStockInRequestDto request = new OutsideStockInRequestDto()
                    {
                        MesTaskId = mesTask.MesTaskId,
                        WarehousingId = mesTask.WarehousingId,
                        WarehousingTime = mesTask.WarehousingTime.ToString(PubConst.Format_DateTime),
                        WarehousingType = mesTask.WarehousingType,
                        WarehouseId = keyValue.Key,
                        OrderNo = mesTask.ProductionPlanId,
                        WorkAreaName = mesTask.WorkAreaName,
                        BatchPlanId = mesTask.BatchPlanId,
                        MaterialList = keyValue.Value.ToArray(),
                    };
                    RouteData<OutsideStockInRequestResult[]> data = proxy.StockIn(request).GetAwaiter().GetResult();
                    if (!data.IsSccuess)
                    {
                        result.Add(data);
                    }
                }
                catch (Exception ex)
                {
                    //TODO Log
                }
            }

            return new RouteData();

        }


        /// <summary>
        /// 出库处理
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("WarehouseEntry")]
        //public OutsideStockOutResult WarehouseEntry([FromBody]OutsideStockOutDto data)
        public string WarehouseEntry(String WarehouseEntryId,
            String WarehouseEntryType,
            String WarehouseEntryTime,
            String ProductionPlanId,
            String BatchPlanId,
            String WorkStationId,
            String WorkAreaName,
            String SuppliesKinds,
            String SuppliesInfoList)
        {
            try
            {
                OutsideStockOutResult result1 = new OutsideStockOutResult()
                {
                    ErrorId = "",
                    ErrorInfo = "",
                    Success = true,
                    WarehouseEntryId = WarehouseEntryId
                };
                return JsonConvert.SerializeObject(result1);



                OutsideStockOutDto data = OutsideStockOutDto.Create(
                       Guard.GuardEmpty(() => WarehouseEntryId),
                       Guard.GuardEmpty(() => WarehouseEntryType),
                       Guard.GuardEmpty(() => WarehouseEntryTime),
                       Guard.GuardEmpty(() => ProductionPlanId),
                       Guard.GuardEmpty(() => BatchPlanId),
                       Guard.GuardEmpty(() => WorkAreaName),
                       Guard.GuardEmpty(() => WorkStationId),
                       Guard.GuardInteger(() => SuppliesKinds),
                       Guard.GuardEmpty(() => SuppliesInfoList)
                   );
                 

                _sqlClient.BeginTran();
                string jsonSuppliesInfoStr = data.SuppliesInfoList;
                OutsideMaterialDto[] suppliesInfos = JsonConvert.DeserializeObject<OutsideMaterialDto[]>(jsonSuppliesInfoStr);
                Wms_mestask mesTask = new Wms_mestask()
                {
                    MesTaskId = PubId.SnowflakeId,
                    MesTaskType = MESTaskTypes.StockOut,
                    WarehousingId = data.WarehouseEntryId, //入库单编号
                    WarehousingType = data.WarehouseEntryType, //入库类型
                    WarehousingTime = data.WarehouseEntryTime.SerialNumberToDateTime(),   //入库时间
                    ProductionPlanId = data.ProductionPlanId, //生产令号
                    BatchPlanId = data.BatchPlanId, //批次号
                    WorkAreaName = data.WorkAreaName, //作业区
                    WorkStationId = data.WorkStationId, //工位号
                    SuppliesKinds = data.SuppliesKinds, //物料种类
                    SuppliesInfoJson = "", //jsonSuppliesInfoStr, //物料信息
                    WorkStatus = MESTaskWorkStatus.WaitPlan,      //等待计划
                    NotifyStatus = MESTaskNotifyStatus.Requested, //已接收
                    CreateDate = DateTime.Now
                };
                if (_sqlClient.Insertable(mesTask).ExecuteCommand() == 0)
                {
                    throw new Exception("mesTask更新异常");
                }

                RouteData routeData = CreateWMSStockout(mesTask, suppliesInfos);
                if (!routeData.IsSccuess)
                {
                    _sqlClient.RollbackTran();
                    OutsideStockOutResult result = new OutsideStockOutResult()
                    {
                        ErrorId = routeData.CodeString,
                        ErrorInfo = routeData.Message,
                        Success = false,
                        WarehouseEntryId = data.WarehouseEntryId,
                        //WarehouseEntryTime = data.WarehouseEntryTime
                    };
                    return JsonConvert.SerializeObject(result);
                }
                else
                {
                    _sqlClient.CommitTran();
                    OutsideStockOutResult result = new OutsideStockOutResult()
                    {
                        Success = true,
                        ErrorId = string.Empty,
                        ErrorInfo = string.Empty,
                        WarehouseEntryId = data.WarehouseEntryId,
                        //WarehouseEntryTime = data.WarehouseEntryTime
                    };
                    return JsonConvert.SerializeObject(result);
                }
            }
            catch (Exception ex)
            {
                _sqlClient.RollbackTran();
                OutsideStockOutResult result = new OutsideStockOutResult()
                {
                    Success = true,
                    ErrorId = "-1",
                    ErrorInfo = ex.ToString(),
                    //WarehouseEntryId = warehouseEntryid,
                    //WarehouseEntryTime = data.WarehouseEntryTime
                };
                return JsonConvert.SerializeObject(result);
            }
        }

        private RouteData CreateWMSStockout(Wms_mestask mesTask, OutsideMaterialDto[] suppliesInfoList)
        {
            Dictionary<long, List<Wms_MaterialInventoryDto>> map = new Dictionary<long, List<Wms_MaterialInventoryDto>>();
            foreach (OutsideMaterialDto materialDto in suppliesInfoList)
            {
                Sys_dict typeDict = _sqlClient.Queryable<Sys_dict>()
                       .First(x => x.DictType == PubDictType.material.ToByte().ToString() && x.DictName == materialDto.SuppliesType);
                if (typeDict == null)
                {
                    return RouteData<Wms_material>.From(PubMessages.E1001_SUPPLIESTYPE_NOTFOUND, $"SuppliesType = {materialDto.SuppliesType}");
                }
                else if (typeDict.WarehouseId == null)
                {
                    return RouteData<Wms_material>.From(PubMessages.E1002_SUPPLIESTYPE_WAREHOUSEID_NOTSET, $"SuppliesType = {materialDto.SuppliesType}");
                }
                long warehouseId = typeDict.WarehouseId.Value;
                List<Wms_MaterialInventoryDto> warehouseMaterialList = null;
                if (map.ContainsKey(warehouseId))
                {
                    warehouseMaterialList = map[warehouseId];
                }
                else
                {
                    warehouseMaterialList = new List<Wms_MaterialInventoryDto>();
                    map.Add(warehouseId, warehouseMaterialList);
                }
                warehouseMaterialList.Add(new Wms_MaterialInventoryDto()
                {
                    MaterialId = "-1",
                    MaterialOnlyId = materialDto.SuppliesOnlyId,
                    MaterialNo = materialDto.SuppliesId,
                    MaterialName = materialDto.SuppliesName,
                    MaterialType = materialDto.SuppliesType,
                    Qty = materialDto.SuppliesNumber,
                    Unit = materialDto.Unit
                });
            }

            List<RouteData> result = new List<RouteData>();
            foreach (KeyValuePair<long, List<Wms_MaterialInventoryDto>> keyValue in map)
            {
                try
                {
                    IWMSBaseApiAccessor proxy = WMSApiManager.GetBaseApiAccessor(keyValue.Key.ToString(), _sqlClient);
                    OutsideStockOutRequestDto request = new OutsideStockOutRequestDto()
                    {
                        MesTaskId = mesTask.MesTaskId,
                        WarehousingId = mesTask.WarehousingId,
                        WarehousingTime = mesTask.WarehousingTime.ToString(PubConst.Format_DateTime),
                        WarehousingType = mesTask.WarehousingType,
                        WarehouseId = keyValue.Key,
                        OrderNo = mesTask.ProductionPlanId,
                        WorkAreaName = mesTask.WorkAreaName,
                        WorkStationId = mesTask.WorkStationId,
                        BatchPlanId = mesTask.BatchPlanId,
                        MaterialList = keyValue.Value.ToArray(),
                    };
                    RouteData data = proxy.StockOut(request).GetAwaiter().GetResult();
                    if (!data.IsSccuess)
                    {
                        result.Add(data);
                    }
                }
                catch (Exception ex)
                {
                    //TODO Log
                }
            }


            return new RouteData();
        }


        /// <summary>
        /// 物料库存查询
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        [HttpGet("MaterialStockEnquiry")]
        //public OutsideMaterialStockEnquiryResult MaterialStockEnquiry(OutsideMaterialStockEnquiryArg arg)
        public string MaterialStockEnquiry(String SuppliesId, String SuppliesName, String SuppliesType, String SuppliesUnit)
        {
            //TODO 需要其他库的查询
            try
            {
                //   OutsideMaterialStockEnquiryArg.Create(
                //       Guard.GuardEmpty(() => SuppliesId),
                //       Guard.GuardEmpty(() => SuppliesName),
                //       Guard.GuardEmpty(() => SuppliesType),
                //       Guard.GuardEmpty(() => SuppliesUnit));
                var inventories = _sqlClient.Queryable<Wms_inventory, Wms_inventorybox, Wms_warehouse>(
                    (i, ib, w) => new object[] {
                   JoinType.Left,i.InventoryBoxId == ib.InventoryBoxId,
                   JoinType.Left,ib.WarehouseId == w.WarehouseId
                    })
                    //.Where(x => x.MaterialNo == SuppliesId || x.MaterialOnlyId == SuppliesId)
                    .Where((i, ib, w) => i.MaterialNo == SuppliesId || i.MaterialOnlyId == SuppliesId)
                    .Select((i, ib, w) => new
                    {
                        i.InventoryId,
                        ib.InventoryBoxNo,
                        w.WarehouseName,
                        ib.StorageRackName,
                        ib.WarehouseId,
                        i.Position,
                        i.MaterialNo,
                        i.MaterialOnlyId,
                        i.MaterialName,
                        i.Qty,
                        i.ModifiedUser,
                        i.ModifiedDate
                    }).ToArray();
                ;

                List<OutsideMaterialStockEnquiryItem> items = new List<OutsideMaterialStockEnquiryItem>();
                foreach (var inventory in inventories)
                {
                    OutsideMaterialStockEnquiryItem item = new OutsideMaterialStockEnquiryItem()
                    {
                        InventoryBoxNo = inventory.InventoryBoxNo,
                        BalanceStock = inventory.Qty.ToString(),
                        PaperStock = inventory.Qty.ToString(),
                        StorageRackPosition = inventory.StorageRackName,
                        WarehouseId = Convert.ToString(inventory.WarehouseId),
                        WarehouseName = inventory.WarehouseName,
                        Position = Convert.ToString(inventory.Position),
                        WarehousePosition = "??" //TODO
                    };
                    items.Add(item);
                }
                OutsideMaterialStockEnquiryResult result = new OutsideMaterialStockEnquiryResult();
                result.Success = true;
                result.SuppliesId = SuppliesId;
                result.SuppliesName = SuppliesName;
                result.SuppliesType = SuppliesType;
                result.SuppliesUnit = SuppliesUnit;
                result.MaterialStockInfo = items.ToArray();
                return JsonConvert.SerializeObject(result);
            }
            catch (Exception ex)
            {
                OutsideMaterialStockEnquiryResult result = new OutsideMaterialStockEnquiryResult()
                {
                    ErrorID = "-1",
                    Success = false,
                };
                return JsonConvert.SerializeObject(result);
            }
        }

        /// <summary>
        /// 物流控制
        /// </summary>
        [HttpPost("LogisticsControl")]
        //public OutsideLogisticsControlResult LogisticsControl([FromBody]OutsideLogisticsControlArg arg)
        public string LogisticsControl(String LogisticsId, String StartPoint, String Destination)
        {
            //var result = WCSApiAccessor.Instance.LogisticsControl(OutsideLogisticsControlArg.Create(LogisticsId,StartPoint, Destination)).Result;

            var result = new OutsideLogisticsControlResult() { EquipmentId = "1", EquipmentName = "EquipmentName-1" };
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 物流（状态）查询
        /// </summary>
        [HttpGet("LogisticsEnquiry")]
        //public OutsideLogisticsEnquiryResult LogisticsEnquiry(OutsideLogisticsEnquiryArg arg)
        public string LogisticsEnquiry(String LogisticsId, String EquipmentId, String EquipmentName)
        {
            // var result = WCSApiAccessor.Instance.LogisticsEnquiry(OutsideLogisticsEnquiryArg.Create(LogisticsId,EquipmentId, EquipmentName)).Result;
            var result = new OutsideLogisticsEnquiryArg() { LogisticsId = LogisticsId, EquipmentId = "1", EquipmentName = "EquipmentName-1" };
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 入库状态查询
        /// </summary>
        [HttpGet("WarehousingStatusEnquiry")]
        //public OutsideWarehousingStatusEnquiryResult WarehousingStatusEnquiry(OutsideWarehousingStatusEnquiryArg arg)
        public string WarehousingStatusEnquiry(string WarehousingId, string WarehousingType)
        {
            Wms_mestask mesTask = _sqlClient.Queryable<Wms_mestask>().First(x => x.WarehousingId == WarehousingId && x.WarehousingType == WarehousingType);
            if (mesTask == null)
            {
                OutsideWarehousingStatusEnquiryResult error = new OutsideWarehousingStatusEnquiryResult()
                {
                    Success = "false",
                    ErrorId = PubMessages.E3000_MES_STOCKINTASK_NOTFOUND.Code.ToString(),
                    ErrorInfo = PubMessages.E3000_MES_STOCKINTASK_NOTFOUND.Message
                };
                return JsonConvert.SerializeObject(error);
            }

            var stockinList = _sqlClient.Queryable<Wms_stockin, Wms_warehouse>(
                (s, w) => new object[] {
                   JoinType.Left,s.WarehouseId==w.WarehouseId
                 })
                 .Where((s, w) => s.MesTaskId == mesTask.MesTaskId)
                 .Select((s, w) => new {
                     s.WarehouseId,
                     w.WarehouseName,
                     s.StockInId,
                     s.StockInNo,
                     s.StockInStatus,
                     s.StockInType
                 })
                .ToList();

            List<WarehousingStatusInfo> statusInfoList = new List<WarehousingStatusInfo>();
            foreach (var stockin in stockinList)
            {
                var stockinDetailList = _sqlClient.Queryable<Wms_stockindetail, Wms_stockindetail_box, Wms_inventorybox, Wms_material>(
                   (sid, sidb, ib, m) => new object[] {
                       JoinType.Left,sid.StockInDetailId == sidb.StockinDetailId,
                       JoinType.Left,sidb.InventoryBoxId == ib.InventoryBoxId,
                       JoinType.Left,sid.MaterialId == m.MaterialId,
                   }
                )
                .Where((sid, sidb, ib, m) => sid.StockInId == stockin.StockInId)
                .Select((sid, sidb, ib, m) => new {
                    sid.WarehouseId,
                    sid.MaterialId,
                    m.MaterialNo,
                    m.MaterialOnlyId,
                    sid.StockInDetailId,
                    sidb.InventoryBoxId,
                    ib.InventoryBoxNo,
                    sidb.Position,
                    sidb.Qty,
                    sid.Status,
                    sid.CreateBy,
                    sid.CreateDate,
                    sid.ModifiedBy,
                    sid.ModifiedDate
                }).MergeTable().ToList();
                foreach (var detail in stockinDetailList)
                {
                    statusInfoList.Add(new WarehousingStatusInfo()
                    {
                        IsNormalWarehousing = detail.Status == StockInStatus.task_finish.ToByte(),
                        WarehouseId = detail.WarehouseId.ToString(),
                        WarehousePosition = null,
                        WarehouseName = stockin.WarehouseName,
                        InventoryBoxNo = detail.InventoryBoxNo,
                        Position = Convert.ToString(detail.Position),//TODO
                        StorageRackPosition = "",
                        SuppliesId = string.IsNullOrWhiteSpace(detail.MaterialOnlyId) ? detail.MaterialId.ToString() : detail.MaterialOnlyId.ToString(),
                        RefreshStock = 0, //TODO
                        WarehousingStep = ((StockInStatus)detail.Status).ToString(),
                        WarehousingFinishTime = detail.ModifiedDate?.ToString("yyyyMMddHHmmss")
                    });
                }
            }

            OutsideWarehousingStatusEnquiryResult result = new OutsideWarehousingStatusEnquiryResult()
            {
                Success = "true",
                ErrorId = null,
                ErrorInfo = null,
                //WarehousingId = WarehousingId,
                //WarehousingType = warehousingType,
                WarehousingStatusInfoList = JsonConvert.SerializeObject(statusInfoList.ToArray()),
                IsNormalWarehousing = mesTask.WorkStatus == MESTaskWorkStatus.WorkComplated,
            };
            return JsonConvert.SerializeObject(result);

        }

        /// <summary>
        /// 出库状态查询
        /// </summary>
        [HttpGet("WarehouseEntryStatusEnquiry")]
        //public OutsideWarehouseEntryStatusEnquiryResult WarehouseEntryStatusEnquiry(OutsideWarehouseEntryStatusEnquiryArg arg)
        public string WarehouseEntryStatusEnquiry(string WarehouseEntryId, string WarehouseEntryType)
        {

            Wms_mestask mesTask = _sqlClient.Queryable<Wms_mestask>().First(x => x.WarehousingId == WarehouseEntryId && x.WarehousingType == WarehouseEntryType);
            if (mesTask == null)
            {
                OutsideWarehouseEntryStatusEnquiryResult error = new OutsideWarehouseEntryStatusEnquiryResult()
                {
                    Success = "false",
                    ErrorId = PubMessages.E3000_MES_STOCKINTASK_NOTFOUND.Code.ToString(),
                    ErrorInfo = PubMessages.E3000_MES_STOCKINTASK_NOTFOUND.Message
                };
                return JsonConvert.SerializeObject(error);
            }

            var stockoutList = _sqlClient.Queryable<Wms_stockout, Wms_warehouse>(
                (s, w) => new object[] {
                   JoinType.Left,s.WarehouseId==w.WarehouseId
                 })
                 .Where((s, w) => s.MesTaskId == mesTask.MesTaskId)
                 .Select((s, w) => new {
                     s.WarehouseId,
                     w.WarehouseName,
                     s.StockOutId,
                     s.StockOutNo,
                     s.StockOutStatus,
                     s.StockOutType
                 })
                .ToList();
            List<WarehousingStatusInfo> statusInfoList = new List<WarehousingStatusInfo>();
            foreach (var stockin in stockoutList)
            {
                var stockoutDetailList = _sqlClient.Queryable<Wms_stockoutdetail, Wms_stockoutdetail_box, Wms_inventorybox, Wms_material>(
                   (sid, sidb, ib, m) => new object[] {
                       JoinType.Left,sid.StockOutDetailId == sidb.StockOutDetailId,
                       JoinType.Left,sidb.InventoryBoxId == ib.InventoryBoxId,
                       JoinType.Left,sid.MaterialId == m.MaterialId,
                   }
                )
                .Where((sid, sidb, ib, m) => sid.StockOutId == stockin.StockOutId)
                .Select((sid, sidb, ib, m) => new {
                    sid.WarehouseId,
                    sid.MaterialId,
                    m.MaterialNo,
                    m.MaterialOnlyId,
                    sid.StockOutDetailId,
                    sidb.InventoryBoxId,
                    ib.InventoryBoxNo,
                    sidb.Position,
                    sidb.Qty,
                    sid.Status,
                    sid.CreateBy,
                    sid.CreateDate,
                    sid.ModifiedBy,
                    sid.ModifiedDate
                }).MergeTable().ToList();
                foreach (var detail in stockoutDetailList)
                {
                    statusInfoList.Add(new WarehousingStatusInfo()
                    {
                        IsNormalWarehousing = detail.Status == StockInStatus.task_finish.ToByte(),
                        WarehouseId = detail.WarehouseId.ToString(),
                        WarehousePosition = null,
                        WarehouseName = stockin.WarehouseName,
                        InventoryBoxNo = detail.InventoryBoxNo,
                        Position = Convert.ToString(detail.Position),
                        StorageRackPosition = "",
                        SuppliesId = string.IsNullOrWhiteSpace(detail.MaterialOnlyId) ? detail.MaterialId.ToString() : detail.MaterialOnlyId.ToString(),
                        RefreshStock = 0,
                        WarehousingStep = ((StockInStatus)detail.Status).ToString(),
                        WarehousingFinishTime = detail.ModifiedDate?.ToString("yyyyMMddHHmmss")
                    });
                }
            }

            OutsideWarehouseEntryStatusEnquiryResult result = new OutsideWarehouseEntryStatusEnquiryResult()
            {
                Success = "true",
                ErrorId = null,
                ErrorInfo = null,
                //WarehouseEntryId = arg.WarehouseEntryId,
                //WarehouseEntryType = arg.WarehouseEntryType,
                WarehouseEntryStatusInfoList = JsonConvert.SerializeObject(statusInfoList.ToArray()),
                IsNormalWarehouseEntry = mesTask.WorkStatus == MESTaskWorkStatus.WorkComplated,
            };
            return JsonConvert.SerializeObject(result);

        }

    }

    [ServiceContract]
    public interface IMESHookController
    {
        /// <summary>
        /// 状态确认
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        string Ping();

        /// <summary>
        /// 入库
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [OperationContract]
        //OutsideStockInResult Warehousing(OutsideStockInDto data);
        string Warehousing(String WarehousingId, String WarehousingType, String WarehousingTime, String ProductionPlanId, String BatchPlanId, String WorkAreaName, String SuppliesKinds, String SuppliesInfoList);

        /// <summary>
        /// 出库
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [OperationContract]
        //OutsideStockOutResult WarehouseEntry(OutsideStockOutDto data);
        string WarehouseEntry(String WarehouseEntryId, String WarehouseEntryType, String WarehouseEntryTime, String ProductionPlanId, String BatchPlanId, String WorkStationId, String WorkAreaName, String SuppliesKinds, String SuppliesInfoList);

        /// <summary>
        /// 物料库存查询
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        [OperationContract]
        //OutsideMaterialStockEnquiryResult MaterialStockEnquiry(OutsideMaterialStockEnquiryArg arg);
        string MaterialStockEnquiry(String SuppliesId, String SuppliesName, String SuppliesType, String SuppliesUnit);

        /// <summary>
        /// 物流控制
        /// </summary>
        [OperationContract]
        //OutsideLogisticsControlResult LogisticsControl(OutsideLogisticsControlArg arg);
        string LogisticsControl(String LogisticsId, String StartPoint, String Destination);

        /// <summary>
        /// 物流（状态）查询
        /// </summary>
        [OperationContract]
        //OutsideLogisticsEnquiryResult LogisticsEnquiry(OutsideLogisticsEnquiryArg arg);
        string LogisticsEnquiry(String LogisticsId, String EquipmentId, String EquipmentName);

        /// <summary>
        /// 入库状态查询
        /// </summary>
        [OperationContract]
        //OutsideWarehousingStatusEnquiryResult WarehousingStatusEnquiry(OutsideWarehousingStatusEnquiryArg arg);
        string WarehousingStatusEnquiry(string WarehousingId, string WarehousingType);

        /// <summary>
        /// 出库状态查询
        /// </summary>
        [OperationContract]
        //OutsideWarehouseEntryStatusEnquiryResult WarehouseEntryStatusEnquiry(OutsideWarehouseEntryStatusEnquiryArg arg);
        string WarehouseEntryStatusEnquiry(string WarehouseEntryId, string WarehouseEntryType);
    }
}
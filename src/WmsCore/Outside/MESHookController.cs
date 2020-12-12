using IServices;
using IServices.Outside;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using YL.Core.Dto;
using YL.Core.Entity;
using YL.NetCore.Attributes;
using YL.Utils;
using YL.Utils.Check;
using YL.Utils.Env;
using YL.Utils.Extensions;
using YL.Utils.Pub;
using static YL.NetCore.Attributes.OutsideLogAttribute;

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
        private ILogger _logger;

        public MESHookController(
            SqlSugarClient sqlClient,
            IWms_mastaskServices mastaskServices,
            IWms_materialServices materialServices,
            ISys_serialnumServices serialnumServices,
            ISys_dictServices dictServices,
            IWms_stockinServices stockinService,
            IWms_stockindetailServices stockinDetailServices,
            IWms_stockoutServices stockoutServices,
            IWms_stockoutdetailServices stockoutDetailServices,
            ILogger<MESHookController> logger)
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
            _logger = logger;
        }

        [HttpGet("Ping")]
        public string Ping()
        {
            return "OK";
        }

        [OutsideLog]
        [HttpPost("Warehousing")]
        //public OutsideStockInResult Warehousing([FromBody]OutsideStockInDto data)
        public string Warehousing(string WarehousingId, string WarehousingType, string WarehousingTime, string ProductionPlanId, string BatchPlanId, string WorkAreaName, string SuppliesKinds, string SuppliesInfoList)
        {
            OutsideStockInDto data = null;
            string resultStr = null;
            try
            {
                data = OutsideStockInDto.Create(
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
                OutsideWarehousingMaterialDto[] suppliesInfos = JsonConvert.DeserializeObject<OutsideWarehousingMaterialDto[]>(jsonSuppliesInfoStr);
                Wms_mestask mesTask = new Wms_mestask()
                {
                    MesTaskId = PubId.SnowflakeId,
                    MesTaskType = MESTaskTypes.StockIn,
                    WarehousingId = data.WarehousingId, //入库单编号
                    WarehousingType = data.WarehousingType, //入库类型
                    WarehousingTime = Convert.ToDateTime(data.WarehousingTime),   //入库时间
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
                    resultStr = JsonConvert.SerializeObject(result);
                    return resultStr;
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
                    resultStr = JsonConvert.SerializeObject(result);
                    return resultStr;
                }
            }
            catch (Exception ex)
            { 
                _sqlClient.RollbackTran();
                OutsideStockInResult result = new OutsideStockInResult()
                {
                    Success = false,
                    ErrorId = "-1",
                    ErrorInfo = ex.ToString()
                    //  WarehousingId = Warehousingid,
                    //  WarehousingTime = Warehousingtime
                };
                resultStr = JsonConvert.SerializeObject(result);
                return resultStr;
            }
            finally {
                LogRequest("Warehousing", data, resultStr);
            }

        }

        private RouteData CreateWMSStockin(Wms_mestask mesTask, OutsideWarehousingMaterialDto[] suppliesInfoList)
        {
            Dictionary<long, List<Wms_WarehousingMaterialInventoryDto>> map = new Dictionary<long, List<Wms_WarehousingMaterialInventoryDto>>();
            Sys_dict[] typeDicts = _sqlClient.Queryable<Sys_dict>()
                     .Where(x => x.DictType == PubDictType.material.ToByte().ToString())
                     .ToArray();

            foreach (OutsideWarehousingMaterialDto materialDto in suppliesInfoList)
            {

                //Sys_dict typeDict = typeDicts.FirstOrDefault(x => x.DictName == materialDto.SuppliesType);
                //if (typeDict == null)
                //{
                //    return RouteData<Wms_material>.From(PubMessages.E1001_SUPPLIESTYPE_NOTFOUND, $"SuppliesType = {materialDto.SuppliesType}");
                //}
                //else if (typeDict.WarehouseId == null)
                //{
                //    return RouteData<Wms_material>.From(PubMessages.E1002_SUPPLIESTYPE_WAREHOUSEID_NOTSET, $"SuppliesType = {materialDto.SuppliesType}");
                //}
                //long warehouseId = typeDict.WarehouseId.Value;


                string warehouseNo = string.IsNullOrWhiteSpace(materialDto.WarehouseId) ? "A00" : materialDto.WarehouseId;
                //MES的WarehouseID对应WMS的WarehouseNo
                Wms_warehouse warehouse = WMSApiManager.GetWarehouse(warehouseNo);
                if (warehouse == null)
                {
                    return RouteData<Wms_material>.From(PubMessages.E1026_SUPPLIES_WAREHOUSEID_NOTFOUND, $"warehouseId = {warehouseNo}");
                }
                long warehouseId = warehouse.WarehouseId;
                List<Wms_WarehousingMaterialInventoryDto> warehouseMaterialList = null;
                if (map.ContainsKey(warehouseId))
                {
                    warehouseMaterialList = map[warehouseId];
                }
                else
                {
                    warehouseMaterialList = new List<Wms_WarehousingMaterialInventoryDto>();
                    map.Add(warehouseId, warehouseMaterialList);
                }
                warehouseMaterialList.Add(new Wms_WarehousingMaterialInventoryDto()
                {
                    MaterialId = "-1",
                    SubWarehousingId = materialDto.SubWarehouseId,
                    UniqueIndex = materialDto.UniqueIndex,
                    MaterialOnlyId = materialDto.SuppliesOnlyId,
                    MaterialNo = materialDto.SuppliesId,
                    MaterialName = materialDto.SuppliesName,
                    MaterialType = materialDto.SuppliesType,
                    Qty = materialDto.SuppliesNumber,
                    Unit = materialDto.Unit
                });
            }

            List<RouteData<OutsideStockInRequestResult[]>> result = new List<RouteData<OutsideStockInRequestResult[]>>();
            foreach (KeyValuePair<long, List<Wms_WarehousingMaterialInventoryDto>> keyValue in map)
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
                    _logger.LogInformation("小WMS的StockIn开始", "");
                    RouteData<OutsideStockInRequestResult[]> data = proxy.StockIn(request).GetAwaiter().GetResult();
                    _logger.LogInformation("小WMS的StockIn结束", "");
                    if (!data.IsSccuess)
                    {
                        string message = $"仓库{keyValue.Key}下发入库任务失败,Code={data.Code},Message={data.Message}";
                        _logger.LogError(message);
                        result.Add(data);
                        return new RouteData() {
                            Code = -1,
                            Message = message
                        };
                    }
                    else
                    {
                        _logger.LogInformation($"仓库{keyValue.Key}下发入库任务成功");
                    }
                }
                catch (Exception ex)
                {
                    string message = $"仓库{keyValue.Key}下发入库任务发生异常";
                    _logger.LogError(ex, message);
                    return new RouteData()
                    {
                        Code = -1,
                        Message = message
                    };
                }
            }

            return new RouteData();

        }


        /// <summary>
        /// 出库处理
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [OutsideLog]
        [HttpPost("WarehouseEntry")]
        //public OutsideStockOutResult WarehouseEntry([FromBody]OutsideStockOutDto data)
        public string WarehouseEntry(
            string WarehouseEntryId,
            string WarehouseEntryType,
            string WarehouseEntryTime,
            string ProductionPlanId,
            string TotalWorkOrder,
            string BatchNumber,
            string BatchPlanId,
            string WorkAreaName,
            string WorkStationId,
            string SuppliesKinds,
            string SuppliesInfoList)
        {
            string resultStr = null;
            OutsideStockOutDto data = null;
            try
            {
                data = OutsideStockOutDto.Create(
                       Guard.GuardEmpty(() => WarehouseEntryId),
                       Guard.GuardEmpty(() => WarehouseEntryType),
                       Guard.GuardEmpty(() => WarehouseEntryTime),
                       Guard.GuardEmpty(() => ProductionPlanId),
                       Guard.GuardEmpty(() => TotalWorkOrder),
                       Guard.GuardEmpty(() => BatchNumber),
                       Guard.GuardEmpty(() => BatchPlanId),
                       Guard.GuardEmpty(() => WorkAreaName),
                       Guard.GuardEmpty(() => WorkStationId),
                       Guard.GuardInteger(() => SuppliesKinds),
                       Guard.GuardEmpty(() => SuppliesInfoList)
                   );


                _sqlClient.BeginTran();
                string jsonSuppliesInfoStr = data.SuppliesInfoList;
                OutsideWarehouseEntryMaterialDto[] suppliesInfos = JsonConvert.DeserializeObject<OutsideWarehouseEntryMaterialDto[]>(jsonSuppliesInfoStr);
                Wms_mestask mesTask = new Wms_mestask()
                {
                    MesTaskId = PubId.SnowflakeId,
                    MesTaskType = MESTaskTypes.StockOut,
                    WarehousingId = data.WarehouseEntryId, //入库单编号
                    WarehousingType = data.WarehouseEntryType, //入库类型
                    WarehousingTime = Convert.ToDateTime(data.WarehouseEntryTime),   //入库时间
                    ProductionPlanId = data.ProductionPlanId, //生产令号
                    TotalWorkOrder = data.TotalWorkOrder,
                    BatchNumber = data.BatchNumber,
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
                    resultStr = JsonConvert.SerializeObject(result);
                    return resultStr;
                }
            }
            catch (Exception ex)
            {
                _sqlClient.RollbackTran();
                OutsideStockOutResult result = new OutsideStockOutResult()
                {
                    Success = false,
                    ErrorId = "-1",
                    ErrorInfo = ex.ToString(),
                    //WarehouseEntryId = warehouseEntryid,
                    //WarehouseEntryTime = data.WarehouseEntryTime
                };
                resultStr = JsonConvert.SerializeObject(result);
                return resultStr;
            }
            finally {
                this.LogRequest("WarehouseEntry", data, resultStr);
            }
        }

        private RouteData CreateWMSStockout(Wms_mestask mesTask, OutsideWarehouseEntryMaterialDto[] suppliesInfoList)
        {
            Dictionary<long, List<Wms_WarehouseEntryMaterialInventoryDto>> map =
                new Dictionary<long, List<Wms_WarehouseEntryMaterialInventoryDto>>();
            foreach (OutsideWarehouseEntryMaterialDto materialDto in suppliesInfoList)
            {
                //Sys_dict typeDict = _sqlClient.Queryable<Sys_dict>()
                //       .First(x => x.DictType == PubDictType.material.ToByte().ToString() && x.DictName == materialDto.SuppliesType);
                //if (typeDict == null)
                //{
                //    return RouteData<Wms_material>.From(PubMessages.E1001_SUPPLIESTYPE_NOTFOUND, $"SuppliesType = {materialDto.SuppliesType}");
                //}
                //else if (typeDict.WarehouseId == null)
                //{
                //    return RouteData<Wms_material>.From(PubMessages.E1002_SUPPLIESTYPE_WAREHOUSEID_NOTSET, $"SuppliesType = {materialDto.SuppliesType}");
                //}
                //long warehouseId = typeDict.WarehouseId.Value;


                string warehouseNo = string.IsNullOrWhiteSpace(materialDto.WarehouseId) ? "A00" : materialDto.WarehouseId;
                //MES的WarehouseID对应WMS的WarehouseNo
                Wms_warehouse warehouse = WMSApiManager.GetWarehouse(warehouseNo);
                if (warehouse == null)
                {
                    return RouteData<Wms_material>.From(PubMessages.E1026_SUPPLIES_WAREHOUSEID_NOTFOUND, $"warehouseId = {warehouseNo}");
                }
                long warehouseId = warehouse.WarehouseId;
                List<Wms_WarehouseEntryMaterialInventoryDto> warehouseMaterialList = null;
                if (map.ContainsKey(warehouseId))
                {
                    warehouseMaterialList = map[warehouseId];
                }
                else
                {
                    warehouseMaterialList = new List<Wms_WarehouseEntryMaterialInventoryDto>();
                    map.Add(warehouseId, warehouseMaterialList);
                }
                warehouseMaterialList.Add(new Wms_WarehouseEntryMaterialInventoryDto()
                {
                    MaterialId = "-1",
                    SubWarehouseEntryId = materialDto.SubWarehouseEntryId,
                    UniqueIndex = materialDto.UniqueIndex,
                    MaterialOnlyId = materialDto.SuppliesOnlyId,
                    MaterialNo = materialDto.SuppliesId,
                    MaterialName = materialDto.SuppliesName,
                    MaterialType = materialDto.SuppliesType,
                    Qty = materialDto.SuppliesNumber,
                    Unit = materialDto.Unit
                });
            }

            List<RouteData> result = new List<RouteData>();
            foreach (KeyValuePair<long, List<Wms_WarehouseEntryMaterialInventoryDto>> keyValue in map)
            {
                try
                {
                    IWMSBaseApiAccessor proxy = WMSApiManager.GetBaseApiAccessor(keyValue.Key.ToString(), _sqlClient);
                    OutsideStockOutRequestDto request = new OutsideStockOutRequestDto()
                    {
                        MesTaskId = mesTask.MesTaskId,
                        WarehouseEntryId = mesTask.WarehousingId,
                        WarehouseEntryTime = mesTask.WarehousingTime.ToString(PubConst.Format_DateTime),
                        WarehouseEntryType = mesTask.WarehousingType,
                        WarehouseId = keyValue.Key,
                        OrderNo = mesTask.ProductionPlanId,
                        WorkNo = mesTask.TotalWorkOrder,
                        BatchNumber = mesTask.BatchNumber,
                        WorkAreaName = mesTask.WorkAreaName,
                        WorkStationId = mesTask.WorkStationId,
                        BatchPlanId = mesTask.BatchPlanId,
                        MaterialList = keyValue.Value.ToArray(),
                    };
                    _logger.LogInformation("小WMS的StockOut开始", "");
                    RouteData data = proxy.StockOut(request).GetAwaiter().GetResult();
                    _logger.LogInformation("小WMS的StockOut结束", "");
                    if (!data.IsSccuess)
                    {
                        string message = $"仓库{keyValue.Key}下发出库任务失败,Code={data.Code},Message={data.Message}";
                        _logger.LogError(message);
                        result.Add(data);
                        return new RouteData()
                        {
                            Code = -1,
                            Message = message
                        };
                    }
                    else
                    {
                        _logger.LogInformation($"仓库{keyValue.Key}下发出库任务成功");
                    }
                }
                catch (Exception ex)
                {
                    string message = $"仓库{keyValue.Key}下发出库任务发生异常";
                    _logger.LogError(ex, message);
                    return new RouteData()
                    {
                        Code = -1,
                        Message = message
                    };
                }
            }


            return new RouteData();
        }


        /// <summary>
        /// 物料库存查询
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        [OutsideLog]
        [HttpGet("MaterialStockEnquiry")]
        //public OutsideMaterialStockEnquiryResult MaterialStockEnquiry(OutsideMaterialStockEnquiryArg arg)
        public string MaterialStockEnquiry(string SuppliesId, string SuppliesName, string SuppliesType, string SuppliesUnit)
        {
            var request = new { SuppliesId, SuppliesName, SuppliesType, SuppliesUnit };
            string resultStr = null;
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
                resultStr = JsonConvert.SerializeObject(result);
                return resultStr;
            }
            catch (Exception)
            {
                OutsideMaterialStockEnquiryResult result = new OutsideMaterialStockEnquiryResult()
                {
                    ErrorID = "-1",
                    Success = false,
                };
                resultStr = JsonConvert.SerializeObject(result);
                return resultStr;
            }
            finally {
                this.LogRequest("MaterialStockEnquiry", request, resultStr);
            }
        }

        /// <summary>
        /// 物流控制
        /// </summary>
        [OutsideLog]
        [HttpPost("LogisticsControl")]
        //public OutsideLogisticsControlResult LogisticsControl([FromBody]OutsideLogisticsControlArg arg)
        public string LogisticsControl(string LogisticsId, string StartPoint, string Destination1, string Destination2, string InventoryBoxSize)
        {
            var result = WCSApiAccessor.Instance.LogisticsControl(
                OutsideLogisticsControlArg.Create(LogisticsId, StartPoint, Destination1, Destination2, InventoryBoxSize)).Result;

            //var result = new OutsideLogisticsControlResult() { EquipmentId = "1", EquipmentName = "EquipmentName-1" };
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 物流（状态）查询
        /// </summary>
        [HttpGet("LogisticsEnquiry")]
        //public OutsideLogisticsEnquiryResult LogisticsEnquiry(OutsideLogisticsEnquiryArg arg)
        public string LogisticsEnquiry(string LogisticsId, string EquipmentId, string EquipmentName)
        {
            var result = WCSApiAccessor.Instance.LogisticsEnquiry(
                OutsideLogisticsEnquiryArg.Create(LogisticsId, EquipmentId, EquipmentName)).Result;
            //var result = new OutsideLogisticsEnquiryArg() { LogisticsId = LogisticsId, EquipmentId = "1", EquipmentName = "EquipmentName-1" };
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// 入库状态查询
        /// </summary>
        [OutsideLog]
        [HttpGet("WarehousingStatusEnquiry")]
        //public OutsideWarehousingStatusEnquiryResult WarehousingStatusEnquiry(OutsideWarehousingStatusEnquiryArg arg)
        public string WarehousingStatusEnquiry(string WarehousingId, string WarehousingType)
        {
            var request = new { WarehousingId, WarehousingType };
            string resultStr = null;
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
                 .Select((s, w) => new
                 {
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
                .Select((sid, sidb, ib, m) => new
                {
                    sid.WarehouseId,
                    sid.SubWarehousingId,
                    sid.UniqueIndex,
                    sid.MaterialId,
                    m.MaterialNo,
                    m.MaterialOnlyId,
                    sid.StockInDetailId,
                    InventoryBoxId = (int?)sidb.InventoryBoxId,
                    ib.InventoryBoxNo,
                    Position = (int?)sidb.Position,
                    Qty = (int?)sidb.Qty,
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
                        //WarehouseId = detail.WarehouseId.ToString(),
                        WarehouseId = WMSApiManager.GetWarehouse(detail.WarehouseId).WarehouseNo,
                        WarehousePosition = null,
                        WarehouseName = stockin.WarehouseName,
                        InventoryBoxNo = detail.InventoryBoxNo,
                        Position = Convert.ToString(detail.Position),//TODO
                        StorageRackPosition = "",
                        SubWarehousingId = detail.SubWarehousingId,
                        UniqueIndex = detail.UniqueIndex,
                        SuppliesId = string.IsNullOrWhiteSpace(detail.MaterialOnlyId) ? detail.MaterialNo.ToString() : detail.MaterialOnlyId.ToString(),
                        RefreshStock = 0, //TODO
                        WarehousingStep = ((StockInStatus)detail.Status).ToString(),
                        WarehousingFinishTime = detail.Status == StockInStatus.task_finish.ToByte() ? detail.ModifiedDate?.ToString("yyyy-MM-dd HH:mm:ss") : ""
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
                WarehousingStatusInfoList = statusInfoList.ToArray(),
                IsNormalWarehousing = mesTask.WorkStatus == MESTaskWorkStatus.WorkComplated,
            };
            resultStr = JsonConvert.SerializeObject(result);
            this.LogRequest("WarehousingStatusEnquiry", request, result);
            return resultStr;

        }

        /// <summary>
        /// 出库状态查询
        /// </summary>
        [OutsideLog]
        [HttpGet("WarehouseEntryStatusEnquiry")]
        //public OutsideWarehouseEntryStatusEnquiryResult WarehouseEntryStatusEnquiry(OutsideWarehouseEntryStatusEnquiryArg arg)
        public string WarehouseEntryStatusEnquiry(string WarehouseEntryId, string WarehouseEntryType)
        {
            var request = new { WarehouseEntryId, WarehouseEntryType };
            string resultStr = null;
            
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
                 .Select((s, w) => new
                 {
                     s.WarehouseId,
                     w.WarehouseName,
                     s.StockOutId,
                     s.StockOutNo,
                     s.StockOutStatus,
                     s.StockOutType
                 })
                .ToList();
            List<WarehouseEntryStatusInfo> statusInfoList = new List<WarehouseEntryStatusInfo>();
            foreach (var stockin in stockoutList)
            {
                var stockoutDetailList = _sqlClient.Queryable<Wms_stockoutdetail, Wms_stockoutdetail_box, Wms_inventorybox, Wms_material>(
                   (sod, sodb, ib, m) => new object[] {
                       JoinType.Left,sod.StockOutDetailId == sodb.StockOutDetailId,
                       JoinType.Left,sodb.InventoryBoxId == ib.InventoryBoxId,
                       JoinType.Left,sod.MaterialId == m.MaterialId,
                   }
                )
                .Where((sod, sodb, ib, m) => sod.StockOutId == stockin.StockOutId)
                .Select((sod, sodb, ib, m) => new
                {
                    sod.WarehouseId,
                    sod.SubWarehouseEntryId,
                    sod.UniqueIndex,
                    sod.MaterialId,
                    m.MaterialNo,
                    m.MaterialOnlyId,
                    sod.StockOutDetailId,
                    InventoryBoxId = (int?)sodb.InventoryBoxId,
                    ib.InventoryBoxNo,
                    Position = (int?)sodb.Position,
                    Qty = (int?)sodb.Qty,
                    sod.Status,
                    sod.CreateBy,
                    sod.CreateDate,
                    sod.ModifiedBy,
                    sod.ModifiedDate
                }).MergeTable().ToList();
                foreach (var detail in stockoutDetailList)
                {
                    statusInfoList.Add(new WarehouseEntryStatusInfo()
                    {
                        IsNormalWarehouseEntry = detail.Status == StockInStatus.task_finish.ToByte(), 
                        SubWarehouseEntryId = detail.SubWarehouseEntryId,
                        UniqueIndex = detail.UniqueIndex,
                        SuppliesId = string.IsNullOrWhiteSpace(detail.MaterialOnlyId) ? detail.MaterialNo.ToString() : detail.MaterialOnlyId.ToString(),
                        RefreshStock = 0, //TODO
                        WorkAreaName = "", //TODO
                        WarehouseEntryStep = ((StockInStatus)detail.Status).ToString(),
                        WarehouseEntryFinishTime = detail.Status == StockInStatus.task_finish.ToByte() ? detail.ModifiedDate?.ToString("yyyy-MM-dd HH:mm:ss") : ""
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
                WarehouseEntryStatusInfoList = statusInfoList.ToArray(),
                IsNormalWarehouseEntry = mesTask.WorkStatus == MESTaskWorkStatus.WorkComplated,
            };
            resultStr = JsonConvert.SerializeObject(result);
            LogRequest("WarehouseEntryStatusEnquiry", request,resultStr);
            return resultStr;


        }

        /// <summary>
        /// 盘库任务下发
        /// </summary>
        /// <param name="StockInventoryId">盘点单号</param>
        /// <param name="Year">年份</param>
        /// <param name="Month">月份</param>
        /// <param name="WarehouseID">仓库编号</param>
        /// <param name="SuppliesInfoList">物料信息</param>
        /// <returns></returns>
        [OutsideLog]
        [HttpPost("StockInventory")]
        public string StockInventory(
            string StockInventoryId, string Year, string Month, string WarehouseID, string SuppliesInfoList)
        {
            var request = new { StockInventoryId, Year , Month, WarehouseID, SuppliesInfoList };
            string resultStr = null;
            try
            {
                Guard.GuardEmpty(() => StockInventoryId);
                //Guard.GuardEmpty(() => WarehouseID); 
                DateTime stockCountDate = Guard.GuardDate(() => Year, () => Month);
                OutsideStockCountMaterialDto_MES[] materialList = Guard.GuardType<OutsideStockCountMaterialDto_MES[]>(() => SuppliesInfoList);
                OutsideStockCountMaterial[] wmsMaterialList = materialList.Select(
                    x => new OutsideStockCountMaterial()
                    {
                        MaterialNo = x.SuppliesId,
                        MaterialOnlyId = x.SuppliesOnlyId,
                        MaterialName = x.SuppliesName,
                        PrevNumber = Guard.GuardInteger(() => x.PrevNumber),
                        MaterialTypeName = x.SuppliesType,
                        UnitName = x.Unit
                    }
                ).ToArray();
                RouteData routedata = StockCountCore(StockInventoryId, stockCountDate, WarehouseID, wmsMaterialList);
                resultStr = JsonConvert.SerializeObject(new OutsideStockCountResultDto_MES()
                {
                    Success = true,
                    StockInventoryId = StockInventoryId,
                    //ErrorId = routedata.Code.ToString(),
                    ErrorId = routedata.Code == 0 ? "" : routedata.Code.ToString(),
                    ErrorInfo = routedata.Message
                });
                return resultStr;
            }
            catch (WMSException ex)
            {
                resultStr = JsonConvert.SerializeObject(new OutsideStockCountResultDto_MES()
                {
                    Success = false,
                    StockInventoryId = StockInventoryId,
                    ErrorId = ex.ErrorMessage.Code.ToString(),
                    ErrorInfo = ex.ErrorMessage.Message
                });
                return resultStr;
            }
            finally {
                this.LogRequest("StockInventory", request,resultStr);
            }
        }
         
        private RouteData StockCountCore(
            string StockInventoryId, DateTime StockCountDate, string WarehouseID, OutsideStockCountMaterial[] materials)
        {

            if (materials.Length == 0)
            {
                return YL.Core.Dto.RouteData.From(PubMessages.E2204_STOCKCOUNT_MATERIAL_ZERO);
            }

            string warehouseNo = string.IsNullOrWhiteSpace(WarehouseID) ? "A00" : WarehouseID;
            Wms_warehouse warehouse = WMSApiManager.GetWarehouse(warehouseNo);
            if (warehouse == null)
            {
                return YL.Core.Dto.RouteData.From(PubMessages.E1026_SUPPLIES_WAREHOUSEID_NOTFOUND, $"warehouseId = {warehouseNo}");
            }
            long warehouseId = warehouse.WarehouseId;
            IWMSBaseApiAccessor proxy = WMSApiManager.GetBaseApiAccessor(warehouseId.ToString(), _sqlClient);

            Wms_mestask mesTask = new Wms_mestask()
            {
                MesTaskId = PubId.SnowflakeId,
                MesTaskType = MESTaskTypes.StockCount,
                WarehousingId = StockInventoryId, //入库单编号
                WarehousingType = String.Empty, //入库类型
                WarehousingTime = StockCountDate,   //入库时间
                ProductionPlanId = String.Empty, //生产令号
                BatchPlanId = String.Empty, //批次号
                WorkAreaName = String.Empty, //作业区
                SuppliesKinds = materials.Length, //物料种类
                SuppliesInfoJson = "", // jsonSuppliesInfoStr, //物料信息
                WorkStatus = MESTaskWorkStatus.WaitPlan,      //等待计划
                NotifyStatus = MESTaskNotifyStatus.Requested, //已接收
                CreateDate = DateTime.Now
            };
            RouteData result = null;
            try
            {
                result = proxy.StockCount(new OutsideStockCountRequestDto()
                {
                    MesTaskId = mesTask.MesTaskId,
                    StockCountNo = StockInventoryId,
                    PlanDate = StockCountDate.ToString(PubConst.Format_Date),
                    MaterialList = materials
                }).GetAwaiter().GetResult();
                if (result == null || !result.IsSccuess)
                {
                    mesTask.WorkStatus = MESTaskWorkStatus.Failed;
                    mesTask.Remark = result.Message;
                }
            }
            catch (Exception ex)
            {
                mesTask.WorkStatus = MESTaskWorkStatus.Failed;
                mesTask.Remark = ex.Message;
            }

            _mastaskServices.Insert(mesTask);
            return result;

        }

        private void LogRequest(string target, object parameter,object result) {
            try
            {
                LogData logData = new LogData();
                logData.parameters = parameter;
                logData.logIp = GlobalCore.GetIp();
                logData.target = target;
                logData.result = result;

                String dir = Path.Combine(AppContext.BaseDirectory, "OutsideLog", target);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                String path = Path.Combine(dir, DateTime.Now.Ticks.ToString() + "_" + logData.logId.ToString());
                System.IO.File.WriteAllText(path, JsonConvert.SerializeObject(logData));
            }
            catch (Exception)
            {
            }
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
        string Warehousing(string WarehousingId, string WarehousingType, string WarehousingTime, string ProductionPlanId, string BatchPlanId, string WorkAreaName, string SuppliesKinds, string SuppliesInfoList);

        /// <summary>
        /// 出库
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [OperationContract]
        //OutsideStockOutResult WarehouseEntry(OutsideStockOutDto data);
        string WarehouseEntry(
            string WarehouseEntryId, string WarehouseEntryType, string WarehouseEntryTime,
            string ProductionPlanId, string TotalWorkOrder, string BatchNumber, string BatchPlanId, string WorkAreaName, string WorkStationId,
            string SuppliesKinds, string SuppliesInfoList);

        /// <summary>
        /// 物料库存查询
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        [OperationContract]
        //OutsideMaterialStockEnquiryResult MaterialStockEnquiry(OutsideMaterialStockEnquiryArg arg);
        string MaterialStockEnquiry(string SuppliesId, string SuppliesName, string SuppliesType, string SuppliesUnit);

        /// <summary>
        /// 物流控制
        /// </summary>
        [OperationContract]
        //OutsideLogisticsControlResult LogisticsControl(OutsideLogisticsControlArg arg);
        string LogisticsControl(string LogisticsId, string StartPoint, string Destination1, string Destination2, string InventoryBoxSize);

        /// <summary>
        /// 物流（状态）查询
        /// </summary>
        [OperationContract]
        //OutsideLogisticsEnquiryResult LogisticsEnquiry(OutsideLogisticsEnquiryArg arg);
        string LogisticsEnquiry(string LogisticsId, string EquipmentId, string EquipmentName);

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

        /// <summary>
        /// 盘库任务下发
        /// </summary>
        [OperationContract] 
        string StockInventory(string StockInventoryId, string Year, string Month, string WarehouseID, string SuppliesInfoList);
    }
}
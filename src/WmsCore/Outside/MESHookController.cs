using IServices;
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
        public OutsideStockInResult Warehousing([FromBody]OutsideStockInDto data)
        {
            try
            {
                _sqlClient.BeginTran();
                string jsonSuppliesInfo = JsonConvert.SerializeObject(data.SuppliesInfoList);
                Wms_mestask mesTask = new Wms_mestask()
                {
                    MesTaskId = PubId.SnowflakeId,
                    MesTaskType = MesTaskTypes.StockIn.ToByte(),
                    WarehousingId = data.WarehousingId, //入库单编号
                    WarehousingType = data.WarehousingType, //入库类型
                    WarehousingTime = data.WarehousingTime.ToDateTime(),   //入库时间
                    ProductionPlanId = data.ProductionPlanId, //生产令号
                    BatchPlanId = data.BatchPlanId, //批次号
                    WorkAreaName = data.WorkAreaName, //作业区
                    SuppliesKinds = data.SuppliesKinds, //物料种类
                    SuppliesInfoJson = jsonSuppliesInfo, //物料信息
                    WorkStatus = MESTaskWorkStatus.WaitPlan.ToByte(),      //等待计划
                    NotifyStatus = MESTaskNotifyStatus.Requested.ToByte(), //已接收
                    CreateDate = DateTime.Now
                };
                _mastaskServices.Insert(mesTask);

                RouteData routeData = CreateWMSStockin(mesTask, data.SuppliesInfoList);
                if (!routeData.IsSccuess)
                {
                    _sqlClient.RollbackTran();
                    return new OutsideStockInResult()
                    {
                        ErrorId = routeData.CodeString,
                        ErrorInfo = routeData.Message,
                        Success = false,
                        WarehousingId = data.WarehousingId,
                        WarehousingTime = data.WarehousingTime
                    };
                }
                _sqlClient.CommitTran();
                return new OutsideStockInResult()
                {
                    Success = true,
                    ErrorId = string.Empty,
                    ErrorInfo = string.Empty,
                    WarehousingId = data.WarehousingId,
                    WarehousingTime = data.WarehousingTime
                };
            }
            catch(Exception ex)
            {
                _sqlClient.RollbackTran();
                return new OutsideStockInResult()
                {
                    Success = true,
                    ErrorId = "-1",
                    ErrorInfo = ex.ToString(),
                    WarehousingId = data.WarehousingId,
                    WarehousingTime = data.WarehousingTime
                };
            }
            
        }

        private RouteData CreateWMSStockin(Wms_mestask mesTask, OutsideMaterialDto[] suppliesInfoList)
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
                    MaterialId = -1,
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
                    IWMSApiProxy proxy = WMSApiAccessor.Get(keyValue.Key.ToString());
                    OutsideStockInRequestDto request = new OutsideStockInRequestDto()
                    {
                        MesTaskId = mesTask.MesTaskId,
                        WarehousingId = mesTask.WarehousingId,
                        WarehousingTime = mesTask.WarehousingTime.ToString(PubConst.Format_DateTime),
                        WarehousingType = mesTask.WarehousingType,
                        OrderNo = mesTask.ProductionPlanId,
                        WorkAreaName = mesTask.WorkAreaName,
                        BatchPlanId = mesTask.BatchPlanId,
                        MaterialList = keyValue.Value.ToArray(),
                    };
                    RouteData data = proxy.StockIn(request).GetAwaiter().GetResult();
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
        public OutsideStockOutResult WarehouseEntry([FromBody]OutsideStockOutDto data)
        {
            try
            {
                _sqlClient.BeginTran();
                string jsonSuppliesInfo = JsonConvert.SerializeObject(data.SuppliesInfoList);
                Wms_mestask mesTask = new Wms_mestask()
                {
                    MesTaskId = PubId.SnowflakeId,
                    MesTaskType = MesTaskTypes.StockOut.ToByte(),
                    WarehousingId = data.WarehouseEntryId, //入库单编号
                    WarehousingType = data.WarehouseEntryType, //入库类型
                    WarehousingTime = data.WarehouseEntryTime.ToDateTime(),   //入库时间
                    ProductionPlanId = data.ProductionPlanId, //生产令号
                    BatchPlanId = data.BatchPlanId, //批次号
                    WorkAreaName = data.WorkAreaName, //作业区
                    WorkStationId = data.WorkStationId, //工位号
                    SuppliesKinds = data.SuppliesKinds, //物料种类
                    SuppliesInfoJson = jsonSuppliesInfo, //物料信息
                    WorkStatus = MESTaskWorkStatus.WaitPlan.ToByte(),      //等待计划
                    NotifyStatus = MESTaskNotifyStatus.Requested.ToByte(), //已接收
                    CreateDate = DateTime.Now
                };
                if(_sqlClient.Insertable(mesTask).ExecuteCommand() == 0)
                {
                    throw new Exception("mesTask更新异常");
                }

                RouteData routeData = CreateWMSStockout(mesTask, data.SuppliesInfoList);
                if (!routeData.IsSccuess)
                {
                    _sqlClient.RollbackTran();
                    return new OutsideStockOutResult()
                    {
                        ErrorId = routeData.CodeString,
                        ErrorInfo = routeData.Message,
                        Success = false,
                        WarehouseEntryId = data.WarehouseEntryId,
                        //WarehouseEntryTime = data.WarehouseEntryTime
                    };
                }
                _sqlClient.CommitTran();
                return new OutsideStockOutResult()
                {
                    Success = true,
                    ErrorId = string.Empty,
                    ErrorInfo = string.Empty,
                    WarehouseEntryId = data.WarehouseEntryId,
                    //WarehouseEntryTime = data.WarehouseEntryTime
                };
            }
            catch (Exception ex)
            {
                _sqlClient.RollbackTran();
                return new OutsideStockOutResult()
                {
                    Success = true,
                    ErrorId = "-1",
                    ErrorInfo = ex.ToString(),
                    WarehouseEntryId = data.WarehouseEntryId,
                    //WarehouseEntryTime = data.WarehouseEntryTime
                };
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
                    MaterialId = -1,
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
                    IWMSApiProxy proxy = WMSApiAccessor.Get(keyValue.Key.ToString());
                    OutsideStockOutRequestDto request = new OutsideStockOutRequestDto()
                    {
                        MesTaskId = mesTask.MesTaskId,
                        WarehousingId = mesTask.WarehousingId,
                        WarehousingTime = mesTask.WarehousingTime.ToString(PubConst.Format_DateTime),
                        WarehousingType = mesTask.WarehousingType,
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
                catch(Exception ex)
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
        public OutsideMaterialStockEnquiryResult MaterialStockEnquiry(OutsideMaterialStockEnquiryArg arg)
        {
            return null;
        }

        /// <summary>
        /// 物流控制
        /// </summary>
        [HttpPost("LogisticsControl")]
        public OutsideLogisticsControlResult LogisticsControl([FromBody]OutsideLogisticsControlArg arg)
        {
            return WCSApiAccessor.Instance.LogisticsControl(arg).Result;
        }

        /// <summary>
        /// 物流（状态）查询
        /// </summary>
        [HttpGet("LogisticsEnquiry")]
        public OutsideLogisticsEnquiryResult LogisticsEnquiry(OutsideLogisticsEnquiryArg arg)
        {
            return WCSApiAccessor.Instance.LogisticsEnquiry(arg).Result;
        }

        /// <summary>
        /// 入库状态查询
        /// </summary>
        [HttpGet("WarehousingStatusEnquiry")]
        public OutsideWarehousingStatusEnquiryResult WarehousingStatusEnquiry(OutsideWarehousingStatusEnquiryArg arg)
        {
            Wms_mestask mesTask = _sqlClient.Queryable<Wms_mestask>().First(x => x.WarehousingId == arg.WarehousingId && x.WarehousingType == arg.WarehousingType);
            if(mesTask == null)
            {
                return new OutsideWarehousingStatusEnquiryResult()
                {
                    Success = "false",
                    ErrorId = PubMessages.E3000_MES_STOCKINTASK_NOTFOUND.Code.ToString(),
                    ErrorInfo = PubMessages.E3000_MES_STOCKINTASK_NOTFOUND.Message
                };
            }

            var stockinList = _sqlClient.Queryable<Wms_stockin,Wms_warehouse>(
                (s, w) => new object[] {
                   JoinType.Left,s.WarehouseId==w.WarehouseId
                 })
                 .Where((s, w) => s.MesTaskId == mesTask.MesTaskId )
                 .Select((s,w) => new {
                     s.WarehouseId,
                     w.WarehouseName,
                     s.StockInId,
                     s.StockInNo,
                     s.StockInStatus,
                     s.StockInType
                 })
                .ToList();
            List<WarehousingStatusInfo> statusInfoList = new List<WarehousingStatusInfo>();
            foreach(var stockin in stockinList)
            {
                var stockinDetailList = _sqlClient.Queryable<Wms_stockindetail,Wms_stockindetail_box,Wms_inventorybox,Wms_material>(
                   (sid,sidb,ib,m) =>  new object[] {
                       JoinType.Left,sid.StockInDetailId == sidb.StockinDetailId,
                       JoinType.Left,sidb.InventoryBoxId == ib.InventoryBoxId,
                       JoinType.Left,sid.MaterialId == m.MaterialId,
                   }
                )
                .Where((sid, sidb, ib, m) => sid.StockInId == stockin.StockInId)
                .Select( (sid,sidb,ib, m) => new {
                    sid.WarehouseId, 
                    sid.MaterialId,
                    m.MaterialNo,
                    m.MaterialOnlyId,
                    sid.StockInDetailId,
                    sidb.InventoryBoxId, 
                    ib.InventoryBoxNo,
                    sidb.Qty,
                    sid.Status,
                    sid.CreateBy,
                    sid.CreateDate,
                    sid.ModifiedBy,
                    sid.ModifiedDate
                }).MergeTable().ToList();
                foreach(var detail in stockinDetailList)
                {
                    statusInfoList.Add(new WarehousingStatusInfo()
                    {
                        IsNormalWarehousing = detail.Status == StockInStatus.task_finish.ToByte(),
                        WarehouseId = detail.WarehouseId.ToString(),
                        WarehousePosition = null,
                        WarehouseName = stockin.WarehouseName,
                        InventoryBoxNo = detail.InventoryBoxNo,
                        Position = "0",//TODO
                        StorageRackPosition = "",
                        SuppliesId = string.IsNullOrWhiteSpace(detail.MaterialOnlyId) ? detail.MaterialId.ToString() : detail.MaterialOnlyId.ToString(),
                        RefreshStock = 0, //TODO
                        WarehousingStep = ((StockInStatus)detail.Status).ToString(),
                        WarehousingFinishTime = detail.ModifiedDate?.ToString("yyyyMMddHHmmss")
                    });
                }
            }

            return new OutsideWarehousingStatusEnquiryResult()
            {
                Success = "true",
                ErrorId = null,
                ErrorInfo = null,
                WarehousingId = arg.WarehousingId,
                WarehousingType = arg.WarehousingType,
                WarehousingStatusInfoList = statusInfoList.ToArray(),
                IsNormalWarehousing = mesTask.WorkStatus == MESTaskWorkStatus.WorkComplated.ToByte(),
            };
             
        }

        /// <summary>
        /// 出库状态查询
        /// </summary>
        [HttpGet("WarehouseEntryStatusEnquiry")]
        public OutsideWarehouseEntryStatusEnquiryResult WarehouseEntryStatusEnquiry(OutsideWarehouseEntryStatusEnquiryArg arg)
        {
            return null;
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
        OutsideStockInResult Warehousing(OutsideStockInDto data);

        /// <summary>
        /// 出库
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [OperationContract]
        OutsideStockOutResult WarehouseEntry(OutsideStockOutDto data);

        /// <summary>
        /// 物料库存查询
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        [OperationContract]
        OutsideMaterialStockEnquiryResult MaterialStockEnquiry(OutsideMaterialStockEnquiryArg arg);

        /// <summary>
        /// 物流控制
        /// </summary>
        [OperationContract]
        OutsideLogisticsControlResult LogisticsControl(OutsideLogisticsControlArg arg);

        /// <summary>
        /// 物流（状态）查询
        /// </summary>
        [OperationContract]
        OutsideLogisticsEnquiryResult LogisticsEnquiry(OutsideLogisticsEnquiryArg arg);

        /// <summary>
        /// 入库状态查询
        /// </summary>
        [OperationContract]
        OutsideWarehousingStatusEnquiryResult WarehousingStatusEnquiry(OutsideWarehousingStatusEnquiryArg arg);

        /// <summary>
        /// 出库状态查询
        /// </summary>
        [OperationContract]
        OutsideWarehouseEntryStatusEnquiryResult WarehouseEntryStatusEnquiry(OutsideWarehouseEntryStatusEnquiryArg arg);
    }
}
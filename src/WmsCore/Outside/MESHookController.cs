using IServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
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

        public string Ping()
        {
            return "OK";
        }

        public OutSideStockInResult Warehousing(OutsideStockInDto data)
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
                    return new OutSideStockInResult()
                    {
                        ErrorId = routeData.CodeString,
                        ErrorInfo = routeData.Message,
                        Success = false,
                        WarehousingId = data.WarehousingId,
                        WarehousingTime = data.WarehousingTime
                    };
                }
                _sqlClient.CommitTran();
                return new OutSideStockInResult()
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
                return new OutSideStockInResult()
                {
                    Success = true,
                    ErrorId = "-1",
                    ErrorInfo = ex.ToString(),
                    WarehousingId = data.WarehousingId,
                    WarehousingTime = data.WarehousingTime
                };
            }
            
        }

        private RouteData CreateWMSStockin(Wms_mestask task, OutsideMaterialDto[] materialDtos)
        {
            Sys_dict stockinDict = _dictServices.QueryableToEntity(x => x.DictType == PubDictType.stockin.ToByte().ToString() && x.DictName == task.WarehousingType);
            if (stockinDict == null)
            {
                return RouteData<Wms_material>.From(PubMessages.E1004_WAREHOUSETYPE_NOTFOUND);
            }

            List<Wms_stockin> stockInList = new List<Wms_stockin>();
            List<Wms_stockindetail> stockinDetailList = new List<Wms_stockindetail>();
            foreach (OutsideMaterialDto materialDto in materialDtos)
            {
                RouteData<Wms_material> materialResult = GetMaterial(materialDto,true);
                if (!materialResult.IsSccuess)
                {
                    return materialResult;
                }
                Wms_material material = materialResult.Data;
                Wms_stockin stockin = stockInList.FirstOrDefault(x => x.WarehouseId == material.WarehouseId);
                if (stockin == null)
                {
                    stockin = new Wms_stockin()
                    {
                        MesTaskId = task.MesTaskId,
                        StockInId = PubId.SnowflakeId,
                        StockInNo = _serialnumServices.GetSerialnum(-1, "Wms_stockin"),
                        StockInType = stockinDict.DictId,
                        OrderNo = task.ProductionPlanId,
                        SupplierId = -1,
                        WarehouseId = material.WarehouseId,
                        StockInStatus = StockInStatus.task_confirm.ToByte(),
                        IsDel = DeleteFlag.Normal.ToByte(),
                        CreateBy = PubConst.InterfaceUserId,
                        CreateDate = DateTime.Now

                    };
                    stockInList.Add(stockin);
                }

                Wms_stockindetail detail = new Wms_stockindetail()
                {
                    StockInDetailId = PubId.SnowflakeId,
                    StockInId = stockin.StockInId,
                    WarehouseId = material.WarehouseId,
                    MaterialId = material.MaterialId,
                    PlanInQty = materialDto.SuppliesNumber,
                    ActInQty = 0,
                    Status = StockInStatus.task_confirm.ToByte(),
                    IsDel = DeleteFlag.Normal.ToByte(),
                    CreateBy = PubConst.InterfaceUserId,
                    CreateDate = DateTime.Now,
                    Remark = ""
                };
                stockinDetailList.Add(detail);
            }
            try
            {
                _stockinServices.Insert(stockInList);
                _stockinDetailServices.Insert(stockinDetailList);
            }
            catch(Exception e)
            {
                return YL.Core.Dto.RouteData.From(PubMessages.E1001_SUPPLIESTYPE_NOTFOUND ,e);
            }

            return new RouteData();
        }

        private RouteData<Wms_material> GetMaterial(OutsideMaterialDto materialDto , bool autoCreate)
        {
            Wms_material material = null;
            if(string.IsNullOrEmpty(materialDto.SuppliesOnlyId))
            {
                material = _materialServices.QueryableToEntity(x => x.MaterialNo == materialDto.SuppliesId);
            }
            else
            {
                material = _materialServices.QueryableToEntity(x => x.MaterialOnlyId == materialDto.SuppliesOnlyId);
            }
            
            if (material == null)
            {
                if (!autoCreate) return RouteData<Wms_material>.From(PubMessages.E1005_MATERIALNO_NOTFOUND);
                Sys_dict typeDict = _dictServices.QueryableToEntity(x => x.DictType == PubDictType.material.ToByte().ToString() && x.DictName == materialDto.SuppliesType);
                if (typeDict == null)
                {
                    return RouteData<Wms_material>.From(PubMessages.E1001_SUPPLIESTYPE_NOTFOUND);
                }
                else if (typeDict.WarehouseId == null)
                {
                    return RouteData<Wms_material>.From(PubMessages.E1002_SUPPLIESTYPE_WAREHOUSEID_NOTSET);
                }
                Sys_dict unitDict = _dictServices.QueryableToEntity(x => x.DictType == PubDictType.unit.ToByte().ToString() && x.DictName == materialDto.Unit);
                if (unitDict == null)
                {
                    return RouteData<Wms_material>.From(PubMessages.E1003_UNIT_NOTFOUND);
                }
                long warehouseId = typeDict.WarehouseId.Value;

                material = new Wms_material()
                {
                    MaterialId = PubId.SnowflakeId,
                    MaterialOnlyId = materialDto.SuppliesOnlyId ?? "",
                    MaterialNo = materialDto.SuppliesId ?? "",
                    MaterialName = materialDto.SuppliesName,
                    MaterialType = typeDict.DictId,
                    WarehouseId = warehouseId,
                    Unit = unitDict.DictId, 
                };
                _materialServices.Insert(material);
            }
            return RouteData<Wms_material>.From(material);
            
        }

        /// <summary>
        /// 出库处理
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public OutSideStockOutResult WarehouseEntry(OutsideStockOutDto data)
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
                    SuppliesKinds = data.SuppliesKinds, //物料种类
                    SuppliesInfoJson = jsonSuppliesInfo, //物料信息
                    WorkStatus = MESTaskWorkStatus.WaitPlan.ToByte(),      //等待计划
                    NotifyStatus = MESTaskNotifyStatus.Requested.ToByte(), //已接收
                    CreateDate = DateTime.Now
                };
                _mastaskServices.Insert(mesTask);

                RouteData routeData = CreateWMSStockout(mesTask, data.SuppliesInfoList);
                if (!routeData.IsSccuess)
                {
                    _sqlClient.RollbackTran();
                    return new OutSideStockOutResult()
                    {
                        ErrorId = routeData.CodeString,
                        ErrorInfo = routeData.Message,
                        Success = false,
                        WarehouseEntryId = data.WarehouseEntryId,
                        //WarehouseEntryTime = data.WarehouseEntryTime
                    };
                }
                _sqlClient.CommitTran();
                return new OutSideStockOutResult()
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
                return new OutSideStockOutResult()
                {
                    Success = true,
                    ErrorId = "-1",
                    ErrorInfo = ex.ToString(),
                    WarehouseEntryId = data.WarehouseEntryId,
                    //WarehouseEntryTime = data.WarehouseEntryTime
                };
            }
        }


        private RouteData CreateWMSStockout(Wms_mestask task, OutsideMaterialDto[] materialDtos)
        {
            Sys_dict stockoutDict = _dictServices.QueryableToEntity(x => x.DictType == PubDictType.stockout.ToByte().ToString() && x.DictName == task.WarehousingType);
            if (stockoutDict == null)
            {
                return RouteData<Wms_material>.From(PubMessages.E1004_WAREHOUSETYPE_NOTFOUND);
            }

            List<Wms_stockout> stockOutList = new List<Wms_stockout>();
            List<Wms_stockoutdetail> stockOutDetailList = new List<Wms_stockoutdetail>();
            foreach (OutsideMaterialDto materialDto in materialDtos)
            {
                RouteData<Wms_material> materialResult = GetMaterial(materialDto,false);
                if (!materialResult.IsSccuess)
                {
                    return materialResult;
                }
                Wms_material material = materialResult.Data;
                Wms_stockout stockout = stockOutList.FirstOrDefault(x => x.WarehouseId == material.WarehouseId);
                if (stockout == null)
                {
                    stockout = new Wms_stockout()
                    {
                        MesTaskId = task.MesTaskId,
                        StockOutId = PubId.SnowflakeId,
                        StockOutNo = _serialnumServices.GetSerialnum(-1, "Wms_stockout"),
                        StockOutType = stockoutDict.DictId,
                        OrderNo = task.WarehousingId,
                        WarehouseId = material.WarehouseId,
                        StockOutStatus = StockOutStatus.task_confirm.ToByte(),
                        IsDel = DeleteFlag.Normal.ToByte(),
                        CreateBy = PubConst.InterfaceUserId,
                        CreateDate = DateTime.Now 
                    };
                    stockOutList.Add(stockout);
                }

                Wms_stockoutdetail detail = new Wms_stockoutdetail()
                {
                    StockOutDetailId = PubId.SnowflakeId,
                    StockOutId = stockout.StockOutId,
                    WarehouseId = material.WarehouseId,
                    MaterialId = material.MaterialId,
                    PlanOutQty = materialDto.SuppliesNumber,
                    ActOutQty = 0,
                    Status = StockOutStatus.task_confirm.ToByte(),
                    IsDel = DeleteFlag.Normal.ToByte(),
                    CreateBy = PubConst.InterfaceUserId,
                    CreateDate = DateTime.Now,
                    Remark = ""
                };
                stockOutDetailList.Add(detail);
            }
            try
            {
                _stockoutServices.Insert(stockOutList);
                _stockoutDetailServices.Insert(stockOutDetailList);
            }
            catch (Exception e)
            {
                return YL.Core.Dto.RouteData.From(PubMessages.E1001_SUPPLIESTYPE_NOTFOUND, e);
            }

            return new RouteData();
        }

    }

    [ServiceContract]
    public interface IMESHookController
    {
        [OperationContract]
        string Ping();

        [OperationContract]
        OutSideStockInResult Warehousing(OutsideStockInDto data);

        [OperationContract]
        OutSideStockOutResult WarehouseEntry(OutsideStockOutDto data);
    }
}
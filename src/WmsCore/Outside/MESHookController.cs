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
                return RouteData<Wms_material>.From(PubMessages.E1004_WarehouseType_NotFound);
            }

            List<Wms_stockin> stockInList = new List<Wms_stockin>();
            List<Wms_stockindetail> stockinDetailList = new List<Wms_stockindetail>();
            foreach (OutsideMaterialDto materialDto in materialDtos)
            {
                RouteData<Wms_material> materialResult = GetOrCreateMaterial(materialDto);
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
                        OrderNo = task.WarehousingId,
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
                return YL.Core.Dto.RouteData.From(PubMessages.E1001_SuppliesType_NotFound ,e);
            }

            return new RouteData();
        }

        private RouteData<Wms_material> GetOrCreateMaterial(OutsideMaterialDto materialDto)
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
                Sys_dict typeDict = _dictServices.QueryableToEntity(x => x.DictType == PubDictType.material.ToByte().ToString() && x.DictName == materialDto.SuppliesType);
                if (typeDict == null)
                {
                    return RouteData<Wms_material>.From(PubMessages.E1001_SuppliesType_NotFound);
                }
                else if (typeDict.WarehouseId == null)
                {
                    return RouteData<Wms_material>.From(PubMessages.E1002_SuppliesType_WarehouseId_NotSet);
                }
                Sys_dict unitDict = _dictServices.QueryableToEntity(x => x.DictType == PubDictType.unit.ToByte().ToString() && x.DictName == materialDto.Unit);
                if (unitDict == null)
                {
                    return RouteData<Wms_material>.From(PubMessages.E1003_Unit_NotFound);
                }
                long warehouseId = typeDict.WarehouseId.Value;

                material = new Wms_material()
                {
                    MaterialId = PubId.SnowflakeId,
                    MaterialOnlyId = materialDto.SuppliesOnlyId,
                    MaterialNo = materialDto.SuppliesId,
                    MaterialName = materialDto.SuppliesName,
                    MaterialType = typeDict.DictId,
                    WarehouseId = warehouseId,
                    Unit = unitDict.DictId,
                    Qty = materialDto.SuppliesNumber,
                };
                _materialServices.Insert(material);
            }
            return RouteData<Wms_material>.From(material);
            
        }

        public OutSideStockOutResult WarehouseEntry(OutsideStockInDto data)
        {
            return new OutSideStockOutResult()
            {

            };
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
        OutSideStockOutResult WarehouseEntry(OutsideStockInDto data);
    }
}
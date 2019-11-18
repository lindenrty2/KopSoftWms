﻿using IServices.Outside;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YL.Core.Dto;
using YL.Core.Entity;
using YL.Utils.Extensions;
using YL.Utils.Pub;

namespace Services.Outside
{
    public class SelfWMSApiAccessor : IWMSApiProxy
    {
        private ISqlSugarClient _sqlClient;
        private Wms_warehouse _warehouse;

        public Wms_warehouse Warehouse { get { return _warehouse; } }

        public SelfWMSApiAccessor(Wms_warehouse warehouse, SqlSugar.ISqlSugarClient sqlClient)
        {
            _warehouse = warehouse;
            _sqlClient = sqlClient;
        }

        public async Task<RouteData<Wms_inventorybox>> GetInventoryBox(long inventoryBoxId)
        {
            Wms_inventorybox box = await _sqlClient.Queryable<Wms_inventorybox>().FirstAsync(x => x.InventoryBoxId == inventoryBoxId);
            return RouteData<Wms_inventorybox>.From(box);
        }

        public Task<RouteData<Wms_inventory[]>> GetInventoryBoxDetail(long inventoryBoxId)
        {
            Wms_inventory[] box = _sqlClient.Queryable<Wms_inventory>().Where(x => x.InventoryBoxId == inventoryBoxId).ToArray();
            return Task.FromResult(RouteData<Wms_inventory[]>.From(box));
        }

        public async Task<RouteData<Wms_inventorybox[]>> GetInventoryBoxList(long? reservoirAreaId, long? storageRackId, int pageIndex,int pageSize, string search, string[] order, string datemin, string datemax)
        {
            ISugarQueryable<Wms_inventorybox> query = _sqlClient.Queryable<Wms_inventorybox>();
            if(reservoirAreaId != null)
            {
                query = query.Where(x => x.ReservoirAreaId == reservoirAreaId);
            }
            if (storageRackId != null)
            {
                query = query.Where(x => x.StorageRackId == storageRackId);
            }
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x => x.InventoryBoxNo.ToString().Contains(search) || x.InventoryBoxName.Contains(search) );
            }
            DateTime minDate;
            if (!string.IsNullOrWhiteSpace(datemin) && DateTime.TryParse(datemin, out minDate))
            {
                query = query.Where(x => x.ModifiedDate >= minDate || x.CreateDate >= minDate);
            }
            DateTime maxDate;
            if (!string.IsNullOrWhiteSpace(datemax) && DateTime.TryParse(datemax, out maxDate))
            {
                query = query.Where(x => x.ModifiedDate <= maxDate || x.CreateDate <= maxDate);
            }
            //Order
            RefAsync<int> totalCount = new RefAsync<int>();
            List<Wms_inventorybox> result = await query.ToPageListAsync(pageIndex,pageSize, totalCount); 
            return RouteData<Wms_inventorybox[]>.From(result.ToArray(), totalCount.Value);
        }
         
        public async Task<RouteData<Wms_MaterialDto>> GetMateral(long materialId)
        {
            Wms_MaterialDto box = await _sqlClient.Queryable<Wms_material>()
                .Where(x => x.MaterialId == materialId)
                .Select(
                (x) => new Wms_MaterialDto
                {
                    MaterialId = x.MaterialId,
                    MaterialOnlyId = x.MaterialOnlyId,
                    MaterialNo = x.MaterialNo,
                    MaterialName = x.MaterialName,
                    MaterialType = x.MaterialTypeName,
                    Unit = x.UnitName
                }).FirstAsync();
            return RouteData<Wms_MaterialDto>.From(box); 
        }

        public async Task<RouteData<Wms_MaterialDto[]>> GetMateralList(int pageIndex, int pageSize, string search, string[] order, string datemin, string datemax)
        {
            ISugarQueryable<Wms_material> query = _sqlClient.Queryable<Wms_material>(); 
            DateTime minDate;
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x => x.MaterialNo.ToString().Contains(search) || x.MaterialOnlyId.ToString().Contains(search) || x.MaterialName.Contains(search));
            }
            if (!string.IsNullOrWhiteSpace(datemin) && DateTime.TryParse(datemin, out minDate))
            {
                query = query.Where(x => x.ModifiedDate >= minDate);
            }
            DateTime maxDate;
            if (!string.IsNullOrWhiteSpace(datemax) && DateTime.TryParse(datemax, out maxDate))
            {
                query = query.Where(x => x.ModifiedDate >= maxDate);
            }
            List<Wms_MaterialDto> result = await query.Select(
              (x) => new Wms_MaterialDto
              {
                  MaterialId = x.MaterialId,
                  MaterialOnlyId = x.MaterialOnlyId,
                  MaterialNo = x.MaterialNo,
                  MaterialName = x.MaterialName,
                  MaterialType = x.MaterialTypeName,
                  Unit = x.UnitName
              }).ToPageListAsync(pageIndex,pageSize);
                                 
            return RouteData<Wms_MaterialDto[]>.From(result.ToArray());
        }

        public async Task<RouteData<Wms_reservoirarea>> GetReservoirArea(long reservoirAreaId)
        {
            Wms_reservoirarea result = await _sqlClient.Queryable<Wms_reservoirarea>()
                  .Where(x => x.ReservoirAreaId == reservoirAreaId)
                  .FirstAsync();
            return RouteData<Wms_reservoirarea>.From(result);
        }

        public async Task<RouteData<Wms_reservoirarea[]>> GetReservoirAreaList(int pageIndex, int pageSize, string search, string[] order, string datemin, string datemax)
        {
            ISugarQueryable<Wms_reservoirarea> query = _sqlClient.Queryable<Wms_reservoirarea>();
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x => x.ReservoirAreaNo.ToString().Contains(search) || x.ReservoirAreaName.Contains(search));
            }
            DateTime minDate;
            if (!string.IsNullOrWhiteSpace(datemin) && DateTime.TryParse(datemin, out minDate))
            {
                query = query.Where(x => x.ModifiedDate >= minDate);
            }
            DateTime maxDate;
            if (!string.IsNullOrWhiteSpace(datemax) && DateTime.TryParse(datemax, out maxDate))
            {
                query = query.Where(x => x.ModifiedDate <= maxDate);
            }
            List<Wms_reservoirarea> result = await query.ToPageListAsync(pageIndex, pageSize);
            return RouteData<Wms_reservoirarea[]>.From(result.ToArray());
        }

        public async Task<RouteData<Wms_storagerack>> GetStorageRack(long storageRackId)
        {
            Wms_storagerack result = await _sqlClient.Queryable<Wms_storagerack>()
                .Where(x => x.StorageRackId == storageRackId)
                .FirstAsync();
            return RouteData<Wms_storagerack>.From(result);
        }

        public async Task<RouteData<Wms_storagerack[]>> GetStorageRackList(long? reservoirAreaId, int pageIndex, int pageSize, string search, string[] order, string datemin, string datemax)
        {
            ISugarQueryable<Wms_storagerack> query = _sqlClient.Queryable<Wms_storagerack>();
         
            if (reservoirAreaId != null)
            {
                query = query.Where(x => x.ReservoirAreaId == reservoirAreaId);
            }
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x => x.StorageRackNo.ToString().Contains(search) || x.StorageRackName.Contains(search));
            }
            DateTime minDate;
            if (!string.IsNullOrWhiteSpace(datemin) && DateTime.TryParse(datemin, out minDate))
            {
                query = query.Where(x => x.ModifiedDate >= minDate);
            }
            DateTime maxDate;
            if (!string.IsNullOrWhiteSpace(datemax) && DateTime.TryParse(datemax, out maxDate))
            {
                query = query.Where(x => x.ModifiedDate <= maxDate);
            }
            List<Wms_storagerack> result = await query.ToPageListAsync(pageIndex, pageSize);
            return RouteData<Wms_storagerack[]>.From(result.ToArray());
        }

        public async Task<RouteData<OutsideInventoryDto[]>> QueryInventory(long? reservoirAreaId, long? storageRackId,long? inventoryBoxId, long? materialId, int pageIndex, int pageSize, string search, string[] order, string datemin, string datemax)
        {
            ISugarQueryable<Wms_inventory, Wms_inventorybox, Wms_storagerack> query = _sqlClient.Queryable<Wms_inventory, Wms_inventorybox, Wms_storagerack>((i, ib, sr) => new object[] {
                   JoinType.Left,i.InventoryBoxId==ib.InventoryBoxId,
                   JoinType.Left,ib.ReservoirAreaId==sr.ReservoirAreaId,
                 })
                 .Where((i, ib, sr) => i.IsDel == DeleteFlag.Normal);
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where((i,ib,sr) => i.MaterialNo.ToString().Contains(search) || i.MaterialOnlyId.ToString().Contains(search) || i.MaterialName.Contains(search));
            }
            DateTime minDate;
            if (!string.IsNullOrWhiteSpace(datemin) && DateTime.TryParse(datemin, out minDate))
            {
                query = query.Where((i, ib, sr) => i.ModifiedDate >= minDate);
            }
            DateTime maxDate;
            if (!string.IsNullOrWhiteSpace(datemax) && DateTime.TryParse(datemax, out maxDate))
            {
                query = query.Where((i, ib, sr) => i.ModifiedDate <= maxDate);
            }

            List<OutsideInventoryDto> result = await query.Select(
              (i,ib,sr) => new OutsideInventoryDto
              { 
                    MaterialId = i.MaterialId ?? -1,
                    MaterialNo = i.MaterialNo,
                    MaterialOnlyId = i.MaterialOnlyId,
                    MaterialName = i.MaterialName,
                    Qty = i.Qty,
                    IsLocked = i.IsLocked,
                    StorageRackId = sr.StorageRackId,
                    StorageRackNo = sr.StorageRackNo,
                    StorageRackName = sr.StorageRackName,
                    InventoryBoxId = ib.InventoryBoxId,
                    InventoryBoxNo = ib.InventoryBoxName,
                    Position = i.Position,
                    CreateBy = i.CreateUser,
                    CreateDate = i.CreateDate.Value.ToString(PubConst.Format_DateTime),
                    ModifiedBy = i.ModifiedUser,
                    ModifiedDate = i.ModifiedDate.Value.ToString(PubConst.Format_DateTime),
              }).ToPageListAsync(pageIndex, pageSize);
            return RouteData<OutsideInventoryDto[]>.From(result.ToArray());
        }

        public async Task<RouteData<OutsideInventoryRecordDto[]>> QueryInventoryRecord(long? reservoirAreaId, long? storageRackId, long? inventoryBoxId, long? materialId, int pageIndex, int pageSize, string search, string[] order, string datemin, string datemax)
        {
            ISugarQueryable<Wms_inventoryrecord, Wms_inventorybox, Wms_storagerack> query = _sqlClient.Queryable<Wms_inventoryrecord, Wms_inventorybox, Wms_storagerack>((ir, ib, sr) => new object[] {
                   JoinType.Left,ir.InventoryBoxId==ib.InventoryBoxId,
                   JoinType.Left,ib.ReservoirAreaId==sr.ReservoirAreaId,
                 })
                            .Where((ir, ib, sr) => ir.IsDel == DeleteFlag.Normal);
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where((ir, ib, sr) => ir.MaterialNo.ToString().Contains(search) || ir.MaterialOnlyId.ToString().Contains(search) || ir.MaterialName.Contains(search));
            }
            DateTime minDate;
            if (!string.IsNullOrWhiteSpace(datemin) && DateTime.TryParse(datemin, out minDate))
            {
                query = query.Where((ir, ib, sr) => ir.CreateDate >= minDate);
            }
            DateTime maxDate;
            if (!string.IsNullOrWhiteSpace(datemax) && DateTime.TryParse(datemax, out maxDate))
            {
                query = query.Where((ir, ib, sr) => ir.CreateDate <= maxDate);
            }

            List<OutsideInventoryRecordDto> result = await query.Select(
              (ir, ib, sr) => new OutsideInventoryRecordDto
              {
                  MaterialId = ir.MaterialId,
                  MaterialNo = ir.MaterialNo,
                  MaterialOnlyId = ir.MaterialOnlyId,
                  MaterialName = ir.MaterialName,
                  Qty = ir.Qty,
                  AfterQty = ir.AfterQty,
                  StorageRackId = sr.StorageRackId,
                  StorageRackNo = sr.StorageRackNo,
                  StorageRackName = sr.StorageRackName,
                  InventoryBoxId = ib.InventoryBoxId,
                  InventoryBoxNo = ib.InventoryBoxName,
                  Position = ir.InventoryPosition,
                  CreateBy = ir.CreateUser,
                  CreateDate = ir.CreateDate.Value.ToString(PubConst.Format_DateTime),
                  ModifiedBy = ir.ModifiedUser,
                  ModifiedDate = ir.ModifiedDate.Value.ToString(PubConst.Format_DateTime),
              }).ToPageListAsync(pageIndex, pageSize);

            return RouteData<OutsideInventoryRecordDto[]>.From(result.ToArray());
        }

        public async Task<RouteData<OutsideStockInQueryResult>> QueryStockIn(long stockInId)
        {
            Wms_stockin stockIn = await _sqlClient.Queryable<Wms_stockin>().FirstAsync();
            List<Wms_stockindetail> stockInDetails = await _sqlClient.Queryable<Wms_stockindetail>()
                .Where(x => x.StockInId == stockInId)
                .ToListAsync();

            List<OutsideStockInQueryResultDetail> details = new List<OutsideStockInQueryResultDetail>();
            foreach (Wms_stockindetail stockInDetail in stockInDetails)
            {
                OutsideStockInQueryResultDetail detail = new OutsideStockInQueryResultDetail()
                {
                    MaterialId = stockInDetail.MaterialId.Value,
                    MaterialNo = stockInDetail.MaterialNo,
                    MaterialName = stockInDetail.MaterialName,
                    MaterialOnlyId = stockInDetail.MaterialOnlyId,
                    PlanInQty = stockInDetail.PlanInQty,
                    ActInQty = stockInDetail.ActInQty,
                    Remark = stockInDetail.Remark,
                    ModifiedBy = stockInDetail.ModifiedUser,
                    ModifiedDate = stockInDetail.ModifiedDate.Value.ToString(PubConst.Format_DateTime),
                    
                    Status = (StockInStatus)stockInDetail.Status
                };
                details.Add(detail);
            }

            OutsideStockInQueryResult result = new OutsideStockInQueryResult()
            {
                StockInId = stockIn.StockInId,
                StockInNo = stockIn.StockInNo,
                MesTaskId = stockIn.MesTaskId.Value,
                StockInTypeName = stockIn.
                OrderNo = stockIn.OrderNo,
                StockInStatus = (StockInStatus)stockIn.StockInStatus,
                Remark = stockIn.Remark,
                Details = details.ToArray(),
                ModifiedBy = stockIn.ModifiedUser,
                ModifiedDate = stockIn.ModifiedDate.Value.ToString(PubConst.Format_DateTime)
            };
            return RouteData<OutsideStockInQueryResult>.From(result);
        }

        public async Task<RouteData<OutsideStockOutQueryResult>> QueryStockOut(long stockOutId)
        {
            Wms_stockout stockOut = await _sqlClient.Queryable<Wms_stockout>().FirstAsync();
            List<Wms_stockoutdetail> stockOutDetails = await _sqlClient.Queryable<Wms_stockoutdetail>()
                .Where(x => x.StockOutId == stockOutId)
                .ToListAsync();

            List<OutsideStockOutQueryResultDetail> details = new List<OutsideStockOutQueryResultDetail>();
            foreach (Wms_stockoutdetail stockOutDetail in stockOutDetails)
            {
                OutsideStockOutQueryResultDetail detail = new OutsideStockOutQueryResultDetail()
                {
                    MaterialId = stockOutDetail.MaterialId.Value,
                    MaterialNo = stockOutDetail.MaterialNo,
                    MaterialName = stockOutDetail.MaterialName,
                    MaterialOnlyId = stockOutDetail.MaterialOnlyId,
                    PlanOutQty = stockOutDetail.PlanOutQty,
                    ActOutQty = stockOutDetail.ActOutQty,
                    Remark = stockOutDetail.Remark,
                    ModifiedBy = stockOutDetail.ModifiedUser,
                    ModifiedDate = stockOutDetail.ModifiedDate.Value.ToString(PubConst.Format_DateTime),

                    Status = (StockOutStatus)stockOutDetail.Status
                };
                details.Add(detail);
            }

            OutsideStockOutQueryResult result = new OutsideStockOutQueryResult()
            {
                StockOutId = stockOut.StockOutId,
                StockOutNo = stockOut.StockOutNo,
                MesTaskId = stockOut.MesTaskId.Value,
                StockOutTypeName = stockOut.
                OrderNo = stockOut.OrderNo,
                StockOutStatus = (StockOutStatus)stockOut.StockOutStatus,
                Remark = stockOut.Remark,
                Details = details.ToArray(),
                ModifiedBy = stockOut.ModifiedUser,
                ModifiedDate = stockOut.ModifiedDate.Value.ToString(PubConst.Format_DateTime)
            };
            return RouteData<OutsideStockOutQueryResult>.From(result);
        }

        public async Task<RouteData<OutsideStockInRequestResult[]>> StockIn(OutsideStockInRequestDto request)
        {
            RouteData<Wms_stockin[]> result = await CreateWMSStockin(request);
            if (!result.IsSccuess)
            {
                return RouteData<OutsideStockInRequestResult[]>.From(result);
            }
            return RouteData<OutsideStockInRequestResult[]>.From(
                result.Data.Select(
                    x => new OutsideStockInRequestResult { StockInId = x.StockInId, StockInNo = x.StockInNo }
                    ).ToArray()
                );
        }
        private async Task<RouteData<Wms_stockin[]>> CreateWMSStockin(OutsideStockInRequestDto request)
        {
            string stockinType = PubDictType.stockin.ToByte().ToString();
            Sys_dict stockinDict = await _sqlClient.Queryable<Sys_dict>()
                .FirstAsync(x => x.DictType == stockinType && x.DictName == request.WarehousingType);
            if (stockinDict == null)
            {
                return RouteData<Wms_stockin[]>.From(PubMessages.E1004_WAREHOUSETYPE_NOTFOUND);
            }

            List<Wms_stockin> stockInList = new List<Wms_stockin>();
            List<Wms_stockindetail> stockinDetailList = new List<Wms_stockindetail>();
            foreach (Wms_MaterialInventoryDto materialDto in request.MaterialList)
            {
                RouteData<Wms_material> materialResult = await GetMaterial(materialDto, true);
                if (!materialResult.IsSccuess)
                {
                    return RouteData<Wms_stockin[]>.From(materialResult);
                }
                Wms_material material = materialResult.Data;
                Wms_stockin stockin = stockInList.FirstOrDefault(x => x.WarehouseId == _warehouse.WarehouseId);
                if (stockin == null)
                {
                    stockin = new Wms_stockin()
                    {
                        MesTaskId = request.MesTaskId,
                        StockInId = PubId.SnowflakeId,
                        StockInNo = request.WarehousingId,
                        StockInType = stockinDict.DictId,
                        StockInTypeName = stockinDict.DictName,
                        OrderNo = request.OrderNo,
                        SupplierId = -1,
                        WarehouseId = _warehouse.WarehouseId,
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
                    WarehouseId = _warehouse.WarehouseId,
                    MaterialId = material.MaterialId,
                    PlanInQty = materialDto.Qty,
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
                if(_sqlClient.Insertable(stockInList).ExecuteCommand() == 0)
                {
                    throw new Exception("stockInList更新失败"); 
                }
                if(_sqlClient.Insertable(stockinDetailList).ExecuteCommand() == 0)
                {
                    throw new Exception("stockinDetailList更新失败"); 
                }
                return RouteData<Wms_stockin[]>.From(stockInList.ToArray());
            }
            catch (Exception e)
            {
                return YL.Core.Dto.RouteData<Wms_stockin[]>.From(PubMessages.E1001_SUPPLIESTYPE_NOTFOUND, e);
            }
        }

        public async Task<RouteData<OutsideStockOutRequestResult[]>> StockOut(OutsideStockOutRequestDto request)
        {
            RouteData<Wms_stockout[]> result = await CreateWMSStockout(request);
            if (!result.IsSccuess)
            {
                return RouteData<OutsideStockOutRequestResult[]>.From(result);
            }
            return RouteData<OutsideStockOutRequestResult[]>.From(
                result.Data.Select(
                    x => new OutsideStockOutRequestResult { StockOutId = x.StockOutId, StockOutNo = x.StockOutNo }
                    ).ToArray()
                );
        } 

        private async Task<RouteData<Wms_stockout[]>> CreateWMSStockout(OutsideStockOutRequestDto request)
        {
            Sys_dict stockoutDict = await _sqlClient.Queryable<Sys_dict>()
                .FirstAsync(x => x.DictType == PubDictType.stockout.ToByte().ToString() && x.DictName == request.WarehousingType);

            if (stockoutDict == null)
            {
                return RouteData<Wms_stockout[]>.From(PubMessages.E1004_WAREHOUSETYPE_NOTFOUND);
            }

            List<Wms_stockout> stockOutList = new List<Wms_stockout>();
            List<Wms_stockoutdetail> stockOutDetailList = new List<Wms_stockoutdetail>();
            foreach (Wms_MaterialInventoryDto materialDto in request.MaterialList)
            {
                RouteData<Wms_material> materialResult = await GetMaterial(materialDto, false);
                if (!materialResult.IsSccuess)
                {
                    return RouteData<Wms_stockout[]>.From(materialResult);
                }
                Wms_material material = materialResult.Data;
                Wms_stockout stockout = stockOutList.FirstOrDefault(x => x.WarehouseId == _warehouse.WarehouseId);
                if (stockout == null)
                {
                    stockout = new Wms_stockout()
                    {
                        MesTaskId = request.MesTaskId,
                        StockOutId = PubId.SnowflakeId,
                        StockOutNo = request.WarehousingId,
                        StockOutType = stockoutDict.DictId,
                        StockOutTypeName = stockoutDict.DictName,
                        OrderNo = request.OrderNo,
                        WarehouseId = _warehouse.WarehouseId,
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
                    WarehouseId = _warehouse.WarehouseId,
                    MaterialId = material.MaterialId,
                    PlanOutQty = materialDto.Qty,
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
                if(_sqlClient.Insertable(stockOutList).ExecuteCommand() == 0)
                {
                    throw new Exception("stockOutList更新失败");
                }
                if(_sqlClient.Insertable(stockOutDetailList).ExecuteCommand() == 0)
                {
                    throw new Exception("stockOutDetailList更新失败");
                }
            }
            catch (Exception e)
            {
                return RouteData<Wms_stockout[]>.From(PubMessages.E0004_DATABASE_UPDATE_FAIL, e);
            }

            return new RouteData<Wms_stockout[]>();
        }



        private async Task<RouteData<Wms_material>> GetMaterial(Wms_MaterialInventoryDto materialDto, bool autoCreate)
        {
            Wms_material material = null;
            if (string.IsNullOrEmpty(materialDto.MaterialOnlyId))
            {
                material = await _sqlClient.Queryable<Wms_material>().FirstAsync(x => x.MaterialNo == materialDto.MaterialNo);
            }
            else
            {
                material = await _sqlClient.Queryable<Wms_material>().FirstAsync(x => x.MaterialOnlyId == materialDto.MaterialOnlyId);
            }

            if (material == null)
            {
                if (!autoCreate) return RouteData<Wms_material>.From(PubMessages.E1005_MATERIALNO_NOTFOUND);
                Sys_dict typeDict = await _sqlClient.Queryable<Sys_dict>()
                    .FirstAsync(x => x.DictType == PubDictType.material.ToByte().ToString() && x.DictName == materialDto.MaterialType);
                if (typeDict == null)
                {
                    return RouteData<Wms_material>.From(PubMessages.E1001_SUPPLIESTYPE_NOTFOUND);
                }
                else if (typeDict.WarehouseId == null)
                {
                    return RouteData<Wms_material>.From(PubMessages.E1002_SUPPLIESTYPE_WAREHOUSEID_NOTSET);
                }
                Sys_dict unitDict = await _sqlClient.Queryable<Sys_dict>().FirstAsync(x => x.DictType == PubDictType.unit.ToByte().ToString() && x.DictName == materialDto.Unit);
                if (unitDict == null)
                {
                    return RouteData<Wms_material>.From(PubMessages.E1003_UNIT_NOTFOUND);
                } 

                material = new Wms_material()
                {
                    MaterialId = PubId.SnowflakeId,
                    MaterialOnlyId = materialDto.MaterialOnlyId ?? "",
                    MaterialNo = materialDto.MaterialNo ?? "",
                    MaterialName = materialDto.MaterialName,
                    MaterialType = typeDict.DictId,
                    MaterialTypeName = typeDict.DictName,
                    WarehouseId = _warehouse.WarehouseId,
                    Unit = unitDict.DictId,
                };
                if (_sqlClient.Insertable(material).ExecuteCommand() == 0)
                {
                    return RouteData<Wms_material>.From(PubMessages.E0004_DATABASE_UPDATE_FAIL);
                }
            }
            return RouteData<Wms_material>.From(material);

        }

        public void Dispose()
        {
            _sqlClient = null;
        }

    }
}

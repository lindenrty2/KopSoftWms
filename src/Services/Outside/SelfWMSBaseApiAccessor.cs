using IServices.Outside;
using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WMSCore.Outside;
using YL.Core.Dto;
using YL.Core.Entity;
using YL.Utils.Extensions;
using YL.Utils.Json;
using YL.Utils.Pub;

namespace Services.Outside
{
    public class SelfWMSBaseApiAccessor : IWMSBaseApiAccessor
    {

        public bool IsOutside => false;
        private ISqlSugarClient _sqlClient;
        private Wms_warehouse _warehouse;
        public Wms_warehouse Warehouse { get { return _warehouse; } }

        private SysUserDto _userDto;
        private SysUserDto UserDto { get { return _userDto; } }

        public SelfWMSBaseApiAccessor(Wms_warehouse warehouse, SqlSugar.ISqlSugarClient sqlClient, SysUserDto userDto)
        {
            _warehouse = warehouse;
            _sqlClient = sqlClient;
            _userDto = userDto;
        }


        //-------------------------------料箱----------------------------------
        public async Task<RouteData<Wms_inventorybox>> GetInventoryBox(long inventoryBoxId)
        {
            Wms_inventorybox box = await _sqlClient.Queryable<Wms_inventorybox>().FirstAsync(x => x.InventoryBoxId == inventoryBoxId);
            return RouteData<Wms_inventorybox>.From(box);
        }

        public async Task<RouteData<Wms_inventory[]>> GetInventoryBoxDetail(long inventoryBoxId)
        {
            List<Wms_inventory> boxDetails = await _sqlClient.Queryable<Wms_inventory>()
                .Where(x => x.InventoryBoxId == inventoryBoxId)
                .OrderBy(x => x.Position)
                .ToListAsync();
            return RouteData<Wms_inventory[]>.From(boxDetails.ToArray());
        }

        public async Task<RouteData<Wms_inventorybox[]>> GetInventoryBoxList(
            long? reservoirAreaId, long? storageRackId, InventoryBoxStatus? status, int pageIndex, int pageSize, string search, string[] order, string datemin, string datemax)
        {
            ISugarQueryable<Wms_inventorybox> query = _sqlClient.Queryable<Wms_inventorybox>();
            if (reservoirAreaId != null)
            {
                query = query.Where(x => x.ReservoirAreaId == reservoirAreaId);
            }
            if (storageRackId != null)
            {
                query = query.Where(x => x.StorageRackId == storageRackId);
            }
            if (status != null)
            {
                query = query.Where(x => x.Status == (int)status);
            }
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x => x.InventoryBoxNo.ToString().Contains(search) || x.InventoryBoxName.Contains(search));
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
            query = query.Sort(order);
            //Order
            RefAsync<int> totalCount = new RefAsync<int>();
            List<Wms_inventorybox> result = await query.ToPageListAsync(pageIndex, pageSize, totalCount);
            return RouteData<Wms_inventorybox[]>.From(result.ToArray(), totalCount.Value);
        }

        //-------------------------------物料----------------------------------

        public async Task<RouteData<Wms_MaterialDto>> GetMateral(long materialId)
        {
            Wms_MaterialDto box = await _sqlClient.Queryable<Wms_material>()
                .Where(x => x.MaterialId == materialId)
                .Select(
                (x) => new Wms_MaterialDto
                {
                    MaterialId = x.MaterialId.ToString(),
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
            ISugarQueryable<Wms_material> query = _sqlClient.Queryable<Wms_material>().Where(x => x.IsDel == DeleteFlag.Normal);
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
                query = query.Where(x => x.ModifiedDate <= maxDate);
            }
            query = query.Sort(order);

            RefAsync<int> totalCount = new RefAsync<int>();
            List<Wms_MaterialDto> result = await query.Select(
              (x) => new Wms_MaterialDto
              {
                  MaterialId = x.MaterialId.ToString(),
                  MaterialOnlyId = x.MaterialOnlyId,
                  MaterialNo = x.MaterialNo,
                  MaterialName = x.MaterialName,
                  MaterialType = x.MaterialTypeName,
                  Unit = x.UnitName
              }).ToPageListAsync(pageIndex, pageSize, totalCount);

            return RouteData<Wms_MaterialDto[]>.From(result.ToArray(), totalCount);
        }

        //-------------------------------库区----------------------------------

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
            query = query.Sort(order);
            RefAsync<int> totalCount = new RefAsync<int>();
            List<Wms_reservoirarea> result = await query.ToPageListAsync(pageIndex, pageSize, totalCount);
            return RouteData<Wms_reservoirarea[]>.From(result.ToArray(), totalCount.Value);
        }

        //-------------------------------货架----------------------------------

        public async Task<RouteData<Wms_storagerack>> GetStorageRack(long storageRackId)
        {
            Wms_storagerack result = await _sqlClient.Queryable<Wms_storagerack>()
                .Where(x => x.StorageRackId == storageRackId)
                .FirstAsync();
            return RouteData<Wms_storagerack>.From(result);
        }


        public async Task<RouteData<Wms_storagerack[]>> GetStorageRackList(long? reservoirAreaId, StorageRackStatus? status, int pageIndex, int pageSize, string search, string[] order, string datemin, string datemax)
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
            if (status != null)
            {
                query = query.Where(x => x.Status == status);
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
            query = query.Sort(order);
            RefAsync<int> totalCount = new RefAsync<int>();
            List<Wms_storagerack> result = await query.ToPageListAsync(pageIndex, pageSize, totalCount);
            return RouteData<Wms_storagerack[]>.From(result.ToArray(), totalCount.Value);
        }


        //-------------------------------库存----------------------------------

        public async Task<RouteData<OutsideInventoryDto[]>> QueryInventory(long? reservoirAreaId, long? storageRackId, long? inventoryBoxId, long? materialId, int pageIndex, int pageSize, string search, string[] order, string datemin, string datemax)
        {
            ISugarQueryable<Wms_inventory, Wms_inventorybox> query = _sqlClient.Queryable<Wms_inventory, Wms_inventorybox>((i, ib) => new object[] {
                   JoinType.Left,i.InventoryBoxId==ib.InventoryBoxId
                 })
                 .Where((i, ib) => i.MaterialId != null && i.Qty > 0 && i.IsDel == DeleteFlag.Normal);
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where((i, ib) => i.MaterialNo.ToString().Contains(search) || i.MaterialOnlyId.ToString().Contains(search) || i.MaterialName.Contains(search) || ib.InventoryBoxNo.Contains(search));
            }
            if (reservoirAreaId != null)
            {
                query = query.Where((i, ib) => ib.ReservoirAreaId == reservoirAreaId);
            }
            if (storageRackId != null)
            {
                query = query.Where((i, ib) => ib.StorageRackId == storageRackId);
            }
            if (inventoryBoxId != null)
            {
                query = query.Where((i, ib) => ib.InventoryBoxId == inventoryBoxId);
            }
            if (materialId != null)
            {
                query = query.Where((i, ib) => i.MaterialId == materialId);
            }
            DateTime minDate;
            if (!string.IsNullOrWhiteSpace(datemin) && DateTime.TryParse(datemin, out minDate))
            {
                query = query.Where((i, ib) => i.ModifiedDate >= minDate);
            }
            DateTime maxDate;
            if (!string.IsNullOrWhiteSpace(datemax) && DateTime.TryParse(datemax, out maxDate))
            {
                query = query.Where((i, ib) => i.ModifiedDate <= maxDate);
            }
            query = query.Sort(order, new string[,] {
                {"CREATEDATE", "i.CREATEDATE" },
                {"CREATEBY", "i.CREATEUSER" },
                {"MODIFIEDDATE", "i.MODIFIEDDATE" },
                {"MODIFIEDBY", "i.MODIFIEDUSER" }}
            );

            RefAsync<int> totalCount = new RefAsync<int>();
            IEnumerable<OutsideInventoryDto> result = await query.Select(
              (i, ib) => new OutsideInventoryDto
              {
                  MaterialId = i.MaterialId ?? -1,
                  MaterialNo = i.MaterialNo,
                  MaterialOnlyId = i.MaterialOnlyId,
                  MaterialName = i.MaterialName,
                  Qty = i.Qty,
                  IsLocked = i.IsLocked,
                  StorageRackId = ib.StorageRackId ?? 0,
                  //StorageRackNo = ib.StorageRackNo,
                  StorageRackName = ib.StorageRackName,
                  InventoryBoxId = ib.InventoryBoxId,
                  InventoryBoxNo = ib.InventoryBoxNo,
                  Floor = ib.Floor ?? 0,
                  Row = ib.Row ?? 0,
                  Column = ib.Column ?? 0,
                  Position = i.Position,
                  CreateBy = i.CreateUser,
                  CreateDate = i.CreateDate.Value.ToString(PubConst.Format_DateTime),
                  ModifiedBy = i.ModifiedUser,
                  ModifiedDate = i.ModifiedDate.Value.ToString(PubConst.Format_DateTime),
              })
              .ToPageListAsync(pageIndex, pageSize, totalCount);

            return RouteData<OutsideInventoryDto[]>.From(result.ToArray(), totalCount.Value);
        }


        //-------------------------------库存记录----------------------------------

        public async Task<RouteData<OutsideInventoryRecordDto[]>> QueryInventoryRecord(long? reservoirAreaId, long? storageRackId, long? inventoryBoxId, long? materialId, int pageIndex, int pageSize, string search, string[] order, string datemin, string datemax)
        {
            ISugarQueryable<Wms_inventoryrecord> query = _sqlClient.Queryable<Wms_inventoryrecord>()
                            .Where((x) => x.IsDel == DeleteFlag.Normal);
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where((x) => x.MaterialNo.ToString().Contains(search) || x.MaterialOnlyId.ToString().Contains(search) || x.MaterialName.Contains(search));
            }
            DateTime minDate;
            if (!string.IsNullOrWhiteSpace(datemin) && DateTime.TryParse(datemin, out minDate))
            {
                query = query.Where((x) => x.CreateDate >= minDate);
            }
            DateTime maxDate;
            if (!string.IsNullOrWhiteSpace(datemax) && DateTime.TryParse(datemax, out maxDate))
            {
                query = query.Where((x) => x.CreateDate <= maxDate);
            }
            query = query.Sort(order, new string[,] { { "POSITION", "INVENTORYPOSITION" } });

            RefAsync<int> totalCount = new RefAsync<int>();

            IEnumerable<OutsideInventoryRecordDto> result = (await query.ToPageListAsync(pageIndex, pageSize, totalCount)).Select(
              (x) => new OutsideInventoryRecordDto
              {
                  StockNo = x.StockNo,
                  MaterialId = x.MaterialId,
                  MaterialNo = x.MaterialNo,
                  MaterialOnlyId = x.MaterialOnlyId,
                  MaterialName = x.MaterialName,
                  Qty = x.Qty,
                  AfterQty = x.AfterQty,
                  InventoryBoxId = x.InventoryBoxId,
                  InventoryBoxNo = x.InventoryBoxNo,
                  Position = x.InventoryPosition,
                  CreateBy = x.CreateUser,
                  CreateDate = x.CreateDate.Value.ToString(PubConst.Format_DateTime),
                  ModifiedBy = x.ModifiedUser,
                  ModifiedDate = x.ModifiedDate?.ToString(PubConst.Format_DateTime),
              });


            return RouteData<OutsideInventoryRecordDto[]>.From(result.ToArray(), totalCount.Value);
        }

        //-------------------------------入库----------------------------------


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

            string stockInNo = request.StockInNo ?? request.WarehousingId;
            if(await _sqlClient.Queryable<Wms_stockin>().AnyAsync(x => x.StockInNo == stockInNo))
            { 
                return RouteData<Wms_stockin[]>.From(PubMessages.E2021_STOCKIN_DUPLICATE);
            }

            List<Wms_stockin> stockInList = new List<Wms_stockin>();
            List<Wms_stockindetail> stockinDetailList = new List<Wms_stockindetail>();
            Wms_stockin stockin = new Wms_stockin()
            {
                MesTaskId = request.MesTaskId,
                StockInId = request.StockInId ?? CreateStockTaskID(request.WarehouseId),
                StockInNo = stockInNo,
                StockInDate = Convert.ToDateTime(request.WarehousingTime),
                StockInType = stockinDict.DictId,
                StockInTypeName = stockinDict.DictName,
                OrderNo = request.OrderNo,
                SupplierId = -1,
                WarehouseId = request.WarehouseId,
                StockInStatus = StockInStatus.task_confirm.ToByte(),
                IsDel = DeleteFlag.Normal.ToByte(),
                CreateBy = PubConst.InterfaceUserId,
                CreateUser = PubConst.InterfaceUserName,
                CreateDate = DateTime.Now,
                ModifiedBy = PubConst.InterfaceUserId,
                ModifiedUser = PubConst.InterfaceUserName,
                ModifiedDate = DateTime.Now,

            };
            stockInList.Add(stockin);

            foreach (Wms_WarehousingMaterialInventoryDto materialDto in request.MaterialList)
            {
                RouteData<Wms_material> materialResult = await GetMaterial(materialDto, true);
                if (!materialResult.IsSccuess)
                {
                    return RouteData<Wms_stockin[]>.From(materialResult);
                }
                Wms_material material = materialResult.Data;

                Wms_stockindetail detail = new Wms_stockindetail()
                {
                    StockInDetailId = PubId.SnowflakeId,
                    StockInId = stockin.StockInId,
                    WarehouseId = request.WarehouseId,
                    SubWarehousingId = materialDto.SubWarehousingId,
                    UniqueIndex = materialDto.UniqueIndex,
                    MaterialId = material.MaterialId,
                    MaterialNo = material.MaterialNo,
                    MaterialOnlyId = material.MaterialOnlyId,
                    MaterialName = material.MaterialName,
                    PlanInQty = materialDto.Qty,
                    ActInQty = 0,
                    Status = StockInStatus.task_confirm.ToByte(),
                    IsDel = DeleteFlag.Normal.ToByte(),
                    CreateBy = PubConst.InterfaceUserId,
                    CreateUser = PubConst.InterfaceUserName,
                    CreateDate = DateTime.Now,
                    ModifiedBy = PubConst.InterfaceUserId,
                    ModifiedUser = PubConst.InterfaceUserName,
                    ModifiedDate = DateTime.Now,
                    Remark = ""
                };
                stockinDetailList.Add(detail);
            }
            try
            {
                if (_sqlClient.Insertable(stockInList).ExecuteCommand() == 0)
                {
                    throw new Exception("stockInList更新失败");
                }
                if (_sqlClient.Insertable(stockinDetailList).ExecuteCommand() == 0)
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

        public async Task<RouteData<OutsideStockInQueryResult[]>> QueryStockInList(long? stockInType, StockInStatus? stockInStatus, int pageIndex, int pageSize, string search, string[] order, string datemin, string datemax)
        {
            var query = _sqlClient.Queryable<Wms_stockin>()
                 .Where((s) => s.WarehouseId == this.Warehouse.WarehouseId && s.IsDel == 1)
                 ;
            if (!string.IsNullOrEmpty(search))
            {
                query.Where((s) => s.StockInNo.Contains(search) || s.OrderNo.Contains(search));
            }
            if (stockInType != null)
            {
                query.Where((s) => s.StockInType == stockInType);
            }
            if (stockInStatus != null)
            {
                query.Where((s) => s.StockInStatus == stockInStatus.ToByte());
            }
            DateTime minDate;
            if (!string.IsNullOrWhiteSpace(datemin) && DateTime.TryParse(datemin, out minDate))
            {
                query = query.Where((s) => s.CreateDate >= minDate);
            }
            DateTime maxDate;
            if (!string.IsNullOrWhiteSpace(datemax) && DateTime.TryParse(datemax, out maxDate))
            {
                query = query.Where((s) => s.CreateDate <= maxDate);
            }
            query = query.Sort(order);

            RefAsync<int> totalNumber = 0;
            List<Wms_stockin> datas = await query.ToPageListAsync(pageIndex, pageSize, totalNumber);
            OutsideStockInQueryResult[] result = datas.Select((s) => new OutsideStockInQueryResult()
            {
                StockInId = s.StockInId.ToString(),
                StockInTypeName = s.StockInTypeName,
                MesTaskId = s.MesTaskId.ToString(),
                OrderNo = s.OrderNo,
                StockInNo = s.StockInNo,
                StockInStatus = (StockInStatus)s.StockInStatus,
                CreateBy = s.CreateUser,
                CreateDate = s.CreateDate.Value.ToString(PubConst.Format_DateTime),
                ModifiedBy = s.ModifiedUser,
                ModifiedDate = s.ModifiedDate.Value.ToString(PubConst.Format_DateTime)
            }).ToArray();
            return RouteData<OutsideStockInQueryResult[]>.From(result, totalNumber.Value);
        }

        public async Task<RouteData<OutsideStockInQueryResult[]>> QueryStockInListNew(long? stockInType, StockInStatus? stockInStatus, int pageIndex, int pageSize, string search, string[] order, string datemin, string datemax)
        {
            var query = _sqlClient.Queryable<Wms_stockin,Wms_stockindetail>((si, sid) => new object[] {
                    JoinType.Right,si.StockInId==sid.StockInId
                })
               .Where((si, sid) => si.WarehouseId == this.Warehouse.WarehouseId && si.IsDel == 1)
               ;
            if (!string.IsNullOrEmpty(search))
            {
                query.Where((si, sid) => si.StockInNo.Contains(search) || si.OrderNo.Contains(search) 
                    || sid.MaterialNo.Contains(search) || sid.MaterialOnlyId.Contains(search) || sid.MaterialName.Contains(search)
                    || sid.UniqueIndex.Contains(search)
                    || sid.PlanInQty.ToString().Contains(search));
            }
            if (stockInType != null)
            {
                query.Where((si, sid) => si.StockInType == stockInType);
            }
            if (stockInStatus != null)
            {
                query.Where((si, sid) => si.StockInStatus == stockInStatus.ToByte());
            }
            DateTime minDate;
            if (!string.IsNullOrWhiteSpace(datemin) && DateTime.TryParse(datemin, out minDate))
            {
                query = query.Where((si, sid) => si.StockInDate >= minDate);
            }
            DateTime maxDate;
            if (!string.IsNullOrWhiteSpace(datemax) && DateTime.TryParse(datemax, out maxDate))
            {
                query = query.Where((si, sid) => si.StockInDate <= maxDate);
            }
            query = query.Sort(order, new string[,] {
                {"DETAILSTATUS", "sid.STATUS" },
                {"CREATEDATE", "si.CREATEDATE" },
                {"CREATEBY", "si.CREATEUSER" },
                {"MODIFIEDDATE", "si.MODIFIEDDATE" },
                {"MODIFIEDBY", "si.MODIFIEDUSER" } } ); 
            RefAsync<int> totalNumber = 0;
            
            List<OutsideStockInQueryResult> result = await query.Select((si, sid) => new OutsideStockInQueryResult()
            {
                StockInId = si.StockInId.ToString(),
                StockInTypeName = si.StockInTypeName,
                MesTaskId = si.MesTaskId.ToString(),
                OrderNo = si.OrderNo,
                StockInNo = si.StockInNo,
                StockInStatus = (StockInStatus)si.StockInStatus,

                SubWarehousingId = sid.SubWarehousingId,
                StockInDetailId = sid.StockInDetailId.ToString(),
                UniqueIndex = sid.UniqueIndex,
                MaterialId = sid.MaterialId.ToString(),
                MaterialNo = sid.MaterialNo,
                MaterialOnlyId = sid.MaterialOnlyId,
                MaterialName = sid.MaterialName,
                PlanInQty = sid.PlanInQty,
                ActInQty = sid.ActInQty,
                DetailStatus = (StockInStatus)sid.Status,

                CreateBy = si.CreateUser,
                CreateDate = si.CreateDate.Value.ToString(PubConst.Format_DateTime),
                ModifiedBy = si.ModifiedUser,
                ModifiedDate = si.ModifiedDate.Value.ToString(PubConst.Format_DateTime)
            }).ToPageListAsync(pageIndex, pageSize, totalNumber);
            return RouteData<OutsideStockInQueryResult[]>.From(result.ToArray(), totalNumber.Value);
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
                    StockInDetailId = stockInDetail.StockInDetailId.ToString(),
                    SubWarehousingId = stockInDetail.SubWarehousingId,
                    UniqueIndex = stockInDetail.UniqueIndex,
                    MaterialId = stockInDetail.MaterialId.Value.ToString(),
                    MaterialNo = stockInDetail.MaterialNo,
                    MaterialName = stockInDetail.MaterialName,
                    MaterialOnlyId = stockInDetail.MaterialOnlyId,
                    PlanInQty = stockInDetail.PlanInQty,
                    ActInQty = stockInDetail.ActInQty,
                    Remark = stockInDetail.Remark,
                    CreateBy = stockInDetail.CreateUser,
                    CreateDate = stockInDetail.CreateDate.Value.ToString(PubConst.Format_DateTime),
                    ModifiedBy = stockInDetail.ModifiedUser,
                    ModifiedDate = stockInDetail.ModifiedDate.Value.ToString(PubConst.Format_DateTime),

                    Status = (StockInStatus)stockInDetail.Status
                };
                details.Add(detail);
            }

            OutsideStockInQueryResult result = new OutsideStockInQueryResult()
            {
                StockInId = stockIn.StockInId.ToString(),
                StockInNo = stockIn.StockInNo,
                MesTaskId = stockIn.MesTaskId.Value.ToString(),
                StockInTypeName = stockIn.StockInTypeName,
                OrderNo = stockIn.OrderNo,
                StockInStatus = (StockInStatus)stockIn.StockInStatus,
                Remark = stockIn.Remark,
                Details = details.ToArray(),
                CreateBy = stockIn.CreateUser,
                CreateDate = stockIn.CreateDate.Value.ToString(PubConst.Format_DateTime),
                ModifiedBy = stockIn.ModifiedUser,
                ModifiedDate = stockIn.ModifiedDate.Value.ToString(PubConst.Format_DateTime)
            };
            return RouteData<OutsideStockInQueryResult>.From(result);
        }

        //-------------------------------出库----------------------------------

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
                    StockOutDetailId = stockOutDetail.StockOutDetailId.ToString(),
                    SubWarehouseEntryId = stockOutDetail.SubWarehouseEntryId,
                    UniqueIndex = stockOutDetail.UniqueIndex,
                    MaterialId = stockOutDetail.MaterialId.Value.ToString(),
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
                StockOutId = stockOut.StockOutId.ToString(),
                StockOutNo = stockOut.StockOutNo,
                MesTaskId = stockOut.MesTaskId.ToString(),
                StockOutTypeName = stockOut.
                OrderNo = stockOut.OrderNo,
                IsPaused = stockOut.IsPaused,
                StockOutStatus = (StockOutStatus)stockOut.StockOutStatus,
                Remark = stockOut.Remark,
                Details = details.ToArray(),
                ModifiedBy = stockOut.ModifiedUser,
                ModifiedDate = stockOut.ModifiedDate.Value.ToString(PubConst.Format_DateTime)
            };
            return RouteData<OutsideStockOutQueryResult>.From(result);
        }

        public async Task<RouteData<OutsideStockOutQueryResult[]>> QueryStockOutList(long? stockOutType, StockOutStatus? stockOutStatus, int pageIndex, int pageSize, string search, string[] order, string datemin, string datemax)
        {
            var query = _sqlClient.Queryable<Wms_stockout>()
              .Where((s) => s.WarehouseId == this.Warehouse.WarehouseId && s.IsDel == 1)
              ;
            if (!string.IsNullOrEmpty(search))
            {
                query.Where((s) => s.StockOutNo.Contains(search) || s.OrderNo.Contains(search) || 
                SqlFunc.Subqueryable<Wms_stockoutdetail>().Where( 
                    sod => sod.StockOutId == s.StockOutId && (
                        sod.MaterialNo.Contains(search)
                        || sod.MaterialOnlyId.Contains(search)
                        || sod.MaterialName.Contains(search)
                        || sod.UniqueIndex.Contains(search) 
                        || sod.SubWarehouseEntryId.Contains(search) 

                    ) ).Any()
                );
            }
            if (stockOutType != null)
            {
                query.Where((s) => s.StockOutType == stockOutType);
            }
            if (stockOutStatus != null)
            {
                query.Where((s) => s.StockOutStatus == stockOutStatus.ToByte());
            }
            DateTime minDate;
            if (!string.IsNullOrWhiteSpace(datemin) && DateTime.TryParse(datemin, out minDate))
            {
                query = query.Where((s) => s.CreateDate >= minDate);
            }
            DateTime maxDate;
            if (!string.IsNullOrWhiteSpace(datemax) && DateTime.TryParse(datemax, out maxDate))
            {
                query = query.Where((s) => s.CreateDate <= maxDate);
            }

            query = query.Sort(order);
            RefAsync<int> totalNumber = 0;
            List<Wms_stockout> datas = await query.ToPageListAsync(pageIndex, pageSize, totalNumber);
            OutsideStockOutQueryResult[] result = datas.Select((s) => new OutsideStockOutQueryResult()
            {
                StockOutId = s.StockOutId.ToString(),
                StockOutTypeName = s.StockOutTypeName,
                MesTaskId = s.MesTaskId.ToString(),
                OrderNo = s.OrderNo,
                WorkNo = s.WorkNo,
                BatchNumber = s.BatchNumber,
                BatchPlanId = s.BatchPlanId,
                WorkAreaName = s.WorkAreaName,
                WorkStationId = s.WorkStationId,
                StockOutNo = s.StockOutNo,
                IsPaused = s.IsPaused,
                StockOutStatus = (StockOutStatus)s.StockOutStatus,
                CreateBy = s.CreateUser,
                CreateDate = s.CreateDate.Value.ToString(PubConst.Format_DateTime),
                ModifiedBy = s.ModifiedUser,
                ModifiedDate = s.ModifiedDate.Value.ToString(PubConst.Format_DateTime)
            }).ToArray();
            return RouteData<OutsideStockOutQueryResult[]>.From(result, totalNumber.Value);
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
            //TODO 出库单号一至的情况下认为是修改出库单
            Sys_dict stockoutDict = await _sqlClient.Queryable<Sys_dict>()
                .FirstAsync(x => x.DictType == PubDictType.stockout.ToByte().ToString() && x.DictName == request.WarehouseEntryType);

            if (stockoutDict == null)
            {
                return RouteData<Wms_stockout[]>.From(PubMessages.E1004_WAREHOUSETYPE_NOTFOUND);
            }

            string stockOutNo = request.StockOutNo ?? request.WarehouseEntryId;
            Wms_stockout dbStockout = await _sqlClient.Queryable<Wms_stockout>().FirstAsync(x => x.StockOutNo == stockOutNo);
            if (dbStockout != null)
            { 
                if(dbStockout.StockOutStatus != (int)StockOutStatus.task_confirm)
                { 
                    return RouteData<Wms_stockout[]>.From(PubMessages.E2125_STOCKOUT_DUPLICATE);
                }
                List<Wms_stockoutdetail> dbStockoutdetails = await _sqlClient.Queryable<Wms_stockoutdetail>()
                    .Where(x => x.StockOutId == dbStockout.StockOutId)
                    .ToListAsync();
                foreach (Wms_WarehouseEntryMaterialInventoryDto materialDto in request.MaterialList)
                {
                    Wms_stockoutdetail dbDetail = dbStockoutdetails.FirstOrDefault(x => x.UniqueIndex == materialDto.UniqueIndex);
                    if(dbDetail == null)
                    { 
                        return RouteData<Wms_stockout[]>.From(PubMessages.E2126_STOCKOUTDETAIL_NOTFOUND,$"UniqueIndex={materialDto.UniqueIndex}");
                    }
                    dbDetail.PlanOutQty = materialDto.Qty;
                    if(dbDetail.PlanOutQty == 0)
                    {
                        dbDetail.Status = (int)StockOutStatus.task_finish;
                    }
                    dbDetail.ModifiedBy = PubConst.InterfaceUserId;
                    dbDetail.ModifiedUser = PubConst.InterfaceUserName;
                    dbDetail.ModifiedDate = DateTime.Now;
                }
                if(_sqlClient.Updateable(dbStockoutdetails).ExecuteCommand() == 0)
                {
                    RouteData<Wms_stockout[]>.From(PubMessages.E0002_UPDATE_COUNT_FAIL,"更新出库详细失败");
                }
                return RouteData<Wms_stockout[]>.From(new Wms_stockout[] { dbStockout });
            }

            //TODO stockOut不再需要列表的形式
            List<Wms_stockout> stockOutList = new List<Wms_stockout>();
            List<Wms_stockoutdetail> stockOutDetailList = new List<Wms_stockoutdetail>();
            foreach (Wms_WarehouseEntryMaterialInventoryDto materialDto in request.MaterialList)
            {
                RouteData<Wms_material> materialResult = await GetMaterial(materialDto, false);
                if (!materialResult.IsSccuess)
                {
                    return RouteData<Wms_stockout[]>.From(materialResult);
                }
                Wms_material material = materialResult.Data;
                Wms_stockout stockout = stockOutList.FirstOrDefault(x => x.WarehouseId == request.WarehouseId);
                if (stockout == null)
                {
                    stockout = new Wms_stockout()
                    {
                        MesTaskId = request.MesTaskId,
                        StockOutId = request.StockOutId ?? CreateStockTaskID(request.WarehouseId),
                        StockOutNo = request.StockOutNo ?? request.WarehouseEntryId,
                        StockOutDate = Convert.ToDateTime(request.WarehouseEntryTime),
                        StockOutType = stockoutDict.DictId,
                        StockOutTypeName = stockoutDict.DictName,
                        OrderNo = request.OrderNo,
                        WorkNo = request.WorkNo,
                        BatchNumber = request.BatchNumber,
                        BatchPlanId = request.BatchPlanId,
                        WorkAreaName = request.WorkAreaName,
                        WorkStationId = request.WorkStationId,
                        WarehouseId = request.WarehouseId,
                        StockOutStatus = StockOutStatus.task_confirm.ToByte(),
                        IsDel = DeleteFlag.Normal.ToByte(),
                        CreateBy = PubConst.InterfaceUserId,
                        CreateUser = PubConst.InterfaceUserName,
                        CreateDate = DateTime.Now,
                        ModifiedBy = PubConst.InterfaceUserId,
                        ModifiedUser = PubConst.InterfaceUserName,
                        ModifiedDate = DateTime.Now,
                    }; ; stockOutList.Add(stockout);
                }

                Wms_stockoutdetail detail = new Wms_stockoutdetail()
                {
                    StockOutDetailId = PubId.SnowflakeId,
                    StockOutId = stockout.StockOutId,
                    WarehouseId = request.WarehouseId,
                    SubWarehouseEntryId = materialDto.SubWarehouseEntryId,
                    UniqueIndex = materialDto.UniqueIndex,
                    MaterialId = material.MaterialId,
                    MaterialNo = material.MaterialNo,
                    MaterialOnlyId = material.MaterialOnlyId,
                    MaterialName = material.MaterialName,
                    PlanOutQty = materialDto.Qty,
                    ActOutQty = 0,
                    Status = StockOutStatus.task_confirm.ToByte(),
                    IsDel = DeleteFlag.Normal.ToByte(),
                    CreateBy = PubConst.InterfaceUserId,
                    CreateUser = PubConst.InterfaceUserName,
                    CreateDate = DateTime.Now,
                    ModifiedBy = PubConst.InterfaceUserId,
                    ModifiedUser = PubConst.InterfaceUserName,
                    ModifiedDate = DateTime.Now,
                    Remark = ""
                };
                stockOutDetailList.Add(detail);
            }
            try
            {
                if (_sqlClient.Insertable(stockOutList).ExecuteCommand() == 0)
                {
                    throw new Exception("stockOutList更新失败");
                }
                if (_sqlClient.Insertable(stockOutDetailList).ExecuteCommand() == 0)
                {
                    throw new Exception("stockOutDetailList更新失败");
                }
                return RouteData<Wms_stockout[]>.From(stockOutList.ToArray());
            }
            catch (Exception e)
            {
                return RouteData<Wms_stockout[]>.From(PubMessages.E0004_DATABASE_UPDATE_FAIL, e);
            }

        }

        private async Task<RouteData<Wms_material>> GetMaterial(Wms_MaterialInventoryDto materialDto, bool autoCreate)
        {
            Wms_material material = null;
            if (string.IsNullOrEmpty(materialDto.MaterialOnlyId))
            {
                material = await _sqlClient.Queryable<Wms_material>().FirstAsync(x => x.MaterialNo == materialDto.MaterialNo && string.IsNullOrEmpty(x.MaterialOnlyId));
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
                    typeDict = new Sys_dict()
                    {
                        DictId = PubId.SnowflakeId,
                        DictName = materialDto.MaterialType,
                        DictType = PubDictType.material.ToByte().ToString(),
                        CreateBy = PubConst.InterfaceUserId,
                        CreateDate = DateTime.Now,
                        ModifiedBy = PubConst.InterfaceUserId,
                        ModifiedDate = DateTime.Now,
                        IsDel= DeleteFlag.Normal
                       
                    };
                    _sqlClient.Insertable(typeDict).ExecuteCommand();
                }
            
                Sys_dict unitDict = await _sqlClient.Queryable<Sys_dict>().FirstAsync(x => x.DictType == PubDictType.unit.ToByte().ToString() && x.DictName == materialDto.Unit);
                if (unitDict == null)
                {
                    unitDict = new Sys_dict()
                    {
                        DictId = PubId.SnowflakeId,
                        DictName = materialDto.Unit,
                        DictType = PubDictType.unit.ToByte().ToString(),
                        CreateBy = PubConst.InterfaceUserId,
                        CreateDate = DateTime.Now,
                        ModifiedBy = PubConst.InterfaceUserId,
                        ModifiedDate = DateTime.Now,
                        IsDel = DeleteFlag.Normal

                    };
                    _sqlClient.Insertable(unitDict).ExecuteCommand();
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
                    UnitName = unitDict.DictName,
                    CreateBy = PubConst.InterfaceUserId,
                    CreateUser = PubConst.InterfaceUserName,
                    CreateDate = DateTime.Now,
                    ModifiedBy = PubConst.InterfaceUserId,
                    ModifiedUser = PubConst.InterfaceUserName,
                    ModifiedDate = DateTime.Now,
                };
                if (_sqlClient.Insertable(material).ExecuteCommand() == 0)
                {
                    return RouteData<Wms_material>.From(PubMessages.E0004_DATABASE_UPDATE_FAIL);
                }
            }
            return RouteData<Wms_material>.From(material);

        }

        private long CreateStockTaskID(long warehouseId)
        {
            return long.Parse($"{warehouseId}{DateTime.Now.ToString("yyyyMMddHHmmss")}");
        }

        //-------------------------------盘库----------------------------------

        /// <summary>
        /// 下发盘库任务
        /// </summary>
        /// <param name="stockCountNo"></param>
        /// <param name="planDate"></param>
        /// <param name="materials"></param>
        /// <returns></returns>
        public async Task<RouteData> StockCount(OutsideStockCountRequestDto request)
        {
            try
            {
                if( await _sqlClient.Queryable<Wms_stockcount>().AnyAsync(x => x.StockCountNo == request.StockCountNo))
                {
                    return RouteData.From(PubMessages.E2205_STOCKCOUNT_DUPLICATE, "盘库任务添加失败"); 
                }
                _sqlClient.Ado.BeginTran();
                Wms_stockcount stockcount = new Wms_stockcount()
                { 
                    StockCountNo = request.StockCountNo,
                    StockCountDate = Convert.ToDateTime( request.PlanDate ),
                    MesTaskId = request.MesTaskId,
                    WarehouseId = this.Warehouse.WarehouseId,
                    StepTotalCount = null,
                    StepFinishCount = null,
                    MaterialCount = request.MaterialList.Length,
                    Status = (int)StockCountStatus.task_confirm,
                    IsDel = DeleteFlag.Normal,
                    CreateBy = PubConst.InterfaceUserId,
                    CreateUser = PubConst.InterfaceUserName,
                    CreateDate = DateTime.Now,
                    ModifiedBy = PubConst.InterfaceUserId,
                    ModifiedUser = PubConst.InterfaceUserName,
                    ModifiedDate = DateTime.Now
                };

                List<Wms_stockcount_material> materialList = new List<Wms_stockcount_material>();
                foreach (OutsideStockCountMaterial om in request.MaterialList)
                {
                    Wms_material material = null;
                    if (string.IsNullOrEmpty(om.MaterialOnlyId))
                    {
                        material = await _sqlClient.Queryable<Wms_material>().FirstAsync(x => x.MaterialNo == om.MaterialNo);
                    }
                    else
                    {
                        material = await _sqlClient.Queryable<Wms_material>().FirstAsync(x => x.MaterialOnlyId == om.MaterialOnlyId);
                    }
                    if (material == null)
                    {
                        return RouteData.From(PubMessages.E1005_MATERIALNO_NOTFOUND);
                    }
                    Wms_stockcount_material sm = new Wms_stockcount_material()
                    {
                        StockCountMaterialId = PubId.SnowflakeId.ToString(), 
                        StockCountNo = stockcount.StockCountNo,
                        MaterialId = material.MaterialId.ToString(),
                        MaterialNo = material.MaterialNo,
                        MaterialName = material.MaterialName,
                        MaterialOnlyId = material.MaterialOnlyId,
                        MaterialTypeName = material.MaterialTypeName,
                        UnitName = material.UnitName,
                        PrevNumber = om.PrevNumber,
                        ProjectedQty = 0,
                        StockCountQty = 0,
                        Status = (int)StockCountStatus.task_confirm,
                        IsDel = DeleteFlag.Normal,
                        CreateBy = PubConst.InterfaceUserId,
                        CreateUser = PubConst.InterfaceUserName,
                        CreateDate = DateTime.Now,
                        ModifiedBy = PubConst.InterfaceUserId,
                        ModifiedUser = PubConst.InterfaceUserName,
                        ModifiedDate = DateTime.Now
                    };
                    materialList.Add(sm);
                }

                if(_sqlClient.Insertable(stockcount).ExecuteCommand() == 0)
                {
                    _sqlClient.Ado.RollbackTran();
                    return RouteData.From(PubMessages.E0002_UPDATE_COUNT_FAIL,"盘库任务添加失败");
                }
                if (_sqlClient.Insertable(materialList).ExecuteCommand() == 0)
                {
                    _sqlClient.Ado.RollbackTran();
                    return RouteData.From(PubMessages.E0002_UPDATE_COUNT_FAIL, "盘库物料添加失败"); 
                } 
                _sqlClient.Ado.CommitTran();
                return new RouteData();
            }
            catch(Exception ex)
            {
                _sqlClient.Ado.RollbackTran();
                return RouteData.From(-1, ex.Message);
            }
        }
         

        public async Task<RouteData<OutsideStockCountDto[]>> QueryStockCountList(StockCountStatus? stockCountStatus, int pageIndex, int pageSize, string search, string[] order, string datemin, string datemax)
        {
            var query = _sqlClient.Queryable<Wms_stockcount>()
              .Where((s) => s.WarehouseId == this.Warehouse.WarehouseId && s.IsDel == 1)
              ;
            if (!string.IsNullOrEmpty(search))
            {
                query.Where((s) => s.StockCountNo.Contains(search) || s.Remark.Contains(search));
            } 
            if (stockCountStatus != null)
            {
                query.Where((s) => s.Status == stockCountStatus.ToByte());
            }
            DateTime minDate;
            if (!string.IsNullOrWhiteSpace(datemin) && DateTime.TryParse(datemin, out minDate))
            {
                query = query.Where((s) => s.CreateDate >= minDate);
            }
            DateTime maxDate;
            if (!string.IsNullOrWhiteSpace(datemax) && DateTime.TryParse(datemax, out maxDate))
            {
                query = query.Where((s) => s.CreateDate <= maxDate);
            }

            query = query.Sort(order);
            RefAsync<int> totalNumber = 0;
            List<Wms_stockcount> result = await query.ToPageListAsync(pageIndex, pageSize, totalNumber);
            OutsideStockCountDto[] datas = result.CastTo<OutsideStockCountDto[]>();
            return RouteData<OutsideStockCountDto[]>.From(datas, totalNumber.Value);
        }

        public async Task<RouteData<OutsideStockCountDto>> QueryStockCount(string stockCountNo)
        {
            var query = _sqlClient.Queryable<Wms_stockcount>()
               .Where((x) => x.WarehouseId == this.Warehouse.WarehouseId 
                   && x.StockCountNo == stockCountNo 
                   && x.IsDel == DeleteFlag.Normal
                   )
               ;  
            
            RefAsync<int> totalNumber = 0;
            Wms_stockcount data = await query.FirstAsync();
            if(data == null)
            {
                return null;
            }

            var materials = _sqlClient.Queryable<Wms_stockcount_material>().Where( (x) =>
                x.StockCountNo == stockCountNo
                && x.IsDel == DeleteFlag.Normal
            ).ToArray();
            var steps = _sqlClient.Queryable<Wms_stockcount_step,Wms_inventorybox>(
                (scs, ib) => new object[] {
                    JoinType.Left,scs.InventoryBoxId==ib.InventoryBoxId
                }
            )
            .Where(
                (scs, ib) =>
                    scs.StockCountNo == stockCountNo
                    && scs.IsDel == DeleteFlag.Normal
            )
            .Select(
                (scs, ib) =>
                    new OutsideStockCountStep
                    {
                        StepId = scs.StepId,
                        StockCountNo = scs.StockCountNo,
                        MaterialId = scs.MaterialId,
                        MaterialOnlyId = scs.MaterialOnlyId,
                        MaterialNo = scs.MaterialNo,
                        MaterialName = scs.MaterialName,
                        MaterialTypeName = scs.MaterialTypeName,
                        UnitName = scs.UnitName,
                        BrandNo = scs.BrandNo,
                        BeforeCount = scs.BeforeCount,
                        StockCount = scs.StockCount,
                        DiffCount = scs.DiffCount,
                        PackageCount = scs.PackageCount,
                        IsMark = scs.IsMark,
                        IsMixture = scs.IsMixture,
                        IsExteriorPerfect = scs.IsExteriorPerfect,
                        InventoryBoxId = scs.InventoryBoxId,
                        InventoryBoxNo = scs.InventoryBoxNo,
                        InventoryBoxName = scs.InventoryBoxName,
                        InventoryPosition = scs.InventoryPosition,
                        Status = scs.Status,
                        Remark = scs.Remark,
                        IsDel = scs.IsDel,
                        CreateBy = scs.CreateBy,
                        CreateUser = scs.CreateUser,
                        CreateDate = scs.CreateDate,
                        StockCountBy = scs.StockCountBy,
                        StockCountUser = scs.StockCountUser,
                        StockCountDate = scs.StockCountDate,
                        InventoryBoxStatus = ib.Status
                    }
            )
            .ToArray();

            OutsideStockCountDto result = data.CastTo<OutsideStockCountDto>();
            result.MaterialList = materials.CastTo<OutsideStockCountMaterial[]>();
            result.StepList = steps;


            return RouteData<OutsideStockCountDto>.From(result);
        }

        public void Dispose()
        {
            _sqlClient = null;
        }

    }
}

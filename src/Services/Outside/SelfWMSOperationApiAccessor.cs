using IServices.Outside;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WMSCore.Outside;
using YL.Core.Dto;
using YL.Core.Entity; 
using YL.Utils.Pub;
using YL.Utils.Extensions;
using System.Linq;
using Newtonsoft.Json;

namespace Services.Outside
{
    public class SelfWMSOperationApiAccessor : IWMSOperationApiAccessor
    {

        private ISqlSugarClient _sqlClient;
        private Wms_warehouse _warehouse;
        public Wms_warehouse Warehouse { get { return _warehouse; } } 
        private SysUserDto _userDto;
        private SysUserDto UserDto { get { return _userDto; } }

        public SelfWMSOperationApiAccessor(Wms_warehouse warehouse, SqlSugar.ISqlSugarClient sqlClient, SysUserDto userDto)
        {
            _warehouse = warehouse;
            _sqlClient = sqlClient;
            _userDto = userDto;
        }
        
        public async Task<RouteData<Wms_inventoryboxTask>> GetInventoryBoxkTask(long inventoryBoxTaskId)
        {
            Wms_inventoryboxTask task = await _sqlClient.Queryable<Wms_inventoryboxTask>()
                .FirstAsync(x => x.InventoryBoxTaskId == inventoryBoxTaskId);
            if (task == null)
            {
                return RouteData<Wms_inventoryboxTask>.From(PubMessages.E1013_INVENTORYBOXTASK_NOTFOUND);
            }
            Wms_inventorybox box = await _sqlClient.Queryable<Wms_inventorybox>()
                .FirstAsync(x => x.InventoryBoxId == task.InventoryBoxId);
            if (box == null)
            {
                return RouteData<Wms_inventoryboxTask>.From(PubMessages.E1011_INVENTORYBOX_NOTFOUND);
            }
            return RouteData<Wms_inventoryboxTask>.From(task);
        }

        public async Task<RouteData> DoAutoSelectBoxOut(int requestSize)
        {


            //查询是否有符合要求的料箱
            var query = _sqlClient.Queryable<Wms_inventorybox>()
               .Where((ib) => ib.UsedSize == 0 && ib.Status == InventoryBoxStatus.InPosition);
            if (requestSize == 1)
            {
                query = query.Where((ib) => ib.Size == 1);
            }
            else
            {
                query = query.Where((ib) => ib.Size == requestSize);
            }
            Wms_inventorybox inventoryBox = await query.FirstAsync();
            //如果没有符合要求的料想,且是要求多宫格料箱时,尝试选取完整料箱进行分割
            if (inventoryBox == null && requestSize > 1)
            {
                inventoryBox = await _sqlClient.Queryable<Wms_inventorybox>()
                    .Where((ib) => ib.Size == 1 && ib.UsedSize == 0 && ib.Status == InventoryBoxStatus.InPosition).FirstAsync();
                if (inventoryBox != null)
                {
                    inventoryBox.Size = requestSize;
                }
                if (_sqlClient.Updateable(inventoryBox).ExecuteCommand() == 0)
                {
                    return YL.Core.Dto.RouteData.From(PubMessages.E0004_DATABASE_UPDATE_FAIL);
                }
            }
            if (inventoryBox == null)
            {
                return YL.Core.Dto.RouteData.From(PubMessages.E1007_INVENTORYBOX_ALLOWUSED);
            }
            RouteData result = await DoInventoryBoxOut( inventoryBox.InventoryBoxId);

            return result;
        }

        /// <summary>
        /// 出库料箱
        /// </summary>
        /// <param name="inventoryBoxId"></param>
        /// <returns></returns>
        public async Task<RouteData> DoInventoryBoxOut(long inventoryBoxId)
        {
            try
            {
                _sqlClient.Ado.BeginTran();
                RouteData result = await DoInventoryBoxOutCore(inventoryBoxId);
                if (!result.IsSccuess)
                {
                    _sqlClient.Ado.RollbackTran();
                    return result;
                }
                _sqlClient.Ado.CommitTran();
                return result;
            }
            catch (Exception)
            {
                _sqlClient.Ado.RollbackTran();
                return YL.Core.Dto.RouteData.From(PubMessages.E2005_STOCKIN_BOXOUT_FAIL);
            }

        }

        /// <summary>
        /// 出库料箱核心
        /// </summary>
        /// <param name="inventoryBoxId"></param>
        /// <returns></returns>
        private async Task<RouteData> DoInventoryBoxOutCore(long inventoryBoxId)
        {
            Wms_inventorybox inventoryBox = await _sqlClient.Queryable<Wms_inventorybox>().FirstAsync(x => x.InventoryBoxId == inventoryBoxId);
            if (inventoryBox == null) { return YL.Core.Dto.RouteData.From(PubMessages.E1011_INVENTORYBOX_NOTFOUND); }
            if (inventoryBox.Status != InventoryBoxStatus.InPosition) { return YL.Core.Dto.RouteData.From(PubMessages.E1012_INVENTORYBOX_NOTINPOSITION); }

            Wms_inventoryboxTask task = await _sqlClient.Queryable<Wms_inventoryboxTask>().FirstAsync(
                x => x.InventoryBoxId == inventoryBox.InventoryBoxId && x.Status == InventoryBoxStatus.Outed.ToByte());
            if (task != null)
            {
                return YL.Core.Dto.RouteData.From(PubMessages.E2301_WCS_STOCKOUT_NOTCOMPLATE_EXIST);
            }
            task = new Wms_inventoryboxTask()
            {
                InventoryBoxTaskId = PubId.SnowflakeId,
                InventoryBoxId = inventoryBoxId,
                InventoryBoxNo = inventoryBox.InventoryBoxNo,
                ReservoirareaId = (long)inventoryBox.ReservoirAreaId,
                StoragerackId = (long)inventoryBox.StorageRackId,
                Data = null,
                OperaterDate = DateTime.Now,
                OperaterId = UserDto.UserId,
                OperaterUser = UserDto.UserName,
                Status = InventoryBoxTaskStatus.task_outing.ToByte()
            };
            
            if (!await SendWCSOutCommand(task, $"料箱编号:{inventoryBox.InventoryBoxNo}"))
            {
                return YL.Core.Dto.RouteData.From(PubMessages.E2300_WCS_OUTCOMMAND_FAIL);
            }

            if (await _sqlClient.Insertable(task).ExecuteCommandAsync() == 0)
            {
                return YL.Core.Dto.RouteData.From(PubMessages.E0005_DATABASE_INSERT_FAIL, "料箱:" + inventoryBox.InventoryBoxName);
            }

            inventoryBox.Status = InventoryBoxStatus.Outing;
            inventoryBox.ModifiedBy = UserDto.UserId;
            inventoryBox.ModifiedUser = UserDto.UserName;
            inventoryBox.ModifiedDate = DateTime.Now;
            if (await _sqlClient.Updateable(inventoryBox).ExecuteCommandAsync() == 0)
            {
                return YL.Core.Dto.RouteData.From(PubMessages.E0004_DATABASE_UPDATE_FAIL, "料箱:" + inventoryBox.InventoryBoxName);
            }

            return YL.Core.Dto.RouteData.From(PubMessages.I2300_WCS_OUTCOMMAND_SCCUESS, "料箱:" + inventoryBox.InventoryBoxName);
        }
         
        public async Task<RouteData> DoStockOutBoxOut(long[] stockOutIds)
        {
            foreach(long stockOutId in stockOutIds)
            {
                RouteData result = await DoStockOutBoxOut(stockOutId);
                if (!result.IsSccuess)
                {
                    return result;
                }
            }
            return new RouteData();
        }

        public async Task<RouteData> DoStockOutBoxOut(long stockOutId)
        {
            try
            {
                _sqlClient.Ado.BeginTran();
                RouteData result = await DoStockOutBoxOutCore( stockOutId);
                if (!result.IsSccuess)
                {
                    _sqlClient.Ado.RollbackTran();
                    return result;
                }
                _sqlClient.Ado.CommitTran();
                return result;
            }
            catch (Exception ex)
            {
                _sqlClient.Ado.RollbackTran();
                return YL.Core.Dto.RouteData.From(PubMessages.E2105_STOCKOUT_BOXOUT_FAIL, ex.Message);
            }

        }

        public async Task<RouteData> DoStockOutBoxOutCore(long stockOutId)
        {
            Wms_stockout stockout = await _sqlClient.Queryable<Wms_stockout>().FirstAsync(x => x.StockOutId == stockOutId);
            if (stockout == null) { return YL.Core.Dto.RouteData.From(PubMessages.E2113_STOCKOUT_NOTFOUND); }
            if (stockout.StockOutStatus == StockOutStatus.task_finish.ToByte()) { return YL.Core.Dto.RouteData.From(PubMessages.E2114_STOCKOUT_ALLOW_FINISHED); }

            if (stockout.StockOutStatus != StockOutStatus.task_working.ToByte())
            {
                stockout.StockOutStatus = StockOutStatus.task_working.ToByte();
                stockout.ModifiedBy = UserDto.UserId;
                stockout.ModifiedDate = DateTime.Now;
                if (_sqlClient.Updateable(stockout).ExecuteCommand() == 0)
                {
                    return YL.Core.Dto.RouteData.From(PubMessages.E2118_STOCKOUT_LOCK_FAIL, "出库单锁定状态更新失败");
                }
            }

            List<Wms_stockoutdetail> details = await _sqlClient.Queryable<Wms_stockoutdetail>().Where(x => x.StockOutId == stockout.StockOutId).ToListAsync();
            List<long> targetBoxIdList = new List<long>();
            foreach (Wms_stockoutdetail detail in details)
            {
                if (detail.Status == StockOutStatus.task_finish.ToByte()) continue;

                //TODO：尽量减少出库料箱数量
                //优先出库自身生产令号的物料
                Wms_inventory[] inventories = _sqlClient.Queryable<Wms_inventory>()
                    .Where(x => x.MaterialId == detail.MaterialId && (x.OrderNo == stockout.OrderNo || (string.IsNullOrEmpty(x.OrderNo) && !x.IsLocked)))
                    .OrderBy(x => x.OrderNo, OrderByType.Desc).ToArray();
                int outedQty = 0;
                int needQty = detail.PlanOutQty - detail.ActOutQty;
                foreach (Wms_inventory inventory in inventories)
                {
                    if (!targetBoxIdList.Contains(inventory.InventoryBoxId))
                    {
                        targetBoxIdList.Add(inventory.InventoryBoxId);
                    }
                    outedQty += inventory.Qty;
                    if (outedQty >= needQty)
                    {
                        //已锁定足够物料
                        break;
                    }
                }

            }
            int outCount = 0;
            foreach (long targetBoxId in targetBoxIdList)
            {
                RouteData result = await DoInventoryBoxOutCore( targetBoxId);
                if (!result.IsSccuess)
                {
                    if (result.Code != PubMessages.E1012_INVENTORYBOX_NOTINPOSITION.Code)
                    {
                        return result;
                    }
                }
                else
                {
                    outCount++;
                }
            }
            if (outCount == 0)
            {
                return YL.Core.Dto.RouteData.From(PubMessages.E2120_STOCKOUT_NOMORE_BOX);
            }
            return YL.Core.Dto.RouteData.From(PubMessages.I1001_BOXBACK_SCCUESS);

        }

        public async Task<RouteData<InventoryDetailDto[]>> InventoryInDetailList(long inventoryBoxTaskId)
        {
            var inventoryBoxTask = await _sqlClient.Queryable<Wms_inventoryboxTask, Wms_inventorybox>(
                (ibt, ib) => new object[] {
                   JoinType.Left,ibt.InventoryBoxId==ib.InventoryBoxId ,
                }
                )
                .Where((ibt, ib) => ibt.InventoryBoxTaskId == inventoryBoxTaskId)
                .Select((ibt, ib) => new {
                    ibt.InventoryBoxTaskId,
                    ibt.InventoryBoxId,
                    ib.InventoryBoxNo,
                    ib.InventoryBoxName,
                    ib.Size
                }).FirstAsync();

            if (inventoryBoxTask == null)
            {
                return YL.Core.Dto.RouteData<InventoryDetailDto[]>.From(PubMessages.E1013_INVENTORYBOXTASK_NOTFOUND);
            }

            var query = _sqlClient.Queryable<Wms_stockindetail_box, Wms_stockindetail, Wms_stockin, Wms_material>(
                (sidb, sid, si, m) => new object[] {
                   JoinType.Left,sidb.StockinDetailId==sid.StockInDetailId ,
                   JoinType.Left,sid.StockInId==si.StockInId ,
                   JoinType.Left,sid.MaterialId==m.MaterialId && m.IsDel == DeleteFlag.Normal
                  })
                 .Where((sidb, sid, si, m) => sidb.InventoryBoxTaskId == inventoryBoxTaskId)
                 .Select((sidb, sid, si, m) => new
                 {
                     Position = sidb.Position,
                     sid.StockInDetailId,
                     MaterialId = m.MaterialId,
                     m.MaterialNo,
                     m.MaterialName,
                     si.OrderNo,
                     PlanInQty = sid.PlanInQty,
                     ActInQty = sid.ActInQty,
                     Qty = sidb.Qty
                 }).MergeTable();

            var rawList = await query.ToListAsync();
            var inventoryList = _sqlClient.Queryable<Wms_inventory>().Where(x => x.InventoryBoxId == inventoryBoxTask.InventoryBoxId).ToArray();

            List<InventoryDetailDto> resultList = new List<InventoryDetailDto>();
            foreach (var inventory in inventoryList)
            {
                if (inventory.MaterialId == null) continue;
                var raw = rawList.FirstOrDefault(x => x.MaterialId == inventory.MaterialId);
                if (raw == null) continue;
                InventoryDetailDto newInventory = new InventoryDetailDto()
                {
                    InventoryPosition = inventory.Position,
                    MaterialId = raw.MaterialId.ToString(),
                    MaterialNo = raw.MaterialNo,
                    MaterialName = raw.MaterialName,
                    InventoryBoxId = inventoryBoxTask.InventoryBoxId.ToString(),
                    InventoryId = inventory.InventoryId.ToString(),
                    StockInDetailId = raw.StockInDetailId.ToString(),
                    StockOutDetailId = null,
                    OrderNo = raw.OrderNo,
                    BeforeQty = (int)inventory.Qty,
                    PlanQty = (int)raw.PlanInQty,
                    ComplateQty = (int)raw.ActInQty,
                    Qty = raw.Qty
                };
                resultList.Add(newInventory);
                rawList.Remove(raw);
            }
            int inventoryCount = inventoryList.Length;
            foreach (var raw in rawList)
            {
                var inventory = inventoryList.FirstOrDefault(x => x.MaterialId == null && string.IsNullOrEmpty(x.OrderNo) && !x.IsLocked);
                if (inventory == null)
                {
                    if (inventoryCount >= inventoryBoxTask.Size)
                    {
                        return YL.Core.Dto.RouteData<InventoryDetailDto[]>.From(PubMessages.E1010_INVENTORYBOX_BLOCK_OVERLOAD);
                    }
                }
                inventoryCount++;
                InventoryDetailDto newInventory = new InventoryDetailDto()
                {
                    InventoryPosition = inventoryCount,
                    MaterialId = raw.MaterialId.ToString(),
                    MaterialNo = raw.MaterialNo,
                    MaterialName = raw.MaterialName,
                    OrderNo = raw.OrderNo,
                    InventoryBoxId = inventoryBoxTask.InventoryBoxId.ToString(),
                    InventoryId = null,
                    StockInDetailId = raw.StockInDetailId.ToString(),
                    StockOutDetailId = null,
                    BeforeQty = 0,
                    PlanQty = (int)raw.PlanInQty,
                    ComplateQty = (int)raw.ActInQty,
                    Qty = raw.Qty
                };
                resultList.Add(newInventory);
            }


            return YL.Core.Dto.RouteData<InventoryDetailDto[]>.From(resultList.ToArray());
        }

        public async Task<RouteData<InventoryDetailDto[]>> InventoryOutDetailList(long inventoryBoxTaskId)
        {
            var inventoryBoxTask = await _sqlClient.Queryable<Wms_inventoryboxTask, Wms_inventorybox>(
               (ibt, ib) => new object[] {
                   JoinType.Left,ibt.InventoryBoxId==ib.InventoryBoxId ,
               }
               )
               .Where((ibt, ib) => ibt.InventoryBoxTaskId == inventoryBoxTaskId)
               .Select((ibt, ib) => new {
                   ibt.InventoryBoxTaskId,
                   ibt.InventoryBoxId,
                   ib.InventoryBoxNo,
                   ib.InventoryBoxName,
                   ib.Size
               }).FirstAsync();

            if (inventoryBoxTask == null)
            {
                return YL.Core.Dto.RouteData<InventoryDetailDto[]>.From(PubMessages.E1013_INVENTORYBOXTASK_NOTFOUND);
            }

            var query = _sqlClient.Queryable<Wms_stockoutdetail_box, Wms_stockoutdetail, Wms_stockout, Wms_material>(
                (sidb, sid, so, m) => new object[] {
                   JoinType.Left,sidb.StockOutDetailId==sid.StockOutDetailId ,
                   JoinType.Left,sid.StockOutId==so.StockOutId ,
                   JoinType.Left,sid.MaterialId==m.MaterialId && m.IsDel == DeleteFlag.Normal
                  })
                 .Where((sidb, sid, so, m) => sidb.InventoryBoxTaskId == inventoryBoxTaskId)
                 .Select((sidb, sid, so, m) => new
                 {
                     sid.StockOutDetailId,
                     MaterialId = m.MaterialId,
                     m.MaterialNo,
                     m.MaterialName,
                     so.OrderNo,
                     PlanOutQty = sid.PlanOutQty,
                     ActOutQty = sid.ActOutQty,
                     Qty = sidb.Qty
                 }).MergeTable();

            var rawList = await query.ToListAsync();
            var inventoryList = _sqlClient.Queryable<Wms_inventory>().Where(x => x.InventoryBoxId == inventoryBoxTask.InventoryBoxId).ToArray();

            List<InventoryDetailDto> resultList = new List<InventoryDetailDto>();
            foreach (var inventory in inventoryList)
            {
                if (inventory.MaterialId == null) continue;
                var raw = rawList.FirstOrDefault(x => x.MaterialId == inventory.MaterialId);
                if (raw == null) continue;
                InventoryDetailDto newInventory = new InventoryDetailDto()
                {
                    InventoryPosition = inventory.Position,
                    MaterialId = raw.MaterialId.ToString(),
                    MaterialNo = raw.MaterialNo,
                    MaterialName = raw.MaterialName,
                    InventoryBoxId = inventoryBoxTask.InventoryBoxId.ToString(),
                    InventoryId = inventory.InventoryId.ToString(),
                    StockInDetailId = null,
                    StockOutDetailId = raw.StockOutDetailId.ToString(),
                    OrderNo = raw.OrderNo,
                    BeforeQty = (int)inventory.Qty,
                    PlanQty = (int)raw.PlanOutQty,
                    ComplateQty = (int)raw.ActOutQty,
                    Qty = raw.Qty
                };
                resultList.Add(newInventory);
                rawList.Remove(raw);
            }
            int inventoryCount = inventoryList.Length;
            foreach (var raw in rawList)
            {
                var inventory = inventoryList.FirstOrDefault(x => x.MaterialId == null);
                if (inventory == null)
                {
                    if (inventoryCount >= inventoryBoxTask.Size)
                    {
                        return YL.Core.Dto.RouteData<InventoryDetailDto[]>.From(PubMessages.E1010_INVENTORYBOX_BLOCK_OVERLOAD);
                    }
                }
                inventoryCount++;
                InventoryDetailDto newInventory = new InventoryDetailDto()
                {
                    InventoryPosition = inventoryCount,
                    MaterialId = raw.MaterialId.ToString(),
                    MaterialNo = raw.MaterialNo,
                    MaterialName = raw.MaterialName,
                    InventoryBoxId = inventoryBoxTask.InventoryBoxId.ToString(),
                    InventoryId = raw.StockOutDetailId.ToString(),
                    StockInDetailId = null,
                    StockOutDetailId = null,
                    BeforeQty = 0,
                    PlanQty = (int)raw.PlanOutQty,
                    ComplateQty = (int)raw.ActOutQty,
                    Qty = raw.Qty
                };
                resultList.Add(newInventory);
            }


            return YL.Core.Dto.RouteData<InventoryDetailDto[]>.From(resultList.ToArray());
        }

        /// <summary>
        /// 料箱归库
        /// </summary>
        /// <param name="mode">1:入库 2:出库 3:人工修正 4:离库</param>
        /// <param name="inventoryBoxTaskId"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        public async Task<RouteData> DoInventoryBoxBack(int mode, long inventoryBoxTaskId, InventoryDetailDto[] details)
        {
            try
            {
                Wms_inventoryboxTask task = await _sqlClient.Queryable<Wms_inventoryboxTask>().FirstAsync(x => x.InventoryBoxTaskId == inventoryBoxTaskId);
                if (task == null) { return YL.Core.Dto.RouteData.From(PubMessages.E1013_INVENTORYBOXTASK_NOTFOUND); }
                if (task.Status < InventoryBoxTaskStatus.task_outed.ToByte()) { return YL.Core.Dto.RouteData.From(PubMessages.E1016_INVENTORYBOX_NOTOUTED); }
                if (task.Status == InventoryBoxTaskStatus.task_backing.ToByte()) { return YL.Core.Dto.RouteData.From(PubMessages.E1017_INVENTORYBOX_ALLOW_BACKING); }
                if (task.Status == InventoryBoxTaskStatus.task_backed.ToByte()) { return YL.Core.Dto.RouteData.From(PubMessages.E1018_INVENTORYBOX_ALLOW_BACKED); }

                Wms_inventorybox inventoryBox = await _sqlClient.Queryable<Wms_inventorybox>().FirstAsync(x => x.InventoryBoxId == task.InventoryBoxId);
                if (inventoryBox == null) { return YL.Core.Dto.RouteData.From(PubMessages.E1011_INVENTORYBOX_NOTFOUND); }
                if (inventoryBox.Status != InventoryBoxStatus.Outed) { return YL.Core.Dto.RouteData.From(PubMessages.E1014_INVENTORYBOX_NOTOUTED); }

                List<Wms_inventory> inventories = await _sqlClient.Queryable<Wms_inventory>().Where(x => x.InventoryBoxId == task.InventoryBoxId).ToListAsync();
                List<Wms_inventory> updatedInventories = new List<Wms_inventory>();

                List<Wms_stockindetail> stockindetails = mode != 3 ? await GetStockInDetails(inventoryBoxTaskId) : new List<Wms_stockindetail>();
                List<Wms_stockindetail> updatedStockindetails = new List<Wms_stockindetail>();
                List<Wms_stockin> stockins = stockindetails.Count() == 0 ? new List<Wms_stockin>() : await _sqlClient.Queryable<Wms_stockin>().Where(x => stockindetails.Select(y => y.StockInId).Contains(x.StockInId)).ToListAsync();

                List<Wms_stockoutdetail> stockoutdetails = mode != 3 ? await GetStockOutDetails(inventoryBoxTaskId) : new List<Wms_stockoutdetail>();
                List<Wms_stockoutdetail> updatedStockoutdetails = new List<Wms_stockoutdetail>();
                List<Wms_stockout> stockouts = stockoutdetails.Count() == 0 ? new List<Wms_stockout>() : await _sqlClient.Queryable<Wms_stockout>().Where(x => stockoutdetails.Select(y => y.StockOutId).Contains(x.StockOutId)).ToListAsync();

                _sqlClient.Ado.BeginTran();

                List<Wms_stockin> relationStockins = new List<Wms_stockin>();
                List<Wms_stockout> relationStockouts = new List<Wms_stockout>();
                foreach (InventoryDetailDto detail in details)
                {
                    Wms_stockindetail stockindetail = string.IsNullOrEmpty(detail.StockInDetailId) ? null : stockindetails.FirstOrDefault(x => x.StockInDetailId == detail.StockInDetailId.ToInt64());
                    Wms_stockoutdetail stockoutdetail = string.IsNullOrEmpty(detail.StockOutDetailId) ? null : stockoutdetails.FirstOrDefault(x => x.StockOutDetailId == detail.StockOutDetailId.ToInt64());
                    Wms_stockin stockin = stockindetail == null ? null : stockins.FirstOrDefault(x => x.StockInId == stockindetail.StockInId);
                    Wms_stockout stockout = stockoutdetail == null ? null : stockouts.FirstOrDefault(x => x.StockOutId == stockoutdetail.StockOutId);

                    if (mode == 1 && stockindetail == null)
                    {
                        _sqlClient.Ado.RollbackTran();
                        return YL.Core.Dto.RouteData.From(PubMessages.E2016_STOCKINDETAIL_INVENTORYBOXTASK_NOTMATCH);
                    }
                    else if (mode == 2 && stockoutdetail == null)
                    {
                        _sqlClient.Ado.RollbackTran();
                        return YL.Core.Dto.RouteData.From(PubMessages.E2116_STOCKOUTDETAIL_INVENTORYBOXTASK_NOTMATCH);
                    }
                    int? beforeQty = null;
                    int qty = 0;
                    int afterQty = 0;
                    Wms_inventory inventory = string.IsNullOrEmpty(detail.InventoryId) ? null : inventories.FirstOrDefault(x => x.InventoryId == detail.InventoryId.ToInt64());
                    if (inventory == null)
                    {
                        beforeQty = 0;
                        inventory = new Wms_inventory()
                        {
                            InventoryId = PubId.SnowflakeId,
                            Position = detail.InventoryPosition,
                            InventoryBoxId = inventoryBox.InventoryBoxId,
                            MaterialId = detail.MaterialId.ToInt64(),
                            MaterialNo = detail.MaterialNo,
                            MaterialOnlyId = detail.MaterialOnlyId,
                            MaterialName = detail.MaterialName,
                            OrderNo = detail.OrderNo,
                            Qty = 0,
                            IsLocked = false,
                            IsDel = (byte)DeleteFlag.Normal
                        };
                        inventories.Add(inventory);
                    }
                    else
                    {
                        beforeQty = inventory.Qty;
                    }
                    if (inventory.MaterialId.ToString() != detail.MaterialId)
                    {
                        _sqlClient.Ado.RollbackTran();
                        return YL.Core.Dto.RouteData.From(PubMessages.E1015_INVENTORYBOX_MATERIAL_NOTMATCH);
                    }
                    if (inventory.IsLocked && detail.OrderNo != inventory.OrderNo)
                    {
                        _sqlClient.Ado.RollbackTran();
                        return YL.Core.Dto.RouteData.From(PubMessages.E1019_INVENTORY_LOCKED, $"料箱编号{inventoryBox.InventoryBoxNo},物料编号{detail.MaterialNo}");
                    }
                    if (mode == 1)
                    {
                        inventory.Qty += detail.Qty; //加上入库数量
                        qty = detail.Qty;
                    }
                    else if (mode == 2)
                    {
                        inventory.Qty -= detail.Qty; //减去出库数量
                        qty -= detail.Qty;
                    }
                    else
                    {
                        qty = detail.Qty - inventory.Qty;
                        inventory.Qty = detail.Qty; //手动出入库，直接填入最终数量
                    }
                    afterQty = inventory.Qty;
                    inventory.ModifiedDate = DateTime.Now;
                    inventory.ModifiedBy = this.UserDto.UserId;
                    inventory.ModifiedUser = this.UserDto.UserName;

                    updatedInventories.Add(inventory);
                    string stockNo = null;
                    if (stockindetail != null)
                    {
                        stockNo = stockin.StockInNo;
                        stockindetail.ActInQty += detail.Qty;
                        stockindetail.ModifiedDate = DateTime.Now;
                        stockindetail.ModifiedBy = this.UserDto.UserId;
                        if (stockindetail.ActInQty >= stockindetail.PlanInQty)
                        {
                            stockindetail.Status = StockInStatus.task_finish.ToByte();
                        }
                        updatedStockindetails.Add(stockindetail);
                        if (!relationStockins.Any(x => x.StockInId == stockindetail.StockInId))
                        { 
                            relationStockins.Add(stockin);
                        }
                    }

                    if (stockoutdetail != null)
                    {
                        stockNo = stockout.StockOutNo;
                        stockoutdetail.ActOutQty += detail.Qty;
                        stockoutdetail.ModifiedDate = DateTime.Now;
                        stockoutdetail.ModifiedBy = this.UserDto.UserId;
                        if (stockoutdetail.ActOutQty >= stockoutdetail.PlanOutQty)
                        {
                            stockoutdetail.Status = StockOutStatus.task_finish.ToByte();
                        }
                        updatedStockoutdetails.Add(stockoutdetail);
                    }

                    Wms_inventoryrecord record = new Wms_inventoryrecord()
                    {
                        InventoryrecordId = PubId.SnowflakeId, 
                        StockInDetailId = stockindetail == null ? (long?)null : stockindetail.StockInDetailId,
                        StockOutDetailId = stockoutdetail == null ? (long?)null : stockoutdetail.StockOutDetailId,
                        StockNo = stockNo,
                        InventoryBoxId = inventoryBox.InventoryBoxId,
                        InventoryBoxNo = inventoryBox.InventoryBoxNo,
                        InventoryId = inventory.InventoryId,
                        InventoryPosition = inventory.Position,
                        MaterialId = detail.MaterialId.ToInt64(),
                        MaterialNo = detail.MaterialNo,
                        MaterialOnlyId = detail.MaterialOnlyId,
                        MaterialName = detail.MaterialName,
                        BeforeQty = beforeQty,
                        Qty = qty,
                        AfterQty = afterQty,
                        CreateBy = this.UserDto.UserId,
                        CreateDate = DateTime.Now,
                        CreateUser = this.UserDto.UserName,
                        IsDel = DeleteFlag.Normal
                    };
                    if (_sqlClient.Insertable(record).ExecuteCommand() == 0)
                    {
                        _sqlClient.Ado.RollbackTran();
                        return YL.Core.Dto.RouteData.From(PubMessages.E1021_INVENTORYRECORD_FAIL);
                    }
                }
                //离库的情况下不发WCS命令
                if (mode != 4)
                {
                    if (!await SendWCSBackCommand(task,$"料箱编号:{inventoryBox.InventoryBoxNo}"))
                    {
                        return YL.Core.Dto.RouteData.From(PubMessages.E2310_WCS_BACKCOMMAND_FAIL);
                    }
                }

                //更新库存
                foreach (Wms_inventory inventoriy in updatedInventories)
                {
                    if (inventoriy.CreateBy == null)
                    {
                        inventoriy.CreateBy = UserDto.UserId;
                        inventoriy.CreateDate = DateTime.Now;
                        inventoriy.CreateUser = UserDto.UserName;
                        if (_sqlClient.Insertable(inventoriy).ExecuteCommand() == 0)
                        {
                            _sqlClient.Ado.RollbackTran();
                            return YL.Core.Dto.RouteData.From(PubMessages.E0005_DATABASE_INSERT_FAIL);
                        }
                    }
                    else
                    {
                        inventoriy.ModifiedBy = UserDto.UserId;
                        inventoriy.ModifiedDate = DateTime.Now;
                        inventoriy.ModifiedUser = UserDto.UserName;
                        if (_sqlClient.Updateable(inventoriy).ExecuteCommand() == 0)
                        {
                            _sqlClient.Ado.RollbackTran();
                            return YL.Core.Dto.RouteData.From(PubMessages.E0004_DATABASE_UPDATE_FAIL);
                        }
                    }
                }
                //更新入库详细
                foreach (Wms_stockindetail stockindetail in updatedStockindetails)
                {
                    if (_sqlClient.Updateable(stockindetail).ExecuteCommand() == 0)
                    {
                        _sqlClient.Ado.RollbackTran();
                        return YL.Core.Dto.RouteData.From(PubMessages.E0002_UPDATE_COUNT_FAIL);

                    }
                }
                //更新出库详细
                foreach (Wms_stockoutdetail stockoutdetial in updatedStockoutdetails)
                {
                    if (_sqlClient.Updateable(stockoutdetial).ExecuteCommand() == 0)
                    {
                        _sqlClient.Ado.RollbackTran();
                        return YL.Core.Dto.RouteData.From(PubMessages.E0002_UPDATE_COUNT_FAIL);

                    }
                }
                task.Status = mode == 4 ? InventoryBoxTaskStatus.task_leaved.ToByte() : InventoryBoxTaskStatus.task_backing.ToByte();
                task.OperaterId = UserDto.UserId;
                task.OperaterDate = DateTime.Now; 
                if (_sqlClient.Updateable(task).ExecuteCommand() == 0)
                {
                    _sqlClient.Ado.RollbackTran();
                    return YL.Core.Dto.RouteData.From(PubMessages.E0004_DATABASE_UPDATE_FAIL);
                }
                if (mode == 4)
                {
                    inventoryBox.StorageRackId = null;
                    inventoryBox.StorageRackName = "";
                    inventoryBox.Status = InventoryBoxStatus.None;
                }
                else
                {
                    inventoryBox.Status = InventoryBoxStatus.Backing;

                }
                inventoryBox.UsedSize = inventories.Count;
                inventoryBox.ModifiedBy = UserDto.UserId;
                inventoryBox.ModifiedDate = DateTime.Now;
                if(_sqlClient.Updateable(inventoryBox).ExecuteCommand() == 0)
                {
                    return YL.Core.Dto.RouteData.From(PubMessages.E0004_DATABASE_UPDATE_FAIL); 
                }


                foreach (Wms_stockin stockin in relationStockins)
                {
                    if (_sqlClient.Queryable<Wms_stockindetail>().Any(x => x.Status != StockInStatus.task_finish.ToByte()))
                    {
                        //尚有未入库任务
                    }
                    else
                    {
                        stockin.StockInStatus = StockInStatus.task_finish.ToByte();
                        stockin.ModifiedBy = UserDto.UserId;
                        stockin.ModifiedDate = DateTime.Now;
                        if (_sqlClient.Updateable(stockin).ExecuteCommand() == 0)
                        {
                            _sqlClient.Ado.RollbackTran();
                            return YL.Core.Dto.RouteData.From(PubMessages.E0002_UPDATE_COUNT_FAIL);
                        }
                    }
                }

                _sqlClient.Ado.CommitTran();
                return YL.Core.Dto.RouteData.From(PubMessages.I2000_STOCKOUT_SCAN_SCCUESS);
            }
            catch (Exception)
            {
                _sqlClient.Ado.RollbackTran();
                return YL.Core.Dto.RouteData.From(PubMessages.E2005_STOCKIN_BOXOUT_FAIL);
            }
        }


        private async Task<List<Wms_stockindetail>> GetStockInDetails(long inventoryBoxTaskId)
        {
            return await _sqlClient.Queryable<Wms_stockindetail, Wms_stockindetail_box>(
                (sid, sidb) => new object[] {
                    JoinType.Left,sid.StockInDetailId == sidb.StockinDetailId,
                })
                .Where((sid, sidb) => sidb.InventoryBoxTaskId == inventoryBoxTaskId)
                .ToListAsync();
        }
        private async Task<List<Wms_stockoutdetail>> GetStockOutDetails(long inventoryBoxTaskId)
        {
            return await _sqlClient.Queryable<Wms_stockoutdetail, Wms_stockoutdetail_box>(
                (sid, sidb) => new object[] {
                JoinType.Left,sid.StockOutDetailId == sidb.StockOutDetailId,
                })
                .Where((sid, sidb) => sidb.InventoryBoxTaskId == inventoryBoxTaskId)
                .ToListAsync();
        }

        public async Task<RouteData<Wms_wcstask[]>> GetWCSTaskList(bool failOnly,int pageIndex, int pageSize, string search, string[] order, string datemin, string datemax)
        {
            ISugarQueryable<Wms_wcstask> query = _sqlClient.Queryable<Wms_wcstask>();
            if (failOnly)
            {
                query = query.Where(x => x.WorkStatus == WCSTaskWorkStatus.Failed || x.NotifyStatus == WCSTaskNotifyStatus.Failed);
            }
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x => x.InventoryBoxNo.Contains(search) );
            }
            DateTime minDate;
            if (!string.IsNullOrWhiteSpace(datemin) && DateTime.TryParse(datemin, out minDate))
            {
                query = query.Where(x => x.RequestDate >= minDate);
            }
            DateTime maxDate;
            if (!string.IsNullOrWhiteSpace(datemax) && DateTime.TryParse(datemax, out maxDate))
            {
                query = query.Where(x => x.RequestDate <= maxDate);
            }
            if(order== null || order.Length == 0)
            {
                query = query.OrderBy(x => x.RequestDate, OrderByType.Desc);
            }
            else
            {
                query = query.OrderBy(string.Join(",",order)); 
            }
            RefAsync<int> totalNumber = new RefAsync<int>();
            List<Wms_wcstask> result = await query.ToPageListAsync(pageIndex, pageSize,totalNumber);
            return RouteData<Wms_wcstask[]>.From(result.ToArray(), totalNumber);
        }

        /// <summary>
        /// 手工设置WCS指令状态
        /// </summary>
        /// <param name="wcsTaskId"></param>
        /// <param name="isSccuess"></param>
        /// <returns></returns>
        public async Task<RouteData> SetWCSTaskStatus(long wcsTaskId, bool isSccuess)
        {
            try
            {
                _sqlClient.Ado.BeginTran();
                RouteData result = await SetWCSTaskStatusCore(wcsTaskId, isSccuess);
                if (!result.IsSccuess)
                {
                    _sqlClient.Ado.RollbackTran();
                }
                else
                {
                    _sqlClient.Ado.CommitTran();
                }
                return result;
            }
            catch(Exception ex)
            {
                _sqlClient.Ado.RollbackTran();
                return RouteData.From(PubMessages.E2307_WCS_TASKSTATUS_UPDATE_FAIL,ex.Message);
            }
        }

        public async Task<RouteData> SetWCSTaskStatusCore(long wcsTaskId, bool isSccuess)
        {
            Wms_wcstask wcsTask = await _sqlClient.Queryable<Wms_wcstask>().FirstAsync(x => x.WcsTaskId == wcsTaskId);
            if (wcsTask == null)
            {
                return RouteData.From(PubMessages.E2302_WCS_TASKID_INVAILD);
            }
            if (wcsTask.WorkStatus == WCSTaskWorkStatus.WorkComplated)
            {
                return RouteData.From(PubMessages.E2306_WCS_TASK_ALLOW_COMPLATED);
            }
            wcsTask.WorkStatus = isSccuess ? WCSTaskWorkStatus.WorkComplated : WCSTaskWorkStatus.Failed;
            wcsTask.NotifyStatus = WCSTaskNotifyStatus.ManualResponsed;
            wcsTask.ResponseDate = DateTime.Now;
            wcsTask.ResponseUserId = UserDto.UserId;
            wcsTask.ResponseUser = UserDto.UserName;
            
            if (await _sqlClient.Updateable(wcsTask).ExecuteCommandAsync() == 0)
            {
                return RouteData.From(PubMessages.E2307_WCS_TASKSTATUS_UPDATE_FAIL);
            }
            if (isSccuess)
            {
                RouteData result = null;
                if (wcsTask.TaskType == WCSTaskTypes.StockOut)
                {
                    result = await ConfirmOutStockCore(wcsTask.WcsTaskId,true);
                }
                else if (wcsTask.TaskType == WCSTaskTypes.StockBack)
                {
                    result = await ConfirmBackStockCore(wcsTask.WcsTaskId,true);
                }
                else
                {
                    throw new NotSupportedException($"WCSTaskType={wcsTask.TaskType}");
                }
                if (!result.IsSccuess)
                {
                    return result;
                }
            }
            
            return RouteData.From(PubMessages.I2301_WCS_TASKSTATUS_UPDATE_SCCUESS);
        }

        /// <summary>
        /// 重发Wcs任务指令
        /// </summary>
        /// <param name="wcsTaskId"></param>
        /// <returns></returns>
        public async Task<RouteData> RepeatWCSTaskStatus(long wcsTaskId)
        {
            Wms_wcstask wcsTask = await _sqlClient.Queryable<Wms_wcstask>().FirstAsync(x => x.WcsTaskId == wcsTaskId);
            if (wcsTask == null)
            {
                return RouteData.From(PubMessages.E2302_WCS_TASKID_INVAILD);
            }
            if (wcsTask.WorkStatus == WCSTaskWorkStatus.WorkComplated)
            {
                return RouteData.From(PubMessages.E2306_WCS_TASK_ALLOW_COMPLATED);
            }
            wcsTask.WorkStatus = WCSTaskWorkStatus.Working;
            wcsTask.NotifyStatus = WCSTaskNotifyStatus.WaitResponse; 
            wcsTask.RequestDate = DateTime.Now;
            wcsTask.RequestUserId = UserDto.UserId;
            wcsTask.RequestUser = UserDto.UserName;

            if (await _sqlClient.Updateable(wcsTask).ExecuteCommandAsync() == 0)
            {
                return RouteData.From(PubMessages.E0004_DATABASE_UPDATE_FAIL, "WCS指令状态更新失败");
            }

            if(wcsTask.TaskType == WCSTaskTypes.StockOut)
            {
                OutStockInfo outStockInfo = JsonConvert.DeserializeObject<OutStockInfo>(wcsTask.Params);
                CreateOutStockResult result = await WCSApiAccessor.Instance.CreateOutStockTask(outStockInfo);
                if (!result.Successd)
                {
                    return RouteData.From(PubMessages.E2300_WCS_OUTCOMMAND_FAIL); 
                }
                else
                {
                    return RouteData.From(PubMessages.I2300_WCS_OUTCOMMAND_SCCUESS); 
                }
            }
            else if (wcsTask.TaskType == WCSTaskTypes.StockOut)
            {
                BackStockInfo backStockInfo = JsonConvert.DeserializeObject<BackStockInfo>(wcsTask.Params);
                CreateBackStockResult result = await WCSApiAccessor.Instance.CreateBackStockTask(backStockInfo);
                if (!result.Successd)
                {
                    return RouteData.From(PubMessages.E2310_WCS_BACKCOMMAND_FAIL);
                }
                else
                { 
                    return RouteData.From(PubMessages.I2300_WCS_BACKCOMMAND_SCCUESS);
                }
            }
            return new RouteData();
        }

        private async Task<bool> SendWCSOutCommand(Wms_inventoryboxTask task,string desc)
        {
            try
            {
                Wms_storagerack storagerack = _sqlClient.Queryable<Wms_storagerack>().First(x => x.StorageRackId == task.StoragerackId);

                long taskId = PubId.SnowflakeId;
                OutStockInfo outStockInfo = new OutStockInfo()
                {
                    TaskId = taskId.ToString(),
                    GetColumn = storagerack.Column.ToString(),
                    GetRow = storagerack.Row.ToString(),
                    GetFloor = storagerack.Floor.ToString()
                };
                 
                CreateOutStockResult result = await WCSApiAccessor.Instance.CreateOutStockTask(outStockInfo);
                if (result.Successd)
                {
                    Wms_wcstask wcsTask = new Wms_wcstask()
                    {
                        WcsTaskId = taskId,
                        TaskType = WCSTaskTypes.StockOut,
                        InventoryBoxId = task.InventoryBoxId,
                        InventoryBoxNo = task.InventoryBoxNo,
                        InventoryBoxTaskId = task.InventoryBoxTaskId,
                        Desc = $"出库指令({desc})",
                        WorkStatus = WCSTaskWorkStatus.Working,
                        NotifyStatus = WCSTaskNotifyStatus.WaitResponse,
                        Params = JsonConvert.SerializeObject(outStockInfo),
                        RequestDate = DateTime.Now
                    };
                    await _sqlClient.Insertable(wcsTask).ExecuteCommandAsync();
                }
                return result.Successd;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<ConfirmOutStockResult> ConfirmOutStock(WCSTaskResult result)
        {
            long taskId = result.TaskId.ToInt64();
            try
            {
                _sqlClient.Ado.BeginTran();
                RouteData confirmResult = await ConfirmOutStockCore(taskId);
                if (!confirmResult.IsSccuess)
                {
                    _sqlClient.Ado.RollbackTran();
                    return new ConfirmOutStockResult
                    {
                        Successd = false,
                        Code = "-1",
                        ErrorDesc = confirmResult.Message

                    };
                }
                _sqlClient.Ado.CommitTran();
                return new ConfirmOutStockResult();
            }
            catch (Exception ex)
            {
                _sqlClient.Ado.RollbackTran();
                return new ConfirmOutStockResult()
                {
                    Successd = false,
                    Code = "-1",
                    ErrorDesc = ex.Message
                };
            }
        }

        protected async Task<RouteData> ConfirmOutStockCore(long taskId,bool isManual = false)
        {
            if (taskId == 0)
            {
                return RouteData.From(PubMessages.E2302_WCS_TASKID_INVAILD);
            }
            Wms_wcstask wcsTask = await _sqlClient.Queryable<Wms_wcstask>().FirstAsync(x => x.WcsTaskId == taskId);
            if (wcsTask == null)
            {
                return RouteData.From(PubMessages.E2305_WCS_TASKID_NOTFOUND);
            }
            Wms_inventoryboxTask boxTask = await _sqlClient.Queryable<Wms_inventoryboxTask>().FirstAsync(x => x.InventoryBoxTaskId == wcsTask.InventoryBoxTaskId);
            if (boxTask == null)
            {
                return RouteData.From(PubMessages.E2303_WCS_STOCKOUTTASK_NOTFOUND);
            }
            if (boxTask.Status != InventoryBoxTaskStatus.task_outing.ToByte() && boxTask.Status != InventoryBoxTaskStatus.task_outed.ToByte())
            {
                return RouteData.From(PubMessages.E2304_WCS_STOCKOUTTASK_NOTOUT);
            }

            Wms_inventorybox box = await _sqlClient.Queryable<Wms_inventorybox>().FirstAsync(x => x.InventoryBoxId == boxTask.InventoryBoxId);
            if (box == null)
            {
                return RouteData.From(PubMessages.E1011_INVENTORYBOX_NOTFOUND);
            }
            if (box.Status != InventoryBoxStatus.Outing && box.Status != InventoryBoxStatus.Outed)
            {
                return RouteData.From(PubMessages.E1008_INVENTORYBOX_NOTOUT);
            }


            wcsTask.WorkStatus = WCSTaskWorkStatus.WorkComplated;
            wcsTask.NotifyStatus = isManual ? WCSTaskNotifyStatus.ManualResponsed : WCSTaskNotifyStatus.Responsed;
            wcsTask.ResponseDate = DateTime.Now;
            if (await _sqlClient.Updateable(wcsTask).ExecuteCommandAsync() == 0)
            {
                return RouteData.From(PubMessages.E0004_DATABASE_UPDATE_FAIL);
            }

            boxTask.Status = InventoryBoxTaskStatus.task_outed.ToByte();
            boxTask.ModifiedBy = PubConst.InterfaceUserId;
            boxTask.ModifiedDate = DateTime.Now;
            if (await _sqlClient.Updateable(boxTask).ExecuteCommandAsync() == 0)
            {
                return RouteData.From(PubMessages.E0004_DATABASE_UPDATE_FAIL);
            }

            box.Status = InventoryBoxStatus.Outed;
            box.ModifiedBy = PubConst.InterfaceUserId;
            box.ModifiedDate = DateTime.Now;
            if (await _sqlClient.Updateable(box).ExecuteCommandAsync() == 0)
            {
                return RouteData.From(PubMessages.E0004_DATABASE_UPDATE_FAIL);
            }
            return new RouteData();
        }

        private async Task<bool> SendWCSBackCommand(Wms_inventoryboxTask task, string desc)
        {
            try
            {
                long taskId = PubId.SnowflakeId;
                Wms_storagerack storagerack = await _sqlClient.Queryable<Wms_storagerack>().FirstAsync(x => x.StorageRackId == task.StoragerackId);

                BackStockInfo backStockInfo = new BackStockInfo()
                {
                    TaskId = task.InventoryBoxTaskId.ToString(),
                    GetColumn = storagerack.Column.ToString(),
                    GetRow = storagerack.Row.ToString(),
                    GetFloor = storagerack.Floor.ToString()
                };
                CreateBackStockResult result = await WCSApiAccessor.Instance.CreateBackStockTask(backStockInfo);
                if (result.Successd)
                {
                    Wms_wcstask wcsTask = new Wms_wcstask()
                    {
                        WcsTaskId = taskId,
                        TaskType = WCSTaskTypes.StockBack,
                        InventoryBoxId = task.InventoryBoxId,
                        InventoryBoxNo = task.InventoryBoxNo,
                        InventoryBoxTaskId = task.InventoryBoxTaskId,
                        Desc = $"入库指令({desc})",
                        WorkStatus = WCSTaskWorkStatus.Working,
                        NotifyStatus = WCSTaskNotifyStatus.WaitResponse,
                        Params = JsonConvert.SerializeObject(backStockInfo),
                        RequestDate = DateTime.Now
                    };
                    await _sqlClient.Insertable(wcsTask).ExecuteCommandAsync();
                }
                return result.Successd;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 归库完成
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public async Task<ConfirmBackStockResult> ConfirmBackStock(WCSTaskResult result)
        {
            long taskId = result.TaskId.ToInt64();
            try
            {
                _sqlClient.Ado.BeginTran();
                RouteData confirmResult = await ConfirmBackStockCore(taskId);
                if (!confirmResult.IsSccuess)
                {
                    _sqlClient.Ado.RollbackTran();
                    return new ConfirmBackStockResult
                    {
                        Successd = false,
                        Code = "-1",
                        ErrorDesc = confirmResult.Message

                    };
                }
                _sqlClient.Ado.CommitTran();
                return new ConfirmBackStockResult();
            }
            catch (Exception ex)
            {
                _sqlClient.Ado.RollbackTran();
                return new ConfirmBackStockResult()
                {
                    Successd = false,
                    Code = "-1",
                    ErrorDesc = ex.Message
                };
            }
        }

        protected async Task<RouteData> ConfirmBackStockCore(long taskId, bool isManual = false)
        {
            try
            {
                if (taskId == 0)
                {
                    return RouteData.From(PubMessages.E2302_WCS_TASKID_INVAILD);
                }
                Wms_wcstask wcsTask = await _sqlClient.Queryable<Wms_wcstask>().FirstAsync(x => x.WcsTaskId == taskId);
                if (wcsTask == null)
                {
                    return RouteData.From(PubMessages.E2305_WCS_TASKID_NOTFOUND);
                }
                Wms_inventoryboxTask boxTask = await _sqlClient.Queryable<Wms_inventoryboxTask>().FirstAsync(x => x.InventoryBoxTaskId == wcsTask.InventoryBoxTaskId);
                if (boxTask == null)
                {
                    return RouteData.From(PubMessages.E2311_WCS_STOCKBACKTASK_NOTFOUND);
                }
                if (boxTask.Status != InventoryBoxTaskStatus.task_backing.ToByte() && boxTask.Status != InventoryBoxTaskStatus.task_backed.ToByte())
                {
                    return RouteData.From(PubMessages.E2312_WCS_STOCKBACKTASK_NOTBACK);
                }

                Wms_inventorybox box = await _sqlClient.Queryable<Wms_inventorybox>().FirstAsync(x => x.InventoryBoxId == boxTask.InventoryBoxId);
                if (box == null)
                {
                    return RouteData.From(PubMessages.E1011_INVENTORYBOX_NOTFOUND);
                }
                if (box.Status != InventoryBoxStatus.Backing && box.Status != InventoryBoxStatus.InPosition)
                {
                    return RouteData.From(PubMessages.E1008_INVENTORYBOX_NOTOUT);
                }

                _sqlClient.Ado.BeginTran();

                wcsTask.WorkStatus = WCSTaskWorkStatus.WorkComplated;
                wcsTask.NotifyStatus = WCSTaskNotifyStatus.Responsed;
                wcsTask.ResponseDate = DateTime.Now;
                if (await _sqlClient.Updateable(wcsTask).ExecuteCommandAsync() == 0)
                {
                    return RouteData.From(PubMessages.E0004_DATABASE_UPDATE_FAIL);
                }

                boxTask.Status = InventoryBoxTaskStatus.task_backed.ToByte();
                boxTask.ModifiedBy = PubConst.InterfaceUserId;
                boxTask.ModifiedDate = DateTime.Now;
                if (await _sqlClient.Updateable(boxTask).ExecuteCommandAsync() == 0)
                {
                    return RouteData.From(PubMessages.E0004_DATABASE_UPDATE_FAIL);
                }

                box.Status = InventoryBoxStatus.InPosition;
                box.ModifiedBy = PubConst.InterfaceUserId;
                box.ModifiedDate = DateTime.Now;
                if (await _sqlClient.Updateable(box).ExecuteCommandAsync() == 0)
                {
                    return RouteData.From(PubMessages.E0004_DATABASE_UPDATE_FAIL);
                }

                _sqlClient.Ado.CommitTran();
                return RouteData.From();
            }
            catch (Exception ex)
            {
                _sqlClient.Ado.RollbackTran();
                return RouteData.From(-1, ex.Message);
            }

        }


        /// <summary>
        /// 入库扫描完成处理
        /// </summary>
        /// <param name="stockInId"></param>
        /// <param name="inventoryBoxId"></param>
        /// <param name="materials"></param>
        /// <param name="remark"></param>
        /// <returns></returns>  
        public async Task<RouteData> DoStockInScanComplate(long stockInId, long inventoryBoxId, Wms_StockMaterialDetailDto[] materials, string remark)
        {
            try
            {
                _sqlClient.Ado.BeginTran();
                RouteData result = await DoStockInScanComplateCore(stockInId, inventoryBoxId, materials, remark);
                if (!result.IsSccuess)
                {
                    _sqlClient.Ado.RollbackTran();
                    return result;
                }
                _sqlClient.Ado.CommitTran();
                return result;
            }
            catch (Exception)
            {
                _sqlClient.Ado.RollbackTran();
                return YL.Core.Dto.RouteData.From(PubMessages.E2005_STOCKIN_BOXOUT_FAIL);
            }

        }
         
        private async Task<RouteData> DoStockInScanComplateCore(long stockInId, long inventoryBoxId, Wms_StockMaterialDetailDto[] materials, string remark )
        {
            if(materials.Select(x => x.Position).GroupBy(x => x).Any(x => x.Count() > 1))
            {
                return YL.Core.Dto.RouteData.From(PubMessages.E2018_STOCKIN_POSITION_DUPLICATE);
            }
            Wms_stockin stockin = await _sqlClient.Queryable<Wms_stockin>().FirstAsync(x => x.StockInId == stockInId);
            if (stockin == null) { return YL.Core.Dto.RouteData.From(PubMessages.E2013_STOCKIN_NOTFOUND); }
            if (stockin.StockInStatus == StockInStatus.task_finish.ToByte()) { return YL.Core.Dto.RouteData.From(PubMessages.E2014_STOCKIN_ALLOW_FINISHED); }

            Wms_inventorybox inventoryBox = await _sqlClient.Queryable<Wms_inventorybox>().FirstAsync(x => x.InventoryBoxId == inventoryBoxId);
            if (inventoryBox == null) { return YL.Core.Dto.RouteData.From(PubMessages.E1011_INVENTORYBOX_NOTFOUND); }
            if (inventoryBox.Status != InventoryBoxStatus.Outed) { return YL.Core.Dto.RouteData.From(PubMessages.E1014_INVENTORYBOX_NOTOUTED); }

            Wms_inventoryboxTask inventoryBoxTask = await _sqlClient.Queryable<Wms_inventoryboxTask>().FirstAsync(x => x.InventoryBoxId == inventoryBoxId && x.Status == InventoryBoxTaskStatus.task_outed.ToByte());
            if (inventoryBoxTask == null) { return YL.Core.Dto.RouteData.From(PubMessages.E1014_INVENTORYBOX_NOTOUTED); } //数据异常 

            Wms_inventory[] inventories = _sqlClient.Queryable<Wms_inventory>().Where(x => x.InventoryBoxId == inventoryBoxId).ToArray();
            int count = inventories.Count(); //料格使用数量
            foreach (Wms_StockMaterialDetailDto material in materials)
            {
                if(inventories.Any(x => x.Position == material.Position))
                {
                    return YL.Core.Dto.RouteData.From(PubMessages.E2019_STOCKIN_POSITION_USED,$"料格:{material.Position}"); 
                }
                Wms_stockindetail detail = await _sqlClient.Queryable<Wms_stockindetail>().FirstAsync(x => x.StockInId == stockInId && x.MaterialId.ToString() == material.MaterialId);
                if (detail == null) { return YL.Core.Dto.RouteData.From(PubMessages.E2001_STOCKINDETAIL_NOTFOUND, $"MaterialId='{material.MaterialId}'"); }
                if (detail.Status == StockInStatus.task_finish.ToByte()) { return YL.Core.Dto.RouteData.From(PubMessages.E2002_STOCKINDETAIL_ALLOW_FINISHED); }

                Wms_stockindetail_box detailbox = await _sqlClient.Queryable<Wms_stockindetail_box>().FirstAsync(
                    x => x.InventoryBoxTaskId == inventoryBoxTask.InventoryBoxTaskId && x.StockinDetailId == detail.StockInDetailId);
                if (detailbox != null && detailbox.Position == material.Position)
                {
                    detailbox.Qty += material.Qty;

                    if (await _sqlClient.Updateable(detailbox).ExecuteCommandAsync() == 0)
                    {
                        return YL.Core.Dto.RouteData.From(PubMessages.E0002_UPDATE_COUNT_FAIL);
                    }
                }
                else
                {
                    count++;
                    if (count > inventoryBox.Size)
                    {
                        return YL.Core.Dto.RouteData<InventoryDetailDto[]>.From(PubMessages.E1010_INVENTORYBOX_BLOCK_OVERLOAD);
                    } 
                    detailbox = new Wms_stockindetail_box()
                    {
                        DetailBoxId = PubId.SnowflakeId,
                        InventoryBoxTaskId = inventoryBoxTask.InventoryBoxTaskId,
                        InventoryBoxId = inventoryBox.InventoryBoxId,
                        Position = material.Position,
                        StockinDetailId = detail.StockInDetailId,
                        Qty = material.Qty,
                        Remark = remark,
                        CreateDate = DateTime.Now,
                        CreateBy = this.UserDto.UserId
                    };

                    if (await _sqlClient.Insertable(detailbox).ExecuteCommandAsync() == 0)
                    {
                        return YL.Core.Dto.RouteData<InventoryDetailDto[]>.From(PubMessages.E2005_STOCKIN_BOXOUT_FAIL);
                    }
                }
                if (detail.Status == StockInStatus.task_confirm.ToByte())
                {
                    detail.Status = StockInStatus.task_working.ToByte();
                    if (await _sqlClient.Updateable(detail).ExecuteCommandAsync() == 0)
                    {
                        return YL.Core.Dto.RouteData<InventoryDetailDto[]>.From(PubMessages.E2005_STOCKIN_BOXOUT_FAIL);
                    }
                }
            }

            if (stockin.StockInStatus == StockInStatus.task_confirm.ToByte())
            {
                stockin.StockInStatus = StockInStatus.task_working.ToByte();
                await _sqlClient.Updateable(stockin).ExecuteCommandAsync();
            }

            return YL.Core.Dto.RouteData.From(PubMessages.I2001_STOCKIN_SCAN_SCCUESS);
        }
         
        /// <summary>
        /// 入库单完成
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="stockInId"></param>
        /// <returns></returns>
        public async Task<RouteData> DoStockInComplate(long stockInId)
        {
            try
            {
                _sqlClient.Ado.BeginTran();

                Wms_stockin stockin = await _sqlClient.Queryable<Wms_stockin>().FirstAsync(x => x.StockInId == stockInId);
                if (stockin == null) { return YL.Core.Dto.RouteData.From(PubMessages.E2013_STOCKIN_NOTFOUND); }
                if (stockin.StockInStatus == StockInStatus.task_finish.ToByte()) {
                    return YL.Core.Dto.RouteData.From(PubMessages.E2014_STOCKIN_ALLOW_FINISHED);
                }

                stockin.StockInStatus = StockInStatus.task_finish.ToByte();
                if (_sqlClient.Updateable(stockin).ExecuteCommand() == 0)
                {
                    return YL.Core.Dto.RouteData.From(PubMessages.E2017_STOCKIN_FAIL, "入库单详细状态更新失败");
                }
                List<Wms_stockindetail> details = await _sqlClient.Queryable<Wms_stockindetail>().Where(
                    x => x.StockInId == stockin.StockInId).ToListAsync();
                foreach (Wms_stockindetail detail in details)
                {
                    if (detail.Status == StockInStatus.task_finish.ToByte()) continue;
                    detail.Status = StockInStatus.task_finish.ToByte();
                    detail.ModifiedBy = this.UserDto.UserId;
                    detail.ModifiedDate = DateTime.Now;
                    if (_sqlClient.Updateable(detail).ExecuteCommand() == 0)
                    {
                        return YL.Core.Dto.RouteData.From(PubMessages.E2017_STOCKIN_FAIL, "入库单详细状态更新失败");
                    }
                }
                _sqlClient.Ado.CommitTran();
                return YL.Core.Dto.RouteData.From(PubMessages.I2002_STOCKIN_SCCUESS);
            }
            catch (Exception ex)
            {
                _sqlClient.Ado.RollbackTran();
                return YL.Core.Dto.RouteData.From(PubMessages.E2017_STOCKIN_FAIL, ex.Message);
            }

        }

        public async Task<RouteData> DoStockOutLock(long stockOutId)
        {
            try
            {
                _sqlClient.Ado.BeginTran();

                Wms_stockout stockout = _sqlClient.Queryable<Wms_stockout>().First(x => x.StockOutId == stockOutId);
                if (stockout == null) { return YL.Core.Dto.RouteData.From(PubMessages.E2113_STOCKOUT_NOTFOUND); }
                if (stockout.StockOutStatus == StockOutStatus.task_finish.ToByte()) { return YL.Core.Dto.RouteData.From(PubMessages.E2114_STOCKOUT_ALLOW_FINISHED); }
                if (stockout.IsLocked) { return YL.Core.Dto.RouteData.From(PubMessages.E2121_STOCKOUT_ALLOW_LOCKED); }

                stockout.IsLocked = true;
                if (_sqlClient.Updateable(stockout).ExecuteCommand() == 0)
                {
                    return YL.Core.Dto.RouteData.From(PubMessages.E2118_STOCKOUT_LOCK_FAIL, "出库单锁定状态更新失败");
                }

                List<Wms_stockoutdetail> details = await _sqlClient.Queryable<Wms_stockoutdetail>().Where(x => x.StockOutId == stockout.StockOutId).ToListAsync();
                foreach (Wms_stockoutdetail detail in details)
                {
                    if (detail.Status == StockOutStatus.task_finish.ToByte()) continue;

                    //优先锁定自身生产令号的物料
                    Wms_inventory[] inventories = _sqlClient.Queryable<Wms_inventory>()
                        .Where(x => x.MaterialId == detail.MaterialId && !x.IsLocked && (x.OrderNo == stockout.OrderNo || string.IsNullOrEmpty(x.OrderNo)))
                        .OrderBy(x => x.OrderNo, OrderByType.Desc).ToArray();
                    int lockedQty = 0;
                    int needQty = detail.PlanOutQty - detail.ActOutQty;
                    foreach (Wms_inventory inventory in inventories)
                    {
                        inventory.IsLocked = true;
                        if (_sqlClient.Updateable(inventory).ExecuteCommand() == 0)
                        {
                            _sqlClient.Ado.RollbackTran();
                            return YL.Core.Dto.RouteData.From(PubMessages.E2118_STOCKOUT_LOCK_FAIL, "料格锁定状态更新失败");
                        }
                        lockedQty += inventory.Qty;
                        if (lockedQty >= needQty)
                        {
                            //已锁定足够物料
                            break;
                        }
                    }
                    if (lockedQty < needQty)
                    {
                        _sqlClient.Ado.RollbackTran();
                        Wms_material material = await _sqlClient.Queryable<Wms_material>().FirstAsync(x => x.MaterialId == detail.MaterialId);
                        return YL.Core.Dto.RouteData.From(PubMessages.E2119_STOCKOUT_MATERIAL_ENOUGH, $"{material.MaterialNo}({material.MaterialName}) 需求/可锁定数：{lockedQty}/{needQty}");
                    }
                }

                _sqlClient.Ado.CommitTran();
                return YL.Core.Dto.RouteData.From(PubMessages.I2103_STOCKOUT_LOCK_SCCUESS);
            }
            catch (Exception ex)
            {
                _sqlClient.Ado.RollbackTran();
                return YL.Core.Dto.RouteData.From(PubMessages.E2118_STOCKOUT_LOCK_FAIL, ex.Message);
            }
        }
         
        /// <summary>
        /// 出库扫描完成
        /// </summary>
        /// <param name="stockOutId"></param>
        /// <param name="inventoryBoxId"></param>
        /// <param name="materials"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public async Task<RouteData> DoStockOutScanComplate(long stockOutId, long inventoryBoxId, Wms_StockMaterialDetailDto[] materials, string remark)
        {
            try
            {
                _sqlClient.Ado.BeginTran();
                RouteData result = await DoStockOutScanComplateCore(stockOutId, inventoryBoxId, materials, remark);
                if (!result.IsSccuess)
                {
                    _sqlClient.Ado.RollbackTran();
                    return result;
                }
                _sqlClient.Ado.CommitTran();
                return result;
            }
            catch (Exception)
            {
                _sqlClient.Ado.RollbackTran();
                return YL.Core.Dto.RouteData.From(PubMessages.E2105_STOCKOUT_BOXOUT_FAIL);
            }
        }

        private async Task<RouteData> DoStockOutScanComplateCore(long stockOutId, long inventoryBoxId, Wms_StockMaterialDetailDto[] materials, string remark)
        {
            try
            {
                Wms_stockout stockout = await _sqlClient.Queryable<Wms_stockout>().FirstAsync(x => x.StockOutId == stockOutId);
                if (stockout == null) { return YL.Core.Dto.RouteData.From(PubMessages.E2113_STOCKOUT_NOTFOUND); }
                if (stockout.StockOutStatus == StockOutStatus.task_finish.ToByte()) { return YL.Core.Dto.RouteData.From(PubMessages.E2114_STOCKOUT_ALLOW_FINISHED); }

                Wms_inventorybox inventoryBox = await _sqlClient.Queryable<Wms_inventorybox>().FirstAsync(x => x.InventoryBoxId == inventoryBoxId);
                if (inventoryBox == null) { return YL.Core.Dto.RouteData.From(PubMessages.E1011_INVENTORYBOX_NOTFOUND); }
                if (inventoryBox.Status != InventoryBoxStatus.Outed) { return YL.Core.Dto.RouteData.From(PubMessages.E1014_INVENTORYBOX_NOTOUTED); }

                Wms_inventoryboxTask inventoryBoxTask = await _sqlClient.Queryable<Wms_inventoryboxTask>().FirstAsync(x => x.InventoryBoxId == inventoryBoxId && x.Status == InventoryBoxTaskStatus.task_outed.ToByte());
                if (inventoryBoxTask == null) { return YL.Core.Dto.RouteData.From(PubMessages.E1014_INVENTORYBOX_NOTOUTED); } //数据异常 


                Wms_inventory[] inventories = _sqlClient.Queryable<Wms_inventory>().Where(x => x.InventoryBoxId == inventoryBoxId).ToArray();

                foreach (Wms_StockMaterialDetailDto material in materials)
                {
                    Wms_stockoutdetail detail = await _sqlClient.Queryable<Wms_stockoutdetail>().FirstAsync(x => x.StockOutId == stockOutId && x.MaterialId.ToString() == material.MaterialId);
                    if (detail == null) { return YL.Core.Dto.RouteData.From(PubMessages.E2101_STOCKOUTDETAIL_NOTFOUND, $"MaterialId='{material.MaterialId}'"); }
                    if (detail.Status == StockOutStatus.task_finish.ToByte()) { return YL.Core.Dto.RouteData.From(PubMessages.E2102_STOCKOUTDETAIL_ALLOW_FINISHED); }

                    Wms_stockoutdetail_box box = await _sqlClient.Queryable<Wms_stockoutdetail_box>().FirstAsync(x => x.InventoryBoxTaskId == inventoryBoxTask.InventoryBoxTaskId && x.StockOutDetailId == detail.StockOutDetailId);
                    if (box != null)
                    {
                        box.Qty += material.Qty;

                        if (_sqlClient.Updateable(box).ExecuteCommand() == 0)
                        {
                            return YL.Core.Dto.RouteData.From(PubMessages.E0002_UPDATE_COUNT_FAIL);
                        }
                    }
                    else
                    {
                        Wms_inventory[] targetInventories = inventories.Where(x => x.MaterialId == detail.MaterialId).ToArray();
                        if (targetInventories.Length == 0)
                        {
                            return YL.Core.Dto.RouteData.From(PubMessages.E1015_INVENTORYBOX_MATERIAL_NOTMATCH);
                        }
                        Wms_inventory targetInventory = targetInventories.FirstOrDefault(x => !x.IsLocked || x.OrderNo == stockout.OrderNo);
                        if (targetInventory == null)
                        {
                            return YL.Core.Dto.RouteData.From(PubMessages.E1020_INVENTORYBOX_MATERIAL_LOCKED, $"物料编号:{material.MaterialNo}");
                        }
                        box = new Wms_stockoutdetail_box()
                        {
                            DetailBoxId = PubId.SnowflakeId,
                            InventoryBoxTaskId = inventoryBoxTask.InventoryBoxTaskId,
                            InventoryBoxId = inventoryBox.InventoryBoxId,
                            StockOutDetailId = detail.StockOutDetailId,
                            Qty = material.Qty,
                            Remark = remark,
                            CreateDate = DateTime.Now,
                            CreateBy = this.UserDto.UserId
                        };
                        _sqlClient.Insertable(box).ExecuteCommand();
                    }
                    if (detail.Status == StockOutStatus.task_confirm.ToByte())
                    {
                        detail.Status = StockOutStatus.task_working.ToByte();
                        if (_sqlClient.Updateable(detail).ExecuteCommand() == 0)
                        {
                            return YL.Core.Dto.RouteData.From(PubMessages.E2105_STOCKOUT_BOXOUT_FAIL);
                        }
                    }
                }

                if (stockout.StockOutStatus == StockOutStatus.task_confirm.ToByte())
                {
                    stockout.StockOutStatus = StockOutStatus.task_working.ToByte();
                    if (_sqlClient.Updateable(stockout).ExecuteCommand() == 0)
                    {
                        return YL.Core.Dto.RouteData.From(PubMessages.E2105_STOCKOUT_BOXOUT_FAIL);
                    }
                }

                _sqlClient.Ado.CommitTran();
                return YL.Core.Dto.RouteData.From(PubMessages.I2101_STOCKOUT_SCAN_SCCUESS);
            }
            catch (Exception)
            {
                _sqlClient.Ado.RollbackTran();
                return YL.Core.Dto.RouteData.From(PubMessages.E2105_STOCKOUT_BOXOUT_FAIL);
            }
        }

        /// <summary>
        /// 设置出库单完成
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="stockOutId"></param>
        /// <returns></returns>
        public async Task<RouteData> DoStockOutComplate(long stockOutId)
        {
            try
            {
                _sqlClient.Ado.BeginTran();

                Wms_stockout stockout = await _sqlClient.Queryable<Wms_stockout>().FirstAsync(x => x.StockOutId == stockOutId);
                if (stockout == null) { return YL.Core.Dto.RouteData.From(PubMessages.E2113_STOCKOUT_NOTFOUND); }
                if (stockout.StockOutStatus == StockOutStatus.task_finish.ToByte()) { return YL.Core.Dto.RouteData.From(PubMessages.E2114_STOCKOUT_ALLOW_FINISHED); }

                stockout.StockOutStatus = StockOutStatus.task_finish.ToByte();
                if (_sqlClient.Updateable(stockout).ExecuteCommand() == 0)
                {
                    return YL.Core.Dto.RouteData.From(PubMessages.E2117_STOCKOUT_FAIL, "出库单详细状态更新失败");
                }
                List<Wms_stockoutdetail> details = await _sqlClient.Queryable<Wms_stockoutdetail>().Where(x => x.StockOutId == stockout.StockOutId).ToListAsync();
                foreach (Wms_stockoutdetail detail in details)
                {
                    if (detail.Status == StockOutStatus.task_finish.ToByte()) continue;
                    detail.Status = StockOutStatus.task_finish.ToByte();
                    detail.ModifiedBy = this.UserDto.UserId;
                    detail.ModifiedDate = DateTime.Now;
                    if (_sqlClient.Updateable(detail).ExecuteCommand() == 0)
                    {
                        return YL.Core.Dto.RouteData.From(PubMessages.E2117_STOCKOUT_FAIL, "出库单详细状态更新失败");
                    }
                }
                _sqlClient.Ado.CommitTran();
                return YL.Core.Dto.RouteData.From(PubMessages.I2102_STOCKOUT_SCCUESS);
            }
            catch (Exception ex)
            {
                _sqlClient.Ado.RollbackTran();
                return YL.Core.Dto.RouteData.From(PubMessages.E2117_STOCKOUT_FAIL, ex.Message);
            }
        }

        /// <summary>
        /// 获取包含指定物料的料箱的列表
        /// </summary>
        /// <param name="materialId"></param>
        /// <returns></returns>
        public async Task<RouteData<Wms_InventoryBoxMaterialInfo[]>> GetInventoryBoxList(string materialNo)
        {
            try
            {

                List<Wms_InventoryBoxMaterialInfo> inventoryBoxs = await _sqlClient.Queryable<Wms_inventorybox,Wms_inventory>(
                    (ib, i) => new object[] {
                       JoinType.Left,ib.InventoryBoxId==i.InventoryBoxId , 
                      }
                    )
                    .Where((ib,i) => i.MaterialNo == materialNo || i.MaterialOnlyId == materialNo)
                    .Select((ib,i) => new Wms_InventoryBoxMaterialInfo
                    {
                        InventoryBoxId = ib.InventoryBoxId,
                        InventoryBoxNo = ib.InventoryBoxNo,
                        InventoryBoxStatus = ib.Status,
                        StorageRackName = ib.StorageRackName,
                        MaterialId = i.MaterialId.Value,
                        MaterialNo = i.MaterialNo,
                        MaterialOnlyId = i.MaterialOnlyId,
                        MaterialName = i.MaterialName,
                        Qty = i.Qty,
                        Floor = ib.Floor.Value,
                        Row = ib.Row.Value,
                        Column = ib.Column.Value,
                        Position = i.Position,
                        ModifiedBy = i.ModifiedUser,
                        ModifiedDate = i.ModifiedDate.Value.ToString("yyyy/MM/dd HH:mm:ss")
                    })
                    .ToListAsync();

                return RouteData<Wms_InventoryBoxMaterialInfo[]>.From(inventoryBoxs.ToArray());
            }
            catch (Exception ex)
            {
                return RouteData<Wms_InventoryBoxMaterialInfo[]>.From(PubMessages.E4103_INVENTORYBOX_GET_FAIL, ex.Message);
            }
        }

        public async Task<RouteData<bool>> HasStockOutNofity()
        {
            try
            {

                bool result = await _sqlClient.Queryable<Wms_stockout>()
                    .Where(x => (x.IsNotified == null || x.IsNotified == false) && x.IsDel == DeleteFlag.Normal)
                    .AnyAsync();

                return RouteData<bool>.From(result);
            }
            catch (Exception ex)
            {
                return RouteData<bool>.From(PubMessages.E0011_DATABASE_UNKNOW_FAIL, ex.Message);
            }
        }

        public async Task<RouteData<Wms_StockOutDto[]>> QueryStockOutNofityList()
        {
            try
            {
                List<Wms_stockout> result = await _sqlClient.Queryable<Wms_stockout>()
                    .Where(x => (x.IsNotified == null || x.IsNotified == false) && x.IsDel == DeleteFlag.Normal)
                    .ToListAsync();

                Wms_StockOutDto[] dto = JsonConvert.DeserializeObject<Wms_StockOutDto[]>(JsonConvert.SerializeObject(result));
                return RouteData<Wms_StockOutDto[]>.From(dto);
            }
            catch (Exception ex)
            {
                return RouteData<Wms_StockOutDto[]>.From(PubMessages.E0011_DATABASE_UNKNOW_FAIL, ex.Message);
            }
        }

        public async Task<RouteData> SetStockOutNofitied()
        {
            try
            {
                List<Wms_stockout> result = await _sqlClient.Queryable<Wms_stockout>()
                    .Where(x => (x.IsNotified == null || x.IsNotified == false) && x.IsDel == DeleteFlag.Normal)
                    .ToListAsync();

                foreach(Wms_stockout stockout in result)
                {
                    stockout.IsNotified = true;
                    _sqlClient.Updateable<Wms_stockout>(result);
                }
                 
                return new RouteData();
            }
            catch (Exception ex)
            {
                return RouteData.From(PubMessages.E0004_DATABASE_UPDATE_FAIL, ex.Message);
            }
        }
    }
}

using Newtonsoft.Json;
using NLog;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMSCore.Outside;
using YL.Core.Dto;
using YL.Core.Entity;
using YL.Utils.Pub;

namespace Services.Outside
{
    public class SelfReservoirAreaManager
    {
        private static int _count1 = 0;
        private static int _count2 = 0;

        public static Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 默认非固定库位库区
        /// </summary>
        public static long DefaultUnfixResrovoirAreaId { get; } = 2;

        public static string GetPLC(long reservoirAreaId, PLCPosition pos)
        {
            if (reservoirAreaId == 1)
            {
                if (pos == PLCPosition.Left)
                {
                    return "1010";
                }
                else if (pos == PLCPosition.Right)
                {
                    return "1020";
                }
                else
                {
                    _count1++;
                    return _count1 % 2 == 1 ? "1010" : "1020";
                }
            }
            else
            {
                if (pos == PLCPosition.Left)
                {
                    return "5010";
                }
                else if (pos == PLCPosition.Right)
                {
                    return "6010";
                }
                else
                {
                    _count2++;
                    return _count2 % 2 == 1 ? "5010" : "6010";
                }
            }
        }

        /// <summary>
        /// 是否是固定料箱位置的库区
        /// </summary>
        /// <returns></returns>
        public static bool IsPositionFix(long reservoirAreaId)
        {
            return reservoirAreaId == 1 || reservoirAreaId == 3;
        }

    }

    public static class SelfReservoirAreaExt
    {

        /// <summary>
        /// 获取空闲库位
        /// </summary>
        /// <returns></returns>
        public static async Task<RouteData<Wms_storagerack>> GetIdleStorageRack(this ISqlSugarClient client, long reservoirAreaId)
        {
            Wms_storagerack storagerack = await client.Queryable<Wms_storagerack, Wms_inventorybox>(
                        (s, ib) => new object[] {
                               JoinType.Left,s.StorageRackId==ib.StorageRackId ,
                        }
                      ).Where((s, ib) => ib.InventoryBoxNo == null && s.ReservoirAreaId == reservoirAreaId && s.Status == StorageRackStatus.Normal)
                      .OrderBy((s, ib) => ib.Column, OrderByType.Desc) //就近原则
                      .OrderBy((s, ib) => ib.Floor, OrderByType.Asc) //就近原则
                      .Select((s, ib) => s).FirstAsync();
            if (storagerack == null)
            {
                return RouteData<Wms_storagerack>.From(PubMessages.E2308_WCS_STORGERACK_FULL, $"");
            }
            return RouteData<Wms_storagerack>.From(storagerack);
        }


        /// <summary>
        /// 获取最适合的料箱
        /// </summary>
        /// <param name="client"></param>
        /// <param name="reservoirAreaId"></param>
        /// <param name="requestSize"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static async Task<RouteData<Wms_inventorybox>> GetBestInvertoryBox(this ISqlSugarClient client, long? reservoirAreaId, int requestSize, PLCPosition pos)
        {
            //查询是否有符合要求的料箱
            var query = client.Queryable<Wms_inventorybox,Wms_storagerack>(
                (ib, s) => new object[] {
                        JoinType.Left,ib.StorageRackId==s.StorageRackId ,
                })
               .Where((ib, s) => 
                   ib.UsedSize < ib.Size
                   && ib.Size == requestSize 
                   && ib.Status == (int)InventoryBoxStatus.InPosition
                   && s.Status == (int)StorageRackStatus.Normal
               );

            if (reservoirAreaId != null)
            {
                query = query.Where((ib) => ib.ReservoirAreaId == reservoirAreaId.Value);
            }

            Wms_inventorybox inventoryBox = await query
                .OrderBy((ib) => ib.Column, OrderByType.Desc)
                .OrderBy((ib) => ib.Floor, OrderByType.Asc)
                .FirstAsync();
            //如果没有符合要求的料想,且是要求多宫格料箱时,尝试选取完整料箱进行分割
            if (inventoryBox == null && requestSize > 1)
            {
                var query2 = client.Queryable<Wms_inventorybox>()
                    .Where((ib) => ib.Size == 1 && ib.UsedSize == 0 &&
                    ib.Status == (int)InventoryBoxStatus.InPosition)
                    .OrderBy((ib) => ib.Column, OrderByType.Desc)
                    .OrderBy((ib) => ib.Floor, OrderByType.Asc);
                if (reservoirAreaId != null)
                {
                    query2 = query2.Where((ib) => ib.ReservoirAreaId == reservoirAreaId.Value);
                }
                inventoryBox = await query2.FirstAsync();

                if (inventoryBox != null)
                {
                    inventoryBox.Size = requestSize;
                }
                if (client.Updateable(inventoryBox).ExecuteCommand() == 0)
                {
                    return YL.Core.Dto.RouteData<Wms_inventorybox>.From(PubMessages.E0004_DATABASE_UPDATE_FAIL);
                }
            }
            if (inventoryBox == null)
            {
                return RouteData<Wms_inventorybox>.From(PubMessages.E1007_INVENTORYBOX_ALLOWUSED);
            }
            return RouteData<Wms_inventorybox>.From(inventoryBox);
        }

        /// <summary>
        /// 确认归库成功以后库存入账
        /// </summary>
        /// <param name="client"></param>
        /// <param name="reservoirAreaId"></param>
        /// <param name="requestSize"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static async Task<RouteData> ConfirmInventory(
            this ISqlSugarClient client,long inventoryBoxTaskId, long inventoryBoxId, SysUserDto user)
        {
            List<Wms_inventory> inventories =
                await client.Queryable<Wms_inventory>().Where(x => x.InventoryBoxId == inventoryBoxId).ToListAsync();

            //仅查询入库详细,出库物料在归库前已结算归库后无需处理
            var stockInDetailInfos =
                await client.Queryable<Wms_stockindetail_box, Wms_stockindetail, Wms_stockin, Wms_inventorybox>(
                (sidb, sid, si, ib) => new object[] {
                        JoinType.Left,sidb.StockinDetailId==sid.StockInDetailId ,
                        JoinType.Left,sid.StockInId==sid.StockInId ,
                        JoinType.Left,sidb.InventoryBoxId==ib.InventoryBoxId
                }
            )
            .Where((sidb, sid, si, ib) => sidb.InventoryBoxTaskId == inventoryBoxTaskId)
            .Select((sidb, sid, si, ib) =>
                new {
                    si.StockInId,
                    si.StockInNo,
                    sid.StockInDetailId,
                    sidb.InventoryBoxId,
                    ib.InventoryBoxNo,
                    sidb.Position
                }
            ).ToListAsync();

            List<Wms_inventory> updateList = new List<Wms_inventory>();
            List<Wms_inventoryrecord> updateRecordList = new List<Wms_inventoryrecord>();
            foreach (Wms_inventory inventory in inventories)
            {
                if (inventory.BufferQty == 0)
                {
                    continue;
                }
                int beforeQty = inventory.Qty;
                int qty = inventory.BufferQty;
                inventory.Qty += inventory.BufferQty;
                inventory.BufferQty = 0;
                inventory.ModifiedBy = user.UserId;
                inventory.ModifiedUser = user.UserName;
                inventory.ModifiedDate = DateTime.Now;
                updateList.Add(inventory);

                var stockInDetailInfo = stockInDetailInfos.FirstOrDefault(
                    x => x.InventoryBoxId == inventoryBoxId && x.Position == inventory.Position);
                if (stockInDetailInfo != null)
                {
                    Wms_inventoryrecord record = new Wms_inventoryrecord()
                    {
                        InventoryrecordId = PubId.SnowflakeId,
                        StockInDetailId = stockInDetailInfo.StockInDetailId,
                        StockOutDetailId = null,
                        StockNo = stockInDetailInfo.StockInNo,
                        InventoryBoxId = inventory.InventoryBoxId,
                        InventoryBoxNo = stockInDetailInfo.InventoryBoxNo,
                        InventoryId = inventory.InventoryId,
                        InventoryPosition = inventory.Position,
                        MaterialId = inventory.MaterialId.Value,
                        MaterialNo = inventory.MaterialNo,
                        MaterialOnlyId = inventory.MaterialOnlyId,
                        MaterialName = inventory.MaterialName,
                        BeforeQty = beforeQty,
                        Qty = qty,
                        AfterQty = inventory.Qty,
                        CreateBy = user.UserId,
                        CreateDate = DateTime.Now,
                        CreateUser = user.UserName,
                        IsDel = DeleteFlag.Normal
                    };
                    updateRecordList.Add(record);
                }
            }

            if (updateList.Count == 0)
            {
                return new RouteData();
            }
            if (client.Updateable(updateList).ExecuteCommand() == 0)
            {
                return RouteData.From(PubMessages.E0002_UPDATE_COUNT_FAIL, "");
            }

            if (client.Insertable(updateRecordList).ExecuteCommand() == 0)
            {
                return RouteData.From(PubMessages.E1021_INVENTORYRECORD_FAIL);
            }


            return new RouteData();
        }

        /// <summary>
        /// 更改料箱相关的入库任务的状态
        /// </summary>
        /// <param name="client"></param>
        /// <param name="inventoryBoxTaskId"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static async Task<RouteData> ConfirmRelationStockIn(
            this ISqlSugarClient client, long inventoryBoxTaskId, SysUserDto user)
        {
            List<Wms_stockindetail_box> stockInDeltailBoxs =
                await client.Queryable<Wms_stockindetail_box>().Where(x => x.InventoryBoxTaskId == inventoryBoxTaskId ).ToListAsync();
            if(stockInDeltailBoxs.Count == 0)
            { 
                return new RouteData(); //如果没有相关任务则不做处理
            }
            List<long> confiredStockInIds = new List<long>();
            foreach(Wms_stockindetail_box detailbox in stockInDeltailBoxs)
            { 
                Wms_stockindetail stockInDetail =
                    await client.Queryable<Wms_stockindetail>().Where(x => x.StockInDetailId == detailbox.StockinDetailId).FirstAsync();
                if(stockInDetail == null)
                {
                    continue;
                }
                if(stockInDetail.Status == (int)StockInStatus.task_finish)
                {
                    continue;
                }
                if(stockInDetail.ActInQty < stockInDetail.PlanInQty)
                {
                    continue;   
                }
                stockInDetail.Status = (int)StockInStatus.task_finish;
                stockInDetail.ModifiedBy = user.UserId;
                stockInDetail.ModifiedUser = user.UserName;
                stockInDetail.ModifiedDate = DateTime.Now;
                if(client.Updateable(stockInDetail).ExecuteCommand() == 0)
                {
                    continue;
                }

                if (!confiredStockInIds.Contains(stockInDetail.StockInId.Value))
                {
                    confiredStockInIds.Add(stockInDetail.StockInId.Value);
                }
            }

            foreach (long stockInId in confiredStockInIds)
            {
                bool hasWorking = await client.Queryable<Wms_stockindetail>().AnyAsync(x => x.StockInId == stockInId && x.Status != (int)StockInStatus.task_finish);
                if (!hasWorking)
                {
                    Wms_stockin stockin =await client.Queryable<Wms_stockin>().FirstAsync(x => x.StockInId == stockInId);
                    if(stockin == null)
                    {
                        continue;
                    }
                    stockin.StockInStatus = (int)StockInStatus.task_finish;
                    stockin.ModifiedBy = user.UserId;
                    stockin.ModifiedUser = user.UserName;
                    stockin.ModifiedDate = DateTime.Now;
                    if(client.Updateable(stockin).ExecuteCommand() == 0)
                    {
                        //LOG
                    }
                    var anyWorking = await client.Queryable<Wms_stockin>()
                        .AnyAsync(x => x.MesTaskId == stockin.MesTaskId 
                        && x.StockInStatus != (int)StockInStatus.task_finish 
                        && x.StockInStatus != (int)StockInStatus.task_canceled);

                    if (!anyWorking)
                    {
                        Wms_mestask mesTask = await client.Queryable<Wms_mestask>()
                            .FirstAsync(x => x.MesTaskId == stockin.MesTaskId);
                        if (mesTask == null)
                        {
                            //正常不可能
                            return YL.Core.Dto.RouteData.From(PubMessages.E3000_MES_STOCKINTASK_NOTFOUND);
                        }
                        try
                        {
                            //通知处理无论成功失败不影响后续处理
                            await client.NofityStockIn(mesTask);
                        }
                        catch { }
                    }
                }
            }
            return new RouteData();

        }
         
        /// <summary>
        /// 通知MES入库完成
        /// </summary>
        /// <param name="stockOutId"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static async Task<RouteData> NofityStockIn(this ISqlSugarClient client, Wms_mestask mesTask)
        { 
            mesTask.ModifiedDate = DateTime.Now;
            mesTask.WorkStatus = MESTaskWorkStatus.WorkComplated;
            mesTask.NotifyStatus = MESTaskNotifyStatus.WaitResponse;
             
            try
            {
                List<Wms_stockin> stockIns = await client.Queryable<Wms_stockin>().Where(x => x.MesTaskId == mesTask.MesTaskId).ToListAsync();

                List<OutsideStockInResponseWarehouse> warehouseList = new List<OutsideStockInResponseWarehouse>();
                foreach (Wms_stockin stockIn in stockIns)
                {
                    OutsideStockInResponseWarehouse warehouse = warehouseList.FirstOrDefault(x => x.WarehouseId == stockIn.WarehouseId.ToString());
                    if (warehouse == null)
                    {
                        Wms_warehouse warehouseData = WMSApiManager.GetWarehouse(stockIn.WarehouseId);
                        warehouse = new OutsideStockInResponseWarehouse()
                        {
                            //WarehouseId = stockIn.WarehouseId.ToString(),
                            WarehouseId = warehouseData?.WarehouseNo, 
                            WarehouseName = warehouseData?.WarehouseName,
                            WarehousePosition = "",
                            WarehousingFinishTime = stockIn.ModifiedDate.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                        };
                        warehouseList.Add(warehouse);
                    }
                    List<Wms_stockindetail> stockInDetails = await client.Queryable<Wms_stockindetail>().Where(x => x.StockInId == stockIn.StockInId).ToListAsync();
                    foreach (Wms_stockindetail stockInDetail in stockInDetails)
                    {
                        OutsideMaterialResult material = new OutsideMaterialResult()
                        {
                            UniqueIndex = stockInDetail.UniqueIndex,
                            SuppliesId = stockInDetail.MaterialNo.ToString(),
                            SuppliesName = stockInDetail.MaterialName,
                            SuppliesNumber = stockInDetail.ActInQty.ToString(),
                            RefreshStock = stockInDetail.ActInQty.ToString(),
                            ErrorId = stockInDetail.ErrorId,
                            ErrorInfo = stockInDetail.ErrorInfo

                        };
                        warehouse.SuppliesInfoList.Add(material);
                        warehouse.SuppliesKinds = warehouse.SuppliesInfoList.Count;
                    }
                }

                OutsideStockInResponse response = new OutsideStockInResponse()
                {
                    WarehousingId = mesTask.WarehousingId,
                    WarehousingEntryNumber = warehouseList.Count,
                    WarehousingEntryFinishList = JsonConvert.SerializeObject(warehouseList)
                };

                SelfReservoirAreaManager._logger.Info($"[通知MES入库完成]开始通知MES,param={JsonConvert.SerializeObject(response)}"); 
                OutsideStockInResponseResult result = await MESApiAccessor.Instance.WarehousingFinish(response);
                SelfReservoirAreaManager._logger.Info($"[通知MES入库完成]通知MES成功,result={JsonConvert.SerializeObject(result)}");
                if (result.IsNormalExecution)
                {
                    mesTask.NotifyStatus = MESTaskNotifyStatus.Responsed;
                }
                else
                {
                    mesTask.Remark = $"Error={result.IsNormalExecution}";
                    mesTask.NotifyStatus = MESTaskNotifyStatus.Failed;
                }
            }
            catch (Exception ex)
            {
                mesTask.Remark = $"InnerError={ex.Message}";
                mesTask.NotifyStatus = MESTaskNotifyStatus.Failed;
                //_logger.LogError(ex, "入库完成通知时发生异常");
                //逻辑继续,寻找其它时机重新通知
                SelfReservoirAreaManager._logger.Error($"[通知MES入库完成]通知MES时发生异常,{ex.ToString()}");
            }
            if (client.Updateable(mesTask).ExecuteCommand() == 0)
            {
                SelfReservoirAreaManager._logger.Error($"[通知MES入库完成]E-0002-更新状态失败");
                return YL.Core.Dto.RouteData.From(PubMessages.E0002_UPDATE_COUNT_FAIL); 
            }

            SelfReservoirAreaManager._logger.Info($"[通知MES入库完成]更新状态成功,NotifyStatus={mesTask.NotifyStatus}");
            if (mesTask.NotifyStatus == MESTaskNotifyStatus.Responsed)
            {
                return new RouteData();
            }
            else
            {
                return YL.Core.Dto.RouteData.From(PubMessages.E3001_MES_STOCKIN_NOFITY_FAIL);

            }
        }

        public static async Task<RouteData> ConfirmRelationStockOut(
            this ISqlSugarClient client, long inventoryBoxTaskId, SysUserDto user)
        {
            List<Wms_stockoutdetail_box> stockOutDeltailBoxs =
                await client.Queryable<Wms_stockoutdetail_box>().Where(x => x.InventoryBoxTaskId == inventoryBoxTaskId).ToListAsync();
            if (stockOutDeltailBoxs.Count == 0)
            {
                return new RouteData(); //如果没有相关任务则不做处理
            }
            List<long> confiredStockOutIds = new List<long>();
            foreach (Wms_stockoutdetail_box detailbox in stockOutDeltailBoxs)
            {
                Wms_stockoutdetail stockOutDetail =
                    await client.Queryable<Wms_stockoutdetail>().Where(x => x.StockOutDetailId == detailbox.StockOutDetailId).FirstAsync();
                if (stockOutDetail == null)
                {
                    continue;
                }
                if (stockOutDetail.Status == (int)StockOutStatus.task_finish)
                {
                    continue;
                }
                if (stockOutDetail.ActOutQty < stockOutDetail.PlanOutQty)
                {
                    continue;
                }
                stockOutDetail.Status = (int)StockOutStatus.task_finish;
                stockOutDetail.ModifiedBy = user.UserId;
                stockOutDetail.ModifiedUser = user.UserName;
                stockOutDetail.ModifiedDate = DateTime.Now;
                if (client.Updateable(stockOutDetail).ExecuteCommand() == 0)
                {
                    continue;
                }

                if (!confiredStockOutIds.Contains(stockOutDetail.StockOutId.Value))
                {
                    confiredStockOutIds.Add(stockOutDetail.StockOutId.Value);
                }
            }

            foreach (long stockOutId in confiredStockOutIds)
            {
                bool hasWorking = await client.Queryable<Wms_stockoutdetail>().AnyAsync(x => x.StockOutId == stockOutId && x.Status != (int)StockInStatus.task_finish);
                if (!hasWorking)
                {
                    Wms_stockout stockout = await client.Queryable<Wms_stockout>().FirstAsync(x => x.StockOutId == stockOutId);
                    if (stockout == null)
                    {
                        continue;
                    }
                    stockout.StockOutStatus = (int)StockOutStatus.task_finish;
                    stockout.ModifiedBy = user.UserId;
                    stockout.ModifiedUser = user.UserName;
                    stockout.ModifiedDate = DateTime.Now;
                    if (client.Updateable(stockout).ExecuteCommand() == 0)
                    {
                        //LOG
                    }

                    bool anyWorking = await client.Queryable<Wms_stockout>()
                         .AnyAsync(x => x.MesTaskId == stockout.MesTaskId
                         && x.StockOutStatus != (int)StockOutStatus.task_finish
                         && x.StockOutStatus != (int)StockOutStatus.task_canceled);

                    if (!anyWorking)
                    {
                        Wms_mestask mesTask = await client.Queryable<Wms_mestask>()
                            .FirstAsync(x => x.MesTaskId == stockout.MesTaskId);
                        if (mesTask == null)
                        {
                            //正常不可能
                            return YL.Core.Dto.RouteData.From(PubMessages.E3100_MES_STOCKOUTTASK_NOTFOUND);
                        }
                        try
                        {
                            //通知处理无论成功失败不影响后续处理
                            await client.NofityStockOut(mesTask);
                        }
                        catch { }
                    }
                }
            }
            return new RouteData();

        }


        /// <summary>
        /// 通知MES出库完成
        /// </summary>
        /// <param name="stockOutId"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static async Task<RouteData> NofityStockOut(this ISqlSugarClient client, Wms_mestask mesTask)
        {
            mesTask.ModifiedDate = DateTime.Now;
            mesTask.WorkStatus = MESTaskWorkStatus.WorkComplated;
            mesTask.NotifyStatus = MESTaskNotifyStatus.WaitResponse;
             
            try
            {
                List<Wms_stockout> stockOuts = await client.Queryable<Wms_stockout>().Where(x => x.MesTaskId == mesTask.MesTaskId).ToListAsync();

                List<OutsideStockOutResponseWarehouse> warehouseList = new List<OutsideStockOutResponseWarehouse>();
                foreach (Wms_stockout stockOut in stockOuts)
                {
                    OutsideStockOutResponseWarehouse warehouse = warehouseList.FirstOrDefault(x => x.WarehouseId == stockOut.WarehouseId.ToString());
                    if (warehouse == null)
                    {

                        warehouse = new OutsideStockOutResponseWarehouse()
                        {
                            //WarehouseId = stockOut.WarehouseId.ToString(),
                            WarehouseId = WMSApiManager.GetWarehouse(stockOut.WarehouseId).WarehouseNo,
                            WarehouseName = "", //TODO
                            WarehousePosition ="",//TODO
                            WorkAreaName = mesTask.WorkAreaName,
                            WarehouseEntryFinishTime = stockOut.ModifiedDate.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                        };
                        warehouseList.Add(warehouse);
                    }
                    List<Wms_stockoutdetail> stockOutDetails = await client.Queryable<Wms_stockoutdetail>().Where(x => x.StockOutId == stockOut.StockOutId).ToListAsync();
                    foreach (Wms_stockoutdetail stockOutDetail in stockOutDetails)
                    {
                        OutsideMaterialResult material = new OutsideMaterialResult()
                        {
                            UniqueIndex = stockOutDetail.UniqueIndex,
                            SuppliesId = stockOutDetail.MaterialNo.ToString(),
                            SuppliesName = stockOutDetail.MaterialName,
                            SuppliesNumber = stockOutDetail.ActOutQty.ToString(),
                            RefreshStock = stockOutDetail.ActOutQty.ToString(),
                            ErrorId = stockOutDetail.ErrorId,
                            ErrorInfo = stockOutDetail.ErrorInfo

                        };
                        warehouse.SuppliesInfoList.Add(material);
                        warehouse.SuppliesKinds = warehouse.SuppliesInfoList.Count;
                    }
                }

                OutsideStockOutResponse response = new OutsideStockOutResponse()
                {
                    WarehouseEntryId = mesTask.WarehousingId,
                    WarehouseEntryFinishCount = warehouseList.Count,
                    WarehouseEntryFinishList = JsonConvert.SerializeObject(warehouseList)
                };

                SelfReservoirAreaManager._logger.Info($"[通知MES出库完成]开始通知MES,param={JsonConvert.SerializeObject(response)}");
                OutsideStockOutResponseResult result = await MESApiAccessor.Instance.WarehouseEntryFinish(response);
                SelfReservoirAreaManager._logger.Info($"[通知MES出库完成]通知MES成功,result={JsonConvert.SerializeObject(result)}");
                if (result.IsNormalExecution)
                {
                    mesTask.NotifyStatus = MESTaskNotifyStatus.Responsed;
                    mesTask.Remark = $"";
                }
                else
                {
                    mesTask.NotifyStatus = MESTaskNotifyStatus.Failed;
                    mesTask.Remark = $"Error={result.IsNormalExecution}";
                }
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "出库完成通知时发生异常");
                //逻辑继续,寻找其它时机重新通知
                mesTask.NotifyStatus = MESTaskNotifyStatus.Failed;
                mesTask.Remark = $"Error={ex.Message}";
                SelfReservoirAreaManager._logger.Error($"[通知MES出库完成]通知MES时发生异常,{ex.ToString()}");
            }
            
            if (client.Updateable(mesTask).ExecuteCommand() == 0)
            {
                SelfReservoirAreaManager._logger.Error($"[通知MES入库完成]E-0002-更新状态失败");
                return YL.Core.Dto.RouteData.From(PubMessages.E0002_UPDATE_COUNT_FAIL);

            }

            SelfReservoirAreaManager._logger.Info($"[通知MES入库完成]更新状态成功,NotifyStatus={mesTask.NotifyStatus}");
            if (mesTask.NotifyStatus == MESTaskNotifyStatus.Responsed)
            {
                return new RouteData();
            }
            else
            {
                return YL.Core.Dto.RouteData.From(PubMessages.E3101_MES_STOCKOUT_NOFITY_FAIL);

            }
        }


    }
}

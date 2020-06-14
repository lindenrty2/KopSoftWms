using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using YL.Core.Dto;
using YL.Core.Entity;
using YL.Utils.Pub;

namespace Services.Outside
{
    public class SelfReservoirAreaManager
    {
        private static int _count1 = 0;
        private static int _count2 = 0;

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
            var query = client.Queryable<Wms_inventorybox>()
               .Where((ib) => ib.UsedSize < ib.Size && ib.Size == requestSize && ib.Status == InventoryBoxStatus.InPosition);

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
                inventoryBox = await client.Queryable<Wms_inventorybox>()
                    .Where((ib) => ib.Size == 1 && ib.UsedSize == 0 &&
                    ib.Status == InventoryBoxStatus.InPosition)
                    .OrderBy((ib) => ib.Column, OrderByType.Desc)
                    .OrderBy((ib) => ib.Floor, OrderByType.Asc)
                    .FirstAsync();
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
        public static async Task<RouteData> ConfirmInventory(this ISqlSugarClient client, long? inventoryBoxId, SysUserDto user)
        {
            List<Wms_inventory> inventories =
                await client.Queryable<Wms_inventory>().Where(x => x.InventoryBoxId == inventoryBoxId).ToListAsync();
            List<Wms_inventory> updateList = new List<Wms_inventory>();
            foreach (Wms_inventory inventory in inventories)
            {
                if (inventory.BufferQty == 0)
                {
                    continue;
                }
                inventory.Qty += inventory.BufferQty;
                inventory.ModifiedBy = user.UserId;
                inventory.ModifiedUser = user.UserName;
                inventory.ModifiedDate = DateTime.Now;
                updateList.Add(inventory);
            }

            if (updateList.Count == 0)
            {
                return new RouteData();
            }
            if (client.Updateable(updateList).ExecuteCommand() == 0)
            {
                return RouteData.From(PubMessages.E0002_UPDATE_COUNT_FAIL, "");
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
                    stockin.StockInStatus = (int)StockInStatus.task_confirm;
                    stockin.ModifiedBy = user.UserId;
                    stockin.ModifiedUser = user.UserName;
                    stockin.ModifiedDate = DateTime.Now;
                    if(client.Updateable(stockin).ExecuteCommand() == 0)
                    {
                        //LOG
                    }
                }
            }
            return new RouteData();

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
                    stockout.StockOutStatus = (int)StockOutStatus.task_confirm;
                    stockout.ModifiedBy = user.UserId;
                    stockout.ModifiedUser = user.UserName;
                    stockout.ModifiedDate = DateTime.Now;
                    if (client.Updateable(stockout).ExecuteCommand() == 0)
                    {
                        //LOG
                    }
                }
            }
            return new RouteData();

        }
    }
}

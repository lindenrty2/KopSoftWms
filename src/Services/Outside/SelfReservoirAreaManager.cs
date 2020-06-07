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
        public static async Task<RouteData<Wms_storagerack>> GetIdleStorageRack(this ISqlSugarClient client,long reservoirAreaId)
        {
            Wms_storagerack storagerack = await client.Queryable<Wms_storagerack, Wms_inventorybox>(
                        (s, ib) => new object[] {
                               JoinType.Left,s.StorageRackId==ib.StorageRackId ,
                        }
                      ).Where((s, ib) => ib.InventoryBoxNo == null && s.ReservoirAreaId == reservoirAreaId && s.Status == StorageRackStatus.Normal)
                      .OrderBy((s, ib) => ib.Column, OrderByType.Desc ) //就近原则
                      .Select((s, ib) => s).FirstAsync();
            if (storagerack == null)
            {
                return RouteData<Wms_storagerack>.From(PubMessages.E2308_WCS_STORGERACK_FULL, $"");
            }
            return RouteData<Wms_storagerack>.From(storagerack);
        }
    }
}

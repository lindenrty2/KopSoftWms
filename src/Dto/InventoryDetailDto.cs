using System;
using System.Collections.Generic;
using System.Text;

namespace YL.Core.Dto
{
    public class InventoryDetailDto
    {
        /// <summary>
        /// 料箱Id
        /// </summary>
        public string InventoryBoxId { get; set; }

        /// <summary>
        /// 料格Id
        /// </summary>
        public string InventoryId { get; set; }

        /// <summary>
        /// 料格索引
        /// </summary>
        public int InventoryPosition { get; set; }

        /// <summary>
        /// 是否被锁定
        /// </summary>
        public bool IsLocked { get; set; }

        /// <summary>
        /// 物料Id
        /// </summary>
        public string MaterialId { get; set; }

        /// <summary>
        /// 物料唯一Id
        /// </summary>
        public string MaterialOnlyId { get; set; }
        /// <summary>
        /// 物料编号
        /// </summary>
        public string MaterialNo { get; set; }

        /// <summary>
        /// 物料名称
        /// </summary>
        public string MaterialName { get; set; }
        /// <summary>
        /// 入库详细Id
        /// </summary>
        public string StockInDetailId { get; set; }
        /// <summary>
        /// 出库详细Id
        /// </summary>
        public string StockOutDetailId { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderNo { get; set; }
        /// <summary>
        /// 计划数量
        /// </summary>
        public int PlanQty { get; set; }

        /// <summary>
        /// 已入库数量
        /// </summary>
        public int ComplateQty { get; set; }

        /// <summary>
        /// 料箱原始数量
        /// </summary>
        public int BeforeQty { get; set; }

        /// <summary>
        /// 当前入库数量
        /// </summary>
        public int Qty { get; set; }

    }
}

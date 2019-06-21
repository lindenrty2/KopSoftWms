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
        public long InventoryBoxId { get; set; }

        /// <summary>
        /// 料格Id
        /// </summary>
        public int InventoryId { get; set; }

        /// <summary>
        /// 物料Id
        /// </summary>
        public long MaterialId { get; set; }
 
        /// <summary>
        /// 计划数量
        /// </summary>
        public long PlanQty { get; set; }

        /// <summary>
        /// 已入库数量
        /// </summary>
        public long ComplateQty { get; set; }

        /// <summary>
        /// 料箱原始数量
        /// </summary>
        public long BeforeQty { get; set; }

        /// <summary>
        /// 当前入库数量
        /// </summary>
        public long Qty { get; set; }

    }
}

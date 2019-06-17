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
        /// 数量
        /// </summary>
        public int Qty { get; set; }

    }
}

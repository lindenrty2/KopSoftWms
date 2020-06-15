using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace YL.Core.Entity
{
    public partial class Wms_stockoutdetail_box
    {
        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
        public long DetailBoxId { get; set; }

        [SugarColumn(DefaultValue = "0", IsNullable = false)]
        public long StockOutDetailId { get; set; }

        /// <summary>
        /// 料箱任务Id
        /// </summary>
        [SugarColumn(DefaultValue = "0", IsNullable = false)]
        public long InventoryBoxTaskId { get; set; }
        /// <summary>
        /// 料箱Id
        /// </summary>
        [SugarColumn(DefaultValue = "0", IsNullable = false)]
        public long InventoryBoxId { get; set; }
        /// <summary>
        /// 操作数量
        /// </summary>
        [SugarColumn(DefaultValue = "0", IsNullable = false)]
        public int Position { get; set; }
        /// <summary>
        /// 计划数量
        /// </summary>
        [SugarColumn(DefaultValue = "0", IsNullable = false)]
        public int PlanQty { get; set; }
        /// <summary>
        /// 实际操作数量
        /// </summary>
        [SugarColumn(DefaultValue = "0", IsNullable = false)]
        public int Qty { get; set; }
        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(Length = 255, IsNullable = true)]
        public string Remark { get; set; }

        /// <summary>
        /// Desc:创建人
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public long? CreateBy { get; set; }

        /// <summary>
        /// Desc:创建时间
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public DateTime? CreateDate { get; set; }

        public Wms_stockoutdetail_box()
        {
            CreateDate = DateTime.Now;
        }

    }
}

using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace YL.Core.Entity
{
    public class Wms_inventoryboxTask
    {
        /// <summary>
        /// 入库任务Id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public long InventoryBoxTaskId { get; set; }
        

        /// <summary>
        /// Desc:库区Id
        /// Default:
        /// Nullable:True
        /// </summary> 
        [SugarColumn(/*, IsIdentity = true*/)]
        public long ReservoirareaId { get; set; }

        /// <summary>
        /// Desc:货架
        /// Default:
        /// Nullable:True
        /// </summary> 
        [SugarColumn(/*, IsIdentity = true*/)]
        public long StoragerackId { get; set; }

        /// <summary>
        /// Desc:料箱Id
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public long InventoryBoxId { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        [SugarColumn(Length = 4000, IsNullable = true)]
        public string Data { get; set; }

        /// <summary>
        /// Desc:审核人
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public long? OperaterId { get; set; }

        /// <summary>
        /// Desc:操作时间
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public DateTime? OperaterDate { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public long? ModifiedBy { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(IsEnableUpdateVersionValidation = true, IsNullable = true)]
        public DateTime? ModifiedDate { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(IsNullable = false, DefaultValue = "1")]
        public byte? Status { get; set; }
    }
}

using System;
using SqlSugar;

namespace YL.Core.Entity
{
    ///<summary>
    ///库存移动
    ///</summary>
    public partial class Wms_inventorymove
    {
        public Wms_inventorymove()
        {
            CreateDate = DateTime.Now;
            IsDel = Convert.ToByte("1");
        }

        /// <summary>
        /// Desc:库存移动主键
        /// Default:
        /// Nullable:False
        /// </summary>
        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
        public long InventorymoveId { get; set; }

        /// <summary>
        /// Desc:库存移动编号
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(Length = 30/*, IsIdentity = true*/)]
        public string InventorymoveNo { get; set; }

        /// <summary>
        /// Desc:原货架Id
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(IsNullable = true/*, IsIdentity = true*/)]
        public long SourceStoragerackId { get; set; }

        /// <summary>
        /// Desc:原料箱Id
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(IsNullable = true/*, IsIdentity = true*/)]
        public long SourceInventoryBoxId { get; set; }

        /// <summary>
        /// Desc:目标货架
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(IsNullable = true/*, IsIdentity = true*/)]
        public long AimStoragerackId { get; set; }

        /// <summary>
        /// Desc:目标料箱Id
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(IsNullable = true/*, IsIdentity = true*/)]
        public long AimInventoryBoxId { get; set; }

        /// <summary>
        /// Desc:状态
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int? Status { get; set; }

        /// <summary>
        /// Desc:备注
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(Length = 255, IsNullable = true)]
        public string Remark { get; set; }

        /// <summary>
        /// Desc:1 0
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn()]
        public int IsDel { get; set; }

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

        /// <summary>
        /// Desc:修改人
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public long? ModifiedBy { get; set; }

        /// <summary>
        /// Desc:修改时间
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public DateTime? ModifiedDate { get; set; }
    }
}
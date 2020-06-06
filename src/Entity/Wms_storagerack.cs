using SqlSugar;
using System;
using YL.Utils.Pub;

namespace YL.Core.Entity
{
    ///<summary>
    /// 货架(库位)
    ///</summary>
    public partial class Wms_storagerack
    {
        public Wms_storagerack()
        {
            this.IsDel = Convert.ToByte("1");
            this.CreateDate = DateTime.Now;
        }

        [SugarColumn(IsIgnore = true)]
        public string StorageRackIdStr { get { return StorageRackId.ToString(); } }

        /// <summary>
        /// Desc:货架Id
        /// Default:
        /// Nullable:False
        /// </summary>
        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
        public long StorageRackId { get; set; }

        /// <summary>
        /// Desc:货架编号
        /// Default:
        /// Nullable:False
        /// </summary>
        [SugarColumn(Length = 20, IsNullable = true/*, IsIdentity = true*/)]
        public string StorageRackNo { get; set; }

        /// <summary>
        /// Desc:货架名称
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(Length = 60, IsNullable = true/*, IsIdentity = true*/)]
        public string StorageRackName { get; set; }

        /// <summary>
        /// Desc:所属仓库
        /// Default:
        /// Nullable:False
        /// </summary>
        [SugarColumn(IsNullable = true/*, IsIdentity = true*/)]
        public long? WarehouseId { get; set; }

        /// <summary>
        /// Desc:所属库区
        /// Default:
        /// Nullable:False
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public long? ReservoirAreaId { get; set; }
        /// <summary>
        /// Desc:所属库区
        /// Default:
        /// Nullable:False
        /// </summary>
        [SugarColumn(Length = 60, IsNullable = true)]
        public string ReservoirAreaName { get; set; }

        /// <summary>
        /// 行
        /// </summary>
        [SugarColumn(ColumnName = "RowP")]
        public int Row { get; set; }

        /// <summary>
        /// 列
        /// </summary>
        [SugarColumn(ColumnName = "ColumnP")]
        public int Column { get; set; }

        /// <summary>
        /// 层
        /// </summary>
        [SugarColumn(ColumnName = "FloorP")]
        public int Floor { get; set; }

        /// <summary>
        /// Desc:备注
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(Length = 255, IsNullable = true)]
        public string Remark { get; set; }

        /// <summary>
        /// Desc:1 0
        /// Default:1
        /// Nullable:True
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int? IsDel { get; set; }

        /// <summary>
        /// 库位状态
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public StorageRackStatus Status { get; set; } = StorageRackStatus.Normal;
        /// <summary>
        /// Desc:创建人
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public long? CreateBy { get; set; }
        /// <summary>
        /// Desc:创建人
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(Length = 60,IsNullable = true)]
        public string CreateUser { get; set; }
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
        /// Desc:修改人
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(Length = 60, IsNullable = true)]
        public string ModifiedUser { get; set; }
        /// <summary>
        /// Desc:修改时间
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public DateTime? ModifiedDate { get; set; }
    }
}
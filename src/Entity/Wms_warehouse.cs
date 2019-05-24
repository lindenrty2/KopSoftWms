using System;
using SqlSugar;

namespace YL.Core.Entity
{
    ///<summary>
    ///
    ///</summary>
    public partial class Wms_warehouse
    {
        public Wms_warehouse()
        {
            this.IsDel = Convert.ToByte("1");
            this.CreateDate = DateTime.Now;
        }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public long WarehouseId { get; set; }

        /// <summary>
        /// Desc:仓库编号
        /// Default:
        /// Nullable:False
        /// </summary>
        [SugarColumn(Length = 20/*, IsIdentity = true*/)]
        public string WarehouseNo { get; set; }

        /// <summary>
        /// Desc:仓库名称
        /// Default:
        /// Nullable:False
        /// </summary>
        [SugarColumn(Length = 60/*, IsIdentity = true*/)]
        public string WarehouseName { get; set; }

        /// <summary>
        /// Desc:是否删除 1未删除  0删除
        /// Default:1
        /// Nullable:True
        /// </summary>
        [SugarColumn()]
        public byte IsDel { get; set; }

        /// <summary>
        /// Desc:备注
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
        /// Default:DateTime.Now
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
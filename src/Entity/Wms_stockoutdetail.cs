using SqlSugar;
using System;
using System.Linq;
using System.Text;

namespace YL.Core.Entity
{
    ///<summary>
    ///
    ///</summary>
    public partial class Wms_stockoutdetail
    {
        public Wms_stockoutdetail()
        {
            IsDel = Convert.ToByte("1");
            CreateDate = DateTime.Now;
        }

        /// <summary>
        /// Desc:主键
        /// Default:
        /// Nullable:False
        /// </summary>
        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
        public long StockOutDetailId { get; set; }

        /// <summary>
        /// Desc:出库单
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(IsNullable = true/*, IsIdentity = true*/)]
        public long? StockOutId { get; set; }

        /// <summary>
        /// Desc:仓库Id
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public long WarehouseId { get; set; }

        /// <summary>
        /// Desc:出库单Id
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string SubWarehouseEntryId { get; set; }

        /// <summary>
        /// Desc:唯一索引
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(Length = 80, IsNullable = true)]
        public string UniqueIndex { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int Status { get; set; }

        /// <summary>
        /// Desc:物料
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(IsNullable = true/*, IsIdentity = true*/)]
        public long? MaterialId { get; set; }

        /// <summary>
        /// Desc:物料编号
        /// Default:
        /// Nullable:False
        /// </summary>
        [SugarColumn(Length = 20, IsNullable = true)]
        public string MaterialOnlyId { get; set; }

        /// <summary>
        /// Desc:物料编号
        /// Default:
        /// Nullable:False
        /// </summary>
        [SugarColumn(Length = 20/*, IsIdentity = true*/)]
        public string MaterialNo { get; set; }

        /// <summary>
        /// Desc:物料名称
        /// Default:
        /// Nullable:False
        /// </summary>
        [SugarColumn(Length = 60/*, IsIdentity = true*/)]
        public string MaterialName { get; set; }

        /// <summary>
        /// Desc:计划数量
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int PlanOutQty { get; set; }

        /// <summary>
        /// Desc:实际数量
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int ActOutQty { get; set; }

        /// <summary>
        /// Desc:料箱Id
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(/*, IsIdentity = true*/)]
        public long InventoryBoxId { get; set; }

        /// <summary>
        /// Desc:料箱格Id
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(/*, IsIdentity = true*/)]
        public long InventoryId { get; set; }
         

        /// <summary>
        /// Desc:1 0
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn()]
        public int IsDel { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(Length = 255, IsNullable = true)]
        public string Remark { get; set; }

        /// <summary>
        /// 错误Id
        /// </summary>
        [SugarColumn(Length = 255, IsNullable = true)]
        public string ErrorId { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        [SugarColumn(Length = 512, IsNullable = true)]
        public string ErrorInfo { get; set; }

        /// <summary>
        /// Desc:创建人
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public long? CreateBy { get; set; }

        /// <summary>
        /// Desc:创建人名
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = true)]
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
        [SugarColumn(Length = 50, IsNullable = true)]
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
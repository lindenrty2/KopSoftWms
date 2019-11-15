using System;
using SqlSugar;
using YL.Utils.Extensions;
using YL.Utils.Pub;

namespace YL.Core.Entity
{
    ///<summary>
    ///
    ///</summary>
    public partial class Wms_stockindetail
    {
        public Wms_stockindetail()
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
        public long StockInDetailId { get; set; }

        /// <summary>
        /// Desc:入库单号
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(IsNullable = true/*, IsIdentity = true*/)]
        public long? StockInId { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(IsNullable = false , DefaultValue = "1")]
        public int Status { get; set; }

        /// <summary>
        /// Desc:仓库Id
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public long WarehouseId { get; set; }

        /// <summary>
        /// Desc:物料
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(IsNullable = true/*, IsIdentity = true*/)]
        public long? MaterialId { get; set; }
         
        /// <summary>
        /// Desc:产品编号
        /// Default:
        /// Nullable:False
        /// </summary>
        [SugarColumn(Length = 20, IsNullable = true)]
        public string MaterialOnlyId { get; set; }

        /// <summary>
        /// Desc:产品编号
        /// Default:
        /// Nullable:False
        /// </summary>
        [SugarColumn(Length = 20/*, IsIdentity = true*/)]
        public string MaterialNo { get; set; }

        /// <summary>
        /// Desc:产品名称
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
        public int PlanInQty { get; set; }

        /// <summary>
        /// Desc:实际数量
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int ActInQty { get; set; }


        /// <summary>
        /// Desc:1 0
        /// Default:1
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
        /// Desc:修改人名
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(Length = 50,IsNullable = true)]
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
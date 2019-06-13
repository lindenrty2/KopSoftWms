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
        [SugarColumn(IsPrimaryKey = true)]
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
        public byte Status { get; set; }

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
        /// Desc:计划数量
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(Length = 18, IsNullable = true)]
        public decimal? PlanInQty { get; set; }

        /// <summary>
        /// Desc:实际数量
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(Length = 18, IsNullable = true)]
        public decimal? ActInQty { get; set; }


        /// <summary>
        /// Desc:1 0
        /// Default:1
        /// Nullable:True
        /// </summary>
        [SugarColumn()]
        public byte IsDel { get; set; }

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
        /// Desc:修改时间
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public DateTime? ModifiedDate { get; set; }
    }

    /// <summary>
    /// 入库任务
    /// </summary>
    public class Wms_StockinTask
    {
        /// <summary>
        /// 入库任务Id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public long StockInTaskId { get; set; }
        
        /// <summary>
        /// Desc:入库详细Id
        /// Default:
        /// Nullable:True
        /// </summary> 
        [SugarColumn(/*, IsIdentity = true*/)]
        public long? StockInDetailId { get; set; }


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
        [SugarColumn(/*, IsIdentity = true*/)]
        public long? InventoryBoxId { get; set; }

        /// <summary>
        /// Desc:料箱格Id
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(/*, IsIdentity = true*/)]
        public long InventoryId { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public decimal? Qty { get; set; }

        /// <summary>
        /// Desc:审核人
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public long? OperaterId { get; set; }

        /// <summary>
        /// Desc:审核时间
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
        [SugarColumn(IsNullable = false, DefaultValue = "1")]
        public byte? Status { get; set; }

    }
}
using SqlSugar;
using System;
using System.Linq;
using System.Text;

namespace YL.Core.Entity
{
    ///<summary>
    ///
    ///</summary>
    public partial class Wms_material
    {
        public Wms_material()
        {
            IsDel = Convert.ToByte("1");
            CreateDate = DateTime.Now;
        }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public long MaterialId { get; set; }

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
        /// Desc:产品类型
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public long? MaterialType { get; set; }

        /// <summary>
        /// Desc:单位
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public long? Unit { get; set; }

        /// <summary>
        /// Desc:默认所属货架
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public long? StoragerackId { get; set; }

        /// <summary>
        /// Desc:默认所属库区
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public long? ReservoirAreaId { get; set; }

        /// <summary>
        /// Desc:默认所属仓库
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public long? WarehouseId { get; set; }

        /// <summary>
        /// Desc:安全库存
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(Length = 18, IsNullable = true)]
        public decimal? Qty { get; set; }

        /// <summary>
        /// Desc:有效期
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int ExpiryDate { get; set; }

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
}
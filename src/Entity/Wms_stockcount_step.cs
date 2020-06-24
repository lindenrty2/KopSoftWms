using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace YL.Core.Entity
{
    ///<summary>
    ///盘库任务分解后的工作项目
    ///</summary>
    public partial class Wms_stockcount_step
    {
        public Wms_stockcount_step()
        {
            IsDel = Convert.ToByte("1");
            CreateDate = DateTime.Now;
        }

        /// <summary>
        /// 工作项目Id
        /// </summary>
        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true, IsIdentity = true)]
        public long StockCountStepId { get; set; }

        /// <summary>
        /// 盘库任务Id
        /// </summary>
        [SugarColumn()]
        public long StockCountId { get; set; }

        /// <summary>
        /// 物料Id
        /// </summary>
        [SugarColumn()]
        public long MaterialId { get; set; }

        /// <summary>
        /// 产品唯一编号
        /// </summary>
        [SugarColumn(Length = 20, IsNullable = true)]
        public string MaterialOnlyId { get; set; }

        /// <summary>
        /// 产品编号
        /// </summary>
        [SugarColumn(Length = 20)]
        public string MaterialNo { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        [SugarColumn(Length = 60)]
        public string MaterialName { get; set; }

        /// <summary>
        /// 产品类型名称
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string MaterialTypeName { get; set; }

        /// <summary>
        /// 单位名称
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string UnitName { get; set; }

        /// <summary>
        /// 牌号
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string BrandNo { get; set; }

        /// <summary>
        /// 盘前数量
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string BeforeCount { get; set; }

        /// <summary>
        /// 盘点数量
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string StockCount { get; set; }

        /// <summary>
        /// 差异数量
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string DiffCount { get; set; }

        /// <summary>
        /// 包装数
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string PackageCount { get; set; }

        /// <summary>
        /// 是否标示
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string IsMark { get; set; }

        /// <summary>
        /// 是否混料
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string IsMixture { get; set; }

        /// <summary>
        /// 库位Id
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public long StorageRackId { get; set; }

        /// <summary>
        /// 库位编号
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string StorageRackNo { get; set; }

        /// <summary>
        /// 库位名称
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string StorageRackName { get; set; }

        /// <summary>
        /// 外观良好
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string IsExteriorPerfect { get; set; }

        /// <summary>
        /// Desc:
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
        /// 是否被删除
        /// </summary>
        [SugarColumn()]
        public int IsDel { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public long? CreateBy { get; set; }

        /// <summary>
        /// 创建人名
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = true)]
        public string CreateUser { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public long? ModifiedBy { get; set; }

        /// <summary>
        /// Desc:修改人名
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
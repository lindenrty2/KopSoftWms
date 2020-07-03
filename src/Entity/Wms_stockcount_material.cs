using System;
using System.Linq;
using System.Text;
using SqlSugar;
using YL.Utils.Pub;

namespace YL.Core.Entity
{
    ///<summary>
    ///盘库任务的计划项目(目前是物料列表)
    ///</summary>
    public partial class Wms_stockcount_material
    {
        public Wms_stockcount_material()
        {
            IsDel = Convert.ToByte("1");
            CreateDate = DateTime.Now;
        }

        /// <summary>
        /// 计划项目Id
        /// </summary>
        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
        public string StockCountMaterialId { get; set; }

        /// <summary>
        /// 盘库任务No
        /// </summary>
        [SugarColumn()]
        public string StockCountNo { get; set; }

        /// <summary>
        /// 物料Id
        /// </summary>
        [SugarColumn()]
        public string MaterialId { get; set; }

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
        /// 期末数量
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int PrevNumber { get; set; }

        /// <summary>
        /// 预计数量
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int ProjectedQty { get; set; }

        /// <summary>
        /// 盘库数量
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int StockCountQty { get; set; }

        /// <summary>
        /// Desc:备注
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(Length = 255, IsNullable = true)]
        public string Remark { get; set; }


        /// <summary>
        /// 状态
        /// </summary>
        [SugarColumn()]
        public int Status { get; set; } = (int)StockCountStatus.task_confirm;

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
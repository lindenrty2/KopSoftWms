using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace YL.Core.Entity
{
    ///<summary>
    ///盘库任务
    ///</summary>
    public partial class Wms_stockcount
    {
        public Wms_stockcount()
        {
            IsDel = Convert.ToByte("1");
            CreateDate = DateTime.Now;
        }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>
        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true, IsIdentity = true)]
        public long StockCountId { get; set; }

        /// <summary>
        /// 盘库编号
        /// </summary>
        [SugarColumn(Length = 32, IsNullable = true/*, IsIdentity = true*/)]
        public string StockCountNo { get; set; }

        /// <summary>
        /// 盘库日期
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public DateTime StockCountDate { get; set; }

        /// <summary>
        /// 计划项目(物料)数量
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int MaterialCount { get; set; }

        /// <summary>
        /// 分解任务总数
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int? StepTotalCount { get; set; } = null;

        /// <summary>
        /// 分解任务完成数量
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int? StepFinishCount { get; set; } = null;

        /// <summary>
        /// Mes任务Id
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public long? MesTaskId { get; set; }

        /// <summary>
        /// 仓库Id
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public long WarehouseId { get; set; }

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
using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace YL.Core.Entity
{
    ///<summary>
    ///
    ///</summary>
    public partial class Wms_stockin
    {
        public Wms_stockin()
        {
            IsDel = Convert.ToByte("1");
            CreateDate = DateTime.Now;
        }
        [SugarColumn(IsIgnore =true)]
        public string StockInIdStr {
            get {
                return StockInId.ToString();
            }
        }
        /// <summary>
        /// Desc:主键
        /// Default:
        /// Nullable:False
        /// </summary>
        [SugarColumn(ColumnName = "Id",IsPrimaryKey = true)]
        public long StockInId { get; set; }

        /// <summary>
        /// Desc:入库单号
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(Length = 32, IsNullable = true/*, IsIdentity = true*/)]
        public string StockInNo { get; set; }

        /// <summary>
        /// Desc:入库类型
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public long? StockInType { get; set; }

        /// <summary>
        /// Desc:入库类型名称
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = true)]
        public string StockInTypeName { get; set; }

        /// <summary>
        /// 仓库Id
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public long WarehouseId { get; set; }

        /// <summary>
        /// Mes任务Id
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public long? MesTaskId { get; set; }

        /// <summary>
        /// Desc:供应商
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public long? SupplierId { get; set; }

        /// <summary>
        /// Desc:订单号
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(Length = 32, IsNullable = true/*, IsIdentity = true*/)]
        public string OrderNo { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int StockInStatus { get; set; }

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
        /// Desc:修改人名
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
using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace YL.Core.Entity
{
    ///<summary>
    ///
    ///</summary>
    public partial class Wms_stockout
    {
        public Wms_stockout()
        {
            IsDel = Convert.ToByte("1");
            CreateDate = DateTime.Now;
        }

        [SugarColumn(IsIgnore = true)]
        public string StockOutIdStr
        {
            get
            {
                return StockOutId.ToString();
            }
        }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>
        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
        public long StockOutId { get; set; }

        /// <summary>
        /// Desc:出库单，系统自动生成
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(Length = 32, IsNullable = true/*, IsIdentity = true*/)]
        public string StockOutNo { get; set; }

        /// <summary>
        /// Desc:出库订单
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(Length = 32/*, IsIdentity = true*/)]
        public string OrderNo { get; set; }

        /// <summary>
        /// Desc:出库类型Id
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public long? StockOutType { get; set; }
        /// <summary>
        /// Desc:出库类型名称
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(Length = 50)]
        public string StockOutTypeName { get; set; }
        /// <summary>
        /// Desc:Mes任务Id
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public long? MesTaskId { get; set; }

        /// <summary>
        /// 仓库Id
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public long WarehouseId { get; set; }

        /// <summary>
        /// Desc:客户
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public long? CustomerId { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int? StockOutStatus { get; set; }

        /// <summary>
        /// Desc:备注
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(Length = 255, IsNullable = true)]
        public string Remark { get; set; }

        /// <summary>
        /// Desc:是否锁定
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public bool IsLocked { get; set; }
        
        /// <summary>
        /// Desc:是否已通知
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public bool? IsNotified { get; set; }

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
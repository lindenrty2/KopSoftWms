using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using SqlSugar;

namespace YL.Core.Entity
{
    ///<summary>
    ///储存箱格
    ///</summary>
    public partial class Wms_inventory
    {
        public Wms_inventory()
        {
            this.IsDel = Convert.ToByte("1");
            this.CreateDate = DateTime.Now;
        }

        /// <summary>
        /// Desc:储存箱格Id
        /// Default:
        /// Nullable:False
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public long InventoryId { get; set; }

        /// <summary>
        /// Desc:物料Id
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(IsNullable = false/*, IsIdentity = true*/)]
        public long? MaterialId { get; set; }
        /// <summary>
        /// Desc:产品编号
        /// Default:
        /// Nullable:False
        /// </summary>
        [SugarColumn(Length = 30, IsNullable = true)]
        public string MaterialOnlyId { get; set; }
        /// <summary>
        /// 物料编号
        /// </summary>
        [SugarColumn(Length = 30, IsNullable = true)]
        public string MaterialNo { get; set; }
        /// <summary>
        /// 物料名
        /// </summary>
        [SugarColumn(Length = 60)]
        public string MaterialName { get; set; }
        /// <summary>
        /// Desc:储存箱Id
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(IsNullable = true/*, IsIdentity = true*/)]
        public long InventoryBoxId { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = true)]
        public string OrderNo { get; set; }

        /// <summary>
        /// 储存箱格位置
        /// </summary> 
        [DefaultValue(1)]
        [SugarColumn(IsNullable = false)]
        public int Position { get; set; } = 1;

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(DefaultValue="0",IsNullable = false)]
        public int Qty { get; set; }

        /// <summary>
        /// Desc:备注
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(Length = 255, IsNullable = true)]
        public string Remark { get; set; }

        /// <summary>
        /// Desc: 是否锁定
        /// Default:
        /// Nullable:False
        /// </summary>
        [SugarColumn()]
        public bool IsLocked { get; set; }

        /// <summary>
        /// Desc:1 0
        /// Default:
        /// Nullable:False
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
        [SugarColumn(Length = 50 ,IsNullable = true)]
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
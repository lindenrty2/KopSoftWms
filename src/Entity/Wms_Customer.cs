using System;
using SqlSugar;

namespace YL.Core.Entity
{
    /// <summary>
    /// 客户
    /// </summary>
    public class Wms_Customer
    {
        public Wms_Customer()
        {
            this.IsDel = Convert.ToByte("1");
            this.CreateDate = DateTime.Now;
        }
        /// <summary>
        /// Desc:客户id
        /// Default:
        /// Nullable:False
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public long CustomerId { get; set; }

        /// <summary>
        /// Desc:客户编号
        /// Default:
        /// Nullable:False
        /// </summary>
        [SugarColumn(Length = 20, IsNullable = true/*, IsIdentity = true*/)]
        public string CustomerNo { get; set; }

        /// <summary>
        /// Desc:客户名称
        /// Default:
        /// Nullable:False
        /// </summary>
        [SugarColumn(Length = 60/*, IsIdentity = true*/)]
        public string CustomerName { get; set; }

        /// <summary>
        /// Desc:地址
        /// Default:
        /// Nullable:False
        /// </summary>
        [SugarColumn(Length = 200, IsNullable = true)]
        public string Address { get; set; }

        /// <summary>
        /// Desc:联系人
        /// Default:
        /// Nullable:False
        /// </summary>
        [SugarColumn(Length = 30, IsNullable = true)]
        public string CustomerPerson { get; set; }

        /// <summary>
        /// Desc:级别
        /// Default:
        /// Nullable:False
        /// </summary>
        [SugarColumn(Length = 10, IsNullable = true)]
        public string CustomerLevel { get; set; }

        /// <summary>
        /// Desc:邮件
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = true)]
        public string Email { get; set; }

        /// <summary>
        /// Desc:电话
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(Length = 20, IsNullable = true)]
        public string Tel { get; set; }

        /// <summary>
        /// Desc:是否删除 1未删除  0删除
        /// Default:1
        /// Nullable:True
        /// </summary>
        [SugarColumn()]
        public byte IsDel { get; set; }

        /// <summary>
        /// Desc:备注
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
        /// Default:DateTime.Now
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

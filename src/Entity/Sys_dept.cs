using SqlSugar;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace YL.Core.Entity
{
    ///<summary>
    ///
    ///</summary>
    public partial class Sys_dept
    {
        public Sys_dept()
        {
        }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)] 
        public long DeptId { get; set; }

        /// <summary>
        /// Desc:部门编号
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(Length = 20, IsNullable = true/*, IsIdentity = true*/)]
        public string DeptNo { get; set; }

        /// <summary>
        /// Desc:部门名称
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(Length = 50)]
        public string DeptName { get; set; }

        /// <summary>
        /// Desc:1未删除   0删除
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn()]
        public int IsDel { get; set; } = 1;

        /// <summary>
        /// Desc:备注
        /// Default:
        /// Nullable:False
        /// </summary>
        [SugarColumn(Length = 255, IsNullable = true)]
        public string Remark { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public long? CreateBy { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public DateTime? CreateDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public long? ModifiedBy { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary> 
        [SugarColumn(IsNullable = true)]
        public DateTime? ModifiedDate { get; set; }
    }
}
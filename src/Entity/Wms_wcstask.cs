using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using YL.Utils.Pub;

namespace YL.Core.Entity
{
    /// <summary>
    /// WCS指令
    /// </summary>
    public partial class Wms_wcstask
    {
        [SugarColumn(IsIgnore = true)]
        public string WcsTaskIdStr { get { return WcsTaskId.ToString(); } }

        /// <summary>
        /// WCS任务Id
        /// </summary>
        [SugarColumn(ColumnName = "Id",IsPrimaryKey = true)]
        public long WcsTaskId { get; set; }

        /// <summary>
        /// WCS任务类型
        /// </summary>
        [SugarColumn(IsNullable = false)]//Length = 2, 
        public WCSTaskTypes TaskType { get; set; } 

        /// <summary>
        /// 料箱Id
        /// </summary>
        [SugarColumn(IsNullable = true)] //Length = 11,
        public long? InventoryBoxId { get; set; }
        /// <summary>
        /// 料箱Id
        /// </summary>
        [SugarColumn(Length = 60, IsNullable = true)]
        public string InventoryBoxNo { get; set; }

        /// <summary>
        /// 料箱任务Id
        /// </summary>
        [SugarColumn(IsNullable = true)] //Length = 11,
        public long? InventoryBoxTaskId { get; set; }

        /// <summary>
        /// 任务描述
        /// </summary>
        [SugarColumn(ColumnName="DescP", Length =200)]
        public string Desc { get; set; }

        /// <summary>
        /// 指令参数
        /// </summary>
        [SugarColumn(Length=3000,IsNullable = true)]
        public string Params { get; set; }
        
        /// <summary>
        /// 操作状态
        /// </summary>
        [SugarColumn( IsNullable = false)]//Length = 2,
        public WCSTaskWorkStatus WorkStatus { get; set; } 

        /// <summary>
        /// 通知状态
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public WCSTaskNotifyStatus NotifyStatus { get; set; } 

        /// <summary>
        /// 是否删除
        /// </summary>
        [SugarColumn(DefaultValue = "1", IsNullable = false)]
        public int IsDel { get; set; }
        
        /// <summary>
        /// Desc:请求时间
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn()]
        public DateTime RequestDate { get; set; }
        /// <summary>
        /// Desc:请求人Id
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(IsNullable = true)] //Length = 11
        public long? RequestUserId { get; set; }
        /// <summary>
        /// Desc:请求人名
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(Length = 60, IsNullable = true)]
        public string RequestUser { get; set; }

        /// <summary>
        /// Desc:回复时间
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public DateTime? ResponseDate { get; set; }
        /// <summary>
        /// Desc:回复人Id
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(IsNullable = true)] //Length = 11
        public long? ResponseUserId { get; set; }
        /// <summary>
        /// Desc:回复人名
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(Length=60,IsNullable = true)]
        public string ResponseUser { get; set; }

        public Wms_wcstask()
        {
            TaskType = WCSTaskTypes.Unknow;
            WorkStatus = WCSTaskWorkStatus.Unknow;
            NotifyStatus = WCSTaskNotifyStatus.WaitRequest;
            IsDel = DeleteFlag.Normal;
            RequestDate = DateTime.Now;
        }
         

    }
}

﻿using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using YL.Utils.Pub;

namespace YL.Core.Entity
{
    public class Wms_mestask
    {
        /// <summary>
        /// MES任务Id
        /// </summary>
        public long MesTaskId { get; set; }
        /// <summary>
        /// MES任务类型
        /// </summary>
        public byte MesTaskType { get; set; }
        /// <summary>
        /// 单号
        /// </summary>
        [SugarColumn(Length = 30, DefaultValue = "", IsNullable = false)]
        public string WarehousingId { get; set; } = "";
        /// <summary>
        /// 入库类型
        /// </summary>
        [SugarColumn(Length = 30, DefaultValue = "", IsNullable = false)]
        public string WarehousingType { get; set; }
        /// <summary>
        /// 入库时间
        /// </summary>
        [SugarColumn(DefaultValue = "", IsNullable = false)]
        public DateTime WarehousingTime { get; set; }
        /// <summary>
        /// 生产令号
        /// </summary>
        [SugarColumn(Length = 30, DefaultValue = "", IsNullable = false)]
        public string ProductionPlanId { get; set; }
        /// <summary>
        /// 批次号
        /// </summary>
        [SugarColumn(Length = 50, DefaultValue = "", IsNullable = false)]
        public string BatchPlanId { get; set; }
        /// <summary>
        /// 作业区
        /// </summary>
        [SugarColumn(Length = 50, DefaultValue = "", IsNullable = false)]
        public string WorkAreaName { get; set; }
        /// <summary>
        /// 物料种类
        /// </summary>
        [SugarColumn(DefaultValue = "0", IsNullable = false)]
        public int SuppliesKinds { get; set; }
        /// <summary>
        /// 物料信息列表
        /// </summary>
        [SugarColumn(Length = 4096, DefaultValue = "", IsNullable = false)]
        public string SuppliesInfoJson { get; set; }

        /// <summary>
        /// 操作状态
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public byte WorkStatus { get; set; }

        /// <summary>
        /// 通知状态
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public byte NotifyStatus { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        [SugarColumn(DefaultValue = "1", IsNullable = false)]
        public byte IsDel { get; set; }
        
        /// <summary>
        /// Desc:创建时间
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// Desc:修改时间
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public DateTime? ModifiedDate { get; set; }

        public Wms_mestask()
        {
            IsDel = 1;
            CreateDate = DateTime.Now;
        }
         

    }
}

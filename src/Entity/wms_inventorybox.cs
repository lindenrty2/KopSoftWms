﻿using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using YL.Utils.Pub;

namespace YL.Core.Entity
{
    /// <summary>
    /// 储存箱(物料箱)
    /// </summary>
    public class Wms_inventorybox
    {
        public Wms_inventorybox()
        {
            this.IsDel = Convert.ToByte("1");
            this.CreateDate = DateTime.Now;
        }

        /// <summary>
        /// Desc:储存箱ID
        /// Default:
        /// Nullable:False
        /// </summary>
        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
        public long InventoryBoxId { get; set; }

        /// <summary>
        /// Desc:储存箱编号
        /// Default:
        /// Nullable:False
        /// </summary>
        [SugarColumn(Length = 32/*, IsIdentity = true*/)]
        public string InventoryBoxNo { get; set; }

        /// <summary>
        /// Desc:储存箱名
        /// Default:
        /// Nullable:False
        /// </summary>
        [SugarColumn(Length = 64/*, IsIdentity = true*/)]
        public string InventoryBoxName { get; set; }

        /// <summary>
        /// 仓库Id
        /// </summary>
        [SugarColumn(IsNullable = true/*, IsIdentity = true*/)]
        public long? WarehouseId { get; set; }

        /// <summary>
        /// 库区Id
        /// </summary>
        [SugarColumn(IsNullable = true/*, IsIdentity = true*/)]
        public long? ReservoirAreaId { get; set; }

        /// <summary>
        /// 库区名字
        /// </summary>
        [SugarColumn(Length = 64, IsNullable = true)]
        public string ReservoirAreaName { get; set; }

        /// <summary>
        /// Desc:货架Id
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(IsNullable = true/*, IsIdentity = true*/)]
        public long? StorageRackId { get; set; }

        /// <summary>
        /// 货架名字
        /// </summary>
        [SugarColumn(Length = 64,IsNullable =true)]
        public string StorageRackName { get; set; }

        /// <summary>
        /// 行
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int? Row { get; set; }
        /// <summary>
        /// 列
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int? Column { get; set; }
        /// <summary>
        /// 层
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int? Floor { get; set; }


        /// <summary>
        /// 格数
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public int Size { get; set; }

        /// <summary>
        /// 已使用格数
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public int UsedSize { get; set; } 

        /// <summary>
        /// Desc:备注
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(Length = 255, IsNullable = true)]
        public string Remark { get; set; }

        /// <summary>
        /// Desc:状态
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public InventoryBoxStatus Status { get; set; }

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
        /// Desc:修改人名
        /// Default:
        /// Nullable:True
        /// </summary>
        [SugarColumn(Length = 60, IsNullable = true)]
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
        [SugarColumn(Length = 60,IsNullable = true)]
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

using System;
using System.Collections.Generic;
using System.Text;

namespace YL.Core.Dto
{
    /// <summary>
    /// 库存
    /// </summary>
    public class OutsideInventoryDto
    {
        /// <summary>
        /// 对接用物料唯一Id
        /// </summary>
        public long MaterialId { get; set; }
        /// <summary>
        /// 物料编号
        /// </summary>
        public string MaterialNo { get; set; }
        /// <summary>
        /// 物料唯一Id
        /// </summary>
        public string MaterialOnlyId { get; set; }
        /// <summary>
        /// 物料名称
        /// </summary>
        public string MaterialName { get; set; }
        /// <summary>
        /// 库存数量
        /// </summary>
        public int Qty { get; set; }
        ///锁定状态    
        public bool IsLocked { get; set; }
        /// <summary>
        /// 货架Id
        /// </summary>
        public long StorageRackId { get; set; }
        /// <summary>
        /// 货架编号
        /// </summary>
        public string StorageRackNo { get; set; }
        /// <summary>
        /// 货架名
        /// </summary>
        public string StorageRackName { get; set; }
        /// <summary>
        /// 对接用料箱唯一Id
        /// </summary>
        public long InventoryBoxId { get; set; }
        /// <summary>
        /// 料箱编号
        /// </summary>
        public string InventoryBoxNo { get; set; }
        /// <summary>
        /// 料格位置(1~9)
        /// </summary>
        public int Position { get; set; }
        /// <summary>
        /// 创建用户
        /// </summary>
        public string CreateBy { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateDate { get; set; }
        /// <summary>
        /// 最后更新用户
        /// </summary>
        public string ModifiedBy { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public string ModifiedDate { get; set; }
    
    }

    /// <summary>
    /// 库存记录
    /// </summary>
    public class OutsideInventoryRecordDto : OutsideInventoryDto
    {
        public int AfterQty { get; set; }
    }
}

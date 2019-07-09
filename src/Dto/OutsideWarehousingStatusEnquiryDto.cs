using System;
using System.Collections.Generic;
using System.Text;

namespace YL.Core.Dto
{
    /// <summary>
    /// 入库状态查询参数
    /// </summary>
    public class OutsideWarehousingStatusEnquiryArg
    {
        /// <summary>
        /// 入库单编号
        /// </summary>
        public string WarehousingId { get; set; }
        /// <summary>
        /// 入库类型
        /// </summary>
        public string WarehousingType { get; set; }

    }

    /// <summary>
    /// 入库状态查询结果
    /// </summary>
    public class OutsideWarehousingStatusEnquiryResult
    {
        /// <summary>
        /// 是否成功收到命令
        /// </summary>
        public string Success { get; set; }
        /// <summary>
        /// 错误编号
        /// </summary>
        public string ErrorId { get; set; }
        /// <summary>
        /// 错误内容
        /// </summary>
        public string ErrorInfo { get; set; }
        /// <summary>
        /// 是否入库完成
        /// </summary>
        public bool IsNormalWarehousing { get; set; }
        /// <summary>
        /// 入库单编号
        /// </summary>
        public string WarehousingId { get; set; }
        /// <summary>
        /// 入库类型
        /// </summary>
        public string WarehousingType { get; set; }
       
        /// <summary>
        /// 入库状态信息
        /// </summary>
        public WarehousingStatusInfo[] WarehousingStatusInfoList { get; set; }

    }

    public class WarehousingStatusInfo
    {

        /// <summary>
        /// 物资编号/图号
        /// </summary>
        public string SuppliesId { get; set; }
        /// <summary>
        /// 入库当前阶段
        /// </summary>
        public string WarehousingStep { get; set; }
        /// <summary>
        /// 是否入库完成
        /// </summary>
        public bool IsNormalWarehousing { get; set; }
        /// <summary>
        /// 入库完成时间
        /// </summary>
        public string WarehousingFinishTime { get; set; }
        /// <summary>
        /// 仓库编号
        /// </summary>
        public string WarehouseId { get; set; }
        /// <summary>
        /// 仓库名称
        /// </summary>
        public string WarehouseName { get; set; }
        /// <summary>
        /// 具体库区
        /// </summary>
        public string WarehousePosition { get; set; }
        /// <summary>
        /// 货架位置
        /// </summary>
        public string StorageRackPosition { get; set; }
        /// <summary>
        /// 料箱编号
        /// </summary>
        public string InventoryBoxNo { get; set; }
        /// <summary>
        /// 料格序号
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// 更新后库存
        /// </summary>
        public int RefreshStock { get; set; }
    }
}

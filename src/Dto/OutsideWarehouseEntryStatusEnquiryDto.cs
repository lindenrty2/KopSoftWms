using System;
using System.Collections.Generic;
using System.Text;

namespace YL.Core.Dto
{
    /// <summary>
    /// 出库状态查询参数
    /// </summary>
    public class OutsideWarehouseEntryStatusEnquiryArg
    {
        /// <summary>
        /// 出库单编号
        /// </summary>
        public string WarehouseEntryId { get; set; }

        /// <summary>
        /// 出库类型
        /// </summary>
        public string WarehouseEntryType { get; set; }

    }

    /// <summary>
    /// 出库状态查询结果
    /// </summary>
    public class OutsideWarehouseEntryStatusEnquiryResult
    {
        /// <summary>
        /// 是否正确收到命令
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
        /// 是否出库完成
        /// </summary>
        public bool IsNormalWarehouseEntry { get; set; }
        /// <summary>
        /// 出库单编号
        /// </summary>
        public string WarehouseEntryId { get; set; }

        public string WarehouseEntryStatusInfoList { get; set; } //WarehouseEntryStatusInfo[]


    }

    public class WarehouseEntryStatusInfo
    {

        public string SuppliesId { get; set; }
        /// <summary>
        /// 出库当前阶段
        /// </summary>
        public string WarehouseEntryStep { get; set; }

        /// <summary>
        /// 是否出库完成
        /// </summary>
        public string IsNormalWarehouseEntry { get; set; }

        /// <summary>
        /// 出库完成时间
        /// </summary>
        public string WarehouseEntryFinishTime { get; set; }

        /// <summary>
        /// 作业区
        /// </summary>
        public string WorkAreaName { get; set; }

        /// <summary>
        /// 更新后库存
        /// </summary>
        public int RefreshStock { get; set; }


    }
}

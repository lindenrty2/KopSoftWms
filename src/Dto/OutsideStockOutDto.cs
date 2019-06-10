using System;
using System.Collections.Generic;
using System.Text;

namespace YL.Core.Dto
{
    public class OutsideStockOutDto
    {
        /// <summary>
        /// 出库单编号
        /// </summary>
        public string WarehouseEntryId { get; set; }
        /// <summary>
        /// 出库类型
        /// </summary>
        public string WarehouseEntryType { get; set; }
        /// <summary>
        /// 出库时间
        /// </summary>
        public string WarehouseEntryTime { get; set; }
        /// <summary>
        /// 生产令号
        /// </summary>
        public string ProductionPlanId { get; set; }
        /// <summary>
        /// 批次号
        /// </summary>
        public string BatchPlanId { get; set; }
        /// <summary>
        /// 作业区
        /// </summary>
        public string WorkAreaName { get; set; }
        /// <summary>
        /// 工位号
        /// </summary>
        public string WorkStationId { get; set; }
        /// <summary>
        /// 物料种类
        /// </summary>
        public string SuppliesKinds { get; set; }
        /// <summary>
        /// 物料信息列表
        /// </summary>
        public OutsideMaterialDto[] SuppliesInfoList { get; set; }
         
    }


    public class OutSideStockOutResult
    {
        /// <summary>
        /// 消息接收成功与否
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// 出库单编号
        /// </summary>
        public bool WarehousingId { get; set; }
        /// <summary>
        /// 错误编号
        /// </summary>
        public String ErrorId { get; set; }
        /// <summary>
        /// 错误内容
        /// </summary>
        public String ErrorInfo { get; set; }

    }
}

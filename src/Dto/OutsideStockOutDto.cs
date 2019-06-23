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
        public int SuppliesKinds { get; set; }
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


    public class OutSideStockOutResponse
    {

        /// <summary>
        /// 出库单编号
        /// </summary>
        public string WarehouseEntryId { get; set; }
        /// <summary>
        /// 出库完成时间
        /// </summary>
        public string WarehouseEntryFinishTime { get; set; }
        /// <summary>
        /// 作业区
        /// </summary>
        public string WorkAreaName { get; set; }
        /// <summary>
        /// 错误编号
        /// </summary>
        public string ErrorId { get; set; }
        /// <summary>
        /// 错误内容
        /// </summary>
        public string ErrorInfo { get; set; }
        /// <summary>
        /// 物料种类
        /// </summary>
        public int SuppliesKinds { get; set; }
        /// <summary>
        /// 物料信息
        /// </summary>
        public OutsideMaterialResult[] SuppliesInfoList { get; set; }

    }


    public class OutSideStockOutResponseResult
    {
        /// <summary>
        /// 入库单编号
        /// </summary>
        public string WarehouseEntryId { get; set; }
        /// <summary>
        /// 是否正常接收
        /// </summary>
        public bool IsNormalExecution { get; set; }
        /// <summary>
        /// 错误编号
        /// </summary>
        public string ErrorId { get; set; }

    }
}

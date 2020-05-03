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
        public string SuppliesInfoList { get; set; } //OutsideMaterialDto[]

        public static OutsideStockOutDto Create(
            String warehouseEntryid,
            String warehouseEntryType,
            String warehouseEntryTime,
            String productionPlanId,
            String batchPlanId,
            String workStationId,
            String workAreaName,
            int suppliesKinds,
            String suppliesInfoList)
        {
            OutsideStockOutDto data = new OutsideStockOutDto()
            {
                WarehouseEntryId = warehouseEntryid,
                WarehouseEntryType = warehouseEntryType,
                WarehouseEntryTime = warehouseEntryTime,
                ProductionPlanId = productionPlanId,
                BatchPlanId = batchPlanId,
                WorkAreaName = workAreaName,
                WorkStationId = workStationId,
                SuppliesKinds = suppliesKinds,
                SuppliesInfoList = suppliesInfoList

            };
            return data;
        }
    }


    public class OutsideStockOutResult
    {
        /// <summary>
        /// 消息接收成功与否
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// 出库单编号
        /// </summary>
        public string WarehouseEntryId { get; set; }
        /// <summary>
        /// 错误编号
        /// </summary>
        public String ErrorId { get; set; }
        /// <summary>
        /// 错误内容
        /// </summary>
        public String ErrorInfo { get; set; }

    }


    public class OutsideStockOutResponse
    {

        /// <summary>
        /// 出库单编号
        /// </summary>
        public string WarehouseEntryId { get; set; }
        /// <summary>
        /// 仓库数量
        /// </summary>
        public int WarehouseEntryFinishCount { get; set; } 
        /// <summary>
        /// 各仓库信息
        /// </summary>
        public string WarehouseEntryFinishList { get; set; } //OutsideStockInResponseWarehouse[]

    }


    public class OutsideStockOutResponseWarehouse
    {
        /// <summary>
        /// 入库完成时间
        /// </summary>
        public string WarehouseEntryFinishTime { get; set; }
        /// <summary>
        /// 仓库编号
        /// </summary>
        public string WarehouseId { get; set; }
        /// <summary>
        /// 仓库名称
        /// </summary>
        public string WarehouseName { get; set; } 
        /// <summary>
        /// 作业区
        /// </summary>
        public string WorkAreaName { get; set; }
        /// <summary>
        /// 物料数量
        /// </summary>
        public int SuppliesKinds { get; set; }
        /// <summary>
        /// 物料信息
        /// </summary>
        public List<OutsideMaterialResult> SuppliesInfoList { get; set; } = new List<OutsideMaterialResult>();
    }




    public class OutsideStockOutResponseResult
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

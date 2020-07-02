using System;
using System.Collections.Generic;
using System.Text;

namespace YL.Core.Dto
{
    /// <summary>
    /// 出库任务用参数
    /// </summary>
    public class OutsideStockOutRequestDto
    {
        /// <summary>
        /// MES任务Id
        /// </summary>
        public long MesTaskId { get; set; } 
        /// <summary>
        /// 出库Id
        /// </summary>
        public string WarehousingId { get; set; }
        /// <summary>
        /// 出库类型
        /// </summary>
        public string WarehousingType { get; set; }
        /// <summary>
        /// 出库时间
        /// </summary>
        public string WarehousingTime { get; set; }
        /// <summary>
        /// 生产令号
        /// </summary>
        public string OrderNo { get; set; }
        /// <summary>
        /// 工作令号
        /// </summary>
        public string WorkNo { get; set; }
        /// <summary>
        /// 齐套批次号
        /// </summary>
        public string BatchNumber { get; set; }
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
        /// 物料列表
        /// </summary>
        public Wms_WarehouseEntryMaterialInventoryDto[] MaterialList { get; set; } 
        /// <summary>
        /// 对接用出库Id
        /// </summary>
        public long? StockOutId { get; set; }
        /// <summary>
        /// 对接用出库编号
        /// </summary>
        public string StockOutNo { get; set; }
        /// <summary>
        /// 仓库Id
        /// </summary>
        public long WarehouseId { get; set; }
    }


    public class OutsideStockOutRequestResult
    {
        public long StockOutId { get; set; }
        public string StockOutNo { get; set; }

    }
}

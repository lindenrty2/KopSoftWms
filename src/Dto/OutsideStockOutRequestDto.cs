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
        public string WarehousingId { get; set; }
        /// <summary>
        /// 入库类型
        /// </summary>
        public string WarehousingType { get; set; }
        /// <summary>
        /// 入库时间
        /// </summary>
        public string WarehousingTime { get; set; }
        /// <summary>
        /// 生产令号
        /// </summary>
        public string OrderNo { get; set; }
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
        public Wms_MaterialInventoryDto[] MaterialList { get; set; }
    }


    public class OutsideStockOutRequestResult
    {
        public long StockOutId { get; set; }
        public string StockOutNo { get; set; }

    }
}

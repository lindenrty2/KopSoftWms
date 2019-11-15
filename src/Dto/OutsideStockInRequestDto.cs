using System;
using System.Collections.Generic;
using System.Text;

namespace YL.Core.Dto
{
    public class OutsideStockInRequestDto
    {
        /// <summary>
        /// MES任务Id
        /// </summary>
        public long MesTaskId { get; set; }
        /// <summary>
        /// 入库单编号
        /// </summary>
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
        /// 物料列表
        /// </summary>
        public Wms_MaterialInventoryDto[] MaterialList { get; set; }


    }

    public class OutsideStockInRequestResult
    {
        public long StockInId { get; set; }
        public string StockInNo { get; set; }

    }
}

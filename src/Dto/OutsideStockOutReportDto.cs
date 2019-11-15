using System;
using System.Collections.Generic;
using System.Text;
using YL.Utils.Pub;

namespace YL.Core.Dto
{
    public class OutsideStockOutReportDto
    {
        /// <summary>
        /// 对接用出库唯一Id
        /// </summary>
        public long StockOutId { get; set; }
        /// <summary>
        /// 出库状态
        /// </summary>
        public StockOutStatus StockOutStatus { get; set; }

        /// <summary>
        /// 详细进度列表
        /// </summary>
        public OutsideStockOutReportDetail[] Details { get; set; }

    }

    public class OutsideStockOutReportDetail
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
        /// 计划出库数量
        /// </summary>
        public int PlanOutQty { get; set; }

        /// <summary>
        /// 完成出库数量
        /// </summary>
        public int ActOutQty { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public StockOutStatus Status { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 最后修改人
        /// </summary>
        public string ModifiedBy { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public string ModifiedDate { get; set; }

        /// <summary>
        /// 提示信息
        /// </summary>
        public string Message { get; set; }

    }
}

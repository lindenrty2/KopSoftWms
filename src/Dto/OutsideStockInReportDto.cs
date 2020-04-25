using System;
using System.Collections.Generic;
using System.Text;
using YL.Utils.Pub;

namespace YL.Core.Dto
{
    public class OutsideStockInReportDto
    {
        /// <summary>
        /// 对接用出库唯一Id
        /// </summary>
        public long StockInId { get; set; }
        /// <summary>
        /// 出库状态
        /// </summary>
        public StockInStatus StockInStatus { get; set; }
        /// <summary>
        /// 详细进度列表
        /// </summary>
        public OutsideStockInReportDetail[] Details { get; set; }


    }

    public class OutsideStockInReportDetail
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
        public int PlanInQty { get; set; }

        /// <summary>
        /// 完成出库数量
        /// </summary>
        public int ActInQty { get; set; }

        /// <summary>
        /// 出库后数量
        /// </summary>
        public int AfterQty { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public StockInStatus Status { get; set; }

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
        public string ErrorId { get; set; }
        /// <summary>
        /// 提示信息
        /// </summary>
        public string ErrorInfo { get; set; }

    }

}

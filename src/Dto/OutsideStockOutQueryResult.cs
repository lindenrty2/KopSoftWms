using System;
using System.Collections.Generic;
using System.Text;
using YL.Utils.Pub;

namespace YL.Core.Dto
{
    public class OutsideStockOutQueryResult
    {
        /// <summary>
        /// 对接用出库唯一Id
        /// </summary>
        public long StockOutId { get; set; }
        /// <summary>
        /// 出库编号
        /// </summary>
        public string StockOutNo { get; set; }
        /// <summary>
        /// 生产令号
        /// </summary>
        public string OrderNo { get; set; }
        /// <summary>
        /// 出库类型名
        /// </summary>
        public string StockOutTypeName { get; set; }
        /// <summary>
        /// MES任务Id
        /// </summary>
        public long MesTaskId { get; set; }
        /// <summary>
        /// 出库状态
        /// </summary>
        public StockOutStatus StockOutStatus { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 最后修改人
        /// </summary>
        public string ModifiedBy { get; set; }
        /// <summary>
        /// 最后修改人名
        /// </summary>
        public string ModifiedUser { get; set; }
        /// <summary>
        /// 最后修改时间
        /// </summary>
        public string ModifiedDate { get; set; }
        /// <summary>
        /// 详细进度列表
        /// </summary>
        public OutsideStockOutQueryResultDetail[] Details { get; set; }

    }

    public class OutsideStockOutQueryResultDetail
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
        /// 取值范围同StockOutStatus
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
        /// 最后修改人名
        /// </summary>
        public string ModifiedUser { get; set; }
        /// <summary>
        /// 最后修改时间
        /// </summary>
        public string ModifiedDate { get; set; }


    }
}

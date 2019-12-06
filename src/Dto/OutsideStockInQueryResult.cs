using System;
using System.Collections.Generic;
using System.Text;
using YL.Utils.Pub;

namespace YL.Core.Dto
{
    public class OutsideStockInQueryResult
    {
        public string WarehouseName { get; set; }
        /// <summary>
        ///  对接用入库唯一Id
        /// </summary>
        public string StockInId { get; set; }
        /// <summary>
        /// 出库编号
        /// </summary>
        public string StockInNo { get; set; }
        /// <summary>
        /// 生产令号
        /// </summary>
        public string OrderNo { get; set; }
        /// <summary>
        /// 出库类型名
        /// </summary>
        public string StockInTypeName { get; set; }
        /// <summary>
        /// MES任务Id
        /// </summary>
        public string MesTaskId { get; set; }
        /// <summary>
        /// 入库状态
        /// </summary>
        public StockInStatus StockInStatus { get; set; }
        /// <summary>
        /// 状态值
        /// </summary>
        public int StockInStatusValue { get { return (int)StockInStatus; } }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateBy { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateDate { get; set; }
        /// <summary>
        /// 最后修改人
        /// </summary>
        public string ModifiedBy { get; set; }
        /// <summary>
        /// 最后修改时间
        /// </summary>
        public string ModifiedDate { get; set; }
        /// <summary>
        /// 详细进度列表
        /// </summary>
        public object[] Details { get; set; }
    }

    public class OutsideStockInQueryResultDetail
    {
        /// <summary>
        /// 入库详细Id
        /// </summary>
        public string StockInDetailId { get; set; }
        /// <summary>
        /// 对接用物料唯一Id
        /// </summary>
        public string MaterialId { get; set; }
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
        /// 计划入库数量
        /// </summary>
        public int PlanInQty { get; set; }
        /// <summary>
        /// 完成入库数量
        /// </summary>
        public int ActInQty { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public StockInStatus Status { get; set; }
        /// <summary>
        /// 状态值
        /// </summary>
        public int StatusValue { get { return (int)Status; } }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateBy { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateDate { get; set; }
        /// <summary>
        /// 最后修改人
        /// </summary>
        public string ModifiedBy { get; set; }
        /// <summary>
        /// 最后修改时间
        /// </summary>
        public string ModifiedDate { get; set; }


    }
}

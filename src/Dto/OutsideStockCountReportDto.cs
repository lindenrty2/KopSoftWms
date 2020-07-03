using System;
using System.Collections.Generic;
using System.Text;
using YL.Core.Entity;
using YL.Utils.Pub;

namespace YL.Core.Dto
{

    /// <summary>
    /// 盘库结果回报
    /// </summary>
    public class OutsideStockCountReportDto
    {
        /// <summary>
        /// 盘库任务编号
        /// </summary>
        public string StockCountNo { get; set; }
        /// <summary>
        /// 盘库整体完成日期
        /// </summary>
        public string CompleteDate { get; set; }
        /// <summary>
        /// 物料列表
        /// </summary>
        public OutsideStockCountReportMaterialDto[] MaterialList { get; set; }


    }

     ///<summary>
    ///盘库结果回报物料信息
    ///</summary>
    public partial class OutsideStockCountReportMaterialDto
    {
        public OutsideStockCountReportMaterialDto()
        { 
        }
         

        /// <summary>
        /// 产品唯一编号
        /// </summary> 
        public string MaterialOnlyId { get; set; }

        /// <summary>
        /// 产品编号
        /// </summary> 
        public string MaterialNo { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary> 
        public string MaterialName { get; set; }

        /// <summary>
        /// 产品类型名称
        /// </summary> 
        public string MaterialType { get; set; }

        /// <summary>
        /// 单位名称
        /// </summary> 
        public string Unit { get; set; }

        /// <summary>
        /// 期末数量
        /// </summary>
        public int PrevNumber { get; set; }

        /// <summary>
        /// 盘前数量
        /// </summary> 
        public int BeforeCount { get; set; }

        /// <summary>
        /// 盘库数量
        /// </summary> 
        public int StockCount { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
         
        /// <summary>
        /// 状态
        /// </summary> 
        public int Status { get; set; } = (int)StockCountStatus.task_confirm;

        /// <summary>
        /// 盘点人姓名
        /// </summary> 
        public string StockCountUser { get; set; }

        /// <summary>
        /// 盘点时间
        /// </summary> 
        public DateTime? StockCountDate { get; set; }
         
    }
}

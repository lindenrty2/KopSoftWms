using System;
using System.Collections.Generic;
using System.Text;
using YL.Core.Entity;

namespace YL.Core.Dto
{

    public class OutsideStockCountReportDto
    {
        /// <summary>
        /// 盘库任务编号
        /// </summary>
        string StockCountNo { get; set; }
        /// <summary>
        /// 盘库整体完成日期
        /// </summary>
        string CompleteDate { get; set; }
        /// <summary>
        /// 物料列表
        /// </summary>
        Wms_stockcount_step[] MaterialList { get; set; }


    }
}

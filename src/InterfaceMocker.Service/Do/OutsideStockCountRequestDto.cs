using System;
using System.Collections.Generic;
using System.Text;

namespace YL.Core.Dto
{
    public class OutsideStockCountRequestDto
    {
        /// <summary>
        /// 仓库No
        /// </summary>
        public string WarehouseId { get; set; }

        public long MesTaskId { get; set; }
        /// <summary>
        /// 盘库任务编号
        /// </summary>
        public string StockCountNo { get; set; }
        /// <summary>
        /// 盘库计划日期
        /// </summary>
        public string PlanDate { get; set; }

        /// <summary>
        /// 物料列表
        /// </summary>
        public OutsideStockCountMaterial[] MaterialList { get; set; }


    }

    public class OutsideStockCountMaterial
    {
        /// <summary>
        /// 物资编号
        /// </summary>
        public string MaterialNo { get; set; }
        /// <summary>
        /// 物资唯一编号
        /// </summary>
        public string MaterialOnlyId { get; set; }
        /// <summary>
        /// 物资名称
        /// </summary>
        public string MaterialName { get; set; }
        /// <summary>
        /// 型号规格
        /// </summary>
        public string MaterialType { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }
    }

     
}

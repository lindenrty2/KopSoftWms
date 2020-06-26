using System;
using System.Collections.Generic;
using System.Text;
using YL.Utils.Pub;

namespace YL.Core.Dto
{
    public class OutsideStockCountRequestDto
    {
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

        public string WarehouseId { get; set; }
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
        public string MaterialTypeName { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        public string UnitName { get; set; }
        /// <summary>
        /// 预计数量
        /// </summary> 
        public int ProjectedQty { get; set; }

        /// <summary>
        /// 盘库数量
        /// </summary> 
        public int StockCountQty { get; set; }

        /// <summary>
        /// Desc:备注
        /// Default:
        /// Nullable:True
        /// </summary> 
        public string Remark { get; set; }


        /// <summary>
        /// 状态
        /// </summary> 
        public int Status { get; set; } = (int)StockCountStatus.task_confirm;
    }

     
}

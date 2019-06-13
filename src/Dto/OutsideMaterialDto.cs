using System;
using System.Collections.Generic;
using System.Text;

namespace YL.Core.Dto
{
    /// <summary>
    /// 外部物料信息
    /// </summary>
    public class OutsideMaterialDto
    {
        /// <summary>
        /// 物料唯一编号
        /// </summary>
        public string SuppliesOnlyId { get; set; }
        /// <summary>
        /// 物料编号/图号
        /// </summary>
        public string SuppliesId { get; set; }
        /// <summary>
        /// 物料名称
        /// </summary>
        public string SuppliesName { get; set; }
        /// <summary>
        /// 型号
        /// </summary>
        public string SuppliesType { get; set; }
        /// <summary>
        /// 物料数量
        /// </summary>
        public int SuppliesNumber { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }

    }

    /// <summary>
    /// 物料操作结果
    /// </summary>
    public class OutsideMaterialResult
    {
        /// <summary>
        /// 物料编号/图号
        /// </summary>
        public string SuppliesId { get; set; }
        /// <summary>
        /// 物料名称
        /// </summary>
        public string SuppliesName { get; set; }
        /// <summary>
        /// 物料数量
        /// </summary>
        public string SuppliesNumber { get; set; }
        /// <summary>
        /// 更新后库存
        /// </summary>
        public string RefreshStock { get; set; }


    }
}

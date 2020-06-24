using System;
using System.Collections.Generic;
using System.Text;

namespace YL.Core.Dto
{
    public class Wms_MaterialDto
    {
        public bool IsSelected { get; set; }
        /// <summary>
        /// 对接用唯一物料定义Id
        /// </summary>
        public string MaterialId { get; set; }
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
        /// 产品类型
        /// </summary>
        public string MaterialType { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }

    }

    public class Wms_MaterialInventoryDto : Wms_MaterialDto
    {
        /// <summary>
        /// 数量
        /// </summary>
        public int Qty { get; set; }
    }

    public class Wms_WarehouseEntryMaterialInventoryDto : Wms_MaterialInventoryDto
    {

        /// <summary>
        /// 子(出库/入库)单号
        /// </summary>
        public string SubWarehouseEntryId { get; set; } 
    }

    public class Wms_WarehousingMaterialInventoryDto : Wms_MaterialInventoryDto
    {

        /// <summary>
        /// 子(出库/入库)单号
        /// </summary>
        public string SubWarehousingId { get; set; }
    }

}

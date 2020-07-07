using System;
using System.Collections.Generic;
using System.Text;

namespace YL.Core.Dto
{

    public class OutsideStockCountDto_MES
    {
        /// <summary>
        /// 盘点单号
        /// </summary>
        public string StockInventoryId { get; set; }
        /// <summary>
        /// 年份
        /// </summary>
        public string Year { get; set; }
        /// <summary>
        /// 月份
        /// </summary>
        public string Month { get; set; }
        /// <summary>
        /// 盘点单号
        /// </summary>
        public string WarehouseID { get; set; }

        /// <summary>
        /// 物料信息
        /// </summary>
        public string SuppliesInfoList { get; set; }
    }

    public class OutsideStockCountResultDto_MES {
        /// <summary>
        /// 盘点单号	
        /// </summary>
        public string StockInventoryId { get; set; }
        /// <summary>
        /// 消息接收成功与否
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// 错误编号
        /// </summary>
        public string ErrorId { get; set; }
        /// <summary>
        /// 错误内容
        /// </summary>
        public string ErrorInfo { get; set; }      
    }


    public class OutsideStockCountMaterialDto_MES
    {
        /// <summary>
        /// 物料唯一编号
        /// </summary>
        public string SuppliesOnlyId { get; set; }
        /// <summary>
        /// 物料编号
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
        /// 单位
        /// </summary>
        public string Unit { get; set; }
        /// <summary>
        /// 期末数量
        /// </summary>
        public string PrevNumber { get; set; }

        /// <summary>
        /// 盘点数量 实际的物料数据，如果物料在多个料箱，则体现多个料箱的总额和
        /// </summary>
        public string InventoryNumber { get; set; }

        /// <summary>
        /// 库位编号 如果是多个料箱，则用分号来隔开
        /// </summary>
        public string StoreId { get; set; }

        /// <summary>
        ///  库位名称 如果是多个料箱，则用分号来隔开
        /// </summary>
        public string StoreName { get; set; }

        /// <summary>
        ///  保管员
        /// </summary>
        public string StoreMan { get; set; }

        /// <summary>
        ///  盘点时间
        /// </summary>
        public String StockCountDate { get; set; }

    }

  

}

using System;
using System.Collections.Generic;
using System.Text;

namespace YL.Core.Dto
{
    /// <summary>
    /// 物料库存查询参数
    /// </summary>
    public class OutsideMaterialStockEnquiryArg
    {
        /// <summary>
        /// 物资编号/图号
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
        public string SuppliesUnit { get; set; }

        public static OutsideMaterialStockEnquiryArg Create(string suppliesid, string suppliesname, string suppliestype, string suppliesunit)
        {
            return new OutsideMaterialStockEnquiryArg()
            {
                SuppliesId = suppliesid,
                SuppliesName = suppliesname,
                SuppliesType = suppliestype,
                SuppliesUnit = suppliesunit
            };
        }
    }

    /// <summary>
    /// 物料库存查询结果
    /// </summary>
    public class OutsideMaterialStockEnquiryResult
    {
        /// <summary>
        /// 消息接收成功与否 
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 错误代码
        /// </summary>
        public string ErrorID { get; set; }

        /// <summary>
        /// 物资编号/图号
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
        public string SuppliesUnit { get; set; }

        /// <summary>
        /// 物料信息
        /// </summary>
        public OutsideMaterialStockEnquiryItem[] MaterialStockInfo { get; set; }
       

    }


    /// <summary>
    /// 物料库存信息
    /// </summary>
    public  class OutsideMaterialStockEnquiryItem
    {
        /// <summary>
        /// 账面库存
        /// </summary>
        public string PaperStock { get; set; }
        /// <summary>
        /// 结余库存
        /// </summary>
        public string BalanceStock { get; set; }

        /// <summary>
        /// 仓库编号
        /// </summary>
        public string WarehouseId { get; set; }

        /// <summary>
        /// 仓库名称
        /// </summary>
        public string WarehouseName { get; set; }

        /// <summary>
        /// 具体库区
        /// </summary>
        public string WarehousePosition { get; set; }

        /// <summary>
        /// 货架位置
        /// </summary>
        public string StorageRackPosition { get; set; }
        /// <summary>
        /// 料箱编号
        /// </summary>
        public string InventoryBoxNo { get; set; }

        /// <summary>
        /// 料格序号
        /// </summary>
        public string Position { get; set; }
    }
}

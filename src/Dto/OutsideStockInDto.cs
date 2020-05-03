using System;
using System.Collections.Generic;
using System.Text;
using YL.Utils;
using YL.Utils.Pub;

namespace YL.Core.Dto
{
    /// <summary>
    /// 外部入库命令
    /// </summary>
    public class OutsideStockInDto
    {
        /// <summary>
        /// 入库单编号
        /// </summary>
        public string WarehousingId { get; set; }
        /// <summary>
        /// 入库类型
        /// </summary>
        public string WarehousingType { get; set; }
        /// <summary>
        /// 入库时间
        /// </summary>
        public string WarehousingTime { get; set; }
        /// <summary>
        /// 生产令号
        /// </summary>
        public string ProductionPlanId { get; set; }
        /// <summary>
        /// 批次号
        /// </summary>
        public string BatchPlanId { get; set; }
        /// <summary>
        /// 作业区
        /// </summary>
        public string WorkAreaName { get; set; }
        /// <summary>
        /// 物料种类
        /// </summary>
        public int SuppliesKinds { get; set; }
        /// <summary>
        /// 物料信息列表
        /// </summary>
        public string SuppliesInfoList { get; set; } //OutsideMaterialDto[]
         
        public static OutsideStockInDto Create(
            string warehousingid, 
            string warehousingtype, 
            string warehousingtime,
            string productionplanid,
            string batchplanid, 
            string workareaname, 
            int supplieskinds,
            string suppliesinfolist)
        { 
            OutsideStockInDto data = new OutsideStockInDto()
            {
                WarehousingId = warehousingid,
                WarehousingType = warehousingtype,
                WarehousingTime = warehousingtime,
                ProductionPlanId = productionplanid, 
                BatchPlanId = batchplanid,
                WorkAreaName = workareaname,
                SuppliesKinds = supplieskinds,
                SuppliesInfoList = suppliesinfolist

            };
            return data;
        }
         
    }

    public class OutsideStockInResult
    {
        /// <summary>
        /// 消息接收成功与否
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// 入库单编号
        /// </summary>
        public string WarehousingId { get; set; }
        /// <String>
        /// 开始入库时间
        /// </summary>
        public String WarehousingTime { get; set; }
        /// <summary>
        /// 错误编号
        /// </summary>
        public String ErrorId { get; set; }
        /// <summary>
        /// 错误内容
        /// </summary>
        public String ErrorInfo { get; set; }

    }


    public class OutsideStockInResponse
    { 
        /// <summary>
        /// 入库单编号
        /// </summary>
        public string WarehousingId { get; set; }
        /// <summary>
        /// 入库仓库个数
        /// </summary>
        public int WarehousingEntryNumber { get; set; }
        /// <summary>
        /// 各入库仓库信息
        /// </summary>
        public string WarehousingEntryFinishList { get; set; } //OutsideStockInResponseWarehouse[]


    }

    public class OutsideStockInResponseWarehouse
    {
        /// <summary>
        /// 入库完成时间
        /// </summary>
        public string WarehousingFinishTime { get; set; }
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
        /// 物料数量
        /// </summary>
        public int SuppliesKinds { get; set; }
        /// <summary>
        /// 物料信息
        /// </summary>
        public List<OutsideMaterialResult> SuppliesInfoList { get; set; } = new List<OutsideMaterialResult>();
    }


    public class OutsideStockInResponseResult
    {
        /// <summary>
        /// 入库单编号
        /// </summary>
        public string WarehousingId { get; set; } 
        /// <summary>
        /// 是否正常接收
        /// </summary>
        public bool IsNormalExecution { get; set; } 
        /// <summary>
        /// 错误编号
        /// </summary>
        public string ErrorId { get; set; }

    }
}

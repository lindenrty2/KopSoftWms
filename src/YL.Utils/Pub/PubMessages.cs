using System;
using System.Collections.Generic;
using System.Text;

namespace YL.Utils.Pub
{
    public class PubMessages
    {
        public static MessageItem E1000_CreateStockIn = new MessageItem(-1000, "创建入库单时发生非预期的异常");
        public static MessageItem E1001_SuppliesType_NotFound = new MessageItem(-1001,"找不到物料类型定义");
        public static MessageItem E1002_SuppliesType_WarehouseId_NotSet = new MessageItem(-1002, "物料类型没有指定仓库");
        public static MessageItem E1003_Unit_NotFound = new MessageItem(-1003, "找不到单位定义");
        public static MessageItem E1004_WarehouseType_NotFound = new MessageItem(-1004, "找不到入库类型定义");


        public static MessageItem E2001_STOCKINDETAIL_NOTFOUND = new MessageItem(-2001, "找不到入库详细");
        public static MessageItem E2002_STOCKINDETAIL_ALLOW_FINISHED = new MessageItem(-2002, "该入库已完成");
        public static MessageItem E2003_INVENTORYBOX_NOTFOUND = new MessageItem(-2003, "找不到料箱");
        public static MessageItem E2004_INVENTORYBOX_NOTINPOSITION = new MessageItem(-2004, "料箱不在库位"); 
        public static MessageItem E2005_STOCKIN_BOXOUT_FAIL = new MessageItem(-2005, "(入库)料箱出库操作失败");
        public static MessageItem E2006_STOCKIN_BOXBACK_FAIL = new MessageItem(-2006, "(入库)料箱归库操作失败");
        public static MessageItem E2007_STOCKINTASK_NOTFOUND = new MessageItem(-2007, "找不到入库任务");
        public static MessageItem E2008_STOCKINTASK_NOTOUT = new MessageItem(-2008, "料箱尚未出库完成，无法归库");
        public static MessageItem E2009_STOCKINTASK_ALLOW_BACKING = new MessageItem(-2009, "料箱已在归库途中，无法再次归库");
        public static MessageItem E2010_STOCKINTASK_ALLOW_BACKED = new MessageItem(-2010, "料箱已归库");
        public static MessageItem E2011_INVENTORYBOX_NOTOUTED = new MessageItem(-2011, "料箱状态异常，需要料箱状态为[出库完成]");
        public static MessageItem E2012_INVENTORYBOX_MATERIAL_NOTMATCH = new MessageItem(-2012, "料箱物料不匹配");
        public static MessageItem E2013_STOCKIN_NOTFOUND = new MessageItem(-2001, "找不到入库详细");
        public static MessageItem E2014_STOCKIN_ALLOW_FINISHED = new MessageItem(-2002, "该入库已完成");

        public static MessageItem E2100_WCS_OUTCOMMAND_FAIL = new MessageItem(-2010, "WCS料箱出库命令失败");
        public static MessageItem E2101_WCS_BACKCOMMAND_FAIL = new MessageItem(-2011, "WCS料箱归库命令失败");
   
    }

    public class MessageItem
    {
        public int Code { get; set; }
        public string Message { get; set; }

        public MessageItem(int code,string message)
        {
            this.Code = code;
            this.Message = message;
        }
    }
}

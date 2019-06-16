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

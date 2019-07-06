using System;
using System.Collections.Generic;
using System.Text;

namespace YL.Utils.Pub
{
    public class PubMessages
    { 
        public static MessageItem I1001_BOXBACK_SCCUESS = new MessageItem(1001, "料箱出库成功");
        public static MessageItem I1002_BOXOUT_SCCUESS = new MessageItem(1002, "料箱归库成功");

        public static MessageItem E0002_UPDATE_COUNT_FAIL = new MessageItem(-2, "数据更新失败,可能有第三方更新了数据");


        public static MessageItem E1001_SUPPLIESTYPE_NOTFOUND = new MessageItem(-1001,"找不到物料类型定义");
        public static MessageItem E1002_SUPPLIESTYPE_WAREHOUSEID_NOTSET = new MessageItem(-1002, "物料类型没有指定仓库");
        public static MessageItem E1003_UNIT_NOTFOUND = new MessageItem(-1003, "找不到单位定义");
        public static MessageItem E1004_WAREHOUSETYPE_NOTFOUND = new MessageItem(-1004, "找不到入库类型定义");
        public static MessageItem E1005_MATERIALNO_NOTFOUND = new MessageItem(-1005, "找不到物料编号");
        public static MessageItem E1006_INVENTORYBOX_MISSING = new MessageItem(-1006, "缺少料箱编号");
        public static MessageItem E1007_INVENTORYBOX_ALLOWUSED = new MessageItem(-1007, "料箱用尽");
        public static MessageItem E1008_INVENTORYBOX_NOTOUT = new MessageItem(-2008, "料箱不在出库状态");
        public static MessageItem E1009_INVENTORYBOX_NOTBACK = new MessageItem(-2009, "料箱不在归库状态");
        public static MessageItem E1010_INVENTORYBOX_BLOCK_OVERLOAD = new MessageItem(-1010, "料箱格数超过定义");
        public static MessageItem E1011_INVENTORYBOX_NOTFOUND = new MessageItem(-1011, "找不到料箱");
        public static MessageItem E1012_INVENTORYBOX_NOTINPOSITION = new MessageItem(-1012, "料箱不在库位");
        public static MessageItem E1013_INVENTORYBOXTASK_NOTFOUND = new MessageItem(-1013, "找不到料箱任务");
        public static MessageItem E1014_INVENTORYBOX_NOTOUTED = new MessageItem(-1014, "料箱状态异常，需要料箱状态为[出库完成]");
        public static MessageItem E1015_INVENTORYBOX_MATERIAL_NOTMATCH = new MessageItem(-1015, "料箱物料不匹配");
        public static MessageItem E1016_INVENTORYBOX_NOTOUTED = new MessageItem(-1016, "料箱尚未出库完成，无法归库");
        public static MessageItem E1017_INVENTORYBOX_ALLOW_BACKING = new MessageItem(-1017, "料箱已在归库途中，无法再次归库");
        public static MessageItem E1018_INVENTORYBOX_ALLOW_BACKED = new MessageItem(-1018, "料箱已归库");
        public static MessageItem E1019_INVENTORY_LOCKED = new MessageItem(-1019, "物料被锁定,无法出入库");
        public static MessageItem E1020_INVENTORYBOX_MATERIAL_LOCKED = new MessageItem(-1020, "料箱中的物料被其他任务锁定,无法使用");

        public static MessageItem I2000_STOCKOUT_SCAN_SCCUESS = new MessageItem(2000, "创建出库单成功");
        public static MessageItem I2001_STOCKIN_SCAN_SCCUESS = new MessageItem(2001, "入库扫码关联成功");
        public static MessageItem I2002_STOCKIN_SCCUESS = new MessageItem(2002, "入库单完成");

        public static MessageItem E2000_CREATE_STOCKIN = new MessageItem(-2000, "创建入库单时发生非预期的异常");
        public static MessageItem E2001_STOCKINDETAIL_NOTFOUND = new MessageItem(-2001, "找不到入库详细");
        public static MessageItem E2002_STOCKINDETAIL_ALLOW_FINISHED = new MessageItem(-2002, "该入库已完成");
        public static MessageItem E2005_STOCKIN_BOXOUT_FAIL = new MessageItem(-2005, "(入库)料箱出库操作失败");
        public static MessageItem E2006_STOCKIN_BOXBACK_FAIL = new MessageItem(-2006, "(入库)料箱归库操作失败");  
        public static MessageItem E2013_STOCKIN_NOTFOUND = new MessageItem(-2013, "找不到入库详细");
        public static MessageItem E2014_STOCKIN_ALLOW_FINISHED = new MessageItem(-2014, "该入库已完成");
        public static MessageItem E2015_STOCKIN_HASNOT_MATERIAL = new MessageItem(-2015, "入库单中找不到该物料");
        public static MessageItem E2016_STOCKINDETAIL_INVENTORYBOXTASK_NOTMATCH = new MessageItem(-2016, "料箱中存在无法关联入库单的项目");
        public static MessageItem E2017_STOCKIN_FAIL = new MessageItem(-2017, "入库单完成失败");

        public static MessageItem I2100_STOCKOUT_SCAN_SCCUESS = new MessageItem(2100, "创建出库单成功");
        public static MessageItem I2101_STOCKOUT_SCAN_SCCUESS = new MessageItem(2101, "出库扫码关联成功");
        public static MessageItem I2102_STOCKOUT_SCCUESS = new MessageItem(2102, "出库单完成");
        public static MessageItem I2103_STOCKOUT_LOCK_SCCUESS = new MessageItem(2103, "出库单锁定成功");
        public static MessageItem I2104_STOCKOUT_BOXOUT_SCCUESS = new MessageItem(2104, "出库单料箱出库成功");

        public static MessageItem E2100_CREATE_STOCKIN = new MessageItem(-2100, "创建出库单时发生非预期的异常");
        public static MessageItem E2101_STOCKOUTDETAIL_NOTFOUND = new MessageItem(-2101, "找不到出库详细");
        public static MessageItem E2102_STOCKOUTDETAIL_ALLOW_FINISHED = new MessageItem(-2102, "该出库已完成");
        public static MessageItem E2105_STOCKOUT_BOXOUT_FAIL = new MessageItem(-2105, "(出库)料箱出库操作失败");
        public static MessageItem E2106_STOCKOUT_BOXBACK_FAIL = new MessageItem(-2106, "(出库)料箱归库操作失败"); 
        public static MessageItem E2113_STOCKOUT_NOTFOUND = new MessageItem(-2113, "找不到入库详细");
        public static MessageItem E2114_STOCKOUT_ALLOW_FINISHED = new MessageItem(-2114, "该出库已完成");
        public static MessageItem E2115_STOCKOUT_HASNOT_MATERIAL = new MessageItem(-2115, "出库单中找不到该物料");
        public static MessageItem E2116_STOCKOUTDETAIL_INVENTORYBOXTASK_NOTMATCH = new MessageItem(-2116, "料箱中存在无法关联入库单的项目");
        public static MessageItem E2117_STOCKOUT_FAIL = new MessageItem(-2117, "出库单完成失败");
        public static MessageItem E2118_STOCKOUT_LOCK_FAIL = new MessageItem(-2118, "出库单锁定失败");
        public static MessageItem E2119_STOCKOUT_MATERIAL_ENOUGH = new MessageItem(-2119, "出库单所需物料没有足够库存");
        public static MessageItem E2120_STOCKOUT_NOMORE_BOX = new MessageItem(-2120, "出库单没有更多的料箱需要出库");
        public static MessageItem E2121_STOCKOUT_ALLOW_LOCKED = new MessageItem(-2121, "该出库单已锁定，无法再次锁定");

        public static MessageItem I2300_WCS_OUTCOMMAND_SCCUESS = new MessageItem(2300, "WCS料箱出库命令成功");
        public static MessageItem E2300_WCS_OUTCOMMAND_FAIL = new MessageItem(-2300, "WCS料箱出库命令失败");

        public static MessageItem E2301_WCS_STOCKOUT_NOTCOMPLATE_EXIST = new MessageItem(-2301, "发现有未完成的出库任务");
        public static MessageItem E2302_WCS_TASKID_INVAILD = new MessageItem(-2302, "WCS任务Id无效");
        public static MessageItem E2303_WCS_STOCKOUTTASK_NOTFOUND = new MessageItem(-2303, "WCS回调出库确认时发现任务不存在");
        public static MessageItem E2304_WCS_STOCKOUTTASK_NOTOUT = new MessageItem(-2304, "WCS回调出库确认时发现任务不处于出库状态"); 

        public static MessageItem I2300_WCS_BACKCOMMAND_SCCUESS = new MessageItem(2301, "WCS料箱归库命令成功");
        public static MessageItem E2310_WCS_BACKCOMMAND_FAIL = new MessageItem(-2301, "WCS料箱归库命令失败");
        public static MessageItem E2311_WCS_STOCKBACKTASK_NOTFOUND = new MessageItem(-2311, "WCS回调归库确认时发现任务不存在");
        public static MessageItem E2312_WCS_STOCKBACKTASK_NOTBACK = new MessageItem(-2312, "WCS回调归库确认时发现任务不处于出库状态");

        public static MessageItem E3000_MES_STOCKINTASK_NOTFOUND = new MessageItem(-3000, "找不到对应的MES入库任务");
        public static MessageItem E3100_MES_STOCKOUTTASK_NOTFOUND = new MessageItem(-3100, "找不到对应的MES出库任务");
    }


}

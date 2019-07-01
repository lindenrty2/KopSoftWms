using System.ComponentModel;

namespace YL.Utils.Pub
{
    public enum PubEnum
    {
        [Description("成功")]
        Success = 100,

        [Description("model验证失败返回")]
        Failed = 101,

        [Description("提示错误")]
        Error = 102
    }

    public enum ExcelVersion
    {
        V2007,
        V2003
    }

    public enum Qualified
    {
        [Description("不合格")]
        IsUn = 0,

        [Description("合格")]
        Un = 1,
    }

    public enum EProductType
    {
        [Description("原物料")]
        Material = 1,

        [Description("半成品")]
        SFG = 2,

        [Description("产成品")]
        Goods = 3,

        [Description("虚拟件")]
        Part = 4,
    }

    public enum PubDictType
    {
        [Description("单位类别")]
        unit = 1,

        [Description("物料分类")]
        material = 2,

        [Description("入库单")]
        stockin = 3,

        [Description("出库单")]
        stockout = 4,

        [Description("设备分类")]
        device = 5,

        [Description("设备产权")]
        property = 6
    }

    public enum PubLevel
    {
        [Description("一级")]
        one = 1,

        [Description("二级")]
        two = 2,

        [Description("三级")]
        three = 3
    }

    /// <summary>
    /// 入出库单状态
    /// </summary>
    public enum StockInStatus
    {
        [Description("任务取消")]
        task_canceled = -1,

        [Description("任务创建中")]
        initial = 1,

        [Description("任务确认")]
        task_confirm = 2,

        [Description("任务进行中")]
        task_working = 3,
         
        [Description("任务完成")]
        task_finish = 4,
    }

    /// <summary>
    /// 入出库单状态
    /// </summary>
    public enum StockOutStatus
    {
        [Description("任务取消")]
        task_canceled = -1,

        [Description("任务创建中")]
        initial = 1,

        [Description("任务确认")]
        task_confirm = 2,

        [Description("任务进行中")]
        task_working = 3,

        [Description("任务完成")]
        task_finish = 4,
    }

    public enum InventoryBoxStatus
    {
        [Description("不在库")]
        None = 0,
        [Description("在库")]
        InPosition = 1,
        [Description("出库中")]
        Outing = 2,
        [Description("出库完成")]
        Outed = 3,
        [Description("归库中")]
        Backing = 4,

    }

    public enum InventoryBoxTaskStatus
    {
        [Description("任务取消")]
        task_canceled = -1,

        [Description("任务确认")]
        task_confirm = 1,
         
        [Description("任务进行中：料箱正在出库")]
        task_outing = 2,

        [Description("任务进行中：料箱出库完成")]
        task_outed = 3,

        [Description("任务进行中：料箱正在归库")]
        task_backing = 4,

        [Description("任务完成：料箱归库完成")]
        task_backed = 5,

        [Description("任务完成：料箱移除出库")]
        task_leaved = 6,
    }

    public enum LogType
    {
        [Description("登录")]
        login = 1,

        [Description("添加")]
        add = 2,

        [Description("修改")]
        update = 3,

        [Description("添加或修改")]
        addOrUpdate = add | update,

        [Description("删除")]
        delete = 4,

        [Description("删除")]
        select = 5,

        [Description("异常")]
        exception = 6,

        [Description("错误")]
        error = 7,

        [Description("导出")]
        export = 8,

        [Description("导入")]
        import = 9,

        [Description("上传")]
        upload = 10,

        [Description("下载")]
        download = 10,


    }

    public enum MesTaskTypes
    {
        [Description("入库任务")]
        StockIn = 1,
        [Description("出库任务")]
        StockOut = 2,

    }


    public enum MESTaskWorkStatus
    {
        [Description("处理已取消")]
        None = -2,
        [Description("处理失败")]
        Failed = -1,
        [Description("待计划")]
        WaitPlan = 1,
        [Description("已计划任务")]
        Planed = 2,
        [Description("已开始处理")]
        Working = 3,
        [Description("处理已完成")]
        WorkComplated = 4,
    }

    public enum MESTaskNotifyStatus
    {
        [Description("回馈失败")]
        Failed = -1,
        [Description("已接收任务")]
        Requested = 1,
        [Description("等待回馈MES")]
        WaitResponse = 2,
        [Description("已反馈MES")]
        Responsed = 3,
    }


    public enum WCSTaskWorkStatus
    {
        [Description("处理失败")]
        Failed = -1,
        [Description("不明")]
        Unknow = 0,
        [Description("待计划")]
        WaitPlan = 1,
        [Description("已计划任务")]
        Planed = 2,
        [Description("已开始处理")]
        Working = 3,
        [Description("处理已完成")]
        WorkComplated = 4,
    }

    public enum WCSTaskNotifyStatus
    {
        [Description("通知失败")]
        Failed = -1,
        [Description("待通知")]
        WaitRequest = 1,
        [Description("等待回馈")]
        WaitResponse = 2,
        [Description("已接收回馈")]
        Responsed = 3,
    }



}
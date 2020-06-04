using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YL.Utils.Pub;

namespace IServices.Outside
{
    /// <summary>
    /// 获取库位参数
    /// </summary>
    public class GetLocationArg
    {
        /// <summary>
        /// 可用通道，KEY是通道编号，value是通道当前任务数量
        /// </summary>
        Dictionary<string, int> DicChannel { get; set; }
        /// <summary>
        /// 托盘条码
        /// </summary>
        public string TrayBarcode { get; set; }
        /// <summary>
        /// 在没有高低货位区分时默认1
        /// </summary>
        public string Height { get; set; }
        /// <summary>
        /// 1=正常申请库位 2=满入申请库位 3=空车入库
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 区域
        /// </summary>
        public string Region { get; set; }
        /// <summary>
        ///  如果是满入申请，则需要将获取到的任务ID上传给WMS。正常申请则为空。
        /// </summary>
        public string TaskId { get; set; }
        /// <summary>
        /// 预留信息
        /// </summary>
        public string Other { get; set; }
          

    }

    public class GetLocationResult
    {
        /// <summary>
        /// 成功与否
        /// </summary>
        public bool Successd { get; set; }
        /// <summary>
        /// 返回code如果失败则返回失败原因
        /// </summary>
        public String ReasonCode { get; set; }
        public String Reason { get; set; }
        /// <summary>
        /// 库位ID
        /// </summary>
        public String LocationId { get; set; }
        /// <summary>
        /// 库位编码
        /// </summary>
        public String LocationCode { get; set; }
        /// <summary>
        /// 库位排
        /// </summary>
        public String LocationRow { get; set; }
        /// <summary>
        /// 库位列
        /// </summary>
        public String LocationColumn { get; set; }
        /// <summary>
        /// 库位层
        /// </summary>
        public String LocationFloor { get; set; }
        /// <summary>
        /// 通道
        /// </summary>
        public String LocationChannel { get; set; }
        /// <summary>
        /// 入库口编码
        /// </summary>
        public String StationCode { get; set; }
        /// <summary>
        /// 任务ID唯一号，后续确认库存时需要反馈
        /// </summary>
        public String TaskId { get; set; }
        /// <summary>
        /// 标签卡条码
        /// </summary>
        public string LOTID { get; set; }
        /// <summary>
        /// 工装号
        /// </summary>
        public string TOOLNO { get; set; }
        /// <summary>
        /// 规格代码
        /// </summary>
        public string ITNBR { get; set; }
        /// <summary>
        /// 规格名称
        /// </summary>
        public string ITDSC { get; set; }
        /// <summary>
        /// 版本
        /// </summary>
        public string VERSION { get; set; }

    }

    /// <summary>
    /// WCS入库任务指令
    /// </summary>
    public class StockInTaskInfo
    {
        //	任务ID，确认出库完成时反馈该ID	
        public string TaskId	{get;set;}
        //	托盘条码/料箱条码	
        public string TrayBarcode	{get;set;}
        //	默认10即可	
        public int Priority	{get;set;}
        //	通道	
        public string Channel	{get;set;}
        //	预留	
        public string PartSn	{get;set;}
        //	预留	
        public string PartCode	{get;set;}
        //	默认1即可	
        public string PartHeight	{get;set;}
        //	预留	
        public int TaskLineId	{get;set;}
        //	库存表ID	
        public int BalanceId	{get;set;}
        //	入库单或者出库单ID	
        public int	StockId	{get;set;}
        //	取货货位ID	
        public int	LocationId	{get;set;}
        //	取货货位CODE	
        public string LocationCode	{get;set;}
        //	放货货位ID	
        public int SetLocationId	{get;set;}
        //	放货货位CODE	
        public string SetLocationCode	{get;set;}
        //	取货排	
        public string GetRow	{get;set;}
        //	取货排	
        public string GetColumn	{get;set;}
        //	取货排	
        public string	GetFloor	{get;set;}
        //	放货排	
        public string	SetRow	{get;set;}
        //	放货列	
        public string SetColumn	{get;set;}
        //	放货层	
        public string SetFloor	{get;set;}
        //	任务类型 IN=入库 OUT=出库	
        public string TaskType	{get;set;}
        //	任务队列，默认100，数字越小越优先	
        public int	TaskQueue	{get;set;}
        //	默认0，完成后>=99	
        public string TaskStatus	{get;set;}
        //	创建人，0=自动创建	
        public int CreateBy	{get;set;}
        //	创建时间	
        public DateTime	CreateDate	{get;set;}
        //	堆垛机任务ID，1-30000递增循环	
        public string TaskNo	{get;set;}
        //	输送线PLC起始地址	
        public string GetPlcCode	{get;set;}
        //	输送点PLC目标地址	
        public string SetPlcCode	{get;set;}
        //	分拣台/接驳台位置	
        public string	Table	{get;set;}

    }

    public class StockInTaskResult
    {
        public bool Successd { get; set; } = true;
        public string ErrorCode { get; set; }
        public string SubErrorCode { get; set; }
        public string ErrorDesc { get; set; }
        public string SubErrorDesc { get; set; }
        public string RequestMethod { get; set; }
        public string Code { get; set; }
        public string Returncode { get; set; }
    }

    public class StockOutTaskInfo
    {
        //	任务ID，确认出库完成时反馈该ID	
        public	string	TaskId	{get;set;}
        //	托盘条码/料箱条码	
        public	string	TrayBarcode	{get;set;}
        //	默认10即可	
        public	int	Priority	{get;set;}
        //	通道	
        public	string	Channel	{get;set;}
        //	预留	
        public	string	PartSn	{get;set;}
        //	预留	
        public	string	PartCode	{get;set;}
        //	默认1即可	
        public	string	PartHeight	{get;set;}
        //	预留	
        public	int	TaskLineId	{get;set;}
        //	库存表ID	
        public	int	BalanceId	{get;set;}
        //	入库单或者出库单ID	
        public	int	StockId	{get;set;}
        //	取货货位ID	
        public	int	LocationId	{get;set;}
        //	取货货位CODE	
        public	string	LocationCode	{get;set;}
        //	放货货位ID	
        public	int	SetLocationId	{get;set;}
        //	放货货位CODE	
        public	string	SetLocationCode	{get;set;}
        //	取货排	
        public	string	GetRow	{get;set;}
        //	取货排	
        public	string	GetColumn	{get;set;}
        //	取货排	
        public	string	GetFloor	{get;set;}
        //	放货排	
        public	string	SetRow	{get;set;}
        //	放货列	
        public	string	SetColumn	{get;set;}
        //	放货层	
        public	string	SetFloor	{get;set;}
        //	任务类型 IN=入库 OUT=出库	
        public	string	TaskType	{get;set;}
        //	任务队列，默认100，数字越小越优先	
        public	int	TaskQueue	{get;set;}
        //	默认0，完成后>=99	
        public	string	TaskStatus	{get;set;}
        //	创建人，0=自动创建	
        public	int	CreateBy	{get;set;}
        //	创建时间	
        public	DateTime	CreateDate	{get;set;}
        //	堆垛机任务ID，1-30000递增循环	
        public	string	TaskNo	{get;set;}
        //	输送线PLC起始地址	
        public	string	GetPlcCode	{get;set;}
        //	输送点PLC目标地址	
        public	string	SetPlcCode	{get;set;}
        //	分拣台/接驳台位置	
        public	string	Table	{get;set;}

    }


    public class CreateOutStockResult
    {
        public bool Successd { get; set; } = true;
        public string ErrorCode { get; set; }
        public string SubErrorCode { get; set; }
        public string ErrorDesc { get; set; }
        public string SubErrorDesc { get; set; }
        public string RequestMethod { get; set; }
        public string Code { get; set; }
        public string Returncode { get; set; }

    }


    /// <summary>
    /// WCS任务确认后回调的内容
    /// </summary>
    public class WCSStockTaskCallBack
    {
        public bool Success { get; set; } = true;

        /// <summary>
        /// 托盘条码
        /// </summary>
        public string TrayBarcode { get; set; }
        public string TaskId { get; set; }
        /// <summary>
        /// 执行成功后默认为ok，失败则说明原因，记录日志
        /// </summary>
        public string Code { get; set; }
        public string Other { get; set; }

    }

    /// <summary>
    /// 调用WCS接口以后的返回值
    /// </summary>
    public class WCSApiResult
    {
        public bool Successd { get; set; } = true;
        /// <summary>
        /// 错误代码
        /// FormatErr = 数据格式异常， DataErr = 数据库异常，OtherErr = 其他异常。
        /// 预留，可不填
        /// </summary>
        public string ErrorCode { get; set; }
        /// <summary>
        /// 表示函数异常代码，函数名称+3位错误序号。如：AddUser异常 则AddUser001。 
        /// 预留，可不填
        /// </summary>
        public string SubErrorCode { get; set; }
        /// <summary>
        /// 异常描述
        /// FormatErr = 数据格式异常， DataErr = 数据库异常，OtherErr = 其他异常。
        /// 预留，可不填
        /// </summary>
        public string ErrorDesc { get; set; }
        /// <summary>
        /// 预留，可不填
        /// </summary>
        public string SubErrorDesc { get; set; }
        /// <summary>
        /// 必填，用于提示用户的信息。
        /// </summary>
        public string RequestMethod { get; set; }

        /// <summary>
        /// 必填，用于提示用户的信息。
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 预留，可不填
        /// </summary>
        public string Returncode { get; set; }


        public WCSApiResult()
        {

        }

        public WCSApiResult(MessageItem message)
        {
            this.Code = message.Code.ToString();
            this.RequestMethod = message.Message;
        }

    }

    public class ConfirmOutStockResult : WCSApiResult
    {
        public ConfirmOutStockResult()
        {

        }

        public ConfirmOutStockResult(MessageItem message) : base(message)
        {

        }
    }

    public class ConfirmBackStockResult : WCSApiResult
    {
        public ConfirmBackStockResult()
        {

        }

        public ConfirmBackStockResult(MessageItem message) : base(message)
        {

        }
    }
}

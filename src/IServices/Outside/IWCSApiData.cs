using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YL.Utils.Pub;

namespace WMSCore.Outside
{
    public class BackStockInfo
    {
        public string TaskId { get; set; }
    }

    public class CreateBackStockResult
    {
        public bool Successd { get; set; } = true;
        public bool ErrorCode { get; set; }
        public bool SubErrorCode { get; set; }
        public bool ErrorDesc { get; set; }
        public bool SubErrorDesc { get; set; }
        public bool RequestMethod { get; set; }
        public bool Code { get; set; }
        public bool Returncode { get; set; }
    }

    public class OutStockInfo
    {
        public string TaskId { get; set; }
        public string TrayBarcode { get; set; }
        public int Priority { get; set; }
        public string Channel { get; set; }
        public string PartSn { get; set; }
        public string PartCode { get; set; }
        public string PartHeight { get; set; }
        public int TaskLineId { get; set; }
        public int BalanceId { get; set; }
        public int StockId { get; set; }
        public int LocationId { get; set; }
        public string LocationCode { get; set; }
        public int SetLocationId { get; set; }
        public string SetLocationCode { get; set; }
        public string GetRow { get; set; }
        public string GetColumn { get; set; }
        public string GetFloor { get; set; }
        public string SetRow { get; set; }
        public string SetColumn { get; set; }
        public string SetFloor { get; set; }
        public string TaskType { get; set; }
        public int TaskQueue { get; set; }
        public string TaskStatus { get; set; }
        public int CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string TaskNo { get; set; }
        public string GetPlcCode { get; set; }
        public string SetPlcCode { get; set; }
        public string LOTID { get; set; }
        public string TOOLNO { get; set; }
        public string ITNBR { get; set; }
        public string ITDSC { get; set; }
        public string VERSION { get; set; }
        public string MATWITH { get; set; }
    }


    public class CreateOutStockResult
    {
        public bool Successd { get; set; } = true;
        public bool ErrorCode { get; set; }
        public bool SubErrorCode { get; set; }
        public bool ErrorDesc { get; set; }
        public bool SubErrorDesc { get; set; }
        public bool RequestMethod { get; set; }
        public bool Code { get; set; }
        public bool Returncode { get; set; }

    }



    public class WCSTaskResult
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

    public class StockTaskResult
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


        public StockTaskResult()
        {

        }

        public StockTaskResult(MessageItem message)
        {
            this.Code = message.Code.ToString();
            this.RequestMethod = message.Message;
        }

    }

    public class ConfirmOutStockResult : StockTaskResult
    {
        public ConfirmOutStockResult()
        {

        }

        public ConfirmOutStockResult(MessageItem message) : base(message)
        {

        }
    }

    public class ConfirmBackStockResult : StockTaskResult
    {
        public ConfirmBackStockResult()
        {

        }

        public ConfirmBackStockResult(MessageItem message) : base(message)
        {

        }
    }
}

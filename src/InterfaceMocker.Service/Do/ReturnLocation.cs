using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InterfaceMocker.Service.Do
{

    /// <summary>
    /// WMS取消订单
    /// </summary>
    public class ReturnLocation
    {
        private bool _Succeed;
        /// <summary>
        /// 是否成功，True=成功 False=失败
        /// </summary>
        public bool Succeed
        {
            set { _Succeed = value; }
            get { return _Succeed; }
        }

        private String _ReasonCode;
        /// <summary>
        /// 返回code如果失败则返回失败原因
        /// </summary>
        public String ReasonCode
        {
            set { _ReasonCode = value; }
            get { return _ReasonCode; }
        }

        private String _Reason;
        /// <summary>
        /// 返回原因
        /// </summary>
        public String Reason
        {
            set { _Reason = value; }
            get { return _Reason; }
        }

        private String _LocationId;
        /// <summary>
        /// 库位ID
        /// </summary>
        public String LocationId
        {
            set { _LocationId = value; }
            get { return _LocationId; }
        }

        private String _LocationCode;
        /// <summary>
        /// 库位编码
        /// </summary>
        public String LocationCode
        {
            set { _LocationCode = value; }
            get { return _LocationCode; }
        }

        private String _LocationRow;
        /// <summary>
        /// 库位排
        /// </summary>
        public String LocationRow
        {
            set { _LocationRow = value; }
            get { return _LocationRow; }
        }

        private String _LocationColumn;
        /// <summary>
        /// 库位列
        /// </summary>
        public String LocationColumn
        {
            set { _LocationColumn = value; }
            get { return _LocationColumn; }
        }

        private String _LocationFloor;
        /// <summary>
        /// 库位层
        /// </summary>
        public String LocationFloor
        {
            set { _LocationFloor = value; }
            get { return _LocationFloor; }
        }

        private String _LocationChannel;
        /// <summary>
        /// 库位巷道
        /// </summary>
        public String LocationChannel
        {
            set { _LocationChannel = value; }
            get { return _LocationChannel; }
        }

        private String _StationCode;
        /// <summary>
        /// 入库口编码
        /// </summary>
        public String StationCode
        {
            set { _StationCode = value; }
            get { return _StationCode; }
        }

        private String _TaskId;
        /// <summary>
        /// 任务ID唯一号，后续确认库存时需要反馈
        /// </summary>
        public String TaskId
        {
            set { _TaskId = value; }
            get { return _TaskId; }
        }
        private String _LOTID;
        public String LOTID
        {
            set { _LOTID = value; }
            get { return _LOTID; }
        }

        private String _TOOLNO;
        public String TOOLNO
        {
            set { _TOOLNO = value; }
            get { return _TOOLNO; }
        }

        private String _ITNBR;
        public String ITNBR
        {
            set { _ITNBR = value; }
            get { return _ITNBR; }
        }

        private String _ITDSC;
        public String ITDSC
        {
            set { _ITDSC = value; }
            get { return _ITDSC; }
        }

        private String _VERSION;
        public String VERSION
        {
            set { _VERSION = value; }
            get { return _VERSION; }
        }

    }
}

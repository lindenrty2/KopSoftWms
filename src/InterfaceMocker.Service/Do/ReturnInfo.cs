using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InterfaceMocker.Service.Do
{
    /// <summary>
    /// 接口返回数据
    /// </summary>
    public class ReturnInfo
    {
        private bool _Successd;
        /// <summary>
        /// 是否成功，True=成功 False=失败
        /// </summary>
        public bool Successd
        {
            set { _Successd = value; }
            get { return _Successd; }
        }

        private String _ErrorCode;
        /// <summary>
        /// 错误代码  FormatErr = 数据格式异常， DataErr = 数据库异常，OtherErr = 其他异常
        /// </summary>
        public String ErrorCode
        {
            set { _ErrorCode = value; }
            get { return _ErrorCode; }
        }

        private String _SubErrorCode;
        /// <summary>
        /// 表示函数异常代码，函数名称+3位错误序号。如：AddUser异常 则AddUser001
        /// </summary>
        public String SubErrorCode
        {
            set { _SubErrorCode = value; }
            get { return _SubErrorCode; }
        }

        private String _ErrorDesc;
        /// <summary>
        /// 异常描述  FormatErr = 数据格式异常， DataErr = 数据库异常，OtherErr = 其他异常
        /// </summary>
        public String ErrorDesc
        {
            set { _ErrorDesc = value; }
            get { return _ErrorDesc; }
        }

        private String _SubErrorDesc;
        /// <summary>
        /// 预留
        /// </summary>
        public String SubErrorDesc
        {
            set { _SubErrorDesc = value; }
            get { return _SubErrorDesc; }
        }

        private String _RequestMethod;
        /// <summary>
        /// 预留
        /// </summary>
        public String RequestMethod
        {
            set { _RequestMethod = value; }
            get { return _RequestMethod; }
        }

        private String _Code;
        /// <summary>
        /// 用于提示用户的信息
        /// </summary>
        public String Code
        {
            set { _Code = value; }
            get { return _Code; }
        }

        private String _Reasoncode;
        /// <summary>
        /// 预留
        /// </summary>
        public String Reasoncode
        {
            set { _Reasoncode = value; }
            get { return _Reasoncode; }
        }
    }
}

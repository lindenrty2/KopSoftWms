using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InterfaceMocker.Service.Do
{
    public class RetValue
    {
        public RetValue()
        {
            this.Success = true;
            this.Returncode = "0";
        }

        public RetValue(bool success, string returncode, string errorcode, string errordesc)
        {
            this.Success = success;
            this.Returncode = returncode;
            this.ErrorCode = errorcode;
            this.ErrorDesc = errordesc;
        }
        public bool Success { set; get; }
        public string Returncode { set; get; }
        public string ErrorCode { set; get; }
        public string ErrorDesc { set; get; }


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InterfaceMocker.Service.Do
{

    public class WmsRetValue
    {
        public WmsRetValue()
        {
            this.Success = true;
            this.Code = "0";
        }

        public WmsRetValue(bool success, string Code, string Other)
        {
            this.Success = success;
            this.Code = Code;
            this.Other = Other;
        }
        public bool Success { set; get; }
        public string Code { set; get; }
        public string Other { set; get; }


    }

}

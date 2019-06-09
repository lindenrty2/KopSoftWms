using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WMSCore.Outside
{
    public class MESHookController : Controller, IMESController
    {
        public string Ping(string s)
        {
            return "";
        }
    }


    [ServiceContract]
    public interface IMESController
    {
        [OperationContract]
        string Ping(string s);
    }
}
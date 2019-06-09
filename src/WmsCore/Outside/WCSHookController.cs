using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WMSCore.Outside
{
    public class WCSHookController : IWCSController
    {
        public string Ping(string s)
        {
            return "OK";
        }
    }

    [ServiceContract]
    public interface IWCSController
    {
        [OperationContract]
        string Ping(string s);
    }
}
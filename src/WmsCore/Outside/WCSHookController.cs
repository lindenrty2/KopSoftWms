using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WMSCore.Outside
{
    public class WCSHookController : Controller
    {
        public string Ping(string s)
        {
            return "OK";
        }
    }

}
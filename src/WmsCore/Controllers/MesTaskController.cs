using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using YL.NetCore.Attributes;
using YL.NetCore.NetCoreApp;

namespace WMSCore.Controllers
{
    public class MesTaskController : BaseController
    {
        [HttpGet]
        [CheckMenu]
        public IActionResult Index()
        {
            return View();
        }
    }
}
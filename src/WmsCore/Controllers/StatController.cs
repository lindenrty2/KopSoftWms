using IServices;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WMSCore.Outside;
using YL.Core.Dto;
using YL.Core.Entity;
using YL.Core.Entity.Fluent.Validation;
using YL.NetCore.Attributes;
using YL.NetCore.NetCoreApp;
using YL.Utils.Extensions;
using YL.Utils.Pub;
using YL.Utils.Table;
using YL.Utils.Json;
using static YL.Core.Dto.PubParams;
using Newtonsoft.Json;
using IServices.Outside;

namespace KopSoftWms.Controllers
{
    public class StatController : BaseController
    { 

        private readonly SqlSugarClient _client; 
        public StatController( 
            SqlSugarClient client
            )
        {
            _client = client; 
        }

        [HttpGet] 
        public async Task<IActionResult> Index()
        {
            return View();
        }
         
    }
}
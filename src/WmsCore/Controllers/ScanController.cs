using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using YL.Core.Dto;
using YL.Core.Entity;
using YL.NetCore.NetCoreApp;
using YL.Utils.Pub;
using YL.Utils.Extensions;
using IServices;

namespace WMSCore.Controllers
{

    public class ScanController : BaseController
    {
        IWms_stockinServices _stockinServices;
        IWms_stockoutServices _stockoutServices;
        SqlSugarClient _client;
        public ScanController(
            IWms_stockinServices stockinServices,
            IWms_stockoutServices stockoutServices,
            SqlSugarClient client
            )
        {
            _stockinServices = stockinServices;
            _stockoutServices = stockoutServices;
            _client = client;
        }

        [HttpGet]
        public IActionResult MainScanPage(long storeId,string stockNo = null, ScanMode mode = ScanMode.StockIn)
        {
            ViewBag.StoreId = storeId; 
            ViewBag.StockNo = stockNo;
            ViewBag.ScanMode = mode;
            return View();
        }

    }
}
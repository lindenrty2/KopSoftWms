﻿using System;
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
using WMSCore.Outside;
using IServices.Outside;
using Newtonsoft.Json;

namespace WMSCore.Controllers
{

    public class ScanController : BaseController
    {
        SqlSugarClient _client;
        public ScanController( 
            SqlSugarClient client
            )
        { 
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

        [HttpGet]
        public async Task<IActionResult> InventoryBoxList(long storeId,string materialNo)
        {
            IWMSOperationApiAccessor accessor = WMSApiManager.GetOperationApiAccessor(storeId.ToString(), _client, this.UserDto);

            ViewBag.StoreId = storeId;
            ViewBag.MaterialNo = materialNo; 

            RouteData<Wms_InventoryBoxMaterialInfo[]> result = await accessor.GetInventoryBoxList(materialNo);
            ViewBag.Data = JsonConvert.SerializeObject(result.Data);
            return View();
        }
         

    }
}
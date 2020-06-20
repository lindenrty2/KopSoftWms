﻿using IServices;
using IServices.Outside;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Threading.Tasks;
using WMSCore.Outside;
using YL.Core.Dto;
using YL.NetCore.Attributes;
using YL.NetCore.NetCoreApp;
using YL.Utils.Pub;
using YL.Utils.Table;
using YL.Utils.Json;

namespace KopSoftWms.Controllers
{
    public class InventoryRecordController : BaseController
    { 

        private readonly SqlSugarClient _client;
        public InventoryRecordController(
            SqlSugarClient client
            )
        {
            _client = client; 
        }

        [HttpGet]
        [CheckMenu]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [OperationLog(LogType.select)]
        public async Task<PageGridData> List([FromForm]PubParams.InventoryBootstrapParams bootstrap)
        {
            //var sd = _inventoryrecordServices.PageList(bootstrap);
            //return Content(sd);

            IWMSBaseApiAccessor wmsAccessor = WMSApiManager.GetBaseApiAccessor(bootstrap.storeId.ToString(), _client);
            RouteData<OutsideInventoryRecordDto[]> result = (await wmsAccessor.QueryInventoryRecord(
                null, null, null, null, bootstrap.pageIndex, bootstrap.limit, bootstrap.search,
                new string[] { bootstrap.sort + " " + bootstrap.order },
                bootstrap.datemin, bootstrap.datemax));
            if (!result.IsSccuess)
            {
                return new PageGridData();
            }
            return result.ToGridData();
        }
    }
}
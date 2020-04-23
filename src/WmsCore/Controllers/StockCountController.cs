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
    public class StockCountController : BaseController
    { 

        private readonly SqlSugarClient _client; 
        public StockCountController( 
            SqlSugarClient client
            )
        {
            _client = client; 
        }

        [HttpGet]
        [CheckMenu]
        public async Task<IActionResult> Index()
        {
            long currentStoreId = (long)ViewData["currentStoreId"];


            IWMSBaseApiAccessor baseAccessor = WMSApiManager.GetBaseApiAccessor(currentStoreId.ToString(), _client);
            ViewBag.ReservoirAreas = (await baseAccessor.GetReservoirAreaList(0, 100, "", null, "", "")).Data;

            IWMSManagementApiAccessor wmsAccessor = WMSApiManager.GetManagementApiAccessor(currentStoreId.ToString(),_client,this.UserDto);
            //ViewBag.StorageRack = (await wmsAccessor.GetStorageRackList( null, 1, 100, null, null, null, null)).Data; 
            ViewBag.MaterialTypes = (await wmsAccessor.GetMaterialTypes()).Data;
            return View();
        }

        [HttpPost]
        [OperationLog(LogType.select)]
        public async Task<string> List(string storeId,string materialTypeId,string materialName)
        {
            IWMSManagementApiAccessor wmsAccessor = WMSApiManager.GetManagementApiAccessor(storeId, _client, this.UserDto);
            long? mtId = string.IsNullOrWhiteSpace( materialTypeId) ? (long?)null : materialTypeId.ToInt32();
            RouteData<Wms_StockCountInventoryBoxDto[]> result = (await wmsAccessor.GetStockCountInventoryBoxList(mtId, materialName));
            if (!result.IsSccuess)
            {
                return new PageGridData().JilToJson();
            }
            return result.ToGridJson();
            //var sd = _inventoryBoxServices.PageList(bootstrap);
            //return Content(sd);
        }
         

         
    }
}
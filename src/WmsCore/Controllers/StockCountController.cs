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
        }

        [HttpGet]
        public async Task<IActionResult> StepList(long storeId, string stockCountNo)
        {
            ViewData["currentStoreId"] = storeId;

            IWMSOperationApiAccessor operationAccessor =
                WMSApiManager.GetOperationApiAccessor(storeId.ToString(), _client,this.UserDto);
            RouteData<OutsideStockCountDto> result = await operationAccessor.BeginStockCount(stockCountNo);

            if (!result.IsSccuess)
            {
                return Content(result.Message);
            }

            return View(result.Data);
        }

        [HttpPost]
        public async Task<PageGridData> QueryPlanList(StockCountBootstrapParams bootstrap)
        {
            IWMSBaseApiAccessor baseAccessor = WMSApiManager.GetBaseApiAccessor(bootstrap.storeId.ToString(), _client);
            RouteData<OutsideStockCountDto[]> result = await baseAccessor.QueryStockCountList(
                bootstrap.StockCountStatus, bootstrap.pageIndex, bootstrap.limit, bootstrap.search,
                new string[] { bootstrap.sort + " " + bootstrap.order },
                bootstrap.datemin, bootstrap.datemax
                );
            return result.ToGridData();
        }

        [HttpPost]
        public async Task<OutsideStockCountMaterial[]> QueryPlanMaterialList(StockCountMaterialBootstrapParams bootstrap)
        {
            IWMSBaseApiAccessor baseAccessor = WMSApiManager.GetBaseApiAccessor(bootstrap.storeId.ToString(), _client);
            RouteData<OutsideStockCountDto> result = await baseAccessor.QueryStockCount(bootstrap.StockCountNo);
            if (!result.IsSccuess || result.Data == null)
            {
                return new OutsideStockCountMaterial[0];
            }
            return result.Data.MaterialList;
        }

        [HttpPost]
        public async Task<OutsideStockCountStep[]> QueryPlanStepList(StockCountMaterialBootstrapParams bootstrap)
        {
            IWMSBaseApiAccessor baseAccessor = WMSApiManager.GetBaseApiAccessor(bootstrap.storeId.ToString(), _client);
            RouteData<OutsideStockCountDto> result = await baseAccessor.QueryStockCount(bootstrap.StockCountNo);
            if (!result.IsSccuess || result.Data == null)
            {
                return new OutsideStockCountStep[0];
            }
            return result.Data.StepList;
        }
    }
}
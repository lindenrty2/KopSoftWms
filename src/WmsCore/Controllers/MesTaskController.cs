using IServices.Outside;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WMSCore.Outside;
using YL.Core.Dto;
using YL.Core.Entity;
using YL.NetCore.Attributes;
using YL.NetCore.NetCoreApp;
using YL.Utils.Extensions;
using YL.Utils.Json;
using YL.Utils.Pub;
using YL.Utils.Table;
using Services.Outside;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace WMSCore.Controllers
{
    public class MesTaskController : BaseController
    {
        private readonly SqlSugarClient _client;
        private readonly ILogger _logger;
        public MesTaskController(SqlSugarClient client,ILogger<MesTaskController> logger)
        {
            _client = client;
            _logger = logger;
        }


        [HttpGet]
        [CheckMenu]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [OperationLog(LogType.select)]
        public async Task<string> List([FromForm]PubParams.MesTaskBootstrapParams bootstrap)
        {
            ISugarQueryable<Wms_mestask> query = _client.Queryable<Wms_mestask>(); 
            if (!string.IsNullOrWhiteSpace(bootstrap.search))
            {
                query = query.Where(x => x.WarehousingId.ToString().Contains(bootstrap.search) || x.BatchPlanId.Contains(bootstrap.search));
            }
            DateTime minDate;
            if (!string.IsNullOrWhiteSpace(bootstrap.datemin) && DateTime.TryParse(bootstrap.datemin, out minDate))
            {
                query = query.Where(x => x.ModifiedDate >= minDate || x.CreateDate >= minDate);
            }
            DateTime maxDate;
            if (!string.IsNullOrWhiteSpace(bootstrap.datemax) && DateTime.TryParse(bootstrap.datemax, out maxDate))
            {
                query = query.Where(x => x.ModifiedDate <= maxDate || x.CreateDate <= maxDate);
            }
            query = query.Sort<Wms_mestask>(new string[] { bootstrap.sort + " " + bootstrap.order });
            //Order
            RefAsync<int> totalCount = new RefAsync<int>();
            List<Wms_mestask> result = await query.ToPageListAsync(bootstrap.pageIndex, bootstrap.limit, totalCount);
            return new PageGridData(result.ToArray(), totalCount.Value).JilToJson(); 
        }
        

        [HttpGet]
        public async Task<IActionResult> Detail(string id)
        {
            long searchId = id.ToInt64();
            if (id.IsEmptyZero())
            {
                return Content("");
            }
            else
            {
                Wms_mestask mesTask = await _client.Queryable<Wms_mestask>().FirstAsync(x => x.MesTaskId == SqlFunc.ToInt64(id)); 
                return View(mesTask);
            }
        }


        [HttpGet]
        public async Task<string> DetailList(string id, MESTaskTypes mesTaskType)
        {
            long searchMesId = id.ToInt64();
            if (id.IsEmptyZero())
            {
                return "";
            }
            //IWMSBaseApiAccessor[] proxies = WMSApiManager.GetAll(_client);

            if (mesTaskType == MESTaskTypes.StockIn)
            {
                List<Wms_stockin> stockins = await _client.Queryable<Wms_stockin>().Where(x => x.MesTaskId == searchMesId).ToListAsync();
                List<OutsideStockInQueryResult> totalResult = new List<OutsideStockInQueryResult>();
                foreach (Wms_stockin stockin in stockins)
                {
                    IWMSBaseApiAccessor proxy = WMSApiManager.GetBaseApiAccessor(stockin.WarehouseId.ToString(), _client, this.UserDto);
                    Wms_warehouse warehouse = proxy.Warehouse;
                    try
                    {
                        RouteData<OutsideStockInQueryResult> result = await proxy.QueryStockIn(stockin.StockInId);
                        result.Data.WarehouseName = warehouse.WarehouseName;
                        totalResult.Add(result.Data);
                    }
                    catch (Exception ex) {
                        this._logger.LogError(ex, $"查询入库信息失败,WarhouseId={stockin.WarehouseId},StockInId={stockin.StockInId}");
                        OutsideStockInQueryResult resultData = new OutsideStockInQueryResult()
                        {
                            WarehouseName = warehouse.WarehouseName,
                            StockInId = stockin.StockInId.ToString(),
                            StockInNo = stockin.StockInNo,
                            StockInTypeName = stockin.StockInTypeName,
                            StockInStatus = (StockInStatus)stockin.StockInStatus, 
                            MesTaskId = stockin.MesTaskId.ToString(), 
                            OrderNo = stockin.OrderNo, 
                            Remark = "详细信息查询失败,仅显示本地信息"
                        };
                        totalResult.Add(resultData);
                    }
                }
                return JsonConvert.SerializeObject(Bootstrap.GridData(totalResult, totalResult.Count));
            }
            else if (mesTaskType == MESTaskTypes.StockOut)
            {
                List<Wms_stockout> stockOuts = await _client.Queryable<Wms_stockout>().Where(x => x.MesTaskId == searchMesId).ToListAsync();
                List<OutsideStockOutQueryResult> totalResult = new List<OutsideStockOutQueryResult>();
                foreach (Wms_stockout stockout in stockOuts)
                {
                    IWMSBaseApiAccessor proxy = WMSApiManager.GetBaseApiAccessor(stockout.WarehouseId.ToString(), _client, this.UserDto);
                    Wms_warehouse warehouse = proxy.Warehouse;
                    try
                    {
                        RouteData<OutsideStockOutQueryResult> result = await proxy.QueryStockOut(stockout.StockOutId);
                        result.Data.WarehouseName = warehouse.WarehouseName;
                        totalResult.Add(result.Data);
                    }
                    catch (Exception ex) {
                        this._logger.LogError(ex, $"查询出库信息失败,WarhouseId={stockout.WarehouseId},StockOutId={stockout.StockOutId}");
                        OutsideStockOutQueryResult resultData = new OutsideStockOutQueryResult()
                        {
                            WarehouseName = warehouse.WarehouseName,
                            StockOutId = stockout.StockOutId.ToString(),
                            StockOutNo = stockout.StockOutNo,
                            StockOutTypeName = stockout.StockOutTypeName,
                            StockOutStatus = (StockOutStatus)stockout.StockOutStatus.Value ,
                            BatchNumber = stockout.BatchNumber,
                            BatchPlanId = stockout.BatchPlanId,
                            MesTaskId = stockout.MesTaskId.ToString(),
                            WorkNo = stockout.WorkNo,
                            WorkAreaName = stockout.WorkAreaName,
                            WorkStationId = stockout.WorkStationId,
                            OrderNo = stockout.OrderNo,
                            Remark = "详细信息查询失败,仅显示本地信息"
                        };
                        totalResult.Add(resultData);
                    }
                }
                return Bootstrap.GridData(totalResult, totalResult.Count).JilToJson();
            }
            else
            {
                return "";
            }
        }
    }
}
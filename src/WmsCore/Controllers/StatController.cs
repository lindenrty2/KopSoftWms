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
using Microsoft.Extensions.Logging;

namespace KopSoftWms.Controllers
{
    public class StatController : BaseController
    { 

        private readonly SqlSugarClient _client;
        private readonly ILogger _logger;
        public StatController(
            SqlSugarClient client,
            ILogger<StatController> logger
            )
        {
            _client = client;
            _logger = logger;
        }

        [HttpGet] 
        public async Task<IActionResult> Index()
        {
            try
            {
                var ibStat = await _client.Queryable<Wms_inventorybox>()
                    .GroupBy(x => x.Status)
                    .Select(x => new { Status = x.Status, Count = SqlFunc.AggregateCount(x.Status) })
                    .ToListAsync();

                ViewBag.TotalInventoryBox = ibStat.Sum(x => x.Count);
                ViewBag.OutingInventoryBox = ibStat.FirstOrDefault(x => x.Status == (int)InventoryBoxStatus.Outing)?.Count ?? 0;
                ViewBag.OutedInventoryBox = ibStat.FirstOrDefault(x => x.Status == (int)InventoryBoxStatus.Outed)?.Count ?? 0;
                ViewBag.BackingInventoryBox = ibStat.FirstOrDefault(x => x.Status == (int)InventoryBoxStatus.Backing)?.Count ?? 0;
                ViewBag.NoneInventoryBox = ibStat.FirstOrDefault(x => x.Status == (int)InventoryBoxStatus.None)?.Count ?? 0;

                ViewBag.UsedInventoryBox = await _client.Queryable<Wms_inventorybox>()
                    .Where(x => x.UsedSize != 0)
                    .CountAsync();
                ViewBag.StockInCount = await _client.Queryable<Wms_stockin>().Where(
                    x => x.StockInDate >= DateTime.Today && x.StockInDate <= DateTime.Today.AddDays(1))
                    .CountAsync();
                ViewBag.StockInedCount = await _client.Queryable<Wms_stockin>().Where(
                    x => x.StockInDate >= DateTime.Today && x.StockInDate <= DateTime.Today.AddDays(1)
                    && x.StockInStatus == (int)StockInStatus.task_finish)
                    .CountAsync();
                ViewBag.StockOutCount = await _client.Queryable<Wms_stockout>().Where(
                    x => x.StockOutDate >= DateTime.Today && x.StockOutDate <= DateTime.Today.AddDays(1))
                    .CountAsync();
                ViewBag.StockOutedCount = await _client.Queryable<Wms_stockout>().Where(
                    x => x.StockOutDate >= DateTime.Today && x.StockOutDate <= DateTime.Today.AddDays(1)
                    && x.StockOutStatus == (int)StockOutStatus.task_finish)
                    .CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"获取统计数据异常");
            }
            return View();
        }
         
    }
}
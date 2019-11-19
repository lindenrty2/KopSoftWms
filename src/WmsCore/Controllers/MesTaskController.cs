using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using YL.Core.Dto;
using YL.Core.Entity;
using YL.NetCore.Attributes;
using YL.NetCore.NetCoreApp;
using YL.Utils.Pub;
using YL.Utils.Table;
using YL.Utils.Json;

namespace WMSCore.Controllers
{
    public class MesTaskController : BaseController
    {
        private readonly SqlSugarClient _client;
        public MesTaskController(SqlSugarClient client)
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
            //Order
            RefAsync<int> totalCount = new RefAsync<int>();
            List<Wms_mestask> result = await query.ToPageListAsync(bootstrap.pageIndex, bootstrap.limit, totalCount);
            return new PageGridData(result.ToArray(), totalCount.Value).JilToJson();


        }


    }
}
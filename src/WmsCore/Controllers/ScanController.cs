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

namespace WMSCore.Controllers
{

    public class ScanController : BaseController
    {
        SqlSugarClient _client;
        public ScanController(SqlSugarClient client)
        {
            _client = client;
        }

        /// <summary>
        /// 扫描完成处理
        /// </summary>
        /// <param name="stockInId"></param>
        /// <param name="inventoryBoxId"></param>
        /// <param name="materials"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<RouteData> DoScanComplate(int mode, long stockId, long inventoryBoxId, Wms_StockMaterialDetailDto[] materials, string remark)
        {
            try
            {
                _client.BeginTran(); 


                _client.CommitTran();
                return null;
            }
            catch (Exception)
            {
                _client.RollbackTran();
                return YL.Core.Dto.RouteData.From(PubMessages.E2005_STOCKIN_BOXOUT_FAIL);
            }

        }


        [HttpPost]
        public async Task<RouteData> DoComplate(long storeId, long stockInId)
        {
            try
            {
                _client.BeginTran();

                Wms_stockin stockin = await _client.Queryable<Wms_stockin>().FirstAsync(x => x.StockInId == stockInId);
                if (stockin == null) { return YL.Core.Dto.RouteData.From(PubMessages.E2013_STOCKIN_NOTFOUND); }
                if (stockin.StockInStatus == StockInStatus.task_finish.ToByte()) { return YL.Core.Dto.RouteData.From(PubMessages.E2014_STOCKIN_ALLOW_FINISHED); }

                stockin.StockInStatus = StockInStatus.task_finish.ToByte();
                if (_client.Updateable(stockin).ExecuteCommand() == 0)
                {
                    return YL.Core.Dto.RouteData.From(PubMessages.E2017_STOCKIN_FAIL, "入库单详细状态更新失败");
                }
                Wms_stockindetail[] details = _client.Queryable<Wms_stockindetail>().Where(x => x.StockInId == stockin.StockInId).ToArray();
                foreach (Wms_stockindetail detail in details)
                {
                    if (detail.Status == StockInStatus.task_finish.ToByte()) continue;
                    detail.Status = StockInStatus.task_finish.ToByte();
                    detail.ModifiedBy = this.UserDto.UserId;
                    detail.ModifiedDate = DateTime.Now;
                    if (_client.Updateable(detail).ExecuteCommand() == 0)
                    {
                        return YL.Core.Dto.RouteData.From(PubMessages.E2017_STOCKIN_FAIL, "入库单详细状态更新失败");
                    }
                }
                _client.CommitTran();
                return YL.Core.Dto.RouteData.From(PubMessages.I2002_STOCKIN_SCCUESS);
            }
            catch (Exception ex)
            {
                _client.RollbackTran();
                return YL.Core.Dto.RouteData.From(PubMessages.E2017_STOCKIN_FAIL, ex.Message);
            } 
        }


    }
}
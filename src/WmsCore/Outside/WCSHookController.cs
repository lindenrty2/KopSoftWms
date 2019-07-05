using IServices;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Threading.Tasks;
using WebApiClient;
using YL.Core.Dto;
using YL.Core.Entity;
using YL.Utils.Extensions;
using YL.Utils.Pub;

namespace WMSCore.Outside
{
    [Route("/hook/wcs")]
    public class WCSHookController : Controller
    {

        private readonly IWms_stockinServices _stockinServices;
        private readonly IWms_stockindetailServices _stockindetailServices;
        private readonly IWms_stockindetailboxServices _stockindetailboxServices;
        private readonly IWms_inventoryBoxServices _inventoryBoxServices;
        private readonly IWms_inventoryBoxTaskServices _inventoryBoxTaskServices;
        private readonly SqlSugarClient _client;
        public WCSHookController(
            SqlSugarClient client,
            IWms_stockinServices stockinServices,
            IWms_stockindetailServices stockindetailServices,
            IWms_stockindetailboxServices stockindetailboxServices,
            IWms_inventoryBoxServices inventoryBoxServices,
            IWms_inventoryBoxTaskServices inventoryBoxTaskServices)
        {
            _client = client;
            _stockinServices = stockinServices;
            _stockindetailServices = stockindetailServices;
            _stockindetailboxServices = stockindetailboxServices;
            _inventoryBoxServices = inventoryBoxServices;
            _inventoryBoxTaskServices = inventoryBoxTaskServices;
        }

        [HttpGet("ping")]
        public string Ping(string s)
        {
            return "OK";
        }

        /// <summary>
        /// 确认出库完成
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        [HttpPost("confirmOutStock")]
        public Task<ConfirmOutStockResult> ConfirmOutStock(WCSTaskResult result)
        {
            try
            {
                long taskId = result.TaskId.ToInt64();
                if (taskId == 0)
                {
                    return Task.FromResult(new ConfirmOutStockResult(PubMessages.E2302_WCS_TASKID_INVAILD));
                }
                Wms_inventoryboxTask boxTask = _inventoryBoxTaskServices.QueryableToEntity(x => x.InventoryBoxTaskId == taskId);
                if (boxTask == null)
                {
                    return Task.FromResult(new ConfirmOutStockResult(PubMessages.E2303_WCS_STOCKOUTTASK_NOTFOUND));
                }
                if (boxTask.Status != InventoryBoxTaskStatus.task_outing.ToByte() && boxTask.Status != InventoryBoxTaskStatus.task_outed.ToByte())
                {
                    return Task.FromResult(new ConfirmOutStockResult(PubMessages.E2304_WCS_STOCKOUTTASK_NOTOUT));
                }

                Wms_inventorybox box = _inventoryBoxServices.QueryableToEntity(x => x.InventoryBoxId == boxTask.InventoryBoxId);
                if (box == null)
                {
                    return Task.FromResult(new ConfirmOutStockResult(PubMessages.E1011_INVENTORYBOX_NOTFOUND));
                }
                if (box.Status != InventoryBoxStatus.Outing && box.Status != InventoryBoxStatus.Outed)
                {
                    return Task.FromResult(new ConfirmOutStockResult(PubMessages.E1008_INVENTORYBOX_NOTOUT));
                }

                _client.BeginTran();

                boxTask.Status = InventoryBoxTaskStatus.task_outed.ToByte();
                boxTask.ModifiedBy = PubConst.InterfaceUserId;
                boxTask.ModifiedDate = DateTime.Now;
                _inventoryBoxTaskServices.UpdateEntity(boxTask);

                box.Status = InventoryBoxStatus.Outed;
                box.ModifiedBy = PubConst.InterfaceUserId;
                box.ModifiedDate = DateTime.Now;
                _inventoryBoxServices.UpdateEntity(box);

                _client.CommitTran();
                return Task.FromResult(new ConfirmOutStockResult());
            }
            catch(Exception ex)
            {
                _client.RollbackTran();
                return Task.FromResult(new ConfirmOutStockResult() {
                    Successd = false,
                    Code = "-1",
                    ErrorDesc = ex.ToString()
                });
            }
        }

        /// <summary>
        /// 确认归库完成
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        [HttpPost("confirmBackStock")]
        public Task<ConfirmBackStockResult> ConfirmBackStock(WCSTaskResult result)
        {
            try
            {
                long taskId = result.TaskId.ToInt64();
                if (taskId == 0)
                {
                    return Task.FromResult(new ConfirmBackStockResult(PubMessages.E2302_WCS_TASKID_INVAILD));
                }
                Wms_inventoryboxTask boxTask = _inventoryBoxTaskServices.QueryableToEntity(x => x.InventoryBoxTaskId == taskId);
                if (boxTask == null)
                {
                    return Task.FromResult(new ConfirmBackStockResult(PubMessages.E2311_WCS_STOCKBACKTASK_NOTFOUND));
                }
                if (boxTask.Status != InventoryBoxTaskStatus.task_backing.ToByte() && boxTask.Status != InventoryBoxTaskStatus.task_backed.ToByte())
                {
                    return Task.FromResult(new ConfirmBackStockResult(PubMessages.E2312_WCS_STOCKBACKTASK_NOTBACK));
                }

                Wms_inventorybox box = _inventoryBoxServices.QueryableToEntity(x => x.InventoryBoxId == boxTask.InventoryBoxId);
                if (box == null)
                {
                    return Task.FromResult(new ConfirmBackStockResult(PubMessages.E1011_INVENTORYBOX_NOTFOUND));
                }
                if (box.Status != InventoryBoxStatus.Backing && box.Status != InventoryBoxStatus.InPosition)
                {
                    return Task.FromResult(new ConfirmBackStockResult(PubMessages.E1008_INVENTORYBOX_NOTOUT));
                }

                _client.BeginTran();

                boxTask.Status = InventoryBoxTaskStatus.task_backed.ToByte();
                boxTask.ModifiedBy = PubConst.InterfaceUserId;
                boxTask.ModifiedDate = DateTime.Now;
                _inventoryBoxTaskServices.UpdateEntity(boxTask);

                box.Status = InventoryBoxStatus.InPosition;
                box.ModifiedBy = PubConst.InterfaceUserId;
                box.ModifiedDate = DateTime.Now;
                _inventoryBoxServices.UpdateEntity(box);

                _client.CommitTran();
                return Task.FromResult(new ConfirmBackStockResult());
            }
            catch (Exception ex)
            {
                _client.RollbackTran();
                return Task.FromResult(new ConfirmBackStockResult()
                {
                    Successd = false,
                    Code = "-1",
                    ErrorDesc = ex.ToString()
                });
            }
        }

        [HttpPost("LogisticsFinish")]
        public async Task<OutsideLogisticsFinishResponseResult> LogisticsFinish(OutsideLogisticsFinishResponse arg)
        {
            return await MESApiAccessor.Instance.LogisticsFinish(arg);
        }

    } 
}
using EventBus;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IServices.Outside;
using YL.Core.Dto;

namespace InterfaceMocker.Service
{
    [Route("/{controller}")]
    public class WCSController : Controller
    {
        public static SimpleEventBus _eventBus = SimpleEventBus.GetDefaultEventBus();

        static WCSController()
        {
        }

        [HttpGet("ping")]
        public string Ping()
        {
            return "OK";
        }

        [HttpPost("CreateStockOut.ashx")]
        public CreateOutStockResult CreateOutStock([FromBody]OutStockInfo outStockInfo)
        {
            CreateOutStockResult result = new CreateOutStockResult()
            {
                Successd = true
            };
            _eventBus.Post(new KeyValuePair<OutStockInfo, CreateOutStockResult>(outStockInfo, result), TimeSpan.Zero);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="backStockInfo"></param>
        /// <returns></returns>
        [HttpPost("CreateStockIn.ashx")]
        public CreateBackStockResult CreateStockIn([FromBody]BackStockInfo backStockInfo)
        {
            CreateBackStockResult result = new CreateBackStockResult()
            {
                Successd = true
            };
            _eventBus.Post(new KeyValuePair<BackStockInfo, CreateBackStockResult>(backStockInfo, result), TimeSpan.Zero);
            return result;
        }

        public static Dictionary<string, LogisticsTask> _logistics = new Dictionary<string, LogisticsTask>();

        /// <summary>
        /// 物流控制
        /// </summary>
        [HttpPost("LogisticsControlWCS.ashx")]
        public async Task<OutsideLogisticsControlResult> LogisticsControl([FromBody]OutsideLogisticsControlArg arg)
        {
            string equipmentId = "E" + DateTime.Now.ToString("HHmmss");
            string equipmentName = "设备" + DateTime.Now.ToString("HHmmss");
            lock (_logistics)
            {
                if (!_logistics.ContainsKey(arg.LogisticsId))
                {
                    _logistics.Add(arg.LogisticsId, new LogisticsTask() { EquipmentId = equipmentId, EquipmentName = equipmentName });
                }
            }
            OutsideLogisticsControlResult result = new OutsideLogisticsControlResult()
            {
                Success = true,
                EquipmentId = equipmentId,
                EquipmentName = equipmentName,
                IsNormalExecution = true
            };
            _eventBus.Post(new KeyValuePair<OutsideLogisticsControlArg, OutsideLogisticsControlResult>(arg, result), TimeSpan.Zero);
            return result;
        }

        /// <summary>
        /// 物流（状态）查询
        /// </summary>
        [HttpPost("LogisticsEnquiryWCS.ashx")]
        public async Task<OutsideLogisticsEnquiryResult> LogisticsEnquiry([FromBody] OutsideLogisticsEnquiryArg arg)
        {
            LogisticsTask task = null;
            lock (_logistics)
            {
                if (_logistics.ContainsKey(arg.LogisticsId))
                {
                    task = _logistics[arg.LogisticsId];
                }
            }
            if(task == null)
            {
                return new OutsideLogisticsEnquiryResult()
                {
                    Status = "Fail",
                    EquipmentId = arg.EquipmentId,
                    EquipmentName = arg.EquipmentName,
                    Position = null
                };
            }
            OutsideLogisticsEnquiryResult result = new OutsideLogisticsEnquiryResult()
            {
                Status = task.Step == -1 ? "OK" : "Processing",
                EquipmentId = task.EquipmentId,
                EquipmentName = task.EquipmentName,
                Position = task.Step == -1 ? "终点" : $"坐标{task.Step}"
            };

            if (task.Step >= 0)
            {
                task.Step++;
            }
            _eventBus.Post(new KeyValuePair<OutsideLogisticsEnquiryArg, OutsideLogisticsEnquiryResult>(arg, result), TimeSpan.Zero);
            return result;
        }

    } 

    public class LogisticsTask
    {
        public string EquipmentId { get; set; }

        public string EquipmentName { get; set; }

        public int Step { get; set; } = 1;
         
    }

}

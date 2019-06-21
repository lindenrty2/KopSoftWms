using EventBus;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using WMSCore.Outside;

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

        [HttpPost("createOutStock")]
        public CreateOutStockResult CreateOutStock(OutStockInfo outStockInfo)
        {
            CreateOutStockResult result = new CreateOutStockResult()
            {
                Successd = true
            };
            _eventBus.Post(new KeyValuePair<OutStockInfo, CreateOutStockResult>(outStockInfo, result), TimeSpan.Zero);
            return result;
        }

        [HttpPost("createBackStock")]
        public CreateBackStockResult CreateBackStock(BackStockInfo backStockInfo)
        {
            CreateBackStockResult result = new CreateBackStockResult()
            {
                Successd = true
            };
            _eventBus.Post(new KeyValuePair<BackStockInfo, CreateBackStockResult>(backStockInfo, result), TimeSpan.Zero);
            return result;
        }
    }


}

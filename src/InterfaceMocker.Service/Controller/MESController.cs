using EventBus;
using InterfaceMocker.Service.Do;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using YL.Core.Dto;

namespace InterfaceMocker.Service
{ 
    public class MESController : Controller , IMESController
    {
        public static SimpleEventBus _eventBus = SimpleEventBus.GetDefaultEventBus();
        [HttpGet]
        public IActionResult GetMaterialInfo(int rfid)
        {
            RetMaterial retModel = new RetMaterial();
            if (rfid == 0)
            {
                retModel.Success = false;
                retModel.ErrorCode = "1";
                retModel.ErrorDesc = "参数不可为0";
            }

            string testStr = JsonConvert.SerializeObject(retModel);

            //样例返回
            return Ok(retModel);
        }

        [HttpPost]
        public OutSideStockInResponseResult ConfirmBalanceMES(OutSideStockInResponse obj)
        {
            OutSideStockInResponseResult retModel = new OutSideStockInResponseResult();
            retModel.WarehousingId = obj.WarehousingId;
            retModel.IsNormalExecution = true;
            _eventBus.Post(new KeyValuePair<OutSideStockInResponse, OutSideStockInResponseResult>(obj, retModel), TimeSpan.Zero);
            return retModel;
        }

        [HttpPost]
        public OutSideStockOutResponseResult ConfirmOutStockMES(OutSideStockOutResponse obj)
        {
            OutSideStockOutResponseResult retModel = new OutSideStockOutResponseResult();
            retModel.WarehouseEntryId = obj.WarehouseEntryId;
            retModel.IsNormalExecution = true;
            _eventBus.Post(new KeyValuePair<OutSideStockOutResponse, OutSideStockOutResponseResult>(obj, retModel), TimeSpan.Zero);
            return retModel;
        }
    }

    [ServiceContract]
    public interface IMESController
    {
        [OperationContract]
        IActionResult GetMaterialInfo(int rfid);

        [OperationContract]
        OutSideStockInResponseResult ConfirmBalanceMES(OutSideStockInResponse obj);

        [OperationContract]
        OutSideStockOutResponseResult ConfirmOutStockMES(OutSideStockOutResponse obj);
    }
}

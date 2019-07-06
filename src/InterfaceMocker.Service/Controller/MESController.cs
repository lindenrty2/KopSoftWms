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
        public OutsideStockInResponseResult ConfirmBalanceMES(OutsideStockInResponse obj)
        {
            OutsideStockInResponseResult retModel = new OutsideStockInResponseResult();
            retModel.WarehousingId = obj.WarehousingId;
            retModel.IsNormalExecution = true;
            _eventBus.Post(new KeyValuePair<OutsideStockInResponse, OutsideStockInResponseResult>(obj, retModel), TimeSpan.Zero);
            return retModel;
        }

        [HttpPost]
        public OutsideStockOutResponseResult ConfirmOutStockMES(OutsideStockOutResponse obj)
        {
            OutsideStockOutResponseResult retModel = new OutsideStockOutResponseResult();
            retModel.WarehouseEntryId = obj.WarehouseEntryId;
            retModel.IsNormalExecution = true;
            _eventBus.Post(new KeyValuePair<OutsideStockOutResponse, OutsideStockOutResponseResult>(obj, retModel), TimeSpan.Zero);
            return retModel;
        }

        /// <summary>
        /// 物流控制完成
        /// </summary>
        public OutsideLogisticsFinishResponseResult LogisticsFinish(OutsideLogisticsFinishResponse obj)
        {
            OutsideLogisticsFinishResponseResult retModel = new OutsideLogisticsFinishResponseResult();
            retModel.LogisticsId = obj.LogisticsId;
            retModel.IsNormalExecution = true;
            _eventBus.Post(new KeyValuePair<OutsideLogisticsFinishResponseResult,OutsideLogisticsFinishResponse>(retModel,obj), TimeSpan.Zero);
            return retModel;
        }
    }

    [ServiceContract]
    public interface IMESController
    {
        [OperationContract]
        IActionResult GetMaterialInfo(int rfid);

        [OperationContract]
        OutsideStockInResponseResult ConfirmBalanceMES(OutsideStockInResponse obj);

        [OperationContract]
        OutsideStockOutResponseResult ConfirmOutStockMES(OutsideStockOutResponse obj);
    }
}

using InterfaceMocker.Service.Do;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.ServiceModel;

namespace InterfaceMocker.Service
{ 
    public class MESController : Controller , IMESController
    { 
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
        public IActionResult ConfirmBalanceMES(dynamic obj)
        {
            RetValue retModel = new RetValue();
            retModel.Returncode = Convert.ToString("1");
            return Ok(retModel);
        }

        [HttpPost]
        public IActionResult ConfirmOutStockMES(dynamic obj)
        {
            RetValue retModel = new RetValue();
            retModel.Returncode = Convert.ToString("1");

            return Ok(retModel);
        }
    }

    [ServiceContract]
    public interface IMESController
    {
        [OperationContract]
        IActionResult GetMaterialInfo(int rfid);

        [OperationContract]
        IActionResult ConfirmBalanceMES(object obj);

        [OperationContract]
        IActionResult ConfirmOutStockMES(object obj);
    }
}

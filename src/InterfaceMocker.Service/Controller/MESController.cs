using InterfaceMocker.Service.Do;
using Microsoft.Owin.Hosting;
using Newtonsoft.Json;
using Owin;
using System;
using System.Collections.Generic;
using System.Net.Http.Formatting; 
using System.Text;
using System.Web.Http;
using System.Web.Http.Cors;

namespace InterfaceMocker.Service
{  
    public class MESController : ApiController
    { 
        [HttpGet]
        public IHttpActionResult GetMaterialInfo(int rfid)
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
        public IHttpActionResult ConfirmBalanceMES(dynamic obj)
        {
            RetValue retModel = new RetValue();
            retModel.Returncode = Convert.ToString("1");
            return Ok(retModel);
        }

        [HttpPost]
        public IHttpActionResult ConfirmOutStockMES(dynamic obj)
        {
            RetValue retModel = new RetValue();
            retModel.Returncode = Convert.ToString("1");

            return Ok(retModel);
        }
    }
}

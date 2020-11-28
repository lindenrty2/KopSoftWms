using IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using YL.Core.Entity.Fluent.Validation;
using YL.Utils.Env;
using YL.Utils.Extensions;
using YL.Utils.Http;
using YL.Utils.Pub;

namespace YL.NetCore.Attributes
{
    public sealed class OutsideLogAttribute : ResultFilterAttribute
    {
        public bool Ignore { get; set; } = true;
        public LogType LogType { get; set; }

        public OutsideLogAttribute()
        {
        }

        public OutsideLogAttribute(bool ignore)
        {
            Ignore = ignore;
        }

        public OutsideLogAttribute(LogType logType)
        {
            LogType = logType;
        }

        public OutsideLogAttribute(bool ignore, LogType logType)
        {
            Ignore = ignore;
            LogType = logType;
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            var services = context.HttpContext.RequestServices;
            var config = services.GetService(typeof(IConfiguration)) as IConfiguration;
            var claims = context.HttpContext.User.Claims;
            string flag = config["Log:operationlog"];
            if (string.IsNullOrWhiteSpace(flag))
            {
                flag = "false";
            }
            if (flag.Equals("true", StringComparison.OrdinalIgnoreCase))
            {
                if (Ignore)
                {
                    //传入参数
                    var parameters = context.ReadResultExecutingContext();
                    //result
                    var result = context.Result;
                    object res = null;
                    if (result is ObjectResult objectResult)
                    {
                        res = objectResult.Value;
                    }
                    else if (result is ContentResult contentResult)
                    {
                        res = contentResult.Content;
                    }
                    else if (result is EmptyResult emptyResult)
                    {
                        res = emptyResult;
                    }
                    else if (result is StatusCodeResult statusCodeResult)
                    {
                        res = statusCodeResult;
                    }
                    else if (result is JsonResult jsonResult)
                    {
                        res = jsonResult.Value.ToString();
                    }
                    else if (result is FileResult fileResult)
                    {
                        res = fileResult.FileDownloadName.IsEmpty() ? fileResult.ContentType : fileResult.FileDownloadName;
                    }
                    else if (result is ViewResult viewResult)
                    {
                        res = viewResult.Model;
                    }
                    else if (result is RedirectResult redirectResult)
                    {
                        res = redirectResult.Url;
                    }
                    //try
                    //{ 
                    //    LogData logData = new LogData();
                    //    logData.parameters = parameters;
                    //    logData.logIp = GlobalCore.GetIp();
                    //    logData.target = parameters.Item2;
                    //    logData.result = res;

                    //    String dir = Path.Combine(AppContext.BaseDirectory, "OutsideLog");
                    //    if (!Directory.Exists(dir))
                    //    {
                    //        Directory.CreateDirectory(dir);
                    //    }
                    //    String path = Path.Combine(dir, DateTime.Now.Ticks.ToString() + "_" + logData.logId.ToString());
                    //    File.WriteAllText(path, JsonConvert.SerializeObject(logData));
                    //}
                    //catch (Exception) {
                    //}
                }
            }
            base.OnResultExecuting(context);
        }

        public class LogData {
            private static long _slogId = 0;

            public long logId = 0;
            public object parameters;
            public object result;
            public string logIp;  
            public string target;

            public LogData() {
                _slogId++;
                logId = _slogId;
            }
        }
    }
}
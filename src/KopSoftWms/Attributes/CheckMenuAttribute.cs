using IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System.Linq;
using System.Security.Claims;
using YL.Utils.Extensions;
using YL.Utils.Pub;

namespace YL.NetCore.Attributes
{
    public class CheckMenuAttribute : ActionFilterAttribute
    {
        public CheckMenuAttribute()
        {
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var viewData = (context.Controller as Controller)?.ViewData;
            var viewBag = (context.Controller as Controller)?.ViewBag;
            var services = context.HttpContext.RequestServices;
            //var log = services.GetService(typeof(ILoggerFactory)) as ILoggerFactory;
            //ILogger<CheckMenuAttribute> logger = log.CreateLogger<CheckMenuAttribute>();
            //logger.LogInformation("");
            string type = "sys";
            var queryDictionary = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(context.HttpContext.Request.QueryString.Value);
            
            if (queryDictionary.TryGetValue("type",out StringValues tryType))
            {
                if (tryType.Count > 0)
                {
                    type = tryType[0];
                    viewData["type"] = type;
                }
            }
            long currentStoreId = 0;
            if (queryDictionary.TryGetValue("storeId", out StringValues tryStoreId))
            {
                if (tryStoreId.Count > 0)
                {
                    currentStoreId = tryStoreId[0].ToInt64();
                    viewData["currentStoreId"] = type;
                }
            }
            var properties = context.ActionDescriptor.Properties;
            var claims = context.HttpContext.User?.Claims;
            var cache = services.GetService(typeof(IMemoryCache)) as IMemoryCache;
            var roleServices = services.GetService(typeof(ISys_roleServices)) as ISys_roleServices;
            var warehouseServices = services.GetService(typeof(IWms_warehouseServices)) as IWms_warehouseServices;
            if (viewData != null)
            {
                if (context.HttpContext.User != null)
                {
                    // ReSharper disable once PossibleNullReferenceException
                    viewData["menu"] = cache.Get(type + "menu") ?? roleServices?.GetMenu(claims.SingleOrDefault(c => c.Type == ClaimTypes.Role).Value.ToInt64(), type + "_menu");
                    viewData["stores"] = cache.Get("stores") ?? warehouseServices?.Queryable().ToList().ToArray();
                }
            }
            if (viewBag != null)
            {
                viewBag.title = properties["title"].ToString();
                viewBag.company = properties["company"].ToString();
                viewBag.customer = properties["customer"].ToString();
                viewBag.nickname = claims.SingleOrDefault(c => c.Type == ClaimTypes.Name).Value;
                viewBag.headimg = claims.SingleOrDefault(c => c.Type == ClaimTypes.Uri).Value;
            }
            base.OnActionExecuting(context);
        }
    }
}
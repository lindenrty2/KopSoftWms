using IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YL.Core.Entity;
using YL.Utils.Extensions;

namespace KopSoftWms.ViewComponents
{
    public class WarehouseViewComponent : ViewComponent
    {
        private readonly IWms_warehouseServices _warehouseServices;

        public WarehouseViewComponent(IWms_warehouseServices warehouseServices)
        {
            _warehouseServices = warehouseServices;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var queryDictionary = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(HttpContext.Request.QueryString.Value);

            long? currentStoreId = null;
            if (queryDictionary.TryGetValue("storeId", out StringValues tryStoreId))
            {
                if (tryStoreId.Count > 0)
                {
                    currentStoreId = tryStoreId[0].ToInt64(); 
                }
            }

            List<Wms_warehouse> model = await _warehouseServices.QueryableToList(c => c.IsDel == 1).ToListAsync(); 
            if (currentStoreId.HasValue)
            {
                model = model.Where(x => x.WarehouseId == currentStoreId.Value).ToList();
            }
            return View(model);
        }
    }
}
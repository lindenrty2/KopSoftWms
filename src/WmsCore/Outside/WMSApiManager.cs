using IServices.Outside;
using Services.Outside;
using SqlSugar;
using System.Collections.Generic;
using YL.Core.Entity;

namespace WMSCore.Outside
{
    public class WMSApiManager
    {
        public static Dictionary<string, IWMSApiProxy> _instanceMap = new Dictionary<string, IWMSApiProxy>();
        public static Dictionary<string, Wms_warehouse> _warehouseMap = new Dictionary<string, Wms_warehouse>();
        public static IWMSApiProxy Get(string key,ISqlSugarClient sqlSugar)
        {
            if(_instanceMap.ContainsKey(key))
            {
                return _instanceMap[key];
            }
            if (!_warehouseMap.ContainsKey(key))
            {
                return null;
            }
            Wms_warehouse warehouse = _warehouseMap[key];
            IWMSApiProxy accessor = null;
            if (string.IsNullOrWhiteSpace(warehouse.IFAddress))
            {
                accessor = new SelfWMSApiAccessor(warehouse,sqlSugar);
            }
            else
            {
                accessor = new WMSApiAccessor(warehouse);
                _instanceMap.Add(key, accessor);
            }
            return accessor;
        }
        

        public static void Regist(Wms_warehouse warehouse)
        {
            _warehouseMap.Add(warehouse.WarehouseId.ToString(),warehouse);
        }
         
    }
}

using IServices.Outside;
using Services.Outside;
using SqlSugar;
using System;
using System.Collections.Generic;
using YL.Core.Entity;

namespace WMSCore.Outside
{
    public class WMSApiManager
    {
        public static Dictionary<string, IWMSApiAccessor> _instanceMap = new Dictionary<string, IWMSApiAccessor>();
        public static Dictionary<string, Wms_warehouse> _warehouseMap = new Dictionary<string, Wms_warehouse>();
        public static IWMSApiAccessor Get(string key,ISqlSugarClient sqlSugar)
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
            IWMSApiAccessor accessor = null;
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

        public static IWMSApiAccessor[] GetAll(ISqlSugarClient client)
        {
            List<IWMSApiAccessor> accessorList = new List<IWMSApiAccessor>();
            foreach (KeyValuePair<string,Wms_warehouse> keyValuePair in _warehouseMap)
            {
                accessorList.Add(Get(keyValuePair.Key, client));
            }
            return accessorList.ToArray();
        }
    }
}

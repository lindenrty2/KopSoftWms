using IServices.Outside;
using Services.Outside;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using YL.Core.Dto;
using YL.Core.Entity;

namespace WMSCore.Outside
{
    public class WMSApiManager
    {
        private static Dictionary<string, IWMSBaseApiAccessor> _baseApiInstanceMap = new Dictionary<string, IWMSBaseApiAccessor>();
        private static Dictionary<string, IWMSOperationApiAccessor> _operationApiInstanceMap = new Dictionary<string, IWMSOperationApiAccessor>();
        private static Dictionary<string, IWMSManagementApiAccessor> _managementApiInstanceMap = new Dictionary<string, IWMSManagementApiAccessor>();
        private static Dictionary<string, Wms_warehouse> _warehouseMap = new Dictionary<string, Wms_warehouse>();

        /// <summary>
        /// 获取仓库信息
        /// </summary>
        /// <param name="warehouseNo"></param>
        /// <returns></returns>
        public static Wms_warehouse GetWarehouse(string warehouseNo)
        {
            return _warehouseMap.Values.FirstOrDefault(x => x.WarehouseNo == warehouseNo);
        }

        /// <summary>
        /// 获取仓库信息
        /// </summary>
        /// <param name="warehouseId"></param>
        /// <returns></returns>
        public static Wms_warehouse GetWarehouse(long warehouseId)
        {
            return _warehouseMap[warehouseId.ToString()];
        }

        /// <summary>
        /// 获取基本Api
        /// </summary>
        /// <param name="key"></param>
        /// <param name="sqlSugar"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static IWMSBaseApiAccessor GetBaseApiAccessor(string key,ISqlSugarClient sqlSugar,SysUserDto user = null)
        {
            if(_baseApiInstanceMap.ContainsKey(key))
            {
                return _baseApiInstanceMap[key];
            }
            if (!_warehouseMap.ContainsKey(key))
            {
                return null;
            }
            Wms_warehouse warehouse = _warehouseMap[key];
            IWMSBaseApiAccessor accessor = null;
            if (string.IsNullOrWhiteSpace(warehouse.IFAddress))
            {
                accessor = new SelfWMSBaseApiAccessor(warehouse,sqlSugar, user);
            }
            else
            {
                accessor = new WMSBaseApiAccessor(warehouse, sqlSugar, user);
                _baseApiInstanceMap.Add(key, accessor);
            }
            return accessor;
        }

        /// <summary>
        /// 获取操作API
        /// </summary>
        /// <param name="key"></param>
        /// <param name="sqlSugar"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static IWMSOperationApiAccessor GetOperationApiAccessor(string key, ISqlSugarClient sqlSugar, SysUserDto user)
        {
            if (_operationApiInstanceMap.ContainsKey(key))
            {
                return _operationApiInstanceMap[key];
            }
            if (!_warehouseMap.ContainsKey(key))
            {
                return null;
            }
            Wms_warehouse warehouse = _warehouseMap[key];
            IWMSOperationApiAccessor accessor = null;
            if (string.IsNullOrWhiteSpace(warehouse.IFAddress))
            {
                accessor = new SelfWMSOperationApiAccessor(warehouse, sqlSugar, user);
            }
            else
            {
                //TODO 操作API外部接口,第三方库暂不使用
                accessor = null; // new WMSApiAccessor(warehouse);
                _operationApiInstanceMap.Add(key, accessor);
            }
            return accessor;
        }

        /// <summary>
        /// 获取管理API
        /// </summary>
        /// <param name="key"></param>
        /// <param name="sqlSugar"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static IWMSManagementApiAccessor GetManagementApiAccessor(string key, ISqlSugarClient sqlSugar, SysUserDto user)
        {
            if (_managementApiInstanceMap.ContainsKey(key))
            {
                return _managementApiInstanceMap[key];
            }
            if (!_warehouseMap.ContainsKey(key))
            {
                return null;
            }
            Wms_warehouse warehouse = _warehouseMap[key];
            IWMSManagementApiAccessor accessor = null;
            if (string.IsNullOrWhiteSpace(warehouse.IFAddress))
            {
                accessor = new SelfWMSManagementApiAccessor(warehouse, sqlSugar, user);
            }
            else
            {
                //TODO 操作API外部接口,第三方库暂不使用
                accessor = null; // new WMSApiAccessor(warehouse);
                _managementApiInstanceMap.Add(key, accessor);
            }
            return accessor;
        }


        public static void Regist(Wms_warehouse warehouse)
        {
            _warehouseMap.Add(warehouse.WarehouseId.ToString(),warehouse);
        }

        public static IWMSBaseApiAccessor[] GetAll(ISqlSugarClient client)
        {
            List<IWMSBaseApiAccessor> accessorList = new List<IWMSBaseApiAccessor>();
            foreach (KeyValuePair<string,Wms_warehouse> keyValuePair in _warehouseMap)
            {
                accessorList.Add(GetBaseApiAccessor(keyValuePair.Key, client));
            }
            return accessorList.ToArray();
        }
    }
}

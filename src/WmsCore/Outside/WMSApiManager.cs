using IServices.Outside;
using Services.Outside;
using SqlSugar;
using System;
using System.Collections.Generic;
using YL.Core.Dto;
using YL.Core.Entity;

namespace WMSCore.Outside
{
    public class WMSApiManager
    {
        public static Dictionary<string, IWMSBaseApiAccessor> _baseApiInstanceMap = new Dictionary<string, IWMSBaseApiAccessor>();
        public static Dictionary<string, IWMSOperationApiAccessor> _operationApiInstanceMap = new Dictionary<string, IWMSOperationApiAccessor>();
        public static Dictionary<string, IWMSManagementApiAccessor> _managementApiInstanceMap = new Dictionary<string, IWMSManagementApiAccessor>();
        public static Dictionary<string, Wms_warehouse> _warehouseMap = new Dictionary<string, Wms_warehouse>();

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
                accessor = new WMSBaseApiAccessor(warehouse);
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
                //TODO 操作API外部接口
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
                //TODO 操作API外部接口
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

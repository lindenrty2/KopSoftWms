﻿using IServices;
using IServices.Outside;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Services.Outside;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using WebApiClient;
using YL.Core.Dto;
using YL.Core.Entity;
using YL.Utils.Extensions;
using YL.Utils.Pub;

namespace WMSCore.Outside
{
    [Route("/hook/mestest")]
    public class MESAccessorTestController : Controller
    {
        MESService.IService _apiProxy;
        public MESAccessorTestController()
        {
            var binding = new BasicHttpBinding();
            binding.SendTimeout = new TimeSpan(1, 0, 0);
            binding.ReceiveTimeout = new TimeSpan(1, 0, 0);
            var factory = new ChannelFactory<MESService.IService>(binding, new EndpointAddress(MESApiAccessor.Host));
            _apiProxy = factory.CreateChannel();
        }


        /// <summary>
        /// 入库完成测试
        /// </summary>
        /// <param name="inStockInfo"></param>
        /// <returns></returns>
        [HttpPost("WarehousingFinish")]
        public async Task<OutsideStockInResponseResult> WarehousingFinish()
        {
            OutsideStockInResponseWarehouse[] list = new OutsideStockInResponseWarehouse[] {
                new OutsideStockInResponseWarehouse()
                {
                    WarehouseId = "",
                    WarehouseName = "测试仓库",
                    WarehousePosition = "A",
                    WarehousingFinishTime = DateTime.Now.ToString("yyyyMMddHHmmss"),
                    SuppliesKinds = 2,
                    SuppliesInfoList = new List<OutsideMaterialResult>(){
                        new OutsideMaterialResult(){ SuppliesId = "B001", SuppliesName = "物料001" , SuppliesNumber = "10", RefreshStock = "12" },
                        new OutsideMaterialResult(){ SuppliesId = "B002", SuppliesName = "物料002" , SuppliesNumber = "20", RefreshStock = "22" }
                    }
                }
            };
            OutsideStockInResponse inStockInfo = new OutsideStockInResponse();
            inStockInfo.WarehousingId = "RK" + DateTime.Now.ToString("yyyyMMddHHmmss");
            inStockInfo.WarehousingEntryNumber = 3;
            inStockInfo.WarehousingEntryFinishList = JsonConvert.SerializeObject(list);
            string reuslt = await _apiProxy.WarehousingFinishAsync(inStockInfo.WarehousingId, inStockInfo.WarehousingEntryNumber, inStockInfo.WarehousingEntryFinishList);
            return JsonConvert.DeserializeObject<OutsideStockInResponseResult>(reuslt);
        }

        /// <summary>
        /// 出库完成测试
        /// </summary>
        /// <param name="outStockInfo"></param>
        /// <returns></returns>
        [HttpPost("WarehouseEntryFinish")]
        public async Task<OutsideStockOutResponseResult> WarehouseEntryFinish()
        {
            OutsideStockOutResponseWarehouse[] list = new OutsideStockOutResponseWarehouse[] {
                new OutsideStockOutResponseWarehouse()
                {
                    WarehouseId = "",
                    WarehouseName = "测试仓库", 
                    WarehouseEntryFinishTime = DateTime.Now.ToString("yyyyMMddHHmmss"),
                    SuppliesKinds = 2,
                    SuppliesInfoList = new List<OutsideMaterialResult>(){
                        new OutsideMaterialResult(){ SuppliesId = "B001", SuppliesName = "物料001" , SuppliesNumber = "10", RefreshStock = "12" },
                        new OutsideMaterialResult(){ SuppliesId = "B002", SuppliesName = "物料002" , SuppliesNumber = "20", RefreshStock = "22" }
                    }
                }
            };
            OutsideStockOutResponse outStockInfo = new OutsideStockOutResponse();
            outStockInfo.WarehouseEntryId = "CK" + DateTime.Now.ToString("yyyyMMddHHmmss");
            outStockInfo.WarehouseEntryFinishCount = 3; 
            outStockInfo.WarehouseEntryFinishList = JsonConvert.SerializeObject(list);
            string result = await _apiProxy.WarehouseEntryFinishAsync(outStockInfo.WarehouseEntryId, outStockInfo.WarehouseEntryFinishCount, outStockInfo.WarehouseEntryFinishList);
            return JsonConvert.DeserializeObject<OutsideStockOutResponseResult>(result);
        }

        /// <summary>
        /// 物流控制完成测试
        /// </summary>
        [HttpPost("LogisticsFinish")]
        public async Task<OutsideLogisticsFinishResponseResult> LogisticsFinish()
        {
            OutsideLogisticsFinishResponse arg = new OutsideLogisticsFinishResponse()
            {
                LogisticsId = "L" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                LogisticsFinishTime = DateTime.Now.ToString("yyyyMMddHHmmss"),
                WorkAreaName = "库区X"
            };
            string result = await _apiProxy.LogisticsFinishAsync(arg.LogisticsId, arg.LogisticsFinishTime, arg.WorkAreaName, arg.ErrorId, arg.ErrorInfo);
            return JsonConvert.DeserializeObject<OutsideLogisticsFinishResponseResult>(result);
        }
    } 
}
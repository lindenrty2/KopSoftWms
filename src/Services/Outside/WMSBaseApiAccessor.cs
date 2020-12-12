using IServices.Outside;
using Services.Outside;
using SqlSugar;
using System;
using System.Net;
using System.Threading.Tasks;
using WebApiClient;
using YL.Core.Dto;
using YL.Core.Entity;
using YL.Utils.Pub;

namespace WMSCore.Outside
{
    public class WMSBaseApiAccessor : IWMSBaseApiAccessor, IWMSApiProxy
    {

        public static string WMSProxy { get; set; }
         
        public bool IsOutside => true;

        public Wms_warehouse Warehouse { get; } 

        private IWMSApiProxy _apiProxy = null;
        private SelfWMSBaseApiAccessor _selfAccessor = null;

        public WMSBaseApiAccessor(Wms_warehouse warehouse,ISqlSugarClient client, SysUserDto userDto)
        {
            this.Warehouse = warehouse;
            HttpApiConfig config = new HttpApiConfig();
            config.HttpHost = new Uri(warehouse.IFAddress);
            if (!string.IsNullOrWhiteSpace(WMSProxy))
            {
                config.HttpHandler.UseProxy = true;
                config.HttpHandler.Proxy = new HttpProxy(WMSProxy);
                WebRequest.DefaultWebProxy = new WebProxy(WMSProxy) { BypassProxyOnLocal = false };
            } 

            _apiProxy = HttpApi.Create<IWMSApiProxy>(config);
            _selfAccessor = new SelfWMSBaseApiAccessor(warehouse, client, userDto);
        }

        public Task<RouteData<Wms_MaterialDto>> GetMateral(long materialId)
        {
            return _apiProxy.GetMateral(materialId);
        }

        public Task<RouteData<Wms_MaterialDto[]>> GetMateralList(int pageIndex,int pageSize, string search, string[] order, string datemin, string datemax)
        {
            return _apiProxy.GetMateralList(pageIndex, pageSize, search, order, datemin, datemax);
        }

        public Task<RouteData<Wms_reservoirarea>> GetReservoirArea(long reservoirAreaId)
        {
            return _apiProxy.GetReservoirArea(reservoirAreaId);
        }

        public Task<RouteData<Wms_reservoirarea[]>> GetReservoirAreaList(int pageIndex, int pageSize, string search, string[] order, string datemin, string datemax)
        {
            return _apiProxy.GetReservoirAreaList(pageIndex, pageSize, search, order, datemin, datemax);
        }

        public Task<RouteData<Wms_storagerack>> GetStorageRack(long storageRackId)
        {
            return _apiProxy.GetStorageRack(storageRackId);
        }

        public Task<RouteData<Wms_storagerack[]>> GetStorageRackList(long? reservoirAreaId, StorageRackStatus? status, int pageIndex, int pageSize, string search, string[] order, string datemin, string datemax)
        {
            return _apiProxy.GetStorageRackList(reservoirAreaId, status, pageIndex, pageSize, search, order, datemin, datemax);
        }

        public Task<RouteData<Wms_inventorybox>> GetInventoryBox(long inventoryBoxId)
        {
            return _apiProxy.GetInventoryBox(inventoryBoxId);
        }

        public Task<RouteData<Wms_inventorybox[]>> GetInventoryBoxList(
            long? reservoirAreaId, long? storageRackId, InventoryBoxStatus? status,
            int pageIndex, int pageSize, string search, string[] order, string datemin, string datemax)
        {
            return _apiProxy.GetInventoryBoxList(reservoirAreaId, storageRackId, status, pageIndex, pageSize, search, order, datemin, datemax);
        }

        public Task<RouteData<Wms_inventory[]>> GetInventoryBoxDetail(long inventoryBoxId)
        {
            return _apiProxy.GetInventoryBoxDetail(inventoryBoxId);
        }

        public async Task<RouteData<OutsideStockOutRequestResult[]>> StockOut( OutsideStockOutRequestDto request)
        {

            RouteData<OutsideStockOutRequestResult[]> result = await _apiProxy.StockOut(request);
    
            if (!result.IsSccuess)
            {
                return result;
            }
            if(result.Data.Length != 1)
            {
                return RouteData<OutsideStockOutRequestResult[]>.From(PubMessages.E2122_WMS_STOCKOUT_RESPONSE_INVAILD);

            }
            request.StockOutId = result.Data[0].StockOutId;
            request.StockOutNo = result.Data[0].StockOutNo;

            await _selfAccessor.StockOut(request);
            return result;
        }

        public Task<RouteData<OutsideStockOutQueryResult>> QueryStockOut(long stockOutId)
        {
            return _apiProxy.QueryStockOut(stockOutId);
        }

        public async Task<RouteData<OutsideStockInRequestResult[]>> StockIn( OutsideStockInRequestDto request)
        {
            RouteData<OutsideStockInRequestResult[]> result = await _apiProxy.StockIn(request);

            if (!result.IsSccuess)
            {
                return result;
            }
            if (result.Data.Length != 1)
            {
                return RouteData<OutsideStockInRequestResult[]>.From(PubMessages.E2122_WMS_STOCKOUT_RESPONSE_INVAILD);

            }
            request.StockInId = result.Data[0].StockInId;
            request.StockInNo = result.Data[0].StockInNo;

            await _selfAccessor.StockIn(request);
            return result;
        }

        public Task<RouteData<OutsideStockInQueryResult>> QueryStockIn(long stockInId)
        {
            return _apiProxy.QueryStockIn(stockInId);
        }

        public Task<RouteData<OutsideInventoryDto[]>> QueryInventory(long? reservoirAreaId, long? storageRackId, long? inventoryBoxId, long? materialId, int pageIndex, int pageSize, string search, string[] order, string datemin, string datemax)
        {
            return _apiProxy.QueryInventory(reservoirAreaId, storageRackId, inventoryBoxId, materialId, pageIndex, pageSize, search, order, datemin, datemax);
        }

        public Task<RouteData<OutsideInventoryRecordDto[]>> QueryInventoryRecord(long? reservoirAreaId, long? storageRackId,long? inventoryBoxId, long? materialId, int pageIndex, int pageSize, string search, string[] order, string datemin, string datemax)
        {
            return _apiProxy.QueryInventoryRecord(reservoirAreaId, storageRackId, inventoryBoxId,materialId, pageIndex, pageSize, search, order, datemin, datemax);
        }

        public Task<RouteData<OutsideStockOutQueryResult[]>> QueryStockOutList(long? stockOutType, StockOutStatus? stockOutStatus, int pageIndex, int pageSize, string search, string[] order, string datemin, string datemax)
        {
            return _apiProxy.QueryStockOutList(stockOutType, stockOutStatus, pageIndex, pageSize, search, order, datemin, datemax);
        }

        public Task<RouteData<OutsideStockInQueryResult[]>> QueryStockInList(long? stockInType, StockInStatus? stockInStatus, int pageIndex, int pageSize, string search, string[] order, string datemin, string datemax)
        {
            return _apiProxy.QueryStockInList(stockInType, stockInStatus, pageIndex, pageSize, search, order, datemin, datemax);
        }
    
        public Task<RouteData<OutsideStockInQueryResult[]>> QueryStockInListNew(long? stockInType, StockInStatus? stockInStatus, int pageIndex, int pageSize, string search, string[] order, string datemin, string datemax)
        {
            return _apiProxy.QueryStockInList(stockInType,stockInStatus, pageIndex, pageSize, search, order, datemin, datemax);
        }

        public Task<RouteData> StockCount(OutsideStockCountRequestDto request)
        {
            return _apiProxy.StockCount(request);
        }

        public Task<RouteData<OutsideStockCountDto[]>> QueryStockCountList(StockCountStatus? stockCountStatus, int pageIndex, int pageSize, string search, string[] order, string datemin, string datemax)
        {
            return _apiProxy.QueryStockCountList(stockCountStatus, pageIndex, pageSize, search, order, datemin, datemax);
        }

        public Task<RouteData<OutsideStockCountDto>> QueryStockCount(string stockCountNo)
        {
            return _apiProxy.QueryStockCount(stockCountNo);
        }

        public void Dispose()
        {
            _apiProxy = null;
        }

    }
}

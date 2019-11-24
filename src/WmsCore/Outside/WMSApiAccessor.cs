using IServices.Outside;
using System;
using System.Threading.Tasks;
using WebApiClient;
using YL.Core.Dto;
using YL.Core.Entity;
using YL.Utils.Pub;

namespace WMSCore.Outside
{
    public class WMSApiAccessor : IWMSApiAccessor, IWMSApiProxy
    {

        public Wms_warehouse Warehouse { get; } 

        private IWMSApiProxy _apiProxy = null;
        public WMSApiAccessor(Wms_warehouse warehouse)
        {
            this.Warehouse = warehouse;
            HttpApiConfig config = new HttpApiConfig();
            config.HttpHost = new Uri(warehouse.IFAddress);
            _apiProxy = HttpApi.Create<IWMSApiProxy>(config);
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

        public Task<RouteData<Wms_storagerack[]>> GetStorageRackList(long? reservoirAreaId, int pageIndex, int pageSize, string search, string[] order, string datemin, string datemax)
        {
            return _apiProxy.GetStorageRackList(reservoirAreaId, pageIndex, pageSize, search, order, datemin, datemax);
        }

        public Task<RouteData<Wms_inventorybox>> GetInventoryBox(long inventoryBoxId)
        {
            return _apiProxy.GetInventoryBox(inventoryBoxId);
        }

        public Task<RouteData<Wms_inventorybox[]>> GetInventoryBoxList(long? reservoirAreaId, long? storageRackId, int pageIndex, int pageSize, string search, string[] order, string datemin, string datemax)
        {
            return _apiProxy.GetInventoryBoxList(reservoirAreaId, storageRackId, pageIndex, pageSize, search, order, datemin, datemax);
        }

        public Task<RouteData<Wms_inventory[]>> GetInventoryBoxDetail(long inventoryBoxId)
        {
            return _apiProxy.GetInventoryBoxDetail(inventoryBoxId);
        }

        public Task<RouteData<OutsideStockOutRequestResult[]>> StockOut( OutsideStockOutRequestDto request)
        {
            return _apiProxy.StockOut(request);
        }

        public Task<RouteData<OutsideStockOutQueryResult>> QueryStockOut(long stockOutId)
        {
            return _apiProxy.QueryStockOut(stockOutId);
        }

        public Task<RouteData<OutsideStockInRequestResult[]>> StockIn( OutsideStockInRequestDto request)
        {
            return _apiProxy.StockIn(request);
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

        public void Dispose()
        {
            _apiProxy = null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using YL.Core.Dto;
using YL.Core.Entity;
using YL.Utils.Pub;

namespace IServices.Outside
{
    /// <summary>
    /// WMS基本API
    /// </summary>
    public interface IWMSBaseApiAccessor 
    {
        bool IsOutside { get; }

        Wms_warehouse Warehouse { get; } 

        //-----------------------------------------------------------------
        /// <summary>
        /// 获取物料定义信息
        /// </summary>
        /// <param name="outStockInfo"></param>
        /// <returns></returns> 
        Task<RouteData<Wms_MaterialDto>> GetMateral(long materialId);

        /// <summary>
        /// 获取物料定义列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="search"></param>
        /// <param name="order"></param>
        /// <param name="datemin"></param>
        /// <param name="datemax"></param>
        /// <returns></returns> 
        Task<RouteData<Wms_MaterialDto[]>> GetMateralList(int pageIndex, int pageSize, string search, string[] order, string datemin, string datemax);

        //-----------------------------------------------------------------

        /// <summary>
        /// 获取物料定义信息
        /// </summary>
        /// <param name="reservoirAreaId"></param>
        /// <returns></returns> 
        Task<RouteData<Wms_reservoirarea>> GetReservoirArea(long reservoirAreaId);

        /// <summary>
        /// 获取库区定义列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="search"></param>
        /// <param name="order"></param>
        /// <param name="datemin"></param>
        /// <param name="datemax"></param>
        /// <returns></returns> 
        Task<RouteData<Wms_reservoirarea[]>> GetReservoirAreaList(int pageIndex, int pageSize, string search, string[] order, string datemin, string datemax);


        //-----------------------------------------------------------------

        /// <summary>
        /// 获取库位定义
        /// </summary>
        /// <param name="storageRackId"></param>
        /// <returns></returns> 
        Task<RouteData<Wms_storagerack>> GetStorageRack(long storageRackId);

        /// <summary>
        /// 获取库位定义列表
        /// </summary>
        /// <param name="reservoirAreaId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="search"></param>
        /// <param name="order"></param>
        /// <param name="datemin"></param>
        /// <param name="datemax"></param>
        /// <returns></returns> 
        Task<RouteData<Wms_storagerack[]>> GetStorageRackList(long? reservoirAreaId, StorageRackStatus? status, int pageIndex, int pageSize, string search, string[] order, string datemin, string datemax);


        //-----------------------------------------------------------------

        /// <summary>
        /// 获取料箱定义
        /// </summary>
        /// <param name="storageRackId"></param>
        /// <returns></returns> 
        Task<RouteData<Wms_inventorybox>> GetInventoryBox(long inventoryBoxId);

        /// <summary>
        /// 获取料箱定义列表
        /// </summary>
        /// <param name="reservoirAreaId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="search"></param>
        /// <param name="order"></param>
        /// <param name="datemin"></param>
        /// <param name="datemax"></param>
        /// <returns></returns> 
        Task<RouteData<Wms_inventorybox[]>> GetInventoryBoxList(long? reservoirAreaId, long? storageRackId, InventoryBoxStatus? status, int pageIndex, int pageSize, string search, string[] order, string datemin, string datemax);

        /// <summary>
        /// 获取料箱物料信息
        /// </summary>
        /// <param name="inventoryBoxId"></param>
        /// <returns></returns> 
        Task<RouteData<Wms_inventory[]>> GetInventoryBoxDetail(long inventoryBoxId);

        //-----------------------------------------------------------------

        /// <summary>
        /// 下发出库任务
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns> 
        Task<RouteData<OutsideStockOutRequestResult[]>> StockOut(OutsideStockOutRequestDto request);


        /// <summary>
        /// 出库任务进度查询
        /// </summary>
        /// <param name="stockOutId"></param>
        /// <returns></returns> 
        Task<RouteData<OutsideStockOutQueryResult>> QueryStockOut(long stockOutId);

        /// <summary>
        /// 获取出库列表
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <param name="search"></param>
        /// <param name="v"></param>
        /// <param name="datemin"></param>
        /// <param name="datemax"></param>
        /// <returns></returns> 
        Task<RouteData<OutsideStockOutQueryResult[]>> QueryStockOutList(long? stockOutType, StockOutStatus? stockOutStatus, int pageIndex, int pageSize, string search, string[] order, string datemin, string datemax);

        //-----------------------------------------------------------------
        /// <summary>
        /// 下发入库任务
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns> 
        Task<RouteData<OutsideStockInRequestResult[]>> StockIn(OutsideStockInRequestDto request);


        /// <summary>
        /// 入库任务进度查询
        /// </summary>
        /// <param name="stockInId"></param>
        /// <returns></returns> 
        Task<RouteData<OutsideStockInQueryResult>> QueryStockIn(long stockInId);

        /// <summary>
        /// 获取入库列表
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <param name="search"></param>
        /// <param name="v"></param>
        /// <param name="datemin"></param>
        /// <param name="datemax"></param>
        /// <returns></returns> 
        Task<RouteData<OutsideStockInQueryResult[]>> QueryStockInList(long? stockInType, StockInStatus? stockInStatus, int pageIndex, int pageSize, string search, string[] order, string datemin, string datemax);


        //-----------------------------------------------------------------

        /// <summary>
        /// 库存查询
        /// </summary>
        /// <param name="reservoirAreaId"></param>
        /// <param name="storageRackId"></param>
        /// <param name="materialId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="search"></param>
        /// <param name="order"></param>
        /// <param name="datemin"></param>
        /// <param name="datemax"></param>
        /// <returns></returns> 
        Task<RouteData<OutsideInventoryDto[]>> QueryInventory(long? reservoirAreaId, long? storageRackId, long? inventoryBoxId, long? materialId, int pageIndex, int pageSize, string search, string[] order, string datemin, string datemax);


        /// <summary>
        /// 库存记录查询
        /// </summary>
        /// <param name="reservoirAreaId"></param>
        /// <param name="storageRackId"></param>
        /// <param name="materialId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="search"></param>
        /// <param name="order"></param>
        /// <param name="datemin"></param>
        /// <param name="datemax"></param>
        /// <returns></returns> 
        Task<RouteData<OutsideInventoryRecordDto[]>> QueryInventoryRecord(long? reservoirAreaId, long? storageRackId, long? inventoryBoxId, long? materialId, int pageIndex, int pageSize, string search, string[] order, string datemin, string datemax);

        /// <summary>
        /// 获取盘库列表
        /// </summary>
        /// <param name="stockCountStatus"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="search"></param>
        /// <param name="order"></param>
        /// <param name="datemin"></param>
        /// <param name="datemax"></param>
        /// <returns></returns>
        Task<RouteData<Wms_stockcount[]>> QueryStockCountList (StockCountStatus? stockCountStatus, int pageIndex, int pageSize, string search, string[] order, string datemin, string datemax);

        /// <summary>
        /// 获取盘库列表
        /// </summary>
        /// <param name="stockCountStatus"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="search"></param>
        /// <param name="order"></param>
        /// <param name="datemin"></param>
        /// <param name="datemax"></param>
        /// <returns></returns>
        Task<RouteData<Wms_stockcount>> QueryStockCount(string stockCountNo);

    }


}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiClient;
using WebApiClient.Attributes;
using YL.Core.Dto;
using YL.Core.Entity;
using YL.Utils.Pub;

namespace IServices.Outside
{

    public interface IWMSApiProxy : IHttpApi
    {

        //-----------------------------------------------------------------
        /// <summary>
        /// 获取物料定义信息
        /// </summary>
        /// <param name="outStockInfo"></param>
        /// <returns></returns>
        [HttpGet("Material/{materialId}")]
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
        [HttpGet("Material/List/{pageIndex}")]
        Task<RouteData<Wms_MaterialDto[]>> GetMateralList(int pageIndex, int pageSize, string search,string[] order,string datemin,string datemax);

        //-----------------------------------------------------------------

        /// <summary>
        /// 获取物料定义信息
        /// </summary>
        /// <param name="reservoirAreaId"></param>
        /// <returns></returns>
        [HttpGet("ReservoirArea/{reservoirAreaId}")]
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
        [HttpGet("ReservoirArea/List/{pageIndex}")]
        Task<RouteData<Wms_reservoirarea[]>> GetReservoirAreaList(int pageIndex, int pageSize, string search, string[] order, string datemin, string datemax);


        //-----------------------------------------------------------------

        /// <summary>
        /// 获取库位定义
        /// </summary>
        /// <param name="storageRackId"></param>
        /// <returns></returns>
        [HttpGet("StorageRack/{storageRackId}")]
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
        [HttpGet("StorageRack/List/{pageIndex}")]
        Task<RouteData<Wms_storagerack[]>> GetStorageRackList(long? reservoirAreaId,int pageIndex, int pageSize, string search, string[] order, string datemin, string datemax);


        //-----------------------------------------------------------------

        /// <summary>
        /// 获取料箱定义
        /// </summary>
        /// <param name="storageRackId"></param>
        /// <returns></returns>
        [HttpGet("InventoryBox/{inventoryBoxId}")]
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
        [HttpGet("InventoryBox/List/{pageIndex}")]
        Task<RouteData<Wms_inventorybox[]>> GetInventoryBoxList(long? reservoirAreaId, long? storageRackId,int pageIndex,int pageSize, string search, string[] order, string datemin, string datemax);

        /// <summary>
        /// 获取料箱物料信息
        /// </summary>
        /// <param name="inventoryBoxId"></param>
        /// <returns></returns>
        [HttpGet("InventoryBox/{inventoryBoxId}/Detail")]
        Task<RouteData<Wms_inventory[]>> GetInventoryBoxDetail(long inventoryBoxId);

        //-----------------------------------------------------------------

        /// <summary>
        /// 下发出库任务
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("StockOut")]
        Task<RouteData<OutsideStockOutRequestResult[]>> StockOut([JsonContent]OutsideStockOutRequestDto request);


        /// <summary>
        /// 出库任务进度查询
        /// </summary>
        /// <param name="stockOutId"></param>
        /// <returns></returns>
        [HttpGet("/StockOut/{stockOutId}")]
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
        [HttpGet("/StockOut/List")]
        Task<RouteData<OutsideStockOutQueryResult[]>> QueryStockOutList(long? stockOutType, StockOutStatus? stockOutStatus, int pageIndex, int pageSize, string search, string[] order, string datemin, string datemax);

        //-----------------------------------------------------------------
        /// <summary>
        /// 下发入库任务
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("StockIn")]
        Task<RouteData<OutsideStockInRequestResult[]>> StockIn([JsonContent]OutsideStockInRequestDto request);


        /// <summary>
        /// 入库任务进度查询
        /// </summary>
        /// <param name="stockInId"></param>
        /// <returns></returns>
        [HttpGet("/StockIn/{stockInId}")]
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
        [HttpGet("/StockIn/List")]
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
        [HttpGet("Inventory/List/{pageIndex}")]
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
        [HttpGet("InventoryRecord/List/{pageIndex}")]
        Task<RouteData<OutsideInventoryRecordDto[]>> QueryInventoryRecord(long? reservoirAreaId, long? storageRackId, long? inventoryBoxId, long? materialId, int pageIndex, int pageSize, string search, string[] order, string datemin, string datemax);




    }

}

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
    /// WMS操作API
    /// </summary>
    public interface IWMSOperationApiAccessor
    {

        Task<RouteData<Wms_inventoryboxTask>> GetInventoryBoxkTask(long inventoryBoxTaskId); 
        /// <summary>
        /// 自动选择料箱出库
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        Task<RouteData> DoAutoSelectBoxOut(long? reservoirAreaId,int size,PLCPosition pos);
        /// <summary>
        /// 指定料箱出库
        /// </summary>
        /// <param name="inventoryBoxId">料箱</param>
        /// <returns></returns>
        Task<RouteData> DoInventoryBoxOut(long inventoryBoxId,PLCPosition pos);
        /// <summary>
        /// 指定出库单出库
        /// </summary>
        /// <param name="stockOutIds">出库单Id列表</param>
        /// <returns></returns>
        Task<RouteData> DoStockOutBoxOut(long[] stockOutIds,PLCPosition pos);
        /// <summary>
        /// 指定出库单出库
        /// </summary>
        /// <param name="stockOutId">出库单Id</param>
        /// <returns></returns>
        Task<RouteData> DoStockOutBoxOut(long stockOutId, PLCPosition pos);

        /// <summary>
        /// 获取指定出库单当前的出库计划
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        Task<Wms_StockOutWorkDetailDto[]> GetStockOutCurrentPlan(long stockoutId);

        /// <summary>
        /// 获取料想入库任务操作详细
        /// </summary>
        /// <param name="inventoryBoxTaskId"></param>
        /// <returns></returns>
        Task<RouteData<InventoryDetailDto[]>> InventoryInDetailList(long inventoryBoxTaskId);
        /// <summary>
        /// 获取料想出库任务操作详细
        /// </summary>
        /// <param name="inventoryBoxTaskId"></param>
        /// <returns></returns>
        Task<RouteData<InventoryDetailDto[]>> InventoryOutDetailList(long inventoryBoxTaskId);

        /// <summary>
        /// 料箱归库操作
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="inventoryBoxTaskId"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        Task<RouteData> DoInventoryBoxBack(StockOperation mode, long inventoryBoxTaskId, InventoryDetailDto[] details, PLCPosition pos);
        /// <summary>
        /// 获取WCS任务列表
        /// </summary>
        /// <param name="stockInId"></param>
        /// <returns></returns>
        Task<RouteData<Wms_wcstask[]>> GetWCSTaskList(bool failOnly, int pageIndex, int pageSize, string search, string[] order, string datemin, string datemax);
        /// <summary>
        /// 手工设置WCS指令状态
        /// </summary>
        /// <param name="wcsTaskId"></param>
        /// <param name="isSccuess">成功/失败</param>
        /// <returns></returns>
        Task<RouteData> SetWCSTaskStatus(long wcsTaskId, string code );
        /// <summary>
        /// 重发Wcs任务指令
        /// </summary>
        /// <param name="wcsTaskId"></param> 
        /// <returns></returns>
        Task<RouteData> RepeatWCSTaskStatus(long wcsTaskId);
        /// <summary>
        /// 出库确认
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        Task<ConfirmOutStockResult> ConfirmOutStock(WCSStockTaskCallBack result);
        /// <summary>
        /// 归库确认
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        Task<ConfirmBackStockResult> ConfirmBackStock(WCSStockTaskCallBack result);
        /// <summary>
        /// 入库单完成
        /// </summary> 
        /// <param name="stockInId"></param>
        /// <returns></returns>
        Task<RouteData> DoStockInComplate(long stockInId);
        /// <summary>
        /// 入库扫描完成
        /// </summary>
        /// <param name="stockInId"></param>
        /// <param name="inventoryBoxId"></param>
        /// <param name="materials"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        Task<RouteData> DoStockInScanComplate(long stockInId, long inventoryBoxId, Wms_StockMaterialDetailDto[] materials, string remark);
        /// <summary>
        /// 根据出库单锁定物料
        /// </summary>
        /// <param name="stockOutId"></param>
        /// <returns></returns>
        Task<RouteData> DoStockOutLock(long stockOutId);
        /// <summary>
        /// 出库扫描完成
        /// </summary>
        /// <param name="stockOutId"></param>
        /// <param name="inventoryBoxId"></param>
        /// <param name="materials"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        Task<RouteData> DoStockOutScanComplate(long stockOutId, long inventoryBoxId, Wms_StockMaterialDetailDto[] materials, string remark);
        /// <summary>
        /// 出库单完成
        /// </summary>
        /// <param name="stockOutId"></param>
        /// <returns></returns>
        Task<RouteData> DoStockOutComplate(long stockOutId);


        /// <summary>
        /// 获取有指定的物料的料箱列表
        /// </summary>
        /// <param name="materialId"></param>
        /// <returns></returns>
        Task<RouteData<Wms_InventoryBoxMaterialInfo[]>> GetInventoryBoxList(string materialNo);


        /// <summary>
        /// 查询是否有出库单
        /// </summary>
        /// <returns></returns>
        Task<RouteData<bool>> HasStockOutNofity();

        /// <summary>
        /// 查询需要通知的出库单列表
        /// </summary>
        /// <returns></returns>
        Task<RouteData<Wms_StockOutDto[]>> QueryStockOutNofityList();

        /// <summary>
        /// 设置出库单已通知
        /// </summary>
        /// <returns></returns>
        Task<RouteData> SetStockOutNofitied();
    }
}

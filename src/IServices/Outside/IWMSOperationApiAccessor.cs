using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using YL.Core.Dto;
using YL.Core.Entity;

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
        Task<RouteData> DoAutoSelectBoxOut(int size);
        /// <summary>
        /// 指定料箱出库
        /// </summary>
        /// <param name="inventoryBoxId">料箱</param>
        /// <returns></returns>
        Task<RouteData> DoInventoryBoxOut(long inventoryBoxId);
        /// <summary>
        /// 指定出库单出库
        /// </summary>
        /// <param name="stockOutId">出库单Id</param>
        /// <returns></returns>
        Task<RouteData> DoStockOutBoxOut(long stockOutId);

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
        Task<RouteData> DoInventoryBoxBack(int mode, long inventoryBoxTaskId, InventoryDetailDto[] details);
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
        Task<RouteData> SetWCSTaskStatus(long wcsTaskId, bool isSccuess);
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
        Task<ConfirmOutStockResult> ConfirmOutStock(WCSTaskResult result);
        /// <summary>
        /// 归库确认
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        Task<ConfirmBackStockResult> ConfirmBackStock(WCSTaskResult result);
    }
}

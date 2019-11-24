using InterfaceMocker.Service.Do;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiClient;
using WebApiClient.Attributes;
using IServices.Outside;
using YL.Core.Dto;

namespace InterfaceMocker.WindowUI.WebApi
{
    public interface IWMSApiProxy : IHttpApi
    {
        /// <summary>
        /// 确认出库完成
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        [HttpPost("confirmOutStock")]
        ITask<ConfirmOutStockResult> ConfirmOutStock([JsonContent]WCSTaskResult result);

        /// <summary>
        /// 确认归库完成
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        [HttpPost("ConfirmBalance")]
        ITask<ConfirmBackStockResult> ConfirmBackStock([JsonContent]WCSTaskResult result);

        /// <summary>
        /// 物流控制
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        [HttpPost("logisticsFinish")]
        ITask<OutsideLogisticsFinishResponseResult> LogisticsFinish([JsonContent]OutsideLogisticsFinishResponse result);
    }
}

using InterfaceMocker.Service.Do;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiClient;
using WebApiClient.Attributes;
using WMSCore.Outside;

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
        ITask<ConfirmOutStockResult> ConfirmOutStock(WCSTaskResult result);

        /// <summary>
        /// 确认归库完成
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        [HttpPost("confirmBackStock")]
        ITask<ConfirmBackStockResult> ConfirmBackStock(WCSTaskResult result);

    }
}

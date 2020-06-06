using InterfaceMocker.WindowUI.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using IServices.Outside;

namespace InterfaceMocker.WindowUI
{
    public class WCSOutStockTaskItemViewModel : TaskItemViewModel
    {
        private OutStockInfo _outStockInfo;
        private CreateOutStockResult _result;
        public WCSOutStockTaskItemViewModel(OutStockInfo outStockInfo, CreateOutStockResult result)
        {
            this.Title = "出库任务" + outStockInfo.TaskId;
            _outStockInfo = outStockInfo;
            _result = result;

            this.Datas.Add(new TaskItemData("收到消息", JsonConvert.SerializeObject(outStockInfo)));
            this.Datas.Add(new TaskItemData("消息回复", JsonConvert.SerializeObject(result)));
        }

        protected override TaskItemCommand[] CreateCommands()
        {
            return new TaskItemCommand[] {
                new SimpleTaskItemCommand("出库成功",SendComplateAsync),
                new SimpleTaskItemCommand("空出失败",SendFail),
            };
        }

        public async void SendComplateAsync(object parameter)
        { 
            WCSTaskResult result = new WCSTaskResult()
            {
                TaskId = _outStockInfo.TaskId,
                Success = true
            };
            try
            {
                ConfirmOutStockResult cResult = await WMSApiAccessor.Instance.ConfirmOutStock(result);
                this.Datas.Add(new TaskItemData("发送回馈", JsonConvert.SerializeObject(result)));
                this.Datas.Add(new TaskItemData("回馈结果", JsonConvert.SerializeObject(cResult)));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString()); 
            }
        }

        public async void SendFail(object parameter)
        {
            WCSTaskResult result = new WCSTaskResult()
            {
                TaskId = _outStockInfo.TaskId,
                Success = false,
                Code = "400", 
            };
            try
            {
                ConfirmOutStockResult cResult = await WMSApiAccessor.Instance.ConfirmOutStock(result);
                this.Datas.Add(new TaskItemData("发送回馈", JsonConvert.SerializeObject(result)));
                this.Datas.Add(new TaskItemData("回馈结果", JsonConvert.SerializeObject(cResult)));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}

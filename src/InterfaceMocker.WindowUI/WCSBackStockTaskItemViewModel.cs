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
    public class WCSBackStockTaskItemViewModel : TaskItemViewModel
    {
        private BackStockInfo _backStockInfo;
        private CreateBackStockResult _result;
        public WCSBackStockTaskItemViewModel(BackStockInfo backStockInfo, CreateBackStockResult result)
        {
            this.Title = "归库任务" + backStockInfo.TaskId;
            _backStockInfo = backStockInfo;
            _result = result;

            this.Datas.Add(new TaskItemData("收到消息", JsonConvert.SerializeObject(backStockInfo)));
            this.Datas.Add(new TaskItemData("消息回复", JsonConvert.SerializeObject(result)));
        }

        protected override TaskItemCommand[] CreateCommands()
        {
            return new TaskItemCommand[] {
                new SimpleTaskItemCommand("归库成功",SendComplateAsync),
                new SimpleTaskItemCommand("满入失败",SendFail),
            };
        }

        public async void SendComplateAsync(object parameter)
        { 
            WCSTaskResult result = new WCSTaskResult()
            {
                TaskId = _backStockInfo.TaskId,
                Success = true
            };
            try
            {
                ConfirmBackStockResult cResult = await WMSApiAccessor.Instance.ConfirmBackStock(result);
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
                TaskId = _backStockInfo.TaskId,
                Success = false,
                Code = "401", 
            };
            try
            {
                ConfirmBackStockResult cResult = await WMSApiAccessor.Instance.ConfirmBackStock(result);
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

using EventBus;
using InterfaceMocker.WindowUI.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using YL.Core.Dto;

namespace InterfaceMocker.WindowUI
{
    public class WCSLogisticsTaskItemViewModel : TaskItemViewModel
    {
        private OutsideLogisticsControlArg _data;
        private OutsideLogisticsControlResult _result;


        public WCSLogisticsTaskItemViewModel(OutsideLogisticsControlArg data, OutsideLogisticsControlResult result)
        {
            _data = data;
            _result = result;
            this.Title = "物流控制任务:" + data.LogisticsId;
            this.Datas.Add(new TaskItemData("收到消息", JsonConvert.SerializeObject(data)));
            this.Datas.Add(new TaskItemData("回复消息", JsonConvert.SerializeObject(result)));
 
        }

        protected override TaskItemCommand[] CreateCommands()
        {
            return new TaskItemCommand[] {
                new SimpleTaskItemCommand("发送完成",SendComplateAsync),
            };
        }

        public async void SendComplateAsync(object parameter)
        {
            OutsideLogisticsFinishResponse response = new OutsideLogisticsFinishResponse()
            {
                LogisticsId = _data.LogisticsId,
                LogisticsFinishTime = DateTime.Now.ToString("yyyyMMddHHmms"),
                WorkAreaName = "WorkAreaName1",
            };
            try
            {
                OutsideLogisticsFinishResponseResult cResult = await WMSApiAccessor.Instance.LogisticsFinish(response);
                this.Datas.Add(new TaskItemData("发送完成", JsonConvert.SerializeObject(response)));
                this.Datas.Add(new TaskItemData("完成回馈", JsonConvert.SerializeObject(cResult)));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        [EventSubscriber]
        public void HandleEvent(KeyValuePair<OutsideLogisticsEnquiryArg, OutsideLogisticsEnquiryResult> args)
        {
            if (args.Key.LogisticsId != this._data.LogisticsId) return;
            this.UserControl.Dispatcher.Invoke(() =>
            {
                this.Datas.Add(new TaskItemData("收到查询", JsonConvert.SerializeObject(args.Key)));
                this.Datas.Add(new TaskItemData("回馈查询", JsonConvert.SerializeObject(args.Value)));
            });
        }

    }
}
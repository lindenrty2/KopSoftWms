using EventBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace InterfaceMocker.WindowUI
{
    public class MesLogisticsTaskItemViewModel : TaskItemViewModel
    {
        private WMSService.OutsideLogisticsControlArg _data;
        private WMSService.OutsideLogisticsControlResult _result;
        private WMSService.IMESHookController _mesHook = null;


        public MesLogisticsTaskItemViewModel(WMSService.OutsideLogisticsControlArg data)
        {
            _data = data;
            this.Title = "物流控制任务:" + data.LogisticsId;
            var binding = new BasicHttpBinding();
            var factory = new ChannelFactory<WMSService.IMESHookController>(binding, "http://localhost:23456/Outside/MesHook.asmx");
            _mesHook = factory.CreateChannel();
            ReSend(null);
        }

        protected override TaskItemCommand[] CreateCommands()
        {
            return new TaskItemCommand[] {
                new SimpleTaskItemCommand("重发",ReSend),
                new SimpleTaskItemCommand("查询状态",Search),

            };
        }

        public async void ReSend(object parameter)
        {
            this.Datas.Add(new TaskItemData("发送控制", JsonConvert.SerializeObject(_data)));
            _result = await _mesHook.LogisticsControlAsync(_data);
            this.Datas.Add(new TaskItemData("发送结果", JsonConvert.SerializeObject(_result)));
        }

        public async void Search(object parameter)
        {
            WMSService.OutsideLogisticsEnquiryArg arg = new WMSService.OutsideLogisticsEnquiryArg()
            {
                LogisticsId = _data.LogisticsId,
                EquipmentId = _result == null ? null : _result.EquipmentId,
                EquipmentName = _result == null ? null : _result.EquipmentName
            };
            this.Datas.Add(new TaskItemData("发送查询", JsonConvert.SerializeObject(arg)));
            var result = await _mesHook.LogisticsEnquiryAsync(arg);
            this.Datas.Add(new TaskItemData("查询结果", JsonConvert.SerializeObject(result)));
        }

        [EventSubscriber]
        public void HandleEvent(KeyValuePair<YL.Core.Dto.OutsideLogisticsFinishResponseResult,YL.Core.Dto.OutsideLogisticsFinishResponse> args)
        {
            if (args.Key.LogisticsId != this._data.LogisticsId) return;
            this.UserControl.Dispatcher.Invoke(() =>
            {
                this.Datas.Add(new TaskItemData("收到完成", JsonConvert.SerializeObject(args.Value)));
                this.Datas.Add(new TaskItemData("回馈完成", JsonConvert.SerializeObject(args.Key)));
            });
        }
    }
}
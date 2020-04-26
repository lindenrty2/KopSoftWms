using EventBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks; 
using YL.Core.Dto;

namespace InterfaceMocker.WindowUI
{
    public class MesStockoutTaskItemViewModel : TaskItemViewModel
    {
        private WMSService.OutsideStockOutDto _data;
        private WMSService.IMESHookController _mesHook = null;


        public MesStockoutTaskItemViewModel(WMSService.OutsideStockOutDto data)
        {
            _data = data; 
            this.Title = "出库任务:" + data.WarehouseEntryId; 
            this.Datas.Add(new TaskItemData("发送", JsonConvert.SerializeObject(data)));
            var binding = new BasicHttpBinding();
            binding.SendTimeout = new TimeSpan(1, 0, 0);
            binding.ReceiveTimeout = new TimeSpan(1, 0, 0); 
            var factory = new ChannelFactory<WMSService.IMESHookController>(binding, "http://localhost:23456/Outside/MesHook.asmx");
            _mesHook = factory.CreateChannel();
            ReSend(null);
        }

        protected override TaskItemCommand[] CreateCommands()
        {
            return new TaskItemCommand[] {
                new SimpleTaskItemCommand("重发",ReSend),
            };
        }

        public async void ReSend(object parameter)
        {
            var result = await _mesHook.WarehouseEntryAsync(_data);
            this.Datas.Add(new TaskItemData("发送结果", JsonConvert.SerializeObject(result)));
        }

        [EventSubscriber]
        public void HandleEvent(KeyValuePair<OutsideStockOutResponse, OutsideStockOutResponseResult> args)
        {
            //if (args.Key.WarehouseEntryId != this._data.WarehouseEntryId) return;
            //System.Windows.Application.Current.Dispatcher.BeginInvoke((Action)(() => {
            //    this.Datas.Add(new TaskItemData("收到回馈", JsonConvert.SerializeObject(args.Key)));
            //    this.Datas.Add(new TaskItemData("回馈结果", JsonConvert.SerializeObject(args.Value)));
            //}));
        }
    }
}

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
    public class MesStockinTaskItemViewModel : TaskItemViewModel
    {
        private WMSService.OutsideStockInDto _data;
        private WMSService.IMESHookController _mesHook = null;


        public MesStockinTaskItemViewModel(WMSService.OutsideStockInDto data)
        {
            _data = data; 
            this.Title = "入库任务:" + data.WarehousingId; 
            this.Datas.Add(new TaskItemData("发送", JsonConvert.SerializeObject(data)));
            var binding = new BasicHttpBinding();
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

        public void ReSend(object parameter)
        {
            _mesHook.WarehousingAsync(_data).ContinueWith(
                  (x) => {
                      System.Windows.Application.Current.Dispatcher.BeginInvoke(
                          (Action)(() => {
                              this.Datas.Add(new TaskItemData("发送结果", JsonConvert.SerializeObject(x.Result)));
                          }
                      ));
                  }); 
        }



        [EventSubscriber]
        public void HandleEvent(KeyValuePair<OutSideStockInResponse, OutSideStockInResponseResult> args)
        {
            if (args.Key.WarehouseId != this._data.WarehousingId) return;
            System.Windows.Application.Current.Dispatcher.BeginInvoke((Action)(() => { 
                this.Datas.Add(new TaskItemData("收到回馈", JsonConvert.SerializeObject(args.Key)));
                this.Datas.Add(new TaskItemData("回馈结果", JsonConvert.SerializeObject(args.Value)));
            }));
        }
    }
}

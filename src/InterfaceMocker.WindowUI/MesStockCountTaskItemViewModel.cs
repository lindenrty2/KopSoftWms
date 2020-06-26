using EventBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using YL.Core.Dto;

namespace InterfaceMocker.WindowUI
{
    public class MesStockCountTaskItemViewModel : TaskItemViewModel
    {
        private OutsideStockCountRequestDto _data;
        private WMSService.IMESHookController _mesHook = null;


        public MesStockCountTaskItemViewModel(OutsideStockCountRequestDto data)
        {
            _data = data;
            this.Title = "盘库任务:" + data.StockCountNo;
            this.Datas.Add(new TaskItemData("发送", JsonConvert.SerializeObject(data)));
            var binding = new BasicHttpBinding();
            binding.SendTimeout = new TimeSpan(1, 0, 0);
            binding.ReceiveTimeout = new TimeSpan(1, 0, 0);
            //var factory = new ChannelFactory<WMSSoap>(binding, "http://localhost:5713/WMS.asmx"); 
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
            //WarehousingRequest request = new WarehousingRequest()
            //{
            //    Body = new WarehousingRequestBody()
            //    {
            //        batchplanid = _data.BatchPlanId,
            //        productionplanid = _data.ProductionPlanId,
            //        suppliesinfolist = _data.SuppliesInfoList,
            //        supplieskinds = _data.SuppliesKinds.ToString(),
            //        warehousingid = _data.WarehousingId,
            //        warehousingtime = _data.WarehousingTime,
            //        warehousingtype = _data.WarehousingType,
            //        workareaname = _data.WorkAreaName
            //    }
            //};
            try
            {
                var result = await _mesHook.StockCountAsync(_data.WarehouseId, _data.StockCountNo, _data.PlanDate, JsonConvert.SerializeObject(_data.MaterialList));
                this.Datas.Add(new TaskItemData("发送结果", JsonConvert.SerializeObject(result)));
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //[EventSubscriber]
        //public void HandleEvent(KeyValuePair<OutsideStockInResponse, OutsideStockCountMaterial> args)
        //{
        //    if (args.Key.WarehousingId != this._data.WarehousingId) return;
        //    this.UserControl.Dispatcher.Invoke(() => {
        //        this.Datas.Add(new TaskItemData("收到回馈", JsonConvert.SerializeObject(args.Key)));
        //        this.Datas.Add(new TaskItemData("回馈结果", JsonConvert.SerializeObject(args.Value)));
        //    });
        //}
    }
}

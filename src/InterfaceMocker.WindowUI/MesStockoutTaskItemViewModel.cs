using EventBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WMSService;
using YL.Core.Dto;

namespace InterfaceMocker.WindowUI
{
    public class MesStockoutTaskItemViewModel : TaskItemViewModel
    {
        private OutsideStockOutDto _data;
        private WMSService.IMESHookController _mesHook = null;


        public MesStockoutTaskItemViewModel(OutsideStockOutDto data)
        {
            _data = data; 
            this.Title = "出库任务:" + data.WarehouseEntryId; 
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
                new SimpleTaskItemCommand("查询",Query),
            };
        }

        public async void Query(object parameter)
        {
            try
            {
                var result = await _mesHook.WarehouseEntryStatusEnquiryAsync(_data.WarehouseEntryId, _data.WarehouseEntryType);
                this.Datas.Add(new TaskItemData("查询结果", JsonConvert.SerializeObject(result)));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public async void ReSend(object parameter)
        {
            //WarehouseEntryRequest request = new WarehouseEntryRequest()
            //{
            //    Body = new WarehouseEntryRequestBody()
            //    {
            //        BatchPlanId = _data.BatchPlanId,
            //        ProductionPlanId = _data.ProductionPlanId,
            //        SuppliesInfoList = _data.SuppliesInfoList,
            //        SuppliesKinds = _data.SuppliesKinds.ToString(),
            //        WarehouseEntryid = _data.WarehouseEntryId,
            //        WarehouseEntryTime = _data.WarehouseEntryTime,
            //        WarehouseEntryType = _data.WarehouseEntryType,
            //        WorkAreaName = _data.WorkAreaName,
            //        WorkStationId = _data.WorkStationId
            //    }
            //};
            try
            {
                var result = await _mesHook.WarehouseEntryAsync(_data.WarehouseEntryId, _data.WarehouseEntryType, _data.WarehouseEntryTime,
                    _data.ProductionPlanId, _data.TotalWorkOrder, _data.BatchNumber, _data.BatchPlanId, _data.WorkAreaName, _data.WorkStationId,
                    _data.SuppliesKinds.ToString(), _data.SuppliesInfoList);
                this.Datas.Add(new TaskItemData("发送结果", JsonConvert.SerializeObject(result)));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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

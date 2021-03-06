﻿using EventBus;
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
    public class MesStockinTaskItemViewModel : TaskItemViewModel
    {
        private OutsideStockInDto _data;
        private WMSService.IMESHookController _mesHook = null;


        public MesStockinTaskItemViewModel(OutsideStockInDto data)
        {
            _data = data; 
            this.Title = "入库任务:" + data.WarehousingId; 
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
                new SimpleTaskItemCommand("查询",Query)

            };
        }

        public async void Query(object parameter)
        {
            try
            {
                var result = await _mesHook.WarehousingStatusEnquiryAsync(_data.WarehousingId, _data.WarehousingType);
                this.Datas.Add(new TaskItemData("查询结果", JsonConvert.SerializeObject(result)));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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
            try { 
                var result = await _mesHook.WarehousingAsync(_data.WarehousingId,_data.WarehousingType,_data.WarehousingTime,_data.ProductionPlanId,_data.BatchPlanId,_data.WorkAreaName,_data.SuppliesKinds.ToString(),_data.SuppliesInfoList);
                this.Datas.Add(new TaskItemData("发送结果", JsonConvert.SerializeObject(result)));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        [EventSubscriber]
        public void HandleEvent(KeyValuePair<OutsideStockInResponse, OutsideStockInResponseResult> args)
        {
            if (args.Key.WarehousingId != this._data.WarehousingId) return;
            this.UserControl.Dispatcher.Invoke(() => { 
                this.Datas.Add(new TaskItemData("收到回馈", JsonConvert.SerializeObject(args.Key)));
                this.Datas.Add(new TaskItemData("回馈结果", JsonConvert.SerializeObject(args.Value)));
            });
        }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WMSService;

namespace InterfaceMocker.WindowUI
{
    /// <summary>
    /// MesStockinTask.xaml 的交互逻辑
    /// </summary>
    public partial class MesStockinTaskControl : UserControl
    {
        private OutsideStockInDto _data;
        private IMESHookController _mesHook = null;

        public MesStockinTaskControl(OutsideStockInDto data)
        {
            _data = data;
            InitializeComponent();
            this.ctlTitle.Content = "入库任务:" + data.WarehousingId;
            this.ctlSend.Text = JsonConvert.SerializeObject(data);

            var binding = new BasicHttpBinding();
            var factory = new ChannelFactory<IMESHookController>(binding, "http://localhost:23456/Outside/MesHook.asmx");
            _mesHook = factory.CreateChannel();
            ReSend_Click(null,null);
        }

        private void SearchStatus_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void ReSend_Click(object sender, RoutedEventArgs e)
        {
            _mesHook.WarehousingAsync(_data).ContinueWith(
                (x) => {
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        try
                        {
                            this.ctlSendResponse.Text = JsonConvert.SerializeObject(x.Result);
                        }
                        catch(Exception ex)
                        {
                            this.ctlSendResponse.Text = ex.ToString();
                        }
                    }));
                }
            );
        }
    }
}

using InterfaceMocker.Service.Do;
using InterfaceMocker.WindowUI.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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
using WMSCore.Outside;

namespace InterfaceMocker.WindowUI
{
    /// <summary>
    /// WCSInventoryTaskControl.xaml 的交互逻辑
    /// </summary>
    public partial class WCSOutStockTaskControl : UserControl
    {
        private OutStockInfo _outStockInfo;
        public WCSOutStockTaskControl(OutStockInfo outStockInfo,CreateOutStockResult result)
        {
            _outStockInfo = outStockInfo;
            InitializeComponent();
            ctlReceive.Text = JsonConvert.SerializeObject(outStockInfo);
            ctlReceiveResponse.Text = JsonConvert.SerializeObject(result);

        }

        private void ReSend_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void OutComplated_Click(object sender, RoutedEventArgs e)
        {
            WCSTaskResult result = new WCSTaskResult()
            {
                TaskId = _outStockInfo.TaskId,
                Success = true
            };
            try
            {
                ConfirmOutStockResult cResult = await WMSApiAccessor.Instance.ConfirmOutStock(result);
                ctlSend.Text = JsonConvert.SerializeObject(result);
                ctlSendResponse.Text = JsonConvert.SerializeObject(cResult);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
                ctlSendResponse.Text = ex.ToString();
            }
        }

        private void OutFail_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

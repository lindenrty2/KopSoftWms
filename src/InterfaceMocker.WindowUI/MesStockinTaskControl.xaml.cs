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
using YL.Core.Dto;

namespace InterfaceMocker.WindowUI
{
    /// <summary>
    /// MesStockinTask.xaml 的交互逻辑
    /// </summary>
    public partial class MesStockinTaskControl : UserControl
    {
        private OutsideStockInDto _data;

        public MesStockinTaskControl(OutsideStockInDto data)
        {
            _data = data;
            InitializeComponent();
            this.ctlSend.Text = JsonConvert.SerializeObject(data);
        }

        private void SearchStatus_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void ReSend_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

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
    /// MesStockinCreateWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MesStockinCreateWindow : Window
    {
        private OutsideStockInDto _data;
        public OutsideStockInDto Data { get { return _data; } }

        public MesStockinCreateWindow()
        {
            InitializeComponent();
            _data = new OutsideStockInDto();
            _data.SuppliesInfoList = new OutsideMaterialDto[2];
            _data.SuppliesInfoList[0] = new OutsideMaterialDto();
            _data.SuppliesInfoList[1] = new OutsideMaterialDto();
            this.DataContext = _data;
        }

        public MesStockinCreateWindow(OutsideStockInDto data) 
        {
            InitializeComponent();
            _data = data;
            this.DataContext = _data;
        }

        private void AddMaterial_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<OutsideMaterialDto> newItem = new OutsideMaterialDto[] { new OutsideMaterialDto() };
            _data.SuppliesInfoList = _data.SuppliesInfoList.Union(newItem).ToArray();
            ctlSuppliesInfoList.ItemsSource = null;
            ctlSuppliesInfoList.ItemsSource = _data.SuppliesInfoList;
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}

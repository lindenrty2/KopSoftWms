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
using WMSService;

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
            _data.WarehousingId = "RKD" + DateTime.Now.ToString("yyyyMMddHHmmss");
            _data.WarehousingTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            _data.WarehousingType = "采购入库单";
            _data.ProductionPlanId = DateTime.Now.Ticks.ToString();
            _data.BatchPlanId = DateTime.Now.TimeOfDay.Ticks.ToString();
            _data.WorkAreaName = "工作区1";
            _data.SuppliesKinds = 1;
            _data.SuppliesInfoList = new OutsideMaterialDto[] {
                new OutsideMaterialDto()
                {
                    SuppliesOnlyId =  "SID" + DateTime.Now.Ticks.ToString(),
                    SuppliesId = "ID" + DateTime.Now.ToString("yyyyMMddHH"),
                    SuppliesName = "物料"+ DateTime.Now.ToString("yyyyMMddHH"),
                    SuppliesNumber = 10,
                    SuppliesType = "型号A",
                    Unit = "个"
                }
            };
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
            IEnumerable<OutsideMaterialDto> newItem = new OutsideMaterialDto[] { new OutsideMaterialDto()
                {
                    SuppliesOnlyId =  "SID" + DateTime.Now.Ticks.ToString(),
                    SuppliesId = "ID" + DateTime.Now.ToString("yyyyMMddHH"),
                    SuppliesName = "物料"+ DateTime.Now.ToString("yyyyMMddHH"),
                    SuppliesNumber = 10,
                    SuppliesType = "型号A",
                    Unit = "个"
                } };
            _data.SuppliesInfoList = _data.SuppliesInfoList.Union(newItem).ToArray();
            _data.SuppliesKinds = _data.SuppliesInfoList.Count();
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

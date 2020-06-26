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
using WMSService;
using YL.Core.Dto;

namespace InterfaceMocker.WindowUI
{
    /// <summary>
    /// MesStockCountCreateWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MesStockCountCreateWindow : Window
    {
        private OutsideStockCountRequestDto _data;
        public OutsideStockCountRequestDto Data { get { return _data; } }

        private string[] SuppliesItems = new string[] { "B001", "B002", "B003", "B004", "B005", "B006", "B007", "B008", "B009", "B010" };
        private string[] OnlySuppliesItems = new string[] {
            "TPY01-00001", "TPY01-00002", "TPY01-00003", "TPY01-00004", "TPY02-00001", "TPY02-00001", "GBX02-00001", "GBX02-00002", "GBX02-00003", "GBX02-00004" };

        private List<YL.Core.Dto.OutsideStockCountMaterial> SuppliesInfoList;
        public MesStockCountCreateWindow()
        {
            InitializeComponent();
            colSupplies.ItemsSource = SuppliesItems;
            _data = new OutsideStockCountRequestDto();
            _data.StockCountNo = "SK-" + DateTime.Now.ToString("yyyyMMddHHmmss");
            _data.PlanDate = DateTime.Now.ToString("yyyyMMddHHmmss"); 
            this.SuppliesInfoList = new List<YL.Core.Dto.OutsideStockCountMaterial>();
            _data.MaterialList = SuppliesInfoList.ToArray();
            AddMaterial_Click(null,null);
            this.DataContext = _data;
        }

        public MesStockCountCreateWindow(OutsideStockCountRequestDto data) 
        {
            InitializeComponent();
            _data = data;
            this.DataContext = _data;
        }

        private void AddMaterial_Click(object sender, RoutedEventArgs e)
        {
            string suppliy = SuppliesItems[SuppliesInfoList.Count % 10];
            IEnumerable<YL.Core.Dto.OutsideStockCountMaterial> newItem = new YL.Core.Dto.OutsideStockCountMaterial[] {
                new YL.Core.Dto.OutsideStockCountMaterial()
                {
                    MaterialOnlyId =  null,
                    MaterialNo = suppliy,
                    MaterialName =  "物料-" + suppliy, 
                    MaterialType = "型号A",
                    Unit = "个"
                } };
            this.SuppliesInfoList.AddRange(newItem);
            ctlSuppliesInfoList.ItemsSource = null;
            ctlSuppliesInfoList.ItemsSource = SuppliesInfoList;
        }

        private void AddSingleMaterial_Click(object sender, RoutedEventArgs e)
        {
            string onlySuppliy = OnlySuppliesItems[SuppliesInfoList.Count % 10];
            IEnumerable<YL.Core.Dto.OutsideStockCountMaterial> newItem = new YL.Core.Dto.OutsideStockCountMaterial[] {
                new YL.Core.Dto.OutsideStockCountMaterial()
                {
                    MaterialOnlyId =  onlySuppliy,
                    MaterialNo = null,
                    MaterialName = "物料-" + DateTime.Now.ToString("yyyyMMddHHmmss"), 
                    MaterialType = "型号A",
                    Unit = "个"
                } };
            this.SuppliesInfoList.AddRange(newItem);
            ctlSuppliesInfoList.ItemsSource = null;
            ctlSuppliesInfoList.ItemsSource = SuppliesInfoList;
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        { 
            _data.MaterialList = SuppliesInfoList.ToArray();
            this.DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}

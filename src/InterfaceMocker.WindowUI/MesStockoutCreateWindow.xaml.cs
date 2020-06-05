using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using WMSService;
using YL.Core.Dto;

namespace InterfaceMocker.WindowUI
{
    /// <summary>
    /// MesStockinCreateWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MesStockoutCreateWindow : Window
    {
        private OutsideStockOutDto _data;
        public OutsideStockOutDto Data { get { return _data; } }

        private string[] SuppliesItems = new string[] { "B001", "B002", "B003", "B004", "B005", "B006", "B007", "B008", "B009", "B010" };
        private string[] OnlySuppliesItems = new string[] {
            "TPY01-00001", "TPY01-00002", "TPY01-00003", "TPY01-00004", "TPY02-00001", "TPY02-00001", "GBX02-00001", "GBX02-00002", "GBX02-00003", "GBX02-00004" };

        private List<YL.Core.Dto.OutsideWarehouseEntryMaterialDto> SuppliesInfoList;
        public MesStockoutCreateWindow()
        {
            InitializeComponent();
            colSupplies.ItemsSource = SuppliesItems;
            _data = new OutsideStockOutDto();
            _data.WarehouseEntryId = "WL-" + DateTime.Now.ToString("yyyyMMddHHmmss");
            _data.WarehouseEntryTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            _data.WarehouseEntryType = "成品出库单";
            _data.ProductionPlanId = DateTime.Now.Ticks.ToString();
            _data.BatchPlanId = DateTime.Now.TimeOfDay.Ticks.ToString();
            _data.WorkAreaName = "工作区1";
            _data.SuppliesKinds = 1;
            this.SuppliesInfoList = new List<YL.Core.Dto.OutsideWarehouseEntryMaterialDto>();
            _data.SuppliesInfoList = JsonConvert.SerializeObject(this.SuppliesInfoList);
            AddMaterial_Click(null,null);
            this.DataContext = _data;
        }

        public MesStockoutCreateWindow(OutsideStockOutDto data) 
        {
            InitializeComponent();
            _data = data;
            this.DataContext = _data;
        }

        private void AddMaterial_Click(object sender, RoutedEventArgs e)
        {
            string suppliy = SuppliesItems[SuppliesInfoList.Count % 10];
            IEnumerable<YL.Core.Dto.OutsideWarehouseEntryMaterialDto> newItem = new YL.Core.Dto.OutsideWarehouseEntryMaterialDto[] { new YL.Core.Dto.OutsideWarehouseEntryMaterialDto()
                {
                    WarehouseId="",
                    SubWarehouseEntryId = "SW-"+ DateTime.Now.ToString("yyyyMMddHHmmss"),
                    SuppliesOnlyId =  null,
                    SuppliesId = suppliy,
                    SuppliesName =  "物料-" + suppliy,
                    SuppliesNumber = 10,
                    SuppliesType = "型号A",
                    Unit = "个"
                } };
            this.SuppliesInfoList.AddRange(newItem);
            ctlSuppliesInfoList.ItemsSource = null;
            ctlSuppliesInfoList.ItemsSource = SuppliesInfoList;
        }

        private void AddSingleMaterial_Click(object sender, RoutedEventArgs e)
        {
            string onlySuppliy = OnlySuppliesItems[SuppliesInfoList.Count % 10];
            IEnumerable<YL.Core.Dto.OutsideWarehouseEntryMaterialDto> newItem = new YL.Core.Dto.OutsideWarehouseEntryMaterialDto[] { new YL.Core.Dto.OutsideWarehouseEntryMaterialDto()
                {
                    WarehouseId="",
                    SubWarehouseEntryId = "SW-"+ DateTime.Now.ToString("yyyyMMddHHmmss"),
                    SuppliesOnlyId = onlySuppliy,
                    SuppliesId = null,
                    SuppliesName = "物料-" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                    SuppliesNumber = 10,
                    SuppliesType = "型号A",
                    Unit = "个"
                } };
            this.SuppliesInfoList.AddRange(newItem);
            ctlSuppliesInfoList.ItemsSource = null;
            ctlSuppliesInfoList.ItemsSource = SuppliesInfoList;
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            _data.WorkStationId = "WS-112931294100984";
            _data.SuppliesInfoList = JsonConvert.SerializeObject(SuppliesInfoList);
            _data.SuppliesKinds = SuppliesInfoList.Count();
            this.DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}

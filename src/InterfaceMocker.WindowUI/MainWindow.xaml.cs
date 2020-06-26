using EventBus;
using InterfaceMocker.Service;
using InterfaceMocker.Service.Do;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using IServices.Outside;
using YL.Core.Dto;

namespace InterfaceMocker.WindowUI
{
    public partial class MainWindow : Window
    {
        ServiceHost _mesHost = new ServiceHost();
        ServiceHost _wcsHost = new ServiceHost();


        private static SimpleEventBus _eventBus = SimpleEventBus.GetDefaultEventBus();
        public MainWindow()
        {
            InitializeComponent();
            _eventBus.Register(this);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            int port = 16001;
            _mesHost.Start(port);
            this.ctlMESAddress.Text = $"http://localhost:{port}/MES.asmx";
            this.ctlWCSAddress.Text = $"http://localhost:{port}/WCS/";
            this.ctlWMSAddress.Text = $"http://localhost:23456/outside/";

        }

        private void NewStockCount_Click(object sender, RoutedEventArgs e)
        {
            MesStockCountCreateWindow window = new MesStockCountCreateWindow();
            if (window.ShowDialog() != true)
            {
                return;
            }
            TaskItemControl taskControl = new TaskItemControl(new MesStockCountTaskItemViewModel(window.Data));
            ctlMESTasks.Children.Add(taskControl);
        }

        private void NewStockIn_Click(object sender, RoutedEventArgs e)
        {
            MesStockinCreateWindow window = new MesStockinCreateWindow();
            if (window.ShowDialog() != true)
            {
                return;
            }
            TaskItemControl taskControl = new TaskItemControl(new MesStockinTaskItemViewModel(window.Data));
            ctlMESTasks.Children.Add(taskControl); 
        }

        private void NewStockOut_Click(object sender, RoutedEventArgs e)
        {
            MesStockoutCreateWindow window = new MesStockoutCreateWindow();
            if (window.ShowDialog() != true)
            {
                return;
            }
            TaskItemControl taskControl = new TaskItemControl(new MesStockoutTaskItemViewModel(window.Data));
            ctlMESTasks.Children.Add(taskControl);

        }

        private void MesLogisticsControl_Click(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.BeginInvoke((Action)(() => {
                OutsideLogisticsControlArg data = new OutsideLogisticsControlArg()
                {
                    LogisticsId = "L" + DateTime.Now.Ticks.ToString(),
                    StartPoint = "Start1",
                    Destination = "Dest1"
                };
                TaskItemControl taskControl = new TaskItemControl(new MesLogisticsTaskItemViewModel(data));
                ctlMESTasks.Children.Add(taskControl);
            }));
        }

        private void MesTaskClear_Click(object sender, RoutedEventArgs e)
        {
            ctlMESTasks.Children.Clear();
        }

        [EventSubscriber]
        public void HandleEvent(KeyValuePair<OutStockInfo, CreateOutStockResult> args)
        {
            this.Dispatcher.Invoke((Action)(() => { 
                TaskItemControl taskControl = new TaskItemControl(new WCSOutStockTaskItemViewModel(args.Key, args.Value));
                ctlWCSTasks.Children.Add(taskControl);
            }));
            
        }

        [EventSubscriber]
        public void HandleEvent(KeyValuePair<BackStockInfo, CreateBackStockResult> args)
        {
            this.Dispatcher.BeginInvoke((Action)(() => {
                TaskItemControl taskControl = new TaskItemControl(new WCSBackStockTaskItemViewModel(args.Key, args.Value));
                ctlWCSTasks.Children.Add(taskControl);
            })); 
        }

        [EventSubscriber]
        public void HandleEvent(KeyValuePair<OutsideLogisticsControlArg , OutsideLogisticsControlResult> args)
        {
            this.Dispatcher.BeginInvoke((Action)(() => {
                TaskItemControl taskControl = new TaskItemControl(new WCSLogisticsTaskItemViewModel(args.Key, args.Value));
                ctlWCSTasks.Children.Add(taskControl);
            }));
        }

        private void WCSTaskClear_Click(object sender, RoutedEventArgs e)
        {
            ctlWCSTasks.Children.Clear();
        }

    }
}

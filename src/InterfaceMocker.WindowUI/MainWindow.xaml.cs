using EventBus;
using InterfaceMocker.Service;
using InterfaceMocker.Service.Do;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WMSCore.Outside;

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

        private void NewStockIn_Click(object sender, RoutedEventArgs e)
        {
            MesStockinCreateWindow window = new MesStockinCreateWindow();
            if (window.ShowDialog() != true)
            {
                return;
            }
            MesStockinTaskControl taskControl = new MesStockinTaskControl(window.Data);
            ctlMESTasks.Children.Add(taskControl);

        }

        private void NewStockOut_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MesTaskClear_Click(object sender, RoutedEventArgs e)
        {
            ctlMESTasks.Children.Clear();
        }

        [EventSubscriber]
        public void HandleEvent(KeyValuePair<OutStockInfo, CreateOutStockResult> args)
        {
            this.Dispatcher.Invoke(() => {
                WCSOutStockTaskControl taskControl = new WCSOutStockTaskControl(args.Key, args.Value);
                ctlWCSTasks.Children.Add(taskControl);
            });            
        }
    }
}

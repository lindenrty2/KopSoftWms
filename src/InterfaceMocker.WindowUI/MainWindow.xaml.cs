using InterfaceMocker.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace InterfaceMocker.WindowUI
{
    public partial class MainWindow : Window
    {
        ServiceHost _mesHost = new ServiceHost();
        ServiceHost _wcsHost = new ServiceHost();


        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            int port = 16001;
            _mesHost.Start(port);
            this.ctlMESAddress.Text = $"http://localhost:{port}/MES.asmx";
            this.ctlWCSAddress.Text = $"http://localhost:{port}/WCS/";
            this.ctlWMSAddress.Text = $"http://localhost:23456/outside/";

        }
    }
}

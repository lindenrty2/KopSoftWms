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
            _mesHost.Start(16001);
        }
    }
}

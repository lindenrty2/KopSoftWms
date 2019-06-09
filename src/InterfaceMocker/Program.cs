
using InterfaceMocker.WindowUI;
using System;

namespace InterfaceMocker
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            MainWindow window = new MainWindow();
            window.ShowDialog();
        }
         
    }
}

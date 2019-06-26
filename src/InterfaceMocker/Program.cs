
using InterfaceMocker.WindowUI;
using System;
using System.Threading.Tasks;

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

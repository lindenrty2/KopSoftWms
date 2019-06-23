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

namespace InterfaceMocker.WindowUI
{
    /// <summary>
    /// TaskControl.xaml 的交互逻辑
    /// </summary>
    public partial class TaskItemControl : UserControl
    {
        private TaskItemViewModel _viewModel;
        public TaskItemControl(TaskItemViewModel viewModel)
        {
            _viewModel = viewModel;
            InitializeComponent();

            this.DataContext = viewModel;
            foreach (TaskItemCommand command in _viewModel.Commands)
            {
                Button button = new Button();
                button.Height = 30;
                button.Content = command.Name;
                button.Command = command;
                ctlCommands.Children.Add(button);
            }

        }

        
    }
}

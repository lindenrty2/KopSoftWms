using EventBus;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace InterfaceMocker.WindowUI
{
    public abstract class TaskItemViewModel
    {
        public string Title { get; set; }

        public ObservableCollection<TaskItemData> Datas { get; } = new ObservableCollection<TaskItemData>();

        public TaskItemCommand[] Commands;

        public UserControl UserControl { get; set; }

        private static SimpleEventBus _eventBus = SimpleEventBus.GetDefaultEventBus();
        public TaskItemViewModel()
        {
            _eventBus.Register(this);
            Commands = CreateCommands();
        }

        protected abstract TaskItemCommand[] CreateCommands();

    }

    public class TaskItemData
    {
        public string Type { get; set; }
        public string Message { get; set; }
        public DateTime DateTime { get; set; }

        public TaskItemData(string type,string message)
        {
            this.Type = type;
            this.Message = message;
        }
    }

    public abstract class TaskItemCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public abstract string Name { get; set; }

        public abstract bool CanExecute(object parameter);

        public abstract void Execute(object parameter);
    }

    public class SimpleTaskItemCommand : TaskItemCommand
    { 
        public override string Name { get; set; }

        private Action<object> _executeAction = null;
        private Func<object, bool> _canExecuteAction = null;

        public SimpleTaskItemCommand(string name, Action<object> executeAction, Func<object,bool> canExecuteAction = null)
        {
            this.Name = name;
            this._executeAction = executeAction;
            this._canExecuteAction = canExecuteAction; 
        }


        public override bool CanExecute(object parameter)
        {
            if (_canExecuteAction == null) return true;
            return _canExecuteAction.Invoke(parameter);
        }

        public override void Execute(object parameter)
        {
            _executeAction?.Invoke(parameter);
        }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MapperDslUI
{
    public class LamdaCommand : ICommand
    {
        private Action<object> executeHandler;
        private Func<object, bool> canExecuteHandler;

#pragma warning disable CS0067
        public event EventHandler CanExecuteChanged;
#pragma warning restore CS0067

        public bool CanExecute(object parameter)
        {
            return canExecuteHandler(parameter);
        }

        public void Execute(object parameter)
        {
            executeHandler(parameter);
        }

        public LamdaCommand(Action<object> executeHandler, Func<object, bool> canExecuteHandler = null)
        {
            this.executeHandler = executeHandler;
            this.canExecuteHandler = canExecuteHandler ?? ((o) => true);
        }
    }
}

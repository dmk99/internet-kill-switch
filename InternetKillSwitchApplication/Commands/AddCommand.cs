using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace InternetKillSwitchApplication.Commands
{
    public class AddCommand : ICommand
    {
        public void Execute(object parameter)
        {
            throw new NotImplementedException();
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }
}

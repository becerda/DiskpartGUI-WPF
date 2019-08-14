using DiskpartGUI.Models;
using DiskpartGUI.Processes;
using DiskpartGUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DiskpartGUI.Commands
{
    class EjectCommand : ICommand
    {
        private readonly MainWindowViewModel mwvm;

        public EjectCommand(MainWindowViewModel viewModel)
        {
            mwvm = viewModel;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            Volume v = parameter as Volume;
            if (v == null)
                return false;
            return v.IsValid();
        }

        public void Execute(object parameter)
        {
            mwvm.EjectVolume();
        }
    }
}

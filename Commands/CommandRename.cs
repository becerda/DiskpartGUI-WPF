using DiskpartGUI.Models;
using DiskpartGUI.ViewModels;
using System;
using System.Windows.Input;

namespace DiskpartGUI.Commands
{
    class CommandRename : ICommand
    {
        private readonly MainWindowViewModel mwvm;

        public CommandRename(MainWindowViewModel viewModel)
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
            mwvm.RenameVolume();
        }
    }
}

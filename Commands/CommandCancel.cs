using DiskpartGUI.ViewModels;
using System;
using System.Windows.Input;

namespace DiskpartGUI.Commands
{
    class CommandCancel : ICommand
    {
        /// <summary>
        /// Reference to the MainWindowViewModel
        /// </summary>
        private readonly RenameWindowViewModel rwvm;

        /// <summary>
        /// Initializing a new instance of CommandCancel
        /// </summary>
        /// <param name="viewModel">The RenameWindowViewModel to bind to</param>
        public CommandCancel(RenameWindowViewModel viewModel)
        {
            rwvm = viewModel;
        }

        /// <summary>
        /// CanExecuteChange event handler
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Can Execute, should always be able to cancel
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns>True</returns>
        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Calls RenaimWindow.Cancel()
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            rwvm.Cancel();
        }
    }
}

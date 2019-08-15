using DiskpartGUI.ViewModels;
using System;
using System.Windows.Input;

namespace DiskpartGUI.Commands
{
    class CommandApply : ICommand
    {
        /// <summary>
        /// Reference to the MainWindowViewModel
        /// </summary>
        private readonly RenameWindowViewModel rwvm;

        /// <summary>
        /// Initializing a new instance of CommandApply
        /// </summary>
        /// <param name="viewModel">The RenameWindowViewModel to bind to</param>
        public CommandApply(RenameWindowViewModel viewModel)
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
        /// Can Execute determined by passed in parameter
        /// </summary>
        /// <param name="parameter">Should be calculated and passed in</param>
        /// <returns>Whether the command can be executed or not</returns>
        public bool CanExecute(object parameter)
        {
            if (parameter == null)
                return false;
            return (bool)parameter;
        }

        /// <summary>
        /// Calls RenaimWindow.Apply()
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            rwvm.Apply();
        }
    }
}

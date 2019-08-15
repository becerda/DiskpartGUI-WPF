using DiskpartGUI.ViewModels;
using System;
using System.Windows.Input;

namespace DiskpartGUI.Commands
{
    abstract class BaseCommand : ICommand
    {
        /// <summary>
        /// Reference to a BaseViewModel
        /// </summary>
        protected BaseViewModel bvm;

        /// <summary>
        /// Initializes a new BaseCommand
        /// </summary>
        /// <param name="viewModel"></param>
        public BaseCommand(BaseViewModel viewModel)
        {
            bvm = viewModel;
        }

        /// <summary>
        /// CanExecuteChanged event handler
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Determins if this command can execute
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public abstract bool CanExecute(object parameter);

        /// <summary>
        /// Executes a command when called
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public abstract void Execute(object parameter);
    }
}

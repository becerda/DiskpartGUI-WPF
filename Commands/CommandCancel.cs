using DiskpartGUI.ViewModels;
using System;
using System.Windows.Input;

namespace DiskpartGUI.Commands
{
    class CommandCancel : BaseCommand
    {
        /// <summary>
        /// Initializing a new instance of CommandCancel
        /// </summary>
        /// <param name="viewModel">The RenameWindowViewModel to bind to</param>
        public CommandCancel(BaseViewModel viewModel) : base(viewModel) { }

        /// <summary>
        /// Can Execute, should always be able to cancel
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns>True</returns>
        public override bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Calls RenaimWindow.Cancel()
        /// </summary>
        /// <param name="parameter"></param>
        public override void Execute(object parameter)
        {
            bvm.Cancel();
        }
    }
}

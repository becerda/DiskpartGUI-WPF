using DiskpartGUI.ViewModels;
using System;
using System.Windows.Input;

namespace DiskpartGUI.Commands
{
    class CommandRefresh : BaseCommand
    {
        /// <summary>
        /// Reference to the MainWindowViewModel
        /// </summary>
        private readonly MainWindowViewModel mwvm;

        /// <summary>
        /// Initializing a new instance of CommandEject
        /// </summary>
        /// <param name="viewModel">The MainWindowViewModel to bind to</param>
        public CommandRefresh(MainWindowViewModel viewModel) : base(null)
        {
            mwvm = viewModel;
        }

        /// <summary>
        /// Can always execute
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public override bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Calls MainWindowViewModel.Refresh()
        /// </summary>
        /// <param name="parameter"></param>
        public override void Execute(object parameter)
        {
            mwvm.Refresh();
        }
    }
}

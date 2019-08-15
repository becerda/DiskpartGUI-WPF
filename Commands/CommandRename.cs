using DiskpartGUI.Models;
using DiskpartGUI.ViewModels;
using System;
using System.Windows.Input;

namespace DiskpartGUI.Commands
{
    class CommandRename : BaseCommand
    {
        /// <summary>
        /// Reference to the MainWindowViewModel
        /// </summary>
        private readonly MainWindowViewModel mwvm;

        /// <summary>
        /// Initializing a new instance of CommandRename
        /// </summary>
        /// <param name="viewModel">The MainWindowViewModel to bind to</param>
        public CommandRename(MainWindowViewModel viewModel) : base(null)
        {
            mwvm = viewModel;
        }

        /// <summary>
        /// Can Execute if Volume.IsValid()
        /// </summary>
        /// <param name="parameter">The Volume to check</param>
        /// <returns></returns>
        public override bool CanExecute(object parameter)
        {
            Volume v = parameter as Volume;
            if (v == null)
                return false;
            return v.IsValid();
        }

        /// <summary>
        /// Calls MainWindowViewModel.RenameVolume()
        /// </summary>
        /// <param name="parameter"></param>
        public override void Execute(object parameter)
        {
            mwvm.RenameVolume();
        }
    }
}

using DiskpartGUI.Models;
using DiskpartGUI.ViewModels;
using System;
using System.Windows.Input;

namespace DiskpartGUI.Commands
{
    class CommandEject : BaseCommand
    {
        /// <summary>
        /// Reference to the MainWindowViewModel
        /// </summary>
        private MainWindowViewModel mwvm;

        /// <summary>
        /// Initializing a new instance of CommandEject
        /// </summary>
        /// <param name="viewModel">The MainWindowViewModel to bind to</param>
        public CommandEject(MainWindowViewModel viewModel) : base(null)
        {
            mwvm = viewModel;
        }

        /// <summary>
        /// Can Execute if the volume is removable and not null
        /// </summary>
        /// <param name="parameter">The Volume to check</param>
        /// <returns></returns>
        public override bool CanExecute(object parameter)
        {
            Volume v = parameter as Volume;
            if (v == null)
                return false;
            return v.IsRemovable();
        }

        /// <summary>
        /// Calls MainWindowViewModel.EjectVolume() or MountVolume()
        /// </summary>
        /// <param name="parameter">The Volume to Mount or Unmount</param>
        public override void Execute(object parameter)
        {
            Volume v = parameter as Volume;
            if (v.IsMounted())
                mwvm.EjectVolume();
            else
                mwvm.MountVolume();
        }
    }
}

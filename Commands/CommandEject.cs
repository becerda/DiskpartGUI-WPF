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
    class CommandEject : ICommand
    {
        /// <summary>
        /// Reference to the MainWindowViewModel
        /// </summary>
        private readonly MainWindowViewModel mwvm;

        /// <summary>
        /// Initializing a new instance of CommandEject
        /// </summary>
        /// <param name="viewModel">The MainWindowViewModel to bind to</param>
        public CommandEject(MainWindowViewModel viewModel)
        {
            mwvm = viewModel;
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
        /// Can Execute if the volume is removable and not null
        /// </summary>
        /// <param name="parameter">The Volume to check</param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
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
        public void Execute(object parameter)
        {
            Volume v = parameter as Volume;
            if (v.IsMounted())
                mwvm.EjectVolume();
            else
                mwvm.MountVolume();
        }
    }
}

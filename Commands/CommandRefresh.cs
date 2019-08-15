﻿using DiskpartGUI.ViewModels;
using System;
using System.Windows.Input;

namespace DiskpartGUI.Commands
{
    class CommandRefresh : ICommand
    {
        /// <summary>
        /// Reference to the MainWindowViewModel
        /// </summary>
        private readonly MainWindowViewModel mwvm;

        /// <summary>
        /// Initializing a new instance of CommandEject
        /// </summary>
        /// <param name="viewModel">The MainWindowViewModel to bind to</param>
        public CommandRefresh(MainWindowViewModel viewModel)
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
        /// Can always execute
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Calls MainWindowViewModel.Refresh()
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            mwvm.Refresh();
        }
    }
}

using DiskpartGUI.ViewModels;
using System;
using System.Windows.Input;

namespace DiskpartGUI.Commands
{
    class CommandApply : BaseCommand
    {
        /// <summary>
        /// Initializing a new instance of CommandApply
        /// </summary>
        /// <param name="viewModel">The RenameWindowViewModel to bind to</param>
        public CommandApply(BaseViewModel viewModel) : base(viewModel) { }

        /// <summary>
        /// Can Execute determined by passed in parameter
        /// </summary>
        /// <param name="parameter">Should be calculated and passed in</param>
        /// <returns>Whether the command can be executed or not</returns>
        public override bool CanExecute(object parameter)
        {
            if (parameter == null)
                return false;
            return (bool)parameter;
        }

        /// <summary>
        /// Calls RenaimWindow.Apply()
        /// </summary>
        /// <param name="parameter"></param>
        public override void Execute(object parameter)
        {
            bvm.Apply();
        }
    }
}

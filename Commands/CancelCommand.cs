using DiskpartGUI.ViewModels;
using System.Windows.Input;

namespace DiskpartGUI.Commands
{
    class CancelCommand : RelayCommand
    {
        public CancelCommand(ApplyCancelViewModel model)
            : base(model.Cancel, Key.Escape) { }
    }
}

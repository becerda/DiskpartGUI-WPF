using DiskpartGUI.ViewModels;
using System.Windows.Input;

namespace DiskpartGUI.Commands
{
    class ApplyCommand : RelayCommand
    {
        public ApplyCommand(ApplyCancelViewModel model)
            : base(model.Apply, model.CanApply, Key.Enter) { }
    }
}

using DiskpartGUI.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiskpartGUI.ViewModels
{
    abstract class ApplyCancelViewModel : ClosablePropertyChangedViewModel
    {
        /// <summary>
        /// The RelayCommand for Apply to be used with bindings
        /// </summary>
        public RelayCommand ApplyCommand { get; private set; }

        /// <summary>
        /// The RelayCommand for Cancel to be used with bindings
        /// </summary>
        public RelayCommand CancelCommand { get; private set; }

        /// <summary>
        /// Sets up Apply and Cancel Commands
        /// </summary>
        public ApplyCancelViewModel() : base()
        {
            ApplyCommand = new RelayCommand(Apply, CanApply);
            CancelCommand = new RelayCommand(Cancel);
        }

        /// <summary>
        /// Apply method to be called
        /// </summary>
        public abstract void Apply();

        /// <summary>
        /// The check to enable Apply method
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public abstract bool CanApply(object o);

        /// <summary>
        /// Cancel method to be called
        /// </summary>
        public abstract void Cancel();
    }
}

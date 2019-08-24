using DiskpartGUI.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DiskpartGUI.ViewModels
{
    /// <summary>
    /// The Exit Status a window can have
    /// </summary>
    public enum ExitStatus
    {
        Applied,
        Canceled,
        Closed
    }

    abstract class ApplyCancelViewModel : ClosablePropertyChangedViewModel
    {
        protected bool applymasterenabled = true;

        /// <summary>
        /// The ExitStatus of a window
        /// </summary>
        public ExitStatus ExitStatus { get; protected set; }

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
            ApplyCommand = new RelayCommand(Apply, CanApply, Key.Enter);
            CancelCommand = new RelayCommand(Cancel, Key.Escape);
            ExitStatus = ExitStatus.Closed;
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
        public virtual bool CanApply(object o)
        {
            return applymasterenabled;
        }

        /// <summary>
        /// Cancel method to be called
        /// </summary>
        public virtual void Cancel()
        {
            ExitStatus = ExitStatus.Canceled;
            RequestWindowClose();
        }
    }
}

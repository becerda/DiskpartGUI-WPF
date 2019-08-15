using DiskpartGUI.Commands;
using DiskpartGUI.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiskpartGUI.ViewModels
{
    abstract class BaseViewModel : INotifyPropertyChanged, IClosable
    {
        /// <summary>
        /// RequestClose event handler
        /// </summary>
        public event EventHandler<EventArgs> RequestClose;

        /// <summary>
        /// Requests the window to close
        /// </summary>
        protected void RequestWindowClose()
        {
            RequestClose?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Property changed event handler
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notifies the property changed event
        /// </summary>
        /// <param name="property"></param>
        protected void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        /// <summary>
        /// CommandApply to be used with bindings
        /// </summary>
        public CommandApply ApplyCommand { get; set; }

        /// <summary>
        /// CommandCancel to be used with bindings
        /// </summary>
        public CommandCancel CancelCommand { get; set; }

        /// <summary>
        /// Apply method to be called
        /// </summary>
        public abstract void Apply();

        /// <summary>
        /// Cancel method to be called
        /// </summary>
        public abstract void Cancel();

    }
}

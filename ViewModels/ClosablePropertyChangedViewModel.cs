using DiskpartGUI.Commands;
using DiskpartGUI.Interfaces;
using System;
using System.ComponentModel;
using System.Windows.Input;

namespace DiskpartGUI.ViewModels
{
    abstract class ClosablePropertyChangedViewModel : INotifyPropertyChanged, IClosable
    {
        /// <summary>
        /// RequestClose event handler
        /// </summary>
        public event EventHandler<EventArgs> RequestClose;

        /// <summary>
        /// Property changed event handler
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The RelayCommand to close a window
        /// </summary>
        public RelayCommand CloseWindowCommand { get; protected set; }

        /// <summary>
        /// Constructor assigning CloseCommand
        /// </summary>
        public ClosablePropertyChangedViewModel()
        {
            
            CloseWindowCommand = new RelayCommand(RequestWindowClose, Key.Escape);
        }

        /// <summary>
        /// Requests the window to close
        /// </summary>
        protected void RequestWindowClose()
        {
            RequestClose?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Notifies the property changed event
        /// </summary>
        /// <param name="property">The property that is being changed</param>
        protected void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        /// <summary>
        /// Method called when the window is finished rendering
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void OnWindowLoaded(object sender, EventArgs e)
        {

        }

    }
}

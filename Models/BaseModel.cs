using System.ComponentModel;

namespace DiskpartGUI.Models
{
    class BaseModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Propery changed event
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Method to call PropertyChanged Event
        /// </summary>
        /// <param name="property"></param>
        protected void NotifyPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

    }
}

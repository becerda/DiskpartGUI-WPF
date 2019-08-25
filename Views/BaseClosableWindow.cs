using DiskpartGUI.Interfaces;
using System.Windows;

namespace DiskpartGUI.Views
{
    public class BaseClosableWindow : Window
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public BaseClosableWindow() : this(null) { }

        /// <summary>
        /// Constructor to set the DataContext
        /// </summary>
        /// <param name="dataContext">The DataContext to use</param>
        public BaseClosableWindow(object dataContext) : base()
        {
            DataContext = dataContext;
            Loaded += Window_Loaded;
        }

        /// <summary>
        /// Window Loaded method to setup RequestClose method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is IClosable)
            {
                (DataContext as IClosable).RequestClose += (_, __) => this.Close();
            }
        }


    }
}

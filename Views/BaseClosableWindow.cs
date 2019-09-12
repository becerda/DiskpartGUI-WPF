using DiskpartGUI.Interfaces;
using DiskpartGUI.ViewModels;
using System.ComponentModel;
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
            if (DataContext is ClosablePropertyChangedViewModel)
                ContentRendered += (DataContext as ClosablePropertyChangedViewModel).OnWindowLoaded;

            Closing += Window_Closing;
        }

        /// <summary>
        /// Called when window is closing. Removes attached listeners
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Loaded -= Window_Loaded;
            if (DataContext is ClosablePropertyChangedViewModel)
                ContentRendered -= (DataContext as ClosablePropertyChangedViewModel).OnWindowLoaded;
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

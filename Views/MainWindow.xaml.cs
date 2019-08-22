using DiskpartGUI.Interfaces;
using DiskpartGUI.ViewModels;
using System;
using System.Windows;

namespace DiskpartGUI.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += Window_Loaded;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = new MainWindowViewModel();

            if (DataContext is IClosable)
            {
                (DataContext as IClosable).RequestClose += (_, __) => this.Close();
            }
        }
    }
}

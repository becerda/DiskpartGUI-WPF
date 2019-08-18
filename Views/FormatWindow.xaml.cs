using DiskpartGUI.Interfaces;
using System.Windows;

namespace DiskpartGUI.ViewModels
{
    /// <summary>
    /// Interaction logic for FormatWindow.xaml
    /// </summary>
    public partial class FormatWindow : Window
    {
        public FormatWindow()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                if (DataContext is IClosable)
                {
                    (DataContext as IClosable).RequestClose += (_, __) => this.Close();
                }
            };
        }
    }
}

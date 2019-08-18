using DiskpartGUI.Interfaces;
using System.Windows;

namespace DiskpartGUI.Views
{
    /// <summary>
    /// Interaction logic for RenameWindow.xaml
    /// </summary>
    public partial class RenameWindow : Window
    {
        public RenameWindow()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                if(DataContext is IClosable)
                {
                    (DataContext as IClosable).RequestClose += (_, __) => this.Close();
                }
            };
        }
    }
}

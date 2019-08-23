using DiskpartGUI.ViewModels;

namespace DiskpartGUI.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : BaseClosableWindow
    {
        public MainWindow() : base(new MainWindowViewModel())
        {
            InitializeComponent();
        }
    }
}

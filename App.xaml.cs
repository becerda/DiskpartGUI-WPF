using DiskpartGUI.Views;
using System.Windows;

namespace DiskpartGUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            MainWindow mw = new MainWindow();
            mw.ShowDialog();
        }
    }
}

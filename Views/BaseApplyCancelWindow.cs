using DiskpartGUI.ViewModels;
using System.Windows.Input;
using System.Windows;
using DiskpartGUI.Commands;

namespace DiskpartGUI.Views
{
    public class BaseApplyCancelWindow : BaseClosableWindow
    {
        public BaseApplyCancelWindow() : base() { }

        public BaseApplyCancelWindow(object dataContext) : base(dataContext) { }

        protected override void Window_Loaded(object sender, RoutedEventArgs e)
        {
            base.Window_Loaded(sender, e);

            RelayCommand c = new ApplyCommand((ApplyCancelViewModel)DataContext);
            KeyGesture keyGesture = new KeyGesture(c.KeyGesture, c.GestureModifier);
            KeyBinding kb = new KeyBinding(c, keyGesture);
            InputBindings.Add(kb);

            c = new CancelCommand((ApplyCancelViewModel)DataContext);
            keyGesture = new KeyGesture(c.KeyGesture, c.GestureModifier);
            kb = new KeyBinding(c, keyGesture);
            InputBindings.Add(kb);
        }
    }
}

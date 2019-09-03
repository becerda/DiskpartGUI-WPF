using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace DiskpartGUI.Helpers
{
    class GridViewColumnVisibilityManager
    {
        static Dictionary<GridViewColumn, double> OriginalColumnWidths = new Dictionary<GridViewColumn, double>();

        public static readonly DependencyProperty IsVisibleProperty = DependencyProperty.RegisterAttached("IsVisible", typeof(bool), typeof(GridViewColumnVisibilityManager), new UIPropertyMetadata(true, OnIsVisibleChanged));

        public static bool GetIsVisible(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsVisibleProperty);
        }

        public static void SetIsVisible(DependencyObject obj, bool value)
        {
            obj.SetValue(IsVisibleProperty, value);
        }

        private static void OnIsVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GridViewColumn gc = d as GridViewColumn;
            if (gc == null)
                return;

            if (GetIsVisible(gc) == false)
            {
                OriginalColumnWidths[gc] = gc.Width;
                gc.Width = 0;
            }
            else
            {
                if (gc.Width == 0)
                    gc.Width = OriginalColumnWidths[gc];
            }
        }
    }
}

﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace DiskpartGUI.Helpers
{
    class BooleanOrConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (object value in values)
            {
                if ((value is bool) && (bool)value == true)
                    return true;
            }
            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("BooleanOrConverter is a OneWay converter.");
        }
    }
}

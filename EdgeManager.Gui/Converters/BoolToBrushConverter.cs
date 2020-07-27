using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace EdgeManager.Gui.Converters
{
    public sealed class BoolToBrushConverter : IValueConverter
    {
        public BoolToBrushConverter()
        {
            this.TrueValue = Brushes.Green;
            this.FalseValue = Brushes.Gray;
        }

        public Brush FalseValue { get; set; }

        public Brush TrueValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(value is bool flag) ? DependencyProperty.UnsetValue : flag ? TrueValue : FalseValue;
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
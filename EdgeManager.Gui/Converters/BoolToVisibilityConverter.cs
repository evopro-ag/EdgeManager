using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace EdgeManager.Gui.Converters
{
    [ValueConversion(typeof (bool), typeof (Visibility))]
    public sealed class BoolToVisibilityConverter : IValueConverter
    {
        public BoolToVisibilityConverter()
        {
            this.TrueValue = Visibility.Visible;
            this.FalseValue = Visibility.Collapsed;
        }

        public Visibility FalseValue { get; set; }

        public Visibility TrueValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(value is bool flag) ? DependencyProperty.UnsetValue : (object) (Visibility) (flag ? (int) this.TrueValue : (int) this.FalseValue);
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            if (object.Equals(value, (object) this.TrueValue))
                return (object) true;
            return object.Equals(value, (object) this.FalseValue) ? (object) false : DependencyProperty.UnsetValue;
        }
    }
}
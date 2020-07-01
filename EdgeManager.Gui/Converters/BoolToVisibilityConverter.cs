using EdgeManager.Interfaces.Enums;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

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

    public sealed class ConnectionStateToBrushConverter : IValueConverter
    {
        public ConnectionStateToBrushConverter()
        {
            this.ConnectedValue = Brushes.Green;
            this.NotConnectedValue = Brushes.Gray;
        }

        public Brush NotConnectedValue { get; set; }

        public Brush ConnectedValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(value is ConnectionState flag) ? DependencyProperty.UnsetValue : flag==ConnectionState.Connected ? ConnectedValue : NotConnectedValue;
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
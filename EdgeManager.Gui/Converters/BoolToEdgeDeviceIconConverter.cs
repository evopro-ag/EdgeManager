using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using FontAwesome.WPF;

namespace EdgeManager.Gui.Converters
{
    public sealed class BoolToEdgeDeviceIconConverter : IValueConverter
    {
        public BoolToEdgeDeviceIconConverter()
        {
            this.TrueValue = FontAwesomeIcon.Cloud;
            this.FalseValue = FontAwesomeIcon.Microchip;
        }

        public FontAwesomeIcon FalseValue { get; set; }

        public FontAwesomeIcon TrueValue { get; set; }

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
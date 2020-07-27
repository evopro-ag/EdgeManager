using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using EdgeManager.Interfaces.Enums;

namespace EdgeManager.Gui.Converters
{
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
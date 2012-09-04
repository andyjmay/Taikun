using System;
using System.Windows;
using System.Windows.Data;

namespace Taikun.Demo.WPF.Converters {
  public class InverseBooleanToVisibilityConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
      if (value is Boolean) {
        return (bool) value ? Visibility.Collapsed : Visibility.Visible;
      }
      return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
      throw new NotImplementedException();
    }
  }
}

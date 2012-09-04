using System;
using System.Windows.Data;

namespace Taikun.Demo.WPF.Converters {
  public class InverseBooleanToVisibilityConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
      if (value is Boolean) {
        return !(bool)value;
      }
      return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
      throw new NotImplementedException();
    }
  }
}

using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace Bluewater.App.Converters;

public class BooleanNegationConverter : IValueConverter
{
  public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    return value is bool boolean ? !boolean : true;
  }

  public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    if (value is bool boolean)
    {
      return !boolean;
    }

    throw new NotSupportedException($"{nameof(BooleanNegationConverter)} supports boolean values only.");
  }
}

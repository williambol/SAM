using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Sam.Converters;

public class PossibleValuesConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not List<int> possibleValues || parameter is not string possibleValue)
        {
            return false;
        }
        
        if (!Int16.TryParse(possibleValue, out Int16 possibleValueInt))
        {
            return false;
        }
        
        return possibleValues.Contains(possibleValueInt);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
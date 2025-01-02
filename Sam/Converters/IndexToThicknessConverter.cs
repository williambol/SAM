using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Models_Point = Sam.Models.Point;
using Point = Sam.Models.Point;

namespace Sam.Converters;

public class IndexToThicknessConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not Models_Point point)
        {
            return new Thickness(1);
        }

        int left = GetLeftThickness(point.X);
        int right = GetRightThickness(point.X);
        int top = GetLeftThickness(point.Y);
        int bottom = GetRightThickness(point.Y);
        
        return new Thickness(left, top, right, bottom);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    private int GetLeftThickness(int index)
    {
        return index == 0 ? 3 : 0;
    }
    
    private int GetRightThickness(int index)
    {
        switch (index)
        {
            case 2:
            case 5:
                return 2;
            case 8:
                return 3;
            default:
                return 1;
        }
    }
}
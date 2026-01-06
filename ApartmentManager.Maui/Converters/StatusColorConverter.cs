using System.Globalization;

namespace ApartmentManager.Maui.Converters;

public class StatusColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var status = value as string;
        return status switch
        {
            "Platit" => Colors.Green,
            "Neplatit" => Colors.Red,
            _ => Colors.Gray
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not Color color) return string.Empty;

        if (Equals(color, Colors.Green))
            return "Platit";
        if (Equals(color, Colors.Red))
            return "Neplatit";

        return string.Empty;
    }
}
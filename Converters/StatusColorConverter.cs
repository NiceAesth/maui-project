using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace PROIECT.Converters
{
    public class StatusColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string status = value as string;
            if (status == "Platit")
                return Colors.Green;
            if (status == "Neplatit")
                return Colors.Red;

            return Colors.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
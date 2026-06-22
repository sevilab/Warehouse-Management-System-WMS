using System;
using System.Globalization;
using System.Windows.Data;

namespace WarehouseManagementSystem.Converters
{
    public class FloorDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int floor)
            {
                return $"{floor}. Kat";
            }
            if (value is string s && int.TryParse(s, out int f))
            {
                return $"{f}. Kat";
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string s)
            {
                var cleaned = s.Replace(". Kat", "").Trim();
                if (int.TryParse(cleaned, out int f))
                    return f;
            }
            return value;
        }
    }
}

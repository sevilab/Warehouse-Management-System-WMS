using System.Globalization;
using System.Windows.Data;
using MaterialDesignThemes.Wpf;

namespace WarehouseManagementSystem.Converters
{
    public class ThemeIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isDark && isDark)
                return PackIconKind.WeatherSunny;
            
            return PackIconKind.WeatherNight;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

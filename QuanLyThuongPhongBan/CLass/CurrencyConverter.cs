using System.Globalization;
using System.Windows.Data;

namespace QuanLyThuongPhongBan.CLass
{
    public class CurrencyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var vnCulture = new CultureInfo("vi-VN");
            string symbol = " ₫";

            if (value is decimal decimalValue)
                return decimalValue.ToString("N0", vnCulture) + symbol;
            if (value is double doubleValue)
                return doubleValue.ToString("N0", vnCulture) + symbol;
            if (value is int intValue)
                return intValue.ToString("N0", vnCulture) + symbol;

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}

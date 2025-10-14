using System.Globalization;
using System.Windows.Data;

namespace QuanLyThuongPhongBan.CLass
{
    public class CurrencyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool showSymbol = App.Settings.IsCurrencySymbolVisible; // global
            var vn = new CultureInfo("vi-VN");
            string symbol = showSymbol ? " ₫" : "";

            if (value is decimal dec)
                return dec.ToString("N0", vn) + symbol;
            if (value is double dbl)
                return dbl.ToString("N0", vn) + symbol;
            if (value is int i)
                return i.ToString("N0", vn) + symbol;

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}

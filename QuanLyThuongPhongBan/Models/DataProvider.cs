using QuanLyThuongPhongBan.Models;

namespace QuanLyThuongPhongBan.Models
{
    class DataProvider
    {
        private static DataProvider? _ins;

        internal static DataProvider Ins
        {
            get { if (_ins == null) _ins = new DataProvider(); return _ins; }
            set =>_ins = value;
        }

        public QuanLyThuongDoanhThuContext DB { get; set; }

        private DataProvider()
        {
            DB = new QuanLyThuongDoanhThuContext();
        }
    }
}

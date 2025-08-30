using QuanLyThuongPhongBan.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace QuanLyThuongPhongBan.Views
{
    /// <summary>
    /// Interaction logic for EditProjectRewardWindow.xaml
    /// </summary>
    public partial class EditProjectRewardWindow : Window
    {
        public ObservableCollection<TbThuongDuAn> ThuongDuAns { get; set; }

        public EditProjectRewardWindow()
        {
            InitializeComponent();// Tạo dữ liệu giả
            ThuongDuAns = new ObservableCollection<TbThuongDuAn>
            {
                new TbThuongDuAn
                {
                    Id = 1,
                    GiaTriHopDong = 1000000,
                    QuyetToan = 900000,
                    TiLeThuongPhongBan = 0.1m,
                    TongGiaTriThuongPhongBan = 90000,
                    NamThuong = "2025",
                    Details = new List<TbThuongDaiDoanDuAn>
                    {
                        new TbThuongDaiDoanDuAn { Id=1, IdPhongBan=101, TiLeThuong=0.05m, GiaTri=50000, GiaTriDieuChinhDot1=2000, TiLeDieuChinhDot1=0.02m, GiaTriDieuChinhDot2=1000, TiLeDieuChinhDot2=0.01m, ThuHoiCongNo=3000, NghiemThu=45000 },
                        new TbThuongDaiDoanDuAn { Id=2, IdPhongBan=102, TiLeThuong=0.03m, GiaTri=30000, GiaTriDieuChinhDot1=1500, TiLeDieuChinhDot1=0.015m, GiaTriDieuChinhDot2=500, TiLeDieuChinhDot2=0.005m, ThuHoiCongNo=2000, NghiemThu=27000 }
                    }
                },
                new TbThuongDuAn
                {
                    Id = 2,
                    GiaTriHopDong = 2000000,
                    QuyetToan = 1800000,
                    TiLeThuongPhongBan = 0.12m,
                    TongGiaTriThuongPhongBan = 216000,
                    NamThuong = "2024",
                    Details = new List<TbThuongDaiDoanDuAn>
                    {
                        new TbThuongDaiDoanDuAn { Id=3, IdPhongBan=201, TiLeThuong=0.06m, GiaTri=120000, GiaTriDieuChinhDot1=5000, TiLeDieuChinhDot1=0.025m, GiaTriDieuChinhDot2=2000, TiLeDieuChinhDot2=0.01m, ThuHoiCongNo=4000, NghiemThu=110000 }
                    }
                }
            };

            DataContext = this;
        }

        private void chkExpandAll_Checked(object sender, RoutedEventArgs e)
        {
            dgMaster.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Visible;
        }

        private void chkExpandAll_Unchecked(object sender, RoutedEventArgs e)
        {
            dgMaster.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.VisibleWhenSelected;
        }
    }
}

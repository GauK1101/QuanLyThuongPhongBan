using CommunityToolkit.Mvvm.ComponentModel;
using QuanLyThuongPhongBan.Services.Interfaces;
using System.Windows;

namespace QuanLyThuongPhongBan.ViewModels.Base
{
    /// <summary>
    /// Base ViewModel kết hợp tất cả chức năng
    /// </summary>
    public abstract partial class DataListViewModelBase : ObservableObject
    {
        // ✅ Kế thừa từ các base classes
        // Loading State
        [ObservableProperty] private Visibility _isLoading = Visibility.Visible;
        [ObservableProperty] private bool _isInitialLoadComplete = false;

        // Search & Filter
        [ObservableProperty] private DateTime? _fromDate;
        [ObservableProperty] private DateTime? _toDate;
        [ObservableProperty] private string _searchKeyword = string.Empty;

        // Pagination
        [ObservableProperty] private int _pageIndex = 1;
        [ObservableProperty] private int _pageSize = 25;
        [ObservableProperty] private int _totalRowCount;
        [ObservableProperty] private int _filteredRowCount;
        [ObservableProperty] private int _maxPageCount;

        // Display Info
        [ObservableProperty] private string _rowCountText = "Đang tải...";
        [ObservableProperty] private string _rowCountColor = "#2C3E50";

        // 🔄 METHODS
        public virtual void ResetFilters()
        {
            FromDate = null;
            ToDate = null;
            SearchKeyword = string.Empty;
        }

        public virtual void ResetPagination()
        {
            PageIndex = 1;
        }

        public virtual void UpdateDisplayInfo(int totalCount, int filteredCount)
        {
            if (totalCount == 0)
            {
                RowCountText = "Không có dữ liệu";
                RowCountColor = "#E74C3C"; // Màu đỏ
            }
            else if (totalCount == filteredCount)
            {
                RowCountText = $"Tổng số: {totalCount} dòng";
                RowCountColor = "#27AE60"; // Màu xanh
            }
            else
            {
                RowCountText = $"Đang hiển thị: {filteredCount}/{totalCount} dòng";
                RowCountColor = "#F39C12"; // Màu cam
            }
        }

        protected virtual void ResetViewState()
        {
            ResetFilters();
            ResetPagination();
            IsInitialLoadComplete = false;
        }
    }
}
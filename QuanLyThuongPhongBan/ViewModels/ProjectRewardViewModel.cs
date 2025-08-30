using Microsoft.EntityFrameworkCore;
using QuanLyThuongPhongBan.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace QuanLyThuongPhongBan.ViewModels
{
    internal class ProjectRewardViewModel : BaseViewModel
    {
        private ObservableCollection<TbThuongDuAn>? _list;
        public ObservableCollection<TbThuongDuAn>? List
        {
            get => _list;
            set
            {
                _list = value;
                OnPropertyChanged();
            }
        }


        private CancellationTokenSource? cancellationTokenSource;
        private void CancelPreviousTask()
        {
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
            }

            cancellationTokenSource = new CancellationTokenSource();
        }

        private async Task RefreshTable()
        {
            CancelPreviousTask();

            try
            {
                if (cancellationTokenSource != null)
                    await Task.Delay(300, cancellationTokenSource.Token);

                // Truy vấn cơ sở dữ liệu với phân trang và tìm kiếm trực tiếp
                var query = await DataProvider.Ins.DB.TbThuongDuAns
                    .AsNoTracking()
                    .AsQueryable()
                    .ToListAsync();

                //// Áp dụng tìm kiếm trực tiếp trên cơ sở dữ liệu
                //query = SearchViewModel.Search(query, SearchFill ?? string.Empty); // Cập nhật SearchViewModel để trả về IQueryable

                //// Thực hiện phân trang
                //var data = await query
                //    .Skip((currentPage - 1) * pageSize)
                //    .Take(pageSize)
                //    .ToListAsync();

                if (List == null)
                    List = new ObservableCollection<TbThuongDuAn>();
                List.Clear();

                foreach (var item in query)
                {
                    List.Add(item);
                }
            }
            catch (Exception) { }
        }
    }
}

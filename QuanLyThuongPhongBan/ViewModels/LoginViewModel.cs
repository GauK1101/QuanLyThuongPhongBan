using QuanLyThuongPhongBan.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace QuanLyThuongPhongBan.ViewModels
{
    internal class LoginViewModel : BaseViewModel
    {
        public bool IsLogin { get; set; }
        public ICommand LoginCommand { get; set; }
        public LoginViewModel()
        {
            IsLogin = false;

            LoginCommand = new RelayCommand<Window>(p => true, Login);
        }
        private void Login(Window p)
        {
            if (p == null) return;

            //if ()
            //{

            //}
            p.Close();
        }
    }
}

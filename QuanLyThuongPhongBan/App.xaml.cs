using HandyControl.Tools;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QuanLyThuongPhongBan.Data;
using QuanLyThuongPhongBan.Helpers;
using QuanLyThuongPhongBan.Services.Implementations;
using QuanLyThuongPhongBan.Services.Interfaces;
using QuanLyThuongPhongBan.ViewModels;
using QuanLyThuongPhongBan.Views.Login;
using QuanLyThuongPhongBan.Views.ProjectRewards;
using QuanLyThuongPhongBan.Views.SmbRewards;
using System.Windows;

namespace QuanLyThuongPhongBan
{
    public partial class App : Application
    {
        public static IHost? AppHost { get; private set; }

        public App()
        {
            AppHost = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    var configuration = context.Configuration;

                    string connectionString = "xCwuCZm2V0TdbL2UaewNn6eFOv15HrXfkkPm5t1xEjvrjfryaT60c8HVFKwtHVtHzfPyGpIT0LItKE4rbm9J1Jng69x/AQ1SJ+pDZXrjINUG782V+4AF7ZvSAEhm0uBgcE8TaqErjHYy9537KQ9koz6CW8yLwjy35pqkrHjEnrq9N+KVKVcdiN5h93iBvCevJEqL9wyuDH4+hMz1S6o4HuFmef2PO8NQ9N74JV+vQC0=";

                    services.AddDbContext<DataContext>(options =>
                    {
                        options.UseSqlServer(AesEncryptionHelper.Decrypt(connectionString));
                        Console.WriteLine("Kết nối với SqlServer!");
                    }, ServiceLifetime.Transient);


                    // Register services                
                    services.AddTransient<IProjectBonusDetailService, ProjectBonusDetailService>();
                    services.AddTransient<IProjectSummaryReportService, ProjectSummaryReportService>();
                    services.AddTransient<ISmbRewardService, SmbRewardService>();
                    services.AddSingleton<ICurrentUserService, CurrentUserService>();
                    services.AddSingleton<IAuthenticationService, AuthenticationService>();
                    services.AddSingleton<ILoginRememberService, LoginRememberService>();

                    // ViewModels
                    services.AddTransient<ProjectSummaryReportViewModel>();
                    services.AddTransient<ProjectBonusDetailViewModel>();
                    services.AddTransient<SmbRewardViewModel>();
                    services.AddTransient<LoginViewModel>();
                    services.AddTransient<MainViewModel>();

                    // Views
                    services.AddTransient<ProjectSummaryReportView>();
                    services.AddTransient<ProjectBonusDetailView>();
                    services.AddTransient<SmbRewardView>();
                    services.AddTransient<LoginView>();
                    services.AddTransient<MainWindow>();
                })
                .Build();
        }

        public static T GetService<T>() where T : class => AppHost.Services.GetService<T>();

        protected override async void OnStartup(StartupEventArgs e)
        {
            if (AppHost == null) return;

            await AppHost.StartAsync();

            var window = AppHost.Services.GetRequiredService<LoginView>();
            window.Show();

            ConfigHelper.Instance.SetLang("en");

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            if (AppHost == null) return;

            await AppHost.StopAsync();
            base.OnExit(e);
        }
    }
}

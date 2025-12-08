using CommunityToolkit.Mvvm.ComponentModel;
using HandyControl.Controls;
using Newtonsoft.Json.Linq;
using QuanLyThuongPhongBan.ViewOfGauK.View;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text;
using System.Windows;
using MessageBox = HandyControl.Controls.MessageBox;

namespace QuanLyThuongPhongBan.ViewOfGauK.ViewModels
{
    public partial class UpdaterViewModel : ObservableObject
    {
        Version current = new Version("1.0.4.3");

        private readonly HttpClient _httpClient;
        private readonly string _tempZipFile = Path.Combine(Path.GetTempPath(), "update.zip");
        private readonly string _extractPath = Path.Combine(Path.GetTempPath(), "update");
        private readonly string _backupPath = Path.Combine(Path.GetTempPath(), "backup");

        // Properties cho binding
        [ObservableProperty]
        private string _statusMessage = "Đang kiểm tra cập nhật...";

        [ObservableProperty]
        private int _progressPercentage;

        [ObservableProperty]
        private bool _isIndeterminate = true;


        public UpdaterViewModel()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
        }

        public async Task CheckForUpdates()
        {
            await CleanupTempFiles();

            try
            {
                string onlineVersion = await GetOnlineVersionAsync();
                if (!string.IsNullOrEmpty(onlineVersion))
                {
                    Version online = new Version(onlineVersion);

                    if (online > current)
                    {
                        var result = MessageBox.Show(
                            $"Đang có phiên bản cập nhật mới {onlineVersion}, bạn có muốn cập nhật không?",
                            "Thông tin",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Information);

                        if (result == MessageBoxResult.Yes)
                        {
                            await StartUpdateProcess();
                        }
                    }
                    else
                    {
                        Growl.SuccessGlobal("Bạn đang dùng phiên bản mới nhất!");
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"Lỗi khi kiểm tra cập nhật: {ex.Message}");

                StatusMessage = "Cập nhật thất bại.";
                IsIndeterminate = false;
                ProgressPercentage = 0;
            }
        }

        private async Task<string> GetOnlineVersionAsync()
        {
            try
            {
                string url = "https://api.github.com/repos/GauK1101/QuanLyThuongPhongBan/releases";
                var response = await _httpClient.GetStringAsync(url);
                var releases = JArray.Parse(response);

                foreach (var release in releases)
                {
                    string? version = release["name"]?.ToString();
                    if (!string.IsNullOrEmpty(version))
                        return version;
                }
            }
            catch (Exception ex)
            {
                StatusMessage = "Cập nhật thất bại.";
                IsIndeterminate = false;
                ProgressPercentage = 0;
            }
            return string.Empty;
        }

        private async Task StartUpdateProcess()
        {
            UpdateView updateView = new UpdateView();
            updateView.DataContext = this;

            Dialog.Show(updateView);

            try
            {
                await DownloadUpdateAsync();
                await ExtractAndApplyUpdateAsync();

                PromptRestart();
            }
            catch (Exception ex)
            {
                Log($"Cập nhật thất bại: {ex.Message}");
                StatusMessage = "Cập nhật thất bại.";
                IsIndeterminate = false;
                ProgressPercentage = 0;
            }
        }

        private async Task DownloadUpdateAsync()
        {
            StatusMessage = "Đang tải xuống...";
            IsIndeterminate = true;

            string downloadUrl = "https://github.com/GauK1101/QuanLyThuongPhongBan/releases/download/project/QuanLyThuongPhongBan.zip";

            using (var response = await _httpClient.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead))
            {
                response.EnsureSuccessStatusCode();

                long totalBytes = response.Content.Headers.ContentLength ?? -1L;
                using (var fileStream = new FileStream(_tempZipFile, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
                using (var contentStream = await response.Content.ReadAsStreamAsync())
                {
                    var buffer = new byte[8192];
                    long totalBytesRead = 0L;
                    int bytesRead;

                    while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        await fileStream.WriteAsync(buffer, 0, bytesRead);
                        totalBytesRead += bytesRead;

                        if (totalBytes > 0)
                        {
                            ProgressPercentage = (int)(totalBytesRead * 100 / totalBytes);
                            StatusMessage = $"Đang tải xuống: {ProgressPercentage}%";
                            IsIndeterminate = false;
                        }
                    }
                }
            }
        }

        private async Task ExtractAndApplyUpdateAsync()
        {
            StatusMessage = "Đang giải nén...";
            IsIndeterminate = true;

            // Giải nén
            if (Directory.Exists(_extractPath))
                Directory.Delete(_extractPath, true);

            Directory.CreateDirectory(_extractPath);
            ZipFile.ExtractToDirectory(_tempZipFile, _extractPath);

            // Lấy thư mục con
            string? innerFolderPath = Directory.GetDirectories(_extractPath).FirstOrDefault();
            if (innerFolderPath == null)
                throw new Exception("Không tìm thấy thư mục trong file nén");

            // Sao lưu
            StatusMessage = "Đang sao lưu...";
            await CreateBackupAsync();

            // Tạo script update
            StatusMessage = "Đang hoàn tất...";
            await CreateUpdateScriptAsync(innerFolderPath);
        }

        private async Task CreateBackupAsync()
        {
            string? appDirectory = Path.GetDirectoryName(Environment.ProcessPath);
            if (string.IsNullOrEmpty(appDirectory)) return;

            if (Directory.Exists(_backupPath))
                Directory.Delete(_backupPath, true);

            foreach (var file in Directory.GetFiles(appDirectory, "*", SearchOption.AllDirectories))
            {
                string backupFile = Path.Combine(_backupPath, Path.GetRelativePath(appDirectory, file));
                string? backupDir = Path.GetDirectoryName(backupFile);

                if (!string.IsNullOrEmpty(backupDir) && !Directory.Exists(backupDir))
                    Directory.CreateDirectory(backupDir);

                File.Copy(file, backupFile, true);
            }
        }

        private async Task CreateUpdateScriptAsync(string innerFolderPath) // innerFolderPath = thư mục temp đã giải nén
        {
            string? appDirectory = Path.GetDirectoryName(Environment.ProcessPath);
            if (string.IsNullOrEmpty(appDirectory)) return;

            string exeName = Path.GetFileName(Environment.ProcessPath)!;
            string batchPath = Path.Combine(Path.GetTempPath(), "QuanLyThuong_Update.bat");

            await using var writer = new StreamWriter(batchPath, false, Encoding.UTF8);

            await writer.WriteLineAsync("@echo off");
            await writer.WriteLineAsync("chcp 65001 >nul");
            await writer.WriteLineAsync("echo.");
            await writer.WriteLineAsync("echo ===========================================");
            await writer.WriteLineAsync("echo     ĐANG CẬP NHẬT PHIÊN BẢN MỚI");
            await writer.WriteLineAsync("echo ===========================================");
            await writer.WriteLineAsync("timeout /t 3 >nul");

            // BƯỚC 1: COPY TOÀN BỘT FILE MỚI VÀO THƯ MỤC CÀI ĐẶT (GHI ĐÈ)
            await writer.WriteLineAsync($":: BƯỚC 1: Copy file mới vào {appDirectory}");
            await writer.WriteLineAsync($"robocopy \"{innerFolderPath}\" \"{appDirectory}\" /MIR /R:5 /W:5 /NFL /NDL /NJH /NJS");
            // /MIR = Mirror: copy + xóa file cũ không còn trong nguồn → dọn sạch hoàn hảo
            await writer.WriteLineAsync("if %errorlevel% 8 goto :error");

            // BƯỚC 2: KHỞI ĐỘNG LẠI ỨNG DỤNG
            await writer.WriteLineAsync($":: BƯỚC 2: Khởi động lại ứng dụng");
            await writer.WriteLineAsync($"start \"\" \"{Path.Combine(appDirectory, exeName)}\"");

            // BƯỚC 3: TỰ XÓA FILE .BAT SAU KHI HOÀN TẤT
            await writer.WriteLineAsync($":: Dọn dẹp file cập nhật");
            await writer.WriteLineAsync("(goto) 2>nul & del /F /Q \"%~f0\"");

            await writer.WriteLineAsync("exit");

            await writer.WriteLineAsync(":error");
            await writer.WriteLineAsync("echo.");
            await writer.WriteLineAsync("echo CẬP NHẬT THẤT BẠI! Vui lòng thử lại.");

            // Chạy .bat
            var psi = new ProcessStartInfo(batchPath)
            {
                UseShellExecute = true,
                CreateNoWindow = false,
                WorkingDirectory = appDirectory
            };

            Process.Start(psi);
        }

        private void PromptRestart()
        {
            var result = MessageBox.Show(
                "Khởi động lại ứng dụng!",
                "Thông báo",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            string batchScript = Path.Combine(Path.GetTempPath(), "update.bat");
            Process.Start(new ProcessStartInfo
            {
                FileName = batchScript,
                CreateNoWindow = true,
                UseShellExecute = false
            });

            Application.Current.Shutdown();
        }

        private Task CleanupTempFiles()
        {
            try
            {
                if (File.Exists(_tempZipFile))
                    File.Delete(_tempZipFile);
                if (Directory.Exists(_extractPath))
                    Directory.Delete(_extractPath, true);
                if (Directory.Exists(_backupPath))
                    Directory.Delete(_backupPath, true);
            }
            catch { }

            return Task.CompletedTask;
        }

        private static readonly string LogFile = Path.Combine(
            Directory.GetCurrentDirectory(),
            "updater.log");

        private static void Log(string message)
        {
            try
            {
                string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} | {message}{Environment.NewLine}";
                Directory.CreateDirectory(Path.GetDirectoryName(LogFile)!);
                File.AppendAllText(LogFile, logEntry);
            }
            catch { /* Nếu không ghi được log thì thôi, đừng làm app crash */ }
        }
    }
}
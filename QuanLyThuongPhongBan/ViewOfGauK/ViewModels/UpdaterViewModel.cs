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
        Version current = new Version("1.0.4");

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
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi kiểm tra cập nhật: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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
                Debug.WriteLine($"Get version error: {ex.Message}");
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
                MessageBox.Show($"Cập nhật thất bại: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private async Task CreateUpdateScriptAsync(string innerFolderPath)
        {
            string? appDirectory = Path.GetDirectoryName(Environment.ProcessPath);
            if (string.IsNullOrEmpty(appDirectory)) return;

            string exeName = Path.GetFileName(Environment.ProcessPath); // tên file .exe đang chạy
            string batchScript = Path.Combine(Path.GetTempPath(), "update.bat");

            using var writer = new StreamWriter(batchScript, false, Encoding.UTF8);
            {
                writer.WriteLine("@echo off");
                writer.WriteLine("chcp 65001 >nul");
                writer.WriteLine("echo Đang cập nhật ứng dụng, vui lòng đợi...");

                // Đợi 1-2 giây để app cũ thoát hẳn
                writer.WriteLine("timeout /t 2 /nobreak >nul");

                // Bước 1: XÓA SẠCH toàn bộ file + folder trong thư mục hiện tại (trừ file .exe và file update.bat)
                writer.WriteLine($":: XÓA SẠCH TOÀN BỘ (trừ {exeName} và file tạm)");
                writer.WriteLine($"pushd \"{appDirectory}\"");

                // Xóa tất cả file (trừ chính nó và update.bat đang chạy)
                writer.WriteLine($"del /q /f \".\\*\" 2>nul");
                writer.WriteLine($"for /d %%x in (\".\") do (");
                writer.WriteLine($"    if /i not \"%%~nx\"==\"{Path.GetFileNameWithoutExtension(exeName)}\" (");
                writer.WriteLine($"        rd /s /q \"%%x\" 2>nul");
                writer.WriteLine($"    )");
                writer.WriteLine($")");

                // Bước 2: Copy toàn bộ file mới từ thư mục tạm vào
                writer.WriteLine($":: COPY FILE MỚI VÀO");
                writer.WriteLine($"robocopy \"{innerFolderPath}\" \"{appDirectory}\" /E /IS /IT /NFL /NDL /NJH /NJS >nul");

                // Bước 3: Khởi động lại ứng dụng
                writer.WriteLine($":: KHỞI ĐỘNG LẠI ỨNG DỤNG");
                writer.WriteLine($"start \"\" \"{Path.Combine(appDirectory, exeName)}\"");

                // Bước 4: Xóa file update.bat và thư mục tạm (tự dọn rác)
                writer.WriteLine($":: DỌN RÁC");
                writer.WriteLine($"(goto) 2>nul & del /q \"%~f0\""); // tự xóa chính file .bat này

                writer.WriteLine("exit");
            }

            // Chạy file .bat với quyền hiện tại
            var startInfo = new ProcessStartInfo
            {
                FileName = batchScript,
                UseShellExecute = true,
                CreateNoWindow = true
            };
            Process.Start(startInfo);
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
            catch (Exception ex)
            {
                Debug.WriteLine($"Cleanup error: {ex.Message}");
            }

            return Task.CompletedTask;
        }
    }
}
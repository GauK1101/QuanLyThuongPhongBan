using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Windows;
using Newtonsoft.Json.Linq;
using QuanLyThuongPhongBan.ViewForGauK.View;

namespace QuanLyThuongPhongBan.ViewForGauK.ViewModels
{
    public class Updater
    {
        private readonly string newVersionUrl;
        private readonly string currentVersion;
        private readonly HttpClient httpClient;

        private bool isShowVersion = true;

        private string tempZipFile = Path.Combine(Path.GetTempPath(), "update.zip");
        private string extractPath = Path.Combine(Path.GetTempPath(), "update");
        private string batchScript = Path.Combine(Path.GetTempPath(), "update.bat");
        private string backupPath = Path.Combine(Path.GetTempPath(), "backup");


        public Updater()
        {
            currentVersion = "1.0.0";
            newVersionUrl = "https://github.com/GauK1101/QuanLyThuongPhongBan/releases/download/QuanLyThuongPhongBan/QuanLyThuongPhongBan.zip";
            httpClient = new HttpClient();

            if (Properties.SettingsUpdate.Default.IsShowVersion)
            {
                MessageBox.Show($"Phiên bản hiện tại: {currentVersion}");

                Properties.SettingsUpdate.Default.IsShowVersion = false;
                Properties.SettingsUpdate.Default.Save();
            }
        }

        private async Task<string> LoadReleasesAsync()
        {
            try
            {
                string url = "https://api.github.com/repos/GauK1101/QuanLyThuongPhongBan/releases";
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3");

                var response = await httpClient.GetStringAsync(url);
                JArray releases = JArray.Parse(response);

                // Kiểm tra từng release
                foreach (var release in releases)
                {
                    string? version = release["name"]?.ToString();

                    if (!string.IsNullOrEmpty(version))
                    {
                        return version;
                    }
                }
            }
            catch (Exception)
            {
                return "";
            }

            return "";
        }

        public async Task CheckForUpdates()
        {
            await CreateCleanupScript();

            try
            {
                Version onlineVersion = new Version(await LoadReleasesAsync());
                Version current = new Version(currentVersion);

                if (onlineVersion != current && Properties.SettingsUpdate.Default.IsCheckMessageShow)
                {
                    Properties.SettingsUpdate.Default.IsCheckMessageShow = false;
                    Properties.SettingsUpdate.Default.Save();

                    MessageBoxResult msg = MessageBox.Show("Đang có 1 phiên bản cập nhật mới, bạn có muốn cập nhất không?", "Thông tin", MessageBoxButton.YesNo, MessageBoxImage.Information);
                    if (msg == MessageBoxResult.Yes)
                    {
                        UpdateWindow updateProgressWindow = new UpdateWindow();
                        updateProgressWindow.Owner = Application.Current.MainWindow;

                        Application.Current.MainWindow.Hide();
                        updateProgressWindow.Show();

                        await DownloadAndUpdate(updateProgressWindow);

                        Properties.SettingsUpdate.Default.IsShowVersion = true;
                        Properties.SettingsUpdate.Default.Save();

                        updateProgressWindow.Close();
                    }

                    Properties.SettingsUpdate.Default.IsCheckMessageShow = true;
                    Properties.SettingsUpdate.Default.Save();
                }
            }
            catch (Exception)
            {
                var window = Application.Current.Windows
                    .OfType<Window>()
                    .FirstOrDefault(w => w.GetType().Name == "UpdateWindow");

                if (window != null)
                {
                    window.Close();
                }
            }
        }

        private async Task DownloadAndUpdate(UpdateWindow progressWindow)
        {
            try
            {
                // Tải xuống tệp nén từ GitHub
                using (var response = await httpClient.GetAsync(newVersionUrl, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();

                    long totalBytes = response.Content.Headers.ContentLength ?? -1L;
                    using (var fileStream = new FileStream(tempZipFile, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
                    {
                        using (var contentStream = await response.Content.ReadAsStreamAsync())
                        {
                            var buffer = new byte[8192];
                            var totalBytesRead = 0L;
                            var bytesRead = 0;
                            while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                            {
                                await fileStream.WriteAsync(buffer, 0, bytesRead);
                                totalBytesRead += bytesRead;

                                if (totalBytes > 0)
                                {
                                    var progress = (int)(totalBytesRead * 100 / totalBytes);
                                    Application.Current.Dispatcher.Invoke(() =>
                                    {
                                        progressWindow.UpdateStatus($"Đang tải xuống: {progress}%");
                                        progressWindow.progressBar.IsIndeterminate = false;
                                        progressWindow.progressBar.Value = progress;
                                    });
                                }
                            }
                        }
                    }
                }

                // Cập nhật thông báo trạng thái
                Application.Current.Dispatcher.Invoke(() =>
                {
                    progressWindow.UpdateStatus("Đang giải nén bản cập nhật...");
                });

                // Giải nén tệp
                if (Directory.Exists(extractPath))
                {
                    Directory.Delete(extractPath, true);
                }
                Directory.CreateDirectory(extractPath);
                ZipFile.ExtractToDirectory(tempZipFile, extractPath);

                // Xác định thư mục con trong tệp nén (thư mục con đầu tiên)
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                string innerFolderPath = Directory.GetDirectories(extractPath).FirstOrDefault();
                if (innerFolderPath == null)
                {
                    throw new Exception("Thư mục con trong tệp nén không được tìm thấy.");
                }

                string appDirectory = Path.GetDirectoryName(Environment.ProcessPath);
                if (Directory.Exists(appDirectory))
                {
                    foreach (var file in Directory.GetFiles(appDirectory, "*", SearchOption.AllDirectories))
                    {
                        string backupFile = Path.Combine(backupPath, Path.GetRelativePath(appDirectory, file));
                        string backupDir = Path.GetDirectoryName(backupFile);

                        if (!string.IsNullOrEmpty(backupDir) && !Directory.Exists(backupDir))
                        {
                            Directory.CreateDirectory(backupDir);
                        }

                        File.Copy(file, backupFile, true);
                    }

                    MessageBoxResult result = MessageBox.Show("Cập nhật hoàn tất. Bạn cần khởi động lại ứng dụng để áp dụng các thay đổi. Bạn có muốn khởi động lại ngay bây giờ?", "Khởi Động Lại", MessageBoxButton.YesNo, MessageBoxImage.Information);

                    // Tạo script để thay thế các tệp sau khi ứng dụng đã đóng
                    string batchScriptTemp = Path.Combine(Path.GetTempPath(), "update.bat");
                    if (result == MessageBoxResult.Yes)
                    {
                        using (StreamWriter writer = new StreamWriter(batchScriptTemp))
                        {
                            writer.WriteLine("timeout /t 1 /nobreak"); // Chờ 3 giây để đảm bảo ứng dụng hiện tại đã đóng
                            foreach (var file in Directory.GetFiles(innerFolderPath, "*", SearchOption.AllDirectories))
                            {
                                string relativePath = file.Substring(innerFolderPath.Length + 1);
                                string destFile = Path.Combine(appDirectory, relativePath);
                                string destDir = Path.GetDirectoryName(destFile);

                                if (!string.IsNullOrEmpty(destDir) && !Directory.Exists(destDir))
                                {
                                    Directory.CreateDirectory(destDir);
                                }

                                writer.WriteLine($"move /y \"{file}\" \"{destFile}\"");
                            }
                            writer.WriteLine($"start \"\" \"{Environment.ProcessPath}\"");
                        }

                        Process.Start(new ProcessStartInfo
                        {
                            FileName = batchScriptTemp,
                            CreateNoWindow = true,
                            UseShellExecute = false
                        });

                        Application.Current.Shutdown();
                    }
                    else
                    {
                        Application.Current.Shutdown();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Tải xuống và cập nhật không thành công: " + ex.Message);
                Application.Current.Shutdown();
            }
        }

        private Task CreateCleanupScript()
        {
            if (File.Exists(batchScript))
                File.Delete(batchScript);
            if (File.Exists(tempZipFile))
                File.Delete(tempZipFile);
            if (Directory.Exists(extractPath))
                Directory.Delete(extractPath, true);
            if (Directory.Exists(backupPath))
                Directory.Delete(backupPath, true);
            return Task.CompletedTask;
        }
    }
}

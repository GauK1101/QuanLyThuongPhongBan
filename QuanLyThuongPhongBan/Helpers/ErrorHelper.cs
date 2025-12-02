using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;

public static class ErrorHelper
{
    public static (string UserMessage, string DevMessage) HandleError(
        Exception ex,
        ILogger logger = null,
        string context = "")
    {
        var devMessage = CreateDetailedDevMessage(ex, context);
        var userMessage = CreateSocialStyleMessage(ex);

        LogEverything(ex, logger, context, devMessage);

        return (userMessage, devMessage);
    }

    private static string CreateSocialStyleMessage(Exception ex)
    {
        var rootEx = GetRootException(ex);

        return rootEx switch
        {
            // === SQLITE ERRORS ===
            Microsoft.Data.Sqlite.SqliteException sqliteEx when sqliteEx.SqliteErrorCode == 19 =>
                "😊 Dữ liệu này đã tồn tại trong hệ thống! Vui lòng kiểm tra lại thông tin.",

            Microsoft.Data.Sqlite.SqliteException sqliteEx when sqliteEx.SqliteErrorCode == 5 =>
                "🔐 Lỗi truy cập database SQLite! Vui lòng khởi động lại ứng dụng!",

            Microsoft.Data.Sqlite.SqliteException sqliteEx when sqliteEx.SqliteErrorCode == 14 =>
                "🗄️ Không thể mở database SQLite! Vui lòng kiểm tra file database!",

            Microsoft.Data.Sqlite.SqliteException sqliteEx when sqliteEx.SqliteErrorCode == 8 =>
                "💾 Lỗi đọc/ghi database SQLite! Vui lòng thử lại!",

            // === SQL SERVER ERRORS ===
            SqlException sqlEx when sqlEx.Number == 2627 =>
                "😊 Dữ liệu này đã tồn tại trong hệ thống! Vui lòng kiểm tra lại thông tin.",

            SqlException sqlEx when sqlEx.Number == 2601 =>
                "🚫 Thông tin này đã tồn tại! Vui lòng nhập giá trị khác!",

            SqlException sqlEx when sqlEx.Number == 547 =>
                "🔗 Không thể thực hiện vì có dữ liệu liên quan đang được sử dụng!",

            SqlException sqlEx when sqlEx.Number == 4060 =>
                "🌐 Mất kết nối đến máy chủ. Vui lòng kiểm tra kết nối mạng và thử lại!",

            // === ENTITY FRAMEWORK CORE ERRORS ===
            DbUpdateException dbEx when IsDuplicateKey(dbEx) =>
                "😊 Dữ liệu này đã tồn tại trong hệ thống! Vui lòng kiểm tra lại thông tin.",

            DbUpdateException dbEx when IsForeignKeyViolation(dbEx) =>
                "🔗 Không thể thực hiện vì có dữ liệu liên quan đang được sử dụng!",

            DbUpdateConcurrencyException =>
                "📝 Nội dung đã được cập nhật bởi người khác. Vui lòng tải lại trang để xem phiên bản mới nhất!",

            // === FACEBOOK STYLE - Hiển thị như notification ===
            DbUpdateException dbEx when IsDuplicateKey(dbEx) =>
                "😊 Nội dung này đã tồn tại! Vui lòng kiểm tra lại thông tin.",

            DbUpdateException dbEx when IsForeignKeyViolation(dbEx) =>
                "🔗 Không thể thực hiện vì có dữ liệu liên quan đang được sử dụng!",

            // === YOUTUBE STYLE - Hiển thị như error message ===
            SqlException sqlEx when sqlEx.Number == 4060 =>
                "🌐 Mất kết nối đến máy chủ. Vui lòng kiểm tra kết nối mạng và thử lại!",

            SqlException sqlEx when sqlEx.Number == 18456 =>
                "🔐 Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại!",

            HttpRequestException =>
                "📡 Mất kết nối mạng. Vui lòng kiểm tra WiFi hoặc dữ liệu di động!",

            TimeoutException =>
                "⏰ Máy chủ phản hồi chậm. Vui lòng thử lại sau!",

            SocketException =>
                "🔌 Không thể kết nối đến máy chủ. Vui lòng thử lại sau!",

            // === GOOGLE STYLE - Simple and clean ===
            IOException =>
                "📁 Lỗi truy cập file. Vui lòng thử lại!",

            UnauthorizedAccessException =>
                "🚫 Bạn không có quyền thực hiện hành động này!",

            // === BUSINESS LOGIC - Friendly explanations ===
            ArgumentNullException =>
                "📝 Vui lòng điền đầy đủ thông tin bắt buộc!",

            ArgumentException =>
                "🤔 Thông tin nhập không hợp lệ. Vui lòng kiểm tra lại!",

            InvalidOperationException =>
                "⚡ Hành động không khả dụng lúc này. Vui lòng thử lại sau!",

            // === MEMORY/PERFORMANCE ===
            OutOfMemoryException =>
                "💾 Ứng dụng hết bộ nhớ! Vui lòng đóng bớt ứng dụng và thử lại!",

            // === DATABASE CONFLICTS ===
            _ when IsConcurrencyConflict(ex) =>
                "👥 Có người khác vừa thay đổi nội dung này. Vui lòng tải lại trang và thử lại!",

            _ when IsDeadlock(ex) =>
                "🔄 Hệ thống đang bận. Vui lòng thử lại trong giây lát!",

            // === DEFAULT - Friendly fallback ===
            _ => "😅 Đã xảy ra lỗi. Vui lòng thử lại hoặc liên hệ hỗ trợ nếu lỗi tiếp diễn!"
        };
    }

    private static string GetSmartDefaultMessage(Exception ex)
    {
        var exType = ex.GetType().Name;

        // Phân loại theo "tone" của exception
        return exType switch
        {
            string s when s.Contains("Timeout") => "⏰ Yêu cầu hết thời gian chờ. Vui lòng thử lại!",
            string s when s.Contains("Network") => "🌐 Lỗi kết nối. Kiểm tra mạng và thử lại!",
            string s when s.Contains("Database") => "🗄️ Lỗi truy vấn dữ liệu. Vui lòng thử lại sau!",
            string s when s.Contains("Validation") => "📝 Dữ liệu không hợp lệ. Vui lòng kiểm tra lại!",
            string s when s.Contains("Security") => "🔐 Lỗi bảo mật. Vui lòng thử đăng nhập lại!",
            string s when s.Contains("File") => "📁 Lỗi hệ thống file. Khởi động lại ứng dụng!",
            _ => "😅 Đã xảy ra lỗi không mong muốn. Vui lòng thử lại hoặc liên hệ hỗ trợ nếu lỗi tiếp diễn! 💁"
        };
    }

    // ===== SMART DETECTION METHODS =====
    private static bool IsDuplicateKey(DbUpdateException ex)
    {
        return ex.InnerException is SqlException sqlEx &&
               (sqlEx.Number == 2627 || sqlEx.Number == 2601);
    }

    private static bool IsForeignKeyViolation(DbUpdateException ex)
    {
        return ex.InnerException is SqlException sqlEx && sqlEx.Number == 547;
    }

    private static bool IsConcurrencyConflict(Exception ex)
    {
        return ex is DbUpdateConcurrencyException ||
               (ex is DbUpdateException dbEx &&
                dbEx.InnerException is SqlException sqlEx &&
                sqlEx.Number == 1205);
    }

    private static bool IsDeadlock(Exception ex)
    {
        return (ex is SqlException sqlEx && sqlEx.Number == 1205) ||
               (ex is DbUpdateException dbEx &&
                dbEx.InnerException is SqlException innerSql &&
                innerSql.Number == 1205);
    }

    private static Exception GetRootException(Exception ex)
    {
        while (ex.InnerException != null)
            ex = ex.InnerException;
        return ex;
    }

    private static string CreateDetailedDevMessage(Exception ex, string context)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"🚨 [ERROR DETAILS]");
        sb.AppendLine($"📝 Context: {context}");
        sb.AppendLine($"📛 Exception: {ex.GetType().FullName}");
        sb.AppendLine($"💬 Message: {ex.Message}");
        sb.AppendLine($"🔗 StackTrace: {ex.StackTrace}");

        // Thông tin SQL nếu có
        if (ex is SqlException sqlEx)
        {
            sb.AppendLine($"🗄️ SQL Error #{sqlEx.Number}");
            sb.AppendLine($"🖥️ Server: {sqlEx.Server}");
            sb.AppendLine($"📋 Procedure: {sqlEx.Procedure}");
            sb.AppendLine($"🔢 LineNumber: {sqlEx.LineNumber}");
        }

        // Inner Exception
        if (ex.InnerException != null)
        {
            sb.AppendLine($"🔍 Inner Exception: {ex.InnerException.GetType().Name}");
            sb.AppendLine($"💬 Inner Message: {ex.InnerException.Message}");
        }

        // Thông tin hệ thống
        sb.AppendLine($"⏰ Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine($"💻 OS: {Environment.OSVersion}");

        return sb.ToString();
    }

    private static void LogEverything(Exception ex, ILogger logger, string context, string devMessage)
    {
        // Log ngắn gọn nhưng đủ thông tin
        logger?.LogError(ex, "🚨 {Context} | {ExceptionType}", context, ex.GetType().Name);

        // Console log với emoji
        Console.WriteLine($"🔴 {DateTime.Now:HH:mm:ss} | {context} | {ex.GetType().Name}: {ex.Message}");

        // Log chi tiết vào file
        LogToFile(devMessage);
    }

    private static void LogToFile(string message)
    {
        try
        {
            var logFile = Path.Combine("Logs", $"errors_{DateTime.Now:yyyyMMdd}.log");
            Directory.CreateDirectory(Path.GetDirectoryName(logFile));
            File.AppendAllText(logFile, $"{message}\n\n");
        }
        catch { /* Ignore file log errors */ }
    }
}
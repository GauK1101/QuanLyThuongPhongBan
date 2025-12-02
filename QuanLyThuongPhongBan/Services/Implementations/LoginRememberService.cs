using QuanLyThuongPhongBan.Helpers;
using QuanLyThuongPhongBan.Models;
using QuanLyThuongPhongBan.Services.Interfaces;
using System.IO;
using System.Text.Json;

namespace QuanLyThuongPhongBan.Services.Implementations
{ 
    public class LoginRememberService : ILoginRememberService
    {
        private const string FilePath = "remember.json";

        public LoginRemember LoadRememberedLogin()
        {
            try
            {
                if (!File.Exists(FilePath)) return new LoginRemember();

                var json = File.ReadAllText(FilePath);
                var encryptedModel = JsonSerializer.Deserialize<LoginRemember>(json);

                if (encryptedModel?.RememberMe == true && !string.IsNullOrEmpty(encryptedModel.EncryptedPassword))
                {
                    encryptedModel.EncryptedPassword = AesEncryptionHelper.Decrypt(encryptedModel.EncryptedPassword);
                }

                return encryptedModel ?? new LoginRemember();
            }
            catch
            {
                return new LoginRemember();
            }
        }

        public void SaveRememberedLogin(LoginRemember model)
        {
            try
            {
                var toSave = new LoginRemember
                {
                    Username = model.Username,
                    RememberMe = model.RememberMe
                };

                if (model.RememberMe && !string.IsNullOrEmpty(model.EncryptedPassword))
                {
                    toSave.EncryptedPassword = AesEncryptionHelper.Encrypt(model.EncryptedPassword);
                }

                var json = JsonSerializer.Serialize(toSave, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(FilePath, json);
            }
            catch { /* yên lặng là vàng */ }
        }

        public void ClearRememberedLogin()
        {
            try { if (File.Exists(FilePath)) File.Delete(FilePath); }
            catch { }
        }
    }
}

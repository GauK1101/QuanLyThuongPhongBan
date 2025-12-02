using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace QuanLyThuongPhongBan.Helpers
{
    public static class AesEncryptionHelper
    {
        // 🎯 KEY CỐ ĐỊNH - ĐỪNG THAY ĐỔI SAU KHI DEPLOY
        private static readonly string EncryptionKey = "b14ca5898a4e4133bbce2ea2315a1916"; // 32 ký tự
        private static readonly byte[] Key = Encoding.UTF8.GetBytes(EncryptionKey);
        private static readonly byte[] Iv = new byte[16]; // IV mặc định

        public static string Encrypt(string plainText)
        {
            using var aes = Aes.Create();
            aes.Key = Key;
            aes.IV = Iv;

            using var encryptor = aes.CreateEncryptor();
            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs))
            {
                sw.Write(plainText);
            }
            return Convert.ToBase64String(ms.ToArray());
        }

        public static string Decrypt(string cipherText)
        {
            using var aes = Aes.Create();
            aes.Key = Key;
            aes.IV = Iv;

            using var decryptor = aes.CreateDecryptor();
            using var ms = new MemoryStream(Convert.FromBase64String(cipherText));
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);
            return sr.ReadToEnd();
        }
    }
}

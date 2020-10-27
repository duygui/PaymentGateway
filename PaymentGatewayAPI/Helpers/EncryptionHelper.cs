using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace PaymentGatewayAPI.Helpers
{
    public static class EncryptionHelper
    {
        private const string EncryptionKey = "mygatewayencryption";
        public static string Encrypt(this string stringToEncrypt)
        {
            // Get the bytes of the string
            var bytesToBeEncrypted = Encoding.UTF8.GetBytes(stringToEncrypt);
            var passwordBytes = Encoding.UTF8.GetBytes(EncryptionKey);

            // Hash the password with SHA256
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            var bytesEncrypted = Encrypt(bytesToBeEncrypted, passwordBytes);
            return Convert.ToBase64String(bytesEncrypted);
        }

        private static byte[] Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes = null;
            var saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using RijndaelManaged AES = new RijndaelManaged();
                var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                AES.KeySize = 256;
                AES.BlockSize = 128;
                AES.Key = key.GetBytes(AES.KeySize / 8);
                AES.IV = key.GetBytes(AES.BlockSize / 8);
                AES.Mode = CipherMode.CBC;

                using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                    cs.Close();
                }
                encryptedBytes = ms.ToArray();
            }

            return encryptedBytes;
        }
    }
}

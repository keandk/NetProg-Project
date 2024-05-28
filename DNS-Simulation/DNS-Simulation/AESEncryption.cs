using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DNS_Simulation
{
    public static class AESEncryption
    {
        private const string Key = "Nhom5DNSSimulation123";
        private const string IVString = "TKienTBinhHThong";

        public static string Encrypt(string plainText)
        {
            byte[] encrypted;
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(Key);
                aes.IV = GenerateIV(IVString);

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(plainText);
                        }
                        encrypted = ms.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(encrypted);
        }

        public static string Decrypt(string encryptedText)
        {
            string plainText = null;
            byte[] encrypted = Convert.FromBase64String(encryptedText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(Key);
                aes.IV = GenerateIV(IVString);

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream(encrypted))
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(cs))
                        {
                            plainText = sr.ReadToEnd();
                        }
                    }
                }
            }

            return plainText;
        }

        private static byte[] GenerateIV(string ivString)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(ivString));
                byte[] iv = new byte[16];
                Array.Copy(hash, iv, 16);
                return iv;
            }
        }
    }
}
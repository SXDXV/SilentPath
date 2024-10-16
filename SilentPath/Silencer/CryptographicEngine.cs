using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SilentPath.Silencer
{
    public class CryptographicEngine
    {
        private static readonly int KeySize = 256;
        private static readonly int BlockSize = 128;

        public static string Encrypt(string plainText, string secretKey)
        {
            using (Aes aes = Aes.Create())
            {
                aes.KeySize = KeySize;
                aes.BlockSize = BlockSize;

                using (var keyDerivationFunction = new Rfc2898DeriveBytes(secretKey, Encoding.UTF8.GetBytes("SilentPathSalt"), 10000))
                {
                    aes.Key = keyDerivationFunction.GetBytes(KeySize / 8);
                    aes.IV = keyDerivationFunction.GetBytes(BlockSize / 8);
                }

                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (var sw = new StreamWriter(cs))
                        {
                            sw.Write(plainText);
                        }
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }


        public static string Decrypt(string cipherText, string secretKey)
        {
            using (Aes aes = Aes.Create())
            {
                aes.KeySize = KeySize;
                aes.BlockSize = BlockSize;

                using (var keyDerivationFunction = new Rfc2898DeriveBytes(secretKey, Encoding.UTF8.GetBytes("SilentPathSalt"), 10000))
                {
                    aes.Key = keyDerivationFunction.GetBytes(KeySize / 8);
                    aes.IV = keyDerivationFunction.GetBytes(BlockSize / 8);
                }

                var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (var ms = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (var sr = new StreamReader(cs))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
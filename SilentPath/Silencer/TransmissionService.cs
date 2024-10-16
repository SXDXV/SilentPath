using System;
using System.Net.Sockets;
using System.Text;
using System.Security.Cryptography;

namespace SilentPath.Silencer
{
    public class TransmissionService
    {
        public static void SendMessage(string message, string secretKey, string serverIp)
        {
            try
            {
                var client = new TcpClient();
                client.Connect(serverIp, 5000);

                NetworkStream stream = client.GetStream();
                
                string encryptedMessage = EncryptMessage(message, secretKey);
                byte[] data = Encoding.UTF8.GetBytes(encryptedMessage);

                stream.Write(data, 0, data.Length);
                Console.WriteLine("Сообщение отправлено");

                byte[] buffer = new byte[1024];
                int byteCount = stream.Read(buffer, 0, buffer.Length);
                string response = Encoding.UTF8.GetString(buffer, 0, byteCount);
                Console.WriteLine($"Ответ от сервера: {response}");

                stream.Close();
                client.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка клиента: {ex.Message}");
            }
        }

        private static string EncryptMessage(string message, string secretKey)
        {
            using (Aes aesAlg = Aes.Create())
            {
                byte[] key = Encoding.UTF8.GetBytes(secretKey.PadRight(32));
                aesAlg.Key = key;
                aesAlg.IV = new byte[16]; 

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (var msEncrypt = new System.IO.MemoryStream())
                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                using (var swEncrypt = new System.IO.StreamWriter(csEncrypt))
                {
                    swEncrypt.Write(message);
                    swEncrypt.Flush();
                    csEncrypt.FlushFinalBlock();
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }
    }
}
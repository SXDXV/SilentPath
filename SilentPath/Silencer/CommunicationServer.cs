using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Security.Cryptography;
using System.Threading;

namespace SilentPath.Silencer
{
    public class CommunicationServer
    {
        private static TcpListener _server;

        public static void StartServer(string secretKey)
        {
            try
            {
                _server = new TcpListener(IPAddress.Any, 5000);
                _server.Start();
                Console.WriteLine("Сервер запущен и ожидает подключения...");

                while (true)
                {
                    var client = _server.AcceptTcpClient();
                    var thread = new Thread(() => HandleClient(client, secretKey));
                    thread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка сервера: {ex.Message}");
            }
        }

        private static void HandleClient(TcpClient client, string secretKey)
        {
            try
            {
                Console.WriteLine("Клиент подключен.");
                NetworkStream stream = client.GetStream();
                
                byte[] buffer = new byte[1024];
                int byteCount = stream.Read(buffer, 0, buffer.Length);
                string encryptedMessage = Encoding.UTF8.GetString(buffer, 0, byteCount);
                
                string decryptedMessage = DecryptMessage(encryptedMessage, secretKey);
                Console.WriteLine($"Получено зашифрованное сообщение: {encryptedMessage}");
                Console.WriteLine($"Расшифрованное сообщение: {decryptedMessage}");

                string response = "Сообщение получено";
                byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                stream.Write(responseBytes, 0, responseBytes.Length);

                stream.Close();
                client.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обработке клиента: {ex.Message}");
            }
        }

        private static string DecryptMessage(string encryptedMessage, string secretKey)
        {
            using (Aes aesAlg = Aes.Create())
            {
                byte[] key = Encoding.UTF8.GetBytes(secretKey.PadRight(32));
                aesAlg.Key = key;
                aesAlg.IV = new byte[16];

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                byte[] encryptedBytes = Convert.FromBase64String(encryptedMessage);

                using (var msDecrypt = new System.IO.MemoryStream(encryptedBytes))
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (var srDecrypt = new System.IO.StreamReader(csDecrypt))
                {
                    return srDecrypt.ReadToEnd();
                }
            }
        }
    }
}

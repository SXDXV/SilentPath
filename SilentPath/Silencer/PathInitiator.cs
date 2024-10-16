using System;

namespace SilentPath.Silencer
{
    public class PathInitiator
    {
        public static void ExecuteSilentMode(string[] args)
        {
            Main(args); 
        }

        private static void Main(string[] args)
        {
            Console.WriteLine("Выберите режим: 1 - Сервер, 2 - Клиент");
            var mode = Console.ReadLine();

            if (mode == "1")
            {
                // Серверная часть
                Console.Write("Введите секретный ключ для сервера: ");
                string secretKey = Console.ReadLine();
                CommunicationServer.StartServer(secretKey);
            }
            else if (mode == "2")
            {
                // Клиентская часть
                Console.Write("Введите IP-адрес сервера: ");
                string serverIp = Console.ReadLine();

                Console.Write("Введите секретный ключ: ");
                string secretKey = Console.ReadLine();

                Console.Write("Введите сообщение для отправки: ");
                string message = Console.ReadLine();

                TransmissionService.SendMessage(message, secretKey, serverIp);
            }
            else
            {
                Console.WriteLine("Неверный выбор режима.");
            }
        }
    }
}
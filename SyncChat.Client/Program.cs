using System;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

namespace SyncChat.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            int port = 8888;
            TcpClient client = new TcpClient();
            client.Connect("127.0.0.1",port);
            Console.WriteLine("Подключено к серверу!");
            NetworkStream stream = client.GetStream();
            Console.Write("Введите сообщение для сервера: ");
            string message = Console.ReadLine();
            byte[] data = Encoding.UTF8.GetBytes(message);
            stream.Write(data, 0, data.Length);
            Console.WriteLine("Сообщение отправлено");
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine($"Прочитано байт: {bytesRead}");
            Console.WriteLine($"Ответ от сервера: {response}");
            client.Close();
            Console.WriteLine("Нажмите любую кнопку для выхода...");
            Console.ReadKey();
        }
    }
}
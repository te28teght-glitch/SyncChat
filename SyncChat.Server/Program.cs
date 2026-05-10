using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SyncChat.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpListener listener = new TcpListener(IPAddress.Any, 8888);
            listener.Start();
            Console.WriteLine("Сервер запущен. Порт:8888. Ожидание подключения...");

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine("Клиент подключился!");

                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Получено: {receivedMessage}");

                byte[] response = Encoding.UTF8.GetBytes(receivedMessage);
                stream.Write(response, 0, response.Length);
                Console.WriteLine("Ответ отправлен клиенту");
    
                
                client.Close();
            }
        }
    }
}
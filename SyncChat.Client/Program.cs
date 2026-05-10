using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SyncChat.Client
{
    class Program
    {
        private static TcpClient? _client;
        private static NetworkStream? _stream;
        private static bool _isRunning = true;

        static void Main(string[] args)
        {
            _client = new TcpClient();
            _client.Connect("127.0.0.1", 8888);
            _stream = _client.GetStream();
            Console.WriteLine("Подключено к серверу!");

            // Поток для прослушивания сообщений от сервера
            Thread receiveThread = new Thread(ReceiveMessages);
            receiveThread.Start();

            // Основной поток для отправки сообщений
            while (_isRunning)
            {
                string? message = Console.ReadLine();
                if (message == "/exit")
                {
                    _isRunning = false;
                    break;
                }

                if (!string.IsNullOrEmpty(message) && _stream != null)
                {
                    byte[] data = Encoding.UTF8.GetBytes(message);
                    _stream.Write(data, 0, data.Length);
                }
            }

            _client?.Close();
            Console.WriteLine("Отключено от сервера");
        }

        static void ReceiveMessages()
        {
            byte[] buffer = new byte[1024];
            try
            {
                while (_isRunning && _stream != null)
                {
                    int bytesRead = _stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"[Сервер]: {message}");
                }
            }
            catch
            {
                // Соединение разорвано
            }
        }
    }
}
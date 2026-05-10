using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SyncChat.Server
{
    class Program
    {
        public static List<ClientHandler> _clients = new List<ClientHandler>();

        static void Main(string[] args)
        {
            TcpListener listener = new TcpListener(IPAddress.Any, 8888);
            listener.Start();
            Console.WriteLine("Сервер запущен на порту 8888. Ожидание подключений...");

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                ClientHandler handler = new ClientHandler(client);

                lock (_clients)
                {
                    _clients.Add(handler);
                }

                Thread clientThread = new Thread(handler.Run);
                clientThread.Start();
            }
        }
    }
}
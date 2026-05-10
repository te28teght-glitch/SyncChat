using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SyncChat.Server
{
    public class ClientHandler
    {
        private TcpClient _client;
        private NetworkStream _stream;
        private string _clientId;
        private byte[] _buffer;

        public ClientHandler(TcpClient client)
        {
            _client = client;
            _stream = _client.GetStream();
            _clientId = Guid.NewGuid().ToString().Substring(0, 8);
            _buffer = new byte[1024];
        }

        public void Run()
        {
            Console.WriteLine($"[{_clientId}] Клиент подключился");

            try
            {
                while (true)
                {
                    int bytesRead = _stream.Read(_buffer, 0, _buffer.Length);
                    
                    if (bytesRead == 0)
                    {
                        Console.WriteLine($"[{_clientId}] Клиент отключился");
                        break;
                    }

                    string message = Encoding.UTF8.GetString(_buffer, 0, bytesRead);
                    Console.WriteLine($"[{_clientId}] {message}");

                    // Рассылаем сообщение всем клиентам
                    BroadcastMessage($"[{_clientId}]: {message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{_clientId}] Ошибка: {ex.Message}");
            }
            finally
            {
                _stream?.Close();
                _client?.Close();
                
                lock (Program._clients)
                {
                    Program._clients.Remove(this);
                }
                Console.WriteLine($"[{_clientId}] Клиент удалён");
            }
        }

        private void BroadcastMessage(string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            
            lock (Program._clients)
            {
                foreach (var client in Program._clients)
                {
                    try
                    {
                        client.SendMessage(data);
                    }
                    catch
                    {
                        // Не удалось отправить — пропускаем
                    }
                }
            }
        }

        public void SendMessage(byte[] data)
        {
            _stream.Write(data, 0, data.Length);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ConsoleApp2
{
    class Server
    {
        private static readonly List<Player> players = new List<Player>();
        private static readonly List<Point> points = new List<Point>();
        private static readonly object lockObject = new object();

        static void Main(string[] args)
        {
            TcpListener listener = null;

            try
            {
                listener = new TcpListener(IPAddress.Any, 2323);
                listener.Start();
                Console.WriteLine("Сервер запущен. Ожидание подключений...");

                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    Player player = new Player(client);
                    lock (lockObject)
                    {
                        players.Add(player);
                        SendPlayerList();
                        SendPoints(player);
                    }

                    Thread thread = new Thread(HandleClient);
                    thread.Start(player);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка сервера: " + ex.Message);
            }
            finally
            {
                listener?.Stop();
            }
        }

        static void HandleClient(object obj)
        {
            Player player = (Player)obj;

            try
            {
                NetworkStream stream = player.Client.GetStream();
                byte[] buffer = new byte[1024];
                int bytesRead;

                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    ProcessMessage(player, message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка соединения с клиентом: " + ex.Message);
            }
            finally
            {
                lock (lockObject)
                {
                    players.Remove(player);
                    BroadcastPlayerListExcept(player);
                }
                player.Client.Close();
                player.Client.Dispose();
            }
        }

        static void ProcessMessage(Player player, string message)
        {
            string[] parts = message.Split(':');
            string command = parts[0];

            switch (command)
            {
                case "POINT":
                    int x = int.Parse(parts[1]);
                    int y = int.Parse(parts[2]);
                    string color = parts[3];
                    Point point = new Point(player, x, y, color);
                    lock (lockObject)
                    {
                        points.Add(point);
                        BroadcastPoint(point);
                    }
                    break;
                case "DISCONNECT":
                    lock (lockObject)
                    {
                        players.Remove(player);
                        BroadcastPlayerListExcept(player);
                    }
                    player.Client.Close();
                    player.Client.Dispose();
                    break;
                default:
                    Console.WriteLine("Неизвестная команда: " + command);
                    break;
            }
        }

        static void SendPlayerList()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("PLAYERS:");
            foreach (Player player in players)
            {
                sb.Append(player.Username + ":" + player.ColorString + ",");
            }
            sb.Append(".");
            string message = sb.ToString().TrimEnd(',');
            byte[] buffer = Encoding.ASCII.GetBytes(message);
            foreach (Player player in players)
            {
                player.Client.GetStream().Write(buffer, 0, buffer.Length);
            }
        }

        static void SendPoints(Player player)
        {
            foreach (Point point in points)
            {
                string message = $"POINT:{point.X}:{point.Y}:{point.Color}.";
                byte[] buffer = Encoding.ASCII.GetBytes(message);
                player.Client.GetStream().Write(buffer, 0, buffer.Length);
            }
        }

        static void BroadcastPoint(Point point)
        {
            string message = $"POINT:{point.X}:{point.Y}:{point.Color}.";
            byte[] buffer = Encoding.ASCII.GetBytes(message);
            foreach (Player player in players)
            {
                player.Client.GetStream().Write(buffer, 0, buffer.Length);
            }
        }

        static void BroadcastPlayerListExcept(Player excludedPlayer)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("PLAYERS:");
            foreach (Player player in players)
            {
                if (player != excludedPlayer)
                {
                    sb.Append(player.Username + ":" + player.ColorString + ",");
                }
            }
            string message = sb.ToString().TrimEnd(',');
            byte[] buffer = Encoding.ASCII.GetBytes(message);
            foreach (Player player in players)
            {
                if (player != excludedPlayer)
                {
                    player.Client.GetStream().Write(buffer, 0, buffer.Length);
                }

                if (players.Count == 0)
                {
                    excludedPlayer.Client.Close();
                    excludedPlayer.Client.Dispose();
                }
            }
        }
    }

    class Player
    {
        public TcpClient Client { get; }
        public string Username { get; }

        public string ColorString { get; }

        public Player(TcpClient client)
        {
            Client = client;
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            var message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            var parts = message.Replace(':', ' ').Split(' ');
            Username = parts[0];
            ColorString = parts[1];
        }
    }

    class Point
    {
        public Player Player { get; }
        public int X { get; }
        public int Y { get; }
        public string Color { get; }

        public Point(Player player, int x, int y, string color)
        {
            Player = player;
            X = x;
            Y = y;
            Color = color;
        }
    }
}
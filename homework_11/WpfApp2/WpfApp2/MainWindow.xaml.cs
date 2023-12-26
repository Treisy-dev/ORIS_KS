using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.ObjectModel;

namespace WpfApp2
{
    public partial class MainWindow : Window
    {
        private TcpClient client;
        private NetworkStream stream;
        private byte[] buffer;
        ObservableCollection<Player> collection = new ObservableCollection<Player>();


        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Closing += MainWindow_Closing;
            string username = PromptUsername();
            if (!string.IsNullOrEmpty(username))
            {
                ConnectToServer(username);
            }
            else
            {
                Close();
            }
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string message = "DISCONNECT:";
            byte[] messageBuffer = Encoding.ASCII.GetBytes(message);
            stream.Write(messageBuffer, 0, messageBuffer.Length);
        }

        private string PromptUsername()
        {
            PromptUsernameWindow promptUsernameWindow = new PromptUsernameWindow();
            bool? result = promptUsernameWindow.ShowDialog();
            if (result == true)
            {
                return promptUsernameWindow.Username;
            }
            return null;
        }

        private void ConnectToServer(string username)
        {
            try
            {
                client = new TcpClient();
                client.Connect("localhost", 2323);
                stream = client.GetStream();
                buffer = new byte[1024];

                var message = username + ':' + GetRandomColor().ToString();
                byte[] usernameBuffer = Encoding.ASCII.GetBytes(message);
                stream.Write(usernameBuffer, 0, usernameBuffer.Length);

                Thread receiveThread = new Thread(ReceiveMessages);
                receiveThread.Start();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка подключения к серверу: " + ex.Message);
                Close();
            }
        }

        private void ReceiveMessages()
        {
            try
            {
                while (true)
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    ProcessMessage(message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка приема сообщений: " + ex.Message);
            }
            finally
            {
                stream.Close();
                client.Close();
            }
        }

        private void ProcessMessage(string message)
        {
            message = message.Replace('.', ' ');
            string[] parts = message.Split(' ');
            foreach (string part in parts)
            {
                if (part.StartsWith("PLAYERS:"))
                {
                    Dispatcher.Invoke(() =>
                    {
                        playerList.Items.Clear();
                    });
                    string[] playerNames = part.Substring(8).Replace(',', ' ').Split(' ');
                    foreach (string playerString in playerNames)
                    {
                        if (playerString.Length > 8)
                        {
                            var playerParts = playerString.Replace(':', ' ').Split(' ');
                            Player player = new Player(playerParts[0], playerParts[1]);
                            Dispatcher.Invoke(() =>
                            {
                                playerList.Items.Add(player);
                            });
                        }
                    }
                }
                else if (part.StartsWith("POINT:"))
                {
                    string[] dotParts = part.Substring(6).Split(':');
                    int x = int.Parse(dotParts[0]);
                    int y = int.Parse(dotParts[1]);
                    string colorString = dotParts[2];
                    Color color = (Color)ColorConverter.ConvertFromString(colorString);
                    Dispatcher.Invoke(() =>
                    {
                        DrawPoint(new Point(x, y), color);
                        canvas.UpdateLayout();
                    });
                }
            }
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point position = e.GetPosition(canvas);
            Color color = GetRandomColor();
            DrawPoint(position, color);
            SendPoint(position, color);
        }

        private void DrawPoint(Point position, Color color)
        {
            Ellipse point = new Ellipse
            {
                Width = 10,
                Height = 10,
                Fill = new SolidColorBrush(color),
                Margin = new Thickness(position.X - 5, position.Y - 5, 0, 0)
            };
            canvas.Children.Add(point);
        }

        private void SendPoint(Point position, Color color)
        {
            string message = $"POINT:{position.X}:{position.Y}:{color}";
            byte[] messageBuffer = Encoding.ASCII.GetBytes(message);
            stream.Write(messageBuffer, 0, messageBuffer.Length);
        }

        private Color GetRandomColor()
        {
            Random random = new Random();
            byte[] rgb = new byte[3];
            random.NextBytes(rgb);
            return Color.FromRgb(rgb[0], rgb[1], rgb[2]);
        }
    }

    public class Player
    {
        public string Username { get; set; }
        public Color Color { get; set; }

        public Player(string username, string color)
        {
            Username = username;
            Color = ParseColor(color);
        }

        public Color ParseColor(string colorString)
        {
            string hexString = colorString.TrimStart('#');

            int colorValue = Convert.ToInt32(hexString, 16);

            byte red = (byte)((colorValue >> 16) & 0xFF);
            byte green = (byte)((colorValue >> 8) & 0xFF);
            byte blue = (byte)(colorValue & 0xFF);

            Color color = Color.FromRgb(red, green, blue);
            return color;
        }
    }
}
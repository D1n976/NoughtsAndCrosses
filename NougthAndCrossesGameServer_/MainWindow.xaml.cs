using Client;
using GameOfNougtsAndCrossesLib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NougthAndCrossesGameServer_
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ConcurrentDictionary<Player, TcpClient> clients = new();
        public ObservableCollection<string> Logs { get; private set; } = new ObservableCollection<string>();
        private Game game = new Game(Field.CreateField(), GameStatus.NotStart);

        public MainWindow()
        {
            InitializeComponent();
            DataContext = Logs;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TcpListener listener = new TcpListener(LocalHost.IPEndPoint);
            listener.Start();
            ServeClients(listener);
        }

        private async void ServeClients(TcpListener listener)
        {
            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                Logs.Add($"Client {client.Client.RemoteEndPoint} connect to server");
                ServeClient(client);
            }
        }

        private bool isRolesAssigned = false;
        private object keyA = new object();
        private async void ServeClient(TcpClient client)
        {
            while (true)
            {
                Game game = await Game.ReceiveInfo(client.GetStream());
                //Добавляем игрока
                if (!clients.ContainsKey(game.Player))
                    clients.TryAdd(game.Player, client);

                //Начинаем игру
                if (clients.Count == 2)
                    this.game.GameStatus = GameStatus.Start;
                else
                    this.game.GameStatus = GameStatus.NotStart;
                game.GameStatus = this.game.GameStatus;

                //Распределяем роли
                if (!isRolesAssigned)
                    if (clients.Count == 1)
                    {
                        game.Player.PlayerMode = PlayerMode.CrossMode;
                        game.Player.IsMoveBlocked = false;
                    }
                    else
                    {
                        game.Player.PlayerMode = PlayerMode.NougthMode;
                        game.Player.IsMoveBlocked = true;
                        lock(keyA)
                            isRolesAssigned = true;
                    }

                //Добавляем сделанный ход в поле
                this.game.Field.SynchronizeWithPlayer(game.Field);
                game.Field = this.game.Field;

                //Определяем, какой игрок сейчас ходит
                if (isRolesAssigned)
                {
                    game.Player.IsMoveBlocked = true;

                    //Проверяем игровое поле, в зависимости от результата, меняем GameStatus
                    this.game.CheckGameStatus();
                    game.GameStatus = this.game.GameStatus;
                }

                //Отсылаем измененные данные клиетну
                clients.Values.ToList().ForEach(x => x.GetStream().Write(game.SendInfo()));
            }
        }
    }
}
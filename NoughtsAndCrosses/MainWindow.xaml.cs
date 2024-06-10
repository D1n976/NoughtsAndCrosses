using Client;
using GameOfNougtsAndCrossesLib;
using MyLibraryNP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
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

namespace NoughtsAndCrosses
{
    public partial class MainWindow : Window
    {
        private Game Game { get; set; }
        private TcpClient tcpClient;
        private SocketInfo SocketInfo { get; set; }

        public MainWindow(PlayerViewModel playerViewModel)
        {
            InitializeComponent();
            SocketInfo = new SocketInfo(playerViewModel.IpAddress, playerViewModel.Port);

            Game = new Game(new Player(playerViewModel.Name, playerViewModel.PlayerMode, false),
            CreateField(), GameStatus.NotStart);

            ConnectToServer();
        }

        private async void ConnectToServer()
        {
            tcpClient = new(SocketInfo.IpAddress, SocketInfo.Port);

            await tcpClient.GetStream().WriteAsync(Game.SendInfo());

            Listen();
        }

        private async void Listen()
        {
            while (tcpClient.Connected)
            {
                try
                {
                    NetworkStream stream = await Task.Run(() => tcpClient.GetStream());
                    if (stream.DataAvailable == true)
                        AcceptInfo(stream);
                }
                catch (SocketException)
                {
                    MessageBox.Show("Произошла ошибка на стороне сервера");
                    Close();
                }
            }
        }

        private async void AcceptInfo(Stream stream)
        {
            Game game = await Game.ReceiveInfo(stream);
            Game.Field = game.Field;
            Game.GameStatus = game.GameStatus;
            if (Game.Player.Name == game.Player.Name)
            {
                Game.Player.PlayerMode = game.Player.PlayerMode;
                Game.Player.IsMoveBlocked = game.Player.IsMoveBlocked;
            }
            else
                Game.Player.IsMoveBlocked = false;

            Game.Field.DrawField(fieldListView);

            if (Game.Player.IsMoveBlocked || Game.GameStatus == GameStatus.NotStart)
                fieldListView.IsEnabled = false;
            else
                fieldListView.IsEnabled = true;

            AnnonceGameStatus();
            AnnounceTheMove(game);
        }

        public Field CreateField()
        {
            Field field = new Field();
            //Привязка кнопок и поля по uid
            for (int row = 0; row < Field.FieldWidth; row++)
                for (int col = 0; col < Field.FieldWidth; col++)
                {
                    Button button = new Button();
                    button.Click += Button_Click;
                    fieldListView.Items.Add(button);
                    button.MinHeight = 50;
                    button.MinWidth = 50;
                    string uid = $"({row};{col})";
                    button.Uid = uid;
                    field[row, col] = new Cell(CellStatus.None, uid);
                }
            return field;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Game.GameStatus != GameStatus.Start)
            {
                MessageBox.Show("Игра окончена!");
                return;
            }

            Button but = ((Button)sender);
            //Игрок делает ход
            Game.Player.Move(Game.Field.FindCell(but));

            //Отсылает серверу о сделанном действии
            await tcpClient.GetStream().WriteAsync(Game.SendInfo());
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
            => Environment.Exit(0);

        private void AnnounceTheMove(Game game)
        {
            if (game.GameStatus != GameStatus.Start)
                return;
            if (Game.Player.IsMoveBlocked)
                plrCurrentTurn.Text = $"Ход противника";
            else
                plrCurrentTurn.Text = $"Ваш ход";
        }
        private void AnnonceGameStatus() 
            => messageTextBlock.Text = MessagesByGameStatus.GetMessage(Game.GameStatus);
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GameOfNougtsAndCrossesLib
{
    public enum GameStatus { NotStart, Start, VictoryOfCrosses, VictoryOfNougth, Draw }

    public class Game
    {
        public Player Player { get; set; }
        public Field Field { get; set; }
        public GameStatus GameStatus { get; set; }

        public Game(Player player, Field field, GameStatus gameStatus)
        {
            Player = player;
            Field = field;
            GameStatus = gameStatus;
        }

        public Game(Field field, GameStatus gameStatus)
        {
            Field = field;
            GameStatus = gameStatus;
        }

        public byte[] SendInfo()
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);

            //Имя игрока
            byte[] buf = Encoding.Unicode.GetBytes(Player.Name);
            writer.Write(buf.Length);
            writer.Write(buf);

            //PlayerMode
            writer.Write(BitConverter.GetBytes((int)Player.PlayerMode));

            //Состояние игрока
            writer.Write(BitConverter.GetBytes(Player.IsMoveBlocked));

            //GameStatus
            writer.Write(BitConverter.GetBytes((int)GameStatus));

            //Игровое поле
            writer.Write(Field.Send());

            return stream.ToArray();
        }

        public static async Task<Game> ReceiveInfo(Stream stream)
        {
            byte[] buffer = new byte[4];
            await stream.ReadAsync(buffer, 0, buffer.Length);

            //Получаем имя игрока
            buffer = new byte[BitConverter.ToInt32(buffer, 0)];
            await stream.ReadAsync(buffer, 0, buffer.Length);
            string name = Encoding.Unicode.GetString(buffer);

            //PlayerMode
            buffer = new byte[4];
            await stream.ReadAsync(buffer, 0, buffer.Length);
            PlayerMode playerMode = (PlayerMode)BitConverter.ToInt32(buffer, 0);

            //Состояние игрока
            buffer = new byte[1];
            await stream.ReadAsync(buffer, 0, buffer.Length);
            bool isMoveBlocked = BitConverter.ToBoolean(buffer, 0);

            //GameStatus
            buffer = new byte[4];
            await stream.ReadAsync(buffer, 0, buffer.Length);
            GameStatus gameStatus = (GameStatus)BitConverter.ToInt32(buffer, 0);

            //Игровое поле
            Field field = new Field(stream);

            return new Game(new Player(name, playerMode, isMoveBlocked), field, gameStatus);
        }

        public void CheckGameStatus() 
            => GameStatus = Field.CheckGameStatus();
    }
}

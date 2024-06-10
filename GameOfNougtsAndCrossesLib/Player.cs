using MyLibraryNP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace GameOfNougtsAndCrossesLib
{
    public enum PlayerMode { CrossMode, NougthMode }
    public class Player
    {
        public string Name { get; set; }
        public PlayerMode PlayerMode { get; set; }
        public SocketInfo SocketInfo { get; set; } = new SocketInfo();
        public bool IsMoveBlocked { get; set; } = false;

        public Player(string name, PlayerMode playerMode, SocketInfo socketInfo, bool isMoveBlocked)
        {
            Name = name;
            PlayerMode = playerMode;
            SocketInfo = socketInfo;
            IsMoveBlocked = isMoveBlocked;
        }

        public Player(string name, PlayerMode playerMode, bool isMoveBlocked)
        {
            Name = name;
            PlayerMode = playerMode;
            IsMoveBlocked = isMoveBlocked;
        }

        //Игрок делает ход  клетка меняется в зивисимости от 
        public void Move(Cell cell) 
            => cell.Status = GetCellStatus(PlayerMode);

        public static CellStatus GetCellStatus(PlayerMode playerMode)
        {
            if (playerMode == PlayerMode.CrossMode)
                return CellStatus.Cross;
            else if (playerMode == PlayerMode.NougthMode)
                return CellStatus.Nougth;
            return CellStatus.None;
        }
    }
}

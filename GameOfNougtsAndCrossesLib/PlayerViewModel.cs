using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfNougtsAndCrossesLib
{
    public class PlayerViewModel : INotifyPropertyChanged
    {
        Player player;

        public PlayerViewModel(Player player)
        {
            this.player = player;
        }

        public string Name
        {
            get => player.Name;
            set
            {
                if (player.Name == value) 
                    return;
                player.Name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            }
        }

        public PlayerMode PlayerMode
        {
            get => player.PlayerMode;
            set
            {
                if (player.PlayerMode == value)
                    return;
                player.PlayerMode = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PlayerMode)));
            }
        }

        public string IpAddress
        {
            get => player.SocketInfo.IpAddress;
            set
            {
                if (player.SocketInfo.IpAddress == value)
                    return;
                player.SocketInfo.IpAddress = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IpAddress)));
            }
        }

        public int Port
        {
            get => player.SocketInfo.Port;
            set
            {
                if (player.SocketInfo.Port == value)
                    return;
                player.SocketInfo.Port = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Port)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

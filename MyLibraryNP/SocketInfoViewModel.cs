using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLibraryNP
{
    public class SocketInfoViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private SocketInfo SocketInfo { get; }

        public SocketInfoViewModel(SocketInfo socketInfo)
        {
            SocketInfo = socketInfo;
        }

        public string IpAddress
        {
            get => SocketInfo.IpAddress;
            set
            {
                if (SocketInfo.IpAddress == value)
                    return;
                SocketInfo.IpAddress = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IpAddress)));
            }
        }

        public int Port
        {
            get => SocketInfo.Port;
            set
            {
                if (SocketInfo.Port == value)
                    return;
                SocketInfo.Port = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Port)));
            }
        }
    }
}

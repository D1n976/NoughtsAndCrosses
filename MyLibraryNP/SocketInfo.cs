using System;

namespace MyLibraryNP
{
    public class SocketInfo
    {
        public string IpAddress { get; set; }
        public int Port { get; set; }

        public SocketInfo(string ipAddress, int port)
        {
            IpAddress = ipAddress;
            Port = port;
        }

        public SocketInfo(){ }
    }
}

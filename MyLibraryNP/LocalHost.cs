using MyLibraryNP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public static class LocalHost
    {
        public static string IpAddress {
            get => Dns.GetHostAddresses(Dns.GetHostName()).Where(address =>
                 address.AddressFamily == AddressFamily.InterNetwork).First().ToString();
        }
        public static int Port { get; set; } = 2021;

        public static IPEndPoint IPEndPoint { get => new IPEndPoint(IPAddress.Parse(IpAddress), Port); }
    }
}

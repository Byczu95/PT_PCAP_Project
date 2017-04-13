using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PT_UI_Design.Models
{
    class NetworkConnection
    {
        public NetworkInterface interfaceA;
        public NetworkInterface interfaceB;

        public NetworkConnection(NetworkInterface a,NetworkInterface b)
        {
            interfaceA = a;
            interfaceB = b;
        }
    }
}

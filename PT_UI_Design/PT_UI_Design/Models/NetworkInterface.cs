using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PT_MAPACKET.Models
{
    class NetworkInterface
    {
        public int pos { get; set; }
        public List<string> IPs;
        public List<string> connections;
        public string MAC { get; set; }
        
        public NetworkInterface(string mac, int index)
        {
            IPs = new List<string>();
            connections = new List<string>();
            MAC = mac;
            pos = index;
        }

        public void AddNewIPAdress(string mac, string ip)
        {
            if(mac == this.MAC)
            {
                if (IPs.Contains(ip)) { }
                else IPs.Add(ip);
            }
        }
    }
}

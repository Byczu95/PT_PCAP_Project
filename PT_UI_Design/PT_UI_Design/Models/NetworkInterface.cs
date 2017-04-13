using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PT_UI_Design.Models
{
    class NetworkInterface
    {
        public List<string> IPs;
        public string MAC { get; set; }
        
        public NetworkInterface(string mac)
        {
            IPs = new List<string>();
            MAC = mac;
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

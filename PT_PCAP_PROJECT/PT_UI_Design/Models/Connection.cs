using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PT_MAPACKET.Models
{
    class Connection
    {
        public string adress;
        public Statistic stats;

        public Connection(string targetAdress)
        {
            adress = targetAdress;
            stats = new Statistic();
        }
    }
}

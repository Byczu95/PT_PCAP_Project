using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PT_MAPACKET.Models
{
    public class MyPacket
    {
        public int Id { get; set; }

        public string Time { get; set; }

        public string SourceMac { get; set; }

        public string DestMac { get; set; }

        public string SourceIP { get; set; }

        public string DestIP { get; set; }

    }
}

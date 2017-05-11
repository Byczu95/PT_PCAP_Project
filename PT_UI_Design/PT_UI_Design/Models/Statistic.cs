using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PT_MAPACKET.Models
{
    class Statistic
    {
        public DateTime firstPacketTime;
        public DateTime lastPacketTime;
        public int Bits;
        public double speed;
        public int packetCount;

        public Statistic()
        {
            firstPacketTime = new DateTime();
            lastPacketTime = new DateTime();
            Bits = 0;
            speed = 0;
        }
    }
}

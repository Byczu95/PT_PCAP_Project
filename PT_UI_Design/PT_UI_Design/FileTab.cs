using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PT_MAPACKET
{
    class FileTab
    {
        public string header { get; set; }

        public FileTab()
        {
            header = "Default tab";
        }

        public FileTab(string path)
        {
            header = Path.GetFileName(path);
            Console.WriteLine("ProjectTab: header = " + header);
        }
    }
}

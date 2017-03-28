using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SharpPcap;
using PacketDotNet;
using SharpPcap.LibPcap;
using NUnit.Framework;

namespace PT_UI_Design
{
    /// <summary>
    /// Interaction logic for PcapFileControl.xaml
    /// </summary>
    public partial class PcapFileControl : UserControl
    {
        private string _filePath;
        private static Label _label;
        public PcapFileControl(string filePath)
        {
            InitializeComponent();
            _label = label;
            _filePath = filePath;

            ICaptureDevice device;

            try
            {
                // Get an offline device
                device = new CaptureFileReaderDevice(_filePath);

                // Open the device
                device.Open();
            }
            catch (Exception e)
            {
                label.Content += "Caught exception when opening file" + e.ToString();
                return;
            }

            device.OnPacketArrival += new PacketArrivalEventHandler(device_OnPacketArrival);
            label.Content += "-- Capturing from " +  _filePath + '\n';

            device.Capture();

            device.Close();
            label.Content += "-- End of file reached.";
            
        }

        private static int packetIndex = 0;

        /// <summary>
        /// Prints the source and dest MAC addresses of each received Ethernet frame
        /// </summary>
        private static void device_OnPacketArrival(object sender, CaptureEventArgs e)
        {
            if (e.Packet.LinkLayerType == PacketDotNet.LinkLayers.Ethernet)
            {
                var packet = PacketDotNet.Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data);
                var ethernetPacket = (PacketDotNet.EthernetPacket)packet;

                _label.Content += packetIndex + "At: ";
                _label.Content += e.Packet.Timeval.Date.ToString() + " : ";
                _label.Content += e.Packet.Timeval.Date.Millisecond + " : MAC: ";
                _label.Content += ethernetPacket.SourceHwAddress + " -> MAC: ";
                _label.Content += ethernetPacket.DestinationHwAddress.ToString() + "\n";
                
                packetIndex++;
            }
            
        }

    }
}

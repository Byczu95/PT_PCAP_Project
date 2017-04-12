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
using PT_UI_Design.Models;
using NUnit.Framework;

namespace PT_UI_Design
{
    /// <summary>
    /// Interaction logic for PcapFileControl.xaml
    /// </summary>
    public partial class PcapFileControl : UserControl
    {
        private string _filePath;
        private static TextBox _textBox;
        private static ListView _listview;
        private static List<MyPacket> packets;

        public PcapFileControl(string filePath)
        {
            InitializeComponent();
            _textBox = textBox;
            _listview = listViewFileData;
            _filePath = filePath;

            filterComboBox.Items.Add("Adres źródłowy MAC");
            filterComboBox.Items.Add("Adres docelowy MAC");
            filterComboBox.Items.Add("Adres źródłowy IP");
            filterComboBox.Items.Add("Adres docelowy IP");

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
                textBox.Text += "Caught exception when opening file" + e.ToString();
                return;
            }

            device.OnPacketArrival += new PacketArrivalEventHandler(device_OnPacketArrival);
            textBox.Text += "-- Capturing from " +  _filePath + "\n\n";

            device.Capture();

            device.Close();
            
        }

        private static int packetIndex = 1;

        /// <summary>
        /// Prints the source and dest IP and MAC addresses of each received Ethernet frame
        /// </summary>
        private static void device_OnPacketArrival(object sender, CaptureEventArgs e)
        {
            if (e.Packet.LinkLayerType == PacketDotNet.LinkLayers.Ethernet)
            {
                var packet = PacketDotNet.Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data);

                if ((PacketDotNet.EthernetPacket)packet != null)
                {
                    var ethernetPacket = (PacketDotNet.EthernetPacket)packet;

                    if (IpPacket.GetEncapsulated(packet) != null)
                    {
                        var ipPacket = IpPacket.GetEncapsulated(packet);

                        _listview.Items.Add(new MyPacket { Id = packetIndex, 
                                                         Time = e.Packet.Timeval.Date.ToString(),
                                                         SourceIP = ipPacket.SourceAddress.MapToIPv4().ToString(), 
                                                         DestIP = ipPacket.DestinationAddress.MapToIPv4().ToString(),
                                                         SourceMac = ethernetPacket.SourceHwAddress.ToString(),
                                                         DestMac = ethernetPacket.DestinationHwAddress.ToString()
                                                            });
                    }
                }
                packetIndex++;
            }
        }

        public static List<MyPacket> getPacketsData()
        {
            packets = new List<MyPacket>();
            foreach(MyPacket p in _listview.Items)
            {
                packets.Add(p);
            }
            return packets;
        }
    }
}

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
using PT_MAPACKET.Models;
using NUnit.Framework;

namespace PT_MAPACKET
{
    /// <summary>
    /// Interaction logic for PcapFileControl.xaml
    /// </summary>
    public partial class PcapFileControl : UserControl, IGetPacket
    {
        private string _filePath;
        private static TextBox _textBox;
        private static ListView _listview;
        public static List<MyPacket> packets;
        public static List<MyPacket> viewPackets;
        private static string checksum;

        public PcapFileControl()
        {
            InitializeComponent();
            _textBox = textBox;
            _listview = listViewFileData;
            //_filePath = filePath;
            packets = new List<MyPacket>();
            viewPackets = new List<MyPacket>();

            filterComboBox.Items.Add("Adres źródłowy MAC");
            filterComboBox.Items.Add("Adres docelowy MAC");
            filterComboBox.Items.Add("Adres źródłowy IP");
            filterComboBox.Items.Add("Adres docelowy IP");

            ICaptureDevice device;
        }
        public PcapFileControl(string filePath)
        {
            InitializeComponent();
            _textBox = textBox;
            _listview = listViewFileData;
            _filePath = filePath;
            packets = new List<MyPacket>();
            viewPackets = new List<MyPacket>();

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
            textBox.Text += "-- Capturing from " + _filePath + "\n\n";

            device.Capture();

            device.Close();
            packetIndex = 1;

        }
        public PcapFileControl(List<MyPacket> Files)
        {
            InitializeComponent();
            _textBox = textBox;
            _listview = listViewFileData;
            //_filePath = filePath;
            int packetIndex = 1;
            packets = new List<MyPacket>();
            viewPackets = new List<MyPacket>();

            filterComboBox.Items.Add("Adres źródłowy MAC");
            filterComboBox.Items.Add("Adres docelowy MAC");
            filterComboBox.Items.Add("Adres źródłowy IP");
            filterComboBox.Items.Add("Adres docelowy IP");

            ICaptureDevice device;

            textBox.Text = "Merged file";


            foreach (MyPacket myPacket in Files)
            {

                _listview.Items.Add(new MyPacket
                {
                    Id = packetIndex,
                    Time = myPacket.Time,
                    SourceIP = myPacket.SourceIP,
                    DestIP = myPacket.DestIP,
                    SourceMac = myPacket.SourceMac,
                    DestMac = myPacket.DestMac
                });

                packets.Add(new MyPacket
                {
                    Id = packetIndex,
                    Time = myPacket.Time,
                    SourceIP = myPacket.SourceIP,
                    DestIP = myPacket.DestIP,
                    SourceMac = myPacket.SourceMac,
                    DestMac = myPacket.DestMac
                });

                packetIndex++;
            }
        }

        private static int packetIndex = 1;

        /// <summary>
        /// Prints the source and dest IP and MAC addresses of each received Ethernet frame
        /// </summary>
        private static void device_OnPacketArrival(object sender, CaptureEventArgs e)
        {
            checksum = "";
            if (e.Packet.LinkLayerType == PacketDotNet.LinkLayers.Ethernet)
            {
                var packet = PacketDotNet.Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data);

                if ((PacketDotNet.EthernetPacket)packet != null)
                {
                    var ethernetPacket = (PacketDotNet.EthernetPacket)packet;
                    if (TcpPacket.GetEncapsulated(packet) != null)
                    {
                        var tcp = TcpPacket.GetEncapsulated(packet);
                        checksum = tcp.Checksum.ToString();
                    }
                    else if (UdpPacket.GetEncapsulated(packet) != null)
                    {
                        var udp = UdpPacket.GetEncapsulated(packet);
                        checksum = udp.Checksum.ToString();
                    }
                    if (IpPacket.GetEncapsulated(packet) != null)
                    {
                        var ipPacket = IpPacket.GetEncapsulated(packet);

                        _listview.Items.Add(new MyPacket
                        {
                            Id = packetIndex,
                            Time = e.Packet.Timeval.Date.ToString() + "." + e.Packet.Timeval.MicroSeconds.ToString(),
                            SourceIP = ipPacket.SourceAddress.MapToIPv4().ToString(),
                            DestIP = ipPacket.DestinationAddress.MapToIPv4().ToString(),
                            SourceMac = ethernetPacket.SourceHwAddress.ToString(),
                            DestMac = ethernetPacket.DestinationHwAddress.ToString(),
                            Checksum = checksum
                        });
                        packets.Add(new MyPacket
                        {
                            Id = packetIndex,
                            Time = e.Packet.Timeval.Date.ToString(),
                            SourceIP = ipPacket.SourceAddress.MapToIPv4().ToString(),
                            DestIP = ipPacket.DestinationAddress.MapToIPv4().ToString(),
                            SourceMac = ethernetPacket.SourceHwAddress.ToString(),
                            DestMac = ethernetPacket.DestinationHwAddress.ToString(),
                            Checksum = checksum
                        });
                        packetIndex++;
                    }
                }

            }
        }

        public List<MyPacket> getPacketsData()
        {
            return packets;
        }

        public List<MyPacket> getPackets()
        {
            return packets;
        }

        private bool packetIsGoodForFilter(MyPacket myP)
        {
            //tu tzeba czyscic filterTextBox.Text i bd dzialac

            switch (filterComboBox.SelectedValue.ToString())
            {
                case "Adres źródłowy MAC":
                    if (myP.SourceMac == filterTextBox.Text)
                        return true;
                    else
                        return false;

                case "Adres docelowy MAC":
                    if (myP.DestMac == filterTextBox.Text)
                        return true;
                    else
                        return false;

                case "Adres źródłowy IP":
                    if (myP.SourceIP == filterTextBox.Text)
                        return true;
                    else
                        return false;

                case "Adres docelowy IP":
                    if (myP.DestIP == filterTextBox.Text)
                        return true;
                    else
                        return false;
                default:
                    return false;
            }
        }



        private void setListview(List<MyPacket> myList)
        {
            foreach (MyPacket elem in myList)
                _listview.Items.Add(elem);
        }

        private void filterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (filterComboBox.SelectedValue.ToString() == "Pokaż wszystko")
            {
                _listview.Items.Clear();
                filterTextBox.IsEnabled = false;
                setListview(packets);
            }
            else
                filterTextBox.IsEnabled = true;
        }


        private void filterButton_Click(object sender, RoutedEventArgs e)
        {
            _listview.Items.Clear();
            viewPackets.Clear();

            if (filterComboBox.SelectedValue.ToString() == "Pokaż wszystko" || filterTextBox.Text == "")
            {
                setListview(packets);
                return;
            }

            foreach (MyPacket elem in packets)
            {
                if (packetIsGoodForFilter(elem))
                {
                    viewPackets.Add(elem);
                    _listview.Items.Add(elem);
                }
            }
        }

        private void filterTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    filterButton_Click(sender, e);
                    break;
                default:

                    break;
            }
        }
    }
    public interface IGetPacket
    {
        List<MyPacket> getPacketsData();
    }
}

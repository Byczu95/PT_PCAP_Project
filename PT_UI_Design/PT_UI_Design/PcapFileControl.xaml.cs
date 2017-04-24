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
        private static List<MyPacket> allPackets; //wszystkie wczytane pakiety
        private static List<MyPacket> viewPackets; //pakiety na liscie prezentowanej uzytkownikowi

        public PcapFileControl(string filePath)
        {
            InitializeComponent();
            _textBox = textBox;
            _listview = listViewFileData;
            _filePath = filePath;
            allPackets = new List<MyPacket>();
            viewPackets = new List<MyPacket>();

            filterComboBox.Items.Add("Pokaż wszystko");
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

            set_listview(allPackets);
        }

        private static int _packetIndex = 1;

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

                        allPackets.Add(new MyPacket
                        {
                            Id = _packetIndex,
                            Time = e.Packet.Timeval.Date.ToString(),
                            SourceIP = ipPacket.SourceAddress.MapToIPv4().ToString(),
                            DestIP = ipPacket.DestinationAddress.MapToIPv4().ToString(),
                            SourceMac = ethernetPacket.SourceHwAddress.ToString(),
                            DestMac = ethernetPacket.DestinationHwAddress.ToString()
                        });
                    }
                }
                _packetIndex++;
            }
        }

        public List<MyPacket> getPacketsData()
        {
            //usuwanie duplikatów itd.
            return allPackets;
        }

        private bool packetIsGoodForFilter(MyPacket myP)
        {
            //tu tzeba czyscic filterTextBox.Text i bd dzialac
            string cleanFilterText = filterTextBox.Text;

                switch (filterComboBox.SelectedValue.ToString())
                {
                    case "Adres źródłowy MAC":
                        if (myP.SourceMac == cleanFilterText)
                            return true;
                        else
                            return false;

                    case "Adres docelowy MAC":
                        if (myP.DestMac == cleanFilterText)
                            return true;
                        else
                            return false;

                    case "Adres źródłowy IP":
                        if (myP.SourceIP == cleanFilterText)
                            return true;
                        else
                            return false;

                    case "Adres docelowy IP":
                        if (myP.SourceMac == cleanFilterText)
                            return true;
                        else
                            return false;
                    default:
                        return false; //packet is not OK
                }      
        }

        private void set_listview(List<MyPacket> myList)
        {
            foreach(MyPacket elem in myList)
                _listview.Items.Add(elem);
        }

        private void filterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (filterComboBox.SelectedValue.ToString() == "Pokaż wszystko")
            {
                _listview.Items.Clear();
                filterTextBox.IsEnabled = false;
                set_listview(allPackets);
            }
            else
                filterTextBox.IsEnabled = true;
        }

        private void filterButton_Click(object sender, RoutedEventArgs e)
        {
            _listview.Items.Clear();
            viewPackets.Clear();

            if (filterComboBox.SelectedValue.ToString() == "Pokaż wszystko")
            {
                set_listview(allPackets);
                return;
            }

            foreach (MyPacket elem in allPackets)
                if (packetIsGoodForFilter(elem))
                    viewPackets.Add(elem);

            set_listview(viewPackets);
        }
    }

    public interface IGetPacket
    {
        List<MyPacket> getPacketsData();
    }
}

using PacketDotNet;
using PT_MAPACKET.Models;
using SharpPcap;
using SharpPcap.LibPcap;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
        private static GridViewColumnHeader _lastHeaderClicked = null;
        private static ListSortDirection _lastDirection = ListSortDirection.Ascending;

        public PcapFileControl()
        {
            InitializeComponent();
            _textBox = textBox;
            _listview = listViewFileData;
            //_filePath = filePath;
            packets = new List<MyPacket>();
            viewPackets = new List<MyPacket>();

            filterComboBox.Items.Add("Adres źródłowy IP");
            filterComboBox.Items.Add("Adres docelowy IP");
            filterComboBox.Items.Add("Adres źródłowy MAC");
            filterComboBox.Items.Add("Adres docelowy MAC");

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

            filterComboBox.Items.Add("Adres źródłowy IP");
            filterComboBox.Items.Add("Adres docelowy IP");
            filterComboBox.Items.Add("Adres źródłowy MAC");
            filterComboBox.Items.Add("Adres docelowy MAC");

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

            filterComboBox.Items.Add("Adres źródłowy IP");
            filterComboBox.Items.Add("Adres docelowy IP");
            filterComboBox.Items.Add("Adres źródłowy MAC");
            filterComboBox.Items.Add("Adres docelowy MAC");

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
                    DestMac = myPacket.DestMac,
                    Checksum = myPacket.Checksum,
                    Length = myPacket.Length
                });

                packets.Add(new MyPacket
                {
                    Id = packetIndex,
                    Time = myPacket.Time,
                    SourceIP = myPacket.SourceIP,
                    DestIP = myPacket.DestIP,
                    SourceMac = myPacket.SourceMac,
                    DestMac = myPacket.DestMac,
                    Checksum = myPacket.Checksum,
                    Length = myPacket.Length
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
            TcpPacket tcp;
            UdpPacket udp;
            if (e.Packet.LinkLayerType == PacketDotNet.LinkLayers.Ethernet)
            {
                var packet = PacketDotNet.Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data);

                if ((PacketDotNet.EthernetPacket)packet != null)
                {
                    var ethernetPacket = (PacketDotNet.EthernetPacket)packet;
                    if (TcpPacket.GetEncapsulated(packet) != null)
                    {
                        tcp = TcpPacket.GetEncapsulated(packet);
                        checksum = tcp.Checksum.ToString();
                    }
                    else if (UdpPacket.GetEncapsulated(packet) != null)
                    {
                        udp = UdpPacket.GetEncapsulated(packet);
                        checksum = udp.Checksum.ToString();
                    }

                    if (IpPacket.GetEncapsulated(packet) != null)
                    {
                        var ipPacket = IpPacket.GetEncapsulated(packet);

                        _listview.Items.Add(new MyPacket
                        {
                            Id = packetIndex,
                            Time = e.Packet.Timeval.Date.ToString() + "." + e.Packet.Timeval.MicroSeconds.ToString(),
                            SourceIP = ipPacket.SourceAddress.ToString(),
                            DestIP = ipPacket.DestinationAddress.ToString(),
                            SourceMac = ethernetPacket.SourceHwAddress.ToString(),
                            DestMac = ethernetPacket.DestinationHwAddress.ToString(),
                            Checksum = checksum,
                            Length = packet.Bytes.Length
                        });
                        packets.Add(new MyPacket
                        {
                            Id = packetIndex,
                            Time = e.Packet.Timeval.Date.ToString(),
                            SourceIP = ipPacket.SourceAddress.MapToIPv4().ToString(),
                            DestIP = ipPacket.DestinationAddress.MapToIPv4().ToString(),
                            SourceMac = ethernetPacket.SourceHwAddress.ToString(),
                            DestMac = ethernetPacket.DestinationHwAddress.ToString(),
                            Checksum = checksum,
                            Length = packet.Bytes.Length
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


        void GridViewColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader headerClicked = e.OriginalSource as GridViewColumnHeader;
            ListSortDirection direction;

            if (headerClicked != null)
            {
                if (headerClicked.Role != GridViewColumnHeaderRole.Padding)
                {
                    if (headerClicked != _lastHeaderClicked)
                    {
                        direction = ListSortDirection.Ascending;
                    }
                    else
                    {
                        if (_lastDirection == ListSortDirection.Ascending)
                        {
                            direction = ListSortDirection.Descending;
                        }
                        else
                        {
                            direction = ListSortDirection.Ascending;
                        }
                    }

                    string header = headerClicked.Column.Header as string;
                    Sort(header, direction);

                    if (direction == ListSortDirection.Ascending)
                    {
                        headerClicked.Column.HeaderTemplate =
                          Resources["HeaderTemplateArrowUp"] as DataTemplate;
                    }
                    else
                    {
                        headerClicked.Column.HeaderTemplate =
                          Resources["HeaderTemplateArrowDown"] as DataTemplate;
                    }

                    // Remove arrow from previously sorted header  
                    if (_lastHeaderClicked != null && _lastHeaderClicked != headerClicked)
                    {
                        _lastHeaderClicked.Column.HeaderTemplate = null;
                    }

                    _lastHeaderClicked = headerClicked;
                    _lastDirection = direction;
                }
            }
        }

        private void Sort(string sortBy, ListSortDirection direction)
        {
            ICollectionView dataView =
              System.Windows.Data.CollectionViewSource.GetDefaultView(_listview.Items);

            dataView.SortDescriptions.Clear();
            SortDescription sd = new SortDescription(sortBy, direction);
            dataView.SortDescriptions.Add(sd);
            dataView.Refresh();
            _listview.Items.Refresh();
        }
    }
    public interface IGetPacket
    {
        List<MyPacket> getPacketsData();
    }
}
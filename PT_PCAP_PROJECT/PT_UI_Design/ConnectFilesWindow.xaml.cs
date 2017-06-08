using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;
using PT_MAPACKET.Models;

namespace PT_MAPACKET
{
    /// <summary>
    /// Interaction logic for ConnectFilesWindow.xaml
    /// </summary>
    public partial class ConnectFilesWindow : Window
    {
        private List<TabItem> _tabItems;

        public ConnectFilesWindow(List<TabItem> tabItems)
        {
            InitializeComponent();
            this._tabItems = tabItems;

            foreach (TabItem elem in _tabItems)
            {
                if (IsAPcapFile(elem.Header.ToString()) && _tabItems.IndexOf(elem) != 0)
                {
                    ListViewCFW.Items.Add(elem.Header.ToString());
                }
            }
        }

        private bool IsAPcapFile(string text)
        {
            if (text.Contains(".pcap")) return true;
            return false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!ListViewCFW.Items.IsEmpty)
            {
                MessageBox.Show("Wybrane pliki zostały połączone");
                ConnectFiles();
            }
            else
            {
                MessageBox.Show("Dodaj pliki, które chcesz połączyć");
            }
            this.Close();
        }

        private bool ConnectFiles()
        {
            PcapFileControl pcapWindow = new PcapFileControl();
            List<MyPacket> packets = new List<MyPacket>();
            int i = 0;
            bool isDuplicate = false;
            foreach (TabItem elem in _tabItems)
            {
                if (ListViewCFW.Items.Contains(elem.Header))
                {
                    if (IsAPcapFile(elem.Header.ToString()))
                    {
                        pcapWindow = (PcapFileControl)elem.Content;
                        foreach (MyPacket p in pcapWindow.listViewFileData.Items)
                        {
                            if (this.deleteDuplicate.IsChecked == true)
                            {
                                if (i > 1) // tu powinno być zero, ale licznik przeskoczy do 1 przez nowo utworzony merged.pcap
                                {
                                    if (packets.Count > 0)
                                    {
                                        foreach (MyPacket listPacket in packets)
                                        {                                                                                          
                                            if ((p.SourceIP.Equals(listPacket.DestIP) && p.Checksum.Equals(listPacket.Checksum)) || !arePacketsTheSame(p, listPacket)) //sprawdzam czy dany pakiet jest duplikatem lub czy ma już swój duplikat na liście
                                            {
                                                isDuplicate = true;
                                            }
                                        }
                                        if (isDuplicate == false) //dodaję pakiet jeśli jest unikalny
                                        {
                                            packets.Add(p);
                                        }

                                    } //jeśli na liście nie ma obiektów
                                    else
                                    {
                                        packets.Add(p);
                                    }
                                }
                                else  //uzupełnij listę pakietami z pierwszego pliku
                                {
                                    packets.Add(p);
                                }
                            }
                            else  // gdy checkbox nie jest zaznaczony
                            {
                                packets.Add(p);
                            }
                        }
                        i++;
                    }
                }
            }

            PcapFileControl pfc = new PcapFileControl(packets);
            _tabItems[0].Content = pfc;

            return true;
        }

        private bool arePacketsTheSame(MyPacket a, MyPacket b)
        {
            if (a.SourceIP.Equals(b.SourceIP) && a.DestIP.Equals(b.DestIP) && a.Checksum.Equals(b.Checksum) && a.Time.Equals(b.Time) && a.Length.Equals(b.Length))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

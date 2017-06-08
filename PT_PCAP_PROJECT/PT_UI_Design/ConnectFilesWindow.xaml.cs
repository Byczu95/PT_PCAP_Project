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
        private List<string> _selectedItems;

        public ConnectFilesWindow(List<TabItem> tabItems)
        {
            InitializeComponent();
            this._tabItems = tabItems;
            InitList();

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
                ConnectFiles();
                MessageBox.Show("Wybrane pliki zostały połączone");
            }
            else
            {
                MessageBox.Show("Dodaj pliki, które chcesz połączyć");
            }
            this.Close();
        }

        private void InitList()
        {
            foreach (TabItem elem in _tabItems)
            {
                if (IsAPcapFile(elem.Header.ToString()) && _tabItems.IndexOf(elem) != 0)
                {
                    ListViewCFW.Items.Add(elem.Header.ToString());
                }
            }
        }

        private void addSelectedItemsToList()
        {
            _selectedItems = new List<string>();
            foreach (String elem in ListViewCFW.SelectedItems)
            {
                if (IsAPcapFile(elem)  /* && elem.is IsSelected == true*/)
                {
                    _selectedItems.Add(elem);
                }
            }
        }
        private bool ConnectFiles()
        {
            addSelectedItemsToList();

            List<MyPacket> packets = new List<MyPacket>();
            int i = 0;
            bool isDuplicate = false;
            foreach (TabItem elem in _tabItems)
            {
                if (ListViewCFW.Items.Contains(elem.Header))
                {
                    if (_selectedItems.Contains(elem.Header))
                    {
                        if (IsAPcapFile(elem.Header.ToString()))
                        {
                            PcapFileControl pcapWindow = new PcapFileControl();
                            pcapWindow = (PcapFileControl)elem.Content;

                            foreach (MyPacket p in pcapWindow.listViewFileData.Items)
                            {
                                if (this.deleteDuplicate.IsChecked == true)
                                {
                                    if (i > 0) // Zawsze dodaję odaję wszystkie pakiety z pierwszego pliku. Przejdź jeśli index pliku jest większy niż 0
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

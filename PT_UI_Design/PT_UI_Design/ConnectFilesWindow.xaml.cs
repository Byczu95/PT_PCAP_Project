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

            foreach(TabItem elem in _tabItems)
            {
                if (IsAPcapFile(elem.Header.ToString()))
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

            foreach (TabItem elem in _tabItems)
            {
                if (ListViewCFW.Items.Contains(elem.Header))
                {
                    if (IsAPcapFile(elem.Header.ToString()))
                    {
                        pcapWindow = (PcapFileControl)elem.Content;
                        foreach(MyPacket p in pcapWindow.listViewFileData.Items)
                        {
                            packets.Add(p);
                        }
                    }
                }
            }
            
            PcapFileControl pfc = new PcapFileControl(packets);
            _tabItems[0].Content = pfc;

            return true; 
        }
    }

}

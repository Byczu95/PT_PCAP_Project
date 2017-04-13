using PT_UI_Design.Models;
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

namespace PT_UI_Design
{
    /// <summary>
    /// Interaction logic for ConnectFilesWindow.xaml
    /// </summary>
    public partial class ConnectFilesWindow : Window
    {
        private List<TabItem> _tabItems;

        public ConnectFilesWindow(List<TabItem> _tabItems)
        {
            InitializeComponent();
            this._tabItems = _tabItems;

            foreach(TabItem elem in _tabItems)
            {
                if (IsAPcapFile(elem.Header.ToString()))
                {
                    ListBoxCFW.Items.Add(elem.Header.ToString());
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
            if (!ListBoxCFW.Items.IsEmpty)
            {
                MessageBox.Show("Wybrane pliki zostały połączone");
                ConnectFiles();
            }
            else
            {
                MessageBox.Show("Dodaj pliki, które chcesz połączyć");
            }
            //dodanie nowej karty do glownego pliku
            this.Close();
        }

        private bool ConnectFiles()
        {
            //Łączenie plików
            return true; //jezeli sie udalo
        }
    }

}
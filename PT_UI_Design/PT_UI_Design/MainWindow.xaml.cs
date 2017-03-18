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
using System.IO;
using Microsoft.Win32;

namespace PT_UI_Design
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<TabItem> _tabItems;
        private TabItem _tabAdd;

        static int a = 3;

        private string ExtractFileName(string fullName)
        {
            string[] split = fullName.Split('\\');
            return split[split.Length-1];
        }

        public MainWindow()
        {
            InitializeComponent();

            _tabItems = new List<TabItem>();
            //_tabAdd = new TabItem();
        }

        private void File_Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "PCAPNG Files (*.pcapng)|*.pcapng";
            openFileDialog.Title = "Please select an file to open.";
            if (openFileDialog.ShowDialog() == true)
            {
                //TODO wczytywanie pliku lub plików
                foreach (String s in openFileDialog.FileNames)
                {
                    _tabAdd = new TabItem();
                    _tabAdd.Header = ExtractFileName(s);
                    if (_tabItems.Count == 0) _tabItems.Add(_tabAdd);
                    else _tabItems.Insert(_tabItems.Count - 1, _tabAdd);
                }

                tabControl.DataContext = null;
                tabControl.DataContext = _tabItems;

                tabControl.SelectedIndex = 0;
                tabControl.Items.Refresh();
            }
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void Analize_Graph_Click(object sender, RoutedEventArgs e)
        {
            WindowGraph winG = new WindowGraph();
            winG.Show();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //TODO zamykanie wszystkich okien
        }
    }
}

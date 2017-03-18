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

        public MainWindow()
        {
            InitializeComponent();

            _tabItems = new List<TabItem>();
            _tabAdd = new TabItem();

        }

        private void File_Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if(openFileDialog.ShowDialog() == true)
            {
                //TODO wczytywanie pliku lub plików
                foreach (String s in openFileDialog.FileNames)
                {
                    _tabAdd.Header = s;
                    _tabItems.Add(_tabAdd);
                    _tabItems.Insert(_tabItems.Count - 1, _tabAdd);
                }

                tabControl.DataContext = _tabItems; //TODO dodawanie nowych a nie nadpisywanie
                tabControl.SelectedIndex = 0;
            }
        }
    }
}

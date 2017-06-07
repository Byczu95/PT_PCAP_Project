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

namespace PT_MAPACKET
{
    /// <summary>
    /// Logika interakcji dla klasy Credits.xaml
    /// </summary>
    public partial class Credits : UserControl
    {
        public Credits()
        {
            InitializeComponent();
            try
            {
                image1.Source = new BitmapImage(new Uri("https://scontent-waw1-1.xx.fbcdn.net/v/t34.0-12/18718608_120332001035854161_515354885_n.png?oh=f25e9538d6c30a69ebcd7f6eee203ad7&oe=592937E9"));
            }
            catch(Exception e)
            {

            }
        }
    }
}

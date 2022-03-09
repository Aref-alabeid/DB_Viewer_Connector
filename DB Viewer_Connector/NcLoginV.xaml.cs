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
using System.Windows.Shapes;

namespace DB_Viewer_Connector
{
    /// <summary>
    /// Interaction logic for NcLoginV.xaml
    /// </summary>
    public partial class NcLoginV : Window
    {
        public NcLoginV()
        {
            InitializeComponent();
            NcLoginVM ncLoginVM = new NcLoginVM();
            this.DataContext = ncLoginVM;

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for NcSQLAbfrageAusfuehrenVM.xaml
    /// </summary>
    public partial class NcSQLAbfrageAusfuehrenV : Window
    {
        public NcSQLAbfrageAusfuehrenV()
        {
            InitializeComponent();

            //IsVisibleChanged += NcSQLAbfrageAusfuehrenV_IsVisibleChanged;
        }

        private void NcSQLAbfrageAusfuehrenV_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible == false)
                return;

            INotifyPropertyChanged viewModel = (INotifyPropertyChanged)this.DataContext;
            viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (!e.PropertyName.Equals("DBData"))
                return;

            dBData.Items.Refresh();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
            System.Windows.Application.Current.MainWindow.Close();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            System.Windows.Application.Current.MainWindow.Close();
        }
    }
}

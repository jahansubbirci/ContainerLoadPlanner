using Microsoft.Win32;
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

namespace ContainerLoadPlanner.Views
{
    /// <summary>
    /// Interaction logic for TescoView.xaml
    /// </summary>
    public partial class TescoView : UserControl
    {
        public TescoView()
        {
            InitializeComponent();
        }

        private void BrowseCfsReportButton_Click(object sender, RoutedEventArgs e)
        {
            BrowseFile(CfsReportFile);

        }

        private void BrowseMyMaerskReport_Click(object sender, RoutedEventArgs e)
        {
            BrowseFile(MyMaerskReport);
        }
        private void BrowseFile(TextBox target)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel|*.xls;*.xlsx";
            if ((bool)openFileDialog.ShowDialog())
            {
                target.Text = openFileDialog.FileName;
            }
        }
    }
}

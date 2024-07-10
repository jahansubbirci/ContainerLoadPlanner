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
    /// Interaction logic for ContainerView.xaml
    /// </summary>
    public partial class ContainerView : UserControl
    {
        public ContainerView()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            dataGrid.Visibility = Visibility.Visible;
        }

        private void SummaryButton_Click(object sender, RoutedEventArgs e)
        {
            dataGrid.Visibility = Visibility.Collapsed;
            SummaryButton.Visibility = Visibility.Collapsed;
            DetailsButton.Visibility = Visibility.Visible;
        }

        private void DetailsButton_Click(object sender, RoutedEventArgs e)
        {
            
            dataGrid.Visibility = Visibility.Visible;
            DetailsButton.Visibility = Visibility.Collapsed;
            SummaryButton.Visibility = Visibility.Visible;
            
        }

        private void dataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            e.Column.Width = DataGridLength.Auto;
        }
    }
}

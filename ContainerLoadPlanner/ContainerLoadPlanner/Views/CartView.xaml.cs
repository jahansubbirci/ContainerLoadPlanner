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
    /// Interaction logic for CartView.xaml
    /// </summary>
    public partial class CartView : Window
    {
        public CartView()
        {
            InitializeComponent();
            this.Title = "Container Cart";
            this.WindowState= System.Windows.WindowState.Maximized;
            this.UpdateLayout();
        }

        private void SelectedContainers_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ContainerScrollViewer.ScrollToVerticalOffset(ContainerScrollViewer.VerticalOffset - e.Delta / 3);
            e.Handled = true;
        }

        private void ScrollViewer_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {

        }
    }
}

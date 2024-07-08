using SharedEntities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace ContainerLoadPlanner.Utilities
{
    public class CfsToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CFS cfs)
            {
                switch (cfs)
                {
                    case CFS.ISATL: return Brushes.DarkSeaGreen;
                    case CFS.SAPL: return Brushes.SteelBlue;
                    case CFS.VERTEX: return Brushes.Red;
                    case CFS.KDS: return Brushes.Green;
                }
            }
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

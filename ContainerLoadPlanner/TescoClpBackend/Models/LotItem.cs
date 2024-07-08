using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TescoClpBackend.ClpLogics;

namespace TescoClpBackend.Models
{

    public class LotItem : ICloneable
    {
        public IGrouping<int, ClpItem> Item { get; set; }
        public double TotalCbm { get; set; }
        public int Priority { get; set; }
        public LotItem(IGrouping<int, ClpItem> items)
        {
            Item = items;
            TotalCbm = items.Sum(a => a.CfsReportItem.Cbm);
        }
        public object Clone()
        {
            return this;// throw new NotImplementedException();
        }

        //internal object Clone()
        //{
        //    return this;
        //}



    }
}

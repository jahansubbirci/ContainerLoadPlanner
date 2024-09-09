using ClpEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TescoClpBackend.ClpLogics;

namespace TescoClpBackend.Models
{

    public class LotItem :ContainerItem, ICloneable
    {
        public IGrouping<int, ClpItem> Item { get; set; }
        public double TotalCbm { get; set; }
        public int Priority { get; set; }
        public LotItem(IGrouping<int, ClpItem> items)
        {
            Item = items;
            TotalCbm = items.Sum(a => a.Cbm);
        }
        public LotItem()
        {
        }
        public object Clone()
        {
            return this.MemberwiseClone();// throw new NotImplementedException();
        }

        //internal object Clone()
        //{
        //    return this;
        //}



    }
}

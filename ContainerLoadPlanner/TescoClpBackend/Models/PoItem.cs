using ClpEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TescoClpBackend.ClpLogics;

namespace TescoClpBackend.Models
{
    public class PoItem:ContainerItem,ICloneable
    {
        public IGrouping<string, ClpItem> Items { get; set; }
        public double TotalCbm { get; set; }
        public int Priority { get; set; }
        public PoItem(IGrouping<string, ClpItem> items)
        {

            Items = items;
            TotalCbm = items.Sum(a => a.Cbm);
        }
        public PoItem()
        {
        }
        public object Clone()
        {
            return this.MemberwiseClone();// throw new NotImplementedException();
        }
    }
}

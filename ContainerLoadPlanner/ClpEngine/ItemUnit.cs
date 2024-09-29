using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClpEngine
{
    public class ItemUnit<T,U> : ICloneable,IGroupBy<T,U> where T : ContainerItem
    {
        public IGrouping<object,T> Items { get; set; }
        public double TotalCbm { get;private set; }
        public ItemUnit<T,U> ChildUnit { get; set; }

        public ItemUnit(IGrouping<object,T>items)
        {
            this.Items= items;
            this.TotalCbm = items.Sum(a=>a.Cbm);
        }
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public IGrouping<object, T> Group(U property)
        {
            throw new NotImplementedException();
        }
    }

    internal interface IGroupBy<T,U>
    {
       IGrouping<object,T>Group(U property);
    }
}

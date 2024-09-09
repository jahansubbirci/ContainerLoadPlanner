using ClpEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedEntities
{
    public class Container<T> where T : ContainerItem
    {
        public string Label { get; set; }
       // public double DefaultCapacity { get; set; }
        public double MaxCapacity { get; set; }
        public double MinAcceptableVolume { get; set; }
        public List<T> Items { get; set; }
        public Guid ContainerId { get; set; }
        public double RemainingCapacity => MaxCapacity - UsedCbm;
        public double UsedCbm { get; set; }
        public double UnitCost { get; set; }

        public Container(string label) {
            Label = label;
            ContainerId= Guid.NewGuid();
            //RemainingCapacity = MaxCapacity;
            
            Items= new List<T>();
         
        }

        public Container(string label, double maxCapacity, double minAcceptableVolume)
        {
            Label = label;
            MaxCapacity = maxCapacity;
            MinAcceptableVolume = minAcceptableVolume;
            ContainerId = Guid.NewGuid();
            Items = new List<T>();
            UsedCbm = 0;
            //RemainingCapacity = maxCapacity;
        }

        public bool CanAddItem(T item)
        {
            return item.Cbm < RemainingCapacity;
        }
        public void AddItem(T item)
        {
            Items.Add(item);
            UsedCbm += item.Cbm;
        }

        public bool CanAddItems(IEnumerable<T> items)
        {
            return items.Sum(a => a.Cbm) < RemainingCapacity;
        }
        public void AddItems(IEnumerable<T> items)
        {
            var totalCbm = items.Sum(a => a.Cbm);
            Items.AddRange(items);
            UsedCbm += totalCbm;
         //   RemainingCapacity -= totalCbm;
        }

        public void RemoveItems(IEnumerable<T> items)
        {
            var totalCbm=items.Sum(a => a.Cbm);
            Items.RemoveAll(a=>items.Select(i=>i.ContainerItemId).Contains(a.ContainerItemId));
            //RemainingCapacity +=totalCbm;
            UsedCbm -= totalCbm;
        }

      
    }
}

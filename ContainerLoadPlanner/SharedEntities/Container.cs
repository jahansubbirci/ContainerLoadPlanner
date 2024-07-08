using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedEntities
{
    public class Container<T> where T : class
    {
        public string Label { get; set; }
       // public double DefaultCapacity { get; set; }
        public double MaxCapacity { get; set; }
        public double MinAccepatableVolume { get; set; }
        public List<T> Items { get; set; }
        public Guid ContainerId { get; set; }
        public double RemainingCapacity { get; set; }
        public double UnitCost { get; set; }

        public Container(string label) {
            Label = label;
            ContainerId= Guid.NewGuid();
            RemainingCapacity = MaxCapacity;
            
            Items= new List<T>();
         
        }
    }
}

using ÉxcelDataExchange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClpEngine
{
    public abstract class ContainerItem
    {
        [Visible(false)]
        public Guid ContainerItemId { get; set; }
        public int Pkgs { get;  set; }
        public double Cbm { get; set; }
        public double CWeight { get; set; }
        public string Destination { get; set; }
       
        public ContainerItem()
        {
            ContainerItemId = Guid.NewGuid();
        }

        public double UnitCbm => Pkgs > 0 ? Cbm / Pkgs : 0;

        // Method to split the item into smaller parts if CBM exceeds the threshold
       
   
   
    }
}

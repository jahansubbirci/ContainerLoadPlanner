using ÉxcelDataExchange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClpEngine
{
    public class ContainerItem
    {
        [Visible(false)]
        public Guid ContainerItemId { get; set; }
        public double Cbm { get; set; }
        public double CWeight { get; set; }
        public string Destination { get; set; }
        public ContainerItem()
        {
            ContainerItemId = Guid.NewGuid();
        }
    }
}

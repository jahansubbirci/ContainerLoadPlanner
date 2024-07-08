using SharedEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContainerLoadPlanner.Models
{
    public class CartItem<T> where T : class
    {
        public Container<T> Container  { get; set; }
        public ContainerSummary Summary { get; set; }
    }
  public  class ContainerSummary
    {
        public string ContainerLabel { get; set; }
        public double TotalCbm { get; set; }
        public double TotalWeight { get; set; }
        public int TotalQty { get; set; }
    }
}

using ClpEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TescoClpBackend.ClpLogics;

namespace TescoClpBackend.Models
{
    public class LOT : ItemUnit<ClpItem>
    {
        public LOT(IGrouping<object, ClpItem> items) : base(items)
        {
            
        }
        
    }
}

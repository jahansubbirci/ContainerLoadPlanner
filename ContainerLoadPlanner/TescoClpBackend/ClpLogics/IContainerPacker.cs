using SharedEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TescoClpBackend.Models;

namespace TescoClpBackend.ClpLogics
{
    public interface IContainerPacker
    {
        List<Container<ClpItem>> PackItems(List<LotItem> lotItems, double minCapacity, double maxCapacity);
    }
}

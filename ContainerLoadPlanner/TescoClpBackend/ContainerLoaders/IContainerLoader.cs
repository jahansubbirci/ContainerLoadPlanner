using SharedEntities;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TescoClpBackend.ClpLogics;
using TescoClpBackend.Models;

namespace TescoClpBackend.ContainerLoaders
{

    public interface IContainerLoader
    {
        List<Container<ClpItem>> Load(Combination combination, ref List<ClpItem> data);
        //  void LoadUntilCap(ref List<InvoiceItem> dcList, ref Container container, double previousCapacity);
    }
}


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
    public class FortyHCLoader : ContainerLoader, IContainerLoader
    {
        protected override Stack<Container<ClpItem>> InitiateContainers(Combination combination)
        {
            Stack<Container<ClpItem>> containers = new Stack<Container<ClpItem>>(combination.FortyHICount);
            for (int i = 0; i < combination.FortyHICount; i++)
            {
                Container<ClpItem> container = new Container<ClpItem>("40HC");


                if (combination.FortyHITolerance > 0)
                {
                    container.MaxCapacity = combination.FortyHITolerance + ContainerConstants.FORTY_HI_DEFAULT_CAPACITY;//ContainerConstants._40HIDefaultCapacity;
                }
                else
                {
                    var bufferCap = 1;
                    container.MaxCapacity = ContainerConstants.FORTY_HI_DEFAULT_CAPACITY+bufferCap;// ContainerConstants._40HIDefaultCapacity + 1;
                }

                container.RemainingCapacity = container.MaxCapacity;
                containers.Push(container);
            }
            return containers;
        }
    }
    public class FortySTDLoader : ContainerLoader, IContainerLoader
    {

        protected override Stack<Container<ClpItem>> InitiateContainers(Combination combination)
        {
            Stack<Container<ClpItem>> containers = new Stack<Container<ClpItem>>(combination.FortySTDCount);
            for (int i = 0; i < combination.FortySTDCount; i++)
            {
                Container<ClpItem> container = new Container<ClpItem>("40STD");

                if (combination.FortySTDTolerance > 0)
                {
                    container.MaxCapacity = combination.FortySTDTolerance + ContainerConstants.FORTY_STD_DEFAULT_CAPACITY;// ContainerConstants._40DRYDefaultCapacity; }
                }
                else
                {
                    //since you cannot utilize whole default capacity add 1 cbm to utilize default capacity
                    var bufferCap=1;
                    container.MaxCapacity = ContainerConstants.FORTY_STD_DEFAULT_CAPACITY+bufferCap;// ContainerConstants._40DRYDefaultCapacity + 1;
                }
                container.RemainingCapacity = container.MaxCapacity;
                containers.Push(container);
            }
            return containers;
        }




    }
    public class MixedLoader : IContainerLoader
    {
        private FortyHCLoader FortyHCLoader { get; set; }
        private FortySTDLoader FortySTDLoader { get; set; }
        public MixedLoader(FortyHCLoader fortyHCLoader,
            FortySTDLoader fortySTDLoader)
        {
            FortyHCLoader = fortyHCLoader;
            FortySTDLoader = fortySTDLoader;
        }

        public List<Container<ClpItem>> Load(Combination combination, ref List<ClpItem> data)
        {
            List<Container<ClpItem>> containers = new List<Container<ClpItem>>();

            containers.AddRange(this.FortyHCLoader.Load(combination, ref data));


            containers.AddRange(this.FortySTDLoader.Load(combination, ref data));
            return containers;
        }
    }
}

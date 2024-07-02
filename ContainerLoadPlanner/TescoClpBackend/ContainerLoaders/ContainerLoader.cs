using SharedEntities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TescoClpBackend.ClpLogics;
using TescoClpBackend.Models;

namespace TescoClpBackend.ContainerLoaders
{
    public abstract class ContainerLoader : IContainerLoader
    {
        public List<Container<ClpItem>> Load(Combination combination, ref List<ClpItem> data)
        {
            List<Container<ClpItem>> loadedContainers = new List<Container<ClpItem>>();

            var containers = InitiateContainers(combination);
            var containerCount = containers.Count;

            for (int i = 0; i < containerCount; i++)
            {
                Container<ClpItem> container = containers.Pop();

                double previousCapacity = container.RemainingCapacity;

                var lotGroup = data
                .GroupBy(a => a.CfsReportItem.Lot)
                .Select(lot => new LotItem(lot))

                //.OrderByDescending(a => a.TotalCbm)
                .ToList();

                //If Lot Contains one high priority PO, all PO should be high priority
                lotGroup.ForEach(a =>
                {
                    if (a.Item.Any(item => item.PoUploadReportItem.Priority == Int32.MaxValue))
                    {
                        a.Priority = Int32.MaxValue;
                    }
                });
                lotGroup = lotGroup
                    .OrderByDescending(a => a.Priority)
                    .ThenByDescending(a => a.TotalCbm)
                    .ToList();

                LoadUntilCap(ref lotGroup, ref container, previousCapacity,ref containers);

                data.RemoveAll(a => container.Items.Contains(a));
                if (container.RemainingCapacity < 0.5)
                {
                    //   loadedContainer.Add(container);
                    if (!loadedContainers.Contains(container))
                        loadedContainers.Add(container);

                    //   break;
                }
                if (!loadedContainers.Contains(container))

                    loadedContainers.Add(container);
                Debug.WriteLine($"Container:{container.ContainerId};-CBM:{container.Items.Sum(a => a.CfsReportItem.Cbm)}");
            }
            return loadedContainers;
        }


        public double LoadQn(ref List<LotItem> lotList, ref Container<ClpItem> container, ref Stack<Container<ClpItem>> containers)
        {
            //lotList.OrderByDescending(a=>a.TotalCbm).ToList();
            

            var capacity = container.RemainingCapacity;
           // lotList = lotList.Where(a => (capacity - a.TotalCbm) > 0).ToList();
            if (lotList.Count() > 0)
            {
             //   var viableLots = lotList.Where(a => (capacity - a.TotalCbm) >= 0);
                var closest = lotList
                   
                    .Aggregate((x, y)=>
                     (capacity - x.TotalCbm) >= 0
                    && (capacity - y.TotalCbm )>= 0
                    &&
                     (capacity - x.TotalCbm) <( capacity - y.TotalCbm)
                    ? x : y);
                var cloned = closest.Clone() as LotItem;
                if (container.Items.Sum(a => a.CfsReportItem.Cbm) + closest.TotalCbm
                    <= container.MaxCapacity)
                {
                    container.Items.AddRange(closest.Item);
                    lotList.Remove(closest);
                    container.RemainingCapacity -= closest.TotalCbm;
                }
                //if (closest.TotalCbm > container.MaxCapacity)
                //{
                //    //var newcontainer= containers.Pop();

                //    PickItemsSplitingLotToFillContainer(lotList, container, closest, cloned);

                //}
                else
                {
                    //SplitLot(container,ref closest);
                    //break;
                    //dcList.Remove(closest);
                    //LoadQn(ref dcList, ref  container);
                    //dcList.Add(cloned as CLPRow);
                }
            }
            return container.RemainingCapacity;
        }

        private void PickItemsSplitingLotToFillContainer(List<LotItem> lotList, Container<ClpItem> container, LotItem closest, LotItem cloned)
        {
            double sum = 0d;
            
            var cap = container.MaxCapacity;
            var itemsToTake = closest.Item.TakeWhile(i => (sum += i.CfsReportItem.Cbm) <= cap).ToList();

            // Add the selected items to the container
            container.Items.AddRange(itemsToTake);

            // Remove the selected items from the lot list
            lotList.Remove(closest);

            // Filter and group the remaining items from the cloned list
            var remainingItems = cloned.Item
                .Where(item => !itemsToTake.Contains(item))
                .GroupBy(g => g.CfsReportItem.Lot)
                .FirstOrDefault(a => a.Key == closest.Item.Key);

            // Create a new LotItem with the remaining items and add it to the lot list
            if (remainingItems != null)
            {
                var clo = new LotItem(items: remainingItems);
                lotList.Add(clo);
            }

            // Update the remaining capacity of the container
            container.RemainingCapacity -= itemsToTake.Sum(a => a.CfsReportItem.Cbm);
        }




        private void LoadUntilCap(ref List<LotItem> lotGroup, ref Container<ClpItem> container, double previousCapacity, ref Stack<Container<ClpItem>> containers)
        {
            
            while (container.RemainingCapacity > 0 && lotGroup.Count() > 0)
            {
                previousCapacity = container.RemainingCapacity;
                /*(dcList,container)=*/
                var capacity = LoadQn(ref lotGroup, ref container,ref containers);
                if (previousCapacity == capacity)
                {
                    break;
                }


            }
        }

        protected abstract Stack<Container<ClpItem>> InitiateContainers(Combination combination);

    }

}

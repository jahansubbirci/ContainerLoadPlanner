using LoggerService;
using NPOI.SS.Formula.Functions;
using SharedEntities;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using TescoClpBackend.Models;

namespace TescoClpBackend.ClpLogics
{
    public class ContainerPacker
    {
        private readonly ILoggerManager loggerManager;

        public ContainerPacker(ILoggerManager loggerManager)
        {
            this.loggerManager = loggerManager;
        }
        public List<Container<ClpItem>> PackItems(List<LotItem> lotItems, double minCapacity, double maxCapacity)
        {
            List<Container<ClpItem>> containers = new List<Container<ClpItem>>();
            loggerManager.LogInfo($"\t{lotItems.First().Item.First().Destination}\tTOTAL CBM:\t{lotItems.Sum(l => l.TotalCbm)}");
            // Sort LotItems by descending TotalCbm to try and fill larger items first
            var sortedItems = lotItems.OrderByDescending(l => l.TotalCbm).ToList();
            var singleLotContainers = LoadMoreThanContainerLots(ref sortedItems);

            foreach (var lotItem in sortedItems)
            {
                // Try to find an existing container that can fit the entire LotItem
                var suitableContainer = containers.FirstOrDefault(c => c.CanAddItems(lotItem.Item));

                if (suitableContainer == null)
                {
                    // If no suitable container is found, create a new one
                    suitableContainer = CreateNewContainer($"40HI", minCapacity, maxCapacity);
                    containers.Add(suitableContainer);
                }

                suitableContainer.AddItems(lotItem.Item);
            }

            // Perform redistribution to ensure all containers meet the minimum capacity requirement
            var underUtilizedContainers = containers.Where(a => a.UsedCbm < a.MinAcceptableVolume);
            if (underUtilizedContainers.Count() > 1)
            {
                RedistributeItems(containers);
            }
            else
            {


            }

            var containersToRemove = containers
                .Where(a => a.UsedCbm < a.MinAcceptableVolume)
                .ToList();

            containers.RemoveAll(a => containersToRemove.Select(c => c.ContainerId).Contains(a.ContainerId));

            containers.AddRange(singleLotContainers);
            containersToRemove.ForEach(a => LogLeftoverItems(a.Items));
            containersToRemove.ForEach(a => loggerManager.LogInfo($"\t{a.Items.First().CfsReportItem.Destination}\tLEFT OVER\t{a.UsedCbm}"));
            containers.ForEach(a => loggerManager.LogInfo($"\t{a.Items.First().CfsReportItem.Destination}\tUSED\t{a.UsedCbm}"));
            return containers;
        }
        private void LogLeftoverItems(List<ClpItem> items)
        {

            items.ForEach(a => loggerManager.LogWarn($"\tLeft over:\t{a.CfsReportItem.ToString()}"));
        }

        private List<Container<ClpItem>> LoadMoreThanContainerLots(ref List<LotItem> sortedItems)
        {
            List<Container<ClpItem>> containers = new List<Container<ClpItem>>();
            var moreThanContainerLots = sortedItems.Where(a => a.TotalCbm > ContainerConstants.FORTY_HI_DEFAULT_CAPACITY + ContainerConstants.FORTY_HI_TOLERANCE).ToList();
            foreach (var lotItem in moreThanContainerLots)
            {
                double _40HICap = ContainerConstants.FORTY_HI_DEFAULT_CAPACITY + ContainerConstants.FORTY_HI_TOLERANCE;
                var requiredContainer =(int)(lotItem.TotalCbm / _40HICap);
                var clone = lotItem.Clone() as LotItem;
                var loadedItems=new List<ClpItem>();
                for (int i = 0; i < requiredContainer; i++)
                {



                    var container = CreateNewContainer("40HI", ContainerConstants.FORTY_HI_MIN_ACCEPTABLE_VOLUME, (ContainerConstants.FORTY_HI_DEFAULT_CAPACITY + ContainerConstants.FORTY_HI_TOLERANCE));
                    var cap = container.MaxCapacity;
                    var items = lotItem.Item.ToList();

                    LoadUntilCap(ref items, ref container, container.RemainingCapacity);
                    containers.Add(container);
                    loadedItems.AddRange(container.Items);
                }
                sortedItems.Remove(lotItem);

                //var loadedItems=containers.Select(a=>a.Items).ToList().ForEach(a=>a.Select(i=>i)).ToList();
                var remainingItems = clone.Item
                    .Where(i=>!loadedItems.Contains(i)).ToList();
                    //.Where(i => !container.Items.Contains(i)).ToList();
                remainingItems.ForEach(a =>
                {
                    a.PoUploadReportItem.Priority = Int32.MaxValue;
                    a.CfsReportItem.Split = true;
                });
                var remainingItem = remainingItems.GroupBy(a => a.CfsReportItem.Lot)
                    .First(a => a.Key == clone.Item.Key);

                if (remainingItem != null)
                {

                    var l = new LotItem(remainingItem);
                    sortedItems.Insert(0, l);
                }
               // containers.Add(container);
            }
            return containers;
        }
        private void LoadUntilCap(ref List<ClpItem> lotGroup, ref Container<ClpItem> container, double previousCapacity)
        {

            while (container.RemainingCapacity > 0 && lotGroup.Count() > 0)
            {
                previousCapacity = container.RemainingCapacity;
                /*(dcList,container)=*/
                var capacity = LoadQn(ref lotGroup, ref container);
                if (previousCapacity == capacity)
                {
                    break;
                }


            }
        }
        private double LoadQn(ref List<ClpItem> itemGroup, ref Container<ClpItem> container)
        {
            var capacity = container.RemainingCapacity;
            var viables = itemGroup.Where(a => capacity - a.Cbm > 0).ToList();
            if (viables.Count() > 0)
            {
                var closest = viables
                    .Aggregate((x, y) => capacity - x.Cbm < capacity - y.Cbm ? x : y);
                if (container.CanAddItem(closest))
                {
                    closest.CfsReportItem.Split = true;
                    container.AddItem(closest);
                    itemGroup.Remove(closest);
                }
            }
            return container.RemainingCapacity;
        }
        private Container<ClpItem> CreateNewContainer(string label, double minCapacity, double maxCapacity)
        {
            return new Container<ClpItem>(label, maxCapacity, minCapacity);
        }

        private void RedistributeItems(List<Container<ClpItem>> containers)
        {
            var underfilledContainers = containers.Where(c => c.UsedCbm < c.MinAcceptableVolume).ToList();
            var overfilledContainers = containers.Where(c => c.UsedCbm > c.MinAcceptableVolume).OrderByDescending(c => c.UsedCbm).ToList();

            foreach (var underfilledContainer in underfilledContainers)
            {
                foreach (var overfilledContainer in overfilledContainers)
                {
                    if (underfilledContainer.UsedCbm >= underfilledContainer.MinAcceptableVolume)
                        break;

                    // Move items from overfilled to underfilled containers
                    var movableLotItems =
                        overfilledContainer.Items
                        .GroupBy(item => item.CfsReportItem.Lot)
                .Where(group => overfilledContainer.UsedCbm - group.Sum(a => a.Cbm) >= overfilledContainer.MinAcceptableVolume &&
                               underfilledContainer.UsedCbm + group.Sum(a => a.Cbm) <= underfilledContainer.MaxCapacity)
                .OrderByDescending(group => group.Sum(a => a.Cbm))
                .Select(lot =>new LotItem(lot))
                .ToList();

                    //    overfilledContainer.Items
                    //.GroupBy(item => item.CfsReportItem.Lot) // or some other meaningful property
                    //.Where(group => group.Sum(item => item.Cbm) + underfilledContainer.UsedCbm <= underfilledContainer.MaxCapacity)
                    //.OrderByDescending(group => group.Sum(item => item.Cbm))
                    //.Select(lot => new LotItem(lot))
                    //.ToList();

                    foreach (var lotItemToMove in movableLotItems)
                    {
                        if(underfilledContainer.UsedCbm>=underfilledContainer.MinAcceptableVolume)
                            break;

                        overfilledContainer.RemoveItems(lotItemToMove.Item);
                        overfilledContainer.UsedCbm -= lotItemToMove.Cbm;

                        if(underfilledContainer.CanAddItems(lotItemToMove.Item)){

                            underfilledContainer.AddItems(lotItemToMove.Item);
                            underfilledContainer.UsedCbm += lotItemToMove.TotalCbm;
                        }
                        if (overfilledContainer.UsedCbm <= overfilledContainer.MaxCapacity && underfilledContainer.UsedCbm >= underfilledContainer.MinAcceptableVolume)
                        {
                            break; // Move to the next underfilled container
                        }
                    }

                    //foreach (var lotItemGroup in movableLotItems)
                    //{
                    //    var lotItemsToMove = lotItemGroup.Item;

                    //    // Remove items from the overfilled container

                    //    overfilledContainer.RemoveItems(lotItemsToMove);
                    //    overfilledContainer.UsedCbm -= lotItemsToMove.Sum(a => a.Cbm);

                    //    // Add items to the underfilled container
                    //    if (underfilledContainer.CanAddItems(lotItemsToMove))
                    //    {
                    //        underfilledContainer.AddItems(lotItemsToMove);
                    //    }

                    //    if (underfilledContainer.UsedCbm >= underfilledContainer.MinAcceptableVolume)
                    //        break;
                    //}
                }
            }

            // Ensure no LotItem is split between containers
            foreach (var container in containers)
            {
                if (container.UsedCbm < container.MinAcceptableVolume)
                {
                    loggerManager.LogWarn($"Container {container.ContainerId} still underutilized. Used CBM:{container.UsedCbm}");
                    //throw new InvalidOperationException($"Container {container.Label} still does not meet the minimum capacity requirement after redistribution.");
                }
            }
        }



    }
}

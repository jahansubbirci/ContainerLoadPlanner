using LoggerService;
using SharedEntities;
using System.Collections.Generic;
using System.Linq;
using TescoClpBackend.Models;

namespace TescoClpBackend.ClpLogics
{
    public class ContainerPackerBase
    {
        protected ILoggerManager loggerManager;
        protected double minHiCapacity;
        protected double maxHiCapacity;
        public ContainerPackerBase(ILoggerManager loggerManager)
        {
            this.loggerManager = loggerManager;
            minHiCapacity = ContainerConstants.FORTY_HI_MIN_ACCEPTABLE_VOLUME;
            maxHiCapacity = ContainerConstants.FORTY_HI_DEFAULT_CAPACITY + ContainerConstants.FORTY_HI_TOLERANCE;
        }

        protected void AddLotItemToContainer(double minCapacity, double maxCapacity, List<Container<ClpItem>> containers, LotItem lotItem)
        {
            // Try to find an existing container that can fit the entire LotItem
            //var suitableContainer = containers
            //   .FirstOrDefault(c => c.CanAddItems(lotItem.Items))
            //    ;

            var suitableContainer = containers
       .Where(c => c.RemainingCapacity >= lotItem.TotalCbm)  // Ensure container has enough space for the whole LotItem
       .OrderBy(c => c.RemainingCapacity - lotItem.TotalCbm)  // Find the closest fit based on remaining capacity
       .FirstOrDefault();

            if (suitableContainer == null)
            {
                // If no suitable container is found, create a new one
                suitableContainer = CreateNewContainer($"40HI", minCapacity, maxCapacity);
                containers.Add(suitableContainer);
            }

            suitableContainer.AddItems(lotItem.Items);
        }
        protected void AddPoItemsToContainer(IEnumerable<ClpItem> poItems, List<Container<ClpItem>> containers)
        {
            double totalPoCbm = poItems.Sum(item => item.CfsReportItem.Cbm);

            while (totalPoCbm > 0)
            {
                // Find a suitable container that can hold part or all of the PO items
                var container = FindSuitableContainer(containers, totalPoCbm);

                if (container == null)
                {
                    // Create a new container if none is found
                    container = new Container<ClpItem>("NewContainer")
                    {
                        MaxCapacity = maxHiCapacity,
                        MinAcceptableVolume = minHiCapacity
                    };
                    containers.Add(container);
                }

                double remainingSpace = container.RemainingCapacity;

                // Add items that fit within the remaining capacity of the container
                var itemsToAdd = poItems.TakeWhile(item => item.CfsReportItem.Cbm <= remainingSpace).ToList();

                foreach (var item in itemsToAdd)
                {
                    item.CfsReportItem.Split = true;
                    container.Items.Add(item);
                    container.UsedCbm += item.CfsReportItem.Cbm;
                }

                // Update total remaining Cbm to pack
                totalPoCbm -= itemsToAdd.Sum(item => item.CfsReportItem.Cbm);
                poItems = poItems.Except(itemsToAdd); // Remove the added items
            }
        }

        protected Container<ClpItem> CreateNewContainer(string label, double minCapacity, double maxCapacity)
        {
            return new Container<ClpItem>(label, maxCapacity, minCapacity);
        }

        protected void FillupContainers(ref IEnumerable<Container<ClpItem>> moreThanContainers, ref List<LotItem> sortedItems)
        {
            var underUtilized = moreThanContainers.Where(a => a.UsedCbm < a.MinAcceptableVolume).ToList();
            foreach (var container in underUtilized)
            {
                if (!sortedItems.Any())
                    break;

                for (int i = 0; i < sortedItems.Count; i++)
                {
                    var lotItem = sortedItems[i];
                    if (lotItem.TotalCbm <= container.RemainingCapacity)
                    {
                        container.Items.AddRange(lotItem.Items);
                        container.UsedCbm += lotItem.TotalCbm;
                        sortedItems.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        protected Container<ClpItem> FindSuitableContainer(List<Container<ClpItem>> containers, double requiredCbm)
        {
            return containers
                .Where(c => c.RemainingCapacity >= requiredCbm)
                .OrderBy(c => c.RemainingCapacity)
                .FirstOrDefault();
        }

        protected IEnumerable<Container<ClpItem>> LoadMoreThanContainerLots(ref List<LotItem> sortedItems)
        {
            //  var minCapacity = ContainerConstants.FORTY_HI_MIN_ACCEPTABLE_VOLUME;
            // var maxCapacity = ContainerConstants.FORTY_HI_DEFAULT_CAPACITY + ContainerConstants.FORTY_HI_TOLERANCE;

            var moreThanContainerLots = sortedItems.Where(a => a.TotalCbm > maxHiCapacity).ToList();
            var containers = new List<Container<ClpItem>>();
            foreach (var lotItem in moreThanContainerLots)
            {

                PackByPo(lotItem, containers);
                sortedItems.Remove(lotItem);
            }
            return containers;
        }

        protected void LogLeftoverItems(List<ClpItem> items)
        {

            items.ForEach(a => loggerManager.LogWarn($"\tLeft over:\t{a.CfsReportItem.ToString()}"));
        }

        protected void PackByPo(LotItem lotItem, List<Container<ClpItem>> containers)
        {
            var groupedPoItems = lotItem.Items.GroupBy(item => item.PoUploadReportItem.Po).ToList();

            foreach (var poGroup in groupedPoItems)
            {
                double totalPoCbm = poGroup.Sum(item => item.CfsReportItem.Cbm);

                if (totalPoCbm <= maxHiCapacity)
                {
                    // Add PO to container if it can fit
                    var container = FindSuitableContainer(containers, totalPoCbm);
                    if (container == null)
                    {
                        container = new Container<ClpItem>("NewContainer")
                        {
                            MaxCapacity = maxHiCapacity,
                            MinAcceptableVolume = minHiCapacity
                        };
                        containers.Add(container);
                    }
                    AddPoItemsToContainer(poGroup, containers);
                }
                else
                {
                    // If PO is too large, we need to break down by SKU
                    PackBySku(poGroup, containers);
                }
            }
        }

        protected void PackBySku(IEnumerable<ClpItem> poItems, List<Container<ClpItem>> containers)
        {
            var groupedSkuItems = poItems.GroupBy(item => item.CfsReportItem.TpnLc).ToList();

            foreach (var skuGroup in groupedSkuItems)
            {
                double totalSkuCbm = skuGroup.Sum(item => item.CfsReportItem.Cbm);

                if (totalSkuCbm <= maxHiCapacity)
                {
                    // Add SKU to container if it can fit
                    var container = FindSuitableContainer(containers, totalSkuCbm);
                    if (container == null)
                    {
                        container = new Container<ClpItem>("NewContainer")
                        {
                            MaxCapacity = maxHiCapacity,
                            MinAcceptableVolume = minHiCapacity
                        };
                        containers.Add(container);
                    }
                    AddSkuItemsToContainer(skuGroup, container);
                }
                else
                {
                    // If SKU is too large, we need to split it further
                    SplitSku(skuGroup, containers);
                }
            }
        }

        protected void RedistributeItems(List<Container<ClpItem>> containers)
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
                .Select(lot => new LotItem(lot))
                .ToList();



                    foreach (var lotItemToMove in movableLotItems)
                    {
                        if (underfilledContainer.UsedCbm >= underfilledContainer.MinAcceptableVolume)
                            break;

                        overfilledContainer.RemoveItems(lotItemToMove.Items);
                        overfilledContainer.UsedCbm -= lotItemToMove.Cbm;

                        if (underfilledContainer.CanAddItems(lotItemToMove.Items))
                        {

                            underfilledContainer.AddItems(lotItemToMove.Items);
                            underfilledContainer.UsedCbm += lotItemToMove.TotalCbm;
                        }
                        if (overfilledContainer.UsedCbm <= overfilledContainer.MaxCapacity && underfilledContainer.UsedCbm >= underfilledContainer.MinAcceptableVolume)
                        {
                            break; // Move to the next underfilled container
                        }
                    }


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

        protected void ReduceUnderUtlized(IEnumerable<Container<ClpItem>> underUtilizedContainers)
        {
            var swappableContainers = underUtilizedContainers.OrderBy(c => c.UsedCbm).ToList(); // Sort by smallest used volume

            for (int i = 0; i < swappableContainers.Count - 1; i++)
            {
                var currentContainer = swappableContainers[i];
                for (int j = i + 1; j < swappableContainers.Count; j++)
                {
                    var nextContainer = swappableContainers[j];

                    if (currentContainer.UsedCbm >= currentContainer.MinAcceptableVolume)
                        break; // Skip if this container is already filled to min volume

                    // Attempt to swap items between currentContainer and nextContainer
                    SwapItemsBetweenContainers(currentContainer, nextContainer);

                    // Recheck if the containers are now sufficiently filled
                    if (currentContainer.UsedCbm >= currentContainer.MinAcceptableVolume || nextContainer.UsedCbm >= nextContainer.MinAcceptableVolume)
                        break; // If either is now filled, move to the next
                }
            }
        }

        protected void SplitSku(IEnumerable<ClpItem> skuItems, List<Container<ClpItem>> containers)
        {
            double totalSkuCbm = skuItems.Sum(item => item.CfsReportItem.Cbm);
            skuItems = skuItems.OrderByDescending(a => a.Cbm);
            foreach (var item in skuItems)
            {
                while (totalSkuCbm > 0)
                {
                    var container = FindSuitableContainer(containers, totalSkuCbm);
                    if (container == null)
                    {
                        container = new Container<ClpItem>("NewContainer")
                        {
                            MaxCapacity = maxHiCapacity,
                            MinAcceptableVolume = minHiCapacity
                        };
                        containers.Add(container);
                    }

                    double remainingCapacity = container.RemainingCapacity;
                    var itemsToAdd = skuItems.Where(i => i.CfsReportItem.Cbm <= remainingCapacity).ToList();

                    foreach (var skuItem in itemsToAdd)
                    {
                        container.Items.Add(skuItem);
                        container.UsedCbm += skuItem.CfsReportItem.Cbm;
                    }

                    totalSkuCbm -= itemsToAdd.Sum(i => i.CfsReportItem.Cbm);
                    skuItems = skuItems.Except(itemsToAdd).ToList();
                }
            }
        }

        protected void SwapItemsBetweenContainers(Container<ClpItem> containerA, Container<ClpItem> containerB)
        {
            var itemsFromA = containerA.Items.GroupBy(a => a.CfsReportItem.Lot).Select(lot => new LotItem(lot)).OrderByDescending(item => item.TotalCbm).ToList();
            var itemsFromB = containerB.Items.GroupBy(a => a.CfsReportItem.Lot).Select(lot => new LotItem(lot)).OrderByDescending(item => item.TotalCbm).ToList();

            foreach (var itemA in itemsFromA)
            {
                foreach (var itemB in itemsFromB)
                {
                    // Check if swapping these items helps fill both containers closer to their minimum volume
                    if (containerA.UsedCbm - itemA.Cbm + itemB.Cbm >= containerA.MinAcceptableVolume ||
                        containerB.UsedCbm - itemB.Cbm + itemA.Cbm >= containerB.MinAcceptableVolume)
                    {
                        // Swap itemA from containerA with itemB from containerB
                        containerA.Items.RemoveAll(i => itemA.Items.Contains(i));

                        containerA.Items.AddRange(itemB.Items);
                        containerA.UsedCbm = containerA.UsedCbm - itemA.Cbm + itemB.Cbm;

                        containerB.Items.RemoveAll(i => itemB.Items.Contains(i));
                        containerB.Items.AddRange(itemA.Items);
                        containerB.UsedCbm = containerB.UsedCbm - itemB.Cbm + itemA.Cbm;

                        return; // Stop swapping after the first successful swap
                    }
                }
            }
        }
        private void AddSkuItemsToContainer(IEnumerable<ClpItem> skuItems, Container<ClpItem> container)
        {
            foreach (var item in skuItems)
            {
                item.CfsReportItem.Split = true;
                container.Items.Add(item);
                container.UsedCbm += item.CfsReportItem.Cbm;
            }
        }
    }
}
using LoggerService;
using SharedEntities;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using TescoClpBackend.Combinators;
using TescoClpBackend.ContainerLoaders;
using TescoClpBackend.Models;

namespace TescoClpBackend.ClpLogics
{
    public class ClpPreparator
    {
        private readonly ICombinator combinator;

        public readonly IContainerLoader containerLoader;
        private readonly LoggerManager loggerManager;

        public ClpPreparator(
            ICombinator combinator,
            IContainerLoader containerLoader, LoggerManager loggerManager)
        {
            this.combinator = combinator;
            this.containerLoader = containerLoader;
            this.loggerManager = loggerManager;
        }
        public Dictionary<string, List<Container<ClpItem>>> Create(IEnumerable<CfsReport> cfsReport,
            IEnumerable<PoUploadReportItem> poUploadReport,
            bool isCutOff)
        {

            //Remove BDDAC

            var airShipmentPo = poUploadReport
                .Where(a => a.TransportationMode == TransportationMode.BDDAC)
                .ToList();
            airShipmentPo.ForEach(a => loggerManager
            .LogInfo($"PO: {a.Po} is of AIR SHIPMENT WITH SHIPMENT MODE BDDAC"));

            var poUploadList = poUploadReport
                .Where(a => a.TransportationMode == TransportationMode.BDCGP)
                .ToList();
            poUploadList.ForEach(a =>
            {
                if (a.EHD.Equals(DateHelper.GetClosestPastEHD(DateTime.Today, ConfigurationConstant.EHD)))
                {
                    a.Priority = Int32.MaxValue;
                }
            });


            var clpData = from cfs in cfsReport
                          join poItem in poUploadReport on cfs.PO equals poItem.Po
                          select new ClpItem
                          {
                              CfsReportItem = cfs,
                              PoUploadReportItem = poItem,
                          };


            var destinationGroup = clpData.GroupBy(a => a.CfsReportItem.Destination);
            return CreateDestinationWiseClp(destinationGroup, isCutOff);
        }

        private Dictionary<string, List<Container<ClpItem>>> CreateDestinationWiseClp(IEnumerable<IGrouping<string, ClpItem>> destinationGroup, bool isCutOff)
        {
            Dictionary<string, List<Container<ClpItem>>> destContainers = new Dictionary<string, List<Container<ClpItem>>>();
            foreach (var destination in destinationGroup)
            {
                var destItems = destination.ToList();


                var lotGroups = destItems.GroupBy(a => a.CfsReportItem.Lot).Select(lot => new LotItem(lot)).ToList();
                lotGroups.ForEach(a =>
                {
                    if (a.Item.Any(i => i.PoUploadReportItem.Priority == Int32.MaxValue))
                    {
                        a.Priority = Int32.MaxValue;
                    }
                });

                LoadFullContainerLots(destContainers, destination, ref lotGroups);
                LoadMoreThanContainerLots(destContainers, destination, ref lotGroups);

                var priorityGroups = lotGroups.GroupBy(a => a.Priority);
                List<ClpItem> priorityGroup = new List<ClpItem>();
                List<ClpItem> nonPriorityGroup = new List<ClpItem>();
                foreach (var priority in priorityGroups)
                {
                    if (priority.Key > 0)
                    {
                        foreach (var p in priority)
                        {
                            priorityGroup.AddRange(p.Item.Select(i => i));
                        }
                    }
                    else
                    {
                        foreach (var p in priority)
                        {
                            nonPriorityGroup.AddRange(p.Item.Select(i => i));
                        }
                    }
                }
                List<Container<ClpItem>> containers = new List<Container<ClpItem>>();

                if (lotGroups.Sum(a => a.TotalCbm) > ContainerConstants.FORTY_HI_DEFAULT_CAPACITY)
                {
                    var priorityCbm = priorityGroup.Sum(a => a.CfsReportItem.Cbm);
                    Combination priorityCombination = combinator.GetCombination(priorityCbm, false);
                    List<Container<ClpItem>> priorityContainers = new List<Container<ClpItem>>();
                    priorityContainers = containerLoader.Load(priorityCombination, ref priorityGroup);
                    LogLeftoverItems(priorityGroup);

                    var underUtilizedContainers =
                        priorityContainers.Where(a => a.Items.Sum(i => i.CfsReportItem.Cbm) < a.MinAccepatableVolume).ToList();
                    foreach (var container in underUtilizedContainers)
                    {
                        containerLoader.FillUpUnderUtilizedContainer(container, ref nonPriorityGroup);
                    }

                    var nonPriorityCbm = nonPriorityGroup.Sum(a => a.CfsReportItem.Cbm);
                    Combination nonPriorityCombination = combinator.GetCombination(nonPriorityCbm, true);
                    List<Container<ClpItem>> nonPriorityContainers = new List<Container<ClpItem>>();
                    nonPriorityContainers = containerLoader.Load(nonPriorityCombination, ref nonPriorityGroup);
                    underUtilizedContainers = nonPriorityContainers
                        .Where(a =>
                        a.Items.Sum(i => i.CfsReportItem.Cbm) < ContainerConstants.FORTY_HI_MIN_ACCEPTABLE_VOLUME)
                        .ToList();
                    
                    LogLeftoverItems(nonPriorityGroup);



                    containers.AddRange(priorityContainers);

                    containers.AddRange(nonPriorityContainers);
                }
                else
                {
                    var cbm = destination.Sum(a => a.CfsReportItem.Cbm);

                    Combination combination = combinator.GetCombination(cbm, true);

                    containers = containerLoader.Load(combination, ref destItems);

                    
                    LogLeftoverItems(destItems);
                }

                //var underUtilized= containers.FindAll(a => a.Items.Sum(a => a.CfsReportItem.Cbm) < a.MinAccepatableVolume);
                
                //   destContainers.Add(destination.Key, containers);
                if (!destContainers.ContainsKey(destination.Key))
                {
                    destContainers.Add(destination.Key, containers);
                }
                else
                {
                    destContainers[destination.Key].AddRange(containers);
                }

                destContainers[destination.Key].ForEach(a => a.UsedCbm = a.Items.Sum(i => i.CfsReportItem.Cbm));
                 var c = destContainers[destination.Key]
                    .Where(a => a.UsedCbm < a.MinAccepatableVolume)
                    .ToList();
                c.ForEach(a => a.Items.ForEach(i => loggerManager.LogInfo($"\tLeft Over:\t{i.CfsReportItem.ToString()}")));
                destContainers[destination.Key].RemoveAll(a => a.UsedCbm < a.MinAccepatableVolume);
            }
            return destContainers;
        }



        private void LoadMoreThanContainerLots(Dictionary<string, List<Container<ClpItem>>> destContainers, IGrouping<string, ClpItem> destination, ref List<LotItem> lotGroups)
        {
            var moreThanContainerLots = lotGroups.Where(a => a.TotalCbm > ContainerConstants.FORTY_HI_DEFAULT_CAPACITY + ContainerConstants.FORTY_HI_TOLERANCE).ToList();
            foreach (var container in moreThanContainerLots)
            {
                Container<ClpItem> c = new Container<ClpItem>("40HI");
                c.MaxCapacity = ContainerConstants.FORTY_HI_DEFAULT_CAPACITY + ContainerConstants.FORTY_HI_TOLERANCE;
                c.RemainingCapacity = c.MaxCapacity;
                var clone = container.Clone() as LotItem;
                double sum = 0d;

                var cap = c.MaxCapacity;
                var itemsToTake = container.Item.TakeWhile(i => (sum += i.CfsReportItem.Cbm) <= cap).ToList();
                c.Items.AddRange(itemsToTake);

                //var remainingItems = container.Item.Where(a => !itemsToTake.Contains(a)).ToList();

                lotGroups.Remove(container);

                var remainingItems = clone.Item
                    .Where(item => !itemsToTake.Contains(item)).ToList();
                remainingItems.ForEach(a => a.PoUploadReportItem.Priority = Int32.MaxValue);
                var remainingItem = remainingItems.GroupBy(a => a.CfsReportItem.Lot)
                    .First(a => a.Key == clone.Item.Key);
                if (remainingItem != null)
                {
                    
                    var l = new LotItem(remainingItem);
                    lotGroups.Add(l);
                }

                c.RemainingCapacity -= itemsToTake.Sum(a => a.CfsReportItem.Cbm);

                if (destContainers.ContainsKey(destination.Key))
                {
                    destContainers[destination.Key].Add(c);
                }
                else
                {
                    destContainers.Add(destination.Key, new List<Container<ClpItem>>() { c });
                }
            }

        }

        private static void LoadFullContainerLots(Dictionary<string, List<Container<ClpItem>>> destContainers, IGrouping<string, ClpItem> destination, ref List<LotItem> lotGroups)
        {
            var singleContainerLots = lotGroups.Where(a =>
            a.TotalCbm >= ContainerConstants.FORTY_HI_MIN_ACCEPTABLE_VOLUME &&
            a.TotalCbm <= (ContainerConstants.FORTY_HI_DEFAULT_CAPACITY + ContainerConstants.FORTY_HI_TOLERANCE)
            ).ToList();
            foreach (var singleContainerLot
                in singleContainerLots)
            {
                Container<ClpItem> c = new Container<ClpItem>("40HI");
                c.Items.AddRange(singleContainerLot.Item);
                if (destContainers.ContainsKey(destination.Key))
                {
                    destContainers[destination.Key].Add(c);
                }
                else
                {
                    destContainers.Add(destination.Key, new List<Container<ClpItem>>() { c });
                }
            }
            lotGroups.RemoveAll(a => singleContainerLots.Contains(a));
        }
        private void LogLeftoverItems(List<ClpItem> items) {

            items.ForEach(a => loggerManager.LogInfo($"\tLeft over:\t{a.CfsReportItem.ToString()}"));
        }
    }
}

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
                
                var priorityGroups=lotGroups.GroupBy(a => a.Priority);
                List<ClpItem>priorityGroup= new List<ClpItem>();
                List<ClpItem>nonPriorityGroup=new List<ClpItem>();
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

                var priorityCbm = priorityGroup.Sum(a => a.CfsReportItem.Cbm);
                Combination priorityCombination = combinator.GetCombination(priorityCbm, false);
                List<Container<ClpItem>> priorityContainers = new List<Container<ClpItem>>();
                priorityContainers = containerLoader.Load(priorityCombination, ref priorityGroup);
                priorityGroup.ForEach(a => loggerManager.LogInfo($"\tLeft over:{a.CfsReportItem.ToString()}"));


                var nonPriorityCbm = nonPriorityGroup.Sum(a => a.CfsReportItem.Cbm);
                Combination nonPriorityCombination=combinator.GetCombination(nonPriorityCbm, true);
                List<Container<ClpItem>> nonPriorityContainers = new List<Container<ClpItem>>();
                nonPriorityContainers=containerLoader.Load(nonPriorityCombination, ref nonPriorityGroup);
                nonPriorityGroup.ForEach(a => loggerManager.LogInfo($"\tLeft over:{a.CfsReportItem.ToString()}"));

                
                //var cbm = destination.Sum(a => a.CfsReportItem.Cbm);

                //Combination combination = combinator.GetCombination(cbm,true);
                List<Container<ClpItem>> containers = new List<Container<ClpItem>>();
                containers.AddRange(priorityContainers);
                containers.AddRange(nonPriorityContainers);

                destContainers.Add(destination.Key, containers);

                //containers = containerLoader.Load(combination, ref destItems);
                //destItems.ForEach(a => loggerManager.LogInfo($"\tLeft over:{a.CfsReportItem.ToString()}"));
                //destContainers.Add(destination.Key, containers);

            }
            return destContainers;
        }
    }
}

using LoggerService;
using NPOI.SS.Formula.Functions;
using SharedEntities;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using TescoClpBackend.ContainerLoaders;
using TescoClpBackend.Models;

namespace TescoClpBackend.ClpLogics
{
    public class ContainerPacker : ContainerPackerBase,IContainerPacker
    {
        //private readonly ILoggerManager loggerManager;
        //private readonly double minHiCapacity;
        //private readonly double maxHiCapacity;

        public ContainerPacker(ILoggerManager loggerManager):base(loggerManager)
        {
            //this.loggerManager = loggerManager;
            //minHiCapacity = ContainerConstants.FORTY_HI_MIN_ACCEPTABLE_VOLUME;
            //maxHiCapacity = ContainerConstants.FORTY_HI_DEFAULT_CAPACITY + ContainerConstants.FORTY_HI_TOLERANCE;
        }
        public List<Container<ClpItem>> PackItems(List<LotItem> lotItems, double minCapacity, double maxCapacity)
        {
            List<Container<ClpItem>> containers = new List<Container<ClpItem>>();
            loggerManager.LogInfo($"\t{lotItems.First().Items.First().Destination}\tTOTAL CBM:\t{lotItems.Sum(l => l.TotalCbm)}");
            // Sort LotItems by descending TotalCbm to try and fill larger items first


            var sortedItems = lotItems.OrderByDescending(l => l.TotalCbm).ToList();
            //var singleLotContainers = LoadMoreThanContainerLots(ref sortedItems);
            var moreThanContainers = LoadMoreThanContainerLots(ref sortedItems);

            FillupContainers(ref moreThanContainers, ref sortedItems);
            containers.AddRange(moreThanContainers);

            foreach (var lotItem in sortedItems)
            {
                AddLotItemToContainer(minCapacity, maxCapacity, containers, lotItem);
            }

            // Perform redistribution to ensure all containers meet the minimum capacity requirement
            //   containers.ForEach(a => loggerManager.LogDebug($"BEFORE REDISTRIBUTION:{a.ContainerId}\t{a.UsedCbm}"));
            #region Redistribution

            var underUtilizedContainers = containers.Where(a => a.UsedCbm < a.MinAcceptableVolume);
            if (underUtilizedContainers.Count() > 1)
            {
                RedistributeItems(containers);
            }
            #endregion

            #region Swapping
            underUtilizedContainers = containers.Where(a => a.UsedCbm < a.MinAcceptableVolume);
            if (underUtilizedContainers.Count() > 1)
            {
                ReduceUnderUtlized(underUtilizedContainers);
            }

            #endregion

            #region Cleaning
            var containersToRemove = containers
                .Where(a => a.UsedCbm < a.MinAcceptableVolume)
                .ToList();

            containers.RemoveAll(a => containersToRemove.Select(c => c.ContainerId).Contains(a.ContainerId));

            //containers.AddRange(singleLotContainers);
            containersToRemove.ForEach(a => LogLeftoverItems(a.Items));
            containersToRemove.ForEach(a => loggerManager.LogInfo($"\t{a.Items.First().CfsReportItem.Destination}\tLEFT OVER\t{a.UsedCbm}"));
            #endregion


            containers.ForEach(a => loggerManager.LogInfo($"\t{a.Items.First().CfsReportItem.Destination}\tUSED\t{a.UsedCbm}"));


            return containers;
        }
    }
}

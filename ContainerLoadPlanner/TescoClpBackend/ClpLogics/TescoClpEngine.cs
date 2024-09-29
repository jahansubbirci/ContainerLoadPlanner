using LoggerService;
using SharedEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TescoClpBackend.Combinators;
using TescoClpBackend.ContainerLoaders;
using TescoClpBackend.Models;

namespace TescoClpBackend.ClpLogics
{
    public class TescoClpEngine:IClpEngine
    {
        private readonly ICombinator combinator;
        private readonly IContainerLoader containerLoader;
        private readonly ILoggerManager loggerManager;
        private readonly IContainerPacker containerPacker;

        public TescoClpEngine(ICombinator combinator,
            IContainerLoader containerLoader,
            ILoggerManager loggerManager,
            IContainerPacker containerPacker)
        {
            this.combinator = combinator;
            this.containerLoader = containerLoader;
            this.loggerManager = loggerManager;
            this.containerPacker = containerPacker;
        }
        public Dictionary<string,List<Container<ClpItem>>>Create(IEnumerable<CfsReport> cfsReport,
            IEnumerable<PoUploadReportItem>poUploadReport,
            ClpEngineSettings settings)
        {
            FilterOutData(ref cfsReport, ref poUploadReport, settings);
            SetPriorityPO(ref poUploadReport);
           
            var clpData=from cfs in cfsReport
                        join poItem in poUploadReport 
                        on cfs.PO equals poItem.Po
                        select new ClpItem { CfsReportItem = cfs ,PoUploadReportItem=poItem};
            clpData.ToList().OrderByDescending(x => x.CfsReportItem.Lot).ToList().GroupBy(a=>a.CfsReportItem.Lot).ToList().ForEach(a => loggerManager.LogDebug($"\tCLP DATA\t{a.First().Destination}\t{a.Key}"));
            var destinationGroups = clpData.GroupBy(a => a.CfsReportItem.Destination);

            var destinationWiseClp=CreateDestinationWiseClp(destinationGroups);

            return destinationWiseClp;
        }
        
        private Dictionary<string,List<Container<ClpItem>>> CreateDestinationWiseClp(IEnumerable<IGrouping<string, ClpItem>> destinationGroups)
        {
            Dictionary<string, List<Container<ClpItem>>> destContainers = new Dictionary<string, List<Container<ClpItem>>>();

            foreach (var destination in destinationGroups)
            {

                var lots=destination.GroupBy(a => a.CfsReportItem.Lot)
                    .OrderByDescending(a=>a.Key)
                    .Select(lot=>new LotItem(lot))
                    
                    .ToList();
              var containers=  containerPacker.PackItems(lots, ContainerConstants.FORTY_HI_MIN_ACCEPTABLE_VOLUME
                    , (ContainerConstants.FORTY_HI_DEFAULT_CAPACITY+ContainerConstants.FORTY_HI_TOLERANCE));
               
                destContainers.Add(destination.Key,containers);
            }
            return destContainers;
        }

        private  void SetPriorityPO(ref IEnumerable<PoUploadReportItem> poUploadReport)
        {
            foreach (var item in poUploadReport)
            {
                if (item.EHD < DateHelper.GetClosestPastEHD(DateTime.Today, ConfigurationConstant.EHD))
                {
                    item.Priority = Int32.MaxValue;
                }
                if (item.EHD.Equals(DateHelper.GetClosestPastEHD(DateTime.Today, ConfigurationConstant.EHD)))
                {
                    item.Priority = Int32.MaxValue - 1;
                }
            }
        }

        private  void FilterOutData(ref IEnumerable<CfsReport> cfsReport, ref IEnumerable<PoUploadReportItem> poUploadReport, ClpEngineSettings settings)
        {
            if (settings.IgnorePoWithoutDocs)
                cfsReport = cfsReport.Where(a => !a.RecvDoc.Equals(DateTime.MinValue));
           cfsReport. OrderByDescending(x => x.Lot).ToList().ForEach(a => loggerManager.LogDebug($"\tDOC RECEIVED\t{a.Destination}\t{a.Lot}"));
            //remove AIR shipment
            poUploadReport = poUploadReport.Where(a => a.TransportationMode == TransportationMode.BDCGP);
        }
    }
}

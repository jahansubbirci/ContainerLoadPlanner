using Caliburn.Micro;
using LoggerService;
using SharedEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TescoClpBackend;
using TescoClpBackend.ClpLogics;
using TescoClpBackend.Combinators;
using TescoClpBackend.ContainerLoaders;

namespace ContainerLoadPlanner.Utilities
{
    public static class TescoExtensions
    {

        public static void AddTescoServices(this SimpleContainer container)
        {
            container.Singleton<FortyHICombinator>();
            container.Singleton<MixContainerCombinator>();
            container.Singleton<FortyHCLoader>(CombinatorConstants.REGULAR);
            container.Singleton<FortySTDLoader>();
            container.Singleton<MixedLoader>(CombinatorConstants.CUT_OFF);
            container.RegisterInstance(
                typeof(ClpPreparator), CombinatorConstants.REGULAR,
                new ClpPreparator(
                container.GetInstance<FortyHICombinator>(),
                container.GetInstance<FortyHCLoader>(),
                container.GetInstance<LoggerManager>())
                );

            container.RegisterInstance(
                typeof(ClpPreparator), CombinatorConstants.CUT_OFF,
             new ClpPreparator(
                container.GetInstance<MixContainerCombinator>(),
                container.GetInstance<MixedLoader>(),
                container.GetInstance<LoggerManager>()));

            container.Singleton<Reporting>();
            ReportingService service = container.GetInstance<ReportingService>();
        }
       
    }
}

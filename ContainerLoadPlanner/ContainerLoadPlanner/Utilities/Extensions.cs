using Caliburn.Micro;
using ÉxcelDataExchange;
using ExcelDataExchange.Reader;
using ÉxcelDataExchange.Reader;
using ÉxcelDataExchange.Writer;
using NPOI.SS.Formula.Functions;
using SharedEntities;
using SharedEntities.Reporting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TescoClpBackend;
using TescoClpBackend.ClpLogics;
using TescoClpBackend.Models;

namespace ContainerLoadPlanner.Utilities
{
    internal static class Extensions
    {
        public static void AddSharedServices(this SimpleContainer container) {
            container.Singleton<CfsDataRetriever>();
            container.Singleton<PoUploadReportDataLoader>();
            

        }
        public static void AddExcelServices(this SimpleContainer container)
        {
            container.Singleton<IExcelDataReader,ExcelDataReaderV2>("general");
            container.Singleton<IExcelDataReader, ExcelDataReaderWithFormula>("formula");
            
            container.Singleton<ExcelUtilities>();
        }
        public static void AddExcelWriterService<T>(this SimpleContainer container)
        {
            container.Singleton<ExcelDataWriter2<T>>();
            
        }
        public static void AddClpReportingService(this SimpleContainer container)  
        {
            container.Singleton<ClpReporting<ClpDto>>();
        }
      
    }
}

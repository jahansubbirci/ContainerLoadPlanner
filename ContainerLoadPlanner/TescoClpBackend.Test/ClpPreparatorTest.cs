using ExcelDataExchange.Reader;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedEntities;
using System;
using System.Linq;
using System.Threading.Tasks;
using TescoClpBackend.ClpLogics;
using TescoClpBackend.Combinators;
using TescoClpBackend.ContainerLoaders;

namespace TescoClpBackend.Test
{
    [TestClass]
    public class ClpPreparatorTest
    {
        string poReportFileName = @"C:\Users\msj046\Documents\TESCO PO REPORT.xlsx";
        string cfsReportFileName = @"C:\Users\msj046\Documents\CFS REPORT.xlsx";

        IExcelDataReader excelDataReader = null;
        PoUploadReportDataLoader poUploadReportDataLoader = null;
        CfsDataRetriever cfsDataRetriever = null;
        ClpPreparator clpPreparator;
        FortyHCLoader fortyHCLoader = null;
        FortySTDLoader fortySTDLoader = null;
        ICombinator combinator = null;
        [TestInitialize]
        public void Init()
        {
            excelDataReader = new ExcelDataReader();
            poUploadReportDataLoader = new PoUploadReportDataLoader(excelDataReader, null);
            cfsDataRetriever = new CfsDataRetriever(excelDataReader);
            fortyHCLoader = new FortyHCLoader();
            fortySTDLoader = new FortySTDLoader();
            //clpPreparator=new ClpPreparator();
        }


        [TestMethod]
        public async Task Create_Test_When_Not_Cutoff()
        {
            //Arrange
            var poDataTask = Task.Run(() =>
            poUploadReportDataLoader.GetPoReport(poReportFileName)
            );
            var cfsDataTask = Task.Run(() =>
            cfsDataRetriever.GetCfsData(cfsReportFileName, "Total Raw", "A1:BZ", CFS.SAPL)
            );
            await Task.WhenAll(poDataTask, cfsDataTask);
            var poData = await poDataTask;
            var cfsData = await cfsDataTask;
            combinator = new FortyHICombinator();
            clpPreparator = new ClpPreparator(combinator, fortyHCLoader,null);

            //Act
            var containers = clpPreparator.Create(cfsData, poData, false);

            //Assert
            Assert.IsTrue(containers.Count >= 4);
        }

        [TestMethod]
        public async Task Create_Test_When_Cutoff()
        {
            //Arrange
            var poDataTask = Task.Run(() =>
            poUploadReportDataLoader.GetPoReport(poReportFileName)
            );
            var cfsDataTask = Task.Run(() =>
            cfsDataRetriever.GetCfsData(cfsReportFileName, "Total Raw", "A1:BZ", CFS.SAPL)
            );
            await Task.WhenAll(poDataTask, cfsDataTask);
            var poData = await poDataTask;
            var cfsData = await cfsDataTask;
            bool isCutOff = true;
            combinator = new MixContainerCombinator();
            clpPreparator = new ClpPreparator(combinator, new MixedLoader(fortyHCLoader, fortySTDLoader),null);

            //Act
            var containers=clpPreparator.Create(cfsData, poData, isCutOff);

            //Assert
            var values=containers.Values.ToList();
            
            Assert.IsTrue(values.Count >= 5);
        }
    }
}

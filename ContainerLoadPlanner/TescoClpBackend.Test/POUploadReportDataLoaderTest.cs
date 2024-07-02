using ExcelDataExchange.Reader;
using ÉxcelDataExchange.Reader;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace TescoClpBackend.Test
{
    [TestClass]
    public class POUploadReportDataLoaderTest
    {
        public POUploadReportDataLoaderTest()
        {
            
        }
        PoUploadReportDataLoader poUploadReportDataLoader;
        IExcelDataReader reader;
        string poReportFileName = @"C:\Users\msj046\Documents\TESCO PO REPORT.xlsx";
        [TestInitialize] public void Init()
        {
            reader=new ExcelDataReaderV2();
             poUploadReportDataLoader=new
                PoUploadReportDataLoader(reader,null);
        }
        [TestMethod]
        public void GetPoReport_ShouldThrowIOException_WhenFileIsOpened()
        {

            Assert.ThrowsException<IOException>(()=>poUploadReportDataLoader.GetPoReport(poReportFileName));
           // Assert.ThrowsException<IndexOutOfRangeException>(() => poUploadReportDataLoader.GetPoReport(poReportFileName));
        }
        [TestMethod]
        public void GetPoReport_ShouldThrowIOException_WhenColumnIsNotFound()
        {

            //Assert.ThrowsException<IOException>(() => poUploadReportDataLoader.GetPoReport(poReportFileName));
            Assert.ThrowsException<ArgumentException>(() => poUploadReportDataLoader.GetPoReport(poReportFileName));
        }
        [TestMethod]
        public void GetPoReport_ShouldReturnList()
        {

            var report=poUploadReportDataLoader.GetPoReport(poReportFileName);
            //Assert.ThrowsException<IOException>(() => poUploadReportDataLoader.GetPoReport(poReportFileName));
            //Assert.ThrowsException<ArgumentException>(() => poUploadReportDataLoader.GetPoReport(poReportFileName));
        }
    }
}

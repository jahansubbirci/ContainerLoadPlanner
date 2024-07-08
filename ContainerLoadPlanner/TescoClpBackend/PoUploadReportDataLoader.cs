using ExcelDataExchange.Reader;
using LoggerService;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TescoClpBackend.Models;

namespace TescoClpBackend
{
    public class PoUploadReportDataLoader
    {
        private readonly IExcelDataReader excelDataReader;
        private readonly ILoggerManager loggerManager;

        public PoUploadReportDataLoader(IExcelDataReader excelDataReader,
            ILoggerManager loggerManager)
        {
            this.excelDataReader = excelDataReader;
            this.loggerManager = loggerManager;
        }
        public IEnumerable<PoUploadReportItem> GetPoReport
            (string fileName)
        {
            string sheetName = "TESCO CLP REPORT_aggr";
            string range = "A1:AZ";
            try
            {
                var dataTable = excelDataReader
                                .GetData(fileName, sheetName, range);

                List<PoUploadReportItem> poUploadReport = new List<PoUploadReportItem>();
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    var row = dataTable.Rows[i];
                    
                    PoUploadReportItem item = new PoUploadReportItem();
                    item.Po = row["PO Number"].ToString();
                    item.EHD = (row["EHD"] != DBNull.Value) ? (DateTime.TryParse(row["EHD"].ToString(),out var ehd)?ehd : DateTime.MinValue):DateTime.MinValue;

                    if (Enum.TryParse<TransportationMode>(row["Place Of Receipt Site"].ToString(), true, out var mode))
                    {
                        item.TransportationMode = mode;
                    }
                    item.IDD = (row["IDD"] != DBNull.Value) ? (DateTime.TryParse(row["IDD"].ToString(),out var idd)?idd:DateTime.MinValue) : DateTime.MinValue;
                    poUploadReport.Add(item);
                }
                return poUploadReport;
            }
            catch (FieldAccessException fileAccessException)
            {
                loggerManager.LogError(fileAccessException, fileAccessException.Message);
                throw;
            }
            catch (ArgumentException argException)
            {
                loggerManager.LogError(argException, argException.Message);
                throw;
            }
            catch (IndexOutOfRangeException ise)
            {
                loggerManager.LogError(ise, ise.Message);

                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}

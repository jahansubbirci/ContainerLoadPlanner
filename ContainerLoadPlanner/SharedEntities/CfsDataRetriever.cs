using ExcelDataExchange.Reader;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedEntities
{
    public class CfsDataRetriever
    {
        private readonly IExcelDataReader excelDataReader;

        public CfsDataRetriever(IExcelDataReader excelDataReader)
        {
            this.excelDataReader = excelDataReader;
        }

        public IEnumerable<CfsReport> GetCfsData
            (string fileName,
            string sheetName,
            string range,
            CFS cfs
            )
        {
            List<CfsReport> cfsItemLines = null;// = new List<CfsReport>();
            var data = excelDataReader.GetData(fileName, sheetName, range);
            switch (cfs)
            {
                case CFS.SAPL:
                    cfsItemLines = GetSaplData(data);
                    break;
            }
            return cfsItemLines;
        }

        private List<CfsReport> GetSaplData(DataTable data)
        {
            List<CfsReport> cfsReportItems = new List<CfsReport>();
            foreach (DataRow row in data.Rows)
            {
                CfsReport cfsReportItem = new CfsReport();

                cfsReportItem.So = row["S/O"].ToString();
                if (!string.IsNullOrWhiteSpace(cfsReportItem.So))
                {
                    cfsReportItem.Consignee = row["Consignee"].ToString();
                    cfsReportItem.Shipper = row["SHIPPER"].ToString();
                    cfsReportItem.Measurement = row["MSMT"].ToString();
                    cfsReportItem.Destination = row["DEST"].ToString();
                    cfsReportItem.PO = row["PO"].ToString();
                    cfsReportItem.Style = row["STYLE"].ToString();
                    cfsReportItem.Item = row["ITEM"].ToString();
                    cfsReportItem.TpnLc = row["TPN/LC"].ToString();
                    cfsReportItem.RefSize = row["REF/SIZE"].ToString();
                    cfsReportItem.RmsStyle = row["RMS STYLE"].ToString();
                    cfsReportItem.Category = row["CAT"].ToString();
                    cfsReportItem.Division = row["DIV"].ToString();
                    cfsReportItem.Department = row["DEPT"].ToString();
                    cfsReportItem.SB = row["SB"].ToString();
                    cfsReportItem.Vat = row["VAT"].ToString();
                    cfsReportItem.Loc = row["LOC"].ToString();
                    cfsReportItem.Commodity = row["COMMODITY"].ToString();
                    cfsReportItem.Remarks = row["REMARKS"].ToString();
                    cfsReportItem.Overflow = row["Overflow"].ToString();
                    //cfsReportItem.StuffingHeldupRemarks = row["Stuffing Heldup Remarks"].ToString();
                    cfsReportItem.Forwarder = row["FORWARDER"].ToString();
                    cfsReportItem.Clr = row["CLR"].ToString();
                    cfsReportItem.Sku = row["SKU"].ToString();
                    cfsReportItem.StrokeNo = row["STROKE NO."].ToString();
                    cfsReportItem.RailNo = row["RAIL NO."].ToString();
                    cfsReportItem.InvoiceNo = row["INVOICE NO"].ToString();
                    cfsReportItem.Id = row["#ID"].ToString();
                    try
                    {
                        cfsReportItem.Lot = Convert.ToInt32(row["LOT"].ToString().Trim());
                        cfsReportItem.Pkgs = Convert.ToInt32(row["PKGS"].ToString().Trim());
                        cfsReportItem.Qty = Convert.ToInt32(row["QTY"].ToString().Trim());
                        cfsReportItem.Cbm = Convert.ToDouble(row["CBM"].ToString().Trim());
                        cfsReportItem.CWeight = Convert.ToDouble(row["CWEIGHT"].ToString().Trim());
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Check LOT/PKGS/QTY/CBM/CWeight for {cfsReportItem.PO}");
                    }
                    if (row["R/CGO"] != DBNull.Value)
                    {
                        cfsReportItem.RCgo = DateTime.TryParse(row["R/CGO"].ToString(), out var rCGo) ? rCGo : DateTime.MinValue;
                    }
                    else
                    {
                        cfsReportItem.RCgo = DateTime.MinValue;
                    }

                    if (row["RECVDOC"] != DBNull.Value)
                    {
                        cfsReportItem.RecvDoc = DateTime.TryParse(row["RECVDOC"].ToString(), out var rcvDoc) ? rcvDoc : DateTime.MinValue;
                    }
                    else
                    {
                        cfsReportItem.RecvDoc = DateTime.MinValue;
                    }

                    if (row["SB DATE"] != DBNull.Value)
                    {
                        cfsReportItem.SbDate = DateTime.TryParse(row["SB DATE"].ToString(), out var sbDate) ? sbDate : DateTime.MinValue;
                    }
                    else
                    {
                        cfsReportItem.SbDate = DateTime.MinValue;
                    }

                    if (row["ERD DATE"] != DBNull.Value)
                    {
                        cfsReportItem.ErdDate = DateTime.TryParse(row["ERD DATE"].ToString(), out var erd) ? erd : DateTime.MinValue;
                    }
                    else
                    {
                        cfsReportItem.ErdDate = DateTime.MinValue;
                    }

                    cfsReportItem.CustomsHouseCode = (row["CUSTOMS HOUSE CODE"] != DBNull.Value) ? Int32.TryParse(row["CUSTOMS HOUSE CODE"].ToString().Trim(), out var customsCode) ? customsCode : 0 : 0;
                    cfsReportItem.InvoiceValue = (row["INVOICE VALUE"] != DBNull.Value) ? Double.TryParse(row["INVOICE VALUE"].ToString().Trim(), out var invoiceValue) ? invoiceValue : 0 : 0;

                    #region object initialization
                    //CfsReport cfsReportItem = new CfsReport()
                    //{
                    //    So = row["S/O"].ToString(),
                    //    Consignee = row["Consignee"].ToString(),
                    //    Shipper = row["SHIPPER"].ToString(),

                    //    //= row["UNIT"].ToString(),
                    //    Measurement = row["MSMT"].ToString(),

                    //    //= row["UNIT"].ToString(),
                    //    Destination = row["DEST"].ToString(),
                    //    PO = row["PO"].ToString(),
                    //    Style = row["STYLE"].ToString(),
                    //    Item = row["ITEM"].ToString(),
                    //    TpnLc = row["TPN/LC"].ToString(),
                    //    RefSize = row["REF/SIZE"].ToString(),
                    //    RmsStyle = row["RMS STYLE"].ToString(),
                    //    Category = row["CAT"].ToString(),
                    //    Division = row["DIV"].ToString(),
                    //    Department = row["DEPT"].ToString(),

                    //    SB = row["SB"].ToString(),

                    //    Vat = row["VAT"].ToString(),
                    //    Loc = row["LOC"].ToString(),
                    //    Commodity = row["COMMODITY"].ToString(),
                    //    Remarks = row["REMARKS"].ToString(),
                    //    Overflow = row["Overflow"].ToString(),
                    //    StuffingHeldupRemarks = row["Stuffing Heldup Remarks"].ToString(),
                    //    //= row["Account"].ToString(),
                    //    Forwarder = row["FORWARDER"].ToString(),
                    //    Clr = row["CLR"].ToString(),
                    //    Sku = row["SKU"].ToString(),
                    //    StrokeNo = row["STROKE NO."].ToString(),
                    //    RailNo = row["RAIL NO."].ToString(),
                    //    InvoiceNo = row["INVOICE NO"].ToString(),
                    //    Id = row["#ID"].ToString(),

                    //    Lot = Convert.ToInt32(row["LOT"].ToString().Trim()),
                    //    Pkgs = Convert.ToInt32(row["PKGS"].ToString().Trim()),
                    //    Qty = Convert.ToInt32(row["QTY"].ToString().Trim()),
                    //    Cbm = Convert.ToDouble(row["CBM"].ToString().Trim()),
                    //    CWeight = Convert.ToDouble(row["CWEIGHT"].ToString().Trim()),
                    //    RCgo = (row["R/CGO"]!=DBNull.Value)?(DateTime.TryParse(row["R/CGO"].ToString(),out var rCGo)?rCGo:DateTime.MinValue):DateTime.MinValue,
                    //    RecvDoc = (row["RECVDOC"] != DBNull.Value) ? (DateTime.TryParse(row["RECVDOC"].ToString(), out var rcvDoc) ? rcvDoc : DateTime.MinValue) : DateTime.MinValue,
                    //    SbDate = (row["SB DATE"] != DBNull.Value) ? (DateTime.TryParse(row["SB DATE"].ToString(), out var sbDate) ? sbDate : DateTime.MinValue) : DateTime.MinValue,
                    //    ErdDate = (row["ERD DATE"] != DBNull.Value) ? (DateTime.TryParse(row["ERD DATE"].ToString(), out var erd) ? erd : DateTime.MinValue) : DateTime.MinValue,
                    //  //  Convert.ToDateTime(row["ERD DATE"].ToString()),
                    //    CustomsHouseCode = Convert.ToInt32(row["CUSTOMS HOUSE CODE"].ToString().Trim()),
                    //    InvoiceValue = Convert.ToDouble(row["INVOICE VALUE"].ToString().Trim()),
                    //    //= row["E-BOOKING NO."].ToString(),

                    //};
                    #endregion
                    cfsReportItems.Add(cfsReportItem);
                }
            }
            return cfsReportItems;
        }
    }
}

using ExcelDataExchange.Reader;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedEntities
{
    internal class CfsDataRetriever
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
            List<CfsReport> cfsItemLines = new List<CfsReport>();
            var data = excelDataReader.GetData(fileName, sheetName, range);
            switch (cfs)
            {
                case CFS.ISATL:
                    cfsItemLines = GetIsatlData(data);
                    break;
            }
            return cfsItemLines;
        }

        private List<CfsReport> GetIsatlData(DataTable data)
        {

            foreach (DataRow row in data.Rows)
            {
                CfsReport cfsReport = new CfsReport()
                {
                    So = row["S/O"].ToString(),
                    Consignee = row["Consignee"].ToString(),
                    Shipper = row["SHIPPER"].ToString(),
                   
                    //= row["UNIT"].ToString(),
                    Measurement = row["MSMT"].ToString(),
                  
                    //= row["UNIT"].ToString(),
                    Destination = row["DEST"].ToString(),
                    PO = row["PO"].ToString(),
                    Style = row["STYLE"].ToString(),
                    Item = row["ITEM"].ToString(),
                    TpnLc = row["TPN/LC"].ToString(),
                    RefSize = row["REF/SIZE"].ToString(),
                    RmsStyle = row["RMS STYLE"].ToString(),
                    Category = row["CAT"].ToString(),
                    Division = row["DIV"].ToString(),
                    Department = row["DEPT"].ToString(),
                   
                    SB = row["SB"].ToString(),
                    
                    Vat = row["VAT"].ToString(),
                    Loc = row["LOC"].ToString(),
                    Commodity = row["COMMODITY"].ToString(),
                    Remarks = row["REMARKS"].ToString(),
                    Overflow = row["Overflow"].ToString(),
                    StuffingHeldupRemarks = row["Stuffing Heldup Remarks"].ToString(),
                    //= row["Account"].ToString(),
                    Forwarder = row["FORWARDER"].ToString(),
                    Clr = row["CLR"].ToString(),
                    Sku = row["SKU"].ToString(),
                    StrokeNo = row["STROKE NO."].ToString(),
                    RailNo = row["RAIL NO."].ToString(),
                    InvoiceNo = row["INVOICE NO"].ToString(),
                    Id = row["#ID"].ToString(),

                    Lot = Convert.ToInt32(row["LOT"].ToString()),
                    Pkgs = Convert.ToInt32(row["PKGS"].ToString().Trim()),
                    Qty = row["QTY"].ToString(),
                    Cbm = Convert.ToDouble(row["CBM"].ToString().Trim()),
                    CWeight = Convert.ToDouble(row["CWEIGHT"].ToString()),
                    RCgo = Convert.ToDateTime(row["R/CGO"].ToString()),
                    RecvDoc = Convert.ToDateTime(row["RECVDOC"].ToString()),
                    SbDate = Convert.ToDateTime(row["SB DATE"].ToString()),
                    ErdDate = Convert.ToDateTime(row["ERD DATE"].ToString()),
                    CustomsHouseCode = Convert.ToInt16(row["CUSTOMS HOUSE CODE"].ToString()),
                    InvoiceValue = Convert.ToDouble(row["INVOICE VALUE"].ToString()),
                    //= row["E-BOOKING NO."].ToString(),
                };
            }
        }
    }
}

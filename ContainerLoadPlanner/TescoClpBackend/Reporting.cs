using ExcelWriterNetFramework;
using NPOI.SS.UserModel;
using SharedEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TescoClpBackend.ClpLogics;

namespace TescoClpBackend
{
    public class Reporting : BaseExcelWriter
    {
        public void CreateReport(string fileName, Dictionary<string, List<Container<ClpItem>>> clpData)
        {
            foreach (var destination in clpData)
            {
                CreateOrOpenWorkbook(fileName, destination.Key, true);
                int i = 0;
                foreach (Container<ClpItem> container in destination.Value)
                {


                    CreateHeader(ref i);
                    foreach (ClpItem item in container.Items)
                    {
                        CreateData(item,ref i);
                       // i++;
                    }
                    CreateSummary(ref i,container);
                    i++;
                }
                Save(fileName, overwrite: true);
            }
            Save(fileName, overwrite: true);
        }

        private void CreateSummary(ref int i,Container<ClpItem>container)
        {
            CreateCell(sheet, i, 4, container.Items.Sum(a => a.CfsReportItem.Pkgs));
            CreateCell(sheet, i, 5, container.Items.Sum(a => a.CfsReportItem.Qty));
            CreateCell(sheet, i, 7, container.Items.Sum(a => a.CfsReportItem.Cbm));
            CreateCell(sheet, i, 8, container.Items.Sum(a => a.CfsReportItem.CWeight));
            i++;
        }

        private void CreateData(ClpItem item,ref int i)
        {
            
            CreateCell(sheet, i, 0, item.CfsReportItem.So);
            CreateCell(sheet, i, 1, item.CfsReportItem.Consignee);
            CreateCell(sheet, i, 2, item.CfsReportItem.Shipper);
            CreateCell(sheet, i, 3, item.CfsReportItem.Lot);
            CreateCell(sheet, i, 4, item.CfsReportItem.Pkgs);
            CreateCell(sheet, i, 5, item.CfsReportItem.Qty);
            CreateCell(sheet, i, 6, item.CfsReportItem.Measurement);
            CreateCell(sheet, i, 7, item.CfsReportItem.Cbm);
            CreateCell(sheet,i,8, item.CfsReportItem.CWeight);
            i++;
        }

      

        private void CreateHeader(ref int i)
        {
            var headerStyle = SetHeaderCellStyle();
            CreateCell(sheet, i, 0, "S/O", headerStyle);
            CreateCell(sheet, i, 1, "CONSIGNEE", headerStyle);
            CreateCell(sheet, i, 2, "SHIPPPER", headerStyle);
            CreateCell(sheet, i, 3, "LOT", headerStyle);
            CreateCell(sheet, i, 4, "PKGS", headerStyle);
            CreateCell(sheet, i, 5, "QTY", headerStyle);
            CreateCell(sheet, i, 6, "UNIT", headerStyle);
            CreateCell(sheet, i, 7, "MSMT", headerStyle);
            CreateCell(sheet, i, 8, "CBM", headerStyle);
            CreateCell(sheet, i, 9, "CWEIGHT", headerStyle);
            CreateCell(sheet, i, 10, "UNIT", headerStyle);
            CreateCell(sheet, i, 11, "DEST", headerStyle);
            CreateCell(sheet, i, 12, "PO", headerStyle);
            CreateCell(sheet, i, 13, "STYLE", headerStyle);
            CreateCell(sheet, i, 14, "ITEM", headerStyle);
            CreateCell(sheet, i, 15, "TPN/LC", headerStyle);
            CreateCell(sheet, i, 16, "REF/SIZE", headerStyle);
            CreateCell(sheet, i, 17, "RMS STYLE", headerStyle);
            CreateCell(sheet, i, 18, "CAT", headerStyle);
            CreateCell(sheet, i, 19, "DIV", headerStyle);
            CreateCell(sheet, i, 20, "DEPT", headerStyle);
            CreateCell(sheet, i, 21, "R/CGO", headerStyle);
            CreateCell(sheet, i, 22, "SB", headerStyle);
            CreateCell(sheet, i, 23, "RECVDOC", headerStyle);
            CreateCell(sheet, i, 24, "SB DATE", headerStyle);
            CreateCell(sheet, i, 25, "ERD DATE", headerStyle);
            CreateCell(sheet, i, 26, "CUSTOMS HOUSE CODE", headerStyle);
            CreateCell(sheet, i, 27, "INVOICE VALUE", headerStyle);
            CreateCell(sheet, i, 28, "E-BOOKING NO.", headerStyle);
            CreateCell(sheet, i, 29, "VAT", headerStyle);
            CreateCell(sheet, i, 30, "LOC", headerStyle);
            CreateCell(sheet, i, 31, "COMMODITY", headerStyle);
            CreateCell(sheet, i, 32, "REMARKS", headerStyle);
            CreateCell(sheet, i, 33, "Overflow", headerStyle);
            CreateCell(sheet, i, 34, "Stuffing Heldup Remarks", headerStyle);
            CreateCell(sheet, i, 35, "Account", headerStyle);
            CreateCell(sheet, i, 36, "FORWARDER", headerStyle);
            CreateCell(sheet, i, 37, "CLR", headerStyle);
            CreateCell(sheet, i, 38, "SKU", headerStyle);
            CreateCell(sheet, i, 39, "STROKE NO.", headerStyle);
            CreateCell(sheet, i, 40, "RAIL NO.", headerStyle);
            CreateCell(sheet, i, 41, "INVOICE NO", headerStyle);
            CreateCell(sheet, i, 42, "#ID", headerStyle);
            i++;
        }
    }
}

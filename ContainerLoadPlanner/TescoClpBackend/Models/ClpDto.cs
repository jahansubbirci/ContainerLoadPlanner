using ExcelWriterNetFramework;
using SharedEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TescoClpBackend.Models
{
    public class ClpDto
    {
       

        public ClpDto(CfsReport cfsReportItem,PoUploadReportItem poUploadItem)
        {
            //this.cfsReportItem = cfsReportItem;
            //this.poUploadItem = poUploadItem;
            SO = cfsReportItem.So;
            Consignee= cfsReportItem.Consignee;
            Shipper= cfsReportItem.Shipper;
            Lot= cfsReportItem.Lot.ToString();
            Pkgs= cfsReportItem.Pkgs;
            Qty= cfsReportItem.Qty;
            Measurement= cfsReportItem.Measurement;
            Cbm= cfsReportItem.Cbm;
            CWeight = CWeight;
            Destination= cfsReportItem.Destination;
            PO= cfsReportItem.PO;
            Style= cfsReportItem.Style;
            Item= cfsReportItem.Item;
            TpnLc = cfsReportItem.TpnLc;
            RefSize= cfsReportItem.RefSize;
            RmsStyle=cfsReportItem.RmsStyle;
            Category= cfsReportItem.Category;
            Division= cfsReportItem.Division;
            Department= cfsReportItem.Department;

            RCgo = cfsReportItem.RCgo;
            SB= cfsReportItem.SB;
            RecvDoc= cfsReportItem.RecvDoc;
            SbDate= cfsReportItem.SbDate;
            ErdDate= cfsReportItem.ErdDate;
            CustomsHouseCode= cfsReportItem.CustomsHouseCode;
            InvoiceValue= cfsReportItem.InvoiceValue;
            Vat= cfsReportItem.Vat;
            Loc= cfsReportItem.Loc;
            Commodity= cfsReportItem.Commodity;
            EHD = poUploadItem.EHD;
            IDD= poUploadItem.IDD;
            TransportationMode= poUploadItem.TransportationMode;
            Remarks = cfsReportItem.Remarks;
            Overflow= cfsReportItem.Overflow;
            StuffingHeldupRemarks = cfsReportItem.StuffingHeldupRemarks;
            Account = cfsReportItem.Account;
            Forwarder= cfsReportItem.Forwarder;
            Clr= cfsReportItem.Clr;
            Sku= cfsReportItem.Sku;
            StrokeNo= cfsReportItem.StrokeNo;
            RailNo = cfsReportItem.RailNo;
            InvoiceNo = cfsReportItem.InvoiceNo;
            Id= cfsReportItem.Id;

        }

        [ColumnHeader("S/O")]
        public string SO { get;private set; }
        public string Consignee { get; set; }
        public string Shipper { get; set; }
        public string Lot { get; set; }
        public int Pkgs { get; set; }
        public int Qty { get; set; }
        public string Measurement { get; set; }
        public double Cbm { get; set; }
        public double CWeight { get; set; }

        [ColumnHeader("DEST")]
        public string Destination { get; set; }
        public string PO { get; set; }
       

        

        public DateTime EHD { get; set; }
        [ColumnHeader("MODE")]
        public TransportationMode TransportationMode { get; set; }
        public string Style { get; set; }

        public DateTime IDD { get; set; }
        public string Item { get; set; }
        public string TpnLc { get; set; }
        public string RefSize { get; set; }
        public string RmsStyle { get; set; }
        [ColumnHeader("CAT")]
        public string Category { get; set; }
        [ColumnHeader("DIV")]
        public string Division { get; set; }
        [ColumnHeader("DEPT")]
        public string Department { get; set; }

        public DateTime RCgo { get; set; }
        public string SB { get; set; }
        public DateTime RecvDoc { get; set; }
        public DateTime SbDate { get; set; }
        public DateTime ErdDate { get; set; }
        public int CustomsHouseCode { get; set; }
        public double InvoiceValue { get; set; }
        public string Vat { get; set; }
        public string Loc { get; set; }
        public string Commodity { get; set; }
        public string Remarks { get; set; }
        public string Overflow { get; set; }
        public string StuffingHeldupRemarks { get; set; }
        public string Account { get; set; }
        public string Forwarder { get; set; }
        public string Clr { get; set; }
        public string Sku { get; set; }
        public string StrokeNo { get; set; }
        public string RailNo { get; set; }
        public string InvoiceNo { get; set; }
        public string Id { get; set; }
    }
}

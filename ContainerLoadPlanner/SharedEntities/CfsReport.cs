using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedEntities
{
    public enum CFS
    {
        SAPL,
        ISATL,
        VERTEX,
        KDS
    }
    public class CfsReport
    {

        public CfsReport() { }
        public string  So { get; set; }
        public string Consignee { get; set; }
        public string Shipper { get; set; }
        public int Lot { get; set; }
        public int Pkgs { get; set; }
        public int Qty { get; set; }
        public string Measurement { get; set; }
        public double Cbm { get; set; }
        public double CWeight{ get; set; }
        public string Destination { get; set; }
        public string PO { get; set; }
        public string  Style { get; set; }
        public string Item { get; set; }
        public string TpnLc { get; set; }
        public string RefSize { get; set; }
        public string RmsStyle { get; set; }
        public string Category { get; set; }
        public string Division { get; set; }
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
        public string Forwarder { get; set; }
        public string Clr { get; set; }
        public string Sku { get; set; }
        public string StrokeNo { get; set; }
        public string RailNo { get; set; }
        public string InvoiceNo { get; set; }
        public string Id { get; set; }
        public override string ToString()
        {
            return $"{So}\t{Consignee}\t{Shipper}\t{Lot}\t{Pkgs}\t{Qty}\t{Measurement}\t{Cbm}\t{CWeight}\t{Destination}\t{PO}\t{Style}\t{Item}\t";
        }
    }
}

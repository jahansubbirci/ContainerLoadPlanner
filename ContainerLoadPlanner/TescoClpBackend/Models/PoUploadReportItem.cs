using ExcelWriterNetFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TescoClpBackend.Models
{
    public enum TransportationMode
    {
        BDCGP,
        BDDAC
    }
    public class PoUploadReportItem
    {
        [ColumnHeader("PO Number")]
        public string Po { get; set; }
        
        public DateTime EHD { get; set; }
        [ColumnHeader("Place Of Receipt Site")]
        public TransportationMode TransportationMode { get; set; }
        public DateTime IDD { get;  set; }
        public int Priority { get; set; }
    }
}

using SharedEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TescoClpBackend.Models;

namespace TescoClpBackend.ClpLogics
{
    public class ClpItem
    {
        public CfsReport CfsReportItem { get; set; }
        public PoUploadReportItem PoUploadReportItem { get; set; }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (CfsReportItem != null)
            {
                sb.Append(CfsReportItem.ToString());

            }
            if (PoUploadReportItem != null)
            {
                sb.Append(PoUploadReportItem.ToString());
            }
            return sb.ToString();
        }
    }
}

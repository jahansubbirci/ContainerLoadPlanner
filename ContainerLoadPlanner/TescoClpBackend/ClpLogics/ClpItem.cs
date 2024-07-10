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
       
    }
}

using ClpEngine;
using SharedEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TescoClpBackend.Models;

namespace TescoClpBackend.ClpLogics
{
    public class ClpItem : ContainerItem
    {
        private CfsReport cfsReportItem;

        public CfsReport CfsReportItem
        {
            get { return cfsReportItem; }
            set
            {
                cfsReportItem = value;
                this.Cbm = cfsReportItem.Cbm;
                this.CWeight = cfsReportItem.CWeight;
                this.Destination= cfsReportItem.Destination;
            }
        }

        //        public CfsReport CfsReportItem { get; set; }
        public PoUploadReportItem PoUploadReportItem { get; set; }

    }
}

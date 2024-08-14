using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TescoClpBackend.Models
{
    public class ClpEngineSettings
    {
        /// <summary>
        /// PO of Which RECVDOC is blank (Document not received) can or cannot be planned
        /// </summary>
        public bool IgnorePoWithoutDocs { get; set; }

        /// <summary>
        /// in Cutoff situation we may use 40 STD containers 
        /// </summary>
        public bool IsCutOff { get; set; }
        /// <summary>
        /// PO of which LOC are in CV(covered van) can be planned seperately during rush hours
        /// </summary>
        public bool PlanCVPoSeperately { get; set; }
    }
}

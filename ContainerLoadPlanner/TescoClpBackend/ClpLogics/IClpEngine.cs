using SharedEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TescoClpBackend.Models;

namespace TescoClpBackend.ClpLogics
{
    public interface IClpEngine
    {
        Dictionary<string, List<Container<ClpItem>>> Create(IEnumerable<CfsReport> cfsReport,
         IEnumerable<PoUploadReportItem> poUploadReport,
         ClpEngineSettings settings);
    }
}

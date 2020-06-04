using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drillholes.Domain.DataObject
{
    public class AssayTableObject : SurveyTableObject
    {
        public new SummaryAssayStatistics SummaryStats { get; set; }

        public string assayKey { get; set; }
    }
}

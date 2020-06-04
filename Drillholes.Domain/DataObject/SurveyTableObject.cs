using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drillholes.Domain.DataObject
{
    public class SurveyTableObject : CollarTableObject
    {
        public string surveyKey { get; set; }
        public new SummarySurveyStatistics SummaryStats { get; set; }

    }
}

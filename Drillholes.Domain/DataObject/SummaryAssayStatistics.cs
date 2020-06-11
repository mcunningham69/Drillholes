using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drillholes.Domain.DataObject
{
    public class SummaryAssayStatistics : SummarySurveyStatistics
    {
        public int AssayCount { get; set; }
        public int MinAssayCount { get; set; }
        public int MaxAssayCount { get; set; }
        public double AverageAssayCount { get; set; }
        public double MinAssayLength { get; set; }
        public double MaxAssayLength { get; set; }
        public double AverageAssayLength { get; set; }

        public SummaryAssayStatistics() : base()
        {

            AssayCount = 0;
            MinAssayCount = 0;
            MaxAssayCount = 0;
            AverageAssayCount = 0.0;
            MinAssayLength = 0.0;
            MaxAssayLength = 0.0;
            AverageAssayLength = 0.0;

            //TODO - add count for most (1) frequent occurring, and (2) length weighted

        }
    }
}

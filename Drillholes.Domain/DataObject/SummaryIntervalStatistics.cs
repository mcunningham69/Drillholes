using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drillholes.Domain.DataObject
{
    public class SummaryIntervalStatistics : SummaryAssayStatistics
    {
        public int IntervalCount { get; set; }
        public int MinIntervalCount { get; set; }
        public int MaxIntervalCount { get; set; }
        public double AverageIntervalCount { get; set; }
        public double MinIntervalLength { get; set; }
        public double MaxIntervalLength { get; set; }
        public double AverageIntervalLength { get; set; }
        public SummaryIntervalStatistics() : base()
        {
            IntervalCount = 0;
            MinIntervalCount = 0;
            MaxIntervalCount = 0;
            AverageIntervalCount = 0.0;
            MinIntervalLength = 0.0;
            MaxIntervalLength = 0.0;
            AverageIntervalLength = 0.0;

            //TODO - add count for most (1) frequent occurring, and (2) length weighted

        }
    }
}

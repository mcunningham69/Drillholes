using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drillholes.Domain.DataObject
{
    public class SummaryContinuousStatistics : SummaryIntervalStatistics
    {
        public int ValueCount { get; set; }
        public int MinValueCount { get; set; }
        public int MaxValueCount { get; set; }
        public double AverageValueCount { get; set; }
        public double MinValueLength { get; set; }
        public double MaxValueLength { get; set; }
        public double AverageValueLength { get; set; }
        public SummaryContinuousStatistics() : base()
        {
            ValueCount = 0;
            MinValueCount = 0;
            MaxValueCount = 0;
            AverageValueCount = 0.0;
            MinValueLength = 0.0;
            MaxValueLength = 0.0;
            AverageValueLength = 0.0;

            //TODO - add count for most (1) frequent occurring, and (2) length weighted

        }
    }
}

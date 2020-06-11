using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drillholes.Domain.DTO
{
    public class SummaryIntervalStatisticsDto : SummaryAssayStatisticsDto
    {

        public int IntervalCount { get; set; }
        public int MinIntervalCount { get; set; }
        public int MaxIntervalCount { get; set; }
        public double AverageIntervalCount { get; set; }
        public double MinIntervalLength { get; set; }
        public double MaxIntervalLength { get; set; }
        public double AverageIntervalLength { get; set; }
     
    }
}

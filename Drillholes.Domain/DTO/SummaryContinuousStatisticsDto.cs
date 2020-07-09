using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drillholes.Domain.DTO
{
    public class SummaryContinuousStatisticsDto : SummaryIntervalStatisticsDto
    {
        public int ValueCount { get; set; }
        public int MinValueCount { get; set; }
        public int MaxValueCount { get; set; }
        public double AverageValueCount { get; set; }
        public double MinValueLength { get; set; }
        public double MaxValueLength { get; set; }
        public double AverageValueLength { get; set; }

    }
}

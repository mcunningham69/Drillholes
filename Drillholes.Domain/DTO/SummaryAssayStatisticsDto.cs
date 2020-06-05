using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drillholes.Domain.DTO
{
    public class SummaryAssayStatisticsDto : SummarySurveyStatisticsDto
    {
        public int AssayCount { get; set; }
        public int MinAssayCount { get; set; }
        public int MaxAssayCount { get; set; }
        public double AverageAssayCount { get; set; }
        public double MinAssayLength { get; set; }
        public double MaxAssayLength { get; set; }
        public double AverageAssayLength { get; set; }

    }
}

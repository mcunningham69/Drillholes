using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drillholes.Domain.DTO
{
    public class SummarySurveyStatisticsDto : SummaryCollarStatisticsDto
    {
        public int surveyCount { get; set; }
        public int MinSurveyCount { get; set; }
        public int MaxSurveyCount { get; set; }
        public double AverageSurveyCount { get; set; }
        public double MinSurveyLength { get; set; }
        public double MaxSurveyLength { get; set; }
        public double AverageSurveyLength { get; set; }
        public new double MinimumDip { get; set; }
        public new double AverageDip { get; set; }
        public new double MaximumDip { get; set; }
        public double MinDipDir { get; set; }
        public double MaxDipDir { get; set; }
        public double AverageDipDir { get; set; }

    }
}

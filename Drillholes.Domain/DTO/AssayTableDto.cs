using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drillholes.Domain.DTO
{
    public class AssayTableDto : SurveyTableDto
    {
        public string assayKey { get; set; }
        public new SummaryAssayStatistics SummaryStats { get; set; }


    }
}

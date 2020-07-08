using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drillholes.Domain.DTO
{
    public class ContinuousTableDto : IntervalTableDto
    {
       // public new SummaryIntervalStatistics SummaryStats { get; set; }

        public string continuousKey { get; set; }
    }
}

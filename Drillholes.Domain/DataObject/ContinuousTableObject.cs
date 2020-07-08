using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drillholes.Domain.DataObject
{
    public class ContinuousTableObject : IntervalTableObject
    {
        public new SummaryIntervalStatistics SummaryStats { get; set; }

        public string continuousKey { get; set; }
    }
}

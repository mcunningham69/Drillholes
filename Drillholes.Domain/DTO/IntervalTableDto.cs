﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drillholes.Domain.DTO
{
    public class IntervalTableDto : AssayTableDto
    {
       // public new SummaryIntervalStatistics SummaryStats { get; set; }

        public string intervalKey { get; set; }
    }
}

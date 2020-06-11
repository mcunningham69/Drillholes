using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Drillholes.Domain.DTO;

namespace Drillholes.Domain.Interfaces
{
    public interface IIntervalStatistics
    {
        Task<SummaryIntervalStatisticsDto> SummaryStatistics(List<ImportTableField> fields, XElement intervalValues);
    }
}

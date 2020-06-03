using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Drillholes.Domain;
using Drillholes.Domain.DTO;
using Drillholes.Domain.Enum;
using Drillholes.Domain.Interfaces;


namespace Drillholes.Validation.Statistics
{
    public class IntervalStatistics : IIntervalStatistics
    {
        public Task<IntervalTableDto> SummaryStatistics(List<ImportTableField> fields, XElement intervalValues, string tableName, string tableLocation, DrillholeImportFormat tableFormat)
        {
            throw new NotImplementedException();
        }
    }
}

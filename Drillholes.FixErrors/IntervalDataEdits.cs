using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Drillholes.Domain;
using Drillholes.Domain.DTO;
using Drillholes.Domain.Interfaces;

namespace Drillholes.FixErrors
{
    public class IntervalDataEdits : IIntervalEdit
    {
        public Task<IntervalTableDto> UpdateValues(List<RowsToEdit> rowsToEdit, XElement intervalValues)
        {
            throw new NotImplementedException();
        }
    }
}

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
    public class ContinuousDataEdits : IContinuousEdit
    {
        public Task<ContinuousTableDto> UpdateValues(List<RowsToEdit> rowsToEdit, XElement continuousValues)
        {
            throw new NotImplementedException();
        }
    }
}

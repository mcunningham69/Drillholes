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
    public class AssayDataEdits : IAssayEdit
    {
        public Task<AssayTableDto> UpdateValues(List<RowsToEdit> rowsToEdit, XElement assayValues)
        {
            throw new NotImplementedException();
        }
    }
}

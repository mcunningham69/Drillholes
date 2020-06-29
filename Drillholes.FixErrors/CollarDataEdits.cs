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
    public class CollarDataEdits : ICollarEdit
    {
        public Task<CollarTableDto> UpdateValues(List<RowsToEdit> RowsToEdit, XElement collarValues, List<ImportTableField> editFields, bool bIgnore)
        {
            throw new NotImplementedException();
        }
    }
}

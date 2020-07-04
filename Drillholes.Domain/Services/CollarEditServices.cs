using Drillholes.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Drillholes.Domain.DTO;
using Drillholes.Domain.DataObject;
using System.Xml.Linq;

namespace Drillholes.Domain.Services
{
    public class CollarEditServices
    {
        private readonly ICollarEdit _edits;

        public CollarEditServices(ICollarEdit edits)
        {
            this._edits = edits;
        }

        public async Task<CollarTableObject> UpdateValues(IMapper mapper, List<RowsToEdit> RowsToEdit, XElement collarValues, List<ImportTableField>editFields)
        {

            var collarEdits = await _edits.UpdateValues(RowsToEdit, collarValues, editFields);

            return mapper.Map<CollarTableDto, CollarTableObject>(collarEdits);
        }
    }
}

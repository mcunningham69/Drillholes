using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Drillholes.Domain.DTO;
using Drillholes.Domain.DataObject;
using System.Xml.Linq;
using Drillholes.Domain.Interfaces;

namespace Drillholes.Domain.Services
{
    public class AssayEditServices
    {
        private readonly IAssayEdit _edits;

        public AssayEditServices(IAssayEdit edits)
        {
            this._edits = edits;
        }

        public async Task<AssayTableObject> UpdateValues(IMapper mapper, List<RowsToEdit> RowsToEdit, XElement assayValues)
        {

            var assayEdits = await _edits.UpdateValues(RowsToEdit, assayValues);

            return mapper.Map<AssayTableDto, AssayTableObject>(assayEdits);
        }
    }
}

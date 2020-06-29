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
    public class IntervalEditServices
    {
        private readonly IIntervalEdit _edits;

        public IntervalEditServices(IIntervalEdit edits)
        {
            this._edits = edits;
        }

        public async Task<IntervalTableObject> UpdateValues(IMapper mapper, List<RowsToEdit> RowsToEdit, XElement intervalValues)
        {

            var intervalEdits = await _edits.UpdateValues(RowsToEdit, intervalValues);

            return mapper.Map<IntervalTableDto, IntervalTableObject>(intervalEdits);
        }
    }
}

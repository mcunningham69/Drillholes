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
    public class ContinuousEditServices
    {
        private readonly IContinuousEdit _edits;

        public ContinuousEditServices(IContinuousEdit edits)
        {
            this._edits = edits;
        }

        public async Task<ContinuousTableObject> UpdateValues(IMapper mapper, List<RowsToEdit> RowsToEdit, XElement continuousValues)
        {

            var intervalEdits = await _edits.UpdateValues(RowsToEdit, continuousValues);

            return mapper.Map<ContinuousTableDto, ContinuousTableObject>(intervalEdits);
        }
    }
}

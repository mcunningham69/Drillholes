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
    public class SurveyEditServices
    {
        private readonly ISurveyEdit _edits;

        public SurveyEditServices(ISurveyEdit edits)
        {
            this._edits = edits;
        }

        public async Task<SurveyTableObject> UpdateValues(IMapper mapper, List<RowsToEdit> RowsToEdit, XElement surveyValues)
        {

            var surveyEdits = await _edits.UpdateValues(RowsToEdit, surveyValues);

            return mapper.Map<SurveyTableDto, SurveyTableObject>(surveyEdits);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drillholes.Domain.Interfaces;
using Drillholes.Domain.DTO;
using Drillholes.Domain.Exceptions;
using Drillholes.Domain.DataObject;
using AutoMapper;
using System.Xml.Linq;

namespace Drillholes.Domain.Services
{
    public class SurveyStatisticsService
    {
        private readonly ISurveyStatistics _survey;
        private SurveyTableDto surveyDto = null;
        public SurveyStatisticsService(ISurveyStatistics survey)
        {
            this._survey = survey;
        }

        public async Task<SurveyTableObject> SummaryStatistics(IMapper mapper, List<ImportTableField> fields,
            XElement surveyValues)
        {
            surveyDto = await _survey.SummaryStatistics(fields, surveyValues);

            if (surveyDto.isValid == false)
            {
                throw new SurveyStatisticsException("Issue with calculating Survey statistics");
            }

            return mapper.Map<SurveyTableDto, SurveyTableObject>(surveyDto);
        }
    }
}


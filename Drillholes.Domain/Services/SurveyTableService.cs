using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drillholes.Domain.Interfaces;
using Drillholes.Domain.DTO;
using Drillholes.Domain.DataObject;
using Drillholes.Domain.Exceptions;
using Drillholes.Domain.Enum;
using AutoMapper;

namespace Drillholes.Domain.Services
{
    public class SurveyTableService
    {
        private readonly ISurveyTable _survey;

        private SurveyTableDto surveyDto = null;

        public SurveyTableService(ISurveyTable survey)
        {
            this._survey = survey;
        }

        public async Task<SurveyTableObject> UpdateFieldvalues(string previousSelection, IMapper mapper, string changeTo, string searchColumn, string strOldName, ImportTableFields surveyTableFields)
        {
            surveyDto = await _survey.UpdateImportParameters(previousSelection, changeTo, searchColumn, strOldName, surveyTableFields);

            if (surveyDto.tableIsValid == false)
            {
                throw new SurveyException("Issue with updating Survey fields");
            }

            return mapper.Map<SurveyTableDto, SurveyTableObject>(surveyDto);
        }

        public async Task<SurveyTableObject> GetSurveyFields(IMapper mapper, string tablePath, 
            DrillholeImportFormat tableFormat, string tableName)
        {
            surveyDto = await _survey.RetrieveTableFieldnames(tableFormat, tablePath, tableName);

            if (surveyDto.tableIsValid == false)
            {
                throw new SurveyException("Issue with Survey table");
            }

            return mapper.Map<SurveyTableDto, SurveyTableObject>(surveyDto);

        }

        public async Task<SurveyTableObject> PreviewData(IMapper mapper, DrillholeTableType tableType, int limit)
        {
            surveyDto = await _survey.PreviewAndImportFields(tableType, limit);

            if (surveyDto.tableIsValid == false)
            {
                throw new SurveyException("Issue with Survey preview table");
            }

            return mapper.Map<SurveyTableDto, SurveyTableObject>(surveyDto);

        }

        public async Task<SurveyTableObject> ImportAllFieldsAsGeneric(IMapper mapper, bool bImport)
        {
            surveyDto = await _survey.ImportAllFieldsAsGeneric(bImport);

            if (surveyDto.tableIsValid == false)
            {
                throw new SurveyException("Issue with importing all fields for Survey table");
            }

            return mapper.Map<SurveyTableDto, SurveyTableObject>(surveyDto);
        }

        public List<string> ReturnFields(IMapper mapper)
        {
            mapper.Map<SurveyTableDto, SurveyTableObject>(surveyDto);
            return surveyDto.fields;

        }

    }
}

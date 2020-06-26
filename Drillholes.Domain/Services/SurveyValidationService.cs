using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Drillholes.Domain.Interfaces;
using Drillholes.Domain.DataObject;
using Drillholes.Domain.DTO;
using AutoMapper;

namespace Drillholes.Domain.Services
{
    public class SurveyValidationService
    {
        private readonly ISurveyValidation _validation;

        public SurveyValidationService(ISurveyValidation validation)
        {
            this._validation = validation;
        }
        public async Task<ValidationSurvey> CheckIsEmpty(IMapper mapper, ValidationMessages ValuesToCheck, XElement surveyValues)
        {
            var surveyValidate = await _validation.CheckIsEmpty(ValuesToCheck, surveyValues);

            return mapper.Map<ValidationSurveyDto, ValidationSurvey>(surveyValidate);
        }


        //all tables - coordinates, distance and survey
        public async Task<ValidationSurvey> CheckForNumeric(IMapper mapper, ValidationMessages ValuesToCheck, XElement surveyValues)
        {

            var surveyValidate = await _validation.CheckForNumeric(ValuesToCheck, surveyValues);

            return mapper.Map<ValidationSurveyDto, ValidationSurvey>(surveyValidate);

        }

        //survey file
        public async Task<ValidationSurvey> CheckSurveyDistance(IMapper mapper, ValidationMessages ValuesToCheck, List<XElement> drillholeValues)
        {
            var surveyValidate = await _validation.CheckSurveyDistance(ValuesToCheck, drillholeValues);

            return mapper.Map<ValidationSurveyDto, ValidationSurvey>(surveyValidate);
        }

        //azimuth and dip

        public async Task<ValidationSurvey> CheckRange(IMapper mapper, ValidationMessages ValuesToCheck, XElement surveyValues)
        {

            var surveyValidate = await _validation.CheckRange(ValuesToCheck, surveyValues);

            return mapper.Map<ValidationSurveyDto, ValidationSurvey>(surveyValidate);
        }

        public async Task<ValidationSurvey> CheckDeviations(IMapper mapper, ValidationMessages ValuesToCheck, XElement surveyValues, int dipTolerance, int aziTolerance)
        {

            var surveyValidate = await _validation.CheckForLargeDeviations(ValuesToCheck, surveyValues, dipTolerance, aziTolerance);

            return mapper.Map<ValidationSurveyDto, ValidationSurvey>(surveyValidate);
        }

        public async Task<ValidationSurvey> CheckMissingCollars(IMapper mapper, ValidationMessages ValuesToCheck, List<XElement> drillholeValues)
        {
            var surveyValidate = await _validation.CheckForMissingCollars(ValuesToCheck, drillholeValues);

            return mapper.Map<ValidationSurveyDto, ValidationSurvey>(surveyValidate);
        }

        //look for duplicates in collar
        public async Task<ValidationSurvey> CheckDuplicates(IMapper mapper, ValidationMessages ValuesToCheck, XElement surveyValues)
        {
            var surveyValidate = await _validation.CheckForDuplicates(ValuesToCheck, surveyValues);

            return mapper.Map<ValidationSurveyDto, ValidationSurvey>(surveyValidate);
        }


        public async Task<ValidationSurvey> CheckMaxDepth(IMapper mapper, ValidationMessages ValuesToCheck, List<XElement> drillholeValues)
        {
            var surveyValidate = await _validation.CheckMaxDepth(ValuesToCheck, drillholeValues);

            return mapper.Map<ValidationSurveyDto, ValidationSurvey>(surveyValidate);
        }
    }
}

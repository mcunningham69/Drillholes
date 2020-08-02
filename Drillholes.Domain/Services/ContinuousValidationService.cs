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
    public class ContinuousValidationService
    {
        private readonly IContinuousValidation _validation;

        public ContinuousValidationService(IContinuousValidation validation)
        {
            this._validation = validation;
        }
        public async Task<ValidationContinuous> CheckIsEmpty(IMapper mapper, ValidationMessages ValuesToCheck, XElement continuousValues)
        {
            var continuousValidate = await _validation.CheckIsEmpty(ValuesToCheck, continuousValues);

            return mapper.Map<ValidationContinuousDto, ValidationContinuous>(continuousValidate);
        }


        //all tables - coordinates, distance and survey
        public async Task<ValidationContinuous> CheckForNumeric(IMapper mapper, ValidationMessages ValuesToCheck, XElement continuousValues)
        {

            var continuousValidate = await _validation.CheckForNumeric(ValuesToCheck, continuousValues);

            return mapper.Map<ValidationContinuousDto, ValidationContinuous>(continuousValidate);

        }

        //survey file
        public async Task<ValidationContinuous> CheckDistance(IMapper mapper, ValidationMessages ValuesToCheck, List<XElement> drillholeValues)
        {
            var continuousValidate = await _validation.CheckDistance(ValuesToCheck, drillholeValues);

            return mapper.Map<ValidationContinuousDto, ValidationContinuous>(continuousValidate);
        }


        public async Task<ValidationContinuous> CheckMissingCollars(IMapper mapper, ValidationMessages ValuesToCheck, List<XElement> drillholeValues)
        {
            var continuousValidate = await _validation.CheckForMissingCollars(ValuesToCheck, drillholeValues);

            return mapper.Map<ValidationContinuousDto, ValidationContinuous>(continuousValidate);
        }

        //look for duplicates in collar
        public async Task<ValidationContinuous> CheckDuplicates(IMapper mapper, ValidationMessages ValuesToCheck, XElement continuousValues)
        {
            var continuousValidate = await _validation.CheckForDuplicates(ValuesToCheck, continuousValues);

            return mapper.Map<ValidationContinuousDto, ValidationContinuous>(continuousValidate);
        }


        public async Task<ValidationContinuous> CheckMaxDepth(IMapper mapper, ValidationMessages ValuesToCheck, List<XElement> drillholeValues)
        {
            var continuousValidate = await _validation.CheckMaxDepth(ValuesToCheck, drillholeValues);

            return mapper.Map<ValidationContinuousDto, ValidationContinuous>(continuousValidate);
        }

        public async Task<ValidationContinuous> CheckStructures(IMapper mapper, ValidationMessages ValuesToCheck, XElement continuousValues)
        {
            var continuousValidate = await _validation.CheckStructuralMeasurements(ValuesToCheck, continuousValues);

            return mapper.Map<ValidationContinuousDto, ValidationContinuous>(continuousValidate);
        }
    }
}

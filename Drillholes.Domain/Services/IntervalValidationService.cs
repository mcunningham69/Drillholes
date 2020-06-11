using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Drillholes.Domain.Interfaces;
using Drillholes.Domain.DataObject;
using AutoMapper;
using Drillholes.Domain.DTO;

namespace Drillholes.Domain.Services
{
    public class IntervalValidationService
    {
        private readonly IIntervalValidation _validation;

        public IntervalValidationService(IIntervalValidation validation)
        {
            this._validation = validation;
        }

        public async Task<ValidationInterval> CheckIsEmpty(IMapper mapper, ValidationMessages ValuesToCheck, XElement drillholeValues)
        {

            var validateValues = await _validation.CheckIsEmpty(ValuesToCheck, drillholeValues);

            return mapper.Map<ValidationIntervalDto, ValidationInterval>(validateValues);
        }


        //all tables - coordinates, distance and survey
        public async Task<ValidationInterval> CheckForNumeric(IMapper mapper, ValidationMessages ValuesToCheck, XElement drillholeValues)
        {

            var validateValues = await _validation.CheckForNumeric(ValuesToCheck, drillholeValues);

            return mapper.Map<ValidationIntervalDto, ValidationInterval>(validateValues);

        }


        //distance fields
        public async Task<ValidationInterval> CheckMissingIntervals(IMapper mapper, ValidationMessages ValuesToCheck, List<XElement> drillholeValues)
        {

            var validateValues = await _validation.CheckMissingIntervals(ValuesToCheck, drillholeValues);

            return mapper.Map<ValidationIntervalDto, ValidationInterval>(validateValues);
        }

        public async Task<ValidationInterval> CheckNegativeIntervals(IMapper mapper, ValidationMessages ValuesToCheck, XElement intervalValues)
        {

            var validateValues = await _validation.CheckNegativeIntervals(ValuesToCheck, intervalValues);

            return mapper.Map<ValidationIntervalDto, ValidationInterval>(validateValues);
        }
        public async Task<ValidationInterval> CheckOverlappingIntervals(IMapper mapper, ValidationMessages ValuesToCheck, XElement intervalValues)
        {

            var validateValues = await _validation.CheckOverlappingIntervals(ValuesToCheck, intervalValues);

            return mapper.Map<ValidationIntervalDto, ValidationInterval>(validateValues);
        }

        public async Task<ValidationInterval> CheckMissingCollars(IMapper mapper, ValidationMessages ValuesToCheck, List<XElement> drillholeValues)
        {

            var validateValues = await _validation.CheckForMissingCollars(ValuesToCheck, drillholeValues);
            return mapper.Map<ValidationIntervalDto, ValidationInterval>(validateValues);
        }

        //look for duplicates in collar
        public async Task<ValidationInterval> CheckDuplicates(IMapper mapper, ValidationMessages ValuesToCheck, XElement assayValues)
        {


            var validateValues = await _validation.CheckForDuplicates(ValuesToCheck, assayValues);


            return mapper.Map<ValidationIntervalDto, ValidationInterval>(validateValues);
        }


        public async Task<ValidationInterval> CheckMaxDepth(IMapper mapper, ValidationMessages ValuesToCheck, List<XElement> drillholeValues)
        {

            var validateValues = await _validation.CheckMaxDepth(ValuesToCheck, drillholeValues);

            return mapper.Map<ValidationIntervalDto, ValidationInterval>(validateValues);
        }

    }

}

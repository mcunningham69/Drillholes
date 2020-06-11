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
    public class AssayValidationService
    {
        private readonly IAssayValidation _validation;

        public AssayValidationService(IAssayValidation validation)
        {
            this._validation = validation;
        }

        public async Task<ValidationAssay> CheckIsEmpty(IMapper mapper, ValidationMessages ValuesToCheck, XElement drillholeValues)
        {

            var validateValues = await _validation.CheckIsEmpty(ValuesToCheck, drillholeValues);

            return mapper.Map<ValidationAssayDto, ValidationAssay>(validateValues);
        }


        //all tables - coordinates, distance and survey
        public async Task<ValidationAssay> CheckForNumeric(IMapper mapper, ValidationMessages ValuesToCheck, XElement drillholeValues)
        {

            var validateValues = await _validation.CheckForNumeric(ValuesToCheck, drillholeValues);

            return mapper.Map<ValidationAssayDto, ValidationAssay>(validateValues);

        }



        //distance fields
        public async Task<ValidationAssay> CheckMissingIntervals(IMapper mapper, ValidationMessages ValuesToCheck, List<XElement> drillholeValues)
        {

            var validateValues = await _validation.CheckMissingIntervals(ValuesToCheck, drillholeValues);

            return mapper.Map<ValidationAssayDto, ValidationAssay>(validateValues);
        }

        public async Task<ValidationAssay> CheckNegativeIntervals(IMapper mapper, ValidationMessages ValuesToCheck, XElement intervalValues)
        {

            var validateValues = await _validation.CheckNegativeIntervals(ValuesToCheck, intervalValues);

            return mapper.Map<ValidationAssayDto, ValidationAssay>(validateValues);
        }
        public async Task<ValidationAssay> CheckOverlappingIntervals(IMapper mapper, ValidationMessages ValuesToCheck, XElement intervalValues)
        {

            var validateValues = await _validation.CheckOverlappingIntervals(ValuesToCheck, intervalValues);

            return mapper.Map<ValidationAssayDto, ValidationAssay>(validateValues);
        }

        public async Task<ValidationAssay> CheckMissingCollars(IMapper mapper, ValidationMessages ValuesToCheck, List<XElement> drillholeValues)
        {

            var validateValues = await _validation.CheckForMissingCollars(ValuesToCheck, drillholeValues);
            return mapper.Map<ValidationAssayDto, ValidationAssay>(validateValues);
        }

        //look for duplicates in collar
        public async Task<ValidationAssay> CheckDuplicates(IMapper mapper, ValidationMessages ValuesToCheck, XElement assayValues)
        {


            var validateValues = await _validation.CheckForDuplicates(ValuesToCheck, assayValues);


            return mapper.Map<ValidationAssayDto, ValidationAssay>(validateValues);
        }


        public async Task<ValidationAssay> CheckMaxDepth(IMapper mapper, ValidationMessages ValuesToCheck, List<XElement> drillholeValues)
        {

            var validateValues = await _validation.CheckMaxDepth(ValuesToCheck, drillholeValues);

            return mapper.Map<ValidationAssayDto, ValidationAssay>(validateValues);
        }

    }

}

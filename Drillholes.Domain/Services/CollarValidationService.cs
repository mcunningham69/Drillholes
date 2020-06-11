using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Drillholes.Domain.Interfaces;
using Drillholes.Domain.Enum;
using Drillholes.Domain.DTO;
using Drillholes.Domain.DataObject;
using AutoMapper;

namespace Drillholes.Domain.Services
{
    public class CollarValidationService
    {
        private readonly ICollarValidation _validation;

        public CollarValidationService(ICollarValidation validation)
        {
            this._validation = validation;
        }

        public async Task<ValidationCollar> CheckIsEmpty(IMapper mapper, ValidationMessages ValuesToCheck, XElement collarValues)
        {

            var collarValidate = await _validation.CheckIsEmpty(ValuesToCheck, collarValues);

            return mapper.Map<ValidationCollarDto, ValidationCollar>(collarValidate);
        }


        //all tables - coordinates, distance and survey
        public async Task<ValidationCollar> CheckIsNumeric(IMapper mapper, ValidationMessages ValuesToCheck, XElement collarValues)
        {

            var collarValidate = await _validation.CheckForNumeric(ValuesToCheck, collarValues);

            return mapper.Map<ValidationCollarDto, ValidationCollar>(collarValidate);

        }

        //look for duplicates in collar
        public async Task<ValidationCollar> CheckDuplicates(IMapper mapper, ValidationMessages ValuesToCheck, XElement collarValues)
        {
            var collarValidate = await _validation.CheckForDuplicates(ValuesToCheck, collarValues);

            return mapper.Map<ValidationCollarDto, ValidationCollar>(collarValidate);
        }


        public async Task<ValidationCollar> CheckMaxDepth(IMapper mapper, ValidationMessages ValuesToCheck, XElement collarValues)
        {

            var collarValidate = await _validation.CheckMaxDepth(ValuesToCheck, collarValues);
            return mapper.Map<ValidationCollarDto, ValidationCollar>(collarValidate);
        }


        //azimuth and dip

        public async Task<ValidationCollar> CheckRange(IMapper mapper, ValidationMessages ValuesToCheck, XElement drillholeValues)
        {

            var collarValidate = await _validation.CheckRange(ValuesToCheck, drillholeValues);
            return mapper.Map<ValidationCollarDto, ValidationCollar>(collarValidate);
        }
        public async Task<ValidationCollar> CheckZeroCoordinate(IMapper mapper, ValidationMessages ValuesToCheck, XElement collarValues)
        {

            var collarValidate = await _validation.CheckForZeroCoordinate(ValuesToCheck, collarValues);
            return mapper.Map<ValidationCollarDto, ValidationCollar>(collarValidate);
        }

    }
}

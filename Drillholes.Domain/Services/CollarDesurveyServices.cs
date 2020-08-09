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
using System.Xml.Linq;

namespace Drillholes.Domain.Services
{
    public class CollarDesurveyServices
    {
        private readonly IDesurveyDrillhole _drillhole;


        public CollarDesurveyServices(IDesurveyDrillhole drillhole)
        {
            this._drillhole = drillhole;
        }

        public async Task<CollarDesurveyObject> CollarVerticalHole(IMapper mapper, DrillholeDesurveyEnum desurveyType, ImportTableFields tableFields, bool bToe, XElement collarData)
        {
            var collarDesurvDto = await _drillhole.CreateCollarVerticalHole(desurveyType, tableFields, bToe, collarData) as CollarDesurveyDto;

            if (collarDesurvDto.IsValid == false)
            {
                throw new CollarException("Issue with desurvey vertical collar data");
            }

            return mapper.Map<CollarDesurveyDto, CollarDesurveyObject>(collarDesurvDto);
        }

        public async Task<CollarDesurveyObject> CollarSurveyHole(IMapper mapper, DrillholeDesurveyEnum desurveyType, ImportTableFields tableFields, bool bToe, XElement collarData)
        {
            var desurvDto = await _drillhole.CreateCollarSurveyHole(desurveyType, tableFields, bToe, collarData) as CollarDesurveyDto;

            if (desurvDto.IsValid == false)
            {
                throw new CollarException("Issue with desurvey collar data");
            }

            return mapper.Map<CollarDesurveyDto, CollarDesurveyObject>(desurvDto);
        }

    }
}

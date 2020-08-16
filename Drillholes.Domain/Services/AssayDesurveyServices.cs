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
    public class AssayDesurveyServices
    {
        private readonly IDesurveyDrillhole _drillhole;


        public AssayDesurveyServices(IDesurveyDrillhole drillhole)
        {
            this._drillhole = drillhole;
        }

      
        public async Task<AssayDesurveyObject> AssayVerticalHole(IMapper mapper, DrillholeDesurveyEnum desurveyType, ImportTableFields collarTableFields, ImportTableFields assayTableFields, bool bToe, bool bCollar, List<XElement> drillholeValues)
        {
            var desurvDto = await _drillhole.CreateAssayVerticalHole(desurveyType, collarTableFields, assayTableFields, bToe, bCollar, drillholeValues) as AssayDesurveyDto;

            if (desurvDto.IsValid == false)
            {
                throw new AssayException("Issue with desurvey vertical assay data");
            }

            return mapper.Map<AssayDesurveyDto, AssayDesurveyObject>(desurvDto);
        }

        public async Task<AssayDesurveyObject> AssaySurveyHole(IMapper mapper, DrillholeDesurveyEnum desurveyType, ImportTableFields collarTableFields, 
            ImportTableFields assayTableFields, bool bToe, bool bCollar, List<XElement> drillholeValues)
        {
            var desurvDto = await _drillhole.CreateAssaySurveyHole(desurveyType, collarTableFields, assayTableFields, bToe, bCollar, drillholeValues) as AssayDesurveyDto; //TODO update interface

            if (desurvDto.IsValid == false)
            {
                throw new AssayException("Issue with desurvey assay data");
            }

            return mapper.Map<AssayDesurveyDto, AssayDesurveyObject>(desurvDto);
        }

        public async Task<AssayDesurveyObject> AssayDownhole(IMapper mapper, DrillholeDesurveyEnum desurveyType, ImportTableFields collarTableFields,
            ImportTableFields assayTableFields, ImportTableFields surveyTableFields, bool bToe, bool bCollar, List<XElement> drillholeValues)
        {
            var desurvDto = await _drillhole.CreateAssayDownhole(desurveyType, collarTableFields, assayTableFields, surveyTableFields, bToe, bCollar, drillholeValues) as AssayDesurveyDto; //TODO update interface

            if (desurvDto.IsValid == false)
            {
                throw new AssayException("Issue with desurvey downhole assay data");
            }

            return mapper.Map<AssayDesurveyDto, AssayDesurveyObject>(desurvDto);
        }


    }
}

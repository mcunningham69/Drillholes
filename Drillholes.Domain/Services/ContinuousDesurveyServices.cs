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
    public class ContinuousDesurveyServices
    {
        private readonly IDesurveyDrillhole _drillhole;


        public ContinuousDesurveyServices(IDesurveyDrillhole drillhole)
        {
            this._drillhole = drillhole;
        }

        public async Task<ContinuousDesurveyObject> ContinuousVerticalHole(IMapper mapper, DrillholeDesurveyEnum desurveyType, ImportTableFields collarTableFields, ImportTableFields continuousFields, bool bToe, bool bCollar, List<XElement> drillholeValues)
        {
            var desurvDto = await _drillhole.CreateContinuousVerticalHole(desurveyType, collarTableFields, continuousFields, bToe, bCollar, drillholeValues) as ContinuousDesurveyDto;

            if (desurvDto.IsValid == false)
            {
                throw new ContinuousException("Issue with desurvey vertical continuous data");
            }

            return mapper.Map<ContinuousDesurveyDto, ContinuousDesurveyObject>(desurvDto);
        }

        public async Task<ContinuousDesurveyObject> ContinuousSurveyHole(IMapper mapper, DrillholeDesurveyEnum desurveyType, ImportTableFields collarTableFields, ImportTableFields contTableFields,
            bool bToe, bool bCollar, List<XElement> drillholeValues, bool bTop)
        {
            var desurvDto = await _drillhole.CreateContinuousSurveyHole(desurveyType, collarTableFields, contTableFields, bToe, bCollar, drillholeValues, bTop) as ContinuousDesurveyDto;

            if (desurvDto.IsValid == false)
            {
                throw new ContinuousException("Issue with desurvey continuous survey data");
            }

            return mapper.Map<ContinuousDesurveyDto, ContinuousDesurveyObject>(desurvDto);
        }

        public async Task<ContinuousDesurveyObject> ContinuousDownhole(IMapper mapper, DrillholeDesurveyEnum desurveyType, ImportTableFields collarTableFields, ImportTableFields contTableFields,
            ImportTableFields surveyTableFields, bool bToe, bool bCollar, List<XElement> drillholeValues, bool bBottom)
        {
            var desurvDto = await _drillhole.CreateContinuousDownhole(desurveyType, collarTableFields, contTableFields, surveyTableFields, bToe, bCollar, drillholeValues, bBottom) as ContinuousDesurveyDto;

            if (desurvDto.IsValid == false)
            {
                throw new ContinuousException("Issue with desurvey downhole continuous data");
            }

            return mapper.Map<ContinuousDesurveyDto, ContinuousDesurveyObject>(desurvDto);
        }

    }
}

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
    public class IntervalDesurveyServices
    {
        private readonly IDesurveyDrillhole _drillhole;


        public IntervalDesurveyServices(IDesurveyDrillhole drillhole)
        {
            this._drillhole = drillhole;
        }

     
        public async Task<IntervalDesurveyObject> IntervalVerticalHole(IMapper mapper, DrillholeDesurveyEnum desurveyType, ImportTableFields collarTableFields, ImportTableFields intervalFields, bool bToe, bool bCollar, List<XElement> drillholeValues)
        {
            var desurvDto = await _drillhole.CreateIntervalVerticalHole(desurveyType, collarTableFields, intervalFields, bToe, bCollar, drillholeValues) as IntervalDesurveyDto;

            if (desurvDto.IsValid == false)
            {
                throw new IntervalException("Issue with desurvey vertical interval data");
            }

            return mapper.Map<IntervalDesurveyDto, IntervalDesurveyObject>(desurvDto);
        }

        public async Task<IntervalDesurveyObject> IntervalSurveyHole(IMapper mapper, DrillholeDesurveyEnum desurveyType, ImportTableFields collarTableFields, ImportTableFields intervalTableFields, bool bToe, bool bCollar, List<XElement> drillholeValues)
        {
            var desurvDto = await _drillhole.CreateIntervalSurveyHole(desurveyType, collarTableFields, intervalTableFields, bToe, bCollar, drillholeValues) as IntervalDesurveyDto;

            if (desurvDto.IsValid == false)
            {
                throw new IntervalException("Issue with desurvey survey interval data");
            }

            return mapper.Map<IntervalDesurveyDto, IntervalDesurveyObject>(desurvDto);
        }

        public async Task<IntervalDesurveyObject> IntervalDownhole(IMapper mapper, DrillholeDesurveyEnum desurveyType, ImportTableFields collarTableFields, ImportTableFields intervalTableFields, 
            ImportTableFields surveyTableFields, bool bToe, bool bCollar, List<XElement> drillholeValues)
        {
            var desurvDto = await _drillhole.CreateIntervalDownhole(desurveyType, collarTableFields, intervalTableFields, surveyTableFields, bToe, bCollar, drillholeValues) as IntervalDesurveyDto;

            if (desurvDto.IsValid == false)
            {
                throw new IntervalException("Issue with desurvey downhole interval data");
            }

            return mapper.Map<IntervalDesurveyDto, IntervalDesurveyObject>(desurvDto);
        }

      

    }
}

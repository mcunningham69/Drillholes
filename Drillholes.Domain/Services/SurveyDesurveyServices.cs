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
    public class SurveyDesurveyServices
    {
        private readonly IDesurveyDrillhole _drillhole;


        public SurveyDesurveyServices(IDesurveyDrillhole drillhole)
        {
            this._drillhole = drillhole;
        }

   

        public async Task<SurveyDesurveyObject> SurveyDownhole(IMapper mapper, DrillholeDesurveyEnum desurveyType, ImportTableFields tableFields, bool bToe, bool bCollar, List<XElement> surveyValues)
        {
            var desurvDto = await _drillhole.CreateSurveyDownhole(desurveyType, tableFields, bToe, bCollar, surveyValues) as SurveyDesurveyDto;

            if (desurvDto.IsValid == false)
            {
                throw new SurveyException("Issue with desurvey downhole survey data");
            }

            return mapper.Map<SurveyDesurveyDto, SurveyDesurveyObject>(desurvDto);
        }

       
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drillholes.Domain.Interfaces;
using Drillholes.Domain.DTO;
using Drillholes.Domain.Exceptions;
using Drillholes.Domain.DataObject;
using Drillholes.Domain.Enum;
using AutoMapper;
using System.Xml.Linq;

namespace Drillholes.Domain.Services
{
    public class CollarStatisticsService
    {
        private readonly ICollarStatistics _collar;
        private CollarTableDto collarDto = null;
        public CollarStatisticsService(ICollarStatistics collar)
        {
            this._collar = collar;
        }

        public async Task<CollarTableObject> SummaryStatistics(IMapper mapper, List<ImportTableField> fields, 
            XElement collarValues, DrillholeSurveyType surveyType)
        {
            collarDto = await _collar.SummaryStatistics(fields, collarValues, surveyType);

            if (collarDto.isValid == false)
            {
                throw new CollarStatisticsException("Issue with calculating Collar statistics");
            }

            return mapper.Map<CollarTableDto, CollarTableObject>(collarDto);
        }
    }

}

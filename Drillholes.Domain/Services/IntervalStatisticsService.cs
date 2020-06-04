using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drillholes.Domain.Interfaces;
using Drillholes.Domain.DTO;
using Drillholes.Domain.Exceptions;
using Drillholes.Domain.DataObject;
using AutoMapper;
using System.Xml.Linq;

namespace Drillholes.Domain.Services
{
    public class IntervalStatisticsService
    {
        private readonly IIntervalStatistics _interval;
        private IntervalTableDto intervalDto = null;
        public IntervalStatisticsService(IIntervalStatistics interval)
        {
            this._interval = interval;
        }

        public async Task<IntervalTableObject> SummaryStatistics(IMapper mapper, List<ImportTableField> fields,
            XElement intervalValues)
        {
            intervalDto = await _interval.SummaryStatistics(fields, intervalValues);

            if (intervalDto.isValid == false)
            {
                throw new SurveyStatisticsException("Issue with calculating Interval statistics");
            }

            return mapper.Map<IntervalTableDto, IntervalTableObject>(intervalDto);
        }
    }
}

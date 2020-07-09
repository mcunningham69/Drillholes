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
    public class ContinuousStatisticsService
    {
        private readonly IContinuousStatistics _continuous;
        private SummaryContinuousStatisticsDto continuousDto = null;
        public ContinuousStatisticsService(IContinuousStatistics continuous)
        {
            this._continuous = continuous;
        }

        public async Task<SummaryContinuousStatistics> SummaryStatistics(IMapper mapper, List<ImportTableField> fields,
            XElement continuousValues)
        {
            continuousDto = await _continuous.SummaryStatistics(fields, continuousValues);

            if (continuousDto.isValid == false)
            {
                throw new ContinuousStatisticsException("Issue with calculating Continuous statistics");
            }

            return mapper.Map<SummaryContinuousStatisticsDto, SummaryContinuousStatistics>(continuousDto);
        }
    }
}

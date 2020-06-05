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
    public class AssayStatisticsService
    {
        private readonly IAssayStatistics _assay;
        private SummaryAssayStatisticsDto summaryStatistics = null;

        public AssayStatisticsService(IAssayStatistics assay)
        {
            this._assay = assay;
        }

        public async Task<SummaryAssayStatistics> SummaryStatistics(IMapper mapper, List<ImportTableField> fields,
            XElement assayValues)
        {
            summaryStatistics = await _assay.SummaryStatistics(fields, assayValues);

            if (summaryStatistics.isValid == false)
            {
                throw new AssayStatisticsException("Issue with calculating Assay statistics");
            }

            return mapper.Map<SummaryAssayStatisticsDto, SummaryAssayStatistics>(summaryStatistics);
        }
    }
}

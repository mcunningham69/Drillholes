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
        private AssayTableDto assayDto = null;
        public AssayStatisticsService(IAssayStatistics assay)
        {
            this._assay = assay;
        }

        public async Task<AssayTableObject> SummaryStatistics(IMapper mapper, List<ImportTableField> fields,
            XElement assayValues)
        {
            assayDto = await _assay.SummaryStatistics(fields, assayValues);

            if (assayDto.isValid == false)
            {
                throw new SurveyStatisticsException("Issue with calculating Assay statistics");
            }

            return mapper.Map<AssayTableDto, AssayTableObject>(assayDto);
        }
    }
}

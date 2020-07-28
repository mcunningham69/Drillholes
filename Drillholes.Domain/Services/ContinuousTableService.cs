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

namespace Drillholes.Domain.Services
{
    public class IntervalTableService
    {
        private readonly IIntervalTable _interval;

        private IntervalTableDto intervalDto = null;

        public IntervalTableService(IIntervalTable interval)
        {
            this._interval = interval;
        }

        public async Task<IntervalTableObject> UpdateFieldvalues(string previousSelection, IMapper mapper, string changeTo, string searchColumn, string strOldName, ImportTableFields intervalTableFields)
        {
            intervalDto = await _interval.UpdateImportParameters(previousSelection, changeTo, searchColumn, strOldName, intervalTableFields);

            if (intervalDto.tableIsValid == false)
            {
                throw new IntervalException("Issue with updating Interval fields");
            }

            return mapper.Map<IntervalTableDto, IntervalTableObject>(intervalDto);
        }

        public async Task<IntervalTableObject> GetSurveyFields(IMapper mapper, string tablePath,
            DrillholeImportFormat tableFormat, string tableName)
        {
            intervalDto = await _interval.RetrieveTableFieldnames(tableFormat, tablePath, tableName);

            if (intervalDto.tableIsValid == false)
            {
                throw new IntervalException("Issue with Interval table");
            }

            return mapper.Map<IntervalTableDto, IntervalTableObject>(intervalDto);

        }

        public async Task<IntervalTableObject> PreviewData(IMapper mapper, DrillholeTableType tableType, int limit)
        {
            intervalDto = await _interval.PreviewAndImportFields(tableType, limit);

            if (intervalDto.tableIsValid == false)
            {
                throw new IntervalException("Issue with Interval preview table");
            }

            return mapper.Map<IntervalTableDto, IntervalTableObject>(intervalDto);

        }

        public async Task<IntervalTableObject> ImportAllFieldsAsGeneric(IMapper mapper, bool bImport)
        {
            intervalDto = await _interval.ImportAllFieldsAsGeneric(bImport);

            if (intervalDto.tableIsValid == false)
            {
                throw new IntervalException("Issue with importing all fields for Interval table");
            }

            return mapper.Map<IntervalTableDto, IntervalTableObject>(intervalDto);
        }

        public List<string> ReturnFields(IMapper mapper)
        {
            mapper.Map<IntervalTableDto, IntervalTableObject>(intervalDto);

            return intervalDto.fields;

        }
    }

}

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
    public class ContinuousTableService
    {
        private readonly IContinuousTable _continuous;

        private ContinuousTableDto continuousDto = null;

        public ContinuousTableService(IContinuousTable continuous)
        {
            this._continuous = continuous;
        }

        public async Task<ContinuousTableObject> UpdateFieldvalues(string previousSelection, IMapper mapper, string changeTo, string searchColumn, string strOldName)
        {
            continuousDto = await _continuous.UpdateImportParameters(previousSelection, changeTo, searchColumn, strOldName);

            if (continuousDto.tableIsValid == false)
            {
                throw new ContinuousException("Issue with updating fields");
            }

            return mapper.Map<ContinuousTableDto, ContinuousTableObject>(continuousDto);
        }

        public async Task<ContinuousTableObject> GetSurveyFields(IMapper mapper, string tablePath,
            DrillholeImportFormat tableFormat, string tableName)
        {
            continuousDto = await _continuous.RetrieveTableFieldnames(tableFormat, tablePath, tableName);

            if (continuousDto.tableIsValid == false)
            {
                throw new ContinuousException("Issue with table");
            }

            return mapper.Map<ContinuousTableDto, ContinuousTableObject>(continuousDto);

        }

        public async Task<ContinuousTableObject> PreviewData(IMapper mapper, DrillholeTableType tableType, int limit)
        {
            continuousDto = await _continuous.PreviewAndImportFields(tableType, limit);

            if (continuousDto.tableIsValid == false)
            {
                throw new ContinuousException("Issue with preview table");
            }

            return mapper.Map<ContinuousTableDto, ContinuousTableObject>(continuousDto);

        }

        public async Task<ContinuousTableObject> ImportAllFieldsAsGeneric(IMapper mapper, bool bImport)
        {
            continuousDto = await _continuous.ImportAllFieldsAsGeneric(bImport);

            if (continuousDto.tableIsValid == false)
            {
                throw new ContinuousException("Issue with importing all fields for table");
            }

            return mapper.Map<ContinuousTableDto, ContinuousTableObject>(continuousDto);
        }

        public List<string> ReturnFields(IMapper mapper)
        {
            mapper.Map<ContinuousTableDto, ContinuousTableObject>(continuousDto);

            return continuousDto.fields;

        }
    }

}

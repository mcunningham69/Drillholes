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
    public class AssayTableService
    {
        private readonly IAssayTable _assay;

        private AssayTableDto assayDto = null;

        public AssayTableService(IAssayTable assay)
        {
            this._assay = assay;
        }

        public async Task<AssayTableObject> UpdateFieldvalues(string previousSelection, IMapper mapper, string changeTo, string searchColumn, string strOldName)
        {
            assayDto = await _assay.UpdateImportParameters(previousSelection, changeTo, searchColumn, strOldName);

            if (assayDto.tableIsValid == false)
            {
                throw new AssayException("Issue with updating Assay fields");
            }

            return mapper.Map<AssayTableDto, AssayTableObject>(assayDto);
        }

        public async Task<AssayTableObject> GetSurveyFields(IMapper mapper, string tablePath,
            DrillholeImportFormat tableFormat, string tableName)
        {
            assayDto = await _assay.RetrieveTableFieldnames(tableFormat, tablePath, tableName);

            if (assayDto.tableIsValid == false)
            {
                throw new AssayException("Issue with Assay table");
            }

            return mapper.Map<AssayTableDto, AssayTableObject>(assayDto);

        }

        public async Task<AssayTableObject> PreviewData(IMapper mapper, DrillholeTableType tableType, int limit)
        {
            assayDto = await _assay.PreviewAndImportFields(tableType, limit);

            if (assayDto.tableIsValid == false)
            {
                throw new AssayException("Issue with Assay preview table");
            }

            return mapper.Map<AssayTableDto, AssayTableObject>(assayDto);

        }

        public async Task<AssayTableObject> ImportAllFieldsAsGeneric(IMapper mapper, bool bImport)
        {
            assayDto = await _assay.ImportAllFieldsAsGeneric(bImport);

            if (assayDto.tableIsValid == false)
            {
                throw new AssayException("Issue with importing all fields for Assay table");
            }

            return mapper.Map<AssayTableDto, AssayTableObject>(assayDto);
        }

        public List<string> ReturnFields(IMapper mapper)
        {
            mapper.Map<AssayTableDto, AssayTableObject>(assayDto);

            return assayDto.fields;

        }
    }
}

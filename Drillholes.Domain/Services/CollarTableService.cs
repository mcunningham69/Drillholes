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

namespace Drillholes.Domain.Services
{
    public class CollarTableService
    {
        private readonly ICollarTable _collar;
        private CollarTableDto collarDto = null;
        public CollarTableService(ICollarTable collar)
        {
            this._collar = collar;
        }

        public async Task<CollarTableObject> GetCollarFields(IMapper mapper, DrillholeImportFormat tableFormat, 
            string tableLocation, string tableName)
        {

            //TODO Implement bProject when layer is in Project
            collarDto = await _collar.RetrieveTableFieldnames(tableFormat, tableLocation, tableName);

            if (collarDto.tableIsValid == false)
            {
                throw new CollarException("Issue with Collar table");
            }

            //TODO - setup to map to existing CollarTableObject
            return mapper.Map<CollarTableDto, CollarTableObject>(collarDto);

        }

        public async Task<CollarTableObject> PreviewData(IMapper mapper, DrillholeTableType tableType, int limit)
        {
            collarDto = await _collar.PreviewAndImportFields(tableType, limit);


            if (collarDto.tableIsValid == false)
            {
                throw new CollarException("Issue with Collar preview table");
            }

            return mapper.Map<CollarTableDto, CollarTableObject>(collarDto);

        }

        public List<string> ReturnFields(IMapper mapper)
        {
            mapper.Map<CollarTableDto, CollarTableObject>(collarDto);
            return collarDto.fields;

        }

        public async Task<CollarTableObject> ImportAllFieldsAsGeneric(IMapper mapper, bool bImport)
        {
            collarDto = await _collar.ImportAllFieldsAsGeneric(bImport);

            if (collarDto.tableIsValid == false)
            {
                throw new CollarException("Issue with importing all fields for Collar table");
            }

            return mapper.Map<CollarTableDto, CollarTableObject>(collarDto);
        }

        public async Task<CollarTableObject> UpdateFieldvalues(string previousSelection, IMapper mapper, string changeTo, string searchColumn, string strOldName)
        {
            collarDto = await _collar.UpdateImportParameters(previousSelection, changeTo, searchColumn, strOldName);

            if (collarDto.tableIsValid == false)
            {
                throw new CollarException("Issue with updating Collar fields");
            }

            return mapper.Map<CollarTableDto, CollarTableObject>(collarDto);
        }
    }

}

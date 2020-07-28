using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drillholes.Domain.DTO;
using Drillholes.Domain.Enum;

namespace Drillholes.Domain.Interfaces
{
    public interface IAssayTable
    {
        Task<AssayTableDto> PreviewAndImportFields(DrillholeTableType tableType, int limit);
        Task<AssayTableDto> RetrieveTableFieldnames(DrillholeImportFormat tableFormat, string tableLocation, string tableName);

        Task<AssayTableDto> ImportAllFieldsAsGeneric(bool bImport);
        Task<AssayTableDto> UpdateImportParameters(string previousSelection, string changeTo, string searchColumn, string strOldName, ImportTableFields assayTableFields);

    }
}

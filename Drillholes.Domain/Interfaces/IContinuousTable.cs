using Drillholes.Domain.DTO;
using Drillholes.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drillholes.Domain.Interfaces
{
    public interface IContinuousTable
    {
        Task<ContinuousTableDto> PreviewAndImportFields(DrillholeTableType tableType, int limit);
        Task<ContinuousTableDto> RetrieveTableFieldnames(DrillholeImportFormat tableFormat, string tableLocation, string tableName);

        Task<ContinuousTableDto> ImportAllFieldsAsGeneric(bool bImport);
        Task<ContinuousTableDto> UpdateImportParameters(string previousSelection, string changeTo, string searchColumn, string strOldName, ImportTableFields continuousTableFields);
    }
}

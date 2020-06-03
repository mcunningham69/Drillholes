using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drillholes.Domain.DTO;
using Drillholes.Domain.Enum;

namespace Drillholes.Domain.Interfaces
{
    public interface IIntervalTable
    {
        Task<IntervalTableDto> PreviewAndImportFields(DrillholeTableType tableType, int limit);
        Task<IntervalTableDto> RetrieveTableFieldnames(DrillholeImportFormat tableFormat, string tableLocation, string tableName);

        Task<IntervalTableDto> ImportAllFieldsAsGeneric(bool bImport);
        Task<IntervalTableDto> UpdateImportParameters(string previousSelection, string changeTo, string searchColumn, string strOldName);

    }
}

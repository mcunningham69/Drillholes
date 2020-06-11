using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drillholes.Domain.Enum;
using Drillholes.Domain.DTO;
using System.Xml.Linq;

namespace Drillholes.Domain.Interfaces
{
    public interface ICollarTable 
    {
        Task<CollarTableDto> PreviewAndImportFields( DrillholeTableType tableType,int limit);
        Task<CollarTableDto> RetrieveTableFieldnames(DrillholeImportFormat tableFormat, string tableLocation, string tableName);
        Task<CollarTableDto> ImportAllFieldsAsGeneric(bool bImport);
        Task<CollarTableDto> UpdateImportParameters(string previousSelection, string changeTo, string searchColumn, string strOldName);

    }
}

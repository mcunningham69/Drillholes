using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drillholes.Domain.Enum;
using Drillholes.Domain.DTO;

namespace Drillholes.Domain.Interfaces
{
    public interface ISurveyTable
    {
        Task<SurveyTableDto> PreviewAndImportFields(DrillholeTableType tableType, int limit);
        Task<SurveyTableDto> RetrieveTableFieldnames(DrillholeImportFormat tableFormat, string tableLocation, string tableName);
        Task<SurveyTableDto> ImportAllFieldsAsGeneric(bool bImport);
        Task<SurveyTableDto> UpdateImportParameters(string previousSelection, string changeTo, string searchColumn, string strOldName, ImportTableFields surveyTableFields);

        Task<SurveyTableDto> RetrieveTemplateFieldnames();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Drillholes.Domain.DTO;
using Drillholes.Domain.Enum;


namespace Drillholes.Domain.Interfaces
{
    public interface ISurveyStatistics
    {
        Task<SurveyTableDto> SummaryStatistics(List<ImportTableField> fields, XElement surveyValues,
            string tableName, string tableLocation, DrillholeImportFormat tableFormat);
    }
}

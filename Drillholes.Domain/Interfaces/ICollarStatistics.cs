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
    public interface ICollarStatistics
    {
      //  Task<CollarTableDto> SummaryStatistics(List<ImportTableField> fields, XElement collarValues,
        //    DrillholeSurveyType surveyType);

        Task<SummaryCollarStatisticsDto> SummaryStatistics(List<ImportTableField> fields, XElement collarValues,
            DrillholeSurveyType surveyType);
    }
}

using Drillholes.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace Drillholes.Domain.Interfaces
{
    public interface ISurveyEdit
    {
        Task<SurveyTableDto> UpdateValues(List<RowsToEdit> rowsToEdit, XElement surveyValues);

    }
}

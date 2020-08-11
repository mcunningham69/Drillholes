using Drillholes.Domain.DTO;
using Drillholes.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drillholes.Domain.DTO
{
    public class SurveyDesurveyDto : CollarDesurveyDto
    {
        public ImportTableFields surveyTableFields { get; set; }
        public List<int> survID { get; set; }
        public List<string> survHoleID { get; set; }
        public List<double> distance { get; set; }
        public List<bool> isSurvey { get; set; }

        public SurveyDesurveyDto()
        {
            survHoleID = new List<string>();
            survID = new List<int>();
            distance = new List<double>();
            isSurvey = new List<bool>();
        }

    }
}

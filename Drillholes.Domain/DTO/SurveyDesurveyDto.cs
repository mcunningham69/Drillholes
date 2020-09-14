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
        public List<double> distSurvFrom { get; set; }
        public List<bool> isSurvey { get; set; }
        public List<int>survId { get; set; }
        public SurveyDesurveyDto()
        {
            bhid = new List<string>();
            colId = new List<int>();
            survId = new List<int>();
            distSurvFrom = new List<double>();
            isSurvey = new List<bool>();
            azimuth = new List<double>();
            dip = new List<double>();
        }

    }
}

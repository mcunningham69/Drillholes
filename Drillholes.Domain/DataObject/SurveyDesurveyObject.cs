using Drillholes.Domain.DTO;
using Drillholes.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drillholes.Domain.DataObject
{
    public class SurveyDesurveyObject : CollarDesurveyObject
    {
        public ImportTableFields surveyTableFields { get; set; }
        public List<double> distFrom { get; set; }
        public List<bool> isSurvey { get; set; }
        public List<int> survId { get; set; }

    }
}

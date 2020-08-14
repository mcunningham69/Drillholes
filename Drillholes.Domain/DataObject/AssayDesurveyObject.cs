using Drillholes.Domain.DTO;
using Drillholes.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drillholes.Domain.DataObject
{
    public class AssayDesurveyObject : SurveyDesurveyObject
    {
        public ImportTableFields assayTableFields { get; set; }
        public List<int> assayId { get; set; }
        public List<double> distTo { get; set; }
        public List<bool> isAssay { get; set; }


    }
}

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
        public List<int> assID { get; set; }
        public List<string> assayBhid { get; set; }
        public List<double> mfrom { get; set; }
        public List<double> mto { get; set; }
        public List<bool> isAssay { get; set; }


    }
}

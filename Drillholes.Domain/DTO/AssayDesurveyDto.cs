using Drillholes.Domain.DTO;
using Drillholes.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drillholes.Domain.DTO
{
    public class AssayDesurveyDto : SurveyDesurveyDto
    {
        public ImportTableFields assayTableFields { get; set; }
        public List<int> assID { get; set; }
        public List<string> assayBhid { get; set; }
        public List<double> mfrom { get; set; }
        public List<double> mto { get; set; }
        public List<bool> isAssay { get; set; }


        public AssayDesurveyDto()
        {
            assID = new List<int>();
            assayBhid = new List<string>();
            mfrom = new List<double>();
            mto = new List<double>();
            length = new List<double>();
            isAssay = new List<bool>();
      
        }

        public void CalculateLength(int i)
        {
            length[i] = mto[i] - mfrom[i];
        }
    }
}

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
        public List<double> distTo { get; set; }
        public List<bool> isAssay { get; set; }

        public AssayDesurveyDto()
        {
            id = new List<int>();
            bhid = new List<string>();
            distFrom = new List<double>();
            distTo = new List<double>();
            length = new List<double>();
            isAssay = new List<bool>();
            dip = new List<double>();
            azimuth = new List<double>();
      
        }

        public void CalculateLength(int i)
        {
            length[i] = distTo[i] - distFrom[i];
        }
    }
}

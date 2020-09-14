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
        public List<double> distFrom { get; set; }
        public List<double> distTo { get; set; }
        public List<bool> isAssay { get; set; }
        public List<int> assayId { get; set; }


        public AssayDesurveyDto()
        {
            colId = new List<int>();
            bhid = new List<string>();
            distSurvFrom = new List<double>();
            distFrom = new List<double>();
            distTo = new List<double>();
            length = new List<double>();
            isAssay = new List<bool>();
            dip = new List<double>();
            azimuth = new List<double>();
            assayId = new List<int>();
            survId = new List<int>();

        }

        public void CalculateLength(int i)
        {
            length[i] = distTo[i] - distFrom[i];
        }
    }
}

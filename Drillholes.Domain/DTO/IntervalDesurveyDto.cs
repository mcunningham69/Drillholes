using Drillholes.Domain.DTO;
using Drillholes.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drillholes.Domain.DTO
{
    public class IntervalDesurveyDto : AssayDesurveyDto
    {
        public ImportTableFields intervalTableFields { get; set; }
        public List<bool> isInterval { get; set; }

        public IntervalDesurveyDto()
        {
            id = new List<int>();
            bhid = new List<string>();
            distFrom = new List<double>();
            distTo = new List<double>();
            length = new List<double>();
            isInterval = new List<bool>();
            dip = new List<double>();
            azimuth = new List<double>();

        }

    }
}

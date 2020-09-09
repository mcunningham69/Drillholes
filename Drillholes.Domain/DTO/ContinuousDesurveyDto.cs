using Drillholes.Domain.DTO;
using Drillholes.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drillholes.Domain.DTO
{
    public class ContinuousDesurveyDto : IntervalDesurveyDto
    {
        public ImportTableFields continuousTableFields { get; set; }
        public List<bool> isContinuous { get; set; }
        public List<int> contId { get; set; }

        //from alpha beta gamma
        public List<double> CalculatedDip { get; set; }
        public List<double> CalculatedAzimuth { get; set; }
        public List<double> CalculatedPlunge { get; set; }
        public List<double> CalculatedTrend { get; set; }

        public ContinuousDesurveyDto()
        {
            contId = new List<int>();
            colId = new List<int>();
            survId = new List<int>();
            bhid = new List<string>();
            distFrom = new List<double>();
            length = new List<double>();
            isContinuous = new List<bool>();
            dip = new List<double>();
            azimuth = new List<double>();
            CalculatedDip = new List<double>();
            CalculatedAzimuth = new List<double>();
            CalculatedPlunge = new List<double>();
            CalculatedTrend = new List<double>();
        }

    }
}

using Drillholes.Domain.DTO;
using Drillholes.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drillholes.Domain.DataObject
{
    public class CollarDesurveyObject
    {
        public int Count { get; set; }
        public ImportTableFields collarTableFields { get; set; }
        public List<int> colId { get; set; }
        public List<string> bhid { get; set; }
        public List<double> x { get; set; }
        public List<double> y { get; set; }
        public List<double> z { get; set; }
        public List<double> length { get; set; }
        public List<double> azimuth { get; set; }
        public List<double> dip { get; set; }
        public List<bool> isCollar { get; set; }
        public DrillholeSurveyType surveyType { get; set; }
        public DrillholeDesurveyEnum desurveyType { get; set; }

        public bool IsValid { get; set; }


        public CollarDesurveyObject()
        {
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drillholes.Domain.Enum;

namespace Drillholes.Domain.DTO
{
    public class CollarDesurveyDto
    {
        public int Count { get; set; }
        public ImportTableFields collarTableFields { get; set; }
        public List<int> id { get; set; }
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
        public CollarDesurveyDto()
        {
            id = new List<int>();
            bhid = new List<string>();
            x = new List<double>();
            y = new List<double>();
            z = new List<double>();
            length = new List<double>();
            azimuth = new List<double>();
            dip = new List<double>();
            isCollar = new List<bool>();
        }
    }

    public class AttributeFields
    {
        public string fieldName { get; set; }
        public string fieldValue { get; set; }
    }
}

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
        public string bhid { get; set; }
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
        public float length { get; set; }
        public float azimuth { get; set; }
        public float dip { get; set; }
        public DrillholeSurveyType surveyType { get; set; }
        public DrillholeDesurveyEnum desurveyType { get; set; }

        public List<AttributeFields> attributeFields { get; set; }

        public bool IsValid { get; set; }
        public CollarDesurveyDto()
        {
            attributeFields = new List<AttributeFields>();
        }
    }

    public class AttributeFields
    {
        public string fieldName { get; set; }
        public string fieldValue { get; set; }
    }
}

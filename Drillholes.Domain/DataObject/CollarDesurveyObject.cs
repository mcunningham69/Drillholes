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
        public string bhid { get; set; }
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
        public float length { get; set; }
        public float azimuth { get; set; }
        public float dip { get; set; }
        public DrillholeSurveyType surveyType { get; set; }
        public DrillholeDesurveyEnum desurveyType { get; set; }
        public bool IsValid { get; set; }


        public List<AttributeFields> attributeFields { get; set; }

        public CollarDesurveyObject()
        {
            attributeFields = new List<AttributeFields>();
        }

    }
}

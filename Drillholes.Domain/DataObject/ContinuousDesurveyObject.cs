using Drillholes.Domain.DTO;
using Drillholes.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drillholes.Domain.DataObject
{
    public class ContinuousDesurveyObject
    {
        public string bhid { get; set; }
        public float distFrom { get; set; }
        public float distTo { get; set; }
        public DrillholeDesurveyEnum desurveyType { get; set; }
        public DrillholeSurveyType surveyType { get; set; }
        public bool IsValid { get; set; }
        public List<AttributeFields> attributeFields { get; set; }

      

    }
}

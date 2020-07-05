﻿using Drillholes.Domain.DTO;
using Drillholes.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drillholes.Domain.DTO
{
    public class SurveyDesurveyDto
    {
        public string bhid { get; set; }
        public float distance { get; set; }
        public float azimuth { get; set; }
        public float dip { get; set; }
        public DrillholeDesurveyEnum desurveyType { get; set; }
        public bool IsValid { get; set; }
        public List<AttributeFields> attributeFields { get; set; }

      

    }
}

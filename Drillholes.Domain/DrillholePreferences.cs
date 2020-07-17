﻿using Drillholes.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drillholes.Domain
{
    public class DrillholePreferences
    {

        public bool NegativeDip { get; set; }
        public bool ImportAllColumns { get; set; }
        public bool IgnoreInvalidValues { get; set; }
        public bool LowerDetection { get; set; }
        public DrillholeSurveyType surveyType { get; set; }
        public double DipTolerance { get; set; }
        public double AzimuthTolerance { get; set; }
        public double DefaultValue { get; set; }
        public bool ImportSurveyOnly { get; set; }
        public bool ImportAssayOnly { get; set; }
        public bool ImportGeologyOnly { get; set; }
        public bool ImportContinuousOnly { get; set; }
        public bool NullifyZeroAssays { get; set; }
        public bool GeologyBase { get; set; }
        public bool CreateCollar { get; set; }
        public bool CreateToe { get; set; }


    }
}

using Drillholes.Domain.Enum;
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
        public bool BottomCore { get; set; }
        public bool TopCore { get; set; }
        public bool CalculateStructures { get; set; }


    }

    public class DrillholeProjectProperties
    {
        public string ProjectName { get; set; }
        public string ProjectParentFolder { get; set; }
        public string ProjectFolder { get; set; }
        public string ProjectFile { get; set; }
        public string DrillholeTables { get; set; }
        public string DrillholePreferences { get; set; }
        public string DrillholeData { get; set; }
        public string DrillholeDesurvey { get; set; }
        public string DrillholeFields { get; set; } 

        public DrillholeProjectProperties()
        {
            ProjectFile = "";
            ProjectFolder = "";
            ProjectName = "";
            ProjectParentFolder = "";
            DrillholeTables = "";
            DrillholePreferences = "";
            DrillholeData = "";
            DrillholeDesurvey = "";
            DrillholeFields = "";
        }
    }
}

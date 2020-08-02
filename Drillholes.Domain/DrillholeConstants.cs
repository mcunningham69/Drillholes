using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drillholes.Domain
{
    public static class DrillholeConstants
    {
        //XML
        public const string drillholeTable = "DrillholeTables";
        public const string drillholeProject = "DrillholeProject";
        public const string drillholeFields = "DrillholeFields";
        public const string drillholePref = "DrillholePreferences";
        public const string drillholeData = "DrillholeData";
        public const string drillholeDesurv = "DrillholeDesurv";

        //validation tests
        public const string checkHole = "Check 'HoleID' field";
        public const string checkX = "Check 'X' field";
        public const string checkY = "Check 'Y' field";
        public const string checkZ = "Check 'Z' field";
        public const string checkTD = "Check 'Length' field";
        public const string checkDip = "Check 'Dip' field";
        public const string checkAzi = "Check 'Azimuth' field";
        public const string checkGrade = "Check 'Grade' field";
        public const string checkDensity = "Check 'Density' field";
        public const string checkNumeric = "Check 'Numeric' field";
        public const string checkDist = "Check 'Distance' field";
        public const string checkFromTo = "Check 'From' and 'To' fields";
        public const string checkFrom = "Check 'From' field";
        public const string checkTo = "Check 'To' field";
        public const string checkCoord = "Check 'X' and 'Y' fields";
        public const string checkAlpha = "Check Alpha values";
        public const string checkBeta = "Check Beta values";
        public const string checkGamma = "Check Gamma values";

        //test type
        public const string IsEmptyOrNull = "Empty or Null";
        public const string IsNumeric = "Is Numeric";
        public const string Duplicates = "Duplicates";
        public const string HoleLength = "Drillhole Length";
        public const string ZeroCoordinate = "Collar Coordinate";
        public const string SurveyRange = "Dip and Azimuth";
        public const string SurveyDeviation = "Survey Deviation";
        public const string SurveyDistance = "Survey Distance";
        public const string MissingCollar = "Missing Collar";
        public const string MissingInterval = "Missing Interval";
        public const string NegativeOrZeroInterval = "Negative or Zero Interval";
        public const string OverlappingInterval = "Interval Overlap";
        public const string ZeroGrade = "Zero Grade";
        public const string Structures = "Structural Measurements";


        //label for combobox
        public const string selectField = "Select Field To Map";
        public const string notImported = "Not Imported";

        //data types 
        public const string _category = "Category";
        public const string _text = "Text";
        public const string _numeric = "Numeric";
        public const string _grade = "Grade";
        public const string _sample = "Sample ID";
        public const string _alpha = "a";
        public const string _beta = "b";
        public const string _gamma = "g";

        public const string _density = "Density";
        public const string _time = "Time";
        public const string _date = "Date";
        public const string _url = "URL";
        public const string _generic = "Same As Source";
        public const string _genericName = "Generic";

        //Import As - collar
        public const string holeID = "Hole ID";
        public const string x = "East (X)";
        public const string y = "North (Y)";
        public const string z = "Elev (Z)";
        public const string maxDepth = "Max Depth";

        //Import As - survey
        public const string survDistance = "Distance";
        public const string azimuth = "Azimuth";
        public const string dip = "Dip";

        //Import As - other/assay/litho
        public const string distFrom = "From";
        public const string distTo = "To";

        //fieldnames in underlying collar table
        public const string holeIDName = "HoleID";
        public const string xName = "X";
        public const string yName = "Y";
        public const string zName = "Z";
        public const string maxName = "MaxDepth";

        //fieldnames in underlying survey table
        public const string distName = "Distance";
        public const string azimuthName = "Azimuth";
        public const string dipName = "Dip";

        //fieldnames in underlying other/assay/litho tables
        public const string distFromName = "distfrom";
        public const string distToName = "distto";
        public const string sampleName = "SampleID";

        //Grouping
        public const string GroupMapFields = "Mandatory Fields";
        public const string GroupOtherFields = "Other Fields";

        //table type
        public const string _collar = "collar";
        public const string _survey = "survey";
        public const string _assay = "assay";
        public const string _litho = "interval";
        public const string _other = "other";
        public const string _Collar = "Collar";
        public const string _Survey = "Survey";
        public const string _Assay = "Assay";
        public const string _Interval = "Interval";
        public const string _Other = "Other";
        public const string _Continuous = "Continuous";

        public const string excel = "Excel Table";
        public const string text = "Text File";
        public const string fgdb = "File Geodatabase Table";
        public const string egdb = "Enterprise Geodatabase Table";
        public const string pgdb = "Personal Geodatabase Table";

        public const string CreatePoints = "New 3D Points....";
        public const string CreateLines = "New 3D Lines....";

        public const string Title = "Drill hole Compiler";
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Drillholes.Domain;
using Drillholes.Domain.DTO;
using Drillholes.Domain.Interfaces;
using Drillholes.Domain.Enum;
using Microsoft.VisualBasic;

namespace Drillholes.Validation.TestMessage
{
    public class SurveyValidation : ISurveyValidation
    {
        private ValidationSurveyDto surveyValidationDto;
        public SurveyValidation()
        {
            surveyValidationDto = new ValidationSurveyDto();

            surveyValidationDto.isValid = true;

        }

        #region Messages
        public async Task<ValidationSurveyDto> CheckForDuplicates(ValidationMessages ValuesToCheck, XElement surveyValues)
        {
            surveyValidationDto.testMessages = ValuesToCheck;

            var elements = surveyValues.Elements();

            foreach (var check in ValuesToCheck.testMessage)
            {
                check.count = elements.Count();
                string fieldID = check.tableFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Select(f => f.columnHeader).Single();
                string distField = check.tableFields.Where(o => o.columnImportName == DrillholeConstants.distName).Select(f => f.columnHeader).Single();

                var holes = elements.GroupBy(x => x.Element(fieldID).Value).Where(group => group.Count() > 1).Select(group => group.Key).ToList();

                //return list of holes
                foreach (var hole in holes)
                {
                    var duplicates = elements.Where(h => h.Element(fieldID).Value == hole).GroupBy(d => d.Element(distField).Value).Where(group => group.Count() > 1).Select(group => group.Key).ToList();

                    string message = "";
                    if (duplicates.Count > 0)
                    {
                        foreach (string distance in duplicates)
                        {
                            var holeAttr = elements.Where(y => y.Element(distField).Value == distance && y.Element(fieldID).Value == hole).Select(z => String.Join(distance, z.Attribute("ID").Value)).ToList();

                            string idList = String.Join(", ", holeAttr.ToArray());
                            int noDups = holeAttr.Count();

                            message = "Value for record '" + holeAttr + "' and HoleID '" + hole + "' of field type ' "
                       + check.tableField.columnImportName + "'  " + " at distance " + distance + " is repeated " + noDups.ToString() +
                             "  times for the following survey records: " + idList;

                            check.ValidationStatus.Add(new DrillholeValidationStatus { ErrorType = DrillholeMessageStatus.Warning, Description = message, 
                                ErrorColour = "Orange", id = Convert.ToInt32(holeAttr.First().ToString())});

                            check.validationMessages.Add(message);
                        }
                    }
                    else
                    {
                        message = "There are no duplicate surveys in the Collar table for hole " + hole +
                        ".";

                        check.validationMessages.Add(message);
                        check.ValidationStatus.Add(new DrillholeValidationStatus { ErrorType = DrillholeMessageStatus.Valid, Description = message, ErrorColour = "Green" });
                    }
                }
            }

            return surveyValidationDto;
        }

        public async Task<ValidationSurveyDto> CheckForMissingCollars(ValidationMessages ValuesToCheck, List<XElement> drillholeValues)
        {
            surveyValidationDto.testMessages = ValuesToCheck;

            string message = "";

            foreach (var check in ValuesToCheck.testMessage)
            {
                if (check.validationTest == DrillholeConstants.checkHole)
                {
                    var collarElements = drillholeValues[0].Elements();
                    var distanceElements = drillholeValues[1].Elements();

                    check.count = collarElements.Count();

                    string holeFieldID = check.tableFields[0].columnHeader;
                    string distanceFieldID = check.tableFields[1].columnHeader;

                    var collarHoles = collarElements.Select(c => c.Element(holeFieldID).Value).ToList();
                    var distanceHoles = distanceElements.GroupBy(x => x.Element(distanceFieldID).Value).Where(group => group.Count() > 0).Select(group => group.Key).ToList();

                    message = "Collar and Survey table have same ordinal hole IDs";

                    //if same holes in same order then returns true
                    bool result = collarHoles.SequenceEqual(distanceHoles);

                    if (result)
                    {
                        check.ValidationStatus.Add(new DrillholeValidationStatus { ErrorType = DrillholeMessageStatus.Valid, Description = message, ErrorColour = "Green" });
                        check.validationMessages.Add(message);
                    }
                    else
                    {
                        result = await CompareLists(collarHoles, distanceHoles); //true if same holes in both tables even if not in same order

                        if (result)
                        {
                            message = "Collar and Survey table have same hole IDs";

                            check.ValidationStatus.Add(new DrillholeValidationStatus { ErrorType = DrillholeMessageStatus.Valid, Description = message, ErrorColour = "Green" });
                            check.validationMessages.Add(message);
                        }
                        else
                        {
                            if (collarHoles.Except(distanceHoles).Count() > 0)
                            {
                                var noDist = collarHoles.Except(distanceHoles);

                                foreach (string dist in noDist)
                                {
                                    message = "Hole '" + dist + "' has no corresponding hole in the Survey table";
                                    check.ValidationStatus.Add(new DrillholeValidationStatus { ErrorType = DrillholeMessageStatus.Warning, Description = message, 
                                        ErrorColour = "Orange", id=-999 });
                                    check.validationMessages.Add(message);
                                }
                            }

                            if (distanceHoles.Except(collarHoles).Count() > 0)
                            {
                                var noDistances = distanceHoles.Except(collarHoles);

                                int counter = 0;
                                foreach (string dist in noDistances)
                                {
                                    var holeAttr = distanceElements.Where(y => y.Element(distanceFieldID).Value == dist).Select(z => z.Attribute("ID").Value).FirstOrDefault();

                                    message = "Survey hole '" + dist + "' has no corresponding value in the Collar table";
                                    check.ValidationStatus.Add(new DrillholeValidationStatus { ErrorType = DrillholeMessageStatus.Warning, Description = message, 
                                        ErrorColour = "Orange", id=Convert.ToInt32(holeAttr) });
                                    check.validationMessages.Add(message);

                                    counter++;
                                }
                            }
                        }
                    }
                }
            }

            return surveyValidationDto;
        }

        private async Task<bool> CompareLists(List<string> collarHoles, List<string> surveyHoles)
        {

            if (collarHoles.Count == surveyHoles.Count)
            {
                var filteredSequence = collarHoles.Where(x => surveyHoles.Contains(x));
                if (filteredSequence.Count() == collarHoles.Count)
                {
                    return true;
                }
                else
                    return false;
            }
            else
                return false;

        }


        public async Task<ValidationSurveyDto> CheckForNumeric(ValidationMessages ValuesToCheck, XElement surveyValues)
        {
            surveyValidationDto.testMessages = ValuesToCheck;

            foreach (var check in ValuesToCheck.testMessage)
            {
                string fieldID = check.tableField.columnHeader;
                string fieldName = check.tableField.columnImportAs;

                CheckNumericValues(surveyValues, check, fieldID, fieldName);
            }

            return surveyValidationDto;
        }

        public async Task<ValidationSurveyDto> CheckIsEmpty(ValidationMessages ValuesToCheck, XElement surveyValues)
        {
            surveyValidationDto.testMessages = ValuesToCheck;

            foreach (var check in ValuesToCheck.testMessage)
            {
                string fieldID = check.tableField.columnHeader;
                string fieldName = check.tableField.columnImportAs;

                CheckEmptyValues(surveyValues, check, fieldID, fieldName);
            }

            return surveyValidationDto;
        }

        private async void CheckEmptyValues(XElement surveyValues, ValidationMessage validationTest, string fieldID, string fieldName)
        {
            var elements = surveyValues.Elements();
            validationTest.count = elements.Count();

            foreach (XElement element in elements)
            {
                string message = "";
                string valueCheck = element.Element(fieldID).Value;
                string holeAttr = element.Attribute("ID").Value;
               

                if (string.IsNullOrEmpty(valueCheck) || string.IsNullOrWhiteSpace(valueCheck))
                {
                    DrillholeMessageStatus _errorType = DrillholeMessageStatus.Error;
                    string _errorColour = "Red";

                    if (fieldName == "Numeric")
                    {
                        _errorType = DrillholeMessageStatus.Warning;
                        _errorColour = "Orange";
                    }

                    message = "Survey for record " + holeAttr + " and field '" + fieldID + "' of field type '"
                        + fieldName + "' has no value";

                    validationTest.validationMessages.Add(message);
                    validationTest.ValidationStatus.Add(new DrillholeValidationStatus { ErrorType = _errorType, Description = message, ErrorColour = _errorColour, id = Convert.ToInt32(holeAttr) });

                    validationTest.verified = false;
                }
                else
                {
                    message = "'" + fieldID + "' of field type '" + fieldName + "' with record " + holeAttr + "' verified as not EMPTY or WHITESPACE";

                    validationTest.validationMessages.Add(message);
                    validationTest.ValidationStatus.Add(new DrillholeValidationStatus { ErrorType = DrillholeMessageStatus.Valid, Description = message, ErrorColour = "Green", id = Convert.ToInt32(holeAttr) });

                }
            }
        }
        private async void CheckNumericValues(XElement drillholeValues, ValidationMessage validationTest, string fieldID, string fieldName)
        {
            var elements = drillholeValues.Elements();
            validationTest.count = elements.Count();

            foreach (XElement element in elements)
            {
                string fieldValue = element.Element(fieldID).Value;
                string holeAttr = element.Attribute("ID").Value;
                string message = "";

                if (!Information.IsNumeric(fieldValue))
                {
                    DrillholeMessageStatus _errorType = DrillholeMessageStatus.Error;
                    string _errorColour = "Red";

                    if (fieldName == "Numeric")
                    {
                        _errorType = DrillholeMessageStatus.Warning;
                        _errorColour = "Orange";
                    }

                    message = "Value for record " + holeAttr + " and field '" + fieldID + "' of field type '"
                       + fieldName + "' is not NUMERIC";

                    validationTest.validationMessages.Add(message);
                    validationTest.ValidationStatus.Add(new DrillholeValidationStatus { ErrorType = _errorType, Description = message, ErrorColour = _errorColour, id = Convert.ToInt32(holeAttr) });
                    validationTest.verified = false;
                }
                else
                {
                    message = "'" + fieldID + "' of field type '" + fieldName + "' verified as NUMERIC";

                    validationTest.validationMessages.Add(message);
                    validationTest.ValidationStatus.Add(new DrillholeValidationStatus { ErrorType = DrillholeMessageStatus.Valid, Description = message, ErrorColour = "Green", id = Convert.ToInt32(holeAttr) });

                }
            }
        }



        public async Task<ValidationSurveyDto> CheckMaxDepth(ValidationMessages ValuesToCheck, List<XElement> drillholeValues)
        {
            surveyValidationDto.testMessages = ValuesToCheck;

            var collarElements = drillholeValues[0].Elements();
            var distanceElements = drillholeValues[1].Elements();

            foreach (var check in ValuesToCheck.testMessage)
            {
                check.count = collarElements.Count();

                if (check.validationTest == DrillholeConstants.checkTD)
                {
                    foreach (XElement element in collarElements)
                    {
                        string holeFieldID = check.tableFields[0].columnHeader;
                        string maxField = check.tableFields.Where(o => o.columnImportName == DrillholeConstants.maxName).Select(f => f.columnHeader).Single();
                        string surveyFieldHoleID = check.tableFields[2].columnHeader;
                        string distanceField = check.tableFields.Where(o => o.columnImportName == DrillholeConstants.distName).Select(f => f.columnHeader).Single();

                        var collar = element.Element(holeFieldID).Value;
                        var length = element.Element(maxField).Value;
                        var distanceQuery = distanceElements.Where(c => c.Element(holeFieldID).Value == collar.ToString()).ToList();

                        double tD = 0.0;

                        if (Information.IsNumeric(length))
                        {
                            tD = Convert.ToDouble(length);

                            List<double> surveyVals = new List<double>();

                            foreach (var value in distanceQuery)
                            {
                                string holeAttr = value.Attribute("ID").Value;

                                string message = "";

                                string distance = value.Element(distanceField).Value;

                                if (Information.IsNumeric(distance))
                                {
                                    surveyVals.Add(Convert.ToDouble(distance));
                                }

                                if (surveyVals.Count > 0)
                                {
                                    double surveyMaxDepth = surveyVals.Max();

                                    if (surveyMaxDepth > tD)
                                    {
                                        message = "Survey record: " + holeAttr + " has a maximum survey distance of " + surveyMaxDepth.ToString() + " which is greater than collar length " + tD;

                                        check.validationMessages.Add(message);
                                        check.ValidationStatus.Add(new DrillholeValidationStatus { ErrorType = DrillholeMessageStatus.Warning, Description = message, ErrorColour = "Orange", id = Convert.ToInt32(holeAttr) });
                                        check.verified = false;
                                    }
                                    else
                                    {
                                        message = "Survey max. distance of " + surveyMaxDepth + " for hole " + collar + " is less than total length of " + tD;
                                        check.ValidationStatus.Add(new DrillholeValidationStatus { ErrorType = DrillholeMessageStatus.Valid, Description = message, ErrorColour = "Green", id = Convert.ToInt32(holeAttr) });
                                        check.validationMessages.Add(message);
                                    }
                                }
                            }
                        }
                    }

                }
            }

            return surveyValidationDto;
        }

        public async Task<ValidationSurveyDto> CheckRange(ValidationMessages ValuesToCheck, XElement surveyValues)
        {
            surveyValidationDto.testMessages = ValuesToCheck;

            var elements = surveyValues.Elements();

            foreach (var check in ValuesToCheck.testMessage)
            {
                check.count = elements.Count();

                string holeID = "";
                string fieldID = "";
                string fieldType = "";

                foreach (XElement element in surveyValues.Elements())
                {
                    string hole = "";
                    string fieldValue = "";
                    string holeAttr = "";
                    string message = "";

                    if (check.validationTest == DrillholeConstants.checkDip)
                    {
                        holeID = check.tableFields[0].columnHeader;
                        fieldID = check.tableFields[1].columnHeader;
                        fieldType = check.tableFields[1].columnImportAs;

                        hole = element.Element(holeID).Value;
                        fieldValue = element.Element(fieldID).Value;
                        holeAttr = element.Attribute("ID").Value;

                        if (Information.IsNumeric(fieldValue))
                        {
                            double value = Convert.ToDouble(fieldValue);

                            if (value > 90 || value < -90)
                            {
                                message = "DIP value for record " + holeAttr + " at field '" + fieldType + "' for hole '" + hole + "' is out of range - DIP = " + fieldValue;

                                check.validationMessages.Add(message);
                                check.ValidationStatus.Add(new DrillholeValidationStatus { ErrorType = DrillholeMessageStatus.Warning, Description = message, ErrorColour = "Orange", id = Convert.ToInt32(holeAttr) });
                                check.verified = false;
                            }
                            else if (value == 0)
                            {
                                message = "DIP value for record " + holeAttr + " at field '" + fieldType + "' for hole '" + hole + " is ZERO";

                                check.ValidationStatus.Add(new DrillholeValidationStatus { ErrorType = DrillholeMessageStatus.Error, Description = message, ErrorColour = "Red", id = Convert.ToInt32(holeAttr) });
                                check.validationMessages.Add(message);
                            }
                            else
                            {
                                message = "DIP value for record " + holeAttr + " at field '" + fieldType + "' for hole '" + hole + " is in range - DIP = " + fieldValue;

                                check.ValidationStatus.Add(new DrillholeValidationStatus { ErrorType = DrillholeMessageStatus.Valid, Description = message, ErrorColour = "Green", id = Convert.ToInt32(holeAttr) });
                                check.validationMessages.Add(message);
                                check.verified = false;

                            }
                        }
                        else
                        {
                            message = "DIP value for record " + holeAttr + " for field '" + fieldID + "' for hole '" + hole + " is not NUMERIC";

                            check.ValidationStatus.Add(new DrillholeValidationStatus { ErrorType = DrillholeMessageStatus.Error, Description = message, ErrorColour = "Red", id = Convert.ToInt32(holeAttr) });

                            check.validationMessages.Add(message);
                            check.verified = false;
                        }

                    }
                    else if (check.validationTest == DrillholeConstants.checkAzi)
                    {
                        holeID = check.tableFields[0].columnHeader;
                        fieldID = check.tableFields[1].columnHeader;
                        fieldType = check.tableFields[1].columnImportAs;

                        hole = element.Element(holeID).Value;
                        fieldValue = element.Element(fieldID).Value;
                        holeAttr = element.Attribute("ID").Value;

                        double value = Convert.ToDouble(fieldValue);

                        if (value > 360 || value < 0)
                        {
                            message = "AZIMUTH value for record " + holeAttr + " at field '" + fieldType + "' for hole '" + hole + "' is out of range - AZIMUTH = " + fieldValue;

                            check.ValidationStatus.Add(new DrillholeValidationStatus { ErrorType = DrillholeMessageStatus.Warning, Description = message, ErrorColour = "Orange", id = Convert.ToInt32(holeAttr) });

                            check.validationMessages.Add(message);
                            check.verified = false;
                        }
                        else if (value == 0)
                        {
                            message = "AZIMUTH value for record " + holeAttr + " at field '" + fieldType + "' for hole '" + hole + "' may be out of range at ZERO?";

                            check.ValidationStatus.Add(new DrillholeValidationStatus { ErrorType = DrillholeMessageStatus.Warning, Description = message, ErrorColour = "Orange", id = Convert.ToInt32(holeAttr) });

                            check.validationMessages.Add(message);
                        }
                        else
                        {
                            message = "AZIMUTH value for record " + holeAttr + " at field '" + fieldType + "' for hole '" + hole + " is in range - AZIMUTH = " + fieldValue;

                            check.ValidationStatus.Add(new DrillholeValidationStatus { ErrorType = DrillholeMessageStatus.Valid, Description = message, ErrorColour = "Green", id = Convert.ToInt32(holeAttr) });
                            check.validationMessages.Add(message);
                        }
                    }

                    else
                    {
                        message = "AZIMUTH value for record " + holeAttr + " for field '" + fieldID + "' for hole '" + hole + " is not NUMERIC";

                        check.ValidationStatus.Add(new DrillholeValidationStatus { ErrorType = DrillholeMessageStatus.Error, Description = message, ErrorColour = "Red", id = Convert.ToInt32(holeAttr) });

                        check.validationMessages.Add(message);
                        check.verified = false;
                    }

                }

            }

            return surveyValidationDto;
        }

        public async Task<ValidationSurveyDto> CheckSurveyDistance(ValidationMessages ValuesToCheck, List<XElement> drillholeValues)
        {
            surveyValidationDto.testMessages = ValuesToCheck;

            foreach (var check in ValuesToCheck.testMessage)
            {
                if (check.validationTest == DrillholeConstants.checkDist)
                {
                    var collarElements = drillholeValues[0].Elements();
                    var surveyElements = drillholeValues[1].Elements();

                    string holeFieldID = check.tableFields[0].columnHeader;
                    string maxField = check.tableFields.Where(o => o.columnImportName == DrillholeConstants.maxName).Select(f => f.columnHeader).Single();
                    string surveyFieldID = check.tableFields[2].columnHeader;
                    string distanceField = check.tableFields.Where(o => o.columnImportName == DrillholeConstants.distName).Select(f => f.columnHeader).Single();

                    var collarHoles = collarElements.Select(c => c.Element(holeFieldID).Value).ToList();
                    var maxValues = collarElements.Select(c => c.Element(maxField).Value).ToList();

                    for (int i = 0; i < collarHoles.Count; i++)
                    {
                        string message = "";

                        string bhid = collarHoles[i];

                        if (Information.IsNumeric(maxValues[i]))
                        {
                            double tD = Convert.ToDouble(maxValues[i]);

                            var surveys = surveyElements.Where(s => s.Element(surveyFieldID).Value == bhid).Select(c => c.Element(distanceField).Value).ToList();

                            List<double> convertSurveys = new List<double>();

                            foreach (var survey in surveys)
                            {
                                if (Information.IsNumeric(survey))
                                {
                                    convertSurveys.Add(Convert.ToDouble(survey));

                                }
                            }

                            if (convertSurveys.Count > 0)
                            {
                                double maxSurvey = convertSurveys.Max();
                                double minSurvey = convertSurveys.Min();

                                if (minSurvey > 0) //if no zero distance, gives a warning
                                {
                                    var survAttr = surveyElements.Where(s => s.Element(surveyFieldID).Value == minSurvey.ToString()).Select(c => c.Attribute("ID").Value).SingleOrDefault();

                                    message = "Survey distance for hole " + bhid + " at survey record " + survAttr + " begins with " + minSurvey.ToString() + ". Normally should begin with ZERO!";
                                    //Add warning
                                    check.validationMessages.Add(message);
                                    check.ValidationStatus.Add(new DrillholeValidationStatus { ErrorType = DrillholeMessageStatus.Warning, Description = message, ErrorColour = "Orange", id = Convert.ToInt32(survAttr) });

                                    check.verified = false;
                                }
                                else
                                {
                                    for (int a = 1; a < convertSurveys.Count; a++)
                                    {

                                        var survAttr = surveyElements.Where(s => s.Element(distanceField).Value  == convertSurveys[a].ToString() && s.Element(surveyFieldID).Value == bhid).Select(c => c.Attribute("ID").Value).FirstOrDefault();



                                        if (convertSurveys[a] > tD) //checks if survey greater than max collar depth
                                        {

                                            message = "Maximum survey distance is greater than the collar length for hole '" + bhid + "' at survey record " + survAttr + ". Survey distance " + convertSurveys[a].ToString() + " > than collar length " + tD.ToString();

                                            check.validationMessages.Add(message);
                                            check.ValidationStatus.Add(new DrillholeValidationStatus { ErrorType = DrillholeMessageStatus.Warning, Description = message, ErrorColour = "Orange", id = Convert.ToInt32(survAttr) });

                                            check.verified = false;
                                        }
                                        else if (convertSurveys[a] < convertSurveys[a - 1]) //checks if survey is less than previous survey. Should always be greater
                                        {
                                            message = "Survey distance " + convertSurveys[a - 1] + " for hole " + bhid + " should be greater than " + convertSurveys[a];

                                            check.ValidationStatus.Add(new DrillholeValidationStatus { ErrorType = DrillholeMessageStatus.Warning, Description = message, ErrorColour = "Orange", id = Convert.ToInt32(survAttr) });

                                            check.verified = false;
                                        }
                                        else //valid survey
                                        {
                                            message = "Survey distance " + convertSurveys[a] + " for hole " + bhid + " is valid";

                                            check.validationMessages.Add(message);
                                            check.ValidationStatus.Add(new DrillholeValidationStatus { ErrorType = DrillholeMessageStatus.Valid, Description = message, ErrorColour = "Green", id = Convert.ToInt32(survAttr) });
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return surveyValidationDto;
        }

        #endregion
    }
}

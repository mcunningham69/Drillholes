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
    public class ContinuousValidation : IContinuousValidation
    {
        private ValidationContinuousDto continuousValidationDto;
        public ContinuousValidation()
        {
            continuousValidationDto = new ValidationContinuousDto();

            continuousValidationDto.isValid = true;

        }

        #region Messages
        public async Task<ValidationContinuousDto> CheckForDuplicates(ValidationMessages ValuesToCheck, XElement continuousValues)
        {
            continuousValidationDto.testMessages = ValuesToCheck;

            var elements = continuousValues.Elements();

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
                             "  times for the following records: " + idList;

                            check.ValidationStatus.Add(new DrillholeValidationStatus
                            {
                                ErrorType = DrillholeMessageStatus.Warning,
                                Description = message,
                                ErrorColour = "Orange",
                                id = Convert.ToInt32(holeAttr.First().ToString()),
                                holeID = hole
                            });

                            check.validationMessages.Add(message);
                        }
                    }
                    else
                    {
                        message = "There are no duplicate values in the Collar table for hole " + hole +
                        ".";

                        check.validationMessages.Add(message);
                        check.ValidationStatus.Add(new DrillholeValidationStatus { ErrorType = DrillholeMessageStatus.Valid, Description = message, ErrorColour = "Green" });
                    }
                }
            }

            return continuousValidationDto;
        }

        public async Task<ValidationContinuousDto> CheckForMissingCollars(ValidationMessages ValuesToCheck, List<XElement> drillholeValues)
        {
            continuousValidationDto.testMessages = ValuesToCheck;

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

                    message = "Collar and Continuous table have same ordinal hole IDs";

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
                            message = "Collar and Continuous table have same hole IDs";

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
                                    message = "Hole '" + dist + "' has no corresponding hole in the Continuous table";
                                    check.ValidationStatus.Add(new DrillholeValidationStatus
                                    {
                                        ErrorType = DrillholeMessageStatus.Warning,
                                        Description = message,
                                        ErrorColour = "Orange",
                                        id = -999,
                                        holeID = dist
                                    });
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

                                    message = "Continuous hole '" + dist + "' has no corresponding value in the Collar table";
                                    check.ValidationStatus.Add(new DrillholeValidationStatus
                                    {
                                        ErrorType = DrillholeMessageStatus.Warning,
                                        Description = message,
                                        ErrorColour = "Orange",
                                        id = Convert.ToInt32(holeAttr),
                                        holeID = dist
                                    });
                                    check.validationMessages.Add(message);

                                    counter++;
                                }
                            }
                        }
                    }
                }
            }

            return continuousValidationDto;
        }

        private async Task<bool> CompareLists(List<string> collarHoles, List<string> continuousHoles)
        {

            if (collarHoles.Count == continuousHoles.Count)
            {
                var filteredSequence = collarHoles.Where(x => continuousHoles.Contains(x));
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


        public async Task<ValidationContinuousDto> CheckForNumeric(ValidationMessages ValuesToCheck, XElement surveyValues)
        {
            continuousValidationDto.testMessages = ValuesToCheck;

            string holeName = "";
            int counter = 0;

            foreach (var check in ValuesToCheck.testMessage)
            {
                if (counter == 0)
                {
                    holeName = check.tableField.columnHeader;
                }
                else
                {
                    string fieldID = check.tableField.columnHeader;
                    string fieldName = check.tableField.columnImportAs;

                    CheckNumericValues(surveyValues, check, fieldID, fieldName, holeName);

                }
                counter++;
            }

            return continuousValidationDto;

        }

        public async Task<ValidationContinuousDto> CheckIsEmpty(ValidationMessages ValuesToCheck, XElement continuousValues)
        {
            continuousValidationDto.testMessages = ValuesToCheck;

            string holeName = "";
            int counter = 0;

            foreach (var check in ValuesToCheck.testMessage)
            {
                if (counter == 0)
                    holeName = check.tableField.columnHeader;

                string fieldID = check.tableField.columnHeader;
                string fieldName = check.tableField.columnImportAs;

                CheckEmptyValues(continuousValues, check, fieldID, fieldName, holeName);

                counter++;
            }

            return continuousValidationDto;
        }

        private async void CheckEmptyValues(XElement continuousValues, ValidationMessage validationTest, string fieldID, string fieldName, string holeName)
        {
            var elements = continuousValues.Elements();
            validationTest.count = elements.Count();

            foreach (XElement element in elements)
            {
                string message = "";
                string valueCheck = element.Element(fieldID).Value;
                string holeAttr = element.Attribute("ID").Value;
                string hole = element.Element(holeName).Value;


                if (string.IsNullOrEmpty(valueCheck) || string.IsNullOrWhiteSpace(valueCheck) || valueCheck == "-")
                {
                    DrillholeMessageStatus _errorType = DrillholeMessageStatus.Error;
                    string _errorColour = "Red";

                    if (fieldName == "Numeric")
                    {
                        _errorType = DrillholeMessageStatus.Warning;
                        _errorColour = "Orange";
                    }

                    message = "Continuous table: for record " + holeAttr + " and field '" + fieldID + "' of field type '"
                        + fieldName + "' has no value";

                    validationTest.validationMessages.Add(message);
                    validationTest.ValidationStatus.Add(new DrillholeValidationStatus { ErrorType = _errorType, Description = message, ErrorColour = _errorColour, id = Convert.ToInt32(holeAttr), holeID = hole });

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
        private async void CheckNumericValues(XElement drillholeValues, ValidationMessage validationTest, string fieldID, string fieldName, string holeName)
        {
            var elements = drillholeValues.Elements();
            validationTest.count = elements.Count();

            foreach (XElement element in elements)
            {
                string fieldValue = element.Element(fieldID).Value;
                string holeAttr = element.Attribute("ID").Value;
                string hole = element.Element(holeName).Value;
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
                    validationTest.ValidationStatus.Add(new DrillholeValidationStatus { ErrorType = _errorType, Description = message, ErrorColour = _errorColour, id = Convert.ToInt32(holeAttr), holeID = hole });
                    validationTest.verified = false;
                }
                else
                {
                    message = "'" + fieldID + "' of field type '" + fieldName + "' verified as NUMERIC";

                    validationTest.validationMessages.Add(message);
                    validationTest.ValidationStatus.Add(new DrillholeValidationStatus { ErrorType = DrillholeMessageStatus.Valid, Description = message, ErrorColour = "Green", id = Convert.ToInt32(holeAttr), holeID = hole });

                }
            }
        }



        public async Task<ValidationContinuousDto> CheckMaxDepth(ValidationMessages ValuesToCheck, List<XElement> drillholeValues)
        {
            continuousValidationDto.testMessages = ValuesToCheck;

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
                       // string continuousFieldHoleID = check.tableFields[2].columnHeader;
                        string distanceField = check.tableFields.Where(o => o.columnImportName == DrillholeConstants.distName).Select(f => f.columnHeader).Single();

                        var collar = element.Element(holeFieldID).Value;
                        var length = element.Element(maxField).Value;
                        var distanceQuery = distanceElements.Where(c => c.Element(holeFieldID).Value == collar.ToString()).ToList();

                        double tD = 0.0;

                        if (Information.IsNumeric(length))
                        {
                            tD = Convert.ToDouble(length);

                            List<double> distanceVals = new List<double>();

                            foreach (var value in distanceQuery)
                            {
                                string holeAttr = value.Attribute("ID").Value;

                                string message = "";

                                string distance = value.Element(distanceField).Value;

                                if (Information.IsNumeric(distance))
                                {
                                    distanceVals.Add(Convert.ToDouble(distance));
                                }

                                if (distanceVals.Count > 0)
                                {
                                    double continuousMaxDepth = distanceVals.Max();

                                    if (continuousMaxDepth > tD)
                                    {
                                        message = "Continuous record: " + holeAttr + " has a maximum distance of " + continuousMaxDepth.ToString() + " which is greater than collar length " + tD;

                                        check.validationMessages.Add(message);
                                        check.ValidationStatus.Add(new DrillholeValidationStatus { ErrorType = DrillholeMessageStatus.Warning, Description = message, ErrorColour = "Orange", id = Convert.ToInt32(holeAttr), holeID = collar }); ;
                                        check.verified = false;
                                    }
                                    else
                                    {
                                        message = "Continuous max. distance of " + continuousMaxDepth + " for hole " + collar + " is valid";
                                        check.ValidationStatus.Add(new DrillholeValidationStatus { ErrorType = DrillholeMessageStatus.Valid, Description = message, ErrorColour = "Green", id = Convert.ToInt32(holeAttr), holeID = collar });
                                        check.validationMessages.Add(message);
                                    }
                                }
                            }
                        }
                    }

                }
            }

            return continuousValidationDto;
        }


        public async Task<ValidationContinuousDto> CheckDistance(ValidationMessages ValuesToCheck, List<XElement> drillholeValues)
        {
            continuousValidationDto.testMessages = ValuesToCheck;

            foreach (var check in ValuesToCheck.testMessage)
            {
                if (check.validationTest == DrillholeConstants.checkDist)
                {
                    var collarElements = drillholeValues[0].Elements();
                    var continuousElements = drillholeValues[1].Elements();

                    string holeFieldID = check.tableFields[0].columnHeader;
                    string maxField = check.tableFields.Where(o => o.columnImportName == DrillholeConstants.maxName).Select(f => f.columnHeader).Single();
                    string continuousFieldID = check.tableFields[2].columnHeader;
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

                            var continuous = continuousElements.Where(s => s.Element(continuousFieldID).Value == bhid).Select(c => c.Element(distanceField).Value).ToList();

                            List<double> convertValues = new List<double>();

                            foreach (var value in continuous)
                            {
                                if (Information.IsNumeric(value))
                                {
                                    convertValues.Add(Convert.ToDouble(value));

                                }
                            }

                            if (convertValues.Count > 0)
                            {
                                //double maxValue = convertValues.Max();
                                double minValue = convertValues.Min();

                                if (minValue > 0) //if no zero distance, gives a warning
                                {
                                    var contAttr = continuousElements.Where(s => s.Element(continuousFieldID).Value == minValue.ToString()).Select(c => c.Attribute("ID").Value).SingleOrDefault();

                                    message = "Continuous distance for hole " + bhid + " at record " + contAttr + " begins with " + minValue.ToString() + ".";
                                    //Add warning
                                    check.validationMessages.Add(message);
                                    check.ValidationStatus.Add(new DrillholeValidationStatus { ErrorType = DrillholeMessageStatus.Warning, Description = message, ErrorColour = "Orange", id = Convert.ToInt32(contAttr), holeID = bhid });

                                    check.verified = false;
                                }
                                else
                                {
                                    for (int a = 1; a < convertValues.Count; a++)
                                    {

                                        var contAttr = continuousElements.Where(s => s.Element(distanceField).Value == convertValues[a].ToString() && s.Element(continuousFieldID).Value == bhid).Select(c => c.Attribute("ID").Value).FirstOrDefault();



                                        if (convertValues[a] > tD) //checks if survey greater than max collar depth
                                        {

                                            message = "Maximum distance is greater than the collar length for hole '" + bhid + "' at record " + contAttr + ". Continuous distance " + convertValues[a].ToString() + " > than collar length " + tD.ToString();

                                            check.validationMessages.Add(message);
                                            check.ValidationStatus.Add(new DrillholeValidationStatus { ErrorType = DrillholeMessageStatus.Warning, Description = message, ErrorColour = "Orange", id = Convert.ToInt32(contAttr), holeID = bhid });

                                            check.verified = false;
                                        }
                                        else if (convertValues[a] < convertValues[a - 1]) //checks if survey is less than previous survey. Should always be greater
                                        {
                                            message = "Continuous distance " + convertValues[a - 1] + " for hole " + bhid + " should be greater than " + convertValues[a];

                                            check.ValidationStatus.Add(new DrillholeValidationStatus { ErrorType = DrillholeMessageStatus.Warning, Description = message, ErrorColour = "Orange", id = Convert.ToInt32(contAttr), holeID = bhid });

                                            check.verified = false;
                                        }
                                        else //valid survey
                                        {
                                            message = "Continuous distance " + convertValues[a] + " for hole " + bhid + " is valid";

                                            check.validationMessages.Add(message);
                                            check.ValidationStatus.Add(new DrillholeValidationStatus { ErrorType = DrillholeMessageStatus.Valid, Description = message, ErrorColour = "Green", id = Convert.ToInt32(contAttr) });
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return continuousValidationDto;
        }

        public async Task<ValidationContinuousDto> CheckStructuralMeasurements(ValidationMessages ValuesToCheck, XElement continuousValues)
        {
            continuousValidationDto.testMessages = ValuesToCheck;

            string holeName = "";
            int counter = 0;

            var elements = continuousValues.Elements();

            foreach (var check in ValuesToCheck.testMessage)
            {
                check.count = elements.Count();

                string holeID = "";
                string fieldID = "";
                string fieldType = "";

                foreach (XElement element in continuousValues.Elements())
                {
                    string hole = "";
                    string fieldValue = "";
                    string holeAttr = "";
                    string message = "";

                    switch (check.validationTest)
                    {
                        case DrillholeConstants.checkAlpha:
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
                                    message = "Alpha value for record " + holeAttr + " at field '" + fieldType + "' for hole '" + hole + "' is out of range: " + fieldValue;

                                    check.validationMessages.Add(message);
                                    check.ValidationStatus.Add(new DrillholeValidationStatus { ErrorType = DrillholeMessageStatus.Error, Description = message, ErrorColour = "Red", id = Convert.ToInt32(holeAttr), holeID = hole });
                                    check.verified = false;
                                }
                                else if (value == 0)
                                {
                                    message = "Alpha value for record " + holeAttr + " at field '" + fieldType + "' for hole '" + hole + " is ZERO";

                                    check.ValidationStatus.Add(new DrillholeValidationStatus { ErrorType = DrillholeMessageStatus.Warning, Description = message, ErrorColour = "Orange", id = Convert.ToInt32(holeAttr), holeID = hole });
                                    check.validationMessages.Add(message);
                                }
                                else
                                {
                                    message = "Alpha value for record " + holeAttr + " at field '" + fieldType + "' for hole '" + hole + " is in range: " + fieldValue;

                                    check.ValidationStatus.Add(new DrillholeValidationStatus { ErrorType = DrillholeMessageStatus.Valid, Description = message, ErrorColour = "Green", id = Convert.ToInt32(holeAttr), holeID = hole });
                                    check.validationMessages.Add(message);
                                    check.verified = false;

                                }
                            }
                            else
                            {
                                message = "Alpha value for record " + holeAttr + " for field '" + fieldID + "' for hole '" + hole + " is not NUMERIC";

                                check.ValidationStatus.Add(new DrillholeValidationStatus { ErrorType = DrillholeMessageStatus.Error, Description = message, ErrorColour = "Red", id = Convert.ToInt32(holeAttr), holeID = hole });

                                check.validationMessages.Add(message);
                                check.verified = false;
                            }
                            break;
                        case DrillholeConstants.checkBeta:
                            holeID = check.tableFields[0].columnHeader;
                            fieldID = check.tableFields[1].columnHeader;
                            fieldType = check.tableFields[1].columnImportAs;

                            hole = element.Element(holeID).Value;
                            fieldValue = element.Element(fieldID).Value;
                            holeAttr = element.Attribute("ID").Value;

                            if (Information.IsNumeric(fieldValue))
                            {
                                double value = Convert.ToDouble(fieldValue);

                                if (value > 360 || value < 360)
                                {
                                    message = "Beta value for record " + holeAttr + " at field '" + fieldType + "' for hole '" + hole + "' is out of range: " + fieldValue;

                                    check.validationMessages.Add(message);
                                    check.ValidationStatus.Add(new DrillholeValidationStatus { ErrorType = DrillholeMessageStatus.Error, Description = message, ErrorColour = "Red", id = Convert.ToInt32(holeAttr), holeID = hole });
                                    check.verified = false;
                                }
                                else if (value == 0)
                                {
                                    message = "Beta value for record " + holeAttr + " at field '" + fieldType + "' for hole '" + hole + " is ZERO";

                                    check.ValidationStatus.Add(new DrillholeValidationStatus { ErrorType = DrillholeMessageStatus.Warning, Description = message, ErrorColour = "Orange", id = Convert.ToInt32(holeAttr), holeID = hole });
                                    check.validationMessages.Add(message);
                                }
                                else
                                {
                                    message = "Beta value for record " + holeAttr + " at field '" + fieldType + "' for hole '" + hole + " is in range: " + fieldValue;

                                    check.ValidationStatus.Add(new DrillholeValidationStatus { ErrorType = DrillholeMessageStatus.Valid, Description = message, ErrorColour = "Green", id = Convert.ToInt32(holeAttr), holeID = hole });
                                    check.validationMessages.Add(message);
                                    check.verified = false;

                                }
                            }
                            else
                            {
                                message = "Beta value for record " + holeAttr + " for field '" + fieldID + "' for hole '" + hole + " is not NUMERIC";

                                check.ValidationStatus.Add(new DrillholeValidationStatus { ErrorType = DrillholeMessageStatus.Error, Description = message, ErrorColour = "Red", id = Convert.ToInt32(holeAttr), holeID = hole });

                                check.validationMessages.Add(message);
                                check.verified = false;
                            }
                            break;
                        case DrillholeConstants.checkGamma:
                            holeID = check.tableFields[0].columnHeader;
                            fieldID = check.tableFields[1].columnHeader;
                            fieldType = check.tableFields[1].columnImportAs;

                            hole = element.Element(holeID).Value;
                            fieldValue = element.Element(fieldID).Value;
                            holeAttr = element.Attribute("ID").Value;

                            if (Information.IsNumeric(fieldValue))
                            {
                                double value = Convert.ToDouble(fieldValue);

                                if (value > 360 || value < -360)
                                {
                                    message = "Gamma value for record " + holeAttr + " at field '" + fieldType + "' for hole '" + hole + "' is out of range: " + fieldValue;

                                    check.validationMessages.Add(message);
                                    check.ValidationStatus.Add(new DrillholeValidationStatus { ErrorType = DrillholeMessageStatus.Error, Description = message, ErrorColour = "Red", id = Convert.ToInt32(holeAttr), holeID = hole });
                                    check.verified = false;
                                }
                                else if (value == 0)
                                {
                                    message = "Gamma value for record " + holeAttr + " at field '" + fieldType + "' for hole '" + hole + " is ZERO";

                                    check.ValidationStatus.Add(new DrillholeValidationStatus { ErrorType = DrillholeMessageStatus.Warning, Description = message, ErrorColour = "Orange", id = Convert.ToInt32(holeAttr), holeID = hole });
                                    check.validationMessages.Add(message);
                                }
                                else
                                {
                                    message = "Gamma value for record " + holeAttr + " at field '" + fieldType + "' for hole '" + hole + " is in range: " + fieldValue;

                                    check.ValidationStatus.Add(new DrillholeValidationStatus { ErrorType = DrillholeMessageStatus.Valid, Description = message, ErrorColour = "Green", id = Convert.ToInt32(holeAttr), holeID = hole });
                                    check.validationMessages.Add(message);
                                    check.verified = false;

                                }
                            }
                            else
                            {
                                message = "Gamma value for record " + holeAttr + " for field '" + fieldID + "' for hole '" + hole + " is not NUMERIC";

                                check.ValidationStatus.Add(new DrillholeValidationStatus { ErrorType = DrillholeMessageStatus.Error, Description = message, ErrorColour = "Red", id = Convert.ToInt32(holeAttr), holeID = hole });

                                check.validationMessages.Add(message);
                                check.verified = false;
                            }
                            break;
                    }
                }
            }

            return continuousValidationDto;
        }


        #endregion
    }
}

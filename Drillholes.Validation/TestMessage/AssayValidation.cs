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
    public class AssayValidation : IAssayValidation
    {
        private ValidationAssayDto assayValidationDto;
        public AssayValidation()
        {
            assayValidationDto = new ValidationAssayDto();

            assayValidationDto.isValid = true;
        }


        public async Task<ValidationAssayDto> CheckForDuplicates(ValidationMessages ValuesToCheck, XElement assayValues)
        {
            assayValidationDto.testMessages = ValuesToCheck;

            var elements = assayValues.Elements();

            foreach (var check in ValuesToCheck.testMessage)
            {
                check.count = elements.Count();
                string fieldID = check.tableFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Select(f => f.columnHeader).Single();
                string distField = check.tableFields.Where(o => o.columnImportName == DrillholeConstants.distFromName).Select(f => f.columnHeader).Single();

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

                            check.ValidationStatus.Add(new DrillholeValidationStatus { ErrorType = DrillholeMessageStatus.Warning, Description = message, ErrorColour = "Orange", id = Convert.ToInt32(holeAttr) });

                            check.validationMessages.Add(message);
                        }
                        check.verified = false;
                    }
                    else
                    {
                        message = "There are no duplicates for " + hole;
                        check.ValidationStatus.Add(new DrillholeValidationStatus { ErrorType = DrillholeMessageStatus.Valid, Description = message, ErrorColour = "Green" });

                        check.validationMessages.Add(message);
                    }
                }
            }

            return assayValidationDto;
        }

        public async Task<ValidationAssayDto> CheckForMissingCollars(ValidationMessages ValuesToCheck, List<XElement> drillholeValues)
        {
            assayValidationDto.testMessages = ValuesToCheck;
            string message = "";
            foreach (var check in ValuesToCheck.testMessage)
            {
                if (check.validationTest == DrillholeConstants.checkHole)
                {
                    var collarElements = drillholeValues[0].Elements();
                    var assayElements = drillholeValues[1].Elements();

                    check.count = collarElements.Count();

                    string holeFieldID = check.tableFields[0].columnHeader;
                    string assayFieldID = check.tableFields[1].columnHeader;

                    var collarHoles = collarElements.Select(c => c.Element(holeFieldID).Value).ToList();
                    var distanceHoles = assayElements.GroupBy(x => x.Element(assayFieldID).Value).Where(group => group.Count() > 0).Select(group => group.Key).ToList();

                    message = "Collar and Assay table have same ordinal hole IDs";

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
                            message = "Collar and Assay table have same hole IDs";

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
                                    message = "Hole '" + dist + "' has no corresponding hole in the Assay table";
                                    check.ValidationStatus.Add(new DrillholeValidationStatus { ErrorType = DrillholeMessageStatus.Warning, Description = message, ErrorColour = "Orange" });
                                    check.validationMessages.Add(message);
                                }
                            }

                            if (distanceHoles.Except(collarHoles).Count() > 0)
                            {
                                var noDistances = distanceHoles.Except(collarHoles);

                                foreach (string dist in noDistances)
                                {
                                    message = "Assay hole '" + dist + "' has no corresponding value in the Collar table";
                                    check.ValidationStatus.Add(new DrillholeValidationStatus { ErrorType = DrillholeMessageStatus.Warning, Description = message, ErrorColour = "Orange" });
                                    check.validationMessages.Add(message);
                                }
                            }
                        }
                    }
                }
            }

            return assayValidationDto;
        }

        private async Task<bool> CompareLists(List<string> collarHoles, List<string> otherHoles)
        {

            if (collarHoles.Count == otherHoles.Count)
            {
                var filteredSequence = collarHoles.Where(x => otherHoles.Contains(x));
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


        public async Task<ValidationAssayDto> CheckForNumeric(ValidationMessages ValuesToCheck, XElement assayValues)
        {
            assayValidationDto.testMessages = ValuesToCheck;

            foreach (var check in ValuesToCheck.testMessage)
            {
                string fieldID = check.tableField.columnHeader;
                string fieldName = check.tableField.columnImportAs;

                CheckNumericValues(assayValues, check, fieldID, fieldName);
            }

            return assayValidationDto;
        }

        public async Task<ValidationAssayDto> CheckIsEmpty(ValidationMessages ValuesToCheck, XElement assayValues)
        {
            assayValidationDto.testMessages = ValuesToCheck;

            foreach (var check in ValuesToCheck.testMessage)
            {
                string fieldID = check.tableField.columnHeader;
                string fieldName = check.tableField.columnImportAs;

                CheckEmptyValues(assayValues, check, fieldID, fieldName);
            }

            return assayValidationDto;
        }

        private async void CheckEmptyValues(XElement assayValues, ValidationMessage validationTest, string fieldID, string fieldName)
        {
            var elements = assayValues.Elements();

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

                    message = "Assay record " + holeAttr + " and field '" + fieldID + "' of field type '"
                        + fieldName + "' has no value";

                    validationTest.validationMessages.Add(message);
                    validationTest.ValidationStatus.Add(new DrillholeValidationStatus
                    {
                        ErrorType = _errorType,
                        Description = message,
                        ErrorColour = _errorColour,
                        id = Convert.ToInt32(holeAttr)
                    });

                    validationTest.verified = false;
                }
                else
                {
                    message = "'" + fieldID + "' of field type '" + fieldName + "' with record " + holeAttr + "' verified as not EMPTY or WHITESPACE";

                    validationTest.validationMessages.Add(message);
                    validationTest.ValidationStatus.Add(new DrillholeValidationStatus
                    { ErrorType = DrillholeMessageStatus.Valid, Description = message, ErrorColour = "Green", id = Convert.ToInt32(holeAttr) });

                }
            }

        }
        private async void CheckNumericValues(XElement drillholeValues, ValidationMessage validationTest, string fieldID, string fieldName)
        {
            var elements = drillholeValues.Elements();
            validationTest.count = elements.Count();

            foreach (XElement element in elements)
            {
                string message = "";
                string fieldValue = element.Element(fieldID).Value;
                string holeAttr = element.Attribute("ID").Value;

                if (!Information.IsNumeric(fieldValue))
                {
                    DrillholeMessageStatus _errorType = DrillholeMessageStatus.Error;
                    string _errorColour = "Red";

                    if (fieldName == "Numeric")
                    {
                        _errorType = DrillholeMessageStatus.Warning;
                        _errorColour = "Orange";
                    }

                    message = "Assay value for record " + holeAttr + " and field '" + fieldID + "' of field type '"
                       + fieldName + "' is not NUMERIC";

                    validationTest.validationMessages.Add(message);
                    validationTest.ValidationStatus.Add(new DrillholeValidationStatus
                    {
                        ErrorType = _errorType,
                        Description = message,
                        ErrorColour = _errorColour,
                        id = Convert.ToInt32(holeAttr)
                    });
                    validationTest.verified = false;

                }
                else
                {
                    message = "'" + fieldID + "' of field type '" + fieldName + "' with record " + holeAttr + "' verified as NUMERIC";
                    validationTest.validationMessages.Add(message);
                    validationTest.ValidationStatus.Add(new DrillholeValidationStatus
                    {
                        ErrorType = DrillholeMessageStatus.Valid,
                        Description = message,
                        ErrorColour = "Green",
                        id = Convert.ToInt32(holeAttr)
                    });
                }
            }

        }


        public async Task<ValidationAssayDto> CheckMaxDepth(ValidationMessages ValuesToCheck, List<XElement> drillholeValues)
        {
            assayValidationDto.testMessages = ValuesToCheck;

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
                        string toField = check.tableFields.Where(o => o.columnImportName == DrillholeConstants.distToName).Select(f => f.columnHeader).Single();

                        var collar = element.Element(holeFieldID).Value;
                        var length = element.Element(maxField).Value;
                        var intervalQuery = distanceElements.Where(c => c.Element(holeFieldID).Value == collar.ToString()).ToList();
                        var holeIDs = distanceElements.Where(c => c.Element(holeFieldID).Value == collar.ToString()).Select(a => a.Attribute("ID")).ToList();

                        double tD = 0.0;

                        if (Information.IsNumeric(length))
                        {
                            tD = Math.Round(Convert.ToDouble(length), 1);

                            List<double> assayIntervals = new List<double>();

                            string message = "";

                            foreach (var value in intervalQuery)
                            {
                                string distance = value.Element(toField).Value;

                                if (Information.IsNumeric(distance))
                                {
                                    assayIntervals.Add(Convert.ToDouble(distance));
                                }
                            }

                            if (assayIntervals.Count > 0)
                            {
                                double assayMaxDepth = assayIntervals.Max();

                                if (assayMaxDepth > tD)
                                {
                                    message = "Assay value has a maximum distance of " + assayMaxDepth.ToString() +
                                        " which is greater than  length " + tD + " for collar " + collar;

                                    check.validationMessages.Add(message);
                                    check.ValidationStatus.Add(new DrillholeValidationStatus
                                    {
                                        ErrorType = DrillholeMessageStatus.Warning,
                                        Description = message,
                                        ErrorColour = "Orange",
                                        id = Convert.ToInt32(holeIDs.Last())
                                    });
                                    check.verified = false;
                                }
                                else
                                {
                                    string submessge = "less than";
                                    if (tD == assayMaxDepth)
                                        submessge = "equal to";

                                    message = "Assay max. distance of " + assayMaxDepth + " for hole " + collar + " is " + submessge + " the total length of " + tD;
                                    check.ValidationStatus.Add(new DrillholeValidationStatus
                                    {
                                        ErrorType = DrillholeMessageStatus.Valid,
                                        Description = message,
                                        ErrorColour = "Green",
                                        id = Convert.ToInt32(holeIDs.Last())
                                    });
                                    check.validationMessages.Add(message);
                                }
                            }

                        }
                    }
                }
            }

            return assayValidationDto;
        }

        public async Task<ValidationAssayDto> CheckMissingIntervals(ValidationMessages ValuesToCheck, List<XElement> drillholeValues)
        {
            assayValidationDto.testMessages = ValuesToCheck;

            foreach (var check in ValuesToCheck.testMessage)
            {
                string message = "";

                if (check.validationTest == DrillholeConstants.checkFromTo)
                {
                    var collarElements = drillholeValues[0].Elements();
                    var intervalElements = drillholeValues[1].Elements();

                    string collarFieldID = check.tableFields[0].columnHeader;  //hole field in collar table
                    string lengthID = check.tableFields[1].columnHeader;  //max depth of trace
                    string assayFieldID = check.tableFields[2].columnHeader;  //hole field in assay table
                    string fromField = check.tableFields[3].columnHeader;
                    string toField = check.tableFields[4].columnHeader;  //to field

                    var intervalHoles = intervalElements.GroupBy(x => x.Element(assayFieldID).Value).Where(group => group.Count() > 0).Select(group => group.Key).ToList();

                    foreach (var hole in intervalHoles)
                    {
                        var lenVal = collarElements.Where(t => t.Element(collarFieldID).Value == hole).Select(c => c.Element(lengthID).Value).Single();
                        var fromVals = intervalElements.Where(s => s.Element(assayFieldID).Value == hole).Select(c => c.Element(fromField).Value).ToList();
                        var toVals = intervalElements.Where(s => s.Element(assayFieldID).Value == hole).Select(c => c.Element(toField).Value).ToList();
                        var holeAttr = intervalElements.Where(s => s.Element(collarFieldID).Value == hole).Select(c => c.Attribute("ID").Value).ToList();

                        double holeLength = 0.0;

                        if (Information.IsNumeric(lenVal))
                            holeLength = Math.Round(Convert.ToDouble(lenVal), 1);

                        List<double> dblFrom = new List<double>();
                        List<double> dblTo = new List<double>();

                        if (fromVals.Count > 0)
                        {
                            for (int d = 0; d < fromVals.Count; d++)
                            {
                                if (Information.IsNumeric(fromVals[d]) && Information.IsNumeric(toVals[d]))
                                {
                                    dblFrom.Add(Convert.ToDouble(fromVals[d]));
                                    dblTo.Add(Convert.ToDouble(toVals[d]));
                                }
                            }

                            for (int v = 0; v < dblFrom.Count; v++)
                            {
                                double dblF = dblFrom[v];
                                double dblT = dblTo[v];

                                if (v == 0)
                                {
                                    if (dblF > 0)
                                    {
                                        message = "Missing Interval between 0 and " + dblF.ToString() + " for hole " + hole;
                                        check.validationMessages.Add(message);
                                        check.ValidationStatus.Add(new DrillholeValidationStatus
                                        {
                                            ErrorType = DrillholeMessageStatus.Warning,
                                            Description = message,
                                            ErrorColour = "Orange",
                                            id = Convert.ToInt32(holeAttr[v])
                                        });
                                        check.verified = false;
                                    }
                                }

                                else if (v > 0)
                                {
                                    dblT = dblTo[v - 1];

                                    if (v == dblFrom.Count - 1)
                                    {
                                        if (holeLength > 0)
                                        {
                                            if (dblTo[v] < holeLength)
                                            {
                                                message = "Missing Interval between " + dblTo[v].ToString() + " and end of hole at '" + holeLength.ToString() + "' for hole " + hole;
                                                check.validationMessages.Add(message);
                                                check.ValidationStatus.Add(new DrillholeValidationStatus
                                                {
                                                    ErrorType = DrillholeMessageStatus.Warning,
                                                    Description = message,
                                                    ErrorColour = "Orange",
                                                    id = Convert.ToInt32(holeAttr[v])
                                                });
                                                check.verified = false;
                                            }
                                        }
                                    }

                                    else if (dblF > dblT)
                                    {
                                        message = "Missing Interval between " + dblTo[v - 1].ToString() + " and " + dblFrom[v].ToString() + " for hole " + hole;
                                        check.validationMessages.Add(message);
                                        check.ValidationStatus.Add(new DrillholeValidationStatus
                                        {
                                            ErrorType = DrillholeMessageStatus.Warning,
                                            Description = message,
                                            ErrorColour = "Orange",
                                            id = Convert.ToInt32(holeAttr[v - 1])
                                        });
                                        check.verified = false;
                                    }
                                }
                            }
                        }
                    }
                }
                if (check.ValidationStatus.Count == 0)
                {
                    message = "There are no missing intervals";
                    check.ValidationStatus.Add(new DrillholeValidationStatus { ErrorType = DrillholeMessageStatus.Valid, Description = message, ErrorColour = "Green" });
                }
            }

            return assayValidationDto;
        }

        public async Task<ValidationAssayDto> CheckNegativeIntervals(ValidationMessages ValuesToCheck, XElement drillholeValues)
        {
            assayValidationDto.testMessages = ValuesToCheck;

            foreach (var check in ValuesToCheck.testMessage)
            {
                string message = "";
                if (check.validationTest == DrillholeConstants.checkFromTo)
                {
                    var intervalElements = drillholeValues.Elements();

                    string holeFieldID = check.tableFields[0].columnHeader;  //hole field
                    string fromField = check.tableFields[1].columnHeader;
                    string toField = check.tableFields[2].columnHeader;  //hole field

                    var intervalHoles = intervalElements.GroupBy(x => x.Element(holeFieldID).Value).Where(group => group.Count() > 0).Select(group => group.Key).ToList();

                    foreach (var hole in intervalHoles)
                    {
                        var fromVals = intervalElements.Where(s => s.Element(holeFieldID).Value == hole).Select(c => c.Element(fromField).Value).ToList();
                        var toVals = intervalElements.Where(s => s.Element(holeFieldID).Value == hole).Select(c => c.Element(toField).Value).ToList();
                        var holeAttrs = intervalElements.Where(i => i.Element(holeFieldID).Value == hole).Select(a => a.Attribute("ID").Value).ToList();

                        double dblFrom = 0.0;
                        double dblTo = 0.0;

                        if (fromVals.Count > 0)
                        {
                            for (int d = 0; d < fromVals.Count; d++)
                            {
                                if (Information.IsNumeric(fromVals[d]))
                                {
                                    dblFrom = Convert.ToDouble(fromVals[d]);

                                    if (Information.IsNumeric(toVals[d]))
                                    {
                                        dblTo = Convert.ToDouble(toVals[d]);

                                        double dblDifference = dblTo - dblFrom;

                                        if (dblTo == 0)
                                        {
                                            message = "Interval is zero for hole " + hole + " at record " + holeAttrs[d] + "!";
                                            check.validationMessages.Add(message);
                                            check.ValidationStatus.Add(new DrillholeValidationStatus
                                            {
                                                ErrorType = DrillholeMessageStatus.Error,
                                                Description = message,
                                                ErrorColour = "Red",
                                                id = Convert.ToInt32(holeAttrs[d])
                                            });

                                            check.verified = false;
                                        }
                                        else if (dblTo < 0)
                                        {
                                            message = "'To' is negative for hole " + hole + " at record " + holeAttrs[d] + "!";// 'From'(" + dblFrom.ToString() + ") is greater than 'To'(" + dblTo.ToString() + ")";
                                            check.ValidationStatus.Add(new DrillholeValidationStatus
                                            {
                                                ErrorType = DrillholeMessageStatus.Warning,
                                                Description = message,
                                                ErrorColour = "Orange",
                                                id = Convert.ToInt32(holeAttrs[d])
                                            });
                                            check.validationMessages.Add(message);
                                            check.verified = false;
                                        }
                                        else if (dblTo < dblFrom)
                                        {
                                            message = "Interval is negative for hole " + hole + " at interval + " + holeAttrs[d] + "! 'From' (" + dblFrom.ToString() + ") is greater than 'To' (" + dblTo.ToString() + ")";

                                            check.validationMessages.Add(message);
                                            check.ValidationStatus.Add(new DrillholeValidationStatus
                                            {
                                                ErrorType = DrillholeMessageStatus.Error,
                                                Description = message,
                                                ErrorColour = "Red",
                                                id = Convert.ToInt32(holeAttrs[d])
                                            });
                                            check.verified = false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (check.ValidationStatus.Count == 0)
                {
                    message = "There are no negative intervals";
                    check.ValidationStatus.Add(new DrillholeValidationStatus { ErrorType = DrillholeMessageStatus.Valid, Description = message, ErrorColour = "Green" });
                }
            }

            return assayValidationDto;
        }

        public async Task<ValidationAssayDto> CheckOverlappingIntervals(ValidationMessages ValuesToCheck, XElement drillholeValues)
        {
            assayValidationDto.testMessages = ValuesToCheck;

            foreach (var check in ValuesToCheck.testMessage)
            {
                string message = "";

                if (check.validationTest == DrillholeConstants.checkFromTo)
                {
                    var intervalElements = drillholeValues.Elements();

                    string holeFieldID = check.tableFields[0].columnHeader;  //hole field
                    string fromField = check.tableFields[1].columnHeader;
                    string toField = check.tableFields[2].columnHeader;  //hole field

                    var intervalHoles = intervalElements.GroupBy(x => x.Element(holeFieldID).Value).Where(group => group.Count() > 0).Select(group => group.Key).ToList();

                    foreach (var hole in intervalHoles)
                    {
                        var holeIDs = intervalElements.Where(s => s.Element(holeFieldID).Value == hole).Select(c => c.Attribute("ID").Value).ToList();
                        var fromVals = intervalElements.Where(s => s.Element(holeFieldID).Value == hole).Select(c => c.Element(fromField).Value).ToList();
                        var toVals = intervalElements.Where(s => s.Element(holeFieldID).Value == hole).Select(c => c.Element(toField).Value).ToList();

                        List<double> dblFrom = new List<double>();
                        List<double> dblTo = new List<double>();

                        if (fromVals.Count > 0)
                        {
                            for (int d = 0; d < fromVals.Count; d++)
                            {
                                if (Information.IsNumeric(fromVals[d]) && Information.IsNumeric(toVals[d]))
                                {
                                    dblFrom.Add(Convert.ToDouble(fromVals[d]));
                                    dblTo.Add(Convert.ToDouble(toVals[d]));
                                }
                            }

                            for (int v = 0; v < dblFrom.Count; v++)
                            {
                                double dblF = dblFrom[v];
                                double dblT = dblTo[v];

                                if (v > 0)
                                {
                                    dblT = dblTo[v - 1];

                                    if (dblF < dblT)
                                    {
                                        if (dblF != dblFrom[v - 1] && dblTo[v] != dblT) //testing for duplicate
                                        {
                                            message = "Overlapping Interval between " + dblF.ToString() + " and " + dblT.ToString() + " for hole " + hole;

                                            check.validationMessages.Add(message);
                                            check.ValidationStatus.Add(new DrillholeValidationStatus
                                            {
                                                ErrorType = DrillholeMessageStatus.Warning,
                                                Description = message,
                                                ErrorColour = "Orange",
                                                id = Convert.ToInt32(holeIDs[v])
                                            });
                                            check.verified = false;
                                        }
                                    }
                                }
                            }

                        }
                    }

                    if (check.ValidationStatus.Count == 0)
                    {
                        message = "There are no overlapping intervals";
                        check.ValidationStatus.Add(new DrillholeValidationStatus { ErrorType = DrillholeMessageStatus.Valid, Description = message, ErrorColour = "Green" });
                    }
                }
            }

            return assayValidationDto;
        }
    }
}

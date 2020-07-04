using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drillholes.Domain.Enum;
using Drillholes.Domain.Interfaces;
using Drillholes.Domain.Exceptions;
using Drillholes.Domain.DTO;
using Drillholes.Domain;
using System.Xml.Linq;
using Microsoft.VisualBasic;

namespace Drillholes.Validation.TestMessage
{
    public class CollarValidation : ICollarValidation
    {
        private ValidationCollarDto validationCollarDto;
        public CollarValidation(DrillholeSurveyType surveyType)
        {
            validationCollarDto = new ValidationCollarDto();

            validationCollarDto.isValid = true;
        }

        public async Task<ValidationCollarDto> CheckForDuplicates(ValidationMessages ValuesToCheck, XElement collarValues)
        {
            validationCollarDto.testMessages = ValuesToCheck;

            var elements = collarValues.Elements();

            foreach (var check in ValuesToCheck.testMessage)
            {
                string fieldID = check.tableField.columnHeader;

                check.count = elements.Count();

                string message = "";

                var showElements = elements.Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE");

                var duplicates = showElements.GroupBy(x => x.Element(fieldID).Value).Where(group => group.Count() > 1).Select(group => group.Key).ToList();

               // var duplicates = elements.GroupBy(x => x.Element(fieldID).Value).Where(group => group.Count() > 1).Select(group => group.Key).ToList();

                if (duplicates.Count > 0)
                {
                    foreach (string hole in duplicates)
                    {
                        var holeAttr = elements.Where(y => y.Element(fieldID).Value == hole).Select(z => String.Join(hole, z.Attribute("ID").Value)).ToList();

                        string idList = String.Join(", ", holeAttr.ToArray());
                        int noDups = holeAttr.Count();

                        message = "The following hole '" + hole + "' of field type ' "
                      + check.tableField.columnImportName + "' is repeated " + noDups.ToString() +
                            "  times for the following records: " + idList;

                        foreach (string attr in holeAttr)
                        {
                            check.ValidationStatus.Add(new DrillholeValidationStatus
                            {
                                ErrorType = DrillholeMessageStatus.Warning,
                                Description = message,
                                ErrorColour = "Orange",
                                id = Convert.ToInt32(attr.ToString()),
                                holeID = hole
                            });

                            check.validationMessages.Add(message);
                        }

                       
                    }
                    check.verified = false;
                }
                else
                {
                    message = "There are no duplicate in the Collar table with a total of " + check.count +
                        " holes";

                    check.validationMessages.Add(message);
                    check.ValidationStatus.Add(new DrillholeValidationStatus
                    {
                        ErrorType = DrillholeMessageStatus.Valid,
                        Description = message,
                        ErrorColour = "Green"
                    });

                }

            }

            return validationCollarDto;
        }

        public async Task<ValidationCollarDto> CheckForNumeric(ValidationMessages ValuesToCheck, XElement collarValues)
        {
            validationCollarDto.testMessages = ValuesToCheck;

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

                    CheckNumericValues(collarValues, check, fieldID, fieldName, holeName);
                }

                counter++;
            }

            return validationCollarDto;
        }

        private async void CheckNumericValues(XElement drillholeValues, ValidationMessage validationTest, string fieldID, string fieldName, string holeName)
        {

            var elements = drillholeValues.Elements();
            var showElements = elements.Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE");

            validationTest.count = showElements.Count();

            foreach (XElement element in showElements)
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

                    validationTest.ValidationStatus.Add(new DrillholeValidationStatus
                    {
                        ErrorType = _errorType,
                        Description = message,
                        ErrorColour = _errorColour,
                        id = Convert.ToInt32(holeAttr),
                        holeID = hole
                    });
                    validationTest.verified = false;
                }
                else
                {
                    message = "'" + fieldID + "' of field type '" + fieldName + "' verified as NUMERIC";

                    validationTest.validationMessages.Add(message);
                    validationTest.ValidationStatus.Add(new DrillholeValidationStatus
                    {
                        ErrorType = DrillholeMessageStatus.Valid,
                        Description = message,
                        ErrorColour = "Green",
                        id = Convert.ToInt32(holeAttr),
                        holeID = hole
                    });
                }
            }

        }

        public async Task<ValidationCollarDto> CheckForZeroCoordinate(ValidationMessages _ValuesToCheck, XElement collarValues)
        {
            validationCollarDto.testMessages = _ValuesToCheck;

            var elements = collarValues.Elements();
            var showElements = elements.Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE");


            foreach (var check in _ValuesToCheck.testMessage)
            {
                string fieldID = check.tableFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Select(f => f.columnHeader).Single();
                string xField = check.tableFields.Where(o => o.columnImportName == DrillholeConstants.xName).Select(f => f.columnHeader).Single();
                string yField = check.tableFields.Where(o => o.columnImportName == DrillholeConstants.yName).Select(f => f.columnHeader).Single();
                string xFieldType = check.tableFields.Where(o => o.columnImportName == DrillholeConstants.xName).Select(f => f.columnImportAs).Single();
                string yFieldType = check.tableFields.Where(o => o.columnImportName == DrillholeConstants.yName).Select(f => f.columnImportAs).Single();

                check.count = elements.Count();

                foreach (XElement element in showElements)
                {
                    string hole = element.Element(fieldID).Value;
                    string xCoord = element.Element(xField).Value;
                    string yCoord = element.Element(yField).Value;
                    string holeAttr = element.Attribute("ID").Value;

                    string message = "";

                    if (Information.IsNumeric(xCoord))
                    {
                        double xVal = Convert.ToDouble(xCoord);

                        if (Information.IsNumeric(yCoord))
                        {
                            double yVal = Convert.ToDouble(yCoord);

                            if (xVal == 0)
                            {
                                message = "The X coordinate for hole " + hole + "  is zero at record " + holeAttr;
                                check.validationMessages.Add("Warning: the X coordinate for hole " + hole + "  is zero at record " + holeAttr);

                                check.ValidationStatus.Add(new DrillholeValidationStatus
                                {
                                    ErrorType = DrillholeMessageStatus.Warning,
                                    Description = message,
                                    ErrorColour = "Orange",
                                    id = Convert.ToInt32(holeAttr),
                                    holeID = hole
                                });

                                check.verified = false;

                            }
                            if (yVal == 0)
                            {
                                message = "The Y coordinate for hole " + hole + "  is zero at record " + holeAttr;

                                check.validationMessages.Add(message);
                                check.ValidationStatus.Add(new DrillholeValidationStatus
                                {
                                    ErrorType = DrillholeMessageStatus.Warning,
                                    Description = message,
                                    ErrorColour = "Orange",
                                    id = Convert.ToInt32(holeAttr),
                                    holeID = hole
                                }) ;

                                check.verified = false;

                            }
                            if (xVal == yVal)
                            {
                                if (xVal == 0)
                                {
                                    message = "Both X and Y coordinates for hole " + hole + "  at record " + holeAttr + " are zero";

                                    check.validationMessages.Add(message);

                                    check.ValidationStatus.Add(new DrillholeValidationStatus
                                    {
                                        ErrorType = DrillholeMessageStatus.Warning,
                                        Description = message,
                                        ErrorColour = "Orange",
                                        id = Convert.ToInt32(holeAttr)
                                    });

                                    check.verified = false;
                                }
                                else
                                {
                                    message = "Both X and Y coordinates for hole " + hole + " at record " + holeAttr + " are the same!";

                                    check.validationMessages.Add(message);

                                    check.ValidationStatus.Add(new DrillholeValidationStatus
                                    {
                                        ErrorType = DrillholeMessageStatus.Warning,
                                        Description = message,
                                        ErrorColour = "Orange",
                                        id = Convert.ToInt32(holeAttr)
                                    });

                                    check.verified = false;
                                }
                            }
                            else
                            {
                                message = "X and Y coordinates for hole " + hole + " seem fine";

                                check.ValidationStatus.Add(new DrillholeValidationStatus
                                {
                                    ErrorType = DrillholeMessageStatus.Valid,
                                    Description = message,
                                    ErrorColour = "Green",
                                    id = Convert.ToInt32(holeAttr)
                                });

                                check.validationMessages.Add(message);

                            }
                        }
                    }
                    else
                    {
                        message = "Coordinates for hole " + hole + " at record " + holeAttr + " may not be NUMERIC";

                        check.ValidationStatus.Add(new DrillholeValidationStatus
                        {
                            ErrorType = DrillholeMessageStatus.Error,
                            Description = message,
                            ErrorColour = "Red",
                            id = Convert.ToInt32(holeAttr)
                        });

                        check.validationMessages.Add(message);
                        check.verified = false;
                    }

                }

            }

            return validationCollarDto;
        }

        public async Task<ValidationCollarDto> CheckIsEmpty(ValidationMessages ValuesToCheck, XElement collarValues)
        {
            validationCollarDto.testMessages = ValuesToCheck;
            string holeName = "";
            int counter = 0;

            foreach (var check in ValuesToCheck.testMessage)
            {
                if (counter == 0)
                    holeName = check.tableField.columnHeader;

                string fieldID = check.tableField.columnHeader;
                string fieldName = check.tableField.columnImportAs;

                CheckEmptyValues(collarValues, check, fieldID, fieldName, holeName);

                counter++;
            }

            return validationCollarDto;
        }

        private async void CheckEmptyValues(XElement collarValues, ValidationMessage validationTest, string fieldID, string fieldName, string holeName)
        {
            var elements = collarValues.Elements();
            var showElements = elements.Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE");

            validationTest.count = elements.Count();

            foreach (XElement element in showElements)
            {
                string valueCheck = element.Element(fieldID).Value;
                string holeAttr = element.Attribute("ID").Value;
                string hole = element.Element(holeName).Value;

                string message = "";

                DrillholeMessageStatus _errorType = DrillholeMessageStatus.Error;
                string _errorColour = "Red";

                if (fieldName == "Numeric")
                {
                    _errorType = DrillholeMessageStatus.Warning;
                    _errorColour = "Orange";
                }

                if (string.IsNullOrEmpty(valueCheck) || string.IsNullOrWhiteSpace(valueCheck))
                {
                    message = "Value for record " + holeAttr + " and field '" + fieldID + "' of field type '"
                        + fieldName + "' IS EMPTY";
                    validationTest.validationMessages.Add(message);

                    validationTest.ValidationStatus.Add(new DrillholeValidationStatus
                    {
                        ErrorType = _errorType,
                        Description = message,
                        ErrorColour = _errorColour,
                        id = Convert.ToInt32(holeAttr),
                        holeID = hole
                    });

                    validationTest.verified = false;
                }
                else if (valueCheck.ToString() == "-")
                {
                    message = "Value for record " + holeAttr + " and field '" + fieldID + "' of field type '"
                        + fieldName + "' IS EMPTY";
                    validationTest.validationMessages.Add(message);

                    validationTest.ValidationStatus.Add(new DrillholeValidationStatus
                    {
                        ErrorType = _errorType,
                        Description = message,
                        ErrorColour = _errorColour,
                        id = Convert.ToInt32(holeAttr),
                        holeID = hole
                    });

                    validationTest.verified = false;
                }
                else
                {
                    message = "'" + fieldID + "' of field type '" + fieldName + "' with record " + holeAttr + " is verified";
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


        public async Task<ValidationCollarDto> CheckMaxDepth(ValidationMessages _ValuesToCheck, XElement collarValues)
        {
            validationCollarDto.testMessages = _ValuesToCheck;

            var elements = collarValues.Elements();

            var showElements = elements.Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE");

            foreach (var check in _ValuesToCheck.testMessage)
            {
                if (check.validationTest == DrillholeConstants.checkTD)
                {
                    string fieldID = check.tableFields[0].columnHeader; //HoleID
                    string depthID = check.tableFields[1].columnHeader; //HoleID
                    string depthType = check.tableFields[1].columnImportAs; //HoleID

                    check.count = elements.Count();

                    foreach (XElement element in showElements)
                    {
                        string hole = element.Element(fieldID).Value;
                        string depthValue = element.Element(depthID).Value;
                        string holeAttr = element.Attribute("ID").Value;

                        string message = "";
                        if (Information.IsNumeric(depthValue))
                        {
                            double value = Convert.ToDouble(depthValue);

                            if (value <= 0)
                            {
                                message = "Drillhole length field '" + depthType + "for hole " + hole + " with record  value '" + holeAttr +
                                     "' must be greater than ZERO";

                                check.ValidationStatus.Add(new DrillholeValidationStatus
                                {
                                    ErrorType = DrillholeMessageStatus.Error,
                                    Description = message,
                                    ErrorColour = "Red",
                                    id = Convert.ToInt32(holeAttr),
                                    holeID = hole
                                });
                                check.validationMessages.Add(message);
                                check.verified = false;

                            }
                            else
                            {
                                message = "Drillhole length field '" + depthID + "' of '" + depthType + "' has a value of " + depthValue + " at record " + holeAttr + " for '" + hole
                                    + "' - verified";

                                check.validationMessages.Add(message);

                                check.ValidationStatus.Add(new DrillholeValidationStatus
                                {
                                    ErrorType = DrillholeMessageStatus.Valid,
                                    Description = message,
                                    ErrorColour = "Green",
                                    id = Convert.ToInt32(holeAttr),
                                    holeID = hole
                                });
                            }
                        }
                        else
                        {
                            message = "Drillhole length with '" + depthType + "' for hole '" + hole
                                + "' with record " + holeAttr + " is not NUMERIC";

                            check.validationMessages.Add(message);
                            check.ValidationStatus.Add(new DrillholeValidationStatus
                            {
                                ErrorType = DrillholeMessageStatus.Error,
                                Description = message,
                                ErrorColour = "Red",
                                id = Convert.ToInt32(holeAttr),
                                holeID = hole
                            });

                            check.verified = false;
                        }
                    }

                }
            }
            return validationCollarDto;
        }

        public async Task<ValidationCollarDto> CheckRange(ValidationMessages _ValuesToCheck, XElement collarValues)
        {
            validationCollarDto.testMessages = _ValuesToCheck;

            var elements = collarValues.Elements();

            var showElements = elements.Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE");

            foreach (var check in _ValuesToCheck.testMessage)
            {
                check.count = collarValues.Elements().Count();

                string holeID = "";
                string fieldID = "";
                string fieldType = "";

                foreach (XElement element in showElements)
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

                                check.ValidationStatus.Add(new DrillholeValidationStatus
                                {
                                    ErrorType = DrillholeMessageStatus.Warning,
                                    Description = message,
                                    ErrorColour = "Orange",
                                    id = Convert.ToInt32(holeAttr),
                                    holeID = hole
                                });

                                check.validationMessages.Add(message);
                                check.verified = false;
                            }
                            else
                            {
                                message = "DIP value for record " + holeAttr + " at field '" + fieldType + "' for hole '" + hole + " is in range - DIP = " + fieldValue;

                                check.validationMessages.Add(message);
                                check.ValidationStatus.Add(new DrillholeValidationStatus
                                {
                                    ErrorType = DrillholeMessageStatus.Valid,
                                    Description = message,
                                    ErrorColour = "Green",
                                    id = Convert.ToInt32(holeAttr),
                                    holeID = hole
                                });
                            }
                        }
                        else
                        {
                            message = "DIP value for record " + holeAttr + " for field '" + fieldID + "' for hole '" + hole + " is not NUMERIC";

                            check.ValidationStatus.Add(new DrillholeValidationStatus
                            {
                                ErrorType = DrillholeMessageStatus.Error,
                                Description = message,
                                ErrorColour = "Red",
                                id = Convert.ToInt32(holeAttr),
                                holeID = hole
                            });

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

                        if (Information.IsNumeric(fieldValue))
                        {
                            double value = Convert.ToDouble(fieldValue);

                            if (value > 360 || value < 0)
                            {
                                message = "AZIMUTH value for record " + holeAttr + " at field '" + fieldType + "' for hole '" + hole + "' is out of range - AZIMUTH = " + fieldValue;

                                check.ValidationStatus.Add(new DrillholeValidationStatus
                                {
                                    ErrorType = DrillholeMessageStatus.Warning,
                                    Description = message,
                                    ErrorColour = "Orange",
                                    id = Convert.ToInt32(holeAttr),
                                    holeID = hole
                                });

                                check.validationMessages.Add(message);
                                check.verified = false;
                            }
                            else
                            {
                                message = "AZIMUTH value for record " + holeAttr + " at field '" + fieldType + "' for hole '" + hole + " is in range - AZIMUTH = " + fieldValue;

                                check.ValidationStatus.Add(new DrillholeValidationStatus
                                {
                                    ErrorType = DrillholeMessageStatus.Valid,
                                    Description = message,
                                    ErrorColour = "Green",
                                    id = Convert.ToInt32(holeAttr),
                                    holeID = hole
                                });
                                check.validationMessages.Add(message);
                            }
                        }
                        else
                        {
                            message = "AZIMUTH value for record " + holeAttr + " for field '" + fieldID + "' for hole '" + hole + " is not NUMERIC";

                            check.ValidationStatus.Add(new DrillholeValidationStatus
                            {
                                ErrorType = DrillholeMessageStatus.Error,
                                Description = message,
                                ErrorColour = "Red",
                                id = Convert.ToInt32(holeAttr),
                                holeID = hole
                            });

                            check.validationMessages.Add(message);
                            check.verified = false;
                        }
                    }
                }
            }

            return validationCollarDto;
        }


    }
}

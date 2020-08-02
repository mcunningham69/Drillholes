using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drillholes.Domain;
using Drillholes.Domain.Interfaces;
using Drillholes.Domain.Enum;
using Drillholes.Domain.DTO;
using Drillholes.Domain.DataObject;
using Drillholes.Domain.Services;
using Drillholes.Validation;
using AutoMapper;
using System.Xml.Linq;
using Drillholes.Validation.TestMessage;

namespace Drillholes.Windows.ViewModel
{
    public class IntervalValidationView : AssayValidationView
    {
        public IntervalValidationService _intervalValidationService;
        public IIntervalValidation _intervalValidateValues;

        public ImportTableFields importIntervalFields { get; set; }

        private XElement _xmlIntervalData;
        public XElement xmlIntervalData
        {
            get
            {
                return this._xmlIntervalData;
            }
            set
            {
                this._xmlIntervalData = value;
                OnPropertyChanged("xmlIntervalData");
            }
        }

        public IntervalValidationView(DrillholeTableType _tableType, XElement _xmlIntervalData) : base(_tableType, _xmlIntervalData)
        {
            tableLabel = (_tableType.ToString() + " Table").ToUpper();

            xmlIntervalData = _xmlIntervalData;

            _intervalValidateValues = new IntervalValidation();

            _intervalValidationService = new IntervalValidationService(_intervalValidateValues);
        }

        public override async Task<bool> ValidateAllTables(bool editData)
        {
            ValidationDelegate mTables = null;
            mTables += CheckForEmptyFields;
            mTables += CheckForDuplicates;
            mTables += CheckNumericFields;
            mTables += CheckMaxDepth;
            mTables += CheckCollarsIntervals;
            mTables += CheckForNegativeIntervals;
            mTables += CheckForMissingIntervals;
            mTables += CheckForOverlappingIntervals;
            mTables += CheckStructuralMeasurements;

            return await mTables(editData);
        }

        public override void InitialiseMapping()
        {
            if (_intervalValidateValues == null)
                _intervalValidateValues = new IntervalValidation();

            var config = new MapperConfiguration(cfg => { cfg.CreateMap<ValidationIntervalDto, ValidationInterval>(); });

            mapper = config.CreateMapper();

            var source = new ValidationIntervalDto();

            var dest = mapper.Map<ValidationIntervalDto, ValidationInterval>(source);

            _intervalValidationService = new IntervalValidationService(_intervalValidateValues);

        }

        #region Validation
        public override async Task<bool> CheckForEmptyFields(bool editData)
        {
            if (mapper == null)
                InitialiseMapping();

            List<ValidationMessage> intervalFieldTest = new List<ValidationMessage>();

            intervalFieldTest.Add(new ValidationMessage
            {
                verified = true,
                validationTest = DrillholeConstants.checkHole,
                count = 0,
                validationMessages = new List<string>(),
                tableField = importIntervalFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Single()
            });

            intervalFieldTest.Add(new ValidationMessage
            {
                verified = true,
                validationTest = DrillholeConstants.checkFrom,
                count = 0,
                validationMessages = new List<string>(),
                tableField = importIntervalFields.Where(o => o.columnImportName == DrillholeConstants.distFromName).Where(m => m.genericType == false).Single()
            });

            intervalFieldTest.Add(new ValidationMessage
            {
                verified = true,
                validationTest = DrillholeConstants.checkTo,
                count = 0,
                validationMessages = new List<string>(),
                tableField = importIntervalFields.Where(o => o.columnImportName == DrillholeConstants.distToName).Where(m => m.genericType == false).Single()
            });

            //check for numeric 
            if (importIntervalFields.Where(g => g.columnImportAs == "Numeric").Where(p => p.genericType == true).Count() > 0)
            {
                var isNumericField = importIntervalFields.Where(g => g.columnImportAs == "Numeric").Where(p => p.genericType == true).ToList();

                foreach (var field in isNumericField)
                {
                    intervalFieldTest.Add(new ValidationMessage
                    {
                        verified = true,
                        count = 0,
                        validationTest = DrillholeConstants.checkNumeric,
                        validationMessages = new List<string>(),
                        tableField = field

                    });

                }

            }


            ValidationMessages intervalTests = new ValidationMessages { testType = DrillholeConstants.IsEmptyOrNull, testMessage = intervalFieldTest };

            var validationCheck = await _intervalValidationService.CheckIsEmpty(mapper, intervalTests, xmlIntervalData);

            DisplayMessages.DisplayResults.Add(validationCheck.testMessages);

            return true;
        }
        public override async Task<bool> CheckForDuplicates(bool editData)
        {
            if (mapper == null)
                InitialiseMapping();

            ImportTableField holeField = importIntervalFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Single();
            ImportTableField fromField = importIntervalFields.Where(o => o.columnImportName == DrillholeConstants.distFromName).Where(m => m.genericType == false).Single();

            List<ImportTableField> tempFields = new List<ImportTableField>();
            tempFields.Add(holeField);
            tempFields.Add(fromField);

            List<ValidationMessage> intervalFieldTest = new List<ValidationMessage>();
            intervalFieldTest.Add(new ValidationMessage
            {
                verified = true,
                validationTest = DrillholeConstants.checkFrom,
                count = 0,
                validationMessages = new List<string>(),
                tableFields = tempFields
            });

            ValidationMessages intervalTests = new ValidationMessages { testType = DrillholeConstants.Duplicates, testMessage = intervalFieldTest };

            var validationCheck = await _intervalValidationService.CheckDuplicates(mapper, intervalTests, xmlIntervalData);

            DisplayMessages.DisplayResults.Add(validationCheck.testMessages);

            return true;
        }
        public override async Task<bool> CheckNumericFields(bool editData)
        {
            if (mapper == null)
                InitialiseMapping();

            List<ValidationMessage> intervalFieldTest = new List<ValidationMessage>();
            intervalFieldTest.Add(new ValidationMessage
            {
                verified = true,
                validationTest = DrillholeConstants.checkHole,
                count = 0,
                validationMessages = new List<string>(),
                tableField = importIntervalFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Single()
            });
            intervalFieldTest.Add(new ValidationMessage
            {
                verified = true,
                validationTest = DrillholeConstants.checkFrom,
                count = 0,
                validationMessages = new List<string>(),
                tableField = importIntervalFields.Where(o => o.columnImportName == DrillholeConstants.distFromName).Where(m => m.genericType == false).Single()
            });

            intervalFieldTest.Add(new ValidationMessage
            {
                verified = true,
                validationTest = DrillholeConstants.checkTo,
                count = 0,
                validationMessages = new List<string>(),
                tableField = importIntervalFields.Where(o => o.columnImportName == DrillholeConstants.distToName).Where(m => m.genericType == false).Single()
            });

            //check for numeric
            if (importIntervalFields.Where(g => g.columnImportAs == "Numeric").Where(p => p.genericType == true).Count() > 0)
            {
                var isNumericField = importIntervalFields.Where(g => g.columnImportAs == "Numeric").Where(p => p.genericType == true).ToList();

                foreach (var field in isNumericField)
                {
                    intervalFieldTest.Add(new ValidationMessage
                    {
                        verified = true,
                        count = 0,
                        validationTest = DrillholeConstants.checkNumeric,
                        validationMessages = new List<string>(),
                        tableField = field

                    });

                }

            }

            ValidationMessages intervalTests = new ValidationMessages { testType = DrillholeConstants.IsNumeric, testMessage = intervalFieldTest };

            var validationCheck = await _intervalValidationService.CheckForNumeric(mapper, intervalTests, xmlIntervalData);

            DisplayMessages.DisplayResults.Add(validationCheck.testMessages);

            return true;
        }
        public override async Task<bool> CheckMaxDepth(bool editData)
        {
            if (mapper == null)
                InitialiseMapping();

            List<XElement> drillholeTableData = new List<XElement>();
            drillholeTableData.Add(xmlCollarData);
            drillholeTableData.Add(xmlIntervalData);

            ImportTableField holeField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Single();
            ImportTableField tdField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.maxName).Where(m => m.genericType == false).Single();
            ImportTableField intervalHoleField = importIntervalFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Single();
            ImportTableField toFields = importIntervalFields.Where(o => o.columnImportName == DrillholeConstants.distToName).Where(m => m.genericType == false).Single();

            List<ImportTableField> tempFields = new List<ImportTableField>();
            tempFields.Add(holeField);
            tempFields.Add(tdField);
            tempFields.Add(intervalHoleField);
            tempFields.Add(toFields);

            List<ValidationMessage> intervalFieldTest = new List<ValidationMessage>();

            //Check if azimuth and dip fields set
            intervalFieldTest.Add(new ValidationMessage
            {
                verified = true,
                validationTest = DrillholeConstants.checkTD,
                count = 0,
                validationMessages = new List<string>(),
                tableFields = tempFields
            });

            ValidationMessages intervalTests = new ValidationMessages { testType = DrillholeConstants.HoleLength, testMessage = intervalFieldTest };

            var validationCheck = await _intervalValidationService.CheckMaxDepth(mapper, intervalTests, drillholeTableData);

            DisplayMessages.DisplayResults.Add(validationCheck.testMessages);

            return true;
        }
        public override async Task<bool> CheckForNegativeIntervals(bool editData)
        {
            if (mapper == null)
                InitialiseMapping();

            ImportTableField intervalHoleField = importIntervalFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Single();
            ImportTableField intervalFromField = importIntervalFields.Where(o => o.columnImportName == DrillholeConstants.distFromName).Where(m => m.genericType == false).Single();
            ImportTableField intervalToField = importIntervalFields.Where(o => o.columnImportName == DrillholeConstants.distToName).Where(m => m.genericType == false).Single();

            List<ImportTableField> tempFields = new List<ImportTableField>();
            tempFields.Add(intervalHoleField);
            tempFields.Add(intervalFromField);
            tempFields.Add(intervalToField);

            List<ValidationMessage> intervalFieldTests = new List<ValidationMessage>();
            intervalFieldTests.Add(new ValidationMessage
            {
                verified = true,
                validationTest = DrillholeConstants.checkFromTo,
                count = 0,
                validationMessages = new List<string>(),
                tableFields = tempFields
            });

            ValidationMessages intervalTests = new ValidationMessages { testType = DrillholeConstants.NegativeOrZeroInterval, testMessage = intervalFieldTests };

            var validationCheck = await _intervalValidationService.CheckNegativeIntervals(mapper, intervalTests, xmlIntervalData);

            DisplayMessages.DisplayResults.Add(validationCheck.testMessages);

            return true;
        }
        public override async Task<bool> CheckForMissingIntervals(bool editData)
        {
            if (mapper == null)
                InitialiseMapping();

            List<XElement> drillholeValues = new List<XElement>();
            drillholeValues.Add(xmlCollarData);
            drillholeValues.Add(xmlIntervalData);

            ImportTableField collarHoleField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Single();
            ImportTableField collarTdField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.maxName).Where(m => m.genericType == false).Single();
            ImportTableField intervalHoleField = importIntervalFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Single();
            ImportTableField intervalFromField = importIntervalFields.Where(o => o.columnImportName == DrillholeConstants.distFromName).Where(m => m.genericType == false).Single();
            ImportTableField intervalToField = importIntervalFields.Where(o => o.columnImportName == DrillholeConstants.distToName).Where(m => m.genericType == false).Single();

            List<ImportTableField> tempFields = new List<ImportTableField>();
            tempFields.Add(collarHoleField);
            tempFields.Add(collarTdField);
            tempFields.Add(intervalHoleField);
            tempFields.Add(intervalFromField);
            tempFields.Add(intervalToField);

            List<ValidationMessage> intervalFieldTest = new List<ValidationMessage>();
            intervalFieldTest.Add(new ValidationMessage
            {
                verified = true,
                validationTest = DrillholeConstants.checkFromTo,
                count = 0,
                validationMessages = new List<string>(),
                tableFields = tempFields
            });

            ValidationMessages intervalTests = new ValidationMessages { testType = DrillholeConstants.MissingInterval, testMessage = intervalFieldTest};

            var validationCheck = await _intervalValidationService.CheckMissingIntervals(mapper, intervalTests, drillholeValues);

            DisplayMessages.DisplayResults.Add(validationCheck.testMessages);

            return true;
        }
        public override async Task<bool> CheckForOverlappingIntervals(bool editData)
        {
            if (mapper == null)
                InitialiseMapping();

            ImportTableField intervalHoleField = importIntervalFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Single();
            ImportTableField intervalFromField = importIntervalFields.Where(o => o.columnImportName == DrillholeConstants.distFromName).Where(m => m.genericType == false).Single();
            ImportTableField intervalToField = importIntervalFields.Where(o => o.columnImportName == DrillholeConstants.distToName).Where(m => m.genericType == false).Single();

            List<ImportTableField> tempFields = new List<ImportTableField>();
            tempFields.Add(intervalHoleField);
            tempFields.Add(intervalFromField);
            tempFields.Add(intervalToField);

            List<ValidationMessage> intervalFieldTest = new List<ValidationMessage>();
            intervalFieldTest.Add(new ValidationMessage
            {
                verified = true,
                validationTest = DrillholeConstants.checkFromTo,
                count = 0,
                validationMessages = new List<string>(),
                tableFields = tempFields
            });

            ValidationMessages intervalTests = new ValidationMessages { testType = DrillholeConstants.OverlappingInterval, testMessage = intervalFieldTest };

            var validationCheck = await _intervalValidationService.CheckOverlappingIntervals(mapper, intervalTests, xmlIntervalData);

            DisplayMessages.DisplayResults.Add(validationCheck.testMessages);

            return true;
        }

        private async Task<bool> CheckCollarsIntervals(bool editData)
        {
            if (mapper == null)
                InitialiseMapping();

            List<XElement> drillholeTableData = new List<XElement>();
            drillholeTableData.Add(xmlCollarData);
            drillholeTableData.Add(xmlIntervalData);

            ImportTableField holeField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Single();
            ImportTableField intervalHoleField = importIntervalFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Single();

            List<ImportTableField> tempFields = new List<ImportTableField>();
            tempFields.Add(holeField);
            tempFields.Add(intervalHoleField);

            List<ValidationMessage> intervalFieldTest = new List<ValidationMessage>();

            //Check if azimuth and dip fields set
            intervalFieldTest.Add(new ValidationMessage
            {
                verified = true,
                validationTest = DrillholeConstants.checkHole,
                count = 0,
                validationMessages = new List<string>(),
                tableFields = tempFields
            });

            ValidationMessages intervalTests = new ValidationMessages { testType = DrillholeConstants.MissingCollar, testMessage = intervalFieldTest };

            var validationCheck = await _intervalValidationService.CheckMissingCollars(mapper, intervalTests, drillholeTableData);

            DisplayMessages.DisplayResults.Add(validationCheck.testMessages);

            return true;
        }

        public virtual async Task<bool> CheckStructuralMeasurements(bool editData)
        {
            if (mapper == null)
                InitialiseMapping();

            bool bAlpha = false;
            bool bBeta = false;
            bool bGamma = false;

            if (importIntervalFields.Where(o => o.columnImportAs == "Alpha").Count() > 0)
                bAlpha = true;

            if (importIntervalFields.Where(o => o.columnImportAs == "Beta").Count() > 0)
                bBeta = true;

            if (importIntervalFields.Where(o => o.columnImportAs == "Gamma").Count() > 0)
                bGamma = true;

            if (!bAlpha && !bBeta && !bGamma)
                return false;


            List<ValidationMessage> intervalFieldTest = new List<ValidationMessage>();

            List<ImportTableField> alphaFields = null;
            List<ImportTableField> betaFields = null;
            List<ImportTableField> gammaFields = null;

            if (bAlpha)
            {
                alphaFields = new List<ImportTableField>();
                alphaFields.Add(importIntervalFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).FirstOrDefault());
                alphaFields.Add(importIntervalFields.Where(o => o.columnImportName == "Alpha").FirstOrDefault());

                intervalFieldTest.Add(new ValidationMessage
                {
                    verified = true,
                    count = 0,
                    validationTest = DrillholeConstants.checkAlpha,
                    validationMessages = new List<string>(),
                    tableFields = alphaFields

                });
            }

            if (bBeta)
            {
                betaFields = new List<ImportTableField>();
                betaFields.Add(importIntervalFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).FirstOrDefault());
                betaFields.Add(importIntervalFields.Where(o => o.columnImportName == "Beta").FirstOrDefault());

                intervalFieldTest.Add(new ValidationMessage
                {
                    verified = true,
                    count = 0,
                    validationTest = DrillholeConstants.checkBeta,
                    validationMessages = new List<string>(),
                    tableFields = betaFields

                });
            }

            if (bGamma)
            {
                gammaFields = new List<ImportTableField>();
                gammaFields.Add(importIntervalFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).FirstOrDefault());
                gammaFields.Add(importIntervalFields.Where(o => o.columnImportName == "Gamma").FirstOrDefault());

                intervalFieldTest.Add(new ValidationMessage
                {
                    verified = true,
                    count = 0,
                    validationTest = DrillholeConstants.checkGamma,
                    validationMessages = new List<string>(),
                    tableFields = gammaFields

                });
            }

            ValidationMessages intervalTests = new ValidationMessages { testType = DrillholeConstants.Structures, testMessage = intervalFieldTest};

            var validationCheck = await _intervalValidationService.CheckStructures(mapper, intervalTests, xmlAssayData);

            DisplayMessages.DisplayResults.Add(validationCheck.testMessages);

            return true;
        }

#endregion
        public override async Task<bool> ReformatResults(ValidationMessage message, string _TestType, string validation, List<string> fields, List<DrillholeMessageStatus> statusMessages, DrillholeTableType tableType,
IEnumerable<XElement> intervalValues)
        {
            foreach (var status in statusMessages)  //iterate three times for each error type
            {
                var count = message.ValidationStatus.Where(e => e.ErrorType == status).Count();  //for checking each status for messages

                int counter = 0; //use this for collar id

                if (count > 0)
                {
                    string holeIDName = importIntervalFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Select(n => n.columnHeader).FirstOrDefault(); //get hole name for querying XML

                    //setup outside the foreach hole loop below as it is the last item to add to the ReshapedToEdit list (which is data bound to the XAML DrillholeEdits form
                    List<GroupByHoles> groupedHoles = new List<GroupByHoles>();

                    List<string> SelectedHoles = new List<string>();
                    List<int> ids = new List<int>();

                    ids = message.ValidationStatus.Where(e => e.ErrorType == status).Select(a => a.id).ToList(); //get IDs of all messages

                    foreach (int id in ids) //get the holes for each record
                    {
                        var value = intervalValues.Where(a => a.Attribute("ID").Value == id.ToString()).Select(h => h.Element(holeIDName).Value).FirstOrDefault();

                        SelectedHoles.Add(value);
                    }

                    var holes = SelectedHoles.GroupBy(x => x).Where(group => group.Count() >= 1).Select(group => group.Key).ToList(); //group the records under each hole

                    bool holeExists = true;
                    bool statusExists = true;

                    foreach (var assayHole in holes)
                    {
                        //set up the classes
                        List<RowsToEdit> rows = new List<RowsToEdit>();  //new
                        List<GroupByTest> groupedTests = new List<GroupByTest>();
                        List<GroupByTable> groupedTables = new List<GroupByTable>();
                        List<GroupByTestField> groupedTestFields = new List<GroupByTestField>();

                        GroupByHoles hole = null;
                        GroupByTable table = null;
                        GroupByTest test = null;
                        GroupByTestField testField = null;
                        ReshapedDataToEdit dataToEdit = null;

                        //note that all these lists and checks reside in the CollarValidationView
                        statusExists = await CheckReshapedData(status); //checks if a previous entry for status exists, and then adds to it.

                        if (!statusExists)
                        {
                            dataToEdit = new ReshapedDataToEdit() { ErrorType = status.ToString(), Ignore = false };
                            EditDrillholeData.Add(dataToEdit);
                            dataToEdit.GroupedHoles = groupedHoles;
                        }
                        else
                        {
                            dataToEdit = await ReturnDataStatus(status);
                        }


                        holeExists = await HoleExist(assayHole, status); //check if hole exists

                        if (!holeExists) //means status and hole don't already exist so add
                        {
                            hole = new GroupByHoles() { Ignore = false, holeid = assayHole };

                            List<GroupByHoles> testGroup = await ReturnGroupedHoles(status);

                            if (testGroup != null)
                                groupedHoles = testGroup;

                            groupedHoles.Add(hole);
                        }
                        else
                        {
                            groupedHoles = await ReturnHolesList(status, assayHole);

                            hole = await ReturnHole(assayHole, status);
                        }

                        bool tableExists = await TableExists(status, assayHole, tableType); //check if table already exists otherwise create new one

                        if (!tableExists)
                        {
                            //check for any other table
                            tableExists = await OtherTableExists(status, assayHole, tableType);

                            if (tableExists)
                            {
                                groupedTables = await ReturnOtherTableList(status, assayHole);
                                table = new GroupByTable() { TableType = tableType.ToString(), Ignore = false };
                                groupedTables.Add(table);
                            }
                            else
                            {

                                table = new GroupByTable() { TableType = tableType.ToString(), Ignore = false };
                                groupedTables.Add(table);
                            }
                        }
                        else
                        {
                            groupedTables = await ReturnTableList(status, assayHole, tableType);
                            table = await ReturnTable(status, assayHole, tableType);
                        }

                        hole.GroupedTables = groupedTables;

                        bool testFieldExists = await TestFieldExists(status, assayHole, tableType, _TestType, validation);

                        if (!testFieldExists)
                        {
                            testField = new GroupByTestField() { Ignore = false, TestField = validation, TableData = rows };
                            groupedTestFields.Add(testField);
                        }
                        else
                        {
                            groupedTestFields = await ReturnTestFields(status, assayHole, tableType, _TestType, validation);

                            testField = await ReturnTestField(status, assayHole, tableType, _TestType, validation);
                        }

                        bool testExists = await TestExists(status, assayHole, tableType, _TestType);

                        if (!testExists)
                        {
                            test = new GroupByTest() { MainTest = _TestType, Ignore = false, TestFields = groupedTestFields };
                            test.TestFields = groupedTestFields;

                            groupedTests = await ReturnTestList(status, assayHole, tableType);

                            groupedTests.Add(test);
                        }
                        else
                        {
                            groupedTests = await ReturnTestList(status, assayHole, tableType, _TestType);


                            test = await ReturnTest(status, assayHole, tableType, _TestType);
                            test.TestFields.Add(testField);

                        }


                        table.GroupedTests = groupedTests;

                        if (_TestType == DrillholeConstants.Duplicates || _TestType == DrillholeConstants.MissingCollar)
                        {

                            var value = intervalValues.Where(a => a.Element(holeIDName).Value == assayHole).Select(v => v.Attribute("ID").Value).FirstOrDefault();
                            string tooltip = message.ValidationStatus.Where(e => e.ErrorType == status && e.id == Convert.ToInt32(value)).Select(p => p.Description).FirstOrDefault();

                            //only need collar id for Duplicates.
                            if (_TestType == DrillholeConstants.Duplicates)
                            {
                                var survIDs = message.ValidationStatus.Where(e => e.ErrorType == status && e.holeID == assayHole).Select(a => a.id).ToList(); //get IDs of all messages

                                foreach (var id in ids)
                                {
                                    rows.Add(new RowsToEdit
                                    {
                                        id_col = counter,
                                        id_ass = id,
                                        testType = _TestType,
                                        validationTest = validation,
                                        Description = tooltip
                                    });

                                }
                            }
                            else if (_TestType == DrillholeConstants.ZeroGrade)
                            {
                                //WRITE  CODE HERE
                                rows.Add(new RowsToEdit
                                {
                                    id_col = counter,
                                    id_ass = 0,
                                    testType = _TestType,
                                    validationTest = validation,
                                    Description = tooltip
                                });

                            }
                            else if (_TestType == DrillholeConstants.MissingCollar)
                            {
                                //WRITE  CODE HERE
                                rows.Add(new RowsToEdit
                                {
                                    id_col = counter,
                                    id_ass = 0,
                                    testType = _TestType,
                                    validationTest = validation,
                                    Description = tooltip
                                });

                            }
                        }

                        else
                        {

                            var survIDs = intervalValues.Where(h => h.Element(fields[0]).Value == assayHole).Select(v => v.Attribute("ID").Value).ToList(); //get the survey IDs for hole to check which records have a message

                            int messageCount = 0;

                            foreach (var _id in survIDs)
                            {
                                if (messageCount == ids.Count) //once all messages reached, bail out of loop
                                    break;

                                int check = Convert.ToInt16(_id);

                                foreach (var value in ids)
                                {
                                    if (value == check) //check survey ID against ID in messages 
                                    {
                                        rows.Add(new RowsToEdit
                                        {
                                            id_col = counter,
                                            id_ass = value,
                                            testType = _TestType,
                                            validationTest = validation,
                                            Description = message.ValidationStatus.Where(e => e.ErrorType == status && e.id == Convert.ToInt32(value)).Select(p => p.Description).FirstOrDefault()
                                        });

                                        messageCount++;
                                        break;
                                    }

                                }
                            }
                        }

                        counter++;

                    }

                }

            }

            return true;
        }


        public override async Task<List<string>> ReturnFieldnamesForXmlQuery() //original in CollarValidationView
        {
            List<string> fields = new List<string>();
            fields.Add(importIntervalFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false) //get name of hole field for querying XML
                .Select(s => s.columnHeader).FirstOrDefault());
            fields.Add(importIntervalFields.Where(o => o.columnImportName == DrillholeConstants.distFromName).Where(m => m.genericType == false) //get other feilds which are required as mandatory, i.e. XYZ and depth
                .Select(s => s.columnHeader).FirstOrDefault());
            fields.Add(importIntervalFields.Where(o => o.columnImportName == DrillholeConstants.distToName).Where(m => m.genericType == false)
                .Select(s => s.columnHeader).FirstOrDefault());


            return fields;
        }
    }
}

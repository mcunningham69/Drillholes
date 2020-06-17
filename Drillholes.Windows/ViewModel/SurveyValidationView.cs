﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drillholes.Domain.Enum;
using Drillholes.Domain.Services;
using Drillholes.Domain.Interfaces;
using Drillholes.Validation.TestMessage;
using AutoMapper;
using Drillholes.Domain.DTO;
using Drillholes.Domain.DataObject;
using Drillholes.Domain;
using System.Xml.Linq;
using Microsoft.Office.Interop.Excel;

namespace Drillholes.Windows.ViewModel
{
    public class SurveyValidationView : CollarValidationView
    {
        public SurveyValidationService _surveyValidationService;
        public ISurveyValidation _surveyValidateValues;

        public ImportTableFields importSurveyFields { get; set; }

        private XElement _xmlSurveyData;
        public XElement xmlSurveyData
        {
            get
            {
                return this._xmlSurveyData;
            }
            set
            {
                this._xmlSurveyData = value;
                OnPropertyChanged("xmlSurveyData");
            }
        }

        public SurveyValidationView(DrillholeTableType _tableType, XElement _xmlSurveyData) : base(_tableType, _xmlSurveyData)
        {
            tableLabel = (_tableType.ToString() + " Table").ToUpper();

            xmlSurveyData = _xmlSurveyData;

            _surveyValidateValues = new SurveyValidation();

            _surveyValidationService = new SurveyValidationService(_surveyValidateValues);
        }


        #region Validation Messages
        public override async Task<bool> ValidateAllTables(bool editData)
        {
            ValidationDelegate mTables = null;
            mTables += CheckForEmptyFields;
            mTables += CheckForDuplicates;
            mTables += CheckNumericFields;
            mTables += CheckMaxDepth;
            mTables += CheckRange;
            mTables += CheckSurveyDistance;
            mTables += CheckCollarsSurveys;

            return await mTables(editData);

        }

        public override void InitialiseMapping()
        {
            if (_surveyValidateValues == null)
                _surveyValidateValues = new SurveyValidation();

            var config = new MapperConfiguration(cfg => { cfg.CreateMap<ValidationSurveyDto, ValidationSurvey>(); });

            mapper = config.CreateMapper();

            var source = new ValidationSurveyDto();

            var dest = mapper.Map<ValidationSurveyDto, ValidationSurvey>(source);

            _surveyValidationService = new SurveyValidationService(_surveyValidateValues);

        }

        public async override Task<bool> CheckForEmptyFields(bool editData)
        {
            if (mapper == null)
                InitialiseMapping();

            List<ValidationMessage> surveyFieldTest = new List<ValidationMessage>();

            surveyFieldTest.Add(new ValidationMessage
            {
                verified = true,
                validationTest = DrillholeConstants.checkHole,
                count = 0,
                validationMessages = new List<string>(),
                tableField = importSurveyFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Single()
            });

            surveyFieldTest.Add(new ValidationMessage
            {
                verified = true,
                validationTest = DrillholeConstants.checkDist,
                count = 0,
                validationMessages = new List<string>(),
                tableField = importSurveyFields.Where(o => o.columnImportName == DrillholeConstants.distName).Where(m => m.genericType == false).Single()
            });

            surveyFieldTest.Add(new ValidationMessage
            {
                verified = true,
                validationTest = DrillholeConstants.checkDip,
                count = 0,
                validationMessages = new List<string>(),
                tableField = importSurveyFields.Where(o => o.columnImportName == DrillholeConstants.dipName).Where(m => m.genericType == false).Single()
            });

            surveyFieldTest.Add(new ValidationMessage
            {
                verified = true,
                validationTest = DrillholeConstants.checkAzi,
                count = 0,
                validationMessages = new List<string>(),
                tableField = importSurveyFields.Where(o => o.columnImportName == DrillholeConstants.azimuthName).Where(m => m.genericType == false).Single()
            });

            var isNumericField = importSurveyFields.Where(o => o.fieldType == "Double").Where(p => p.genericType == true).ToList();

            if (isNumericField.Count > 0)
            {
                foreach (var field in isNumericField)
                {
                    surveyFieldTest.Add(new ValidationMessage
                    {
                        verified = true,
                        count = 0,
                        validationTest = DrillholeConstants.checkNumeric,
                        validationMessages = new List<string>(),
                        tableField = field

                    });

                }
            }

            ValidationMessages surveyTests = new ValidationMessages { testType = DrillholeConstants.IsEmptyOrNull, testMessage = surveyFieldTest };

            var validationCheck = await _surveyValidationService.CheckIsEmpty(mapper, surveyTests, xmlSurveyData);

            DisplayMessages.DisplayResults.Add(validationCheck.testMessages);

            return true;
        }
        public async override Task<bool> CheckForDuplicates(bool editData)
        {
            if (mapper == null)
                InitialiseMapping();

            ImportTableField holeField = importSurveyFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Single();
            ImportTableField distanceField = importSurveyFields.Where(o => o.columnImportName == DrillholeConstants.distName).Where(m => m.genericType == false).Single();

            List<ImportTableField> tempFields = new List<ImportTableField>();
            tempFields.Add(holeField);
            tempFields.Add(distanceField);

            List<ValidationMessage> surveyFieldTest = new List<ValidationMessage>();
            surveyFieldTest.Add(new ValidationMessage
            {
                verified = true,
                validationTest = DrillholeConstants.checkDist,
                count = 0,
                validationMessages = new List<string>(),
                tableFields = tempFields
            });

            ValidationMessages surveyTests = new ValidationMessages { testType = DrillholeConstants.Duplicates, testMessage = surveyFieldTest};

            var validationCheck = await _surveyValidationService.CheckDuplicates(mapper, surveyTests, xmlSurveyData);

            DisplayMessages.DisplayResults.Add(validationCheck.testMessages);

            return true;
        }
        private async Task<bool> CheckSurveyDistance(bool editData)
        {
            if (mapper == null)
                InitialiseMapping();

            List<XElement> drillholeTableData = new List<XElement>();
            drillholeTableData.Add(xmlCollarData);
            drillholeTableData.Add(xmlSurveyData);

            ImportTableField holeField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Single();
            ImportTableField tdField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.maxName).Where(m => m.genericType == false).Single();
            ImportTableField surveyHoleField = importSurveyFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Single();
            ImportTableField distanceField = importSurveyFields.Where(o => o.columnImportName == DrillholeConstants.distName).Where(m => m.genericType == false).Single();

            List<ImportTableField> tempFields = new List<ImportTableField>();
            tempFields.Add(holeField);
            tempFields.Add(tdField);
            tempFields.Add(surveyHoleField);
            tempFields.Add(distanceField);

            List<ValidationMessage> surveyFieldTest = new List<ValidationMessage>();

            //Check if azimuth and dip fields set
            surveyFieldTest.Add(new ValidationMessage
            {
                verified = true,
                validationTest = DrillholeConstants.checkDist,
                count = 0,
                validationMessages = new List<string>(),
                tableFields = tempFields
            });

            ValidationMessages surveyTests = new ValidationMessages { testType = DrillholeConstants.SurveyDistance, testMessage = surveyFieldTest };

            var validationCheck = await _surveyValidationService.CheckSurveyDistance(mapper, surveyTests, drillholeTableData);

            DisplayMessages.DisplayResults.Add(validationCheck.testMessages);
            return true;
        }
        public async override Task<bool> CheckNumericFields(bool editData)
        {
            if (mapper == null)
                InitialiseMapping();

            List<ValidationMessage> surveyFieldTest = new List<ValidationMessage>();
            surveyFieldTest.Add(new ValidationMessage
            {
                verified = true,
                validationTest = DrillholeConstants.checkDist,
                count = 0,
                validationMessages = new List<string>(),
                tableField = importSurveyFields.Where(o => o.columnImportName == DrillholeConstants.distName).Where(m => m.genericType == false).Single()
            });

            surveyFieldTest.Add(new ValidationMessage
            {
                verified = true,
                validationTest = DrillholeConstants.checkDip,
                count = 0,
                validationMessages = new List<string>(),
                tableField = importSurveyFields.Where(o => o.columnImportName == DrillholeConstants.dipName).Where(m => m.genericType == false).Single()
            });

            surveyFieldTest.Add(new ValidationMessage
            {
                verified = true,
                validationTest = DrillholeConstants.checkAzi,
                count = 0,
                validationMessages = new List<string>(),
                tableField = importSurveyFields.Where(o => o.columnImportName == DrillholeConstants.azimuthName).Where(m => m.genericType == false).Single()
            });

            var isNumericField = importSurveyFields.Where(o => o.fieldType == "Double").Where(p => p.genericType == true).ToList();

            if (isNumericField.Count > 0)
            {
                foreach (var field in isNumericField)
                {
                    surveyFieldTest.Add(new ValidationMessage
                    {
                        verified = true,
                        count = 0,
                        validationTest = DrillholeConstants.checkNumeric,
                        validationMessages = new List<string>(),
                        tableField = field

                    });

                }
            }

            ValidationMessages surveyTests = new ValidationMessages { testType = DrillholeConstants.IsNumeric, testMessage = surveyFieldTest };

            var validationCheck = await _surveyValidationService.CheckForNumeric(mapper, surveyTests, xmlSurveyData);

            DisplayMessages.DisplayResults.Add(validationCheck.testMessages);

            return true;
        }
        public async override Task<bool> CheckMaxDepth(bool editData)
        {
            if (mapper == null)
                InitialiseMapping();

            List<XElement> drillholeTableData = new List<XElement>();
            drillholeTableData.Add(xmlCollarData);
            drillholeTableData.Add(xmlSurveyData);

            ImportTableField holeField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Single();
            ImportTableField tdField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.maxName).Where(m => m.genericType == false).Single();
            ImportTableField surveyHoleField = importSurveyFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Single();
            ImportTableField distanceField = importSurveyFields.Where(o => o.columnImportName == DrillholeConstants.distName).Where(m => m.genericType == false).Single();

            List<ImportTableField> tempFields = new List<ImportTableField>();
            tempFields.Add(holeField);
            tempFields.Add(tdField);
            tempFields.Add(surveyHoleField);
            tempFields.Add(distanceField);

            List<ValidationMessage> surveyFieldTest = new List<ValidationMessage>();

            //Check if azimuth and dip fields set
            surveyFieldTest.Add(new ValidationMessage
            {
                verified = true,
                validationTest = DrillholeConstants.checkTD,
                count = 0,
                validationMessages = new List<string>(),
                tableFields = tempFields
            });

            ValidationMessages surveyTests = new ValidationMessages { testType = DrillholeConstants.HoleLength, testMessage = surveyFieldTest };

            var validationCheck = await _surveyValidationService.CheckMaxDepth(mapper, surveyTests, drillholeTableData);

            DisplayMessages.DisplayResults.Add(validationCheck.testMessages);

            return true;
        }

        public async override Task<bool> CheckRange(bool editData)
        {
            if (mapper == null)
                InitialiseMapping();

            ImportTableField holeField = importSurveyFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Single();
            ImportTableField dipField = importSurveyFields.Where(o => o.columnImportName == DrillholeConstants.dipName).Where(m => m.genericType == false).Single();
            ImportTableField aziField = importSurveyFields.Where(o => o.columnImportName == DrillholeConstants.azimuthName).Where(m => m.genericType == false).Single();

            List<ImportTableField> dipFields = new List<ImportTableField>();
            dipFields.Add(holeField);
            dipFields.Add(dipField);

            List<ValidationMessage> surveyFieldTest = new List<ValidationMessage>();

            //Check if azimuth and dip fields set
            surveyFieldTest.Add(new ValidationMessage
            {
                verified = true,
                validationTest = DrillholeConstants.checkDip,
                count = 0,
                validationMessages = new List<string>(),
                tableField = importSurveyFields.Where(o => o.columnImportName == DrillholeConstants.dipName).Where(m => m.genericType == false).Single(),
                tableFields = dipFields
            });

            List<ImportTableField> aziFields = new List<ImportTableField>();
            aziFields.Add(holeField);
            aziFields.Add(aziField);

            surveyFieldTest.Add(new ValidationMessage
            {
                verified = true,
                validationTest = DrillholeConstants.checkAzi,
                count = 0,
                validationMessages = new List<string>(),
                tableField = importSurveyFields.Where(o => o.columnImportName == DrillholeConstants.azimuthName).Where(m => m.genericType == false).Single(),
                tableFields = aziFields
            });

            ValidationMessages surveyTests = new ValidationMessages { testType = DrillholeConstants.SurveyRange, testMessage = surveyFieldTest};

            var validationCheck = await _surveyValidationService.CheckRange(mapper, surveyTests, xmlSurveyData);

            DisplayMessages.DisplayResults.Add(validationCheck.testMessages);

            return true;
        }

        private async Task<bool> CheckCollarsSurveys(bool editData)
        {
            if (mapper == null)
                InitialiseMapping();

            List<XElement> drillholeTableData = new List<XElement>();
            drillholeTableData.Add(xmlCollarData);
            drillholeTableData.Add(xmlSurveyData);

            ImportTableField holeField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Single();
            ImportTableField surveyHoleField = importSurveyFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Single();

            List<ImportTableField> tempFields = new List<ImportTableField>();
            tempFields.Add(holeField);
            tempFields.Add(surveyHoleField);

            List<ValidationMessage> surveyFieldTest = new List<ValidationMessage>();

            //Check if azimuth and dip fields set
            surveyFieldTest.Add(new ValidationMessage
            {
                verified = true,
                validationTest = DrillholeConstants.checkHole,
                count = 0,
                validationMessages = new List<string>(),
                tableFields = tempFields
            });

            ValidationMessages surveyTests = new ValidationMessages { testType = DrillholeConstants.MissingCollar, testMessage = surveyFieldTest };

            var validationCheck = await _surveyValidationService.CheckMissingCollars(mapper, surveyTests, drillholeTableData);

            DisplayMessages.DisplayResults.Add(validationCheck.testMessages);

            return true;
        }
        #endregion


        public override async void ReshapeMessages(DrillholeTableType tableType)
        {
            List<string> fields = await ReturnFieldnamesForXmlQuery();
            List<DrillholeMessageStatus> status = await ReturnStatusAlerts(); //set up three alert states

            var surveyvalues = xmlSurveyData.Elements(); 

            foreach (var test in ShowTestMessages)  //get messages after running validation above
            {
                switch (test.testType) //select on the different testtype
                {
                    case DrillholeConstants.IsEmptyOrNull:

                        foreach (var message in test.testMessage)
                        {
                            if (message.validationTest == DrillholeConstants.checkHole)  //specific field being checked
                            {
                                await ReformatResults(message, DrillholeConstants.IsEmptyOrNull, DrillholeConstants.checkHole, fields, status, tableType, surveyvalues);
                            }
                            else if (message.validationTest == DrillholeConstants.checkDist)
                            {
                                await ReformatResults(message, DrillholeConstants.IsEmptyOrNull, DrillholeConstants.checkDist, fields, status, tableType, surveyvalues);
                            }
                           
                            else if (message.validationTest == DrillholeConstants.checkAzi)
                            {
                                await ReformatResults(message, DrillholeConstants.IsEmptyOrNull, DrillholeConstants.checkAzi, fields, status, tableType, surveyvalues);
                            }
                            else if (message.validationTest == DrillholeConstants.checkDip)
                            {
                                await ReformatResults(message, DrillholeConstants.IsEmptyOrNull, DrillholeConstants.checkDip, fields, status, tableType, surveyvalues);
                            }
                        }
                        break;

                    case DrillholeConstants.IsNumeric:
                        foreach (var message in test.testMessage)
                        {
                            if (message.validationTest == DrillholeConstants.checkDist)
                            {
                                await ReformatResults(message, DrillholeConstants.IsNumeric, DrillholeConstants.checkDist, fields, status, tableType, surveyvalues);
                            }
                            else if (message.validationTest == DrillholeConstants.checkAzi)
                            {
                                await ReformatResults(message, DrillholeConstants.IsNumeric, DrillholeConstants.checkAzi, fields, status, tableType, surveyvalues);
                            }
                            else if (message.validationTest == DrillholeConstants.checkDip)
                            {
                                await ReformatResults(message, DrillholeConstants.IsNumeric, DrillholeConstants.checkDip, fields, status, tableType, surveyvalues);
                            }
                        }

                        break;

                    case DrillholeConstants.Duplicates:
                        foreach (var message in test.testMessage)
                        {
                            await ReformatResults(message, DrillholeConstants.Duplicates, DrillholeConstants.checkDist, fields, status, tableType, surveyvalues);
                        }
                        break;

                    case DrillholeConstants.HoleLength:
                        foreach (var message in test.testMessage)
                        {
                            await ReformatResults(message, DrillholeConstants.HoleLength, DrillholeConstants.checkTD, fields, status, tableType, surveyvalues);
                        }
                        break;

                    case DrillholeConstants.SurveyDistance:
                        foreach (var message in test.testMessage)
                        {
                            await ReformatResults(message, DrillholeConstants.SurveyDistance, DrillholeConstants.checkDist, fields, status, tableType, surveyvalues);
                        }
                        break;

                    case DrillholeConstants.SurveyRange:
                        foreach (var message in test.testMessage)
                        {
                            if (message.validationTest == DrillholeConstants.checkDip)
                            {
                                await ReformatResults(message, DrillholeConstants.SurveyRange, DrillholeConstants.checkDip, fields, status, tableType, surveyvalues);
                            }
                            else if (message.validationTest == DrillholeConstants.checkAzi)
                            {
                                await ReformatResults(message, DrillholeConstants.SurveyRange, DrillholeConstants.checkAzi, fields, status, tableType, surveyvalues);
                            }
                        }
                        break;
                    case DrillholeConstants.MissingCollar:
                        foreach (var message in test.testMessage)
                        {
                            if (message.validationTest == DrillholeConstants.checkHole)
                            {
                                await ReformatResults(message, DrillholeConstants.MissingCollar, DrillholeConstants.checkHole, fields, status, tableType, surveyvalues);
                            }
                           
                        }
                        break;
                }
            }


        }

        public override async Task<bool> ReformatResults(ValidationMessage message, string _TestType, string validation, List<string> fields, List<DrillholeMessageStatus> statusMessages, DrillholeTableType tableType,
    IEnumerable<XElement> surveyValues)
        {
            foreach (var status in statusMessages)  //iterate three times for each error type
            {
                List<string> tooltips = message.ValidationStatus.Where(e => e.ErrorType == status).Select(p => p.Description).ToList();
                var count = message.ValidationStatus.Where(e => e.ErrorType == status).Count();  //for checking each status for messages

                int counter = 0; //use this for collar id

                if (count > 0)
                {
                    //note that all these lists and checks reside in the CollarValidationView
                    bool bCheck = await CheckReshapedData(status); //checks if a previous entry for status exists, and then adds to it.

                    //setup outside the foreach hole loop below as it is the last item to add to the ReshapedToEdit list (which is data bound to the XAML DrillholeEdits form
                    List<GroupByHoles> groupedHoles = new List<GroupByHoles>();

                    List<int> ids = message.ValidationStatus.Where(e => e.ErrorType == status).Select(a => a.id).ToList(); //get IDs of all messages

                    List<string> SelectedHoles = new List<string>();

                    string holeIDName = importSurveyFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Select(n => n.columnHeader).FirstOrDefault(); //get hole name for querying XML

                    foreach(int id in ids) //get the holes for each record
                    {
                        var value = surveyValues.Where(a => a.Attribute("ID").Value == id.ToString()).Select(h => h.Element(holeIDName).Value).FirstOrDefault();

                        SelectedHoles.Add(value);
                    }

                    var holes = SelectedHoles.GroupBy(x => x).Where(group => group.Count() >= 1).Select(group => group.Key).ToList(); //group the records under each hole

                    foreach (var surveyHole in holes)
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

                        bool surveyHoleExists = await HoleExist(surveyHole, status); //check if hole exists

                        if (!surveyHoleExists) //means status and hole don't already exist so add
                        {
                            hole = new GroupByHoles() { Ignore = false, holeid = surveyHole };
                            groupedHoles.Add(hole);
                        }
                        else
                        {
                            groupedHoles = await ReturnHolesList(status, surveyHole);

                            hole = await ReturnHole(surveyHole, status);
                        }

                        bool tableExists = await TableExists(status, surveyHole, tableType); //check if table already exists otherwise create new one

                        if (!tableExists)
                        {
                            //check for any other table
                            tableExists = await OtherTableExists(status, surveyHole, tableType);

                            if (tableExists)
                            {
                                groupedTables = await ReturnOtherTableList(status, surveyHole);
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
                            groupedTables = await ReturnTableList(status, surveyHole, tableType);
                            table = await ReturnTable(status, surveyHole, tableType);
                        }

                        hole.GroupedTables = groupedTables;

                        bool testFieldExists = await TestFieldExists(status, surveyHole, tableType, _TestType, validation);

                        if (!testFieldExists)
                        {
                            testField = new GroupByTestField() { Ignore = false, TestField = validation, TableData = rows };
                            groupedTestFields.Add(testField);
                        }
                        else
                        {
                            groupedTestFields = await ReturnTestFields(status, surveyHole, tableType, _TestType, validation);

                            testField = await ReturnTestField(status, surveyHole, tableType, _TestType, validation);
                        }

                        bool testExists = await TestExists(status, surveyHole, tableType, _TestType);

                        if (!testExists)
                        {
                            test = new GroupByTest() { MainTest = _TestType, Ignore = false, TestFields = groupedTestFields };
                            test.TestFields = groupedTestFields;

                            groupedTests = await ReturnTestList(status, surveyHole, tableType);

                            groupedTests.Add(test);
                        }
                        else
                        {
                            groupedTests = await ReturnTestList(status, surveyHole, tableType, _TestType);


                            test = await ReturnTest(status, surveyHole, tableType, _TestType);
                            test.TestFields.Add(testField);

                        }


                        table.GroupedTests = groupedTests;

                        //return records within hole


                        //only need collar id for Duplicates.
                        if (_TestType == DrillholeConstants.Duplicates)
                        {
                            rows.Add(new RowsToEdit
                            {
                                id_col = counter,
                                id_sur = counter,
                                testType = _TestType,
                                validationTest = validation,
                                Description = tooltips[counter]
                            });
                        }
                        else
                        {

                            List<string> survIDs = surveyValues.Where(h => h.Element(fields[0]).Value == surveyHole).Select(v => v.Attribute("ID").Value).ToList(); //get the survey IDs for hole to check which records have a message

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
                                            id_sur = value,
                                            testType = _TestType,
                                            validationTest = validation,
                                            Description = tooltips[messageCount]
                                        });

                                        messageCount++;
                                        break;
                                    }
  
                                }
                            }
                        }

                        counter++;

                    }


                    if (!bCheck)
                        EditDrillholeData.Add(new ReshapedDataToEdit { Count = 0, ErrorType = status.ToString(), Ignore = false, GroupedHoles = groupedHoles }); //only add if new status type
                }

            }

            return true;
        }



        public override async Task<List<string>> ReturnFieldnamesForXmlQuery() //original in CollarValidationView
        {
            List<string> fields = new List<string>();
            fields.Add(importSurveyFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false) //get name of hole field for querying XML
                .Select(s => s.columnHeader).FirstOrDefault());
            fields.Add(importSurveyFields.Where(o => o.columnImportName == DrillholeConstants.distName).Where(m => m.genericType == false) //get other feilds which are required as mandatory, i.e. XYZ and depth
                .Select(s => s.columnHeader).FirstOrDefault());
            fields.Add(importSurveyFields.Where(o => o.columnImportName == DrillholeConstants.azimuthName).Where(m => m.genericType == false)
                .Select(s => s.columnHeader).FirstOrDefault());
            fields.Add(importSurveyFields.Where(o => o.columnImportName == DrillholeConstants.dipName).Where(m => m.genericType == false)
                .Select(s => s.columnHeader).FirstOrDefault());

            return fields;
        }

    }
}

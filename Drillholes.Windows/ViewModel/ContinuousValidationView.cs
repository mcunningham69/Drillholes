using System;
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
    public class ContinuousValidationView : CollarValidationView
    {
        public ContinuousValidationService _continuousValidationService;
        public IContinuousValidation _continuousValidateValues;

        public ImportTableFields importContinuousFields { get; set; }

        private XElement _xmlContinuousData;
        public XElement xmlContinuousData
        {
            get
            {
                return this._xmlContinuousData;
            }
            set
            {
                this._xmlContinuousData = value;
                OnPropertyChanged("xmlContinuousData");
            }
        }

        public ContinuousValidationView(DrillholeTableType _tableType, XElement _xmlContinuousData) : base(_tableType, _xmlContinuousData)
        {
            tableLabel = (_tableType.ToString() + " Table").ToUpper();

            xmlContinuousData = _xmlContinuousData;

            _continuousValidateValues = new ContinuousValidation();

            _continuousValidationService = new ContinuousValidationService(_continuousValidateValues);
        }


        #region Validation Messages
        public override async Task<bool> ValidateAllTables(bool editData)
        {
            ValidationDelegate mTables = null;
            mTables += CheckForEmptyFields;
            mTables += CheckForDuplicates;
            mTables += CheckNumericFields;
            mTables += CheckMaxDepth;
            mTables += CheckDistance;
            mTables += CheckCollars;

            return await mTables(editData);

        }

        public override void InitialiseMapping()
        {
            if (_continuousValidateValues == null)
                _continuousValidateValues = new ContinuousValidation();

             var config = new MapperConfiguration(cfg => { cfg.CreateMap<ValidationContinuousDto, ValidationContinuous>(); });

            mapper = config.CreateMapper();

            var source = new ValidationContinuousDto();

            var dest = mapper.Map<ValidationContinuousDto, ValidationContinuous>(source);

            _continuousValidationService = new ContinuousValidationService(_continuousValidateValues);

        }

        public async override Task<bool> CheckForEmptyFields(bool editData)
        {
            if (mapper == null)
                InitialiseMapping();

            List<ValidationMessage> continuousFieldTest = new List<ValidationMessage>();

            continuousFieldTest.Add(new ValidationMessage
            {
                verified = true,
                validationTest = DrillholeConstants.checkHole,
                count = 0,
                validationMessages = new List<string>(),
                tableField = importContinuousFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Single()
            });

            continuousFieldTest.Add(new ValidationMessage
            {
                verified = true,
                validationTest = DrillholeConstants.checkDist,
                count = 0,
                validationMessages = new List<string>(),
                tableField = importContinuousFields.Where(o => o.columnImportName == DrillholeConstants.distName).Where(m => m.genericType == false).Single()
            });


            var isNumericField = importContinuousFields.Where(o => o.fieldType == "Double").Where(p => p.genericType == true).ToList();

            if (isNumericField.Count > 0)
            {
                foreach (var field in isNumericField)
                {
                    continuousFieldTest.Add(new ValidationMessage
                    {
                        verified = true,
                        count = 0,
                        validationTest = DrillholeConstants.checkNumeric,
                        validationMessages = new List<string>(),
                        tableField = field

                    });

                }
            }

            ValidationMessages continuousTests = new ValidationMessages { testType = DrillholeConstants.IsEmptyOrNull, testMessage = continuousFieldTest };

            var validationCheck = await _continuousValidationService.CheckIsEmpty(mapper, continuousTests, xmlContinuousData);

            DisplayMessages.DisplayResults.Add(validationCheck.testMessages);

            return true;
        }
        public async override Task<bool> CheckForDuplicates(bool editData)
        {
            if (mapper == null)
                InitialiseMapping();

            ImportTableField holeField = importContinuousFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Single();
            ImportTableField distanceField = importContinuousFields.Where(o => o.columnImportName == DrillholeConstants.distName).Where(m => m.genericType == false).Single();

            List<ImportTableField> tempFields = new List<ImportTableField>();
            tempFields.Add(holeField);
            tempFields.Add(distanceField);

            List<ValidationMessage> continuousFieldTest = new List<ValidationMessage>();
            continuousFieldTest.Add(new ValidationMessage
            {
                verified = true,
                validationTest = DrillholeConstants.checkDist,
                count = 0,
                validationMessages = new List<string>(),
                tableFields = tempFields
            });

            ValidationMessages continuousTests = new ValidationMessages { testType = DrillholeConstants.Duplicates, testMessage = continuousFieldTest };

            var validationCheck = await _continuousValidationService.CheckDuplicates(mapper, continuousTests, xmlContinuousData);

            DisplayMessages.DisplayResults.Add(validationCheck.testMessages);

            return true;
        }
        private async Task<bool> CheckDistance(bool editData)
        {
            if (mapper == null)
                InitialiseMapping();

            List<XElement> drillholeTableData = new List<XElement>();
            drillholeTableData.Add(xmlCollarData);
            drillholeTableData.Add(xmlContinuousData);

            ImportTableField holeField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Single();
            ImportTableField tdField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.maxName).Where(m => m.genericType == false).Single();
            ImportTableField continuousHoleField = importContinuousFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Single();
            ImportTableField distanceField = importContinuousFields.Where(o => o.columnImportName == DrillholeConstants.distName).Where(m => m.genericType == false).Single();

            List<ImportTableField> tempFields = new List<ImportTableField>();
            tempFields.Add(holeField);
            tempFields.Add(tdField);
            tempFields.Add(continuousHoleField);
            tempFields.Add(distanceField);

            List<ValidationMessage> continuousFieldTest = new List<ValidationMessage>();

            //Check if azimuth and dip fields set
            continuousFieldTest.Add(new ValidationMessage
            {
                verified = true,
                validationTest = DrillholeConstants.checkDist,
                count = 0,
                validationMessages = new List<string>(),
                tableFields = tempFields
            });

            ValidationMessages continuousTests = new ValidationMessages { testType = DrillholeConstants.SurveyDistance, testMessage = continuousFieldTest };

            var validationCheck = await _continuousValidationService.CheckDistance(mapper, continuousTests, drillholeTableData);

            DisplayMessages.DisplayResults.Add(validationCheck.testMessages);
            return true;
        }
        public async override Task<bool> CheckNumericFields(bool editData)
        {
            if (mapper == null)
                InitialiseMapping();

            List<ValidationMessage> continuousFieldTest = new List<ValidationMessage>();
            continuousFieldTest.Add(new ValidationMessage
            {
                verified = true,
                validationTest = DrillholeConstants.checkHole,
                count = 0,
                validationMessages = new List<string>(),
                tableField = importContinuousFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Single()
            });
            continuousFieldTest.Add(new ValidationMessage
            {
                verified = true,
                validationTest = DrillholeConstants.checkDist,
                count = 0,
                validationMessages = new List<string>(),
                tableField = importContinuousFields.Where(o => o.columnImportName == DrillholeConstants.distName).Where(m => m.genericType == false).Single()
            });



            var isNumericField = importContinuousFields.Where(o => o.fieldType == "Double").Where(p => p.genericType == true).ToList();

            if (isNumericField.Count > 0)
            {
                foreach (var field in isNumericField)
                {
                    continuousFieldTest.Add(new ValidationMessage
                    {
                        verified = true,
                        count = 0,
                        validationTest = DrillholeConstants.checkNumeric,
                        validationMessages = new List<string>(),
                        tableField = field

                    });

                }
            }

            ValidationMessages continuousTests = new ValidationMessages { testType = DrillholeConstants.IsNumeric, testMessage = continuousFieldTest };

            var validationCheck = await _continuousValidationService.CheckForNumeric(mapper, continuousTests, xmlContinuousData);

            DisplayMessages.DisplayResults.Add(validationCheck.testMessages);

            return true;
        }
        public async override Task<bool> CheckMaxDepth(bool editData)
        {
            if (mapper == null)
                InitialiseMapping();

            List<XElement> drillholeTableData = new List<XElement>();
            drillholeTableData.Add(xmlCollarData);
            drillholeTableData.Add(xmlContinuousData);

            ImportTableField holeField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Single();
            ImportTableField tdField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.maxName).Where(m => m.genericType == false).Single();
            ImportTableField continuousHoleField = importContinuousFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Single();
            ImportTableField distanceField = importContinuousFields.Where(o => o.columnImportName == DrillholeConstants.distName).Where(m => m.genericType == false).Single();

            List<ImportTableField> tempFields = new List<ImportTableField>();
            tempFields.Add(holeField);
            tempFields.Add(tdField);
            tempFields.Add(continuousHoleField);
            tempFields.Add(distanceField);

            List<ValidationMessage> continuousFieldTest = new List<ValidationMessage>();

            //Check if azimuth and dip fields set
            continuousFieldTest.Add(new ValidationMessage
            {
                verified = true,
                validationTest = DrillholeConstants.checkTD,
                count = 0,
                validationMessages = new List<string>(),
                tableFields = tempFields
            });

            ValidationMessages continuousTests = new ValidationMessages { testType = DrillholeConstants.HoleLength, testMessage = continuousFieldTest };

            var validationCheck = await _continuousValidationService.CheckMaxDepth(mapper, continuousTests, drillholeTableData);

            DisplayMessages.DisplayResults.Add(validationCheck.testMessages);

            return true;
        }


        private async Task<bool> CheckCollars(bool editData)
        {
            if (mapper == null)
                InitialiseMapping();

            List<XElement> drillholeTableData = new List<XElement>();
            drillholeTableData.Add(xmlCollarData);
            drillholeTableData.Add(xmlContinuousData);

            ImportTableField holeField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Single();
            ImportTableField continuousHoleField = importContinuousFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Single();

            List<ImportTableField> tempFields = new List<ImportTableField>();
            tempFields.Add(holeField);
            tempFields.Add(continuousHoleField);

            List<ValidationMessage> continuousFieldTest = new List<ValidationMessage>();

            //Check if azimuth and dip fields set
            continuousFieldTest.Add(new ValidationMessage
            {
                verified = true,
                validationTest = DrillholeConstants.checkHole,
                count = 0,
                validationMessages = new List<string>(),
                tableFields = tempFields
            });

            ValidationMessages continuousTests = new ValidationMessages { testType = DrillholeConstants.MissingCollar, testMessage = continuousFieldTest };

            var validationCheck = await _continuousValidationService.CheckMissingCollars(mapper, continuousTests, drillholeTableData);

            DisplayMessages.DisplayResults.Add(validationCheck.testMessages);

            return true;
        }

        #endregion


        public override async void ReshapeMessages(DrillholeTableType tableType)
        {
            List<string> fields = await ReturnFieldnamesForXmlQuery();
            List<DrillholeMessageStatus> status = await ReturnStatusAlerts(); //set up three alert states

            var continuousvalues = xmlContinuousData.Elements(); 

            foreach (var test in ShowTestMessages)  //get messages after running validation above
            {
                switch (test.testType) //select on the different testtype
                {
                    case DrillholeConstants.IsEmptyOrNull:

                        foreach (var message in test.testMessage)
                        {
                            if (message.validationTest == DrillholeConstants.checkHole)  //specific field being checked
                            {
                                await ReformatResults(message, DrillholeConstants.IsEmptyOrNull, DrillholeConstants.checkHole, fields, status, tableType, continuousvalues);
                            }
                            else if (message.validationTest == DrillholeConstants.checkDist)
                            {
                                await ReformatResults(message, DrillholeConstants.IsEmptyOrNull, DrillholeConstants.checkDist, fields, status, tableType, continuousvalues);
                            }

                        }
                        break;

                    case DrillholeConstants.IsNumeric:
                        foreach (var message in test.testMessage)
                        {
                            if (message.validationTest == DrillholeConstants.checkDist)
                            {
                                await ReformatResults(message, DrillholeConstants.IsNumeric, DrillholeConstants.checkDist, fields, status, tableType, continuousvalues);
                            }

                        }

                        break;

                    case DrillholeConstants.Duplicates:
                        foreach (var message in test.testMessage)
                        {
                            await ReformatResults(message, DrillholeConstants.Duplicates, DrillholeConstants.checkDist, fields, status, tableType, continuousvalues);
                        }
                        break;

                    case DrillholeConstants.HoleLength:
                        foreach (var message in test.testMessage)
                        {
                            await ReformatResults(message, DrillholeConstants.HoleLength, DrillholeConstants.checkTD, fields, status, tableType, continuousvalues);
                        }
                        break;

                    case DrillholeConstants.SurveyDistance:
                        foreach (var message in test.testMessage)
                        {
                            await ReformatResults(message, DrillholeConstants.SurveyDistance, DrillholeConstants.checkDist, fields, status, tableType, continuousvalues);
                        }
                        break;


                    case DrillholeConstants.MissingCollar:
                        foreach (var message in test.testMessage)
                        {
                            if (message.validationTest == DrillholeConstants.checkHole)
                            {
                                await ReformatResults(message, DrillholeConstants.MissingCollar, DrillholeConstants.checkHole, fields, status, tableType, continuousvalues);
                            }
                           
                        }
                        break;
                }
            }


        }

        public override async Task<bool> ReformatResults(ValidationMessage message, string _TestType, string validation, List<string> fields, List<DrillholeMessageStatus> statusMessages, DrillholeTableType tableType,
    IEnumerable<XElement> continuousValues)
        {

            foreach (var status in statusMessages)  //iterate three times for each error type
            {
                var count = message.ValidationStatus.Where(e => e.ErrorType == status).Count();  //for checking each status for messages

                int counter = 0; //use this for collar id

                if (count > 0)
                {
                   
                    //setup outside the foreach hole loop below as it is the last item to add to the ReshapedToEdit list (which is data bound to the XAML DrillholeEdits form
                    List<GroupByHoles> groupedHoles = new List<GroupByHoles>();

                    List<int> ids = message.ValidationStatus.Where(e => e.ErrorType == status).Select(a => a.id).ToList(); //get IDs of all messages

                    List<string> SelectedHoles = new List<string>();

                    string holeIDName = importContinuousFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Select(n => n.columnHeader).FirstOrDefault(); //get hole name for querying XML

                    foreach(int id in ids) //get the holes for each record
                    {
                        if (id > -999)
                        {
                            var value = continuousValues.Where(a => a.Attribute("ID").Value == id.ToString()).Select(h => h.Element(holeIDName).Value).FirstOrDefault();

                            SelectedHoles.Add(value);
                        }
                    }

                    var holes = SelectedHoles.GroupBy(x => x).Where(group => group.Count() >= 1).Select(group => group.Key).ToList(); //group the records under each hole

                    bool holeExists = true;
                    bool statusExists = true;

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


                        holeExists = await HoleExist(surveyHole, status); //check if hole exists

                        if (!holeExists) //means status and hole don't already exist so add
                        {
                            hole = new GroupByHoles() { Ignore = false, holeid = surveyHole };

                            List<GroupByHoles> testGroup = await ReturnGroupedHoles(status);

                            if (testGroup != null)
                                groupedHoles = testGroup;

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

                        if (_TestType == DrillholeConstants.Duplicates || _TestType == DrillholeConstants.MissingCollar)
                        {
                            var value = continuousValues.Where(a => a.Element(holeIDName).Value == surveyHole).Select(v => v.Attribute("ID").Value).FirstOrDefault();
                            string tooltip = message.ValidationStatus.Where(e => e.ErrorType == status && e.id == Convert.ToInt32(value)).Select(p => p.Description).FirstOrDefault();

                            //only need collar id for Duplicates.
                            if (_TestType == DrillholeConstants.Duplicates)
                            {
                                rows.Add(new RowsToEdit
                                {
                                    // id_col = counter,
                                    id_con = counter,
                                    testType = _TestType,
                                    validationTest = validation,
                                    TableType = DrillholeTableType.continuous,
                                    ErrorType = status,
                                    Description = tooltip
                                });
                            }
                            else if (_TestType == DrillholeConstants.MissingCollar)
                            {

                                rows.Add(new RowsToEdit
                                {
                                    id_con = Convert.ToInt32(value),
                                    testType = _TestType,
                                    validationTest = validation,
                                    TableType = DrillholeTableType.continuous,
                                    ErrorType = status,
                                    Description = tooltip
                                });
                            }
                        }

                        else
                        {
                            List<string> survIDs = continuousValues.Where(h => h.Element(fields[0]).Value == surveyHole).Select(v => v.Attribute("ID").Value).ToList(); //get the survey IDs for hole to check which records have a message

                            int messageCount = 0;

                            foreach (var _id in survIDs)
                            {
                                if (messageCount == ids.Count) //once all messages reached, bail out of loop
                                    break;

                                int check = Convert.ToInt16(_id);

                                foreach (var result in ids)
                                {
                                    if (result == check) //check survey ID against ID in messages 
                                    {
                                        rows.Add(new RowsToEdit
                                        {
                                            // id_col = counter,
                                            id_con = result,
                                            testType = _TestType,
                                            validationTest = validation,
                                            TableType = DrillholeTableType.continuous,
                                            ErrorType = status,
                                            Description = message.ValidationStatus.Where(e => e.ErrorType == status && e.id == Convert.ToInt32(result)).Select(p => p.Description).FirstOrDefault()
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

        public override async Task<System.Data.DataTable> PopulateGridValues(List<RowsToEdit> _edit, DrillholeTableType tableType, bool preview)
        {
            System.Data.DataTable dataTable = new System.Data.DataTable();
            var continuous = xmlContinuousData.Elements();

            dataTable = await AddColumns(tableType, preview);

            List<object> rowValues = new List<object>();

            var holeID = importContinuousFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Select(m => m.columnHeader).FirstOrDefault();
            var distance = importContinuousFields.Where(o => o.columnImportName == DrillholeConstants.distName).Select(m => m.columnHeader).FirstOrDefault();

            foreach (var value in _edit) //populate dataTable
            {

                rowValues.Add(value.id_con.ToString());
                rowValues.Add(continuous.Where(h => h.Attribute("ID").Value == value.id_con.ToString()).Select(o => o.Element(holeID).Value).Single());
                rowValues.Add(continuous.Where(h => h.Attribute("ID").Value == value.id_con.ToString()).Select(o => o.Element(distance).Value).Single());

                rowValues.Add(value.testType);
                rowValues.Add(value.Description);

                dataTable.Rows.Add(rowValues.ToArray());

                rowValues.Clear();
            }

            return dataTable; //TODO
        }

        public virtual async Task<System.Data.DataTable> AddColumns(DrillholeTableType tableType, bool preview)
        {
            System.Data.DataTable dataTable = new System.Data.DataTable();

            dataTable.Columns.Add("ID");
            dataTable.Columns.Add(DrillholeConstants.holeID);

            dataTable.Columns.Add(DrillholeConstants.distName);

            dataTable.Columns.Add("Validation");
            dataTable.Columns.Add("Description");

            return dataTable;
        }

        public override async Task<List<string>> ReturnFieldnamesForXmlQuery() //original in CollarValidationView
        {
            List<string> fields = new List<string>();
            fields.Add(importContinuousFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false) //get name of hole field for querying XML
                .Select(s => s.columnHeader).FirstOrDefault());
            fields.Add(importContinuousFields.Where(o => o.columnImportName == DrillholeConstants.distName).Where(m => m.genericType == false) //get other feilds which are required as mandatory, i.e. XYZ and depth
                .Select(s => s.columnHeader).FirstOrDefault());

            return fields;
        }

    }
}

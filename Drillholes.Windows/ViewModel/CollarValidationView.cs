using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drillholes.Domain.Services;
using Drillholes.Domain.Interfaces;
using Drillholes.Domain.Enum;
using Drillholes.Validation.TestMessage;
using AutoMapper;
using System.Runtime.CompilerServices;
using Drillholes.Domain;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using Drillholes.Domain.DataObject;
using Drillholes.Domain.DTO;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Threading;
using System.Data;
using System.Windows.Controls;

namespace Drillholes.Windows.ViewModel
{
    public class CollarValidationView : ViewEditModel
    {      

        public delegate Task<bool> ValidationDelegate(bool editData);

        public CollarValidationService _validationService;
        public ICollarValidation _validateValues;

        public IMapper mapper = null;
        public virtual DrillholeSurveyType surveyType { get; set; }

        public ImportTableFields importCollarFields { get; set; }

        public DisplayValidationMessages DisplayMessages = new DisplayValidationMessages();
        public ObservableCollection<ValidationMessages> ShowTestMessages { get { return DisplayMessages.DisplayResults; } }

        public DrillholeDataToEdit EditData = new DrillholeDataToEdit();

        public ObservableCollection<ReshapedDataToEdit> EditDrillholeData { get; set; }


        private XElement _xmlCollarData;
        public XElement xmlCollarData
        {
            get
            {
                return this._xmlCollarData;
            }
            set
            {
                this._xmlCollarData = value;
                OnPropertyChanged("xmlCollarData");
            }
        }

        private string _tableLabel;
        public virtual string tableLabel
        {
            get
            {
                return this._tableLabel;
            }
            set
            {
                this._tableLabel = value;
                OnPropertyChanged("tableLabel");
            }
        }

        public CollarValidationView()
        { }

        public CollarValidationView(DrillholeTableType _tableType, XElement _xmlCollarData)
        { }
        public CollarValidationView(DrillholeTableType _tableType, DrillholeSurveyType _surveyType, XElement _xmlCollarData)
        {
            surveyType = _surveyType;
            xmlCollarData = _xmlCollarData;
            _tableLabel = (_tableType.ToString() + " Table").ToUpper();

            _validateValues = new CollarValidation(surveyType);

            _validationService = new CollarValidationService(_validateValues);
        }

        public virtual void InitialiseMapping()
        {
            if (_validateValues == null)
                _validateValues = new CollarValidation(surveyType);

            var config = new MapperConfiguration(cfg => { cfg.CreateMap<ValidationCollarDto, ValidationCollar>(); });

            mapper = config.CreateMapper();

            var source = new ValidationCollarDto();

            var dest = mapper.Map<ValidationCollarDto, ValidationCollar>(source);

            _validationService = new CollarValidationService(_validateValues);

        }

        #region Messages
        public virtual async Task<bool> ValidateAllTables(bool editData)
        {
            ValidationDelegate mTables = null;
            mTables += CheckForEmptyFields;
            mTables += CheckNumericFields;
            mTables += CheckForDuplicates;
            mTables += CheckMaxDepth;
            mTables += CheckZeroCoordinates;

            if (surveyType == DrillholeSurveyType.collarsurvey)
            {
                mTables += CheckRange;
            }

            return await mTables(editData);
        }

        
        public virtual async Task<bool> CheckForEmptyFields(bool editData)
        {
            if (mapper == null)
                InitialiseMapping();

            List<ValidationMessage> collarTest = new List<ValidationMessage>();


            collarTest.Add
                (new ValidationMessage
                {
                    verified = true,
                    count = 0,
                    validationTest = DrillholeConstants.checkHole,
                    validationMessages = new List<string>(),
                    tableField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Single()
                });


            collarTest.Add(new ValidationMessage
            {
                verified = true,
                count = 0,
                validationTest = DrillholeConstants.checkX,
                validationMessages = new List<string>(),
                tableField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.xName).Where(m => m.genericType == false).Single()
            });



            collarTest.Add(new ValidationMessage
            {
                verified = true,
                count = 0,
                validationTest = DrillholeConstants.checkY,
                validationMessages = new List<string>(),
                tableField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.yName).Where(m => m.genericType == false).Single()

            });
            collarTest.Add(new ValidationMessage
            {
                verified = true,
                count = 0,
                validationTest = DrillholeConstants.checkZ,
                validationMessages = new List<string>(),
                tableField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.zName).Where(m => m.genericType == false).Single()

            });
            collarTest.Add(new ValidationMessage
            {
                verified = true,
                count = 0,
                validationTest = DrillholeConstants.checkTD,
                validationMessages = new List<string>(),
                tableField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.maxName).Where(m => m.genericType == false).Single()

            });

            if (DrillholeSurveyType.collarsurvey == surveyType)
            {
                collarTest.Add(new ValidationMessage
                {
                    verified = true,
                    count = 0,
                    validationTest = DrillholeConstants.checkDip,
                    validationMessages = new List<string>(),
                    tableField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.dipName).Where(m => m.genericType == false).Single()

                });
                collarTest.Add(new ValidationMessage
                {
                    verified = true,
                    count = 0,
                    validationTest = DrillholeConstants.checkAzi,
                    validationMessages = new List<string>(),
                    tableField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.azimuthName).Where(m => m.genericType == false).Single()

                });
            }

            var isNumericField = importCollarFields.Where(o => o.fieldType == "Double").Where(p => p.genericType == true).ToList();

            if (isNumericField.Count > 0)
            {
                foreach (var field in isNumericField)
                {
                    collarTest.Add(new ValidationMessage
                    {
                        verified = true,
                        count = 0,
                        validationTest = DrillholeConstants.checkNumeric,
                        validationMessages = new List<string>(),
                        tableField = field

                    });

                }
            }

            ValidationMessages collarTests = new ValidationMessages { testType = DrillholeConstants.IsEmptyOrNull, testMessage = collarTest };

            var validationCheck = await _validationService.CheckIsEmpty(mapper, collarTests, xmlCollarData);

            DisplayMessages.DisplayResults.Add(validationCheck.testMessages);

            return true;
        }
        public virtual async Task<bool> CheckNumericFields(bool editData)
        {
            if (mapper == null)
                InitialiseMapping();

            List<ValidationMessage> collarTest = new List<ValidationMessage>();

            collarTest.Add(new ValidationMessage
            {
                verified = true,
                validationTest = DrillholeConstants.checkHole,
                count = 0,
                validationMessages = new List<string>(),
                tableField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Single()
            });

            collarTest.Add(new ValidationMessage
            {
                verified = true,
                validationTest = DrillholeConstants.checkX,
                count = 0,
                validationMessages = new List<string>(),
                tableField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.xName).Where(m => m.genericType == false).Single()
            });

            collarTest.Add(new ValidationMessage
            {
                verified = true,
                validationTest = DrillholeConstants.checkY,
                count = 0,
                validationMessages = new List<string>(),
                tableField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.yName).Where(m => m.genericType == false).Single()
            });

            collarTest.Add(new ValidationMessage
            {
                verified = true,
                validationTest = DrillholeConstants.checkZ,
                count = 0,
                validationMessages = new List<string>(),
                tableField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.zName).Where(m => m.genericType == false).Single()
            });

            collarTest.Add(new ValidationMessage
            {
                verified = true,
                validationTest = DrillholeConstants.checkTD,
                count = 0,
                validationMessages = new List<string>(),
                tableField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.maxName).Where(m => m.genericType == false).Single()
            });

            if (DrillholeSurveyType.collarsurvey == surveyType)
            {
                collarTest.Add(new ValidationMessage
                {
                    verified = true,
                    validationTest = DrillholeConstants.checkDip,
                    count = 0,
                    validationMessages = new List<string>(),
                    tableField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.dipName).Where(m => m.genericType == false).Single()
                });

                collarTest.Add(new ValidationMessage
                {
                    verified = true,
                    validationTest = DrillholeConstants.checkAzi,
                    count = 0,
                    validationMessages = new List<string>(),
                    tableField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.azimuthName).Where(m => m.genericType == false).Single()
                });
            }

            //check for numeric fields
            var isNumericField = importCollarFields.Where(o => o.fieldType == "Double").Where(p => p.genericType == true).ToList();

            if (isNumericField.Count > 0)
            {
                foreach (var field in isNumericField)
                {
                    collarTest.Add(new ValidationMessage
                    {
                        verified = true,
                        count = 0,
                        validationTest = DrillholeConstants.checkNumeric,
                        validationMessages = new List<string>(),
                        tableField = field

                    });

                }
            }

            ValidationMessages collarTests = new ValidationMessages { testType = DrillholeConstants.IsNumeric, testMessage = collarTest};

            var validationCheck = await _validationService.CheckIsNumeric(mapper, collarTests, xmlCollarData);

            DisplayMessages.DisplayResults.Add(validationCheck.testMessages);

            return true;
        }

        public virtual async Task<bool> CheckForDuplicates(bool editData)
        {
            if (mapper == null)
                InitialiseMapping();

            List<ValidationMessage> collarTest = new List<ValidationMessage>();

            List<XElement> drillholeData = new List<XElement>();
            drillholeData.Add(xmlCollarData);

            List<ValidationMessage> collarFieldTest = new List<ValidationMessage>();

            collarTest.Add(new ValidationMessage
            {
                verified = true,
                validationTest = DrillholeConstants.checkHole,
                count = 0,
                validationMessages = new List<string>(),
                tableField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Single()
            });

            ValidationMessages collarTests = new ValidationMessages { testType = DrillholeConstants.Duplicates, testMessage = collarTest };

            var validationCheck = await _validationService.CheckDuplicates(mapper, collarTests, xmlCollarData);

            DisplayMessages.DisplayResults.Add(validationCheck.testMessages);

            return true;
        }
        public virtual async Task<bool> CheckMaxDepth(bool editData)
        {
            if (mapper == null)
                InitialiseMapping();

            List<XElement> drillholeTableData = new List<XElement>();
            drillholeTableData.Add(xmlCollarData);

            ImportTableField holeField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Single();
            ImportTableField maxField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.maxName).Where(m => m.genericType == false).Single();
            
            List<ImportTableField> tempFields = new List<ImportTableField>();
            tempFields.Add(holeField);
            tempFields.Add(maxField);

            List<ValidationMessage> collarTest = new List<ValidationMessage>();

            //Check if azimuth and dip fields set
            collarTest.Add(new ValidationMessage
            {
                verified = true,
                validationTest = DrillholeConstants.checkTD,
                count = 0,
                validationMessages = new List<string>(),
                tableField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.maxName).Where(m => m.genericType == false).Single(),
                tableFields = tempFields
            });

            ValidationMessages collarTests = new ValidationMessages { testType = DrillholeConstants.HoleLength, testMessage = collarTest };

            var validationCheck = await _validationService.CheckMaxDepth(mapper, collarTests, xmlCollarData);

            DisplayMessages.DisplayResults.Add(validationCheck.testMessages);

            return true;
        }
        public virtual async Task<bool> CheckZeroCoordinates(bool editData)
        {
            if (mapper == null)
                InitialiseMapping();

            List<ValidationMessage> collarFieldTest = new List<ValidationMessage>();

            ImportTableField holeField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Single();
            ImportTableField xField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.xName).Where(m => m.genericType == false).Single();
            ImportTableField yField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.yName).Where(m => m.genericType == false).Single();

            List<ImportTableField> tempFields = new List<ImportTableField>();
            tempFields.Add(holeField);
            tempFields.Add(xField);
            tempFields.Add(yField);

            //Check if azimuth and dip fields set
            collarFieldTest.Add(new ValidationMessage
            {
                verified = true,
                validationTest = DrillholeConstants.checkCoord,
                count = 0,
                validationMessages = new List<string>(),
                tableFields = tempFields
            });

            ValidationMessages collarTests = new ValidationMessages { testType = DrillholeConstants.ZeroCoordinate, testMessage = collarFieldTest};

            var validationCheck = await _validationService.CheckZeroCoordinate(mapper, collarTests, xmlCollarData);

            DisplayMessages.DisplayResults.Add(validationCheck.testMessages);

            return true;
        }

        public virtual async Task<bool> CheckRange(bool editData)
        {
            if (mapper == null)
                InitialiseMapping();

            List<ValidationMessage> collarFieldTest = new List<ValidationMessage>();

            ImportTableField holeField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Single();
            ImportTableField dipField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.dipName).Where(m => m.genericType == false).Single();
            ImportTableField aziField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.azimuthName).Where(m => m.genericType == false).Single();

            List<ImportTableField> dipFields = new List<ImportTableField>();
            dipFields.Add(holeField);
            dipFields.Add(dipField);

            //Check if azimuth and dip fields set
            collarFieldTest.Add(new ValidationMessage
            {
                verified = true,
                validationTest = DrillholeConstants.checkDip,
                count = 0,
                validationMessages = new List<string>(),
                tableField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.dipName).Where(m => m.genericType == false).Single(),
                tableFields = dipFields
            });

            List<ImportTableField> aziFields = new List<ImportTableField>();
            aziFields.Add(holeField);
            aziFields.Add(aziField);
            collarFieldTest.Add(new ValidationMessage
            {
                verified = true,
                validationTest = DrillholeConstants.checkAzi,
                count = 0,
                validationMessages = new List<string>(),
                tableField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.azimuthName).Where(m => m.genericType == false).Single(),
                tableFields = aziFields
            });

            ValidationMessages collarTests = new ValidationMessages { testType = DrillholeConstants.SurveyRange, testMessage = collarFieldTest };

            var validationCheck = await _validationService.CheckRange(mapper, collarTests, xmlCollarData);

            DisplayMessages.DisplayResults.Add(validationCheck.testMessages);

            return true;
        }

        #endregion

        #region TreeView databound
        public virtual async void ReshapeMessages(DrillholeTableType tableType)
        {
            List<string> fields = await ReturnFieldnamesForXmlQuery();
            List<DrillholeMessageStatus> status = await ReturnStatusAlerts();

            EditDrillholeData = new ObservableCollection<ReshapedDataToEdit>();

            var collarValues = xmlCollarData.Elements();  //get dollar data from stored XML

            foreach (var test in ShowTestMessages)  //get messages after running validation above
            {
                switch (test.testType) //select on the different testtype
                {
                    case DrillholeConstants.IsEmptyOrNull:
                        foreach (var message in test.testMessage)
                        {
                            if (message.validationTest == DrillholeConstants.checkHole)  //specific field being checked
                            {
                                await ReformatResults(message, DrillholeConstants.IsEmptyOrNull, DrillholeConstants.checkHole, fields, status, tableType, collarValues);
                            }
                            else if (message.validationTest == DrillholeConstants.checkX)
                            {
                                await ReformatResults(message, DrillholeConstants.IsEmptyOrNull, DrillholeConstants.checkX, fields, status, tableType, collarValues);
                            }
                            else if (message.validationTest == DrillholeConstants.checkY)
                            {
                                await ReformatResults(message, DrillholeConstants.IsEmptyOrNull, DrillholeConstants.checkY, fields, status, tableType, collarValues);
                            }
                            else if (message.validationTest == DrillholeConstants.checkZ)
                            {
                                await ReformatResults(message, DrillholeConstants.IsEmptyOrNull, DrillholeConstants.checkZ, fields, status, tableType, collarValues);
                            }
                            else if (message.validationTest == DrillholeConstants.checkTD)
                            {
                                await ReformatResults(message, DrillholeConstants.IsEmptyOrNull, DrillholeConstants.checkTD, fields, status, tableType, collarValues);
                            }
                            else if (message.validationTest == DrillholeConstants.checkAzi)
                            {
                                await ReformatResults(message, DrillholeConstants.IsEmptyOrNull, DrillholeConstants.checkAzi, fields, status, tableType, collarValues);
                            }
                            else if (message.validationTest == DrillholeConstants.checkDip)
                            {
                                await ReformatResults(message, DrillholeConstants.IsEmptyOrNull, DrillholeConstants.checkDip, fields, status, tableType, collarValues);
                            }
                        }
                        break;

                    case DrillholeConstants.IsNumeric:
                        foreach (var message in test.testMessage)
                        {
                            if (message.validationTest == DrillholeConstants.checkX)
                            {
                                await ReformatResults(message, DrillholeConstants.IsNumeric, DrillholeConstants.checkX, fields, status, tableType, collarValues);
                            }
                            else if (message.validationTest == DrillholeConstants.checkY)
                            {
                                await ReformatResults(message, DrillholeConstants.IsNumeric, DrillholeConstants.checkY, fields, status, tableType, collarValues);
                            }
                            else if (message.validationTest == DrillholeConstants.checkZ)
                            {
                                await ReformatResults(message, DrillholeConstants.IsNumeric, DrillholeConstants.checkZ, fields, status, tableType, collarValues);
                            }
                            else if (message.validationTest == DrillholeConstants.checkTD)
                            {
                                await ReformatResults(message, DrillholeConstants.IsNumeric, DrillholeConstants.checkTD, fields, status, tableType, collarValues);
                            }
                            else if (message.validationTest == DrillholeConstants.checkAzi)
                            {
                                await ReformatResults(message, DrillholeConstants.IsNumeric, DrillholeConstants.checkAzi, fields, status, tableType, collarValues);
                            }
                            else if (message.validationTest == DrillholeConstants.checkDip)
                            {
                                await ReformatResults(message, DrillholeConstants.IsNumeric, DrillholeConstants.checkDip, fields, status, tableType, collarValues);
                            }
                        }

                        break;

                    case DrillholeConstants.Duplicates:
                        foreach (var message in test.testMessage)
                        {
                            await ReformatResults(message, DrillholeConstants.Duplicates, DrillholeConstants.checkHole, fields, status, tableType, collarValues);
                        }
                        break;

                    case DrillholeConstants.HoleLength:
                        foreach (var message in test.testMessage)
                        {
                            await ReformatResults(message, DrillholeConstants.HoleLength, DrillholeConstants.checkTD, fields, status, tableType, collarValues);
                        }
                        break;

                    case DrillholeConstants.ZeroCoordinate:
                        foreach (var message in test.testMessage)
                        {
                            await ReformatResults(message, DrillholeConstants.ZeroCoordinate, DrillholeConstants.checkCoord, fields, status, tableType, collarValues);
                        }
                        break;

                    case DrillholeConstants.SurveyRange:
                        foreach (var message in test.testMessage)
                        {
                            if (message.validationTest == DrillholeConstants.checkDip)
                            {
                                await ReformatResults(message, DrillholeConstants.SurveyRange, DrillholeConstants.checkDip, fields, status, tableType, collarValues);
                            }
                            else if (message.validationTest == DrillholeConstants.checkAzi)
                            {
                                await ReformatResults(message, DrillholeConstants.SurveyRange, DrillholeConstants.checkAzi, fields, status, tableType, collarValues);
                            }
                        }
                        break;
                }
            }


        }

        public virtual async Task<bool> ReformatResults(ValidationMessage message, string _TestType, string validation, List<string> fields, List<DrillholeMessageStatus> statusMessages, DrillholeTableType tableType,
            IEnumerable<XElement> collarValues)
        {

            foreach (var status in statusMessages)  //iterate three times for each error type
            {
                List<int> _status = message.ValidationStatus.Where(e => e.ErrorType == status).Select(p => p.id).ToList();

              //  int counter = 0;
                if (_status.Count > 0)
                {

                    List<GroupByHoles> groupedHoles = new List<GroupByHoles>();

                    List<int> ids = message.ValidationStatus.Where(e => e.ErrorType == status).Select(a => a.id).ToList(); //get IDs of all messages

                    List<string> SelectedHoles = new List<string>();
                    string holeIDName = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Select(n => n.columnHeader).FirstOrDefault(); //get hole name for querying XML

                    foreach (int id in ids) //get the holes for each record
                    {
                        var value = collarValues.Where(a => a.Attribute("ID").Value == id.ToString()).Select(h => h.Element(holeIDName).Value).FirstOrDefault();

                        SelectedHoles.Add(value);
                    }

                    var holes = SelectedHoles.GroupBy(x => x).Where(group => group.Count() >= 1).Select(group => group.Key).ToList(); //group the records under each hole

                    bool holeExists = true;
                    bool statusExists = true;

                    foreach (var queryHole in holes)
                    {
                        List<RowsToEdit> rows = new List<RowsToEdit>();  //new
                        List<GroupByTest> groupedTests = new List<GroupByTest>(); 
                        List<GroupByTable> groupedTables = new List<GroupByTable>();
                        List<GroupByTestField> groupedTestFields = new List<GroupByTestField>();

                        GroupByHoles hole = new GroupByHoles();
                        GroupByTable table = new GroupByTable();
                        GroupByTest test = new GroupByTest();
                        GroupByTestField testField = new GroupByTestField();
                        ReshapedDataToEdit dataToEdit = null;

                        statusExists = await CheckReshapedData(status);

                        if (!statusExists)
                        {
                            dataToEdit = new ReshapedDataToEdit() {ErrorType = status.ToString(), Ignore = false };
                            EditDrillholeData.Add(dataToEdit);
                            dataToEdit.GroupedHoles = groupedHoles;
                        }
                        else
                        {
                            dataToEdit = await ReturnDataStatus(status);
                        }

                        holeExists = await HoleExist(queryHole, status);

                        if (!holeExists) //means status and hole don't already exist so add
                        {
                            hole = new GroupByHoles() { Ignore = false, holeid = queryHole };
                           
                            List<GroupByHoles> testGroup = await ReturnGroupedHoles(status);

                            if (testGroup != null)
                                groupedHoles = testGroup;

                            groupedHoles.Add(hole);
                        }
                        else
                        {
                            groupedHoles = await ReturnHolesList(status, queryHole);

                            hole = await ReturnHole(queryHole, status);
                        }

                        bool tableExists = await TableExists(status, queryHole, tableType);

                        if (!tableExists)
                        {
                            //check for any other table
                            tableExists = await OtherTableExists(status, queryHole, tableType);

                            if (tableExists)
                            {
                                groupedTables = await ReturnOtherTableList(status, queryHole);
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
                            groupedTables = await ReturnTableList(status, queryHole, tableType);
                            table = await ReturnTable(status, queryHole, tableType);
                        }

                        hole.GroupedTables = groupedTables;

                        bool testFieldExists = await TestFieldExists(status, queryHole, tableType, _TestType, validation);

                        if (!testFieldExists)
                        {
                            testField = new GroupByTestField() { Ignore = false, TestField = validation, TableData = rows };
                            groupedTestFields.Add(testField);
                        }
                        else
                        {
                            groupedTestFields = await ReturnTestFields(status, queryHole, tableType, _TestType, validation);

                            testField = await ReturnTestField(status, queryHole, tableType, _TestType, validation);
                        }

                        bool testExists = await TestExists(status, queryHole, tableType, _TestType);

                        if (!testExists)
                        {
                            test = new GroupByTest() { MainTest = _TestType, Ignore = false, TestFields = groupedTestFields };
                            test.TestFields = groupedTestFields;

                            groupedTests = await ReturnTestList(status, queryHole, tableType);

                            groupedTests.Add(test);
                        }
                        else
                        {
                            groupedTests = await ReturnTestList(status, queryHole, tableType, _TestType);


                            test = await ReturnTest(status, queryHole, tableType, _TestType);
                            test.TestFields.Add(testField);

                        }

                        table.GroupedTests = groupedTests;

                        var value = collarValues.Where(a => a.Element(holeIDName).Value == queryHole).Select(v => v.Attribute("ID").Value).FirstOrDefault();
                        string tooltip = message.ValidationStatus.Where(e => e.ErrorType == status && e.id == Convert.ToInt32(value)).Select(p => p.Description).FirstOrDefault();

                        rows.Add(new RowsToEdit
                        {
                            id_col = Convert.ToInt16(value),
                            holeid = queryHole,
                            testType = _TestType,
                            validationTest = validation,
                            TableType = DrillholeTableType.collar,
                            ErrorType = status,
                            Description = tooltip
                        });

                       // counter++;
                    }

                }

            }

            return true;
        }

        public async Task<ReshapedDataToEdit> ReturnDataStatus(DrillholeMessageStatus ErrorStatus)
        {

            var status = EditDrillholeData.Where(t => t.ErrorType == ErrorStatus.ToString()).Where(i => i.Ignore == false).FirstOrDefault();

            return status;

        }

        public virtual async Task<List<string>>ReturnFieldnamesForXmlQuery()
        {
            List<string> fields = new List<string>();
            fields.Add(importCollarFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false) //get name of hole field for querying XML
                .Select(s => s.columnHeader).FirstOrDefault());
            fields.Add(importCollarFields.Where(o => o.columnImportName == DrillholeConstants.xName).Where(m => m.genericType == false) //get other feilds which are required as mandatory, i.e. XYZ and depth
                .Select(s => s.columnHeader).FirstOrDefault());
            fields.Add(importCollarFields.Where(o => o.columnImportName == DrillholeConstants.yName).Where(m => m.genericType == false)
                .Select(s => s.columnHeader).FirstOrDefault());
            fields.Add(importCollarFields.Where(o => o.columnImportName == DrillholeConstants.zName).Where(m => m.genericType == false)
                .Select(s => s.columnHeader).FirstOrDefault());
            fields.Add(importCollarFields.Where(o => o.columnImportName == DrillholeConstants.maxName).Where(m => m.genericType == false)
                .Select(s => s.columnHeader).FirstOrDefault());

            if (surveyType == DrillholeSurveyType.collarsurvey) //if using dip and azimuth in collar and not downhole survey table
            {
                fields.Add(importCollarFields.Where(o => o.columnImportName == DrillholeConstants.azimuthName).Where(m => m.genericType == false)
                    .Select(s => s.columnHeader).FirstOrDefault());
                fields.Add(importCollarFields.Where(o => o.columnImportName == DrillholeConstants.dipName).Where(m => m.genericType == false)
                    .Select(s => s.columnHeader).FirstOrDefault());
            }

            return fields;
        }

        public async Task<List<DrillholeMessageStatus>>ReturnStatusAlerts()
        {
            List<DrillholeMessageStatus> statusMessages = new List<DrillholeMessageStatus>();
            statusMessages.Add(DrillholeMessageStatus.Error);
            statusMessages.Add(DrillholeMessageStatus.Warning);
          //  statusMessages.Add(DrillholeMessageStatus.Valid);

            return statusMessages;
        }


        public async Task<bool> CheckReshapedData(DrillholeMessageStatus ErrorStatus)
        {
            var status = EditDrillholeData.Where(t => t.ErrorType == ErrorStatus.ToString()).Where(i => i.Ignore == false).FirstOrDefault();

            if (status == null)
                return false;


            return true;
        }


        public async Task<bool> TestExists(DrillholeMessageStatus ErrorStatus, string holeID, DrillholeTableType tableType, string testType)
        {
            var status = EditDrillholeData.Where(t => t.ErrorType == ErrorStatus.ToString()).Where(i => i.Ignore == false).Select(h => h.GroupedHoles);

            if (status != null)
            {
                foreach (var hole in status)
                {
                    var check = hole.Where(h => h.holeid == holeID).Count();

                    if (check > 0)
                    {
                        var tables = hole.Where(h => h.holeid == holeID).Select(t => t.GroupedTables);

                        foreach (var table in tables)
                        {
                            var checkTable = table.Where(n => n.TableType == tableType.ToString()).Count();

                            if (checkTable > 0)
                            {
                                var results = table.Where(n => n.TableType == tableType.ToString()).Select(r => r.GroupedTests).FirstOrDefault();

                                if (results == null)
                                    return false;

                                foreach (var result in results)
                                {
                                    if (result.MainTest == testType)
                                    {
                                        return true;

                                    }
                                }

                            }
                        }
                    }
                }
            }

            
            return false;
        }


        public async Task<GroupByTest> ReturnTest(DrillholeMessageStatus ErrorStatus, string holeID, DrillholeTableType tableType, string testType)
        {
            var status = EditDrillholeData.Where(t => t.ErrorType == ErrorStatus.ToString()).Where(i => i.Ignore == false).Select(h => h.GroupedHoles);

            if (status != null)
            {
                foreach (var holes in status)
                {
                    var check = holes.Where(h => h.holeid == holeID).Count();

                    if (check > 0)
                    {
                        var tables = holes.Where(h => h.holeid == holeID).Select(t => t.GroupedTables);

                        foreach (var table in tables)
                        {
                            var tests = table.Where(n => n.TableType == tableType.ToString()).Select(t => t.GroupedTests).FirstOrDefault();

                            var checkTest = tests.Where(a => a.MainTest == testType).Count();

                            if (checkTest > 0)
                            {
                                return tests.Where(a => a.MainTest == testType).FirstOrDefault();

                            }
                        }
                    }
                }
            }

            return null;
        }


        public async Task<List<GroupByTestField>> ReturnTestFields( DrillholeMessageStatus ErrorStatus, string holeID, DrillholeTableType tableType, string testType, string validation)
        {
            List<GroupByTestField> rows = new List<GroupByTestField>();

            var status = EditDrillholeData.Where(t => t.ErrorType == ErrorStatus.ToString()).Where(i => i.Ignore == false).Select(h => h.GroupedHoles);

            if (status != null)
            {
                foreach (var hole in status)
                {
                    var check = hole.Where(h => h.holeid == holeID).Count();

                    if (check > 0)
                    {
                        var tables = hole.Where(h => h.holeid == holeID).Select(t => t.GroupedTables);

                        foreach (var table in tables)
                        {
                            if (table.Where(n => n.TableType == tableType.ToString()) != null)
                            {
                                var results = table.Where(n => n.TableType == tableType.ToString()).Select(r => r.GroupedTests).FirstOrDefault();

                                foreach (var result in results)
                                {
                                    if (result.MainTest == testType)
                                    {
                                        var values = result.TestFields;

                                        if (values.Where(b => b.TestField == validation) != null)
                                            rows = result.TestFields;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return rows;
        }

        public async Task<bool> TestFieldExists( DrillholeMessageStatus ErrorStatus, string holeID, DrillholeTableType tableType, string testType, string validation)
        {
            var status = EditDrillholeData.Where(t => t.ErrorType == ErrorStatus.ToString()).Where(i => i.Ignore == false).Select(h => h.GroupedHoles);

            if (status != null)
            {
                foreach (var hole in status)
                {
                    var check = hole.Where(h => h.holeid == holeID).Count();

                    if (check > 0)
                    {
                        var tables = hole.Where(h => h.holeid == holeID).Select(t => t.GroupedTables);

                        foreach (var table in tables)
                        {
                            var checkTable = table.Where(n => n.TableType == tableType.ToString()).Count();

                            if (checkTable > 0)
                            {
                                var results = table.Where(n => n.TableType == tableType.ToString()).Select(r => r.GroupedTests).FirstOrDefault();

                                if (results == null)
                                    return false;

                                foreach (var result in results)
                                {
                                    
                                    if (result.MainTest == testType)
                                    {
                                        var values = result.TestFields;

                                        foreach (var value in values)
                                        {
                                            if (value.TestField == validation)
                                                return true;
                                        }

                                    }
                                }

                            }
                        }
                    }
                }
            }

            return false;
        }

        public async Task<GroupByTestField> ReturnTestField(DrillholeMessageStatus ErrorStatus, string holeID, DrillholeTableType tableType, string testType, string validation)
        {
            var status = EditDrillholeData.Where(t => t.ErrorType == ErrorStatus.ToString()).Where(i => i.Ignore == false).Select(h => h.GroupedHoles);

            if (status != null)
            {
                foreach (var hole in status)
                {
                    var check = hole.Where(h => h.holeid == holeID).Count();

                    if (check > 0)
                    {
                        var tables = hole.Where(h => h.holeid == holeID).Select(t => t.GroupedTables);

                        foreach (var table in tables)
                        {
                            if (table.Where(n => n.TableType == tableType.ToString()) != null)
                            {
                                var results = table.Where(n => n.TableType == tableType.ToString()).Select(r => r.GroupedTests).FirstOrDefault();

                                foreach (var result in results)
                                {
                                    if (result.MainTest == testType)
                                    {
                                        var values = result.TestFields;

                                        foreach (var value in values)
                                        {
                                            if (value.TestField == validation)
                                                return value;
                                        }

                                    }
                                }

                            }
                        }
                    }
                }
            }

            return null;
        }

        public async Task<List<GroupByTest>> ReturnTestList(DrillholeMessageStatus ErrorStatus, string holeID, DrillholeTableType tableType, string testType)
        {
            List<GroupByTest> groupedTests = new List<GroupByTest>();

            var status = EditDrillholeData.Where(t => t.ErrorType == ErrorStatus.ToString()).Where(i => i.Ignore == false).Select(h => h.GroupedHoles);

            if (status != null)
            {
                foreach (var holes in status)
                {
                    var check = holes.Where(h => h.holeid == holeID).Count();

                    if (check > 0)
                    {
                        var tables = holes.Where(h => h.holeid == holeID).Select(t => t.GroupedTables);

                        foreach (var table in tables)
                        {
                            var tests = table.Where(n => n.TableType == tableType.ToString()).Select(t => t.GroupedTests).FirstOrDefault();

                            if (tests.Where(a => a.MainTest == testType) != null)
                            {
                                return tests;

                            }

                        }
                    }
                }
            }

            return groupedTests;
        }

        public async Task<List<GroupByTest>> ReturnTestList(DrillholeMessageStatus ErrorStatus, string holeID, DrillholeTableType tableType)
        {
            List<GroupByTest> groupedTests = new List<GroupByTest>();

            var status = EditDrillholeData.Where(t => t.ErrorType == ErrorStatus.ToString()).Where(i => i.Ignore == false).Select(h => h.GroupedHoles);

            if (status != null)
            {
                foreach (var holes in status)
                {
                    var check = holes.Where(h => h.holeid == holeID).Count();

                    if (check > 0)
                    {
                        var tables = holes.Where(h => h.holeid == holeID).Select(t => t.GroupedTables);

                        foreach (var table in tables)
                        {
                            
                            var checkTable = table.Where(n => n.TableType == tableType.ToString()).Select(t => t.GroupedTests).FirstOrDefault();

                            if (checkTable == null)
                                return groupedTests;
                            else
                                return checkTable;

                        }
                    }
                }
            }

            return groupedTests;
        }

        public async Task<bool> TableExists(DrillholeMessageStatus ErrorStatus, string holeID, DrillholeTableType tableType)
        {
            var status = EditDrillholeData.Where(t => t.ErrorType == ErrorStatus.ToString()).Where(i => i.Ignore == false).Select(h => h.GroupedHoles);

            if (status != null)
            {
                foreach (var hole in status)
                {
                    var check = hole.Where(h => h.holeid == holeID).Count();

                    if (check > 0)
                    {

                        var tables = hole.Where(h => h.holeid == holeID).Select(t => t.GroupedTables);

                        foreach (var table in tables)
                        {
                            if (table == null)
                                return false;

                            var checkTable = table.Where(n => n.TableType == tableType.ToString()).Count();

                            if (checkTable > 0)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;

        }

        public async Task<bool> OtherTableExists(DrillholeMessageStatus ErrorStatus, string holeID, DrillholeTableType tableType)
        {
            var status = EditDrillholeData.Where(t => t.ErrorType == ErrorStatus.ToString()).Where(i => i.Ignore == false).Select(h => h.GroupedHoles);

            if (status != null)
            {
                foreach (var hole in status)
                {
                    var check = hole.Where(h => h.holeid == holeID).Count();

                    if (check > 0)
                    {
                        var tables = hole.Where(h => h.holeid == holeID).Select(t => t.GroupedTables).Count();

                        if(tables > 0)
                        {
                            return true;
                        }

                    }
                }
            }

            return false;

        }

        public async Task<GroupByTable> ReturnTable(DrillholeMessageStatus ErrorStatus, string holeID, DrillholeTableType tableType)
        {
            var status = EditDrillholeData.Where(t => t.ErrorType == ErrorStatus.ToString()).Where(i => i.Ignore == false).Select(h => h.GroupedHoles);

            if (status != null)
            {
                foreach (var hole in status)
                {
                    var check = hole.Where(h => h.holeid == holeID).Count();

                    if (check > 0)
                    {
                        var tables = hole.Where(h => h.holeid == holeID).Select(t => t.GroupedTables);

                        foreach (var table in tables)
                        {
                            if (table.Where(n => n.TableType == tableType.ToString()) != null)
                            {
                                return table.Where(n => n.TableType == tableType.ToString()).FirstOrDefault();
                            }
                        }
                    }
                }
            }

            return null;

        }

        public async Task<GroupByTable> ReturnOtherTable(DrillholeMessageStatus ErrorStatus, string holeID)
        {
            var status = EditDrillholeData.Where(t => t.ErrorType == ErrorStatus.ToString()).Where(i => i.Ignore == false).Select(h => h.GroupedHoles);

            if (status != null)
            {
                foreach (var hole in status)
                {
                    var check = hole.Where(h => h.holeid == holeID).Count();

                    if (check > 0)
                    {
                        var tables = hole.Where(h => h.holeid == holeID).Select(t => t.GroupedTables).FirstOrDefault();

                            return tables[0];
                        
                    }
                }
            }

            return null;

        }

        public async Task<List<GroupByTable>> ReturnTableList(DrillholeMessageStatus ErrorStatus, string holeID, DrillholeTableType tableType)
        {
            List<GroupByTable> groupedTables = new List<GroupByTable>();

            var status = EditDrillholeData.Where(t => t.ErrorType == ErrorStatus.ToString()).Where(i => i.Ignore == false).Select(h => h.GroupedHoles);

            if (status != null)
            {
                foreach (var hole in status)
                {
                    var check = hole.Where(h => h.holeid == holeID);

                    if (check != null)
                    {
                        var tables = hole.Where(h => h.holeid == holeID).Select(t => t.GroupedTables);

                        foreach (var table in tables)
                        {
                            var checkTable = table.Where(n => n.TableType == tableType.ToString()).Count();


                            if (checkTable > 0)
                            {
                                return table;
                            }
                        }
                    }
                }
            }

            return groupedTables;

        }

        public async Task<List<GroupByTable>> ReturnOtherTableList(DrillholeMessageStatus ErrorStatus, string holeID)
        {
            List<GroupByTable> groupedTables = new List<GroupByTable>();

            var status = EditDrillholeData.Where(t => t.ErrorType == ErrorStatus.ToString()).Where(i => i.Ignore == false).Select(h => h.GroupedHoles);

            if (status != null)
            {
                foreach (var hole in status)
                {
                    var check = hole.Where(h => h.holeid == holeID);

                    if (check != null)
                    {
                        var tables = hole.Where(h => h.holeid == holeID).Select(t => t.GroupedTables);

                        if (tables != null)
                        {
                            foreach(var table in tables)
                            {
                                if (table != null)
                                    return table;
                            }
                        }
                    }
                }
            }

            return groupedTables;

        }


        public async Task<List<GroupByHoles>> ReturnHolesList(DrillholeMessageStatus ErrorStatus, string holeID)
        {
            List<GroupByHoles> rows = new List<GroupByHoles>();

            var holes = EditDrillholeData.Where(t => t.ErrorType == ErrorStatus.ToString()).Where(i => i.Ignore == false).Select(h => h.GroupedHoles);

            if (holes != null)
            {
                foreach (var hole in holes)
                {
                    var check = hole.Where(h => h.holeid == holeID).Count();

                    if (check > 0)
                    {
                        rows = hole;

                        break;
                    }
                }

            }

            return rows;
        }

        public async Task<List<GroupByHoles>> ReturnGroupedHoles(DrillholeMessageStatus ErrorStatus)
        {
           //List<GroupByHoles> rows = new List<GroupByHoles>();

            List<GroupByHoles> rows = EditDrillholeData.Where(t => t.ErrorType == ErrorStatus.ToString()).Where(i => i.Ignore == false).Select(h => h.GroupedHoles).FirstOrDefault();

          
            return rows;
        }



        public async Task<bool> HoleExist(string holeID, DrillholeMessageStatus ErrorStatus)
        {
            bool bCheck = false;

            var holes = EditDrillholeData.Where(t => t.ErrorType == ErrorStatus.ToString()).Where(i => i.Ignore == false).Select(h => h.GroupedHoles);

            if (holes != null)
            {
                foreach (var hole in holes)
                {
                    var check = hole.Where(h => h.holeid == holeID).Count();

                    if (check > 0)
                    {
                        return true;
                    }
                }

            }

            return bCheck;

        }


        public async Task<GroupByHoles> ReturnHole(string holeID, DrillholeMessageStatus ErrorStatus)
        {

            var holes = EditDrillholeData.Where(t => t.ErrorType == ErrorStatus.ToString()).Where(i => i.Ignore == false).Select(h => h.GroupedHoles);

            if (holes != null)
            {
                foreach (var hole in holes)
                {
                    var value = hole.Where(h => h.holeid == holeID).FirstOrDefault();

                    if (value != null)
                        return value;

                }

            }


            return null;

        }

        #endregion

        #region Actual Data
        public virtual async Task<DataTable> PopulateGridValues(List<RowsToEdit> _edit, DrillholeTableType tableType, bool preview)
        {
            DataTable dataTable = new DataTable();
            var collar = xmlCollarData.Elements();

            dataTable = await AddColumns(tableType, preview);

            List<object> rowValues = new List<object>();

            //holeid

            //x
            var x = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.xName).Select(m => m.columnHeader).Single();
            
            //y
            var y = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.yName).Select(m => m.columnHeader).Single();

            //z
            var z = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.zName).Select(m => m.columnHeader).Single();

            var holeID = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Select(m => m.columnHeader).FirstOrDefault();


            //max depth
            var depth = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.maxName).Select(m => m.columnHeader).Single();

            foreach (var value in _edit) //populate dataTable
            {
                // rowValues.Add(value.Ignore);
                rowValues.Add(value.id_col.ToString());
                rowValues.Add(collar.Where(h => h.Attribute("ID").Value == value.id_col.ToString()).Select(o => o.Element(holeID).Value).Single());
                rowValues.Add(collar.Where(h => h.Attribute("ID").Value == value.id_col.ToString()).Select(o => o.Element(x).Value).Single());
                rowValues.Add(collar.Where(h => h.Attribute("ID").Value == value.id_col.ToString()).Select(o => o.Element(y).Value).Single());
                rowValues.Add(collar.Where(h => h.Attribute("ID").Value == value.id_col.ToString()).Select(o => o.Element(z).Value).Single());
                rowValues.Add(collar.Where(h => h.Attribute("ID").Value == value.id_col.ToString()).Select(o => o.Element(depth).Value).Single());

                if (surveyType == DrillholeSurveyType.collarsurvey)
                {
                    var azi = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.azimuthName).Select(m => m.columnHeader).Single();
                    var dip = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.dipName).Select(m => m.columnHeader).Single();

                    rowValues.Add(collar.Where(h => h.Attribute("ID").Value == value.id_col.ToString()).Select(o => o.Element(azi).Value).Single());
                    rowValues.Add(collar.Where(h => h.Attribute("ID").Value == value.id_col.ToString()).Select(o => o.Element(dip).Value).Single());
                }

                rowValues.Add(value.testType);

                if (!preview)
                    rowValues.Add(value.Description);

                dataTable.Rows.Add(rowValues.ToArray());
            }

            return dataTable; //TODO
        }

        public virtual async Task<DataTable> AddColumns(DrillholeTableType tableType, bool preview)
        {
            DataTable dataTable = new DataTable();
            // dataTable.Columns.Add("Ignore");
            dataTable.Columns.Add("ID");
            dataTable.Columns.Add(DrillholeConstants.holeID);
            dataTable.Columns.Add(DrillholeConstants.x);
            dataTable.Columns.Add(DrillholeConstants.y);
            dataTable.Columns.Add(DrillholeConstants.z);
            dataTable.Columns.Add(DrillholeConstants.maxDepth);

            if (surveyType == DrillholeSurveyType.collarsurvey)
            {
                dataTable.Columns.Add(DrillholeConstants.azimuth);
                dataTable.Columns.Add(DrillholeConstants.dip);
            }

            dataTable.Columns.Add("Validation");

            if (!preview)
                dataTable.Columns.Add("Description");

            return dataTable;
        }

        public async Task<RowsToEdit> CollarRow(string holeID, string testType)
        {
            List<string> fields = new List<string>();
            fields.Add(importCollarFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false)
                .Select(s => s.columnHeader).FirstOrDefault());
            fields.Add(importCollarFields.Where(o => o.columnImportName == DrillholeConstants.xName).Where(m => m.genericType == false)
                .Select(s => s.columnHeader).FirstOrDefault());
            fields.Add(importCollarFields.Where(o => o.columnImportName == DrillholeConstants.yName).Where(m => m.genericType == false)
                .Select(s => s.columnHeader).FirstOrDefault());
            fields.Add(importCollarFields.Where(o => o.columnImportName == DrillholeConstants.zName).Where(m => m.genericType == false)
                .Select(s => s.columnHeader).FirstOrDefault());
            fields.Add(importCollarFields.Where(o => o.columnImportName == DrillholeConstants.maxName).Where(m => m.genericType == false)
                .Select(s => s.columnHeader).FirstOrDefault());

            var collarValues = xmlCollarData.Elements();

            var queryHole = collarValues.Where(h => h.Element(fields[0]).Value == holeID).Select(v => v.Attribute("ID").Value).FirstOrDefault();
            var queryX = collarValues.Where(h => h.Element(fields[0]).Value == holeID).Select(v => v.Element(fields[1]).Value).FirstOrDefault();
            var queryY = collarValues.Where(h => h.Element(fields[0]).Value == holeID).Select(v => v.Element(fields[2]).Value).FirstOrDefault();
            var queryZ = collarValues.Where(h => h.Element(fields[0]).Value == holeID).Select(v => v.Element(fields[3]).Value).FirstOrDefault();
            var queryTD = collarValues.Where(h => h.Element(fields[0]).Value == holeID).Select(v => v.Element(fields[4]).Value).FirstOrDefault();

            RowsToEdit collarRow = new RowsToEdit();
            collarRow.id_col = Convert.ToInt16(queryHole);
            collarRow.holeid = holeID;
            collarRow.x = queryX;
            collarRow.y = queryY;
            collarRow.z = queryZ;
            collarRow.maxDepth = queryTD;
            collarRow.testType = testType;

            return collarRow;
        }

        public void SetDataContext(DataGrid dataEdits, DataTable dataTable)
        {
            if (dataTable != null)
            {
                if (dataTable.Columns.Count > 0)
                    dataEdits.DataContext = dataTable;
            }
        }

        #endregion

    }

}

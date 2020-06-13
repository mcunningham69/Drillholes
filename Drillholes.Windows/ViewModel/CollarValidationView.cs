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

        #region Edits
        public virtual async void ReshapeMessages()
        {
            List<string> fields = await ReturnFieldnamesForXmlQuery();
            List<DrillholeMessageStatus> status = await ReturnStatusAlerts();

            List<ReshapedDataToEdit> reshapedData = new List<ReshapedDataToEdit>();  //setup main variable used for data binding. It will have 3 entries for Error, Warning and Valid

            foreach (var test in ShowTestMessages)  //get messages after running validation above
            {
                switch (test.testType) //select on the different testtype
                {
                    case DrillholeConstants.IsEmptyOrNull:
                        foreach (var message in test.testMessage)
                        {
                            if (message.validationTest == DrillholeConstants.checkHole)  //specific field being checked
                            {
                                await ReformatResults(message, DrillholeConstants.IsEmptyOrNull, DrillholeConstants.checkHole, fields, status, reshapedData);
                            }
                            else if (message.validationTest == DrillholeConstants.checkX)
                            {
                                await ReformatResults(message, DrillholeConstants.IsEmptyOrNull, DrillholeConstants.checkX, fields, status, reshapedData);
                            }
                            else if (message.validationTest == DrillholeConstants.checkY)
                            {
                                await ReformatResults(message, DrillholeConstants.IsEmptyOrNull, DrillholeConstants.checkY, fields, status, reshapedData);
                            }
                            else if (message.validationTest == DrillholeConstants.checkZ)
                            {
                                await ReformatResults(message, DrillholeConstants.IsEmptyOrNull, DrillholeConstants.checkZ, fields, status, reshapedData);
                            }
                            else if (message.validationTest == DrillholeConstants.checkTD)
                            {
                                await ReformatResults(message, DrillholeConstants.IsEmptyOrNull, DrillholeConstants.checkTD, fields, status, reshapedData);
                            }
                            else if (message.validationTest == DrillholeConstants.checkAzi)
                            {
                                await ReformatResults(message, DrillholeConstants.IsEmptyOrNull, DrillholeConstants.checkAzi, fields, status, reshapedData);
                            }
                            else if (message.validationTest == DrillholeConstants.checkDip)
                            {
                                await ReformatResults(message, DrillholeConstants.IsEmptyOrNull, DrillholeConstants.checkDip, fields, status, reshapedData);
                            }
                        }
                        break;

                    case DrillholeConstants.IsNumeric:
                        foreach (var message in test.testMessage)
                        {
                            if (message.validationTest == DrillholeConstants.checkX)
                            {
                                await ReformatResults(message, DrillholeConstants.IsNumeric, DrillholeConstants.checkX, fields, status, reshapedData);
                            }
                            else if (message.validationTest == DrillholeConstants.checkY)
                            {
                                await ReformatResults(message, DrillholeConstants.IsNumeric, DrillholeConstants.checkY, fields, status, reshapedData);
                            }
                            else if (message.validationTest == DrillholeConstants.checkZ)
                            {
                                await ReformatResults(message, DrillholeConstants.IsNumeric, DrillholeConstants.checkZ, fields, status, reshapedData);
                            }
                            else if (message.validationTest == DrillholeConstants.checkTD)
                            {
                                await ReformatResults(message, DrillholeConstants.IsNumeric, DrillholeConstants.checkTD, fields, status, reshapedData);
                            }
                            else if (message.validationTest == DrillholeConstants.checkAzi)
                            {
                                await ReformatResults(message, DrillholeConstants.IsNumeric, DrillholeConstants.checkAzi, fields, status, reshapedData);
                            }
                            else if (message.validationTest == DrillholeConstants.checkDip)
                            {
                                await ReformatResults(message, DrillholeConstants.IsNumeric, DrillholeConstants.checkDip, fields, status, reshapedData);
                            }
                        }

                        break;

                    case DrillholeConstants.Duplicates:
                        foreach (var message in test.testMessage)
                        {
                            await ReformatResults(message, DrillholeConstants.Duplicates, DrillholeConstants.checkHole, fields, status, reshapedData);
                        }
                        break;

                    case DrillholeConstants.HoleLength:
                        foreach (var message in test.testMessage)
                        {
                            await ReformatResults(message, DrillholeConstants.HoleLength, DrillholeConstants.checkTD, fields, status, reshapedData);
                        }
                        break;

                    case DrillholeConstants.ZeroCoordinate:
                        foreach (var message in test.testMessage)
                        {
                            await ReformatResults(message, DrillholeConstants.ZeroCoordinate, DrillholeConstants.checkCoord, fields, status, reshapedData);
                        }
                        break;

                    case DrillholeConstants.SurveyRange:
                        foreach (var message in test.testMessage)
                        {
                            if (message.validationTest == DrillholeConstants.checkDip)
                            {
                                await ReformatResults(message, DrillholeConstants.SurveyRange, DrillholeConstants.checkDip, fields, status, reshapedData);
                            }
                            else if (message.validationTest == DrillholeConstants.checkAzi)
                            {
                                await ReformatResults(message, DrillholeConstants.SurveyRange, DrillholeConstants.checkAzi, fields, status, reshapedData);
                            }
                        }
                        break;


                }
            }


        }

        //First run 
        public virtual async Task<bool> ReformatResults(ValidationMessage message, string _TestType, string validation, List<string> fields, List<DrillholeMessageStatus> statusMessages, List<ReshapedDataToEdit> reshapedData)
        {
            var collarValues = xmlCollarData.Elements();  //get dollar data from stored XML

            foreach (var status in statusMessages)  //iterate three times for each error type
            {
                List<int> _status = message.ValidationStatus.Where(e => e.ErrorType == status).Select(p => p.id).ToList();

                if (_status.Count > 0)
                {
                    List<string> tooltips = message.ValidationStatus.Where(e => e.ErrorType == status).Select(p => p.Description).ToList();

                    ReshapedDataToEdit data = null;
                    bool bCheck = await CheckReshapedData(reshapedData, status);

                    //if (bCheck)
                    //{

                    //    data = reshapedData.Where(d => d.ErrorType == status.ToString()).FirstOrDefault();
                    //}
                    //else
                    //{
                    //    data = new ReshapedDataToEdit() { Count = 0, ErrorType = status.ToString(), Ignore = false };
                    //}

                    //List<GroupByHoles> groupedHoles = null;
                    //List<RowsToEdit> rows = new List<RowsToEdit>();  //new
                    //List<GroupByTest> groupedTests = null;
                    //List<GroupByTable> groupedTables = null;
                    //List<GroupByTestField> groupedTestFields = new List<GroupByTestField>();

                    foreach (int valid in _status)
                    {
                        List<GroupByHoles> groupedHoles = new List<GroupByHoles>();

                        List<RowsToEdit> rows = new List<RowsToEdit>();  //new
                        List<GroupByTest> groupedTests = new List<GroupByTest>(); ;
                        List<GroupByTable> groupedTables = new List<GroupByTable>();
                        List<GroupByTestField> groupedTestFields = new List<GroupByTestField>();

                        GroupByHoles hole = null;
                        GroupByTable table = null;
                        GroupByTest test = null;
                        GroupByTestField testField = null;

                        var queryHole = collarValues.Where(h => h.Attribute("ID").Value == valid.ToString()).Select(v => v.Element(fields[0]).Value).FirstOrDefault();

                        groupedHoles = await ReturnHolesList(reshapedData, status, queryHole);

                        bool holeExists = await HoleExist(reshapedData, queryHole, status);

                        if (!holeExists) //means status and hole don't already exist so add
                        {
                            hole = new GroupByHoles() { Ignore = false, holeid = queryHole };
                            groupedHoles.Add(hole);
                        }
                        else
                            hole = await ReturnHole(reshapedData, queryHole, status);

                        groupedTables = await ReturnTableList(reshapedData, status, queryHole, DrillholeTableType.collar);

                        bool tableExists = await TableExists(reshapedData, status, queryHole, DrillholeTableType.collar);

                        if (!tableExists)
                        {
                            table = new GroupByTable() { TableType = "collar", Ignore = false };
                            groupedTables.Add(table);
                        }
                        else
                            table = await ReturnTable(reshapedData, status, queryHole, DrillholeTableType.collar);

                        hole.GroupedTables = groupedTables;

                        testField = new GroupByTestField() { Ignore = false, TestField = validation, TableData = rows };
                        groupedTestFields.Add(testField);

                        groupedTests = await ReturnTestList(reshapedData, status, queryHole, DrillholeTableType.collar, _TestType);
                        bool testExists = await TestExists(reshapedData, status, queryHole, DrillholeTableType.collar, _TestType);

                        if (!testExists)
                        {
                            test = new GroupByTest() { MainTest = _TestType, Ignore = false, TestFields = groupedTestFields };
                            groupedTests.Add(test);
                        }
                        else
                        {
                            test = await ReturnTest(reshapedData, status, queryHole, DrillholeTableType.collar, _TestType);
                            test.TestFields.Add(testField);

                        }

                        table.GroupedTests = groupedTests;

                        //bool testFieldExists = await TestFieldExists(reshapedData, status, queryHole, DrillholeTableType.collar, _TestType, validation);

                        //if (!testFieldExists)
                        //{
                        //    groupedTestFields.Add(new GroupByTestField() { Ignore = false, TestField = validation, TableData = rows });
                        //}
                        //else
                        //    groupedTestFields.Add(testField);

                        var tips = tooltips[valid];

                        rows.Add(new RowsToEdit
                        {
                            id_col = valid,
                            // holeid = queryHole,
                            //  x = queryX,
                            // y = queryY,
                            //  z = queryZ,
                            // maxDepth = queryTD,
                            // azimuth = queryAzi,
                            // dip = queryDip,
                            //ErrorType = status,
                            testType = _TestType,
                            validationTest = validation,
                            Description = tips
                        });

                        if (!bCheck)
                            reshapedData.Add(new ReshapedDataToEdit { Count = 0, ErrorType = status.ToString(), Ignore = false, GroupedHoles = groupedHoles });



                    }
                }
                else
                {
                    //reshapedData.Add(new ReshapedDataToEdit { Count = 0, Ignore = true, ErrorType = status.ToString(), GroupedHoles = groupedHoles });
                    //groupedHoles.Add(new GroupByHoles() { Ignore = true, holeid = "", GroupedTables = groupedTables });
                    //groupedTests.Add(new GroupByTest() { Ignore = true, MainTest = _TestType, TestFields = groupedTestFields });
                    //groupedTables.Add(new GroupByTable() { Ignore = true, GroupedTests = groupedTests, TableType = "collar" });
                    //groupedTestFields.Add(new GroupByTestField() { Ignore = true, TestField = validation, TableData = rows }); 
                    //rows.Add(new RowsToEdit() { Ignore = true });
                }
            }

            return true;
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
            statusMessages.Add(DrillholeMessageStatus.Valid);

            return statusMessages;
        }

        public async Task<bool> CheckReshapedData(List<ReshapedDataToEdit> reshapedData, DrillholeMessageStatus ErrorStatus)
        {
            var status = reshapedData.Where(t => t.ErrorType == ErrorStatus.ToString()).Where(i => i.Ignore == false).FirstOrDefault();

            if (status == null)
                return false;
            

            return true;
        }
        public async Task<List<RowsToEdit>> ReturnRowsToEditList()
        {
            List<RowsToEdit> rows = new List<RowsToEdit>();

            return rows;
        }

        public async Task<List<GroupByTestField>> ReturnTestFieldsList()
        {
            List<GroupByTestField> rows = new List<GroupByTestField>();

            return rows;
        }

        public async Task<bool> TestExists(List<ReshapedDataToEdit> reshapedData, DrillholeMessageStatus ErrorStatus, string holeID, DrillholeTableType tableType, string testType)
        {
            var status = reshapedData.Where(t => t.ErrorType == ErrorStatus.ToString()).Where(i => i.Ignore == false).FirstOrDefault();

            if (status == null)
            {
                return false;
            }
            else
            {
                var hole = status.GroupedHoles.Where(h => h.holeid == holeID).FirstOrDefault();

                if (hole == null)
                    return false;
                else
                {
                    return true;
                }
            }
        }

        public async Task<GroupByTest> ReturnTest(List<ReshapedDataToEdit> reshapedData, DrillholeMessageStatus ErrorStatus, string holeID, DrillholeTableType tableType, string testType)
        {
            var status = reshapedData.Where(t => t.ErrorType == ErrorStatus.ToString()).Where(i => i.Ignore == false).Select(h => h.GroupedHoles);

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
                                return tests.Where(a => a.MainTest == testType).FirstOrDefault();

                            }
                        }
                    }
                }
            }

            return null;
        }

        public async Task<bool> TestFieldExists(List<ReshapedDataToEdit> reshapedData, DrillholeMessageStatus ErrorStatus, string holeID, DrillholeTableType tableType, string testType, string validation)
        {
            var status = reshapedData.Where(t => t.ErrorType == ErrorStatus.ToString()).Where(i => i.Ignore == false).Select(h => h.GroupedHoles);

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
                                        return true;
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        public async Task<List<GroupByTest>> ReturnTestList(List<ReshapedDataToEdit> reshapedData, DrillholeMessageStatus ErrorStatus, string holeID, DrillholeTableType tableType, string testType)
        {
            List<GroupByTest> groupedTests = new List<GroupByTest>();

            var status = reshapedData.Where(t => t.ErrorType == ErrorStatus.ToString()).Where(i => i.Ignore == false).Select(h => h.GroupedHoles);

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
                                groupedTests = tests;

                            }
                        } 
                    }
                }
            }

            return groupedTests;
        }

        public async Task<bool> TableExists(List<ReshapedDataToEdit> reshapedData, DrillholeMessageStatus ErrorStatus, string holeID, DrillholeTableType tableType)
        {
            var status = reshapedData.Where(t => t.ErrorType == ErrorStatus.ToString()).Where(i => i.Ignore == false).Select(h => h.GroupedHoles);

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
                                return true;
                            }
                        }
                    }
                }
            }

            return false;

        }

        public async Task<GroupByTable> ReturnTable(List<ReshapedDataToEdit> reshapedData, DrillholeMessageStatus ErrorStatus, string holeID, DrillholeTableType tableType)
        {
            var status = reshapedData.Where(t => t.ErrorType == ErrorStatus.ToString()).Where(i => i.Ignore == false).Select(h => h.GroupedHoles);

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

        public async Task<List<GroupByTable>> ReturnTableList(List<ReshapedDataToEdit> reshapedData, DrillholeMessageStatus ErrorStatus, string holeID, DrillholeTableType tableType)
        {
            List<GroupByTable> groupedTables = new List<GroupByTable>(); ;

            var status = reshapedData.Where(t => t.ErrorType == ErrorStatus.ToString()).Where(i => i.Ignore == false).Select(h => h.GroupedHoles);

            if (status != null)
            {
                foreach (var hole in status)
                {
                    var check = hole.Where(h => h.holeid == holeID).Count();

                    if (check > 0)
                    {
                        var tables = hole.Where(h => h.holeid == holeID).Select(t => t.GroupedTables);

                        foreach(var table in tables)
                        {
                            if (table.Where(n => n.TableType == tableType.ToString()) != null)
                            {
                                return table;
                            }
                        }
                    }
                }
            }
            
            return groupedTables;

        }

        public async Task<List<GroupByHoles>> ReturnHolesList(List<ReshapedDataToEdit> reshapedData, DrillholeMessageStatus ErrorStatus, string holeID)
        {
            List<GroupByHoles> rows = new List<GroupByHoles>();

            var holes = reshapedData.Where(t => t.ErrorType == ErrorStatus.ToString()).Where(i => i.Ignore == false).Select(h => h.GroupedHoles);

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

        public async Task<bool> HoleExist(List<ReshapedDataToEdit> reshapedData, string holeID, DrillholeMessageStatus ErrorStatus)
        {
            bool bCheck = false;

            var holes = reshapedData.Where(t => t.ErrorType == ErrorStatus.ToString()).Where(i => i.Ignore == false).Select(h => h.GroupedHoles);

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

        public async Task<GroupByHoles>ReturnHole(List<ReshapedDataToEdit> reshapedData, string holeID, DrillholeMessageStatus ErrorStatus)
        {
           
            var holes = reshapedData.Where(t => t.ErrorType == ErrorStatus.ToString()).Where(i => i.Ignore == false).Select(h => h.GroupedHoles);

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

    }

}

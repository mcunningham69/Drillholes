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


        public virtual async void ReshapeMessages()
        {

            foreach(var test in ShowTestMessages)
            {
                switch (test.testType)
                {
                    case DrillholeConstants.IsEmptyOrNull:
                        foreach(var message in test.testMessage)
                        {
                            if (message.validationTest == DrillholeConstants.checkHole)
                            {
                                ReformatResults(message, DrillholeConstants.IsEmptyOrNull, DrillholeConstants.checkHole);
                            }
                            else if (message.validationTest == DrillholeConstants.checkX)
                            {
                                ReformatResults(message, DrillholeConstants.IsEmptyOrNull, DrillholeConstants.checkX);
                            }
                            else if (message.validationTest == DrillholeConstants.checkY)
                            {
                                ReformatResults(message, DrillholeConstants.IsEmptyOrNull, DrillholeConstants.checkY);
                            }
                            else if (message.validationTest == DrillholeConstants.checkZ)
                            {
                                ReformatResults(message, DrillholeConstants.IsEmptyOrNull, DrillholeConstants.checkZ);
                            }
                            else if (message.validationTest == DrillholeConstants.checkTD)
                            {
                                ReformatResults(message, DrillholeConstants.IsEmptyOrNull, DrillholeConstants.checkTD);
                            }
                            else if (message.validationTest == DrillholeConstants.checkAzi)
                            {
                                ReformatResults(message, DrillholeConstants.IsEmptyOrNull, DrillholeConstants.checkAzi);
                            }
                            else if (message.validationTest == DrillholeConstants.checkDip)
                            {
                                ReformatResults(message, DrillholeConstants.IsEmptyOrNull, DrillholeConstants.checkDip);
                            }
                        }
                        break;

                    case DrillholeConstants.IsNumeric:
                        foreach (var message in test.testMessage)
                        {
                            if (message.validationTest == DrillholeConstants.checkX)
                            {
                                ReformatResults(message, DrillholeConstants.IsNumeric, DrillholeConstants.checkX);
                            }
                            else if (message.validationTest == DrillholeConstants.checkY)
                            {
                                ReformatResults(message, DrillholeConstants.IsNumeric, DrillholeConstants.checkY);
                            }
                            else if (message.validationTest == DrillholeConstants.checkZ)
                            {
                                ReformatResults(message, DrillholeConstants.IsNumeric, DrillholeConstants.checkZ);
                            }
                            else if (message.validationTest == DrillholeConstants.checkTD)
                            {
                                ReformatResults(message, DrillholeConstants.IsNumeric, DrillholeConstants.checkTD);
                            }
                            else if (message.validationTest == DrillholeConstants.checkAzi)
                            {
                                ReformatResults(message, DrillholeConstants.IsNumeric, DrillholeConstants.checkAzi);
                            }
                            else if (message.validationTest == DrillholeConstants.checkDip)
                            {
                                ReformatResults(message, DrillholeConstants.IsNumeric, DrillholeConstants.checkDip);
                            }
                        }

                        break;

                    case DrillholeConstants.Duplicates:
                        foreach (var message in test.testMessage)
                        {
                            ReformatResults(message, DrillholeConstants.Duplicates, DrillholeConstants.checkHole);
                        }
                        break;

                    case DrillholeConstants.HoleLength:
                        foreach (var message in test.testMessage)
                        {
                            ReformatResults(message, DrillholeConstants.HoleLength, DrillholeConstants.checkTD);
                        }
                        break;

                    case DrillholeConstants.ZeroCoordinate:
                        foreach (var message in test.testMessage)
                        {
                            ReformatResults(message, DrillholeConstants.ZeroCoordinate, DrillholeConstants.checkCoord);
                        }
                        break;

                    case DrillholeConstants.SurveyRange:
                        foreach (var message in test.testMessage)
                        {
                            if (message.validationTest == DrillholeConstants.checkDip)
                            {
                                ReformatResults(message, DrillholeConstants.SurveyRange, DrillholeConstants.checkDip);
                            }
                            else if (message.validationTest == DrillholeConstants.checkAzi)
                            {
                                ReformatResults(message, DrillholeConstants.SurveyRange, DrillholeConstants.checkAzi);
                            }
                        }
                        break;


                }
            }

            GroupByTable groupByTable = new GroupByTable() { TableType = "Collar"};

            List<ReshapedDataToEdit> reshapedData = new List<ReshapedDataToEdit>();

            reshapedData.Add(new ReshapedDataToEdit { ErrorType = DrillholeMessageStatus.Error.ToString() });
            reshapedData.Add(new ReshapedDataToEdit { ErrorType = DrillholeMessageStatus.Warning.ToString() });
            reshapedData.Add(new ReshapedDataToEdit { ErrorType = DrillholeMessageStatus.Valid.ToString() });

        }

        public virtual async void ReformatResults(ValidationMessage message, string _TestType, string validation)
        {
            List<ReshapedDataToEdit> reshapedData = new List<ReshapedDataToEdit>();

            List<DrillholeMessageStatus> statusMessages = new List<DrillholeMessageStatus>();
            statusMessages.Add(DrillholeMessageStatus.Error);
            statusMessages.Add(DrillholeMessageStatus.Warning);
            statusMessages.Add(DrillholeMessageStatus.Valid);

            var holeName = importCollarFields.Where(f => f.columnImportName == DrillholeConstants.holeIDName).Select(s => s.columnHeader).FirstOrDefault();

            //get fields to query XML
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

            if (surveyType == DrillholeSurveyType.collarsurvey)
            {
                fields.Add(importCollarFields.Where(o => o.columnImportName == DrillholeConstants.azimuthName).Where(m => m.genericType == false)
                    .Select(s => s.columnHeader).FirstOrDefault());
                fields.Add(importCollarFields.Where(o => o.columnImportName == DrillholeConstants.dipName).Where(m => m.genericType == false)
                    .Select(s => s.columnHeader).FirstOrDefault());
            }



            foreach (var status in statusMessages)
            {
                List<int> _status = message.ValidationStatus.Where(e => e.ErrorType == status).Select(p => p.id).ToList();
                List<string> tooltips = message.ValidationStatus.Where(e => e.ErrorType == status).Select(p => p.Description).ToList();

                RowsToEdit rowToEdit = new RowsToEdit() { Ignore = false, ErrorType = status };
                List<RowsToEdit> rows = new List<RowsToEdit>();
                rows.Add(rowToEdit);

                GroupByTestField testField = new GroupByTestField() { Ignore = false, TableData = rows, TestField = validation };
                List<GroupByTestField> testFields = new List<GroupByTestField>();
                testFields.Add(testField);

                GroupByTest test = new GroupByTest() { Ignore = false, TestFields = testFields, MainTest = _TestType };
                List<GroupByTest> groupedTests = new List<GroupByTest>();
                groupedTests.Add(test);

                GroupByTable table = new GroupByTable() { Ignore = false, TableType = "Collar", GroupedTests = groupedTests }; ;
                List<GroupByTable> groupedTables = new List<GroupByTable>();
                groupedTables.Add(table);

                GroupByHoles  hole = new GroupByHoles() { Ignore = false, GroupedTables = groupedTables };
                List<GroupByHoles> groupedHoles = new List<GroupByHoles>();
                groupedHoles.Add(hole);

                ReshapedDataToEdit _edits = new ReshapedDataToEdit() { Count = 0, ErrorType = status.ToString(), Ignore = false, GroupedHoles = groupedHoles };
                reshapedData.Add(_edits);

                if (_status.Count > 0)
                {

                }
                else
                {
                    _edits.Ignore = true;
                    hole.Ignore = true;
                    table.Ignore = true;
                    test.Ignore = true;
                    test.MainTest = "";
                    testField.TestField = "";
                    testField.Ignore = true;
                    rowToEdit.Ignore = true;


                }


            }


        }


    }

}

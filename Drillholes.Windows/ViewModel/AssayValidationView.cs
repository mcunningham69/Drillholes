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
    public class AssayValidationView : CollarValidationView
    {
        public AssayValidationService _assayValidationService;
        public IAssayValidation _assayValidateValues;

        public ImportTableFields importAssayFields { get; set; }

        private XElement _xmlAssayData;
        public XElement xmlAssayData
        {
            get
            {
                return this._xmlAssayData;
            }
            set
            {
                this._xmlAssayData = value;
                OnPropertyChanged("xmlAssayData");
            }
        }

        public AssayValidationView(DrillholeTableType _tableType, XElement _xmlAssayData) : base(_tableType, _xmlAssayData)
        {
            tableLabel = (_tableType.ToString() + " Table").ToUpper();

            xmlAssayData = _xmlAssayData;

            _assayValidateValues = new AssayValidation();

            _assayValidationService = new AssayValidationService(_assayValidateValues);
        }

        #region Messages
        public override async Task<bool> ValidateAllTables(bool editData)
        {
            ValidationDelegate mTables = null;
            mTables += CheckForEmptyFields;
            mTables += CheckForDuplicates;
            mTables += CheckNumericFields;
            mTables += CheckMaxDepth;
            mTables += CheckCollarsAssays;
            mTables += CheckForNegativeIntervals;
            mTables += CheckForMissingIntervals;
            mTables += CheckForOverlappingIntervals;
            mTables += CheckForZeroGrades;

            return await mTables(editData);
        }

        public override void InitialiseMapping()
        {
            if (_assayValidateValues == null)
                _assayValidateValues = new AssayValidation();

            var config = new MapperConfiguration(cfg => { cfg.CreateMap<ValidationAssayDto, ValidationAssay>(); });

            mapper = config.CreateMapper();

            var source = new ValidationAssayDto();

            var dest = mapper.Map<ValidationAssayDto, ValidationAssay>(source);

            _assayValidationService = new AssayValidationService(_assayValidateValues);

        }

       
        public override async Task<bool> CheckForEmptyFields(bool editData)
        {
            if (mapper == null)
                InitialiseMapping();

            List<ValidationMessage> assayFieldTest = new List<ValidationMessage>();

            assayFieldTest.Add(new ValidationMessage
            {
                verified = true,
                validationTest = DrillholeConstants.checkHole,
                count = 0,
                validationMessages = new List<string>(),
                tableField = importAssayFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Single()
            });

            assayFieldTest.Add(new ValidationMessage
            {
                verified = true,
                validationTest = DrillholeConstants.checkFrom,
                count = 0,
                validationMessages = new List<string>(),
                tableField = importAssayFields.Where(o => o.columnImportName == DrillholeConstants.distFromName).Where(m => m.genericType == false).Single()
            });

            assayFieldTest.Add(new ValidationMessage
            {
                verified = true,
                validationTest = DrillholeConstants.checkTo,
                count = 0,
                validationMessages = new List<string>(),
                tableField = importAssayFields.Where(o => o.columnImportName == DrillholeConstants.distToName).Where(m => m.genericType == false).Single()
            });

            //check for grade
            if (importAssayFields.Where(o => o.columnImportAs == "Grade").Count() > 0)
            {
                var gradeFields = importAssayFields.Where(o => o.columnImportAs == "Grade").ToList();

                foreach (var field in gradeFields)
                {
                    assayFieldTest.Add(new ValidationMessage
                    {
                        verified = true,
                        count = 0,
                        validationTest = DrillholeConstants.checkGrade,
                        validationMessages = new List<string>(),
                        tableField = field

                    });
                }
            }

            //check for numeric but not including grade
            if (importAssayFields.Where(g => g.columnImportAs == "Numeric").Where(p => p.genericType == true).Count() > 0)
            {
                var isNumericField = importAssayFields.Where(g => g.columnImportAs == "Numeric").Where(p => p.genericType == true).ToList();

                foreach (var field in isNumericField)
                {
                    assayFieldTest.Add(new ValidationMessage
                    {
                        verified = true,
                        count = 0,
                        validationTest = DrillholeConstants.checkNumeric,
                        validationMessages = new List<string>(),
                        tableField = field

                    });

                }

            }


            ValidationMessages assayTests = new ValidationMessages { testType = DrillholeConstants.IsEmptyOrNull, testMessage = assayFieldTest };

            var validationCheck = await _assayValidationService.CheckIsEmpty(mapper, assayTests, xmlAssayData);

            DisplayMessages.DisplayResults.Add(validationCheck.testMessages);

            return true;
        }
        public override async Task<bool> CheckForDuplicates(bool editData)
        {
            if (mapper == null)
                InitialiseMapping();

            ImportTableField holeField = importAssayFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Single();
            ImportTableField fromField = importAssayFields.Where(o => o.columnImportName == DrillholeConstants.distFromName).Where(m => m.genericType == false).Single();

            List<ImportTableField> tempFields = new List<ImportTableField>();
            tempFields.Add(holeField);
            tempFields.Add(fromField);

            List<ValidationMessage> assayFieldTest = new List<ValidationMessage>();
            assayFieldTest.Add(new ValidationMessage
            {
                verified = true,
                validationTest = DrillholeConstants.checkFrom,
                count = 0,
                validationMessages = new List<string>(),
                tableFields = tempFields
            });

            ValidationMessages assayTests = new ValidationMessages { testType = DrillholeConstants.Duplicates, testMessage = assayFieldTest };

            var validationCheck = await _assayValidationService.CheckDuplicates(mapper, assayTests, xmlAssayData);

            DisplayMessages.DisplayResults.Add(validationCheck.testMessages);

            return true;
        }
        public async Task<bool> CheckCollarsAssays(bool editData)
        {
            if (mapper == null)
                InitialiseMapping();

            List<XElement> drillholeTableData = new List<XElement>();
            drillholeTableData.Add(xmlCollarData);
            drillholeTableData.Add(xmlAssayData);

            ImportTableField holeField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Single();
            ImportTableField assayHoleField = importAssayFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Single();

            List<ImportTableField> tempFields = new List<ImportTableField>();
            tempFields.Add(holeField);
            tempFields.Add(assayHoleField);

            List<ValidationMessage> assayFieldTest = new List<ValidationMessage>();

            //Check if azimuth and dip fields set
            assayFieldTest.Add(new ValidationMessage
            {
                verified = true,
                validationTest = DrillholeConstants.checkHole,
                count = 0,
                validationMessages = new List<string>(),
                tableFields = tempFields
            });

            ValidationMessages assayTests = new ValidationMessages { testType = DrillholeConstants.MissingCollar, testMessage = assayFieldTest };

            var validationCheck = await _assayValidationService.CheckMissingCollars(mapper, assayTests, drillholeTableData);

            DisplayMessages.DisplayResults.Add(validationCheck.testMessages);

            return true;
        }
        public override async Task<bool> CheckNumericFields(bool editData)
        {
            if (mapper == null)
                InitialiseMapping();

            List<ValidationMessage> assayFieldTest = new List<ValidationMessage>();
            assayFieldTest.Add(new ValidationMessage
            {
                verified = true,
                validationTest = DrillholeConstants.checkHole,
                count = 0,
                validationMessages = new List<string>(),
                tableField = importAssayFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Single()
            });

            assayFieldTest.Add(new ValidationMessage
            {
                verified = true,
                validationTest = DrillholeConstants.checkFrom,
                count = 0,
                validationMessages = new List<string>(),
                tableField = importAssayFields.Where(o => o.columnImportName == DrillholeConstants.distFromName).Where(m => m.genericType == false).Single()
            });

            assayFieldTest.Add(new ValidationMessage
            {
                verified = true,
                validationTest = DrillholeConstants.checkTo,
                count = 0,
                validationMessages = new List<string>(),
                tableField = importAssayFields.Where(o => o.columnImportName == DrillholeConstants.distToName).Where(m => m.genericType == false).Single()
            });

            //check for grade
            //check for grade
            if (importAssayFields.Where(o => o.columnImportAs == "Grade").Count() > 0)
            {
                var gradeFields = importAssayFields.Where(o => o.columnImportAs == "Grade").ToList();

                foreach (var field in gradeFields)
                {
                    assayFieldTest.Add(new ValidationMessage
                    {
                        verified = true,
                        count = 0,
                        validationTest = DrillholeConstants.checkGrade,
                        validationMessages = new List<string>(),
                        tableField = field

                    });
                }
            }

            //check for numeric but not including grade
            if (importAssayFields.Where(g => g.columnImportAs == "Numeric").Where(p => p.genericType == true).Count() > 0)
            {
                var isNumericField = importAssayFields.Where(g => g.columnImportAs == "Numeric").Where(p => p.genericType == true).ToList();

                foreach (var field in isNumericField)
                {
                    assayFieldTest.Add(new ValidationMessage
                    {
                        verified = true,
                        count = 0,
                        validationTest = DrillholeConstants.checkNumeric,
                        validationMessages = new List<string>(),
                        tableField = field

                    });

                }

            }

            ValidationMessages assayTests = new ValidationMessages { testType = DrillholeConstants.IsNumeric, testMessage = assayFieldTest };

            var validationCheck = await _assayValidationService.CheckForNumeric(mapper, assayTests, xmlAssayData);

            DisplayMessages.DisplayResults.Add(validationCheck.testMessages);

            return true;
        }
        public override async Task<bool> CheckMaxDepth(bool editData)
        {
            if (mapper == null)
                InitialiseMapping();

            List<XElement> drillholeTableData = new List<XElement>();
            drillholeTableData.Add(xmlCollarData);
            drillholeTableData.Add(xmlAssayData);

            ImportTableField holeField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Single();
            ImportTableField tdField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.maxName).Where(m => m.genericType == false).Single();
            ImportTableField assayHoleField = importAssayFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Single();
            ImportTableField toFields = importAssayFields.Where(o => o.columnImportName == DrillholeConstants.distToName).Where(m => m.genericType == false).Single();

            List<ImportTableField> tempFields = new List<ImportTableField>();
            tempFields.Add(holeField);
            tempFields.Add(tdField);
            tempFields.Add(assayHoleField);
            tempFields.Add(toFields);

            List<ValidationMessage> assayFieldTest = new List<ValidationMessage>();

            //Check if azimuth and dip fields set
            assayFieldTest.Add(new ValidationMessage
            {
                verified = true,
                validationTest = DrillholeConstants.checkTD,
                count = 0,
                validationMessages = new List<string>(),
                tableFields = tempFields
            });

            ValidationMessages assayTests = new ValidationMessages { testType = DrillholeConstants.HoleLength, testMessage = assayFieldTest };

            var validationCheck = await _assayValidationService.CheckMaxDepth(mapper, assayTests, drillholeTableData);

            DisplayMessages.DisplayResults.Add(validationCheck.testMessages);

            return true;
        }

        public async virtual Task<bool> CheckForNegativeIntervals(bool editData)
        {
            if (mapper == null)
                InitialiseMapping();

            ImportTableField assayHoleField = importAssayFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Single();
            ImportTableField assayFromField = importAssayFields.Where(o => o.columnImportName == DrillholeConstants.distFromName).Where(m => m.genericType == false).Single();
            ImportTableField assayToField = importAssayFields.Where(o => o.columnImportName == DrillholeConstants.distToName).Where(m => m.genericType == false).Single();

            List<ImportTableField> tempFields = new List<ImportTableField>();
            tempFields.Add(assayHoleField);
            tempFields.Add(assayFromField);
            tempFields.Add(assayToField);

            List<ValidationMessage> assayFieldTest = new List<ValidationMessage>();
            assayFieldTest.Add(new ValidationMessage
            {
                verified = true,
                validationTest = DrillholeConstants.checkFromTo,
                count = 0,
                validationMessages = new List<string>(),
                tableFields = tempFields
            });

            ValidationMessages assayTests = new ValidationMessages { testType = DrillholeConstants.NegativeOrZeroInterval, testMessage = assayFieldTest };

            var validationCheck = await _assayValidationService.CheckNegativeIntervals(mapper, assayTests, xmlAssayData);

            DisplayMessages.DisplayResults.Add(validationCheck.testMessages);

            return true;
        }

        public async virtual Task<bool> CheckForMissingIntervals(bool editData)
        {
            if (mapper == null)
                InitialiseMapping();

            List<XElement> drillholeValues = new List<XElement>();
            drillholeValues.Add(xmlCollarData);
            drillholeValues.Add(xmlAssayData);

            ImportTableField collarHoleField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Single();
            ImportTableField collarTdField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.maxName).Where(m => m.genericType == false).Single();
            ImportTableField assayHoleField = importAssayFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Single();
            ImportTableField assayFromField = importAssayFields.Where(o => o.columnImportName == DrillholeConstants.distFromName).Where(m => m.genericType == false).Single();
            ImportTableField assayToField = importAssayFields.Where(o => o.columnImportName == DrillholeConstants.distToName).Where(m => m.genericType == false).Single();

            List<ImportTableField> tempFields = new List<ImportTableField>();
            tempFields.Add(collarHoleField);
            tempFields.Add(collarTdField);
            tempFields.Add(assayHoleField);
            tempFields.Add(assayFromField);
            tempFields.Add(assayToField);

            List<ValidationMessage> assayFieldTest = new List<ValidationMessage>();
            assayFieldTest.Add(new ValidationMessage
            {
                verified = true,
                validationTest = DrillholeConstants.checkFromTo,
                count = 0,
                validationMessages = new List<string>(),
                tableFields = tempFields
            });

            ValidationMessages assayTests = new ValidationMessages { testType = DrillholeConstants.MissingInterval, testMessage = assayFieldTest };

            var validationCheck = await _assayValidationService.CheckMissingIntervals(mapper, assayTests, drillholeValues);

            DisplayMessages.DisplayResults.Add(validationCheck.testMessages);

            return true;
        }

        public async virtual Task<bool> CheckForOverlappingIntervals(bool editData)
        {
            if (mapper == null)
                InitialiseMapping();

            ImportTableField assayHoleField = importAssayFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Single();
            ImportTableField assayFromField = importAssayFields.Where(o => o.columnImportName == DrillholeConstants.distFromName).Where(m => m.genericType == false).Single();
            ImportTableField assayToField = importAssayFields.Where(o => o.columnImportName == DrillholeConstants.distToName).Where(m => m.genericType == false).Single();

            List<ImportTableField> tempFields = new List<ImportTableField>();
            tempFields.Add(assayHoleField);
            tempFields.Add(assayFromField);
            tempFields.Add(assayToField);

            List<ValidationMessage> assayFieldTest = new List<ValidationMessage>();
            assayFieldTest.Add(new ValidationMessage
            {
                verified = true,
                validationTest = DrillholeConstants.checkFromTo,
                count = 0,
                validationMessages = new List<string>(),
                tableFields = tempFields
            });

            ValidationMessages assayTests = new ValidationMessages { testType = DrillholeConstants.OverlappingInterval, testMessage = assayFieldTest };

            var validationCheck = await _assayValidationService.CheckOverlappingIntervals(mapper, assayTests, xmlAssayData);

            DisplayMessages.DisplayResults.Add(validationCheck.testMessages);

            return true;
        }

        private async Task<bool> CheckForZeroGrades(bool editData)
        {
            if (mapper == null)
                InitialiseMapping();

            List<ValidationMessage> assayFieldTest = new List<ValidationMessage>();

            List<ImportTableField> tempFields = new List<ImportTableField>();

            //check for grade
            if (importAssayFields.Where(o => o.columnImportAs == "Grade").Count() > 0)
            {
                ImportTableField assayHoleField = importAssayFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Single();
                tempFields.Add(assayHoleField);

                //import SampleID if it exists
                ImportTableField SampleIDField = importAssayFields.Where(o => o.columnImportName == DrillholeConstants.sampleName).Where(m => m.genericType == true).FirstOrDefault();

                if (SampleIDField == null)
                    SampleIDField = new ImportTableField() { columnImportName = DrillholeConstants.sampleName };

                tempFields.Add(SampleIDField);


                var gradeFields = importAssayFields.Where(o => o.columnImportAs == "Grade").ToList();

                foreach (var field in gradeFields)
                {
                    tempFields.Add(field);
                }


                assayFieldTest.Add(new ValidationMessage
                {
                    verified = true,
                    count = 0,
                    validationTest = DrillholeConstants.checkGrade,
                    validationMessages = new List<string>(),
                    tableFields = tempFields

                });

                ValidationMessages assayTests = new ValidationMessages { testType = DrillholeConstants.ZeroGrade, testMessage = assayFieldTest };

                var validationCheck = await _assayValidationService.CheckOverlappingIntervals(mapper, assayTests, xmlAssayData);

                DisplayMessages.DisplayResults.Add(validationCheck.testMessages);
            }
            return true;
        }

        #endregion

        public override async void ReshapeMessages(DrillholeTableType tableType)
        {
            List<string> fields = await ReturnFieldnamesForXmlQuery();
            List<DrillholeMessageStatus> status = await ReturnStatusAlerts(); //set up three alert states

            var assayvalues = xmlAssayData.Elements();

            foreach (var test in ShowTestMessages)  //get messages after running validation above
            {
                switch (test.testType) //select on the different testtype
                {
                    case DrillholeConstants.IsEmptyOrNull:

                        foreach (var message in test.testMessage)
                        {
                            if (message.validationTest == DrillholeConstants.checkHole)  //specific field being checked
                            {
                                await ReformatResults(message, DrillholeConstants.IsEmptyOrNull, DrillholeConstants.checkHole, fields, status, tableType, assayvalues);
                            }
                            else if (message.validationTest == DrillholeConstants.checkFrom)
                            {
                                await ReformatResults(message, DrillholeConstants.IsEmptyOrNull, DrillholeConstants.checkFrom, fields, status, tableType, assayvalues);
                            }

                            else if (message.validationTest == DrillholeConstants.checkTo)
                            {
                                await ReformatResults(message, DrillholeConstants.IsEmptyOrNull, DrillholeConstants.checkTo, fields, status, tableType, assayvalues);
                            }
                           
                        }
                        break;

                    case DrillholeConstants.IsNumeric:
                        foreach (var message in test.testMessage)
                        {
                            if (message.validationTest == DrillholeConstants.checkFrom)
                            {
                                await ReformatResults(message, DrillholeConstants.IsNumeric, DrillholeConstants.checkFrom, fields, status, tableType, assayvalues);
                            }
                            else if (message.validationTest == DrillholeConstants.checkTo)
                            {
                                await ReformatResults(message, DrillholeConstants.IsNumeric, DrillholeConstants.checkTo, fields, status, tableType, assayvalues);
                            }
                            else if (message.validationTest == DrillholeConstants.checkGrade)
                            {
                                await ReformatResults(message, DrillholeConstants.IsNumeric, DrillholeConstants.checkGrade, fields, status, tableType, assayvalues);
                            }
                        }

                        break;

                    case DrillholeConstants.Duplicates:
                        foreach (var message in test.testMessage)
                        {
                            await ReformatResults(message, DrillholeConstants.Duplicates, DrillholeConstants.checkFrom, fields, status, tableType, assayvalues);
                        }
                        break;

                    case DrillholeConstants.HoleLength:
                        foreach (var message in test.testMessage)
                        {
                            await ReformatResults(message, DrillholeConstants.HoleLength, DrillholeConstants.checkTD, fields, status, tableType, assayvalues);
                        }
                        break;

                    case DrillholeConstants.NegativeOrZeroInterval:
                        foreach (var message in test.testMessage)
                        {
                            await ReformatResults(message, DrillholeConstants.NegativeOrZeroInterval, DrillholeConstants.checkFromTo, fields, status, tableType, assayvalues);
                        }
                        break;

                    case DrillholeConstants.MissingInterval:
                        foreach (var message in test.testMessage)
                        {

                            await ReformatResults(message, DrillholeConstants.MissingInterval, DrillholeConstants.checkFromTo, fields, status, tableType, assayvalues);

                        }
                        break;
                    case DrillholeConstants.MissingCollar:  //TEST
                        foreach (var message in test.testMessage)
                        {
                            if (message.validationTest == DrillholeConstants.checkHole)
                            {
                                await ReformatResults(message, DrillholeConstants.MissingCollar, DrillholeConstants.checkHole, fields, status, tableType, assayvalues);
                            }

                        }
                        break;
                    case DrillholeConstants.OverlappingInterval:
                        foreach (var message in test.testMessage)
                        {
                            if (message.validationTest == DrillholeConstants.checkFromTo)
                            {
                                await ReformatResults(message, DrillholeConstants.OverlappingInterval, DrillholeConstants.checkFromTo, fields, status, tableType, assayvalues);
                            }

                        }
                        break;
                    //case DrillholeConstants.ZeroGrade:
                    //    foreach (var message in test.testMessage)
                    //    {
                    //        if (importAssayFields.Where(o => o.columnImportAs == "Grade").Count() > 0)
                    //        {
                    //            var gradeFields = importAssayFields.Where(o => o.columnImportAs == "Grade").ToList();

                    //            foreach (var field in gradeFields)
                    //            {
                    //                if (message.validationTest == DrillholeConstants.checkGrade)
                    //                {
                    //                    await ReformatResults(message, DrillholeConstants.ZeroGrade, DrillholeConstants.checkFromTo, fields, status, tableType, surveyvalues); //TODO
                    //                }
                    //            }
                    //        }

                                

                    //    }
                    //    break;
                }
            }
        }

        public override async Task<bool> ReformatResults(ValidationMessage message, string _TestType, string validation, List<string> fields, List<DrillholeMessageStatus> statusMessages, DrillholeTableType tableType,
IEnumerable<XElement> assayValues)
        {
            foreach (var status in statusMessages)  //iterate three times for each error type
            {
                var count = message.ValidationStatus.Where(e => e.ErrorType == status).Count();  //for checking each status for messages

                int counter = 0; //use this for collar id

                if (count > 0)
                {
                    string holeIDName = importAssayFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Select(n => n.columnHeader).FirstOrDefault(); //get hole name for querying XML


                    //setup outside the foreach hole loop below as it is the last item to add to the ReshapedToEdit list (which is data bound to the XAML DrillholeEdits form
                    List<GroupByHoles> groupedHoles = new List<GroupByHoles>();

                    List<string> SelectedHoles = new List<string>();
                    List<int> ids = new List<int>();

                         ids = message.ValidationStatus.Where(e => e.ErrorType == status).Select(a => a.id).ToList(); //get IDs of all messages

                        foreach (int id in ids) //get the holes for each record
                        {
                            var value = assayValues.Where(a => a.Attribute("ID").Value == id.ToString()).Select(h => h.Element(holeIDName).Value).FirstOrDefault();

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

                            var value = assayValues.Where(a => a.Element(holeIDName).Value == assayHole).Select(v => v.Attribute("ID").Value).FirstOrDefault();
                            string tooltip = message.ValidationStatus.Where(e => e.ErrorType == status && e.id == Convert.ToInt32(value)).Select(p => p.Description).FirstOrDefault();


                            //return records within hole

                            // List<string> survIDs = null;

                            //if(status == DrillholeMessageStatus.Valid)
                            //{
                            //    if (_TestType == DrillholeConstants.MissingInterval || _TestType == DrillholeConstants.NegativeOrZeroInterval || _TestType == DrillholeConstants.OverlappingInterval)
                            //    {
                            //        var survIDs = message.ValidationStatus.Where(e => e.ErrorType == status && e.holeID == assayHole).Select(a => a.id).ToList(); //get IDs of all messages

                            //        foreach (var id in ids)
                            //        {
                            //            rows.Add(new RowsToEdit
                            //            {
                            //                id_col = counter,
                            //                id_ass = id,
                            //                testType = _TestType,
                            //                validationTest = validation,
                            //                Description = tooltips[counter]
                            //            });

                            //        }
                            //    }
                            //}

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

                            var survIDs = assayValues.Where(h => h.Element(fields[0]).Value == assayHole).Select(v => v.Attribute("ID").Value).ToList(); //get the survey IDs for hole to check which records have a message

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
            fields.Add(importAssayFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false) //get name of hole field for querying XML
                .Select(s => s.columnHeader).FirstOrDefault());
            fields.Add(importAssayFields.Where(o => o.columnImportName == DrillholeConstants.distFromName).Where(m => m.genericType == false) //get other feilds which are required as mandatory, i.e. XYZ and depth
                .Select(s => s.columnHeader).FirstOrDefault());
            fields.Add(importAssayFields.Where(o => o.columnImportName == DrillholeConstants.distToName).Where(m => m.genericType == false)
                .Select(s => s.columnHeader).FirstOrDefault());

            //check for grade
            if (importAssayFields.Where(o => o.columnImportAs == "Grade").Count() > 0)
            {
                var gradeFields = importAssayFields.Where(o => o.columnImportAs == "Grade").ToList();

                foreach (var field in gradeFields)
                {
                    fields.Add(field.columnHeader);
                }
            }

            return fields;
        }

    }
}

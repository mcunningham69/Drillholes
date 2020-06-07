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


            ValidationMessages assayTests = new ValidationMessages { testType = DrillholeConstants.IsEmptyOrNull, testMessage = assayFieldTest};

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

            ValidationMessages assayTests = new ValidationMessages { testType = DrillholeConstants.IsNumeric, testMessage = assayFieldTest};

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

            ValidationMessages assayTests = new ValidationMessages { testType = DrillholeConstants.HoleLength, testMessage = assayFieldTest};

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

            ValidationMessages assayTests = new ValidationMessages { testType = DrillholeConstants.MissingInterval, testMessage = assayFieldTest};

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
    }
}

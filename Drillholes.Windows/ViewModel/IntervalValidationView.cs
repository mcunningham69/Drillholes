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

        public override async Task<bool> ValidateAllTables()
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

            return await mTables();
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


        public override async Task<bool> CheckForEmptyFields()
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
        public override async Task<bool> CheckForDuplicates()
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
        public override async Task<bool> CheckNumericFields()
        {
            if (mapper == null)
                InitialiseMapping();

            List<ValidationMessage> intervalFieldTest = new List<ValidationMessage>();
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
        public override async Task<bool> CheckMaxDepth()
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
        public override async Task<bool> CheckForNegativeIntervals()
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
        public override async Task<bool> CheckForMissingIntervals()
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
        public override async Task<bool> CheckForOverlappingIntervals()
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

        public async Task<bool> CheckCollarsIntervals()
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
    }
}

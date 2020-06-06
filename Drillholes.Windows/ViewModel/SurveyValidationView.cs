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

            _surveyValidateValues = new SurveyValidation();

            _surveyValidationService = new SurveyValidationService(_surveyValidateValues);
        }

        public override async Task<bool> ValidateAllTables()
        {
            ValidationDelegate mTables = null;
            mTables += CheckForEmptyFields;
            mTables += CheckForDuplicates;
            mTables += CheckNumericFields;
            mTables += CheckMaxDepth;
            mTables += CheckRange;
            mTables += CheckSurveyDistance;
            mTables += CheckCollarsSurveys;

            return await mTables();

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

        public async override Task<bool> CheckForEmptyFields()
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
                tableField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Single()
            });

            surveyFieldTest.Add(new ValidationMessage
            {
                verified = true,
                validationTest = DrillholeConstants.checkDist,
                count = 0,
                validationMessages = new List<string>(),
                tableField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.distName).Where(m => m.genericType == false).Single()
            });

            surveyFieldTest.Add(new ValidationMessage
            {
                verified = true,
                validationTest = DrillholeConstants.checkDip,
                count = 0,
                validationMessages = new List<string>(),
                tableField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.dipName).Where(m => m.genericType == false).Single()
            });

            surveyFieldTest.Add(new ValidationMessage
            {
                verified = true,
                validationTest = DrillholeConstants.checkAzi,
                count = 0,
                validationMessages = new List<string>(),
                tableField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.azimuthName).Where(m => m.genericType == false).Single()
            });

            var isNumericField = importCollarFields.Where(o => o.fieldType == "Double").Where(p => p.genericType == true).ToList();

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
        public async override Task<bool> CheckForDuplicates()
        {
            if (mapper == null)
                InitialiseMapping();

            ImportTableField holeField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Single();
            ImportTableField distanceField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.distName).Where(m => m.genericType == false).Single();

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
        private async Task<bool> CheckSurveyDistance()
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
        public async override Task<bool> CheckNumericFields()
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
                tableField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.distName).Where(m => m.genericType == false).Single()
            });

            surveyFieldTest.Add(new ValidationMessage
            {
                verified = true,
                validationTest = DrillholeConstants.checkDip,
                count = 0,
                validationMessages = new List<string>(),
                tableField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.dipName).Where(m => m.genericType == false).Single()
            });

            surveyFieldTest.Add(new ValidationMessage
            {
                verified = true,
                validationTest = DrillholeConstants.checkAzi,
                count = 0,
                validationMessages = new List<string>(),
                tableField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.azimuthName).Where(m => m.genericType == false).Single()
            });

            var isNumericField = importCollarFields.Where(o => o.fieldType == "Double").Where(p => p.genericType == true).ToList();

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
        public async override Task<bool> CheckMaxDepth()
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

        public async override Task<bool> CheckRange()
        {
            return true;
        }

        private async Task<bool> CheckCollarsSurveys()
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

    }
}

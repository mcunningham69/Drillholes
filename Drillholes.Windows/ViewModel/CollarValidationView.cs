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

namespace Drillholes.Windows.ViewModel
{
    public class CollarValidationView : ViewEditModel
    {      

        public delegate Task<bool> ValidationDelegate();

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

        public virtual async Task<bool> ValidateAllTables()
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

            return await mTables();
        }

        public virtual void InitialiseMapping()
        {
            if (_validateValues == null)
                _validateValues = new CollarValidation(surveyType);

            var config = new MapperConfiguration(cfg => { cfg.CreateMap<ValidationCollarDto, ValidationCollar>(); });

            mapper = config.CreateMapper();

            var source = new ValidationCollarDto();

            var dest = mapper.Map< ValidationCollarDto, ValidationCollar> (source);

            _validationService = new CollarValidationService(_validateValues);

        }

        public virtual async Task<bool> CheckForEmptyFields()
        {
            if (mapper == null)
                InitialiseMapping();

            List<ValidationMessage> collarTest = new List<ValidationMessage>();
            List<ImportTableField> tempFields = new List<ImportTableField>();

            tempFields.Add(importCollarFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Single());

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
        public virtual async Task<bool> CheckNumericFields()
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

        public virtual async Task<bool> CheckForDuplicates()
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
        public virtual async Task<bool> CheckMaxDepth()
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
        public virtual async Task<bool> CheckZeroCoordinates()
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

        public virtual async Task<bool> CheckRange()
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
    }

}

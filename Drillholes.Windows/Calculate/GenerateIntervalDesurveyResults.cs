using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using Drillholes.Domain;
using Drillholes.Domain.Enum;
using Drillholes.Domain.DataObject;
using Drillholes.Domain.Services;
using Drillholes.Domain.Interfaces;
using Drillholes.Domain.DTO;
using Drillholes.XML;
using AutoMapper;
using System.Xml.Linq;
using System.Data;
using System.Windows.Controls;
using Microsoft.Office.Interop.Excel;
using Drillholes.Windows.ViewModel;

namespace Drillholes.Windows.Calculate
{
    public class GenerateIntervalDesurveyResults : ViewEditModel
    {
        private IntervalDesurveyServices _desurveyService;

        private IDesurveyDrillhole _desurveyTable;

        //TODO - show results
        public System.Data.DataTable dataGrid { get; set; }

        private IMapper intervalDesurvMapper;

        private ImportTableFields _tableFields;
        public ImportTableFields tableFields
        {
            get
            {
                return this._tableFields;
            }
            set
            {
                this._tableFields = value;
                OnPropertyChanged("tableFields");
            }
        }

        public XmlService _xmlService;
        public IDrillholeXML _xml;

        public bool savedSession { get; set; }
        public string sessionName { get; set; }
        public string projectLocation { get; set; }

        private string DesurveyTableXmlName { get; set; }

        public List<XElement> intervalXmlData { get; set; }

        public GenerateIntervalDesurveyResults(bool _savedSession, string _sessionName, string _projectLocation, ImportTableFields _fields,
            List<XElement> drillholeData)
        {
           
            //dataGrid = new System.Data.DataTable();

            savedSession = _savedSession;
            sessionName = _sessionName;
            projectLocation = _projectLocation;
            intervalXmlData = drillholeData;
            tableFields = _fields;

            XmlSetUP();

        }

        public async void XmlSetUP()
        {
            //create XML temp table
            if (_xml == null)
                _xml = new Drillholes.XML.XmlController();

            if (_xmlService == null)
                _xmlService = new XmlService(_xml);

            if (!savedSession)
            {
                DesurveyTableXmlName = XmlDefaultPath.GetFullPathAndFilename(DrillholeConstants.drillholeDesurv, "interval");
            }
            else
            {
                DesurveyTableXmlName = XmlDefaultPath.GetProjectPathAndFilename(DrillholeConstants.drillholeDesurv, "interval", sessionName, projectLocation) ;
            }
        }


        //TODO move out of here
     
        private async void InitialiseIntervalMapping()
        {
            _desurveyTable = new Drillholes.CreateDrillholes.CreateHolesByType();

            var config = new MapperConfiguration(cfg => { cfg.CreateMap<IntervalDesurveyDto, IntervalDesurveyObject>(); });

            intervalDesurvMapper = config.CreateMapper();
            var source = new IntervalDesurveyDto();

            var dest = intervalDesurvMapper.Map<IntervalDesurveyDto, IntervalDesurveyObject>(source);

            _desurveyService = new IntervalDesurveyServices(_desurveyTable);
        }
     

        public async void SetDataContext(DataGrid dataPreview)
        {

            if (dataGrid.Columns.Count > 0)
                dataPreview.DataContext = dataGrid;
        }

        public async Task<bool>GenerateIntervalDesurveyVertical(bool bToe, DrillholeDesurveyEnum surveyMethod)
        {
            if (intervalDesurvMapper == null)
                InitialiseIntervalMapping();

            //surveymethod has to be Tangential
            var intervalResults = await _desurveyService.IntervalVerticalHole(intervalDesurvMapper, surveyMethod, tableFields, bToe, intervalXmlData);

            return true;
        }

        public async Task<bool> GenerateAssayDesurveyFromCollarSurvey(bool bToe, DrillholeDesurveyEnum surveyMethod)
        {
            if (intervalDesurvMapper == null)
                InitialiseIntervalMapping();

            //surveymethod has to be Tangential
            var intervalResults = await _desurveyService.IntervalSurveyHole(intervalDesurvMapper, surveyMethod, tableFields, bToe, intervalXmlData);

            return true;
        }

        public async Task<bool> GenerateAssayDesurveyFromDownhole(bool bToe, DrillholeDesurveyEnum surveyMethod)
        {
            if (intervalDesurvMapper == null)
                InitialiseIntervalMapping();

            //surveymethod has to be Tangential
            var collarResults = await _desurveyService.IntervalDownhole(intervalDesurvMapper, surveyMethod, tableFields, bToe, intervalXmlData);

            return true;
        }
    }
}

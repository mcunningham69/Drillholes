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
    public class GenerateSurveyDesurveyResults : ViewEditModel
    {
        private SurveyDesurveyServices _desurveyService;

        private IDesurveyDrillhole _desurveyTable;

        //TODO - show results
        public System.Data.DataTable dataGrid { get; set; }

        private IMapper surveyDesurvMapper;

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

        public List<XElement> surveyXmlData { get; set; }

        public XmlService _xmlService;
        public IDrillholeXML _xml;

        public bool savedSession { get; set; }
        public string sessionName { get; set; }
        public string projectLocation { get; set; }

        private string DesurveyTableXmlName { get; set; }

        public GenerateSurveyDesurveyResults(bool _savedSession, string _sessionName, string _projectLocation, ImportTableFields _fields,
            List<XElement> drillholeData, List<XElement> _surveyXml)
        {

            //dataGrid = new System.Data.DataTable();

            savedSession = _savedSession;
            sessionName = _sessionName;
            projectLocation = _projectLocation;
            surveyXmlData = _surveyXml;
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
                DesurveyTableXmlName = XmlDefaultPath.GetFullPathAndFilename(DrillholeConstants.drillholeDesurv, "survey");
            }
            else
            {
                DesurveyTableXmlName = XmlDefaultPath.GetProjectPathAndFilename(DrillholeConstants.drillholeDesurv, "survey", sessionName, projectLocation);
            }
        }


        //TODO move out of here
   
        private async void InitialiseSurveyMapping()
        {
            _desurveyTable = new Drillholes.CreateDrillholes.CreateHolesByType();

            var config = new MapperConfiguration(cfg => { cfg.CreateMap<SurveyDesurveyDto, SurveyDesurveyObject>(); });

            surveyDesurvMapper = config.CreateMapper();
            var source = new SurveyDesurveyDto();

            var dest = surveyDesurvMapper.Map<SurveyDesurveyDto, SurveyDesurveyObject>(source);

            _desurveyService = new SurveyDesurveyServices(_desurveyTable);
        }
       

        public async void SetDataContext(DataGrid dataPreview)
        {

            if (dataGrid.Columns.Count > 0)
                dataPreview.DataContext = dataGrid;
        }

        public async Task<bool> GenerateSurvey(bool bToe, DrillholeDesurveyEnum surveyMethod)
        {
            if (surveyDesurvMapper == null)
                InitialiseSurveyMapping();

            //surveymethod has to be Tangential
            var surveyResults = await _desurveyService.SurveyDownhole(surveyDesurvMapper, surveyMethod, tableFields, bToe, surveyXmlData );

            return true;
        }

    }
}

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
    public class GenerateAssayDesurveyResults : ViewEditModel
    {
        private AssayDesurveyServices _desurveyService;

        private IDesurveyDrillhole _desurveyTable;

        //TODO - show results
        public System.Data.DataTable dataGrid { get; set; }

        private IMapper assayDesurvMapper;

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

        public List<XElement> assayXmlData { get; set; }

        public GenerateAssayDesurveyResults(bool _savedSession, string _sessionName, string _projectLocation, ImportTableFields _fields,
            List<XElement> drillholeData)
        {
           
            //dataGrid = new System.Data.DataTable();

            savedSession = _savedSession;
            sessionName = _sessionName;
            projectLocation = _projectLocation;
            assayXmlData = drillholeData;
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
                DesurveyTableXmlName = XmlDefaultPath.GetFullPathAndFilename(DrillholeConstants.drillholeDesurv, "assay");
            }
            else
            {
                DesurveyTableXmlName = XmlDefaultPath.GetProjectPathAndFilename(DrillholeConstants.drillholeDesurv, "assay", sessionName, projectLocation) ;
            }
        }


        //TODO move out of here
     
        private async void InitialiseAssayMapping()
        {
            _desurveyTable = new Drillholes.CreateDrillholes.CreateHolesByType();

            var config = new MapperConfiguration(cfg => { cfg.CreateMap<AssayDesurveyDto, AssayDesurveyObject>(); });

            assayDesurvMapper = config.CreateMapper();
            var source = new AssayDesurveyDto();

            var dest = assayDesurvMapper.Map<AssayDesurveyDto, AssayDesurveyObject>(source);

            _desurveyService = new AssayDesurveyServices(_desurveyTable);
        }
     

        public async void SetDataContext(DataGrid dataPreview)
        {

            if (dataGrid.Columns.Count > 0)
                dataPreview.DataContext = dataGrid;
        }

        public async Task<bool>GenerateAssayDesurveyVertical(bool bToe, DrillholeDesurveyEnum surveyMethod)
        {
            if (assayDesurvMapper == null)
                InitialiseAssayMapping();

            //surveymethod has to be Tangential
            var collarResults = await _desurveyService.AssayVerticalHole(assayDesurvMapper, surveyMethod, tableFields, bToe, assayXmlData );

            return true;
        }

        public async Task<bool> GenerateAssayDesurveyFromCollarSurvey(bool bToe, DrillholeDesurveyEnum surveyMethod)
        {
            if (assayDesurvMapper == null)
                InitialiseAssayMapping();

            //surveymethod has to be Tangential
            var assayResults = await _desurveyService.AssaySurveyHole(assayDesurvMapper, surveyMethod, tableFields, bToe, assayXmlData);

            return true;
        }

        public async Task<bool> GenerateAssayDesurveyFromDownhole(bool bToe, DrillholeDesurveyEnum surveyMethod)
        {
            if (assayDesurvMapper == null)
                InitialiseAssayMapping();

            //surveymethod has to be Tangential
            var assayResults = await _desurveyService.AssayDownhole(assayDesurvMapper, surveyMethod, tableFields, bToe, assayXmlData);

            return true;
        }
    }
}

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
    public class GenerateAssayDesurveyResults : GenerateSurveyDesurveyResults
    {
        private AssayDesurveyServices _desurveyService;

        //TODO - show results
        private IMapper assayDesurvMapper;
        public ImportTableFields assayTableFields { get; set; }

        private string DesurveyTableXmlName { get; set; }

        public List<XElement> assayXmlData { get; set; }

        public GenerateAssayDesurveyResults(bool _savedSession, string _sessionName, string _projectLocation, ImportTableFields _assayFields,
            List<XElement> drillholeData) : base(_savedSession, _sessionName, _projectLocation, _assayFields, drillholeData)
        {
           
            //dataGrid = new System.Data.DataTable();

            savedSession = _savedSession;
            sessionName = _sessionName;
            projectLocation = _projectLocation;
            assayXmlData = drillholeData;
            assayTableFields = _assayFields;

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
            var assayResults = await _desurveyService.AssayVerticalHole(assayDesurvMapper, surveyMethod, collarTableFields, assayTableFields, bToe, assayXmlData ); //ADD COLLARTABLEFIELDS AND INHERIT

            //create tableFields table and store desurveyed results
            await _xmlService.Drillholedesurveydata(DesurveyTableXmlName, assayResults, DrillholeConstants.drillholeDesurv, DrillholeTableType.assay);

            //save to xml
            if (savedSession)
                await _xmlService.Drillholedesurveydata(projectLocation + "\\" + sessionName + ".dh", DesurveyTableXmlName, DrillholeConstants.drillholeProject, DrillholeConstants.drillholeData, DrillholeTableType.assay);

            return true;
        }

        public async Task<bool> GenerateAssayDesurveyFromCollarSurvey(bool bToe, DrillholeDesurveyEnum surveyMethod)
        {
            if (assayDesurvMapper == null)
                InitialiseAssayMapping();

            //surveymethod has to be Tangential
            var assayResults = await _desurveyService.AssaySurveyHole(assayDesurvMapper, surveyMethod, collarTableFields, assayTableFields, bToe, assayXmlData);

            return true;
        }

        public async Task<bool> GenerateAssayDesurveyFromDownhole(bool bToe, DrillholeDesurveyEnum surveyMethod)
        {
            if (assayDesurvMapper == null)
                InitialiseAssayMapping();

            //surveymethod has to be Tangential
            var assayResults = await _desurveyService.AssayDownhole(assayDesurvMapper, surveyMethod, collarTableFields, assayTableFields, bToe, assayXmlData);

            return true;
        }
    }
}

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
    public class GenerateCollarDesurveyResults
    {
        private CollarDesurveyServices _desurveyService;

        public IDesurveyDrillhole _desurveyTable;

        //TODO - show results
        public System.Data.DataTable dataGrid { get; set; }

        private IMapper collarDesurvMapper;
        public ImportTableFields collarTableFields { get; set; }
        


        public XmlService _xmlService;
        public IDrillholeXML _xml;

        public List<XElement> collarXmlData { get; set; }
        public bool savedSession { get; set; }
        public string sessionName { get; set; }
        public string projectLocation { get; set; }

        public string DesurveyTableXmlName { get; set; }

        public GenerateCollarDesurveyResults(bool _savedSession, string _sessionName, string _projectLocation, ImportTableFields _collarTableFields,
            List<XElement> drillholeData)
        {
           
            //dataGrid = new System.Data.DataTable();

            savedSession = _savedSession;
            sessionName = _sessionName;
            projectLocation = _projectLocation;
            collarXmlData = drillholeData;
            collarTableFields = _collarTableFields;

            XmlSetUP(DrillholeTableType.collar);

        }

        public async void XmlSetUP(DrillholeTableType tableType)
        {
            //create XML temp table
            if (_xml == null)
                _xml = new Drillholes.XML.XmlController();

            if (_xmlService == null)
                _xmlService = new XmlService(_xml);

            if (!savedSession)
            {
                DesurveyTableXmlName =  await XmlDefaultPath.GetFullPathAndFilename(DrillholeConstants.drillholeDesurv, tableType.ToString());
            }
            else
            {
                DesurveyTableXmlName = await XmlDefaultPath.GetProjectPathAndFilename(DrillholeConstants.drillholeDesurv, tableType.ToString(), sessionName, projectLocation) ;
            }
        }


        //TODO move out of here
        private async void InitialiseCollarMapping()
        {
            _desurveyTable = new Drillholes.CreateDrillholes.CreateHolesByType();

            var config = new MapperConfiguration(cfg => { cfg.CreateMap<CollarDesurveyDto, CollarDesurveyObject>(); });

            collarDesurvMapper = config.CreateMapper();
            var source = new CollarDesurveyDto();

            var dest = collarDesurvMapper.Map<CollarDesurveyDto, CollarDesurveyObject>(source);

            _desurveyService = new CollarDesurveyServices(_desurveyTable);
        }
      
        public async void SetDataContext(DataGrid dataPreview)
        {

            if (dataGrid.Columns.Count > 0)
                dataPreview.DataContext = dataGrid;
        }

        public async Task<bool>GenerateCollarDesurveyVertical(bool bToe, DrillholeDesurveyEnum surveyMethod)
        {
            if (collarDesurvMapper == null)
                InitialiseCollarMapping();

            //surveymethod has to be Tangential
            var collarResults = await _desurveyService.CollarVerticalHole(collarDesurvMapper, surveyMethod, collarTableFields, bToe, collarXmlData );

            StoreResultsToXml(collarResults, false);

            return true;
        }

        public async Task<bool> GenerateCollarDesurveyFromCollarSurvey(bool bToe, DrillholeDesurveyEnum surveyMethod)
        {

            if (collarDesurvMapper == null)
                InitialiseCollarMapping();

            //surveymethod has to be Tangential
            var collarResults = await _desurveyService.CollarSurveyHole(collarDesurvMapper, surveyMethod, collarTableFields, bToe, collarXmlData);

            StoreResultsToXml(collarResults, false);

            return true;
        }

        private async void StoreResultsToXml(CollarDesurveyObject collarResults, bool bDownhole)
        {

            //create tableFields table and store desurveyed results
            await _xmlService.Drillholedesurveydata(DesurveyTableXmlName, collarResults, DrillholeConstants.drillholeDesurv, DrillholeTableType.collar, bDownhole);

            //save to xml
            if (savedSession)
                await _xmlService.Drillholedesurveydata(projectLocation + "\\" + sessionName + ".dh", DesurveyTableXmlName, DrillholeConstants.drillholeProject, DrillholeConstants.drillholeData, DrillholeTableType.collar,bDownhole);

        }
    }
}

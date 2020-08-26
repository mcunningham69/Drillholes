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
    public class GenerateContinuousDesurveyResults : GenerateAssayDesurveyResults
    {
        private ContinuousDesurveyServices _desurveyService;

        private IMapper continuousDesurvMapper;

        public ImportTableFields continuousTableFields { get; set; }

        public List<XElement> continuousXmlData { get; set; }

        public GenerateContinuousDesurveyResults(bool _savedSession, string _sessionName, string _projectLocation, ImportTableFields _continuousTableFields,
            List<XElement> drillholeData) : base(_savedSession, _sessionName, _projectLocation, _continuousTableFields, drillholeData)
        {
           
            //dataGrid = new System.Data.DataTable();

            savedSession = _savedSession;
            sessionName = _sessionName;
            projectLocation = _projectLocation;
            continuousXmlData = drillholeData;
            continuousTableFields = _continuousTableFields;

            XmlSetUP(DrillholeTableType.continuous);

        }


        //TODO move out of here
     
        private async void InitialiseContinuousMapping()
        {
            _desurveyTable = new Drillholes.CreateDrillholes.CreateHolesByType();

            var config = new MapperConfiguration(cfg => { cfg.CreateMap<ContinuousDesurveyDto, ContinuousDesurveyObject>(); });

            continuousDesurvMapper = config.CreateMapper();
            var source = new ContinuousDesurveyDto();

            var dest = continuousDesurvMapper.Map<ContinuousDesurveyDto, ContinuousDesurveyObject>(source);

            _desurveyService = new ContinuousDesurveyServices(_desurveyTable);
        }
     

        public async Task<bool>GenerateContinuousDesurveyVertical(bool bToe, bool bCollar, DrillholeDesurveyEnum surveyMethod)
        {
            if (continuousDesurvMapper == null)
                InitialiseContinuousMapping();

            //surveymethod has to be Tangential
            var contResults = await _desurveyService.ContinuousVerticalHole(continuousDesurvMapper, surveyMethod, collarTableFields, continuousTableFields, bToe, bCollar, continuousXmlData );

            //create tableFields table and store desurveyed results
            await _xmlService.Drillholedesurveydata(DesurveyTableXmlName, contResults, DrillholeConstants.drillholeDesurv, DrillholeTableType.continuous, false);

            //save to xml
            if (savedSession)
                await _xmlService.Drillholedesurveydata(projectLocation + "\\" + sessionName + ".dh", DesurveyTableXmlName, DrillholeConstants.drillholeProject, DrillholeConstants.drillholeData, DrillholeTableType.continuous, false);

            StoreResultsToXml(contResults,false);

            return true;
        }

        public async Task<bool> GenerateContinuousDesurveyFromCollarSurvey(bool bToe, bool bCollar, DrillholeDesurveyEnum surveyMethod)
        {
            if (continuousDesurvMapper == null)
                InitialiseContinuousMapping();

            //surveymethod has to be Tangential
            var contResults = await _desurveyService.ContinuousSurveyHole(continuousDesurvMapper, surveyMethod, collarTableFields, continuousTableFields, bToe, bCollar, continuousXmlData);

            StoreResultsToXml(contResults,false);

            return true;
        }

        public async Task<bool> GenerateContinuousDesurveyFromDownhole(bool bToe, bool bCollar, DrillholeDesurveyEnum surveyMethod)
        {
            if (continuousDesurvMapper == null)
                InitialiseContinuousMapping();

            //surveymethod has to be Tangential
            var contResults = await _desurveyService.ContinuousDownhole(continuousDesurvMapper, surveyMethod, collarTableFields, continuousTableFields, surveyTableFields, bToe, bCollar, continuousXmlData);

            StoreResultsToXml(contResults, true);

            return true;
        }

        private async void StoreResultsToXml(ContinuousDesurveyObject contResults, bool bDownhole)
        {

            //create tableFields table and store desurveyed results
            await _xmlService.Drillholedesurveydata(DesurveyTableXmlName, contResults, DrillholeConstants.drillholeDesurv, DrillholeTableType.continuous, bDownhole);

            //save to xml
            if (savedSession)
                await _xmlService.Drillholedesurveydata(projectLocation + "\\" + sessionName + ".dh", DesurveyTableXmlName, DrillholeConstants.drillholeProject, DrillholeConstants.drillholeData, DrillholeTableType.continuous, bDownhole);

        }
    }
}

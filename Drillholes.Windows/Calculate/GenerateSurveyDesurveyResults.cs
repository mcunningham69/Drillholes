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
    public class GenerateSurveyDesurveyResults : GenerateCollarDesurveyResults
    {
        private SurveyDesurveyServices _desurveyService;

        private IMapper surveyDesurvMapper;
        public ImportTableFields surveyTableFields { get; set; }

        public List<XElement> surveyXmlData { get; set; }

        public GenerateSurveyDesurveyResults(bool _savedSession, string _sessionName, string _projectLocation, ImportTableFields _surveyTableFields,
            List<XElement> drillholeData):base(_savedSession, _sessionName, _projectLocation, _surveyTableFields, drillholeData)
        {
            //dataGrid = new System.Data.DataTable();

            savedSession = _savedSession;
            sessionName = _sessionName;
            projectLocation = _projectLocation;
            surveyXmlData = drillholeData;
            surveyTableFields = _surveyTableFields;

            XmlSetUP(DrillholeTableType.survey);

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
       

        public async Task<bool> GenerateSurvey(bool bToe, bool bCollar, DrillholeDesurveyEnum surveyMethod)
        {
            if (surveyDesurvMapper == null)
                InitialiseSurveyMapping();

            //surveymethod has to be Tangential
            var surveyResults = await _desurveyService.SurveyDownhole(surveyDesurvMapper, surveyMethod, collarTableFields, surveyTableFields, bToe, bCollar, surveyXmlData );

            StoreResultsToXml(surveyResults, true);

            return true;
        }

        private async void StoreResultsToXml(SurveyDesurveyObject surveyResults, bool bDownhole)
        {

            //create tableFields table and store desurveyed results
            await _xmlService.Drillholedesurveydata(DesurveyTableXmlName, surveyResults, DrillholeConstants.drillholeDesurv, DrillholeTableType.survey, bDownhole);

            //save to xml
            if (savedSession)
                await _xmlService.Drillholedesurveydata(projectLocation + "\\" + sessionName + ".dh", DesurveyTableXmlName, DrillholeConstants.drillholeProject, DrillholeConstants.drillholeData, DrillholeTableType.survey, bDownhole);

        }

    }
}

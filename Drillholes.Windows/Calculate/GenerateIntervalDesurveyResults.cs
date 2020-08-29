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
    public class GenerateIntervalDesurveyResults : GenerateAssayDesurveyResults
    {
        private IntervalDesurveyServices _desurveyService;

        //TODO - show results

        private IMapper intervalDesurvMapper;

        public ImportTableFields intervalTableFields { get; set; }

        public List<XElement> intervalXmlData { get; set; }

        public GenerateIntervalDesurveyResults(bool _savedSession, string _sessionName, string _projectLocation, ImportTableFields _intervalTableFields,
            List<XElement> drillholeData) : base(_savedSession, _sessionName, _projectLocation, _intervalTableFields, drillholeData)
        {
           
            //dataGrid = new System.Data.DataTable();

            savedSession = _savedSession;
            sessionName = _sessionName;
            projectLocation = _projectLocation;
            intervalXmlData = drillholeData;
            intervalTableFields = _intervalTableFields;

            XmlSetUP(DrillholeTableType.interval);

        }
     
        private async void InitialiseIntervalMapping()
        {
            _desurveyTable = new Drillholes.CreateDrillholes.CreateHolesByType();

            var config = new MapperConfiguration(cfg => { cfg.CreateMap<IntervalDesurveyDto, IntervalDesurveyObject>(); });

            intervalDesurvMapper = config.CreateMapper();
            var source = new IntervalDesurveyDto();

            var dest = intervalDesurvMapper.Map<IntervalDesurveyDto, IntervalDesurveyObject>(source);

            _desurveyService = new IntervalDesurveyServices(_desurveyTable);
        }
     

        public async Task<bool>GenerateIntervalDesurveyVertical(bool bToe, bool bCollar, DrillholeDesurveyEnum surveyMethod)
        {
            if (intervalDesurvMapper == null)
                InitialiseIntervalMapping();

            //surveymethod has to be Tangential
            var intervalResults = await _desurveyService.IntervalVerticalHole(intervalDesurvMapper, surveyMethod, collarTableFields, intervalTableFields, bToe, bCollar, intervalXmlData);

            StoreResultsToXml(intervalResults,false);
           
            return true;

        }

        public async Task<bool> GenerateIntervalDesurveyFromCollarSurvey(bool bToe, bool bCollar, DrillholeDesurveyEnum surveyMethod)
        {
            if (intervalDesurvMapper == null)
                InitialiseIntervalMapping();

            //surveymethod has to be Tangential
            var intervalResults = await _desurveyService.IntervalSurveyHole(intervalDesurvMapper, surveyMethod, collarTableFields, intervalTableFields, bToe, bCollar, intervalXmlData);

            StoreResultsToXml(intervalResults,false);

            return true;
        }

        public async Task<bool> GenerateIntervalDesurveyFromDownhole(bool bToe, bool bCollar, DrillholeDesurveyEnum surveyMethod)
        {
            if (intervalDesurvMapper == null)
                InitialiseIntervalMapping();

            //surveymethod has to be Tangential
            var intervalResults = await _desurveyService.IntervalDownhole(intervalDesurvMapper, surveyMethod, collarTableFields, intervalTableFields, surveyTableFields, bToe, bCollar, intervalXmlData);

            StoreResultsToXml(intervalResults, true);

            return true;
        }

        private async void StoreResultsToXml(IntervalDesurveyObject intervalResults, bool bDownhole)
        {
            //create tableFields table and store desurveyed results
            await _xmlService.Drillholedesurveydata(DesurveyTableXmlName, intervalResults, DrillholeConstants.drillholeDesurv, DrillholeTableType.interval, bDownhole);

            //save to xml
            if (savedSession)
                await _xmlService.Drillholedesurveydata(projectLocation + "\\" + sessionName + ".dh", DesurveyTableXmlName, DrillholeConstants.drillholeProject, DrillholeConstants.drillholeData, DrillholeTableType.interval, bDownhole);
            

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drillholes.Domain;
using Drillholes.Domain.Enum;
using Drillholes.Domain.DataObject;
using Drillholes.Domain.DTO;
using Drillholes.Domain.Services;
using Drillholes.Domain.Interfaces;
using AutoMapper;
using System.Data;

namespace Drillholes.Windows.ViewModel
{
    public class SurveyView : CollarView
    {
        private SurveyTableService _surveyService;
        private ISurveyTable _surveyTable;

        SurveyStatisticsService _surveyStatisticsService;
        ISurveyStatistics _surveyStatistics;


        public SurveyTableObject surveyTableObject { get; set; }

        private ImportTableFields _surveyDataFields;
        public ImportTableFields surveyDataFields
        {
            get
            {
                return this._surveyDataFields;
            }
            set
            {
                this._surveyDataFields = value;
                OnPropertyChanged("surveyDataFields");
            }
        }

        public SurveyView(DrillholeImportFormat _tableFormat, DrillholeTableType _tableType, string _tableLocation, string _tableName) 
            : base(_tableFormat, _tableType, _tableLocation, _tableName)
        {
            surveyTableObject = new SurveyTableObject()
            {
                tableFormat = _tableFormat,
                tableType = _tableType,
                tableLocation = _tableLocation,
                tableName = _tableName,
                surveyType = DrillholeSurveyType.downholesurvey //default
            };

            tableName = _tableName;
            tableLocation = _tableLocation;
            tableFormat = _tableFormat.ToString();
        }

        public override void InitialiseTableMapping()
        {
            //Add ArcSDE
            if (DrillholeImportFormat.fgdb_table == surveyTableObject.tableFormat ||
                DrillholeImportFormat.egdb_table == surveyTableObject.tableFormat)
            {
                //_collarTable = new Drillholes.ArcDialog.CollarEngine();
            }
            else if (DrillholeImportFormat.excel_table == surveyTableObject.tableFormat)
            { }//   _collarTable = new Drillholes.ExcelDialog.CollarEngine();
            else
                _surveyTable = new Drillholes.FileDialog.SurveyImport();

            var config = new MapperConfiguration(cfg => { cfg.CreateMap<SurveyTableDto, SurveyTableObject>(); });

            classMapper = config.CreateMapper();
            var source = new SurveyTableDto();

            var dest = classMapper.Map<SurveyTableDto, SurveyTableObject>(source);

            _surveyService = new SurveyTableService(_surveyTable);

        }

        public override async Task<bool> RetrieveFieldsToMap()
        {
            if (classMapper == null)
                InitialiseTableMapping();

            var surveyService = await _surveyService.GetSurveyFields(classMapper, surveyTableObject.tableLocation, 
                surveyTableObject.tableFormat, surveyTableObject.tableName);

            //manual map 
            surveyTableObject.fields = surveyService.fields;
            surveyTableObject.tableData = surveyService.tableData;
            surveyTableObject.surveyKey = surveyService.tableData.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Select(p => p.columnHeader).FirstOrDefault().ToString();

            surveyDataFields = surveyService.tableData;
            return true;
        }

        public virtual async Task<bool> PreviewDataToImport(int limit)
        {
            List<string> fields = new List<string>();

            if (surveyTableObject.xPreview == null)
            {

                var surveyService = await _surveyService.PreviewData(classMapper, surveyTableObject.tableType, limit);

                surveyTableObject.xPreview = surveyService.xPreview;

            }

            fields = _surveyService.ReturnFields(classMapper);

            if (dataGrid.Columns.Count != fields.Count())
            {

                foreach (string name in fields)
                {
                    dataGrid.Columns.Add(name);
                }


                tableCaption = char.ToUpper(surveyTableObject.tableType.ToString()[0]) +
                    surveyTableObject.tableType.ToString().Substring(1);


                FillTable(); //tableType = 'Collar'

            }

            return true;
        }

        public virtual void FillTable()
        {

            var elements = surveyTableObject.xPreview.Descendants("Surveys").  //survey
                       Select(e => e.Elements());


            foreach (var rows in elements)
            {
                List<XmlNameAndValue> _namesAndValues = new List<XmlNameAndValue>();
                foreach (var element in rows)
                {

                    _namesAndValues.Add(new XmlNameAndValue { Name = element.Name.ToString(), Value = element.Value });
                }

                List<string> myValues = new List<string>();

                foreach (XmlNameAndValue node in _namesAndValues)
                {
                    if (node.Value.ToString() == "")
                    {
                        node.Value = "-";
                    }

                    myValues.Add(node.Value.ToString());

                }

                if (myValues.Count > 0)
                    dataTable.Rows.Add(myValues.ToArray());

            }

            int noOfRecords = dataTable.Rows.Count;

            //white space for formatting on status bar
            string displayItems = (noOfRecords == 1 ? noOfRecords.ToString() + " " + surveyTableObject.tableType +
                "          " : noOfRecords.ToString() + " " + surveyTableObject.tableType + "s          ");

        }

        public override async void ImportGenericFields(bool bImport)
        {
            if (_surveyService != null)
            {
                var surveyService = await _surveyService.ImportAllFieldsAsGeneric(classMapper, bImport);
                surveyTableObject.tableData = surveyService.tableData;
            }

        }

        public override async Task<bool> UpdateFieldnamesInXml()
        {
            UpdateFieldnamesXml.UpdateFieldnamesXML(surveyTableObject.tableType, surveyTableObject.tableData,
                surveyTableObject.collarKey, surveyTableObject.surveyKey, surveyTableObject.tableName);

            return true;
        }

        public virtual void SetSecondaryKey(string holeKey)
        {
            surveyTableObject.surveyKey = surveyDataFields.Where(o => o.columnImportName == holeKey).Select(p => p.columnHeader).FirstOrDefault().ToString();
        }

        public override async Task<bool> UpdateHoleKeyInXml()
        {
            UpdateFieldnamesXml.UpdateFieldnameInXml(DrillholeConstants._Survey, "Constraint", surveyTableObject.surveyKey);

            return true;
        }

        public override void UpdateFieldvalues(string previousSelection, string selectedValue, ImportTableField _searchList,
           bool bImport)
        {
            string _strSearch = _searchList.columnHeader;
            string _strName = _searchList.columnImportName;

            var surveyService = _surveyService.UpdateFieldvalues(previousSelection, classMapper, selectedValue,
                _strSearch, _strName);

            surveyDataFields = surveyService.Result.tableData;

            if (selectedValue == DrillholeConstants.notImported)
            {
                ImportGenericFields(bImport);
            }

            surveyTableObject.surveyKey = surveyDataFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Select(p => p.columnHeader).FirstOrDefault().ToString();

        }

        //public override async Task<bool> SummaryStatistics()
        //{
        //    //if (statisticsMapper == null)
        //    //    _surveyStatistics = await InitialiseStatisticsMapping();

        //    return true;
        //}
    }
}

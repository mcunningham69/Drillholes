using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drillholes.Domain;
using Drillholes.Domain.Enum;
using Drillholes.Domain.DataObject;
using Drillholes.Domain.Services;
using Drillholes.Domain.Interfaces;
using Drillholes.Domain.DTO;
using Drillholes.XML;
using AutoMapper;
using System.Data;

namespace Drillholes.Windows.ViewModel
{
    public class ContinuousView : IntervalView
    {
        public ContinuousTableObject continuousTableObject { get; set; }

        private ContinuousTableService _continuousService;
        private IContinuousTable _continuousTable;

        private ImportTableFields _continuousDataFields;
        public ImportTableFields continuousDataFields
        {
            get
            {
                return this._continuousDataFields;
            }
            set
            {
                this._continuousDataFields = value;
                OnPropertyChanged("continuousDataFields");
            }
        }

        public ContinuousView(DrillholeImportFormat _tableFormat, DrillholeTableType _tableType, string _tableLocation, string _tableName, bool _savedSession, string _sessionName, string _projectLocation)
            : base(_tableFormat, _tableType, _tableLocation, _tableName, _savedSession, _sessionName, _projectLocation)
        {
            continuousTableObject = new ContinuousTableObject()
            {
                tableFormat = _tableFormat,
                tableType = _tableType,
                tableLocation = _tableLocation,
                tableName = _tableName,
                surveyType = DrillholeSurveyType.downholesurvey //default
            };

            savedSession = _savedSession;
            sessionName = _sessionName;
            projectLocation = _projectLocation;
        }


        public override void InitialiseTableMapping()
        {
            //Add ArcSDE
            if (DrillholeImportFormat.fgdb_table == intervalTableObject.tableFormat ||
                DrillholeImportFormat.egdb_table == intervalTableObject.tableFormat)
            {
                //_collarTable = new Drillholes.ArcDialog.CollarEngine();
            }
            else if (DrillholeImportFormat.excel_table == intervalTableObject.tableFormat)
            { }//   _collarTable = new Drillholes.ExcelDialog.CollarEngine();
            else
                _continuousTable = new Drillholes.FileDialog.ContinuousImport();

            var config = new MapperConfiguration(cfg => { cfg.CreateMap<ContinuousTableDto, ContinuousTableObject>(); });

            classMapper = config.CreateMapper();
            var source = new ContinuousTableDto();

            var dest = classMapper.Map<ContinuousTableDto, ContinuousTableObject>(source);

            _continuousService = new ContinuousTableService(_continuousTable);

        }

        public override async Task<bool> RetrieveFieldsToMap()
        {
            if (classMapper == null)
                InitialiseTableMapping();

            var continuousService = await _continuousService.GetSurveyFields(classMapper, intervalTableObject.tableLocation,
                intervalTableObject.tableFormat, intervalTableObject.tableName);

            //manual map 
            continuousTableObject.fields = continuousService.fields;
            continuousTableObject.tableData = continuousService.tableData;
            continuousTableObject.intervalKey = continuousService.tableData.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Select(p => p.columnHeader).FirstOrDefault().ToString();

            continuousDataFields = continuousService.tableData;

            if (continuousDataFields != null)
                await _xmlService.DrillholeFields(fullPathnameFields, continuousService.tableData, DrillholeTableType.continuous, rootNameFields);

            return true;
        }

        public override async Task<bool> PreviewDataToImport(int limit)
        {
            List<string> fields = new List<string>();

            if (continuousTableObject.xPreview == null)
            {

                var continuousService = await _continuousService.PreviewData(classMapper, continuousTableObject.tableType, limit);

                continuousTableObject.xPreview = continuousService.xPreview;

                await _xmlService.DrillholeData(fullPathnameData, continuousService.xPreview, DrillholeTableType.continuous, DrillholeConstants._Continuous, rootNameData);


            }

            fields = _continuousService.ReturnFields(classMapper);

            if (dataGrid.Columns.Count != fields.Count())
            {

                foreach (string name in fields)
                {
                    dataGrid.Columns.Add(name);
                }


                tableCaption = char.ToUpper(continuousTableObject.tableType.ToString()[0]) +
                    continuousTableObject.tableType.ToString().Substring(1);


                FillTable(); //tableType = 'Collar'

            }

            return true;
        }

        public virtual void FillTable()
        {

            dataGrid.Rows.Clear();

            var continuousElements = continuousTableObject.xPreview.Elements();


            foreach (var element in continuousElements)
            {
                if (element.Attribute("Ignore").Value.ToUpper() == "FALSE")
                {
                    var rows = element.Elements();
                    List<XmlNameAndValue> _namesAndValues = new List<XmlNameAndValue>();
                    foreach (var row in rows)
                    {

                        _namesAndValues.Add(new XmlNameAndValue { Name = row.Name.ToString(), Value = row.Value });
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
                        dataGrid.Rows.Add(myValues.ToArray());
                }
            }

            int noOfRecords = dataGrid.Rows.Count;

            //white space for formatting on status bar
            string displayItems = (noOfRecords == 1 ? noOfRecords.ToString() + " " + continuousTableObject.tableType +
                "          " : noOfRecords.ToString() + " " + continuousTableObject.tableType + "s          ");

        }

        public override async void ImportGenericFields(bool bImport)
        {
            if (_continuousService != null)
            {
                var continuousService = await _continuousService.ImportAllFieldsAsGeneric(classMapper, bImport);
                continuousTableObject.tableData = continuousService.tableData;
            }

        }

        public override async Task<bool> UpdateFieldnamesInXml()
        {
            UpdateFieldnamesXml.UpdateFieldnamesXML(continuousTableObject.tableType, continuousTableObject.tableData,
                continuousTableObject.collarKey, continuousTableObject.intervalKey, continuousTableObject.tableName);

            return true;
        }

        public override void SetSecondaryKey(string holeKey)
        {
            continuousTableObject.intervalKey = continuousDataFields.Where(o => o.columnImportName == holeKey).Select(p => p.columnHeader).FirstOrDefault().ToString();
        }

        public override async Task<bool> UpdateHoleKeyInXml()
        {
            UpdateFieldnamesXml.UpdateFieldnameInXml(DrillholeConstants._Interval, "Constraint", intervalTableObject.intervalKey);

            return true;
        }

        public override async void UpdateFieldvalues(string previousSelection, string selectedValue, ImportTableField _searchList,
           bool bImport)
        {

            string _strSearch = _searchList.columnHeader;
            string _strName = _searchList.columnImportName;

            var continuousService = _continuousService.UpdateFieldvalues(previousSelection, classMapper, selectedValue,
                _strSearch, _strName);


            continuousDataFields = continuousService.Result.tableData;

            if (continuousDataFields != null)
                await _xmlService.DrillholeFields(fullPathnameFields, continuousDataFields, DrillholeTableType.continuous, rootNameFields);


            if (selectedValue == DrillholeConstants.notImported)
            {

                ImportGenericFields(bImport);
            }

            surveyTableObject.surveyKey = continuousDataFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Select(p => p.columnHeader).FirstOrDefault().ToString();

        }

       
    }
}

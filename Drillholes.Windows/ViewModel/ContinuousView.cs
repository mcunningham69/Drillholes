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
using AutoMapper;
using System.Data;

namespace Drillholes.Windows.ViewModel
{
    public class ContinuousView : IntervalView
    {
        public IntervalTableObject continuousTableObject { get; set; }

        private IntervalTableService _continuousService;
        private IIntervalTable _continuousTable;

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

        public ContinuousView(DrillholeImportFormat _tableFormat, DrillholeTableType _tableType, string _tableLocation, string _tableName)
            : base(_tableFormat, _tableType, _tableLocation, _tableName)
        {
            continuousTableObject = new IntervalTableObject()
            {
                tableFormat = _tableFormat,
                tableType = _tableType,
                tableLocation = _tableLocation,
                tableName = _tableName,
                surveyType = DrillholeSurveyType.downholesurvey //default
            };
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
                _continuousTable = new Drillholes.FileDialog.IntervalImport();

            var config = new MapperConfiguration(cfg => { cfg.CreateMap<IntervalTableDto, IntervalTableObject>(); });

            classMapper = config.CreateMapper();
            var source = new IntervalTableDto();

            var dest = classMapper.Map<IntervalTableDto, IntervalTableObject>(source);

            _continuousService = new IntervalTableService(_continuousTable);

        }

        public override async Task<bool> RetrieveFieldsToMap()
        {
            if (classMapper == null)
                InitialiseTableMapping();

            var intervalService = await _continuousService.GetSurveyFields(classMapper, intervalTableObject.tableLocation,
                intervalTableObject.tableFormat, intervalTableObject.tableName);

            //manual map 
            intervalTableObject.fields = intervalService.fields;
            intervalTableObject.tableData = intervalService.tableData;
            intervalTableObject.intervalKey = intervalService.tableData.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Select(p => p.columnHeader).FirstOrDefault().ToString();

            intervalDataFields = intervalService.tableData;
            return true;
        }

        public virtual async Task<bool> PreviewDataToImport(int limit)
        {
            List<string> fields = new List<string>();

            if (intervalTableObject.xPreview == null)
            {

                var intervalService = await _continuousService.PreviewData(classMapper, intervalTableObject.tableType, limit);

                intervalTableObject.xPreview = intervalService.xPreview;

            }

            fields = _continuousService.ReturnFields(classMapper);

            if (dataGrid.Columns.Count != fields.Count())
            {

                foreach (string name in fields)
                {
                    dataGrid.Columns.Add(name);
                }


                tableCaption = char.ToUpper(intervalTableObject.tableType.ToString()[0]) +
                    intervalTableObject.tableType.ToString().Substring(1);


                FillTable(tableCaption, dataGrid); //tableType = 'Collar'

            }

            return true;
        }

        public virtual void FillTable(string descendants, DataTable dataTable)
        {

            var elements = intervalTableObject.xPreview.Descendants(descendants).  //assay
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
            string displayItems = (noOfRecords == 1 ? noOfRecords.ToString() + " " + intervalTableObject.tableType +
                "          " : noOfRecords.ToString() + " " + intervalTableObject.tableType + "s          ");

        }

        public override async void ImportGenericFields(bool bImport)
        {
            if (_continuousService != null)
            {
                var intervalService = await _continuousService.ImportAllFieldsAsGeneric(classMapper, bImport);
                intervalTableObject.tableData = intervalService.tableData;
            }

        }

        public override async Task<bool> UpdateFieldnamesInXml()
        {
            UpdateFieldnamesXml.UpdateFieldnamesXML(intervalTableObject.tableType, intervalTableObject.tableData,
                intervalTableObject.collarKey, intervalTableObject.intervalKey, intervalTableObject.tableName);

            return true;
        }

        public override void SetSecondaryKey(string holeKey)
        {
            intervalTableObject.intervalKey = intervalDataFields.Where(o => o.columnImportName == holeKey).Select(p => p.columnHeader).FirstOrDefault().ToString();
        }

        public override async Task<bool> UpdateHoleKeyInXml()
        {
            UpdateFieldnamesXml.UpdateFieldnameInXml(DrillholeConstants._Interval, "Constraint", intervalTableObject.intervalKey);

            return true;
        }

        public override void UpdateFieldvalues(string previousSelection, string selectedValue, ImportTableField _searchList,
           bool bImport)
        {

            string _strSearch = _searchList.columnHeader;
            string _strName = _searchList.columnImportName;

            var intervalService = _continuousService.UpdateFieldvalues(previousSelection, classMapper, selectedValue,
                _strSearch, _strName);


            intervalDataFields = intervalService.Result.tableData;


            if (selectedValue == DrillholeConstants.notImported)
            {

                ImportGenericFields(bImport);
            }

            intervalTableObject.surveyKey = intervalDataFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Select(p => p.columnHeader).FirstOrDefault().ToString();

        }

       
    }
}

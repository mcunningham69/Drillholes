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
    public class IntervalView : AssayView
    {
        public IntervalTableObject intervalTableObject { get; set; }

        private IntervalTableService _intervalService;
        private IIntervalTable _intervalTable;

        private ImportTableFields _intervalDataFields;
        public ImportTableFields intervalDataFields
        {
            get
            {
                return this._intervalDataFields;
            }
            set
            {
                this._intervalDataFields = value;
                OnPropertyChanged("intervalDataFields");
            }
        }

        public IntervalView(DrillholeImportFormat _tableFormat, DrillholeTableType _tableType, string _tableLocation, string _tableName)
            : base(_tableFormat, _tableType, _tableLocation, _tableName)
        {
            intervalTableObject = new IntervalTableObject()
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
                _intervalTable = new Drillholes.FileDialog.IntervalImport();

            var config = new MapperConfiguration(cfg => { cfg.CreateMap<IntervalTableDto, IntervalTableObject>(); });

            classMapper = config.CreateMapper();
            var source = new IntervalTableDto();

            var dest = classMapper.Map<IntervalTableDto, IntervalTableObject>(source);

            _intervalService = new IntervalTableService(_intervalTable);

        }

        public override async Task<bool> RetrieveFieldsToMap()
        {
            if (classMapper == null)
                InitialiseTableMapping();

            var intervalService = await _intervalService.GetSurveyFields(classMapper, intervalTableObject.tableLocation,
                intervalTableObject.tableFormat, intervalTableObject.tableName);

            //manual map 
            intervalTableObject.fields = intervalService.fields;
            intervalTableObject.tableData = intervalService.tableData;
            intervalTableObject.intervalKey = intervalService.tableData.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Select(p => p.columnHeader).FirstOrDefault().ToString();

            intervalDataFields = intervalService.tableData;

            if (intervalDataFields != null)
                await _xmlService.DrillholeFields(fullPathnameFields, intervalService.tableData, DrillholeTableType.interval, rootNameFields);

            return true;
        }

        public virtual async Task<bool> PreviewDataToImport(int limit)
        {
            List<string> fields = new List<string>();

            if (intervalTableObject.xPreview == null)
            {

                var intervalService = await _intervalService.PreviewData(classMapper, intervalTableObject.tableType, limit);

                intervalTableObject.xPreview = intervalService.xPreview;

                await _xmlService.DrillholeData(fullPathnameData, intervalService.xPreview, DrillholeTableType.interval, DrillholeConstants._Interval + "s", rootNameData);


            }

            fields = _intervalService.ReturnFields(classMapper);

            if (dataGrid.Columns.Count != fields.Count())
            {

                foreach (string name in fields)
                {
                    dataGrid.Columns.Add(name);
                }


                tableCaption = char.ToUpper(intervalTableObject.tableType.ToString()[0]) +
                    intervalTableObject.tableType.ToString().Substring(1);


                FillTable(); //tableType = 'Collar'

            }

            return true;
        }

        public virtual void FillTable()
        {

            dataGrid.Rows.Clear();

            var intervalElements = intervalTableObject.xPreview.Elements();


            foreach (var element in intervalElements)
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
            string displayItems = (noOfRecords == 1 ? noOfRecords.ToString() + " " + intervalTableObject.tableType +
                "          " : noOfRecords.ToString() + " " + intervalTableObject.tableType + "s          ");

        }

        public override async void ImportGenericFields(bool bImport)
        {
            if (_intervalService != null)
            {
                var intervalService = await _intervalService.ImportAllFieldsAsGeneric(classMapper, bImport);
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

        public override async void UpdateFieldvalues(string previousSelection, string selectedValue, ImportTableField _searchList,
           bool bImport)
        {

            string _strSearch = _searchList.columnHeader;
            string _strName = _searchList.columnImportName;

            var intervalService = _intervalService.UpdateFieldvalues(previousSelection, classMapper, selectedValue,
                _strSearch, _strName);


            intervalDataFields = intervalService.Result.tableData;

            if (intervalDataFields != null)
                await _xmlService.DrillholeFields(fullPathnameFields, intervalDataFields, DrillholeTableType.interval, rootNameFields);

            if (selectedValue == DrillholeConstants.notImported)
            {

                ImportGenericFields(bImport);
            }

            intervalTableObject.surveyKey = intervalDataFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Select(p => p.columnHeader).FirstOrDefault().ToString();

        }

       
    }
}

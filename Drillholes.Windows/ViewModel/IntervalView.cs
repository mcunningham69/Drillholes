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

        public IntervalView(DrillholeImportFormat _tableFormat, DrillholeTableType _tableType, string _tableLocation, string _tableName, bool _savedSession, string _sessionName, string _projectLocation)
            : base(_tableFormat, _tableType, _tableLocation, _tableName, _savedSession, _sessionName, _projectLocation)
        {
            intervalTableObject = new IntervalTableObject()
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

            importAllColumns = "Import All Columns In " + (_tableType.ToString().ToUpper()) + " Table";

            dataGrid = new System.Data.DataTable();

            savedSession = _savedSession;
            sessionName = _sessionName;
            projectLocation = _projectLocation;

            XmlSetUP(_tableType.ToString());

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

        public override async Task<bool> RetrieveFieldsToMap(bool bOpen)
        {
            if (classMapper == null)
                InitialiseTableMapping();

            if (bOpen)
            {
                var intervalObject = await _xmlService.DrillholeFields(projectLocation + "\\" + sessionName + ".dh", fullPathnameFields, DrillholeConstants.drillholeProject, DrillholeConstants.drillholeFields, intervalTableObject.tableType) as IntervalTableObject;
                intervalTableObject.tableData = intervalObject.tableData;
                intervalTableObject.fields = intervalObject.fields;
                intervalTableObject.collarKey = intervalObject.tableData.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Select(p => p.columnHeader).FirstOrDefault().ToString();

                collarTableObject.tableIsValid = true;

                collarDataFields = collarTableObject.tableData;

            }
            else
            {

                var intervalService = await _intervalService.GetSurveyFields(classMapper, intervalTableObject.tableLocation,
                intervalTableObject.tableFormat, intervalTableObject.tableName);

                //manual map 
                intervalTableObject.fields = intervalService.fields;
                intervalTableObject.tableData = intervalService.tableData;
                intervalTableObject.intervalKey = intervalService.tableData.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Select(p => p.columnHeader).FirstOrDefault().ToString();

                intervalDataFields = intervalService.tableData;

                if (intervalDataFields != null)
                {
                    //create tableFields table
                    await _xmlService.DrillholeFields(fullPathnameFields, intervalService.tableData, DrillholeTableType.interval, rootNameFields);

                    if (savedSession)
                        _xmlService.DrillholeFields(projectLocation + "\\" + sessionName + ".dh", fullPathnameFields, DrillholeConstants.drillholeProject, intervalTableObject.tableType);
                }
            }
            return true;
        }

        public override async Task<bool> PreviewDataToImport(int limit, bool bOpen)
        {
            List<string> fields = new List<string>();

            if (bOpen)
            {
                var intervalObject = await _xmlService.DrillholeData(projectLocation + "\\" + sessionName + ".dh", fullPathnameFields, DrillholeConstants.drillholeProject, DrillholeConstants.drillholeData, intervalTableObject.tableType);
                intervalTableObject.xPreview = intervalObject;

            }
            else
            {
                if (intervalTableObject.xPreview == null)
                {
                    var intervalService = await _intervalService.PreviewData(classMapper, intervalTableObject.tableType, limit);

                    intervalTableObject.xPreview = intervalService.xPreview;

                    await _xmlService.DrillholeData(fullPathnameData, intervalService.xPreview, DrillholeTableType.interval, DrillholeConstants._Interval + "s", rootNameData);

                    if (savedSession)
                        _xmlService.DrillholeData(projectLocation + "\\" + sessionName + ".dh", fullPathnameData, DrillholeConstants.drillholeProject, intervalTableObject.tableType);

                }
            }
            fields = intervalTableObject.fields;

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

        public override async void FillTable()
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

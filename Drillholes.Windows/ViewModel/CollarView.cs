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

namespace Drillholes.Windows.ViewModel
{
    public class CollarView : ViewEditModel
    {
        private CollarTableService _collarService;

        private ICollarTable _collarTable;

        public System.Data.DataTable dataGrid { get; set; }

        public IMapper classMapper = null;


        private ImportTableFields _collarDataFields;
        public ImportTableFields collarDataFields
        {
            get
            {
                return this._collarDataFields;
            }
            set
            {
                this._collarDataFields = value;
                OnPropertyChanged("collarDataFields");
            }
        }

        private CollarTableObject _collarTableObject;
        public CollarTableObject collarTableObject
        {
            get
            {
                return this._collarTableObject;
            }
            set
            {
                this._collarTableObject = value;
                OnPropertyChanged("collarTableObject");
            }
        }

        public string tableCaption { get; set; }
        public string tableName { get; set; }
        public string tableLocation { get; set; }
        public string tableFormat { get; set; }
        public string collarKey { get; set; }
        public bool skipTable { get; set; }
        public bool importChecked { get; set; }

        public string _importAllColumns;
        public string importAllColumns
        {
            get
            {
                return this._importAllColumns;
            }
            set
            {
                this._importAllColumns = value;
                OnPropertyChanged("importAllColumns");
            }
        }
        private string _tableFields;
        public string tableFields
        {
            get
            {
                return this._tableFields;
            }
            set
            {
                this._tableFields = value;
                OnPropertyChanged("tableFields");
            }
        }

        public XmlService _xmlService;
        public IDrillholeXML _xml;

        public string fullPathnameFields { get; set; }
        public string fullPathnameData { get; set; }

        public string rootNameFields = "DrillholeFields";
        public string rootNameData = "DrillholeData";

        public bool savedSession { get; set; }

        public string sessionName { get; set; }
        public string projectLocation { get; set; }

        public CollarView(DrillholeImportFormat _tableFormat, DrillholeTableType _tableType, string _tableLocation,
            string _tableName, bool _savedSession, string _sessionName, string _projectLocation)
        {
            collarTableObject = new CollarTableObject()
            {
                tableFormat = _tableFormat,
                tableType = _tableType,
                tableLocation = _tableLocation,
                tableName = _tableName,
                surveyType = DrillholeSurveyType.vertical //default
            };

            tableName = _tableName;
            tableLocation = _tableLocation;
            tableFormat = _tableFormat.ToString();

            importAllColumns = "Import All Columns In " + (_tableType.ToString().ToUpper())  + " Table";

            dataGrid = new System.Data.DataTable();

            savedSession = _savedSession;
            sessionName = _sessionName;
            projectLocation = _projectLocation;

            XmlSetUP(_tableType.ToString());

        }

        public void XmlSetUP(string tableType)
        {
            //create XML temp table
            if (_xml == null)
                _xml = new Drillholes.XML.XmlController();

            if (_xmlService == null)
                _xmlService = new XmlService(_xml);

            if (!savedSession)
            {
                fullPathnameFields = XmlDefaultPath.GetFullPathAndFilename(rootNameFields, tableType);
                fullPathnameData = XmlDefaultPath.GetFullPathAndFilename(rootNameData, tableType);
            }
            else
            {
                fullPathnameFields = XmlDefaultPath.GetProjectPathAndFilename(rootNameFields, tableType, sessionName, projectLocation) ;
                fullPathnameData = XmlDefaultPath.GetProjectPathAndFilename(rootNameData, tableType, sessionName, projectLocation);
            }

        }

        public async Task<bool> UpdateXmlPreferences(string fullName, string xmlName, bool valueToChange)
        {
            await _xmlService.DrillholePreferences(fullName, xmlName, valueToChange, DrillholeConstants.drillholePref);

            return true;
        }

        public async Task<bool> UpdateXmlPreferences(string fullName, string xmlName, string valueToChange)
        {
            await _xmlService.DrillholePreferences(fullName, xmlName, valueToChange, DrillholeConstants.drillholePref);

            return true;
        }

        //TODO move out of here
        public virtual void InitialiseTableMapping()
        {
            //Add ArcSDE
            if (DrillholeImportFormat.fgdb_table == collarTableObject.tableFormat ||
                DrillholeImportFormat.egdb_table == collarTableObject.tableFormat)
            {
                //_collarTable = new Drillholes.ArcDialog.CollarEngine();
            }
            else if (DrillholeImportFormat.excel_table == collarTableObject.tableFormat)
            { }//   _collarTable = new Drillholes.ExcelDialog.CollarEngine();
            else
                _collarTable = new Drillholes.FileDialog.CollarImport();

            var config = new MapperConfiguration(cfg => { cfg.CreateMap<CollarTableDto, CollarTableObject>(); });

            classMapper = config.CreateMapper();
            var source = new CollarTableDto();

            var dest = classMapper.Map<CollarTableDto, CollarTableObject>(source);

            _collarService = new CollarTableService(_collarTable);

        }


        public virtual void SetDataContext(DataGrid dataPreview)
        {

            if (dataGrid.Columns.Count > 0)
                dataPreview.DataContext = dataGrid;
        }

        public virtual async Task<bool> RetrieveFieldsToMap(bool bOpen)
        {
            if (classMapper == null)
                InitialiseTableMapping();


            if (bOpen)
            {
                var collarObject = await _xmlService.DrillholeFields(projectLocation + "\\" + sessionName + ".dh", fullPathnameFields, DrillholeConstants.drillholeProject, DrillholeConstants.drillholeFields, collarTableObject.tableType) as CollarTableObject;
                collarTableObject.tableData = collarObject.tableData;
                collarTableObject.fields = collarObject.fields;
                collarTableObject.collarKey = collarObject.tableData.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Select(p => p.columnHeader).FirstOrDefault().ToString();

                collarTableObject.tableIsValid = true;

                collarDataFields = collarTableObject.tableData;

            }
            else
            {
                var collarService = await _collarService.GetCollarFields(classMapper, collarTableObject.tableFormat,
                    collarTableObject.tableLocation, collarTableObject.tableName);

                //manual map 
                collarTableObject.fields = collarService.fields;
                collarTableObject.tableData = collarService.tableData;
                collarTableObject.collarKey = collarService.tableData.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Select(p => p.columnHeader).FirstOrDefault().ToString();

                collarDataFields = collarService.tableData;

                if (collarDataFields != null)
                {
                    //create tableFields table
                    await _xmlService.DrillholeFields(fullPathnameFields, collarService.tableData, DrillholeTableType.collar, rootNameFields);

                    if (savedSession)
                    {
                        _xmlService.DrillholeFields(projectLocation + "\\" + sessionName + ".dh", fullPathnameFields, DrillholeConstants.drillholeProject, collarTableObject.tableType);
                    }
                }
            }



            return true;
        }

        public virtual async Task<bool> PreviewDataToImport(int limit, bool bOpen)
        {
            if (classMapper == null)
                InitialiseTableMapping();

            List<string> fields = new List<string>();

            if (bOpen)
            {
                var collarObject = await _xmlService.DrillholeData(projectLocation + "\\" + sessionName + ".dh", fullPathnameFields, DrillholeConstants.drillholeProject, DrillholeConstants.drillholeData, collarTableObject.tableType);
                collarTableObject.xPreview = collarObject;

            }
            else
            {
                if (collarTableObject.xPreview == null)
                {

                    var collarService = await _collarService.PreviewData(classMapper, collarTableObject.tableType, limit);

                    collarTableObject.xPreview = collarService.xPreview;

                    await _xmlService.DrillholeData(fullPathnameData, collarService.xPreview, DrillholeTableType.collar, DrillholeConstants._Collar + "s", rootNameData);

                    if (savedSession)
                        _xmlService.DrillholeData(projectLocation + "\\" + sessionName + ".dh", fullPathnameData, DrillholeConstants.drillholeProject, collarTableObject.tableType);

                }


            }

            fields = collarTableObject.fields;

            if (dataGrid.Columns.Count != fields.Count())
            {

                foreach (string name in fields)
                {
                    dataGrid.Columns.Add(name);
                }


                tableCaption = char.ToUpper(collarTableObject.tableType.ToString()[0]) +
                    collarTableObject.tableType.ToString().Substring(1);
            }

            FillTable();

            return true;
        }

        public virtual async void UpdateFieldvalues(string previousSelection, string selectedValue, ImportTableField _searchList,
            bool bImport)
        {

            string _strSearch = _searchList.columnHeader;
            string _strName = _searchList.columnImportName;

            var collarService = _collarService.UpdateFieldvalues(previousSelection, classMapper, selectedValue,
                _strSearch, _strName);


            collarDataFields = collarService.Result.tableData;

            if (collarDataFields != null)
                await _xmlService.DrillholeFields(fullPathnameFields, collarDataFields, DrillholeTableType.collar, rootNameFields);

            if (selectedValue == DrillholeConstants.notImported)
            {

                ImportGenericFields(bImport);
            }

            if (selectedValue == "Hole ID")
                collarKey = collarDataFields.Where(o => o.columnImportAs == DrillholeConstants.holeID).Select(p => p.columnHeader).FirstOrDefault().ToString();

        }

        public virtual async void FillTable()
        {
            dataGrid.Rows.Clear();

            var collarElements = collarTableObject.xPreview.Elements();

            foreach(var element in collarElements)
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
            string displayItems = (noOfRecords == 1 ? noOfRecords.ToString() + " " + collarTableObject.tableType +
                "          " : noOfRecords.ToString() + " " + collarTableObject.tableType + "s          ");

        }

        public virtual void ImportAllColumns(bool bImport)
        {
            importChecked = bImport;

        }

        public virtual void SkipTheTable(bool bSkip)
        {
            skipTable = bSkip;
        }

        public virtual async void ImportGenericFields(bool bImport)
        {
            if (_collarService != null)
            {
                var collarService = await _collarService.ImportAllFieldsAsGeneric(classMapper, bImport);
                collarTableObject.tableData = collarService.tableData;
            }

        }

        public virtual async Task<bool> UpdateFieldnamesInXml()
        {
            UpdateFieldnamesXml.UpdateFieldnamesXML(collarTableObject.tableType, collarTableObject.tableData,
                collarTableObject.collarKey, "", collarTableObject.tableName);

            return true;
        }

        public virtual async Task<bool> UpdateHoleKeyInXml()
        {
            UpdateFieldnamesXml.UpdateFieldnameInXml(DrillholeConstants._Collar, "Constraint", collarTableObject.collarKey);

            return true;
        }

        public virtual void SetPrimaryKey(string holeKey)
        {
            collarTableObject.collarKey = collarDataFields.Where(o => o.columnImportName == holeKey).Select(p => p.columnHeader).FirstOrDefault().ToString();
        }

     
    }
}

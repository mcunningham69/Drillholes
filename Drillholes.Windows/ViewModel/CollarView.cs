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
using AutoMapper;
using System.Xml.Linq;
using System.Data;
using System.Windows.Controls;

namespace Drillholes.Windows.ViewModel
{
    public class CollarView : ViewEditModel
    {
        public CollarTableObject collarTableObject { get; set; }
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

        public string tableCaption { get; set; }
        public string collarKey { get; set; }
        public bool skipTable { get; set; }
        public bool importChecked {get;set;}

        public CollarView(DrillholeImportFormat _tableFormat, DrillholeTableType _tableType, string _tableLocation,
            string _tableName)
        {
            collarTableObject = new CollarTableObject()
            {
                tableFormat = _tableFormat,
                tableType = _tableType,
                tableLocation = _tableLocation,
                tableName = _tableName,
                surveyType = DrillholeSurveyType.vertical //default
            };

            dataGrid = new System.Data.DataTable();

        }

        //TODO move out of here
        public virtual void InitialiseMapping()
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

        public virtual async Task<bool> RetrieveFieldsToMap()
        {
            if (classMapper == null)
                InitialiseMapping();

            var collarService = await _collarService.GetCollarFields(classMapper, collarTableObject.tableFormat,
                collarTableObject.tableLocation, collarTableObject.tableName);

            //manual map 
            collarTableObject.fields = collarService.fields;
            collarTableObject.tableData = collarService.tableData;
            collarTableObject.collarKey = collarService.tableData.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Select(p => p.columnHeader).FirstOrDefault().ToString();

            collarDataFields = collarService.tableData;
            return true;
        }

        public virtual async Task<bool> PreviewDataToImport(int limit)
        {
            List<string> fields = new List<string>();

            if (collarTableObject.xPreview == null)
            {

                var collarService = await _collarService.PreviewData(classMapper, collarTableObject.tableType, limit);

                collarTableObject.xPreview = collarService.xPreview;

            }

            fields = _collarService.ReturnFields(classMapper);

            if (dataGrid.Columns.Count != fields.Count())
            {

                foreach (string name in fields)
                {
                    dataGrid.Columns.Add(name);
                }


                tableCaption = char.ToUpper(collarTableObject.tableType.ToString()[0]) + 
                    collarTableObject.tableType.ToString().Substring(1);


                FillTable(tableCaption, dataGrid); //tableType = 'Collar'

            }

            return true;
        }

        public virtual void UpdateFieldvalues(string previousSelection, string selectedValue, ImportTableField _searchList, 
            bool bImport)
        {

            string _strSearch = _searchList.columnHeader;
            string _strName = _searchList.columnImportName;

            var collarService = _collarService.UpdateFieldvalues(previousSelection, classMapper, selectedValue, 
                _strSearch, _strName);


            collarDataFields = collarService.Result.tableData;


            if (selectedValue == DrillholeConstants.notImported)
            {

                ImportGenericFields(bImport);
            }

            if (selectedValue == "Hole ID")
                collarKey = collarDataFields.Where(o => o.columnImportAs == DrillholeConstants.holeID).Select(p => p.columnHeader).FirstOrDefault().ToString();

        }

        public virtual void FillTable(string descendants, DataTable dataTable)
        {

            var elements = collarTableObject.xPreview.Descendants(descendants).  //collar
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

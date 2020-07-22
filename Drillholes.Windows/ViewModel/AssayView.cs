﻿using System;
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
    public class AssayView : SurveyView
    {
        public AssayTableObject assayTableObject { get; set; }
        private AssayTableService _assayService;
        private IAssayTable _assayTable;

        private ImportTableFields _assayDataFields;
        public ImportTableFields assayDataFields
        {
            get
            {
                return this._assayDataFields;
            }
            set
            {
                this._assayDataFields = value;
                OnPropertyChanged("assayDataFields");
            }
        }

        public AssayView(DrillholeImportFormat _tableFormat, DrillholeTableType _tableType, string _tableLocation, string _tableName, bool _savedSession, string _sessionName, string _projectLocation)
            : base(_tableFormat, _tableType, _tableLocation, _tableName, _savedSession, _sessionName, _projectLocation)
        {
            assayTableObject = new AssayTableObject()
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
            if (DrillholeImportFormat.fgdb_table == assayTableObject.tableFormat ||
                DrillholeImportFormat.egdb_table == assayTableObject.tableFormat)
            {
                //_collarTable = new Drillholes.ArcDialog.CollarEngine();
            }
            else if (DrillholeImportFormat.excel_table == assayTableObject.tableFormat)
            { }//   _collarTable = new Drillholes.ExcelDialog.CollarEngine();
            else
                _assayTable = new Drillholes.FileDialog.AssayImport();

            var config = new MapperConfiguration(cfg => { cfg.CreateMap<AssayTableDto, AssayTableObject>(); });

            classMapper = config.CreateMapper();
            var source = new AssayTableDto();

            var dest = classMapper.Map<AssayTableDto, AssayTableObject>(source);

            _assayService = new AssayTableService(_assayTable);

        }

        public override async Task<bool> RetrieveFieldsToMap()
        {
            if (classMapper == null)
                InitialiseTableMapping();

            var assayService = await _assayService.GetSurveyFields(classMapper, assayTableObject.tableLocation,
                assayTableObject.tableFormat, assayTableObject.tableName);

            //manual map 
            assayTableObject.fields = assayService.fields;
            assayTableObject.tableData = assayService.tableData;
            assayTableObject.assayKey = assayService.tableData.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Select(p => p.columnHeader).FirstOrDefault().ToString();

            assayDataFields = assayService.tableData;

            if (assayDataFields != null)
                await _xmlService.DrillholeFields(fullPathnameFields, assayService.tableData, DrillholeTableType.assay, rootNameFields);

            return true;
        }

        public virtual async Task<bool> PreviewDataToImport(int limit)
        {
            List<string> fields = new List<string>();

            if (assayTableObject.xPreview == null)
            {

                var assayService = await _assayService.PreviewData(classMapper, assayTableObject.tableType, limit);

                assayTableObject.xPreview = assayService.xPreview;

                await _xmlService.DrillholeData(fullPathnameData, assayService.xPreview, DrillholeTableType.assay, DrillholeConstants._Assay + "s", rootNameData);


            }

            fields = _assayService.ReturnFields(classMapper);

            if (dataGrid.Columns.Count != fields.Count())
            {

                foreach (string name in fields)
                {
                    dataGrid.Columns.Add(name);
                }


                tableCaption = char.ToUpper(assayTableObject.tableType.ToString()[0]) +
                    assayTableObject.tableType.ToString().Substring(1);


                FillTable(); //tableType = 'Collar'

            }

            return true;
        }

        public virtual void FillTable()
        {

            dataGrid.Rows.Clear();

            var assayElements = assayTableObject.xPreview.Elements();

            foreach (var element in assayElements)
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
            string displayItems = (noOfRecords == 1 ? noOfRecords.ToString() + " " + assayTableObject.tableType +
                "          " : noOfRecords.ToString() + " " + assayTableObject.tableType + "s          ");

        }

        public override async void ImportGenericFields(bool bImport)
        {
            if (_assayService != null)
            {
                var assayService = await _assayService.ImportAllFieldsAsGeneric(classMapper, bImport);
                assayTableObject.tableData = assayService.tableData;
            }

        }

        public override async Task<bool> UpdateFieldnamesInXml()
        {
            UpdateFieldnamesXml.UpdateFieldnamesXML(assayTableObject.tableType, assayTableObject.tableData,
                assayTableObject.collarKey, assayTableObject.assayKey, assayTableObject.tableName);

            return true;
        }

        public override void SetSecondaryKey(string holeKey)
        {
            assayTableObject.assayKey = assayDataFields.Where(o => o.columnImportName == holeKey).Select(p => p.columnHeader).FirstOrDefault().ToString();
        }

        public override async Task<bool> UpdateHoleKeyInXml()
        {
            UpdateFieldnamesXml.UpdateFieldnameInXml(DrillholeConstants._Assay, "Constraint", assayTableObject.assayKey);

            return true;
        }

        public override async void UpdateFieldvalues(string previousSelection, string selectedValue, ImportTableField _searchList,
           bool bImport)
        {

            string _strSearch = _searchList.columnHeader;
            string _strName = _searchList.columnImportName;

            var assayService = _assayService.UpdateFieldvalues(previousSelection, classMapper, selectedValue,
                _strSearch, _strName);


            assayDataFields = assayService.Result.tableData;

            if (assayDataFields != null)
                await _xmlService.DrillholeFields(fullPathnameFields, assayDataFields, DrillholeTableType.assay, rootNameFields);

            if (selectedValue == DrillholeConstants.notImported)
            {

                ImportGenericFields(bImport);
            }

            surveyTableObject.surveyKey = assayDataFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Select(p => p.columnHeader).FirstOrDefault().ToString();

        }

    }
}

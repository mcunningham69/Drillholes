﻿using System;
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
using Drillholes.XML;

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

        public SurveyView(DrillholeImportFormat _tableFormat, DrillholeTableType _tableType, string _tableLocation, string _tableName, bool _savedSession, string _sessionName, string _projectLocation) 
            : base(_tableFormat, _tableType, _tableLocation, _tableName, _savedSession, _sessionName, _projectLocation)
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

        public override async Task<bool> RetrieveFieldsToMap(bool bOpen)
        {
            if (classMapper == null)
                InitialiseTableMapping();

            if (bOpen)
            {
                await RetrieveFieldsForOpenSession();
            }

            else
            {
                var surveyService = await _surveyService.GetSurveyFields(classMapper, surveyTableObject.tableLocation,
                    surveyTableObject.tableFormat, surveyTableObject.tableName);

                //manual map 
                surveyTableObject.fields = surveyService.fields;
                surveyTableObject.tableData = surveyService.tableData;
                surveyTableObject.surveyKey = surveyService.tableData.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Select(p => p.columnHeader).FirstOrDefault().ToString();

                surveyTableObject.isValid = true;

                surveyDataFields = surveyService.tableData;

                if (surveyDataFields != null)
                {
                    //create tableFields table
                    await _xmlService.DrillholeFields(fullPathnameFields, surveyService.tableData, DrillholeTableType.survey, rootNameFields);

                    if (savedSession)
                        _xmlService.DrillholeFields(projectLocation + "\\" + sessionName + ".dh", fullPathnameFields, DrillholeConstants.drillholeProject, surveyTableObject.tableType);
                }
            }
            return true;
        }

        public override async Task<bool> RetrieveFieldsForOpenSession()
        {
            var surveyFields = await _xmlService.DrillholeFields(projectLocation + "\\" + sessionName + ".dh", fullPathnameFields, DrillholeConstants.drillholeProject, DrillholeConstants.drillholeFields, surveyTableObject.tableType) as ImportTableFields;
            
            if (surveyFields.Count == 0)
            {
                var surveyService = await _surveyService.GetSurveyFields(classMapper, surveyTableObject.tableLocation,
                    surveyTableObject.tableFormat, surveyTableObject.tableName);

                surveyTableObject.fields = surveyService.fields;
                surveyTableObject.tableData = surveyService.tableData;
                surveyTableObject.surveyKey = surveyService.tableData.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Select(p => p.columnHeader).FirstOrDefault().ToString();

            }
            else
            {
                surveyTableObject.tableData = surveyFields;

                var names = surveyFields.Select(a => a.columnHeader);

                List<string> fieldNames = new List<string>();

                foreach (string field in names)
                {
                    fieldNames.Add(field);
                }

                surveyTableObject.fields = fieldNames;


                surveyTableObject.collarKey = surveyFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Select(p => p.columnHeader).FirstOrDefault().ToString();
            }
            

            surveyTableObject.tableIsValid = true;

           surveyDataFields = surveyTableObject.tableData;

            return true;

        }

        public override async Task<bool> PreviewDataToImport(int limit, bool bOpen)
        {
            List<string> fields = new List<string>();

            if (bOpen)
            {
                var surveyObject = await _xmlService.DrillholeData(projectLocation + "\\" + sessionName + ".dh", fullPathnameFields, DrillholeConstants.drillholeProject, DrillholeConstants.drillholeData, surveyTableObject.tableType);
                surveyTableObject.xPreview = surveyObject;

                if (surveyObject != null)
                {
                    if (savedSession)
                        _xmlService.DrillholeData(projectLocation + "\\" + sessionName + ".dh", fullPathnameData, DrillholeConstants.drillholeProject, surveyTableObject.tableType);
                }
            }

            //else
            //{
                if (surveyTableObject.xPreview == null)
                {

                    var surveyService = await _surveyService.PreviewData(classMapper, surveyTableObject.tableType, limit);

                    surveyTableObject.xPreview = surveyService.xPreview;

                    await _xmlService.DrillholeData(fullPathnameData, surveyService.xPreview, DrillholeTableType.survey, DrillholeConstants._Survey + "s", rootNameData);

                    if (savedSession)
                        _xmlService.DrillholeData(projectLocation + "\\" + sessionName + ".dh", fullPathnameData, DrillholeConstants.drillholeProject, surveyTableObject.tableType);
                }
          //  }

            fields = surveyTableObject.fields;

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

        public override async void FillTable()
        {
            dataGrid.Rows.Clear();

            var surveyElements = surveyTableObject.xPreview.Elements();


            foreach (var element in surveyElements)
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
            string displayItems = (noOfRecords == 1 ? noOfRecords.ToString() + " " + surveyTableObject.tableType +
                "          " : noOfRecords.ToString() + " " + surveyTableObject.tableType + "s          ");

        }

        public override async void ImportGenericFields(bool bImport, bool bOpen)
        {
            if (bOpen)
                await RetrieveFieldsForOpenSession();
            else
            {
                if (_surveyService != null)
                {
                    var surveyService = await _surveyService.ImportAllFieldsAsGeneric(classMapper, bImport);
                    surveyTableObject.tableData = surveyService.tableData;
                    surveyDataFields = surveyService.tableData;

                    if (surveyDataFields != null)
                        await _xmlService.DrillholeFields(fullPathnameFields, surveyDataFields, DrillholeTableType.survey, rootNameFields);
                }
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

        public override async void UpdateFieldvalues(string previousSelection, string selectedValue, ImportTableField _searchList,
           bool bImport, bool bOpen)
        {
            if (_surveyService == null)
                InitialiseTableMapping();

            string _strSearch = _searchList.columnHeader;
            string _strName = _searchList.columnImportName;

            var surveyService = _surveyService.UpdateFieldvalues(previousSelection, classMapper, selectedValue,
                _strSearch, _strName, surveyTableObject.tableData);

            surveyDataFields = surveyService.Result.tableData;

            if (surveyDataFields != null)
                await _xmlService.DrillholeFields(fullPathnameFields, surveyDataFields, DrillholeTableType.survey, rootNameFields);

            if (selectedValue == DrillholeConstants.notImported)
            {
                ImportGenericFields(bImport, bOpen);
            }

            surveyTableObject.surveyKey = surveyDataFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Select(p => p.columnHeader).FirstOrDefault().ToString();

        }
    }
}

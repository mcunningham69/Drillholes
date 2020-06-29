using Drillholes.Domain.Interfaces;
using Drillholes.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Drillholes.Domain.Enum;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Controls;
using Drillholes.Domain;
using System.Xml.Linq;
using Drillholes.FixErrors;


namespace Drillholes.Windows.ViewModel
{
    public class CollarEditView
    {
        public CollarEditServices _editService;
        public ICollarEdit _editValues;

        public virtual DrillholeSurveyType surveyType { get; set; }

        public ImportTableFields importCollarFields { get; set; }
        private string tableLabel { get; set; }
        public XElement xmlCollarData { get; set; }
        public ObservableCollection<ReshapedDataToEdit> EditDrillholeData { get; set; }
        public DisplayValidationMessages DisplayMessages = new DisplayValidationMessages();

        public ObservableCollection<ValidationMessages> ShowTestMessages { get { return DisplayMessages.DisplayResults; } }


        public IMapper mapper = null;

        public CollarEditView(DrillholeTableType _tableType, DrillholeSurveyType _surveyType, XElement _xmlCollarData)
        {
            surveyType = _surveyType;
            xmlCollarData = _xmlCollarData;
            tableLabel = (_tableType.ToString() + " Table").ToUpper();

            _editValues = new CollarDataEdits();

            _editService = new CollarEditServices(_editValues);
        }

        #region Actual Data

        public virtual async Task<DataTable> AddColumns(DrillholeTableType tableType, bool preview)
        {
            DataTable dataTable = new DataTable();

            DataColumn column = new DataColumn();
            column.ColumnName = "ID";
            column.ReadOnly = true;
            dataTable.Columns.Add(column);
            dataTable.Columns.Add(DrillholeConstants.holeID);
            dataTable.Columns.Add(DrillholeConstants.x);
            dataTable.Columns.Add(DrillholeConstants.y);
            dataTable.Columns.Add(DrillholeConstants.z);
            dataTable.Columns.Add(DrillholeConstants.maxDepth);

            if (surveyType == DrillholeSurveyType.collarsurvey)
            {
                dataTable.Columns.Add(DrillholeConstants.azimuth);
                dataTable.Columns.Add(DrillholeConstants.dip);
            }

            dataTable.Columns.Add("Validation");

            if (!preview)
                dataTable.Columns.Add("Description");



            return dataTable;
        }

        public async Task<RowsToEdit> CollarRow(string holeID, string testType)
        {
            List<string> fields = new List<string>();
            fields.Add(importCollarFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false)
                .Select(s => s.columnHeader).FirstOrDefault());
            fields.Add(importCollarFields.Where(o => o.columnImportName == DrillholeConstants.xName).Where(m => m.genericType == false)
                .Select(s => s.columnHeader).FirstOrDefault());
            fields.Add(importCollarFields.Where(o => o.columnImportName == DrillholeConstants.yName).Where(m => m.genericType == false)
                .Select(s => s.columnHeader).FirstOrDefault());
            fields.Add(importCollarFields.Where(o => o.columnImportName == DrillholeConstants.zName).Where(m => m.genericType == false)
                .Select(s => s.columnHeader).FirstOrDefault());
            fields.Add(importCollarFields.Where(o => o.columnImportName == DrillholeConstants.maxName).Where(m => m.genericType == false)
                .Select(s => s.columnHeader).FirstOrDefault());

            var collarValues = xmlCollarData.Elements();

            var queryHole = collarValues.Where(h => h.Element(fields[0]).Value == holeID).Select(v => v.Attribute("ID").Value).FirstOrDefault();
            var queryX = collarValues.Where(h => h.Element(fields[0]).Value == holeID).Select(v => v.Element(fields[1]).Value).FirstOrDefault();
            var queryY = collarValues.Where(h => h.Element(fields[0]).Value == holeID).Select(v => v.Element(fields[2]).Value).FirstOrDefault();
            var queryZ = collarValues.Where(h => h.Element(fields[0]).Value == holeID).Select(v => v.Element(fields[3]).Value).FirstOrDefault();
            var queryTD = collarValues.Where(h => h.Element(fields[0]).Value == holeID).Select(v => v.Element(fields[4]).Value).FirstOrDefault();

            RowsToEdit collarRow = new RowsToEdit();
            collarRow.id_col = Convert.ToInt16(queryHole);
            collarRow.holeid = holeID;
            collarRow.x = queryX;
            collarRow.y = queryY;
            collarRow.z = queryZ;
            collarRow.maxDepth = queryTD;
            collarRow.testType = testType;

            return collarRow;
        }

        public void SetDataContext(DataGrid dataEdits, DataTable dataTable)
        {
            if (dataTable != null)
            {
                if (dataTable.Columns.Count > 0)
                    dataEdits.DataContext = dataTable;
            }
        }

        #endregion

    }
}

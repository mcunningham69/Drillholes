using Drillholes.Domain;
using Drillholes.Domain.DataObject;
using Drillholes.Domain.Enum;
using Drillholes.Windows.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Drillholes.Windows.Dialogs
{
    /// <summary>
    /// Interaction logic for DrillholeEdits.xaml
    /// </summary>
    public partial class DrillholeEdits : Window
    {
        private static DrillholeSaveEdits SaveEditMode { get; set; }

        private EditEnvironment editEnvironment { get; set; }
        private static bool hasEdits { get; set; }
        public CollarTableObject collarObject { get; set; }
        public SurveyTableObject surveyObject { get; set; }
        public AssayTableObject assayObject { get; set; }
        public IntervalTableObject intervalObject { get; set; }

        public List<RowsToEdit>editedRows { get; set; }

        private CollarValidationView collarEdits { get; set; }
        private CollarEditView collarEdit { get; set; }

        private SurveyValidationView surveyEdits { get; set; }
        private SurveyEditView surveyEdit { get; set; }

        private AssayValidationView assayEdits { get; set; }
        private AssayEditView assayEdit { get; set; }

        private IntervalValidationView intervalEdits { get; set; }
        private IntervalEditView intervalEdit { get; set; }

        private bool bIgnore { get; set; }
        public int selectedIndex { get; set; }

        private static DrillholeEdits m_instance;
        public DrillholeEdits()
        {
            hasEdits = false;
            SaveEditMode = DrillholeSaveEdits.Yes;
            editEnvironment = new EditEnvironment();
            editEnvironment.StopEditSession();
            bIgnore = false;

            InitializeComponent();
        }

        public static DrillholeEdits GetInstance()
        {

            if (m_instance == null)
            {
                hasEdits = false;

                return m_instance = new DrillholeEdits();
            }


            return m_instance;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (collarObject != null)
                DisplayMessages();
        }

        private async void DisplayMessages()
        {
            if (selectedIndex == 0)
            {
                ValidateCollar();


             //   DataContext = collarEdits;


            }
            else if (selectedIndex == 1)
            {
                ValidateSurvey(true);

                DataContext = surveyEdits;

            }
            else if (selectedIndex == 2)
            {

                ValidateAssay();

                DataContext = assayEdits;

            }
            else if (selectedIndex == 3)
            {

                ValidateInterval();

                DataContext = intervalEdits;

            }
        }

        private async void ValidateCollar()
        {
            collarEdits = new CollarValidationView(DrillholeTableType.collar, collarObject.surveyType, collarObject.xPreview);
            collarEdits.importCollarFields = collarObject.tableData;
            await collarEdits.ValidateAllTables(true);

            collarEdits.ReshapeMessages(DrillholeTableType.collar);

            DataContext = collarEdits;
        }

        private async void ValidateSurvey(bool bEdit)
        {
            if (bEdit)
                ValidateCollar();

            surveyEdits = new SurveyValidationView(DrillholeTableType.survey, surveyObject.xPreview);
            surveyEdits.EditDrillholeData = collarEdits.EditDrillholeData;
            surveyEdits.importSurveyFields = surveyObject.tableData;
            surveyEdits.importCollarFields = collarObject.tableData;
            surveyEdits.xmlCollarData = collarObject.xPreview;
            await surveyEdits.ValidateAllTables(true);

            surveyEdits.ReshapeMessages(DrillholeTableType.survey);
        }

        private async void ValidateAssay()
        {
            ValidateCollar();

            assayEdits = new AssayValidationView(DrillholeTableType.assay, assayObject.xPreview);

            if (assayObject.surveyType == DrillholeSurveyType.downholesurvey)
            {
                if (surveyObject != null)
                {
                    ValidateSurvey(false);
                    assayEdits.EditDrillholeData = surveyEdits.EditDrillholeData;
                }
                else
                    assayEdits.EditDrillholeData = collarEdits.EditDrillholeData;

            }
            else
                assayEdits.EditDrillholeData = collarEdits.EditDrillholeData;

            assayEdits.importAssayFields = assayObject.tableData;
            assayEdits.importCollarFields = collarObject.tableData;
            assayEdits.xmlCollarData = collarObject.xPreview;
            await assayEdits.ValidateAllTables(true);

            assayEdits.ReshapeMessages(DrillholeTableType.assay);

        }

        private async void ValidateInterval()
        {
            ValidateCollar();

            intervalEdits = new IntervalValidationView(DrillholeTableType.interval, intervalObject.xPreview);

            if (intervalObject.surveyType == DrillholeSurveyType.downholesurvey)
            {
                if (surveyObject != null)
                {
                    ValidateSurvey(false);
                    intervalEdits.EditDrillholeData = surveyEdits.EditDrillholeData;

                    if (assayObject != null)
                    {
                        ValidateAssay();
                        intervalEdits.EditDrillholeData = assayEdits.EditDrillholeData;

                    }
                    else
                        intervalEdits.EditDrillholeData = surveyEdits.EditDrillholeData;

                }
                else
                {
                    if (assayObject != null)
                    {
                        ValidateAssay();
                        intervalEdits.EditDrillholeData = assayEdits.EditDrillholeData;

                    }
                    else
                        intervalEdits.EditDrillholeData = collarEdits.EditDrillholeData;

                }


            }
            else if (assayObject != null)
                intervalEdits.EditDrillholeData = assayEdits.EditDrillholeData;
            else
                intervalEdits.EditDrillholeData = collarEdits.EditDrillholeData;


            intervalEdits.importIntervalFields = intervalObject.tableData;
            intervalEdits.importCollarFields = collarObject.tableData;
            intervalEdits.xmlCollarData = collarObject.xPreview;
            await intervalEdits.ValidateAllTables(true);

            intervalEdits.ReshapeMessages(DrillholeTableType.interval);

        }
        private async void btnExit_Click(object sender, RoutedEventArgs e)
        {
            bool bCheck = await UnSavedEdits();

            if (!bCheck)
                return;

            m_instance = null;
            this.Close();
        }


        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveEdits();

            SaveState();
        }

        private async void SaveEdits()
        {
            switch (selectedIndex)
            {
                case 0: //collar table
                    if (collarEdit == null)
                        collarEdit = new CollarEditView(collarObject.surveyType, collarObject.xPreview, collarObject.tableData);

                    collarObject.xPreview = await collarEdit.SaveEdits(editedRows);

                    dataEdits.Visibility = Visibility.Hidden;
                    lblEdits.Visibility = Visibility.Visible;

                    collarEdits = null;
                    DataContext = null;
                    
                    ValidateCollar();
                    break;

                case 1:
                    if (surveyEdit == null)
                        surveyEdit = new SurveyEditView(surveyObject.xPreview, surveyObject.tableData);

                    await surveyEdit.SaveEdits(editedRows, bIgnore);
                    break;

                case 2:
                    if (assayEdit == null)
                        assayEdit = new AssayEditView(assayObject.xPreview, assayObject.tableData);

                    await assayEdit.SaveEdits(editedRows, bIgnore);
                    break;

                case 3:
                    if (intervalEdit == null)
                        intervalEdit = new IntervalEditView(intervalObject.xPreview, intervalObject.tableData);

                    await intervalEdit.SaveEdits(editedRows, bIgnore);
                    break;

            }

            editEnvironment.StopEditSession();

        }

        private void btnTD_Click(object sender, RoutedEventArgs e)
        {

        }

        private void tvEdit_SourceUpdated(object sender, DataTransferEventArgs e)
        {

        }

        private async void tvEdit_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            List<RowsToEdit> _edits = new List<RowsToEdit>();

            if (tvEdit.SelectedItem == null)
                return;

            var value = tvEdit.SelectedItem.GetType();

            if (value.Name == "GroupByTest")
            {
                var edits = tvEdit.SelectedItem as GroupByTest;

                var testResults = edits.TestFields;

                foreach (var testResult in testResults)
                {
                    var rows = testResult.TableData;

                    foreach(var row in rows)
                        _edits.Add(row);
                }
            }
            else if (value.Name == "GroupByTestField")
            {
                var edits = tvEdit.SelectedItem as GroupByTestField;

                var testResults = edits;

                if (testResults.TableData.Count == 1)
                {
                    var edit = testResults.TableData[0] as RowsToEdit;
                    _edits.Add(edit);
                }
                else
                {
                    foreach(var val in testResults.TableData)
                        _edits.Add(val);
                }

            }
            else if (value.Name == "RowsToEdit")
            {
                var edit = tvEdit.SelectedItem as RowsToEdit;

                _edits.Add(edit);
            }
            else
                return;

            //if not in editmode go straight on
            if (editEnvironment.editSession == DrillholeEditSession.Started) //check if in edit mode
            {
                bool checkRow = false;

                if (editedRows != null)
                {
                    //check if row exists
                    checkRow = await CheckIfRowExists(_edits); //if in edit mode, check if hole has already been edited and exists in list of modified data
                }

                if (checkRow)
                {
                    var values = editedRows.Where(h => h.holeid == _edits[0].holeid).ToList();
                    values[0].testType = _edits[0].testType;
                    values[0].validationTest = _edits[0].validationTest;
                    values[0].Description = _edits[0].Description;

                    ReturnRows(values, true); //if it exists then modify values
                }
                else
                    ReturnRows(_edits,  false); //if it doesn't exist run as normal

            }
            else
            {
                ReturnRows(_edits,  false); //run normal procedure to populate row from error message
            }

        }

        private async Task<bool>CheckIfRowExists(List<RowsToEdit> _edits)
        {
            string holeID = _edits[0].holeid;

            int holeCount = editedRows.Where(h => h.holeid == _edits[0].holeid).Count();

            if (holeCount > 0)
                return true;
            else
                return false;
        }

        private async void ReturnRows(List<RowsToEdit> _edits,  bool bModify)
        {
            DataTable collarTable = null;
            DataTable previewTable = null;
            DataTable surveyTable = null;
            DataTable assayTable = null;
            DataTable intervalTable = null;

            string holeID = _edits[0].holeid;
            string testType = _edits[0].testType;

            if (_edits[0].TableType == DrillholeTableType.collar)
            {
                if (bModify)
                    collarTable = await collarEdits.ModifyGridValues(_edits, DrillholeTableType.collar, false);
                else
                    collarTable = await collarEdits.PopulateGridValues(_edits, DrillholeTableType.collar, false);

                collarEdits.SetDataContext(dataEdits, collarTable);

                dataCollar.Visibility = Visibility.Hidden;
                lblPreview.Visibility = Visibility.Visible;

                dataEdits.Visibility = Visibility.Visible;
                lblEdits.Visibility = Visibility.Hidden;
            }
            else if (_edits[0].TableType == DrillholeTableType.survey)
            {

                surveyTable = surveyEdits.PopulateGridValues(_edits, DrillholeTableType.survey, false).Result;  //TODO => override

                _edits.Clear();

                //Get CollarRow
                _edits.Add(collarEdits.CollarRow(holeID, testType).Result);
                previewTable = await collarEdits.PopulateGridValues(_edits, DrillholeTableType.collar, true);

                collarEdits.SetDataContext(dataCollar, previewTable);
                surveyEdits.SetDataContext(dataEdits, surveyTable);

                dataCollar.Visibility = Visibility.Visible;
                lblPreview.Visibility = Visibility.Hidden;
            }
            else if (_edits[0].TableType == DrillholeTableType.assay)
            {
                assayTable = await assayEdits.PopulateGridValues(_edits, DrillholeTableType.assay, false); //TODO override

                _edits.Clear();

                //Get CollarRow
                _edits.Add(collarEdits.CollarRow(holeID, testType).Result);
                previewTable = await collarEdits.PopulateGridValues(_edits, DrillholeTableType.collar, true);

                collarEdits.SetDataContext(dataCollar, previewTable);
                assayEdits.SetDataContext(dataEdits, assayTable);

                dataCollar.Visibility = Visibility.Visible;
                lblPreview.Visibility = Visibility.Hidden;
            }

            else if (_edits[0].TableType == DrillholeTableType.interval)
            {
                intervalTable = await intervalEdits.PopulateGridValues(_edits, DrillholeTableType.interval, false); //TODO override
                intervalEdits.SetDataContext(dataEdits, intervalTable);

                _edits.Clear();

                //Get CollarRow
                _edits.Add(collarEdits.CollarRow(holeID, testType).Result);
                previewTable = await collarEdits.PopulateGridValues(_edits, DrillholeTableType.collar, true);

                collarEdits.SetDataContext(dataCollar, previewTable);

                dataCollar.Visibility = Visibility.Visible;
                lblPreview.Visibility = Visibility.Hidden;
            }
            foreach (var column in dataEdits.Columns)
            {
                string name = column.Header.ToString();

                //make readonly columns
                if (name == "ID" || name == "Message")
                {
                    column.IsReadOnly = true;
                }

            }
        }

        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            bool bCheck = await UnSavedEdits();

            if (!bCheck)
                return;

            m_instance = null;

        }

        #region Editting Data
        private async void SaveState()
        {
            if (editEnvironment.hasEdits)
            {
                btnSave.IsEnabled = true;
                editEnvironment.StartEditSession();
            }
            else
            {
                btnSave.IsEnabled = false;
                editEnvironment.StopEditSession();

            }
        }
        private async void btnStartEdits_Click(object sender, RoutedEventArgs e)
        {

            //btnTD.IsEnabled = true;
            btnStartEdits.Visibility = Visibility.Hidden;
            btnStopEdits.Visibility = Visibility.Visible;

            dataEdits.IsReadOnly = false;
            dataCollar.IsReadOnly = false;
            editEnvironment.StartEditSession();

            DataContext = collarEdits;
        }

        private async void btnStopEdits_Click(object sender, RoutedEventArgs e)
        {

            bool bCheck = await UnSavedEdits();

            if (!bCheck)
                return;


            editEnvironment.StopEditSession();

            btnSave.IsEnabled = false;
            btnStartEdits.Visibility = Visibility.Visible;
            btnStopEdits.Visibility = Visibility.Hidden;
            dataEdits.IsReadOnly = true;
            dataCollar.IsReadOnly = true;

        }

        private async Task<bool> UnSavedEdits()
        {
            if (editEnvironment.hasEdits)
            {

                MessageBoxResult toSaveEdits = MessageBox.Show("Save Edits?", "Edits", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                if (MessageBoxResult.Cancel == toSaveEdits)
                    return false;
                else if (MessageBoxResult.Yes == toSaveEdits)
                {
                    SaveEdits();

                    editedRows.Clear();
                }
                else
                {
                    //TODO to refresh row
                    editedRows.Clear();

                }
            }

            editEnvironment.StopEditSession();

            return true;
        }

        //activated when focus leaves last cell (in edit mode)
        private async void dataEdits_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            //get the row of data (except ignore)
            DataRowView edits = e.Row.DataContext as DataRowView;

            RowsToEdit row = null;
            var array = edits.Row.ItemArray;

            if (editedRows == null)
            {
                editedRows = new List<RowsToEdit>();
            }

            int idCount = editedRows.Where(a => a.id_col.ToString() == array[1].ToString()).Count();

            if (idCount == 0)
            {
                //as above
                row = await ReturnRow(array);
                editedRows.Add(row);

            }
            else
            {
                //if row exists return and check which values need to modified from Switch below
                row = editedRows.Where(a => a.id_col.ToString() == array[1].ToString()).FirstOrDefault();

            }

                var changeTo = e.EditingElement as TextBox;

                switch (e.Column.Header.ToString())
                {
                    case "Ignore":
                        CheckBox bState = e.EditingElement as CheckBox;
                        bIgnore = (bool)bState.IsChecked;
                        row.Ignore = bIgnore;
                        break;
                    case DrillholeConstants.holeID:
                        row.holeid = changeTo.Text;
                        break;
                    case DrillholeConstants.x:
                        row.x = changeTo.Text;
                        break;
                    case DrillholeConstants.y:
                        row.y = changeTo.Text;
                        break;
                    case DrillholeConstants.z:
                        row.z = changeTo.Text;
                        break;
                    case DrillholeConstants.maxDepth:
                        row.maxDepth = changeTo.Text;
                        break;
                    case DrillholeConstants.azimuth:
                        row.azimuth = changeTo.Text;
                        break;
                    case DrillholeConstants.dip:
                        row.dip = changeTo.Text;
                        break;
                    case DrillholeConstants.survDistance:
                        row.distance = changeTo.Text;
                        break;
                    case DrillholeConstants.distFrom:
                        row.distanceFrom = changeTo.Text;
                        break;

                    case DrillholeConstants.distTo:
                        row.distanceTo = changeTo.Text;
                        break;
                }
            // }

            editEnvironment.hasEdits = true;

            btnSave.IsEnabled = true;


        }

        private async Task<RowsToEdit>ReturnRow(object[] array)
        {
            RowsToEdit row = null;

            if (array[0].ToString() == "False")
                bIgnore = false;
            else
                bIgnore = true;

            if (selectedIndex == 0)
            {
                row = new RowsToEdit()
                {
                    Ignore = bIgnore,
                    id_col = Convert.ToInt32(array[1]),
                    holeid = array[2].ToString(),
                    x = array[3].ToString(),
                    y = array[4].ToString(),
                    z = array[5].ToString(),
                    maxDepth = array[6].ToString()

                };

                if (DrillholeSurveyType.collarsurvey == collarObject.surveyType)
                {
                    row.azimuth = array[6].ToString();
                    row.dip = array[7].ToString();
                }
            }
            else if (selectedIndex == 1)
            {
                 row = new RowsToEdit()
                {
                    Ignore = bIgnore,
                    id_sur = Convert.ToInt32(array[1]),
                    holeid = array[2].ToString(),
                    distance = array[3].ToString(),
                    azimuth = array[4].ToString(),
                    dip = array[5].ToString()

                };

            }
            else if (selectedIndex == 2)
            {
                row = new RowsToEdit()
                {
                    Ignore = bIgnore,
                    id_ass = Convert.ToInt32(array[1]),
                    holeid = array[2].ToString(),
                    distanceFrom = array[3].ToString(),
                    distanceTo = array[4].ToString()
                };

            }
            else if (selectedIndex == 3)
            {
                row = new RowsToEdit()
                {
                    Ignore = bIgnore,
                    id_int = Convert.ToInt32(array[1]),
                    holeid = array[2].ToString(),
                    distanceFrom = array[3].ToString(),
                    distanceTo = array[4].ToString()
                };

            }

            return row;
        }

        private void dataEdits_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "Ignore")
            {
                DataGridCheckBoxColumn checkBoxColumn = new DataGridCheckBoxColumn();
                checkBoxColumn.Header = e.Column.Header;
                checkBoxColumn.Binding = new Binding(e.PropertyName);
                checkBoxColumn.IsThreeState = true;

                // Replace the auto-generated column with the checkBoxColumn.
                e.Column = checkBoxColumn;
            }
          
        }

      
    }
    #endregion

}

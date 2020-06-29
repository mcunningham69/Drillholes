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
        private static bool editMode { get; set; }
        private static DrillholeSaveEdits SaveEditMode { get; set; }

        private static bool hasEdits { get; set; }
        public CollarTableObject collarObject { get; set; }
        public SurveyTableObject surveyObject { get; set; }
        public AssayTableObject assayObject { get; set; }
        public IntervalTableObject intervalObject { get; set; }

        public List<RowsToEdit>editedRows { get; set; }

        private CollarValidationView collarEdits { get; set; }
        private CollarEditView collarEdit { get; set; }

        private SurveyValidationView surveyEdits { get; set; }
        private AssayValidationView assayEdits { get; set; }
        private IntervalValidationView intervalEdits { get; set; }

        public int selectedIndex { get; set; }

        private static DrillholeEdits m_instance;
        public DrillholeEdits()
        {
            editMode = false;
            hasEdits = false;
            SaveEditMode = DrillholeSaveEdits.Yes;

            InitializeComponent();
        }

        public static DrillholeEdits GetInstance()
        {

            if (m_instance == null)
            {
                editMode = false;
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


                DataContext = collarEdits;


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
            //collarEdits = new CollarValidationView(DrillholeTableType.collar, collarObject.surveyType, collarObject.xPreview);
            //collarEdits.importCollarFields = collarObject.tableData;
            //await collarEdits.ValidateAllTables(true);

            //collarEdits.ReshapeMessages(DrillholeTableType.collar);
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
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
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
            hasEdits = false;
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

                var edit = testResults.TableData[0] as RowsToEdit;
                _edits.Add(edit);

            }
            else if (value.Name == "RowsToEdit")
            {
                var edit = tvEdit.SelectedItem as RowsToEdit;

                _edits.Add(edit);
            }
            else
                return;

            DataTable collarTable = null;
            DataTable previewTable = null;
            DataTable surveyTable = null;
            DataTable assayTable = null;
            DataTable intervalTable = null;

            string holeID = _edits[0].holeid;
            string testType = _edits[0].testType;

            if (_edits[0].TableType == DrillholeTableType.collar)
            {
                collarTable = await collarEdits.PopulateGridValues(_edits, DrillholeTableType.collar, false);

                collarEdits.SetDataContext(dataEdits, collarTable);

                dataCollar.Visibility = Visibility.Hidden;
                lblPreview.Visibility = Visibility.Visible;
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



        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            m_instance = null;
            hasEdits = false;
            editMode = false;
        }

        private async void SaveState()
        {
            if (hasEdits)
            {
                btnSave.IsEnabled = true;
            }
            else
            {
                btnSave.IsEnabled = false;
            }
        }
        private void btnStartEdits_Click(object sender, RoutedEventArgs e)
        {
            if (editMode)
            {
                if (hasEdits)
                {
                    MessageBoxResult toSaveEdits = MessageBox.Show("Save Edits?", "Edits", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                    if (MessageBoxResult.Cancel == toSaveEdits)
                        return;
                    else if (MessageBoxResult.Yes == toSaveEdits)
                    {
                        SaveEdits();
                    }

                }

                btnStartEdits.Content = "Start Editing";
                btnSave.IsEnabled = false;
                btnReplaceAll.IsEnabled = false;
                btnTD.IsEnabled = false;
                dataEdits.IsReadOnly = true;
                dataCollar.IsReadOnly = true;

                editMode = false;
                hasEdits = false;

                
            }
            else
            {
                btnStartEdits.Content = "Stop Editing";
                btnReplaceAll.IsEnabled = true;
                btnTD.IsEnabled = true;
                dataEdits.IsReadOnly = false;
                dataCollar.IsReadOnly = false;
                editMode = true;
            }
        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            //simulate adding data
            hasEdits = true;
            SaveState();
        }

        private void dataEdits_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            //if (e.EditAction == DataGridEditAction.Commit)
            //{
            //    if (SaveEditMode == DrillholeSaveEdits.Yes)
            //    {
            //        //check either to add new row or modify existing row

            //        hasEdits = false;
            //        btnSave.IsEnabled = false;
            //    }
            //    else if (SaveEditMode == DrillholeSaveEdits.No)
            //    {

            //        hasEdits = false;
            //        btnSave.IsEnabled = false;
            //    }
            //    else
            //    {

            //    }

            //    var edits = e.Row.DataContext;
            //}

        }

        private void dataEdits_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {

        }



        private async void dataEdits_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {

            if (e.Column.Header.ToString() == "Ignore")
            {
                CheckBox bState = e.EditingElement as CheckBox;

                DataRowView edits = e.Row.DataContext as DataRowView;

                RowsToEdit row = null;
                var array = edits.Row.ItemArray;

                if (editedRows == null)
                {
                    editedRows = new List<RowsToEdit>();
                    row = await ReturnRow((bool)bState.IsChecked, array);
                    editedRows.Add(row);
                }
                else
                {
                    int holeCount = editedRows.Where(a => a.holeid == array[1].ToString()).Count();

                    if (holeCount == 0)
                    {
                        row = await ReturnRow((bool)bState.IsChecked, array);
                        editedRows.Add(row);

                    }
                    else
                    {
                        row = editedRows.Where(a => a.holeid == array[1].ToString()).FirstOrDefault();

                        if (row != null)
                            row.Ignore = (bool)bState.IsChecked;
                    }

                }
            }

            hasEdits = true;
            btnSave.IsEnabled = true;


        }

        private async Task<RowsToEdit>ReturnRow(bool bChecked, object[] array)
        {
            RowsToEdit row = new RowsToEdit()
            {
                Ignore = bChecked,
                id_col = Convert.ToInt32(array[0]),
                holeid = array[1].ToString(),
                x = array[2].ToString(),
                y = array[3].ToString(),
                z = array[4].ToString(),
                maxDepth = array[5].ToString()

            };

            if (DrillholeSurveyType.collarsurvey == collarObject.surveyType)
            {
                row.azimuth = array[6].ToString();
                row.dip = array[7].ToString();
            }

            return row;
        }
    }

 
}

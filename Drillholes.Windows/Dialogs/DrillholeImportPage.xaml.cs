using Drillholes.Domain.DataObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Drillholes.Domain.Enum;
using System.Xml.Linq;
using Drillholes.Domain;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Drillholes.Domain.Services;
using Drillholes.Domain.Interfaces;

namespace Drillholes.Windows.Dialogs
{
    /// <summary>
    /// Interaction logic for DrillholeImportPage.xaml
    /// </summary>
    public partial class DrillholeImportPage : Page
    {
        private DrillholeSurveyType surveyType { get; set; }
        private string tableCollarLocation { get; set; }
        private string tableCollarName { get; set; }
        private string tableSurveyLocation { get; set; }
        private string tableSurveyName { get; set; }
        private string tableAssayLocation { get; set; }
        private string tableAssayName { get; set; }
        private string tableIntervalLocation { get; set; }
        private string tableIntervalName { get; set; }
        private string tableContinuousLocation { get; set; }
        private string tableContinuousName { get; set; }
        DrillholeImportFormat collarTableFormat { get; set; }
        DrillholeImportFormat surveyTableFormat { get; set; }
        DrillholeImportFormat intervalTableFormat { get; set; }
        DrillholeImportFormat continuousTableFormat { get; set; }

        DrillholeImportFormat assayTableFormat { get; set; }

        bool bEnableSurvey { get; set; }
        bool bEnableAssay { get; set; }
        bool bEnableContinuous { get; set; }
        bool bEnableInterval { get; set; }

        public List<DrillholeTable> tables { get; set; }

        #region Setup Views
        public ViewModel.CollarView collarPreviewModel { get; set; }
        public ViewModel.SurveyView surveyPreviewModel { get; set; }

        public ViewModel.AssayView assayPreviewModel { get; set; }

        public ViewModel.IntervalView intervalPreviewModel { get; set; }

        public ViewModel.ContinuousView continuousPreviewModel { get; set; }

        public bool savedSession { get; set; }
        public bool openSession { get; set; }
        private string projectSession { get; set; }
        private string projectLocation { get; set; }

        private string xmlProjectFile { get; set; }

        private bool[] bFirstRun { get; set; }

        #endregion


        public DrillholeImportPage()
        {
            InitializeComponent();
        }

        public DrillholeImportPage(List<DrillholeTable> _classes, bool _savedSession, string _xmlProjectFile, string _projectSession, string _projectLocation)
        {
            InitializeComponent();

            bFirstRun = new bool[5];
            for(int i =0; i < 5; i++)
            {
                bFirstRun[i] = false;
            }

            tables = _classes;
            savedSession = _savedSession;
            xmlProjectFile = _xmlProjectFile;
            projectSession = _projectSession;
            projectLocation = _projectLocation;

            var tabItems = _tabcontrol.Items;

            if (_classes == null)
                return;

            foreach (var importTable in _classes)
            {
                if (importTable.tableType == DrillholeTableType.collar)
                {
                    tableCollarLocation = _classes.Where(i => i.tableType == DrillholeTableType.collar)
                .Select(s => s.tableLocation).SingleOrDefault();
                    collarTableFormat = _classes.Where(i => i.tableType == DrillholeTableType.collar)
                        .Select(s => s.tableFormat).SingleOrDefault();
                    tableCollarName = _classes.Where(i => i.tableType == DrillholeTableType.collar)
                        .Select(s => s.tableName).SingleOrDefault();

                    SetModelViews(DrillholeTableType.collar);

                }

                else if (importTable.tableType == DrillholeTableType.survey)
                {
                    tableSurveyLocation = _classes.Where(i => i.tableType == DrillholeTableType.survey).Select(s => s.tableLocation).SingleOrDefault();
                    surveyTableFormat = _classes.Where(i => i.tableType == DrillholeTableType.survey).Select(s => s.tableFormat).SingleOrDefault();
                    tableSurveyName = _classes.Where(i => i.tableType == DrillholeTableType.survey).Select(s => s.tableName).SingleOrDefault();

                    foreach (TabItem item in tabItems)
                    {
                        if (item.Header.ToString() == "Survey")
                            item.IsEnabled = true;
                    }

                    SetModelViews(DrillholeTableType.survey); //collar and survey same

                }
                else if (importTable.tableType == DrillholeTableType.assay)
                {
                    tableAssayLocation = _classes.Where(i => i.tableType == DrillholeTableType.assay).Select(s => s.tableLocation).SingleOrDefault();
                    assayTableFormat = _classes.Where(i => i.tableType == DrillholeTableType.assay).Select(s => s.tableFormat).SingleOrDefault();
                    tableAssayName = _classes.Where(i => i.tableType == DrillholeTableType.assay).Select(s => s.tableName).SingleOrDefault();

                    foreach (TabItem item in tabItems)
                    {
                        if (item.Header.ToString() == "Assay")
                            item.IsEnabled = true;
                    }

                    SetModelViews(DrillholeTableType.assay);

                }
                else if (importTable.tableType == DrillholeTableType.interval)
                {
                    tableIntervalLocation = _classes.Where(i => i.tableType == DrillholeTableType.interval).Select(s => s.tableLocation).SingleOrDefault();
                    intervalTableFormat = _classes.Where(i => i.tableType == DrillholeTableType.interval).Select(s => s.tableFormat).SingleOrDefault();
                    tableIntervalName = _classes.Where(i => i.tableType == DrillholeTableType.interval).Select(s => s.tableName).SingleOrDefault();

                    foreach (TabItem item in tabItems)
                    {
                        if (item.Header.ToString() == "Interval")
                            item.IsEnabled = true;
                    }

                    SetModelViews(DrillholeTableType.interval);
                }

                else if (importTable.tableType == DrillholeTableType.continuous)
                {
                    tableContinuousLocation = _classes.Where(i => i.tableType == DrillholeTableType.continuous).Select(s => s.tableLocation).SingleOrDefault();
                    continuousTableFormat = _classes.Where(i => i.tableType == DrillholeTableType.continuous).Select(s => s.tableFormat).SingleOrDefault();
                    tableContinuousName = _classes.Where(i => i.tableType == DrillholeTableType.continuous).Select(s => s.tableName).SingleOrDefault();

                    foreach (TabItem item in tabItems)
                    {
                        if (item.Header.ToString() == "Continuous")
                            item.IsEnabled = true;
                    }

                    SetModelViews(DrillholeTableType.continuous);
                }

            }

        }
       
        private void SetModelViews(DrillholeTableType tableType)
        {
            switch (tableType)
            {
                case DrillholeTableType.collar:
                    {
                        collarPreviewModel = new ViewModel.CollarView(collarTableFormat, DrillholeTableType.collar, tableCollarLocation, tableCollarName, savedSession, projectSession,projectLocation);
                        break;
                    }
                case DrillholeTableType.survey:
                    surveyPreviewModel = new ViewModel.SurveyView(surveyTableFormat, DrillholeTableType.survey, tableSurveyLocation, tableSurveyName, savedSession, projectSession, projectLocation);
                    bEnableSurvey = true;
                    break;
                case DrillholeTableType.assay:
                    assayPreviewModel = new ViewModel.AssayView(assayTableFormat, DrillholeTableType.assay, tableAssayLocation, tableAssayName, savedSession, projectSession, projectLocation);
                    bEnableAssay = true;

                    break;
                case DrillholeTableType.interval:
                    intervalPreviewModel = new ViewModel.IntervalView(intervalTableFormat, DrillholeTableType.interval, tableIntervalLocation, tableIntervalName, savedSession, projectSession, projectLocation);
                    bEnableInterval = true;
                    break;

                case DrillholeTableType.continuous:
                    continuousPreviewModel = new ViewModel.ContinuousView(continuousTableFormat, DrillholeTableType.continuous, tableContinuousLocation, tableContinuousName, savedSession, projectSession, projectLocation);
                    bEnableContinuous = true;
                    break;

            }
        }

        public async Task<bool> ImportColumns(DrillholeTableType tableType, bool isChecked)
        {

            if (tableType == DrillholeTableType.collar)
            {
                bFirstRun[0] = true;
                collarPreviewModel.ImportAllColumns(isChecked);
                collarPreviewModel.ImportGenericFields(isChecked, savedSession);

            }
            else if (tableType == DrillholeTableType.survey)
            {
                if (surveyPreviewModel.surveyDataFields != null)
                {
                    surveyPreviewModel.ImportAllColumns(isChecked);
                    surveyPreviewModel.ImportGenericFields(isChecked, savedSession);
                    bFirstRun[1] = true;
                }
            }
            else if (tableType == DrillholeTableType.assay)
            {
                if (assayPreviewModel.assayDataFields != null)
                {
                    assayPreviewModel.ImportAllColumns(isChecked);
                    assayPreviewModel.ImportGenericFields(isChecked, savedSession);
                    bFirstRun[2] = true;
                }

            }
            else if (tableType == DrillholeTableType.interval)
            {
                if (intervalPreviewModel.intervalDataFields != null)
                {
                    bFirstRun[3] = true;
                    intervalPreviewModel.ImportAllColumns(isChecked);
                    intervalPreviewModel.ImportGenericFields(isChecked, savedSession);
                }

            }
            else if (tableType == DrillholeTableType.continuous)
            {
                if (continuousPreviewModel.continuousDataFields != null)
                {
                    bFirstRun[4] = true;
                    continuousPreviewModel.ImportAllColumns(isChecked);
                    continuousPreviewModel.ImportGenericFields(isChecked, savedSession);
                }
            }

            return true;
        }

        private async void _tabcontrol_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (collarPreviewModel == null)
                return;

            int _tabIndex = _tabcontrol.SelectedIndex;


            if (_tabIndex == -1)
                return;

            int nLimit = -99;

            //get preferences from XML
            DrillholePreferences preferences = await CheckPreferencesFromXml();


            try
            {
                if (_tabIndex == 0) //COLLAR
                {
                    if (collarPreviewModel.collarTableObject.tableData == null)
                    {
                        await LoadCollarTableAndFields(nLimit);
                    }

                    collarPreviewModel.ImportAllColumns(preferences.ImportCollarColumns);

                    if (preferences.ImportCollarColumns)
                    {
                        if (!bFirstRun[0])
                        {
                            collarPreviewModel.ImportGenericFields(true, savedSession);

                            bFirstRun[0] = true;
                        }
                    }

                    DataContext = collarPreviewModel;
                    collarPreviewModel.SetDataContext(dataPreview);

                }
                else if (_tabIndex == 1)//survey
                {

                    if (surveyPreviewModel.surveyTableObject.tableData == null)
                    {
                        await LoadSurveyTableAndFields(nLimit);

                    }

                    surveyPreviewModel.ImportAllColumns(preferences.ImportSurveyColumns);

                    if (preferences.ImportSurveyColumns)
                    {
                        if (!bFirstRun[1])
                        {
                            surveyPreviewModel.ImportGenericFields(true, savedSession);

                            bFirstRun[1] = true;
                        }
                    }

                    DataContext = surveyPreviewModel;
                    surveyPreviewModel.SetDataContext(dataPreview);

                }
                else if (_tabIndex == 2) //assay
                {
                    if (assayPreviewModel.assayTableObject.tableData == null)
                    {
                        await LoadAssayTableAndFields(nLimit);
                    }

                    assayPreviewModel.ImportAllColumns(preferences.ImportAssayColumns);

                    if (preferences.ImportAssayColumns)
                    {
                        if (!bFirstRun[2])
                        {
                            assayPreviewModel.ImportGenericFields(true, savedSession);

                            bFirstRun[2] = true;
                        }
                    }
                    DataContext = assayPreviewModel;
                    assayPreviewModel.SetDataContext(dataPreview);
                }
                else if (_tabIndex == 3)//interval
                {
                    if (intervalPreviewModel.intervalTableObject.tableData == null)
                    {
                        await LoadIntervalTableAndFields(nLimit);
                    }

                    intervalPreviewModel.ImportAllColumns(preferences.ImportIntervalColumns);
                    if (preferences.ImportIntervalColumns)
                    {
                        if (!bFirstRun[3])
                        {
                            intervalPreviewModel.ImportGenericFields(true, savedSession);

                            bFirstRun[3] = true;
                        }
                    }

                    DataContext = intervalPreviewModel;
                    intervalPreviewModel.SetDataContext(dataPreview);

                }
                else if (_tabIndex == 4)//interval
                {
                    if (continuousPreviewModel.continuousTableObject.tableData == null)
                    {
                        await LoadContinuousTableAndFields(nLimit);
                    }

                    continuousPreviewModel.ImportAllColumns(preferences.ImportContinuousColumns);
                    if (preferences.ImportContinuousColumns)
                    {
                        if (!bFirstRun[4])
                        {
                            continuousPreviewModel.ImportGenericFields(true, savedSession);

                            bFirstRun[4] = true;
                        }
                    }
                    DataContext = continuousPreviewModel;
                    continuousPreviewModel.SetDataContext(dataPreview);

                }
            }

            catch (Exception ex)
            {
                string strMessage = ex.Message;
            }
            finally
            {
                //house keeping
               // SetSurveyType(false);
            }
        }

        public async void RefreshPreviewData()
        {
            if (_tabcontrol.SelectedIndex == 0)
                await collarPreviewModel.PreviewDataToImport(-99, openSession);
            else if (_tabcontrol.SelectedIndex == 1)
            {
                await surveyPreviewModel.PreviewDataToImport(-99, openSession);
            }
            else if (_tabcontrol.SelectedIndex == 2)
            {
                await assayPreviewModel.PreviewDataToImport(-99, openSession);
            }
            else if (_tabcontrol.SelectedIndex == 3)
            {
                await intervalPreviewModel.PreviewDataToImport(-99, openSession);
            }
            else if (_tabcontrol.SelectedIndex == 4)
            {
                await continuousPreviewModel.PreviewDataToImport(-99, openSession);
            }

        }
        #region Load Tables
        private async Task<bool> LoadCollarTableAndFields(int nLimit)
        {
            //returns preferences on first read
            CheckPreferencesFromXml();

            await collarPreviewModel.RetrieveFieldsToMap(openSession); //on start, holeIDName set to empty string

            await collarPreviewModel.PreviewDataToImport(nLimit, openSession); //50 is the limit of records to preview

            chkSkip.IsEnabled = false;
            chkSkip.IsChecked = false;

            return true;

        }



        private async Task<bool> LoadSurveyTableAndFields(int nLimit)
        {
            if (surveyPreviewModel.surveyTableObject.tableData == null)
            {
                await surveyPreviewModel.RetrieveFieldsToMap(openSession); //on start, holeIDName set to empty string
                surveyPreviewModel.surveyTableObject.collarKey = collarPreviewModel.collarTableObject.collarKey;
                await surveyPreviewModel.PreviewDataToImport(nLimit, openSession); //50 is the limit of records to preview

            }

            chkSkip.IsEnabled = true;
            chkSkip.IsChecked = surveyPreviewModel.skipTable;

            return true;
        }


        private async Task<bool> LoadAssayTableAndFields(int nLimit)
        {
            DrillholePreferences preferences = await CheckPreferencesFromXml();

            if (assayPreviewModel.assayTableObject.tableData == null)
            {
                if (preferences.surveyType == DrillholeSurveyType.vertical)
                    assayPreviewModel.assayTableObject.surveyType = DrillholeSurveyType.vertical;
                else if (preferences.surveyType == DrillholeSurveyType.collarsurvey)
                    assayPreviewModel.assayTableObject.surveyType = DrillholeSurveyType.collarsurvey;
                else
                    assayPreviewModel.assayTableObject.surveyType = DrillholeSurveyType.downholesurvey;

                await assayPreviewModel.RetrieveFieldsToMap(openSession); //on start, holeIDName set to empty string
                assayPreviewModel.assayTableObject.collarKey = collarPreviewModel.collarTableObject.collarKey;

                if (bEnableSurvey)
                    assayPreviewModel.assayTableObject.surveyKey = surveyPreviewModel.surveyTableObject.surveyKey;

                await assayPreviewModel.PreviewDataToImport(nLimit, openSession); //50 is the limit of records to preview
            }

            return true;
        }

        private async Task<bool> LoadIntervalTableAndFields(int nLimit)
        {
            DrillholePreferences preferences = await CheckPreferencesFromXml();

            if (intervalPreviewModel.intervalTableObject.tableData == null)
            {
                if (preferences.surveyType == DrillholeSurveyType.vertical)
                    intervalPreviewModel.intervalTableObject.surveyType = DrillholeSurveyType.vertical;
                else if (preferences.surveyType == DrillholeSurveyType.collarsurvey)
                    intervalPreviewModel.intervalTableObject.surveyType = DrillholeSurveyType.collarsurvey;
                else
                    intervalPreviewModel.intervalTableObject.surveyType = DrillholeSurveyType.downholesurvey;

                await intervalPreviewModel.RetrieveFieldsToMap(openSession); //on start, holeIDName set to empty string
                intervalPreviewModel.intervalTableObject.collarKey = collarPreviewModel.collarTableObject.collarKey;

                if (bEnableSurvey)
                    intervalPreviewModel.surveyTableObject.surveyKey = surveyPreviewModel.surveyTableObject.surveyKey;

                if (bEnableAssay && assayPreviewModel.assayTableObject.assayKey != "")
                    intervalPreviewModel.assayTableObject.assayKey = assayPreviewModel.assayTableObject.assayKey;

                await intervalPreviewModel.PreviewDataToImport(nLimit, openSession); //50 is the limit of records to preview

            }

            return true;
        }

        private async Task<bool> LoadContinuousTableAndFields(int nLimit)
        {
            DrillholePreferences preferences = await CheckPreferencesFromXml();

            if (continuousPreviewModel.continuousTableObject.tableData == null)
            {
                if (preferences.surveyType == DrillholeSurveyType.vertical)
                    continuousPreviewModel.intervalTableObject.surveyType = DrillholeSurveyType.vertical;
                else if (preferences.surveyType == DrillholeSurveyType.collarsurvey)
                    continuousPreviewModel.intervalTableObject.surveyType = DrillholeSurveyType.collarsurvey;
                else
                    continuousPreviewModel.intervalTableObject.surveyType = DrillholeSurveyType.downholesurvey;

                await continuousPreviewModel.RetrieveFieldsToMap(openSession); //on start, holeIDName set to empty string
                continuousPreviewModel.intervalTableObject.collarKey = collarPreviewModel.collarTableObject.collarKey;

                if (bEnableSurvey)
                    continuousPreviewModel.surveyTableObject.surveyKey = surveyPreviewModel.surveyTableObject.surveyKey;

                if (bEnableAssay && assayPreviewModel.assayTableObject.assayKey != "")
                    continuousPreviewModel.assayTableObject.assayKey = assayPreviewModel.assayTableObject.assayKey;

                if (bEnableInterval && intervalPreviewModel.intervalTableObject.intervalKey != "")
                    continuousPreviewModel.intervalTableObject.intervalKey = intervalPreviewModel.intervalTableObject.intervalKey;

                await continuousPreviewModel.PreviewDataToImport(nLimit, openSession); //50 is the limit of records to preview
            }

            return true;
        }
        #endregion

        private async Task<DrillholePreferences> CheckPreferencesFromXml()
        {
            return await collarPreviewModel.ReadXmlPreferences();
        }

        //private async void UpdateImportAllStatus() //TODO this maybe redundant??
        //{
        //    //TODO - have individual import all for each table. At moment it does it for all 
        //    DrillholePreferences preferences = await CheckPreferencesFromXml();


        //    string fullPathName = "";
        //    if (savedSession)
        //        fullPathName = await XmlDefaultPath.GetProjectPathAndFilename(DrillholeConstants.drillholePref, "alltables", projectSession, projectLocation);
        //    else
        //        fullPathName = await XmlDefaultPath.GetFullPathAndFilename(DrillholeConstants.drillholePref, "allTables");

        //    if (_tabcontrol.SelectedIndex == 0)
        //    {
        //        collarPreviewModel.ImportAllColumns(preferences.ImportCollarColumns);
        //        collarPreviewModel.ImportGenericFields(true, openSession);
        //    }

        //    else if (_tabcontrol.SelectedIndex == 1)
        //    {
        //        surveyPreviewModel.ImportAllColumns(preferences.ImportSurveyColumns);
        //        surveyPreviewModel.ImportGenericFields(true, openSession);
        //    }

        //    else if (_tabcontrol.SelectedIndex == 2)
        //    {
        //        assayPreviewModel.ImportAllColumns(preferences.ImportAssayColumns);
        //        assayPreviewModel.ImportGenericFields(true, openSession);
        //    }
        //    else if (_tabcontrol.SelectedIndex == 3)
        //    {

        //        intervalPreviewModel.ImportAllColumns(preferences.ImportIntervalColumns);
        //        intervalPreviewModel.ImportGenericFields(true, openSession);
        //    }
        //    else if (_tabcontrol.SelectedIndex == 4)
        //    {

        //        continuousPreviewModel.ImportAllColumns(preferences.ImportContinuousColumns);
        //        continuousPreviewModel.ImportGenericFields(true, openSession);
        //    }

        //}


        private void chkSkip_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void btnStatistics_Click(object sender, RoutedEventArgs e)
        {
            if (_tabcontrol.SelectedIndex == -1)
                return;

            switch (_tabcontrol.SelectedIndex)
            {
                case 0:
                    NavigationService.Navigate(new DrillholeSummaryStatisticsPage(0, collarPreviewModel.collarTableObject,
                    surveyPreviewModel.surveyTableObject, assayPreviewModel.assayTableObject, intervalPreviewModel.intervalTableObject, continuousPreviewModel.continuousTableObject));
                    break;
                case 1:
                    NavigationService.Navigate(new DrillholeSummaryStatisticsPage(1, collarPreviewModel.collarTableObject,
                    surveyPreviewModel.surveyTableObject, assayPreviewModel.assayTableObject, intervalPreviewModel.intervalTableObject, continuousPreviewModel.continuousTableObject));
                    break;
                case 2:
                    NavigationService.Navigate(new DrillholeSummaryStatisticsPage(2, collarPreviewModel.collarTableObject,
                    surveyPreviewModel.surveyTableObject, assayPreviewModel.assayTableObject, intervalPreviewModel.intervalTableObject, continuousPreviewModel.continuousTableObject));
                    break;
                case 3:
                    NavigationService.Navigate(new DrillholeSummaryStatisticsPage(3, collarPreviewModel.collarTableObject,
                    surveyPreviewModel.surveyTableObject, assayPreviewModel.assayTableObject, intervalPreviewModel.intervalTableObject, continuousPreviewModel.continuousTableObject));
                    break;
                case 4:
                    NavigationService.Navigate(new DrillholeSummaryStatisticsPage(4, collarPreviewModel.collarTableObject,
                    surveyPreviewModel.surveyTableObject, assayPreviewModel.assayTableObject, intervalPreviewModel.intervalTableObject, continuousPreviewModel.continuousTableObject));
                    break;
            }
        }

        private void ValidateDrillholes(object sender, RoutedEventArgs e)
        {
            if (_tabcontrol.SelectedIndex == -1)
                return;

            DrillholeSummaryMessagesPage validationMessages = new DrillholeSummaryMessagesPage(_tabcontrol.SelectedIndex);

            switch (_tabcontrol.SelectedIndex)
            {
                case 0:
                    validationMessages.collarObject = collarPreviewModel.collarTableObject;
                    break;
                case 1:
                    validationMessages.collarObject = collarPreviewModel.collarTableObject;
                    validationMessages.surveyObject = surveyPreviewModel.surveyTableObject;

                    break;
                case 2:
                    validationMessages.collarObject = collarPreviewModel.collarTableObject;
                    validationMessages.assayObject = assayPreviewModel.assayTableObject;
                    break;
                case 3:
                    validationMessages.collarObject = collarPreviewModel.collarTableObject;
                    validationMessages.intervalObject = intervalPreviewModel.intervalTableObject;
                    break;
                case 4:
                    validationMessages.collarObject = collarPreviewModel.collarTableObject;
                    validationMessages.continuousObject = continuousPreviewModel.continuousTableObject;
                    break;
            }

            NavigationService.Navigate(validationMessages);

        }

        private void btnValidate_Click(object sender, RoutedEventArgs e)
        {
            int _tabIndex = _tabcontrol.SelectedIndex;

            if (_tabIndex == -1)
                return;

            DrillholeEditsPage edits = new DrillholeEditsPage();

            edits.projectSession = projectSession;
            edits.projectLocation = projectLocation;
            edits.savedSession = savedSession;

            switch (_tabIndex)
            {
                case 0:
                    edits.collarObject = collarPreviewModel.collarTableObject;
                    if (collarPreviewModel.collarTableObject.surveyType == surveyType)
                    {
                        if (surveyPreviewModel.surveyTableObject.tableData != null)
                            edits.surveyObject = surveyPreviewModel.surveyTableObject;
                    }

                    if (assayPreviewModel.surveyTableObject.tableData != null)
                        edits.assayObject = assayPreviewModel.assayTableObject;

                    if (intervalPreviewModel.surveyTableObject.tableData != null)
                        edits.intervalObject = intervalPreviewModel.intervalTableObject;

                    edits.selectedIndex = 0;

                    collarPreviewModel.FillTable();

                    DataContext = collarPreviewModel;
                    collarPreviewModel.SetDataContext(dataPreview);
                    break;

                case 1:
                    edits.collarObject = collarPreviewModel.collarTableObject;
                    edits.surveyObject = surveyPreviewModel.surveyTableObject;

                    if (assayPreviewModel.surveyTableObject.tableData != null)
                        edits.assayObject = assayPreviewModel.assayTableObject;

                    if (intervalPreviewModel.surveyTableObject.tableData != null)
                        edits.intervalObject = intervalPreviewModel.intervalTableObject;

                    edits.selectedIndex = 1;
                    break;

                case 2:
                    edits.collarObject = collarPreviewModel.collarTableObject;
                    edits.assayObject = assayPreviewModel.assayTableObject;

                    if (collarPreviewModel.collarTableObject.surveyType == DrillholeSurveyType.downholesurvey)
                    {
                        if (surveyPreviewModel.surveyTableObject.tableData != null)
                            edits.surveyObject = surveyPreviewModel.surveyTableObject;
                    }

                    if (intervalPreviewModel.intervalTableObject.tableData != null)
                        edits.intervalObject = intervalPreviewModel.intervalTableObject;

                    edits.selectedIndex = 2;
                    break;

                case 3:
                    edits.collarObject = collarPreviewModel.collarTableObject;
                    edits.intervalObject = intervalPreviewModel.intervalTableObject;

                    if (collarPreviewModel.collarTableObject.surveyType == DrillholeSurveyType.downholesurvey)
                    {
                        if (surveyPreviewModel.surveyTableObject.tableData != null)
                            edits.surveyObject = surveyPreviewModel.surveyTableObject;
                    }

                    if (assayPreviewModel.assayTableObject.tableData != null)
                        edits.assayObject = assayPreviewModel.assayTableObject;

                    edits.selectedIndex = 3;
                    break;

                case 4:
                    edits.collarObject = collarPreviewModel.collarTableObject;
                    edits.continuousObject = continuousPreviewModel.continuousTableObject;

                    if (collarPreviewModel.collarTableObject.surveyType == DrillholeSurveyType.downholesurvey)
                    {
                        if (surveyPreviewModel.surveyTableObject.tableData != null)
                            edits.surveyObject = surveyPreviewModel.surveyTableObject;
                    }

                    if (assayPreviewModel.assayTableObject.tableData != null)
                        edits.assayObject = assayPreviewModel.assayTableObject;

                    if (intervalPreviewModel.intervalTableObject.tableData != null)
                        edits.intervalObject = intervalPreviewModel.intervalTableObject;

                    edits.selectedIndex = 4;
                    break;
            }

            NavigationService.Navigate(edits);

        }

        private void cboImportAs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //cboImportAs = (ComboBox)sender;

            ComboBox cboImportAs = sender as ComboBox;

            try
            {
                if (cboImportAs.Text == "" || cboImportAs.Text == null)
                    return;
                else
                {
                    var value = cboImportAs.SelectedItem as ComboBoxItem;

                    string selectedValue = value.Content.ToString();



                    UpdateFieldMatching(cboImportAs);
                }

                if (((ComboBoxItem)cboImportAs.SelectedItem).Content.ToString() == DrillholeConstants.notImported)
                {
                    CheckImportAllStatus();
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
        }

        private async void UpdateFieldMatching(ComboBox cboImportAs)
        {
            DrillholePreferences preferences = await CheckPreferencesFromXml();

            string selectedValue = ((ComboBoxItem)cboImportAs.SelectedItem).Content.ToString();
            ImportTableField _searchList = cboImportAs.DataContext as ImportTableField;

            if (_tabcontrol.SelectedIndex == 0)
            {
                collarPreviewModel.UpdateFieldvalues(cboImportAs.Text, selectedValue, _searchList, preferences.ImportCollarColumns, openSession);
                collarPreviewModel.SetPrimaryKey(DrillholeConstants.holeIDName);

                DataContext = collarPreviewModel;

                //update key 
               // await collarPreviewModel.UpdateHoleKeyInXml();

                //TODO update changed field in XML
            }

            else if (_tabcontrol.SelectedIndex == 1)
            {
                surveyPreviewModel.UpdateFieldvalues(cboImportAs.Text, selectedValue, _searchList, preferences.ImportSurveyColumns, openSession);
                surveyPreviewModel.SetSecondaryKey(DrillholeConstants.holeIDName);

                surveyPreviewModel.SetDataContext(dataPreview);
                DataContext = surveyPreviewModel;

                //update key 
              //  await surveyPreviewModel.UpdateHoleKeyInXml();

                //TODO update changed field in XML
            }
            else if (_tabcontrol.SelectedIndex == 2)
            {
                assayPreviewModel.UpdateFieldvalues(cboImportAs.Text, selectedValue, _searchList, preferences.ImportAssayColumns, openSession);

                assayPreviewModel.SetSecondaryKey(DrillholeConstants.holeIDName);

                DataContext = assayPreviewModel;

                //update key 
                //await assayPreviewModel.UpdateHoleKeyInXml();

                //TODO update changed field in XML

            }
            else if (_tabcontrol.SelectedIndex == 3)
            {
                intervalPreviewModel.UpdateFieldvalues(cboImportAs.Text, selectedValue, _searchList, preferences.ImportIntervalColumns, openSession);
                intervalPreviewModel.SetSecondaryKey(DrillholeConstants.holeIDName);

                DataContext = intervalPreviewModel;

                //update key 
              //  await intervalPreviewModel.UpdateHoleKeyInXml();

                //TODO update changed field in XML
            }

            else if (_tabcontrol.SelectedIndex == 4)
            {
                continuousPreviewModel.UpdateFieldvalues(cboImportAs.Text, selectedValue, _searchList, preferences.ImportContinuousColumns, openSession);
                continuousPreviewModel.SetSecondaryKey(DrillholeConstants.holeIDName);

                DataContext = continuousPreviewModel;

            }
        }


        private async void CheckImportAllStatus()
        {
            DrillholePreferences preferences = await CheckPreferencesFromXml();

            if (_tabcontrol.SelectedIndex == 0)
            {
                if (preferences.ImportCollarColumns)
                {
                    collarPreviewModel.importChecked = false;
                    collarPreviewModel.ImportGenericFields(false, openSession);

                    DataContext = collarPreviewModel;
                }
            }

            if (_tabcontrol.SelectedIndex == 1)
            {
             if (preferences.ImportSurveyColumns)
                {
                    surveyPreviewModel.importChecked = false;
                    surveyPreviewModel.ImportGenericFields(false, openSession);

                    DataContext = surveyPreviewModel;
                }
            }
            if (_tabcontrol.SelectedIndex == 2)
            {
             if (preferences.ImportAssayColumns)
                {
                    assayPreviewModel.importChecked = false;
                    assayPreviewModel.ImportGenericFields(false, openSession);

                    DataContext = assayPreviewModel;
                }
            }
            if (_tabcontrol.SelectedIndex == 3)
            {
                if (preferences.ImportIntervalColumns)
                {
                    intervalPreviewModel.importChecked = false;
                    intervalPreviewModel.ImportGenericFields(false, openSession);

                    DataContext = intervalPreviewModel;
                }
            }
            if (_tabcontrol.SelectedIndex == 4)
            {
                if (preferences.ImportContinuousColumns)
                {
                    continuousPreviewModel.importChecked = false;
                    continuousPreviewModel.ImportGenericFields(false, openSession);

                    DataContext = continuousPreviewModel;
                }
            }
            
        }

        private void lblFile_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService.CanGoBack)
            {
                this.NavigationService.GoBack();
            }
            else
            {
                MessageBox.Show("Sorry but no entries in back navigation history.", "Apologies", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private async void btnDesurvey_Click(object sender, RoutedEventArgs e)
        {

            //XDocument xmlProjectFile = await _xmlService.DrillholePreferences(projectFile, preferences, DrillholeConstants.drillholeProject);


            //READ xml preferences for survey type and survey method
            //First check if saved session or data is coming from TEMP
            DrillholePreferences preferences = await collarPreviewModel.ReadXmlPreferences();
            ///NEED
            ///1) SurveyType
            ///2) SurveyMethod
            ///3) Create toe
            ///4) Include collar -DEFAULT

            bool bToe = preferences.CreateToe;
            bool bCollar = preferences.CreateCollar;


            if (_tabcontrol.SelectedIndex == 0) //collar
            {
                List<XElement> collarValues = new List<XElement>();
                collarValues.Add(collarPreviewModel.collarTableObject.xPreview);

                Calculate.GenerateCollarDesurveyResults collarResults = new Calculate.GenerateCollarDesurveyResults(savedSession, projectSession, projectLocation, collarPreviewModel.collarDataFields,
                   collarValues);

                if (preferences.surveyType == DrillholeSurveyType.vertical)
                {
                    await collarResults.GenerateCollarDesurveyVertical(bToe, DrillholeDesurveyEnum.Tangential);
                }
                else
                {
                    await collarResults.GenerateCollarDesurveyFromCollarSurvey(bToe, DrillholeDesurveyEnum.Tangential);
                }
            }
            else if (_tabcontrol.SelectedIndex == 1) //downhole survey
            {
                List<XElement> surveyValues = new List<XElement>();
                surveyValues.Add(collarPreviewModel.collarTableObject.xPreview);
                surveyValues.Add(surveyPreviewModel.surveyTableObject.xPreview);

                Calculate.GenerateSurveyDesurveyResults surveyResults = new Calculate.GenerateSurveyDesurveyResults(savedSession, projectSession, projectLocation, surveyPreviewModel.surveyDataFields,
                   surveyValues)
                {
                    collarTableFields = collarPreviewModel.collarDataFields
                };

                await surveyResults.GenerateSurvey(bToe, bCollar, preferences.DesurveyMethod);

            }
            else if (_tabcontrol.SelectedIndex == 2) //Assay
            {
                List<XElement> assayValues = new List<XElement>();

                assayValues.Add(collarPreviewModel.collarTableObject.xPreview);

                if (surveyPreviewModel.surveyTableObject.xPreview != null)
                {
                    assayValues.Add(surveyPreviewModel.surveyTableObject.xPreview);
                }
                else
                {
                    XElement surveyValues = new XElement("Survey", "Empty");
                    assayValues.Add(surveyValues);
                }

                assayValues.Add(assayPreviewModel.assayTableObject.xPreview);

                Calculate.GenerateAssayDesurveyResults assayResults = new Calculate.GenerateAssayDesurveyResults(savedSession, projectSession, projectLocation, assayPreviewModel.assayDataFields,
                   assayValues)
                {
                    collarTableFields = collarPreviewModel.collarDataFields,
                    surveyTableFields = surveyPreviewModel.surveyDataFields
                };

                if (preferences.surveyType == DrillholeSurveyType.vertical)
                {
                    await assayResults.GenerateAssayDesurveyVertical(bToe, bCollar, preferences.DesurveyMethod);
                }
                else if (preferences.surveyType == DrillholeSurveyType.collarsurvey)
                {
                    await assayResults.GenerateAssayDesurveyFromCollarSurvey(bToe, bCollar, preferences.DesurveyMethod);
                }
                else //downhole surveys
                {
                    assayResults.surveyTableFields = surveyPreviewModel.surveyDataFields;
                    await assayResults.GenerateAssayDesurveyFromDownhole(bToe, bCollar, preferences.DesurveyMethod);
                }
            }
            else if (_tabcontrol.SelectedIndex == 3) //Interval
            {
                List<XElement> intervalValues = new List<XElement>();

                intervalValues.Add(collarPreviewModel.collarTableObject.xPreview);

                if (surveyPreviewModel.surveyTableObject.xPreview != null)
                {
                    intervalValues.Add(surveyPreviewModel.surveyTableObject.xPreview);
                }
                else
                {
                    XElement surveyValues = new XElement("Survey", "Empty");
                    intervalValues.Add(surveyValues);
                }

                intervalValues.Add(intervalPreviewModel.intervalTableObject.xPreview);

                Calculate.GenerateIntervalDesurveyResults intervalResults = new Calculate.GenerateIntervalDesurveyResults(savedSession, projectSession, projectLocation, intervalPreviewModel.intervalDataFields,
                   intervalValues)
                {
                    collarTableFields = collarPreviewModel.collarDataFields,
                    surveyTableFields = surveyPreviewModel.surveyDataFields
                };

                if (preferences.surveyType == DrillholeSurveyType.vertical)
                {
                    await intervalResults.GenerateIntervalDesurveyVertical(bToe, bCollar, preferences.DesurveyMethod);
                }
                else if (preferences.surveyType == DrillholeSurveyType.collarsurvey)
                {
                    await intervalResults.GenerateIntervalDesurveyFromCollarSurvey(bToe, bCollar, preferences.DesurveyMethod);
                }
                else
                {
                    intervalResults.surveyTableFields = surveyPreviewModel.surveyDataFields;
                    await intervalResults.GenerateIntervalDesurveyFromDownhole(bToe, bCollar, preferences.DesurveyMethod);
                }
            }
            else if (_tabcontrol.SelectedIndex == 4) //Continuous
            {
                List<XElement> continuousValues = new List<XElement>();

                continuousValues.Add(collarPreviewModel.collarTableObject.xPreview);

                if (surveyPreviewModel.surveyTableObject.xPreview != null)
                {
                    continuousValues.Add(surveyPreviewModel.surveyTableObject.xPreview);
                }
                else
                {
                    XElement surveyValues = new XElement("Survey", "Empty");
                    continuousValues.Add(surveyValues);
                }

                continuousValues.Add(continuousPreviewModel.continuousTableObject.xPreview);

                Calculate.GenerateContinuousDesurveyResults continuousResults = new Calculate.GenerateContinuousDesurveyResults(savedSession, projectSession, projectLocation, continuousPreviewModel.continuousDataFields,
                   continuousValues)
                {
                    collarTableFields = collarPreviewModel.collarDataFields,
                    surveyTableFields = surveyPreviewModel.surveyDataFields
                };

                if (preferences.surveyType == DrillholeSurveyType.vertical)
                {
                    await continuousResults.GenerateContinuousDesurveyVertical(bToe, bCollar, preferences.DesurveyMethod);
                }
                else if (preferences.surveyType == DrillholeSurveyType.collarsurvey)
                {
                    await continuousResults.GenerateContinuousDesurveyFromCollarSurvey(bToe, bCollar, preferences.DesurveyMethod);
                }
                else
                {
                    continuousResults.surveyTableFields = surveyPreviewModel.surveyDataFields;
                    await continuousResults.GenerateContinuousDesurveyFromDownhole(bToe, bCollar, preferences.DesurveyMethod);
                }
            }



            //CREATE results


            //Save to desruvey XML file

        }
    }
}

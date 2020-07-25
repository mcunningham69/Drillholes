﻿using Drillholes.Domain.DataObject;
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
        private string projectSession { get; set; }
        private string projectLocation { get; set; }

        private string xmlProjectFile { get; set; }

        #endregion


        public DrillholeImportPage()
        {
            InitializeComponent();
        }

        public DrillholeImportPage(List<DrillholeTable> _classes, bool _savedSession, string _xmlProjectFile, string _projectSession, string _projectLocation)
        {
            InitializeComponent();

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
        private void SetSurveyType(bool edits)
        {
            if (edits)
            {
                if ((bool)radVertical.IsChecked)
                {
                    surveyType = DrillholeSurveyType.vertical;
                }
                else if ((bool)radSurvey.IsChecked)
                {
                    surveyType = DrillholeSurveyType.collarsurvey;
                }
                else
                {
                    surveyType = DrillholeSurveyType.downholesurvey;
                }
            }
            else
            {
                if ((bool)radVertical.IsChecked)
                {
                    collarPreviewModel.collarTableObject.surveyType = DrillholeSurveyType.vertical;

                    if (assayPreviewModel.assayTableObject != null)
                        assayPreviewModel.assayTableObject.surveyType = DrillholeSurveyType.vertical;

                    if (intervalPreviewModel.intervalTableObject != null)
                        intervalPreviewModel.intervalTableObject.surveyType = DrillholeSurveyType.vertical;

                    if (continuousPreviewModel.continuousTableObject != null)
                        continuousPreviewModel.continuousTableObject.surveyType = DrillholeSurveyType.vertical;
                }
                else if ((bool)radSurvey.IsChecked)
                {
                    collarPreviewModel.collarTableObject.surveyType = DrillholeSurveyType.collarsurvey;

                    if (assayPreviewModel.assayTableObject != null)
                        assayPreviewModel.assayTableObject.surveyType = DrillholeSurveyType.collarsurvey;

                    if (intervalPreviewModel.intervalTableObject != null)
                        intervalPreviewModel.intervalTableObject.surveyType = DrillholeSurveyType.collarsurvey;

                    if (continuousPreviewModel.continuousTableObject != null)
                        continuousPreviewModel.continuousTableObject.surveyType = DrillholeSurveyType.collarsurvey;
                }
                else
                {
                    collarPreviewModel.collarTableObject.surveyType = DrillholeSurveyType.downholesurvey;
                    surveyPreviewModel.surveyTableObject.surveyType = DrillholeSurveyType.downholesurvey;

                    if (assayPreviewModel.assayTableObject != null)
                        assayPreviewModel.assayTableObject.surveyType = DrillholeSurveyType.downholesurvey;

                    if (intervalPreviewModel.intervalTableObject != null)
                        intervalPreviewModel.intervalTableObject.surveyType = DrillholeSurveyType.downholesurvey;

                    if (continuousPreviewModel.continuousTableObject != null)
                        continuousPreviewModel.continuousTableObject.surveyType = DrillholeSurveyType.downholesurvey;
                }
            }
        }

        private async void _tabcontrol_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (collarPreviewModel == null)
                return;

            int _tabIndex = _tabcontrol.SelectedIndex;


            if (_tabIndex == -1)
                return;

            int nLimit = -99;

            //downhole survey is default, but if no survey table then change
            if (tableSurveyName == "")
            {
                radVertical.IsChecked = true;
                radDhole.IsEnabled = false;
            }

            try
            {
                if (_tabIndex == 0) //COLLAR
                {
                    if (collarPreviewModel.collarTableObject.tableData == null)
                    {
                        await LoadCollarTableAndFields(nLimit);
                    }

                    chkImport.IsChecked = collarPreviewModel.importChecked;
                    collarPreviewModel.ImportAllColumns((bool)chkImport.IsChecked);

                    DataContext = collarPreviewModel;
                    collarPreviewModel.SetDataContext(dataPreview);

                    radSurvey.Visibility = Visibility.Visible;
                    radVertical.Visibility = Visibility.Visible;

                }
                else if (_tabIndex == 1)//survey
                {

                    if (surveyPreviewModel.surveyTableObject.tableData == null)
                    {
                        await LoadSurveyTableAndFields(nLimit);

                    }

                    DataContext = surveyPreviewModel;
                    surveyPreviewModel.SetDataContext(dataPreview);

                    radDhole.Visibility = Visibility.Visible;
                    radVertical.Visibility = Visibility.Hidden;
                    radDhole.IsEnabled = true;
                    radDhole.IsChecked = true;
                    radSurvey.Visibility = Visibility.Hidden;

                }
                else if (_tabIndex == 2) //assay
                {
                    if (assayPreviewModel.assayTableObject.tableData == null)
                    {
                        await LoadAssayTableAndFields(nLimit);
                    }

                    DataContext = assayPreviewModel;
                    assayPreviewModel.SetDataContext(dataPreview);

                    radDhole.Visibility = Visibility.Visible;
                    radVertical.Visibility = Visibility.Visible;
                    radSurvey.Visibility = Visibility.Visible;
                }
                else if (_tabIndex == 3)//interval
                {
                    if (intervalPreviewModel.intervalTableObject.tableData == null)
                    {
                        await LoadIntervalTableAndFields(nLimit);
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
                SetSurveyType(false);
            }
        }


        #region Load Tables
        private async Task<bool> LoadCollarTableAndFields(int nLimit)
        {

            await collarPreviewModel.RetrieveFieldsToMap(); //on start, holeIDName set to empty string

            await collarPreviewModel.PreviewDataToImport(nLimit); //50 is the limit of records to preview

            //sets the fieldnames
         //   await collarPreviewModel.UpdateFieldnamesInXml();

            chkSkip.IsEnabled = false;
            chkSkip.IsChecked = false;

            chkImport.IsChecked = collarPreviewModel.importChecked;
            collarPreviewModel.ImportAllColumns((bool)chkImport.IsChecked);



            return true;

        }

        private async Task<bool> LoadSurveyTableAndFields(int nLimit)
        {
            if (surveyPreviewModel.surveyTableObject.tableData == null)
            {
                await surveyPreviewModel.RetrieveFieldsToMap(); //on start, holeIDName set to empty string
                surveyPreviewModel.surveyTableObject.collarKey = collarPreviewModel.collarTableObject.collarKey;
                await surveyPreviewModel.PreviewDataToImport(nLimit); //50 is the limit of records to preview

                //sets the fieldnames
                await surveyPreviewModel.UpdateFieldnamesInXml(); //update XML => eventually save and open project

            }

            chkSkip.IsEnabled = true;
            chkSkip.IsChecked = surveyPreviewModel.skipTable;

            chkImport.IsChecked = surveyPreviewModel.importChecked;
            surveyPreviewModel.ImportAllColumns((bool)chkImport.IsChecked);

            return true;
        }


        private async Task<bool> LoadAssayTableAndFields(int nLimit)
        {
            if (assayPreviewModel.assayTableObject.tableData == null)
            {
                if ((bool)radVertical.IsChecked)
                    assayPreviewModel.assayTableObject.surveyType = DrillholeSurveyType.vertical;
                else if ((bool)radSurvey.IsChecked)
                    assayPreviewModel.assayTableObject.surveyType = DrillholeSurveyType.collarsurvey;
                else
                    assayPreviewModel.assayTableObject.surveyType = DrillholeSurveyType.downholesurvey;

                await assayPreviewModel.RetrieveFieldsToMap(); //on start, holeIDName set to empty string
                assayPreviewModel.assayTableObject.collarKey = collarPreviewModel.collarTableObject.collarKey;

                if (bEnableSurvey)
                    assayPreviewModel.assayTableObject.surveyKey = surveyPreviewModel.surveyTableObject.surveyKey;

                await assayPreviewModel.PreviewDataToImport(nLimit); //50 is the limit of records to preview

                await assayPreviewModel.UpdateFieldnamesInXml();
            }

            chkSkip.IsEnabled = true;
            chkSkip.IsChecked = assayPreviewModel.skipTable;

            chkImport.IsChecked = assayPreviewModel.importChecked;
            assayPreviewModel.ImportAllColumns((bool)chkImport.IsChecked);

            return true;
        }

        private async Task<bool> LoadIntervalTableAndFields(int nLimit)
        {
            if (intervalPreviewModel.intervalTableObject.tableData == null)
            {
                if ((bool)radVertical.IsChecked)
                    intervalPreviewModel.intervalTableObject.surveyType = DrillholeSurveyType.vertical;
                else if ((bool)radSurvey.IsChecked)
                    intervalPreviewModel.intervalTableObject.surveyType = DrillholeSurveyType.collarsurvey;
                else
                    intervalPreviewModel.intervalTableObject.surveyType = DrillholeSurveyType.downholesurvey;

                await intervalPreviewModel.RetrieveFieldsToMap(); //on start, holeIDName set to empty string
                intervalPreviewModel.intervalTableObject.collarKey = collarPreviewModel.collarTableObject.collarKey;

                if (bEnableSurvey)
                    intervalPreviewModel.surveyTableObject.surveyKey = surveyPreviewModel.surveyTableObject.surveyKey;

                if (bEnableAssay && assayPreviewModel.assayTableObject.assayKey != "")
                    intervalPreviewModel.assayTableObject.assayKey = assayPreviewModel.assayTableObject.assayKey;

                await intervalPreviewModel.PreviewDataToImport(nLimit); //50 is the limit of records to preview

                //sets the fieldnames
                await intervalPreviewModel.UpdateFieldnamesInXml(); //update XML => eventually save and open project
            }

            chkSkip.IsEnabled = true;
            chkSkip.IsChecked = intervalPreviewModel.skipTable;

            chkImport.IsChecked = intervalPreviewModel.importChecked;
            intervalPreviewModel.ImportAllColumns((bool)chkImport.IsChecked);

            return true;
        }

        private async Task<bool> LoadContinuousTableAndFields(int nLimit)
        {
            if (continuousPreviewModel.continuousTableObject.tableData == null)
            {
                if ((bool)radVertical.IsChecked)
                    continuousPreviewModel.intervalTableObject.surveyType = DrillholeSurveyType.vertical;
                else if ((bool)radSurvey.IsChecked)
                    continuousPreviewModel.intervalTableObject.surveyType = DrillholeSurveyType.collarsurvey;
                else
                    continuousPreviewModel.intervalTableObject.surveyType = DrillholeSurveyType.downholesurvey;

                await continuousPreviewModel.RetrieveFieldsToMap(); //on start, holeIDName set to empty string
                continuousPreviewModel.intervalTableObject.collarKey = collarPreviewModel.collarTableObject.collarKey;

                if (bEnableSurvey)
                    continuousPreviewModel.surveyTableObject.surveyKey = surveyPreviewModel.surveyTableObject.surveyKey;

                if (bEnableAssay && assayPreviewModel.assayTableObject.assayKey != "")
                    continuousPreviewModel.assayTableObject.assayKey = assayPreviewModel.assayTableObject.assayKey;

                if (bEnableInterval && intervalPreviewModel.intervalTableObject.intervalKey != "")
                    continuousPreviewModel.intervalTableObject.intervalKey = intervalPreviewModel.intervalTableObject.intervalKey;

                await continuousPreviewModel.PreviewDataToImport(nLimit); //50 is the limit of records to preview

                //sets the fieldnames
                await continuousPreviewModel.UpdateFieldnamesInXml(); //update XML => eventually save and open project
            }

            chkSkip.IsEnabled = true;
            chkSkip.IsChecked = continuousPreviewModel.skipTable;

            chkImport.IsChecked = continuousPreviewModel.importChecked;
            continuousPreviewModel.ImportAllColumns((bool)chkImport.IsChecked);

            return true;
        }
        #endregion

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnCreateHole_Click(object sender, RoutedEventArgs e)
        {

        }

        private void chkImport_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)chkImport.IsChecked)
            {
                UpdateImportAllStatus();
            }
        }

        private void UpdateImportAllStatus()
        {
            if (_tabcontrol.SelectedIndex == 0)
            {
                collarPreviewModel.ImportAllColumns((bool)chkImport.IsChecked);
                collarPreviewModel.ImportGenericFields(true);

            }

            else if (_tabcontrol.SelectedIndex == 1)
            {
                surveyPreviewModel.ImportAllColumns((bool)chkImport.IsChecked);
                surveyPreviewModel.ImportGenericFields(true);

            }

            else if (_tabcontrol.SelectedIndex == 2)
            {

                assayPreviewModel.ImportAllColumns((bool)chkImport.IsChecked);
                assayPreviewModel.ImportGenericFields(true);

            }
            else if (_tabcontrol.SelectedIndex == 3)
            {

                intervalPreviewModel.ImportAllColumns((bool)chkImport.IsChecked);
                intervalPreviewModel.ImportGenericFields(true);

            }
            else if (_tabcontrol.SelectedIndex == 4)
            {

                continuousPreviewModel.ImportAllColumns((bool)chkImport.IsChecked);
                continuousPreviewModel.ImportGenericFields(true);

            }

        }

        private void radSurvey_Click(object sender, RoutedEventArgs e)
        {
            SetSurveyType(false);

        }

        private void radVertical_Click(object sender, RoutedEventArgs e)
        {
            SetSurveyType(false);

        }

        private void radDhole_Click(object sender, RoutedEventArgs e)
        {
            SetSurveyType(false);

        }

        private void chkSkip_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void btnStatistics_Click(object sender, RoutedEventArgs e)
        {


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
            NavigationService.Navigate(new DrillholeSummaryMessagesPage());

        }

        private void btnValidate_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new DrillholeEditsPage());

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
            string selectedValue = ((ComboBoxItem)cboImportAs.SelectedItem).Content.ToString();
            ImportTableField _searchList = cboImportAs.DataContext as ImportTableField;

            if (_tabcontrol.SelectedIndex == 0)
            {
                collarPreviewModel.UpdateFieldvalues(cboImportAs.Text, selectedValue, _searchList, (bool)chkImport.IsChecked);
                collarPreviewModel.SetPrimaryKey(DrillholeConstants.holeIDName);

                DataContext = collarPreviewModel;

                //update key 
                await collarPreviewModel.UpdateHoleKeyInXml();

                //TODO update changed field in XML
            }

            else if (_tabcontrol.SelectedIndex == 1)
            {
                surveyPreviewModel.UpdateFieldvalues(cboImportAs.Text, selectedValue, _searchList, (bool)chkImport.IsChecked);
                surveyPreviewModel.SetSecondaryKey(DrillholeConstants.holeIDName);

                surveyPreviewModel.SetDataContext(dataPreview);
                DataContext = surveyPreviewModel;

                //update key 
                await surveyPreviewModel.UpdateHoleKeyInXml();

                //TODO update changed field in XML
            }
            else if (_tabcontrol.SelectedIndex == 2)
            {
                assayPreviewModel.UpdateFieldvalues(cboImportAs.Text, selectedValue, _searchList, (bool)chkImport.IsChecked);

                assayPreviewModel.SetSecondaryKey(DrillholeConstants.holeIDName);

                DataContext = assayPreviewModel;

                //update key 
                await assayPreviewModel.UpdateHoleKeyInXml();

                //TODO update changed field in XML

            }
            else if (_tabcontrol.SelectedIndex == 3)
            {
                intervalPreviewModel.UpdateFieldvalues(cboImportAs.Text, selectedValue, _searchList, (bool)chkImport.IsChecked);
                intervalPreviewModel.SetSecondaryKey(DrillholeConstants.holeIDName);

                DataContext = intervalPreviewModel;

                //update key 
                await intervalPreviewModel.UpdateHoleKeyInXml();

                //TODO update changed field in XML
            }

            else if (_tabcontrol.SelectedIndex == 4)
            {
                continuousPreviewModel.UpdateFieldvalues(cboImportAs.Text, selectedValue, _searchList, (bool)chkImport.IsChecked);
                continuousPreviewModel.SetSecondaryKey(DrillholeConstants.holeIDName);

                DataContext = continuousPreviewModel;

                //update key 
                await continuousPreviewModel.UpdateHoleKeyInXml();

            }
        }


        private void CheckImportAllStatus()
        {
            if (_tabcontrol.SelectedIndex == 0)
            {
                if ((bool)chkImport.IsChecked)
                {
                    collarPreviewModel.importChecked = false;
                    collarPreviewModel.ImportGenericFields(false);
                }

                else if ((bool)chkImport.IsChecked)
                {
                    surveyPreviewModel.importChecked = false;
                    surveyPreviewModel.ImportGenericFields(false);
                }
                else if ((bool)chkImport.IsChecked)
                {
                    assayPreviewModel.importChecked = false;
                    assayPreviewModel.ImportGenericFields(false);
                }
                else if ((bool)chkImport.IsChecked)
                {
                    intervalPreviewModel.importChecked = false;
                    intervalPreviewModel.ImportGenericFields(false);
                }
                else if ((bool)chkImport.IsChecked)
                {
                    continuousPreviewModel.importChecked = false;
                    continuousPreviewModel.ImportGenericFields(false);
                }
            }
        }

        private void lblFile_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

    
    }
}

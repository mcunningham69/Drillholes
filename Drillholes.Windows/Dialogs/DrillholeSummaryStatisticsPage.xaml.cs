using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Drillholes.Domain;
using Drillholes.Domain.DataObject;
using Drillholes.Windows.ViewModel;
using Drillholes.Domain.Enum;
using System.Xml.Linq;

namespace Drillholes.Windows.Dialogs
{
    /// <summary>
    /// Interaction logic for DrillholeSummaryStatisticsPage.xaml
    /// </summary>
    public partial class DrillholeSummaryStatisticsPage : Page
    {
        private CollarTableObject collarObject { get; set; }
        private SurveyTableObject surveyObject { get; set; }
        private AssayTableObject assayObject { get; set; }
        private IntervalTableObject intervalObject { get; set; }
        private ContinuousTableObject continuousObject { get; set; }


        private CollarStatisticsView collarStatisticsView { get; set; }
        private SurveyStatisticsView surveyStatisticsView { get; set; }
        private AssayStatisticsView assayStatisticsView { get; set; }
        private IntervalStatisticsView intervalStatisticsView { get; set; }
        private ContinuousStatisticsView continuousStatisticsView { get; set; }

        private int selectedIndex { get; set; }


        public DrillholeSummaryStatisticsPage(int _selectedIndex, CollarTableObject _collarView, SurveyTableObject _surveyView,
            AssayTableObject _assayView, IntervalTableObject _intervalView, ContinuousTableObject _continuous)
        {
            InitializeComponent();

            collarObject = _collarView;
            surveyObject = _surveyView;
            assayObject = _assayView;
            intervalObject = _intervalView;
            continuousObject = _continuous;

            selectedIndex = _selectedIndex;
        }

        private void ValidatedTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ValidateTabs.SelectedIndex == 0)
                DataContext = collarStatisticsView;
            else if (ValidateTabs.SelectedIndex == 1)
                DataContext = surveyStatisticsView;
            else if (ValidateTabs.SelectedIndex == 2)
                DataContext = assayStatisticsView;
            else if (ValidateTabs.SelectedIndex == 3)
                DataContext = intervalStatisticsView;
            else if (ValidateTabs.SelectedIndex == 4)
                DataContext = continuousStatisticsView;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            EnableTabs();
            SummariseValues();

            var item = sender as TabControl;

            ValidateSelectedTab();
        }
        private async void EnableTabs()
        {
            TabItem tabItem = null;

            if (surveyObject.tableData != null)
            {
                tabItem = ValidateTabs.Items.GetItemAt(1) as TabItem;
                tabItem.IsEnabled = true;
            }

            if (assayObject.tableData != null)
            {
                tabItem = ValidateTabs.Items.GetItemAt(2) as TabItem;
                tabItem.IsEnabled = true;

            }

            if (intervalObject.tableData != null)
            {
                tabItem = ValidateTabs.Items.GetItemAt(3) as TabItem;
                tabItem.IsEnabled = true;

            }

            if (continuousObject.tableData != null)
            {
                tabItem = ValidateTabs.Items.GetItemAt(4) as TabItem;
                tabItem.IsEnabled = true;

            }
        }

        public void ValidateSelectedTab()
        {
            ValidateTabs.SelectedIndex = selectedIndex;
        }

        private async void SummariseValues()
        {
            bool bCheck = true;
            if (selectedIndex == 0)
            {
                bCheck = await CollarStatistics();

            }
            else if (selectedIndex == 1)
            {
                bCheck = await CollarStatistics();
                bCheck = await SurveyStatistics();
            }
            else if (selectedIndex == 2)
            {
                bCheck = await CollarStatistics();

                if (surveyObject.tableData != null)
                {
                    bCheck = await SurveyStatistics();
                }
                bCheck = await AssayStatistics();
            }
            else if (selectedIndex == 3)
            {
                bCheck = await CollarStatistics();

                if (surveyObject.tableData != null)
                {
                    bCheck = await SurveyStatistics();
                }

                if (assayObject.tableData != null)
                    bCheck = await AssayStatistics();

                bCheck = await IntervalStatistics();
            }
            else if (selectedIndex == 4)
            {
                bCheck = await CollarStatistics();

                if (surveyObject.tableData != null)
                {
                    bCheck = await SurveyStatistics();
                }

                if (assayObject.tableData != null)
                    bCheck = await AssayStatistics();

                if (intervalObject.tableData != null)
                    bCheck = await IntervalStatistics();

                bCheck = await ContinuousStatistics();
            }

            if (!bCheck)
                MessageBox.Show("Problem with statistics");
        }


        private async Task<bool> CollarStatistics()
        {
            string tableName = collarObject.tableName;
            string tableLocation = collarObject.tableLocation;
            string tableFormat = "collar";
            DrillholeSurveyType surveyType = collarObject.surveyType;
            XElement xPreview = collarObject.xPreview;
            ImportTableFields fields = collarObject.tableData;

            collarStatisticsView = new CollarStatisticsView(tableName, tableLocation, tableFormat,
                fields, surveyType, xPreview);

            await collarStatisticsView.SummaryStatistics();

            DataContext = collarStatisticsView;

            return true;
        }

        private async Task<bool> SurveyStatistics()
        {
            string tableName = surveyObject.tableName;
            string tableLocation = surveyObject.tableLocation;
            string tableFormat = "survey";

            XElement xPreview = surveyObject.xPreview;
            ImportTableFields fields = surveyObject.tableData;

            surveyStatisticsView = new SurveyStatisticsView(tableName, tableLocation, tableFormat,
                fields, DrillholeSurveyType.downholesurvey, xPreview);

            await surveyStatisticsView.SummaryStatistics();

            DataContext = surveyStatisticsView;

            return true;
        }

        private async Task<bool> AssayStatistics()
        {
            string tableName = assayObject.tableName;
            string tableLocation = assayObject.tableLocation;
            string tableFormat = "assay";
            DrillholeSurveyType surveyType = assayObject.surveyType;

            XElement xPreview = assayObject.xPreview;
            ImportTableFields fields = assayObject.tableData;

            assayStatisticsView = new AssayStatisticsView(tableName, tableLocation, tableFormat,
                fields, surveyType, xPreview);

            await assayStatisticsView.SummaryStatistics();

            DataContext = assayStatisticsView;

            return true;
        }

        private async Task<bool> IntervalStatistics()
        {
            string tableName = intervalObject.tableName;
            string tableLocation = intervalObject.tableLocation;
            string tableFormat = "interval";
            DrillholeSurveyType surveyType = intervalObject.surveyType;

            XElement xPreview = intervalObject.xPreview;
            ImportTableFields fields = intervalObject.tableData;

            intervalStatisticsView = new IntervalStatisticsView(tableName, tableLocation, tableFormat,
                fields, surveyType, xPreview);

            await intervalStatisticsView.SummaryStatistics();

            DataContext = intervalStatisticsView;

            return true;
        }

        private async Task<bool> ContinuousStatistics()
        {
            string tableName = continuousObject.tableName;
            string tableLocation = continuousObject.tableLocation;
            string tableFormat = "continuous";
            DrillholeSurveyType surveyType = continuousObject.surveyType;

            XElement xPreview = continuousObject.xPreview;
            ImportTableFields fields = continuousObject.tableData;

            continuousStatisticsView = new ContinuousStatisticsView(tableName, tableLocation, tableFormat,
                fields, surveyType, xPreview);

            await continuousStatisticsView.SummaryStatistics();

            DataContext = continuousStatisticsView;

            return true;
        }

        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            //if (this.NavigationService.CanGoBack)
            //    this.NavigationService.GoBack();
            //else
            //{
            //    MessageBox.Show("Sorry but no entries in back navigation history.", "Apologies", MessageBoxButton.OK, MessageBoxImage.Information);
            //}
        }
    }
}

using Drillholes.Windows.ViewModel;
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
using System.Windows.Shapes;

namespace Drillholes.Windows.Dialogs
{
    /// <summary>
    /// Interaction logic for DrillholeSummaryStatistics.xaml
    /// </summary>
    public partial class DrillholeSummaryStatistics : Window
    {
        private CollarView collarPreviewModel { get; set; }
        private SurveyView surveyPreviewModel { get; set; }
        private AssayView assayPreviewModel { get; set; }
        private IntervalView intervalPreviewModel { get; set; }

        public int selectedIndex { get; set; }
        public DrillholeSummaryStatistics(CollarView _collarView, SurveyView _surveyView, AssayView _assayView, IntervalView _intervalView)
        {
            InitializeComponent();

            collarPreviewModel = _collarView;
            surveyPreviewModel = _surveyView;
            assayPreviewModel = _assayView;
            intervalPreviewModel = _intervalView;
        }

        private void CloseForm(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            EnableTabs();
            SummariseValues();

            var item = sender as TabControl;

            ValidateSelectedTab();
        }

        public void ValidateSelectedTab()
        {
            ValidateTabs.SelectedIndex = selectedIndex;
        }

        private void ValidatedTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ValidateTabs.SelectedIndex == 0)
                DataContext = collarPreviewModel;
            else if (ValidateTabs.SelectedIndex == 1)
                DataContext = surveyPreviewModel;
            else if (ValidateTabs.SelectedIndex == 2)
                DataContext = assayPreviewModel;
            else if (ValidateTabs.SelectedIndex == 3)
                DataContext = intervalPreviewModel;
        }

        private async void EnableTabs()
        {
            TabItem tabItem = null;

            if (surveyPreviewModel != null)
            {
                tabItem = ValidateTabs.Items.GetItemAt(1) as TabItem;
                tabItem.IsEnabled = true;
            }

            if (assayPreviewModel != null)
            {
                tabItem = ValidateTabs.Items.GetItemAt(2) as TabItem;
                tabItem.IsEnabled = true;

            }

            if (intervalPreviewModel != null)
            {
                tabItem = ValidateTabs.Items.GetItemAt(3) as TabItem;
                tabItem.IsEnabled = true;

            }
        }

        private async void SummariseValues()
        {
            bool bCheck;
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

                if (surveyPreviewModel != null)
                {
                    bCheck = await SurveyStatistics();
                }
                bCheck = await AssayStatistics();
            }
            else if (selectedIndex == 3)
            {
                bCheck = await CollarStatistics();

                if (surveyPreviewModel != null)
                {
                    bCheck = await SurveyStatistics();
                }

                if (assayPreviewModel != null)
                    bCheck = await AssayStatistics();

                bCheck = await IntervalStatistics();
            }
        }


        private async Task<bool> CollarStatistics()
        {
            await collarPreviewModel.SummaryStatistics();

            DataContext = collarPreviewModel;

            return true;
        }

        private async Task<bool> SurveyStatistics()
        {
            await surveyPreviewModel.SummaryStatistics();
            DataContext = surveyPreviewModel;

            return true;
        }

        private async Task<bool> AssayStatistics()
        {
            await assayPreviewModel.SummaryStatistics();
            DataContext = assayPreviewModel;

            return true;
        }

        private async Task<bool> IntervalStatistics()
        {
            await intervalPreviewModel.SummaryStatistics();
            DataContext = intervalPreviewModel;

            return true;
        }
    }
}

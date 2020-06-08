using Drillholes.Domain.DataObject;
using Drillholes.Domain.Enum;
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
    /// Interaction logic for DrillholeEdits.xaml
    /// </summary>
    public partial class DrillholeEdits : Window
    {
        public CollarTableObject collarObject { get; set; }
        public SurveyTableObject surveyObject { get; set; }
        public AssayTableObject assayObject { get; set; }
        public IntervalTableObject intervalObject { get; set; }

        private CollarValidationView collarEditView { get; set; }
        private SurveyValidationView surveyEditView { get; set; }
        private AssayValidationView assayEditView { get; set; }
        private IntervalValidationView intervalEditView { get; set; }

        public DrillholeSurveyType surveyType { get; set; }
        public int selectedIndex { get; set; }

        private static DrillholeEdits m_instance;
        public DrillholeEdits()
        {
            InitializeComponent();
        }

        public static DrillholeEdits GetInstance()
        {
            if (m_instance == null)
            {

                return m_instance = new DrillholeEdits();
            }


            return m_instance;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DisplayMessages();
        }

        private async void DisplayMessages()
        {
            if (selectedIndex == 0)
            {
                ValidateCollar();


                DataContext = collarEditView;

            }
            else if (selectedIndex == 1)
            {
                ValidateCollar();
                ValidateSurvey();

                DataContext = surveyEditView;

            }
            else if (selectedIndex == 2)
            {
                ValidateCollar();

                if (DrillholeSurveyType.downholesurvey == surveyType)
                {
                    if (surveyObject != null)
                    {
                        if (surveyObject.tableData != null)
                            ValidateSurvey();
                    }
                }

                ValidateAssay();

                DataContext = assayEditView;

            }
            else if (selectedIndex == 3)
            {
                ValidateCollar();

                if (DrillholeSurveyType.downholesurvey == surveyType)
                {
                    if (surveyObject != null)
                    {
                        if (surveyObject.tableData != null)
                            ValidateSurvey();
                    }
                }

                if (assayObject != null)
                {
                    if (assayObject.tableData != null)
                        ValidateAssay();
                }

                ValidateInterval();

                DataContext = intervalEditView;

            }
        }

        private async void ValidateCollar()
        {
            collarEditView = new CollarValidationView(DrillholeTableType.collar, collarObject.surveyType, collarObject.xPreview);
            collarEditView.importCollarFields = collarObject.tableData;
            await collarEditView.ValidateAllTables(true);

            collarEditView.ReshapeMessages();
        }

        //testtype
        //validation test & validation status

        private async void ValidateSurvey()
        {
            surveyEditView = new SurveyValidationView(DrillholeTableType.survey, surveyObject.xPreview);
            surveyEditView.importSurveyFields = surveyObject.tableData;
            surveyEditView.importCollarFields = collarObject.tableData;
            surveyEditView.xmlCollarData = collarObject.xPreview;
            await surveyEditView.ValidateAllTables(true);
        }

        private async void ValidateAssay()
        {
            assayEditView = new AssayValidationView(DrillholeTableType.assay, assayObject.xPreview);
            assayEditView.importAssayFields = assayObject.tableData;
            assayEditView.importCollarFields = collarObject.tableData;
            assayEditView.xmlCollarData = collarObject.xPreview;
            await assayEditView.ValidateAllTables(true);
        }

        private async void ValidateInterval()
        {
            intervalEditView = new IntervalValidationView(DrillholeTableType.interval, intervalObject.xPreview);
            intervalEditView.importIntervalFields = intervalObject.tableData;
            intervalEditView.importCollarFields = collarObject.tableData;
            intervalEditView.xmlCollarData = collarObject.xPreview;
            await intervalEditView.ValidateAllTables(true);
        }
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            m_instance = null;
            this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnTD_Click(object sender, RoutedEventArgs e)
        {

        }

        private void tvEdit_SourceUpdated(object sender, DataTransferEventArgs e)
        {

        }

        private void tvEdit_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            m_instance = null;
        }
    }
}

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

        private CollarValidationView collarEdits { get; set; }
        private SurveyValidationView surveyEdits { get; set; }
        private AssayValidationView assayEdits { get; set; }
        private IntervalValidationView intervalEdits { get; set; }

   //     public DrillholeSurveyType surveyType { get; set; }
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


                DataContext = collarEdits;

            }
            else if (selectedIndex == 1)
            {
                //ValidateCollar();
                ValidateSurvey(true);

                DataContext = surveyEdits;

            }
            else if (selectedIndex == 2)
            {
              //  ValidateCollar();

              // // if (DrillholeSurveyType.downholesurvey == surveyType)
              ////  {
              //      if (surveyObject != null)
              //      {
              //          if (surveyObject.tableData != null)
              //              ValidateSurvey();
              //      }
               // }

                ValidateAssay();

                DataContext = assayEdits;

            }
            else if (selectedIndex == 3)
            {
              //  ValidateCollar();

              ////  if (DrillholeSurveyType.downholesurvey == surveyType)
              ////  {
              //      if (surveyObject != null)
              //      {
              //          if (surveyObject.tableData != null)
              //              ValidateSurvey();
              //      }
              ////  }

              //  if (assayObject != null)
              //  {
              //      if (assayObject.tableData != null)
              //          ValidateAssay();
              //  }

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

            if (intervalObject.surveyType == DrillholeSurveyType.downholesurvey)
            {
                ValidateSurvey(false);

                if (assayObject.tableData != null)
                {
                    ValidateAssay();
                    intervalEdits.EditDrillholeData = assayEdits.EditDrillholeData;

                }
                else
                    intervalEdits.EditDrillholeData = surveyEdits.EditDrillholeData;

            }
            else if (assayObject.tableData != null)
                intervalEdits.EditDrillholeData = assayEdits.EditDrillholeData;
            else
                intervalEdits.EditDrillholeData = collarEdits.EditDrillholeData;


            intervalEdits = new IntervalValidationView(DrillholeTableType.interval, intervalObject.xPreview);
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

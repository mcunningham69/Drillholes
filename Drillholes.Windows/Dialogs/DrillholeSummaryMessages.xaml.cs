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
    /// Interaction logic for DrillholeSummaryMessages.xaml
    /// </summary>
    public partial class DrillholeSummaryMessages : Window
    {
        public CollarTableObject collarObject { get; set; }
        public SurveyTableObject surveyObject { get; set; }
        public AssayTableObject assayObject { get; set; }
        public IntervalTableObject intervalObject { get; set; }

        private CollarValidationView collarMessagesView { get; set; }
        private SurveyValidationView surveyMessagesView { get; set; }
        private AssayValidationView assayMessagesView { get; set; }
        private IntervalValidationView intervalMessagesView { get; set; }

        public int selectedIndex { get; set; }

        private static DrillholeSummaryMessages m_instance;


        public DrillholeSummaryMessages()
        {
            InitializeComponent();

        }

        public static DrillholeSummaryMessages GetInstance()
        {
            if (m_instance == null)
            {

                return m_instance = new DrillholeSummaryMessages();
            }


            return m_instance;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            m_instance = null;
            this.Close();

        }

        private void btnFixErrors_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //if (selectedIndex == 0)
            //    DataContext = collarMessagesView;
            //else if (selectedIndex == 1)
            //    DataContext = surveyMessagesView;
            //else if (selectedIndex == 2)
            //    DataContext = assayMessagesView;
            //else if (selectedIndex == 3)
            //    DataContext = intervalMessagesView;

            DisplayMessages();
        }

        private async void DisplayMessages()
        {
            if (selectedIndex == 0)
            {
                collarMessagesView = new CollarValidationView(DrillholeTableType.collar, collarObject.surveyType, collarObject.xPreview);
                collarMessagesView.importCollarFields = collarObject.tableData;
                await collarMessagesView.ValidateAllTables(false);

                DataContext = collarMessagesView;

            }
            else if (selectedIndex == 1)
            {
                surveyMessagesView = new SurveyValidationView(DrillholeTableType.survey, surveyObject.xPreview);
                surveyMessagesView.importSurveyFields = surveyObject.tableData;
                surveyMessagesView.importCollarFields = collarObject.tableData;
                surveyMessagesView.xmlCollarData = collarObject.xPreview;
                await surveyMessagesView.ValidateAllTables(false);

                DataContext = surveyMessagesView;

            }
            else if (selectedIndex == 2)
            {
                assayMessagesView = new AssayValidationView(DrillholeTableType.assay, assayObject.xPreview);
                assayMessagesView.importAssayFields = assayObject.tableData;
                assayMessagesView.importCollarFields = collarObject.tableData;
                assayMessagesView.xmlCollarData = collarObject.xPreview;
                await assayMessagesView.ValidateAllTables(false);

                DataContext = assayMessagesView;

            }
            else if (selectedIndex == 3)
            {
                intervalMessagesView = new IntervalValidationView(DrillholeTableType.interval, intervalObject.xPreview);
                intervalMessagesView.importIntervalFields = intervalObject.tableData;
                intervalMessagesView.importCollarFields = collarObject.tableData;
                intervalMessagesView.xmlCollarData = collarObject.xPreview;
                await intervalMessagesView.ValidateAllTables(false);

                DataContext = intervalMessagesView;

            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            m_instance = null;
        }
    }
}

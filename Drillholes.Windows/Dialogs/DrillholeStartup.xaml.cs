using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Drillholes.Windows.Dialogs;
using System.Windows.Controls.Ribbon;
using System.IO;
using Drillholes.Domain.Interfaces;
using Drillholes.Domain.Services;
using Drillholes.XML;
using Drillholes.Domain.Enum;
using Microsoft.VisualBasic;
using System.Reflection;
using Drillholes.Domain;
using Microsoft.Win32;
using System.Xml.Linq;
using System.Windows.Navigation;
using Drillholes.Domain.DataObject;
using System.Collections.Specialized;

namespace Drillholes.Windows.Dialogs
{
    /// <summary>
    /// Interaction logic for DrillholeStartup.xaml
    /// </summary>
    public partial class DrillholeStartup : RibbonWindow
    {
        private XmlService _xmlService;
        private IDrillholeXML _xml;

        private ExportResultsService _exportService;
        private IDrillholeExport _export;

       // private string rootName = "DrillholePreferences";
        private string fullName { get; set; }
        private string xmlName { get; set; }
        private bool[] survey { get; set; }
        private bool[] geology { get; set; }
        private bool savedProject { get; set; }
        private string projectSession { get; set; }
        private string projectLocation { get; set; }
        private string projectFile { get; set; }

        private DrillholeImportFormat exportFormat { get; set; }



        private DrillholeDialogPage dialogPage { get; set; }
        public DrillholeStartup()
        {
            InitializeComponent();

            survey = new bool[3];
            geology = new bool[2];
            savedProject = false;
            projectSession = "";
            exportFormat = DrillholeImportFormat.text_csv;

            dialogPage = new DrillholeDialogPage();
        }

        private async Task<bool> ManageXmlPreferences()
        {
            DrillholePreferences preferences = await SetPreferencesToXml();

            if (!savedProject)
            {
                //get pathname
                if (fullName == "" || fullName == null)
                    fullName = await XmlDefaultPath.GetFullPathAndFilename(DrillholeConstants.drillholePref, "alltables");
            }
            else
            {
                fullName = await XmlDefaultPath.GetProjectPathAndFilename(DrillholeConstants.drillholePref, "alltables", projectSession, projectLocation);
            }

            //create XML temp table
            if (_xml == null)
                _xml = new Drillholes.XML.XmlController();

            if (_xmlService == null)
                _xmlService = new XmlService(_xml);

            await _xmlService.DrillholePreferences(fullName, preferences, DrillholeConstants.drillholePref);

            return true;
        }

        private async void SetupExportService()
        {
            if (_export == null)
                _export = new Drillholes.FileDialog.FileExportDrillholes();

            if (_exportService == null)
                _exportService = new ExportResultsService(_export);
        }

        private async Task<bool> ManageXmlProperties(DrillholeProjectProperties properties)
        {
            //creates .dh project file
            await _xmlService.DrillholeProjectProperties(properties, DrillholeConstants.drillholeProject);

            //update project file with path to preferences file
            if (savedProject)
                _xmlService.DrillholePreferences(projectFile, fullName, DrillholeConstants.drillholeProject, DrillholeTableType.other);

            return true;
        }

        private async Task<bool> ManageXmlTableParameters(DrillholeImportPage page)
        {
            //Create table with paths and parameters

            string fullPathname = await XmlDefaultPath.GetProjectPathAndFilename(DrillholeConstants.drillholeTable, "alltables", projectSession, projectLocation);
            await _xmlService.TableParameters(fullPathname, page.tables, DrillholeConstants.drillholeTable);
            _xmlService.TableParameters(projectLocation + "\\" + projectSession + ".dh", fullPathname, DrillholeConstants.drillholeProject, DrillholeTableType.other);


            //get collar name for fields
            fullPathname = await XmlDefaultPath.GetProjectPathAndFilename("DrillholeFields", DrillholeTableType.collar.ToString(), projectSession, projectLocation);

            //Create table fields for collar and entry into .dh file
            await CreateTableFieldForImportDialogPage(fullPathname, DrillholeTableType.collar, page.collarPreviewModel.collarDataFields, "DrillholeFields");

            fullPathname = await XmlDefaultPath.GetProjectPathAndFilename("DrillholeData", DrillholeTableType.collar.ToString(), projectSession, projectLocation);
            await CreateTableDataForImportDialogPage(fullPathname, page.collarPreviewModel.collarTableObject.xPreview, DrillholeTableType.collar, "DrillholeData");

            if (page.surveyPreviewModel != null)
            {
                if (page.surveyPreviewModel.surveyDataFields != null)
                {
                    fullPathname = await XmlDefaultPath.GetProjectPathAndFilename("DrillholeFields", DrillholeTableType.survey.ToString(), projectSession, projectLocation);
                    await CreateTableFieldForImportDialogPage(fullPathname, DrillholeTableType.survey, page.surveyPreviewModel.surveyDataFields, "DrillholeFields");

                    fullPathname = await XmlDefaultPath.GetProjectPathAndFilename("DrillholeData", DrillholeTableType.survey.ToString(), projectSession, projectLocation);
                    await CreateTableDataForImportDialogPage(fullPathname, page.surveyPreviewModel.surveyTableObject.xPreview, DrillholeTableType.survey, "DrillholeData");
                }
            }

            if (page.assayPreviewModel != null)
            {
                if (page.assayPreviewModel.assayDataFields != null)
                {
                    fullPathname = await XmlDefaultPath.GetProjectPathAndFilename("DrillholeFields", DrillholeTableType.assay.ToString(), projectSession, projectLocation);
                    await CreateTableFieldForImportDialogPage(fullPathname, DrillholeTableType.assay, page.assayPreviewModel.assayDataFields, "DrillholeFields");

                    fullPathname = await XmlDefaultPath.GetProjectPathAndFilename("DrillholeData", DrillholeTableType.assay.ToString(), projectSession, projectLocation);
                    await CreateTableDataForImportDialogPage(fullPathname, page.assayPreviewModel.assayTableObject.xPreview, DrillholeTableType.assay, "DrillholeData");
                }
            }
            if (page.intervalPreviewModel != null)
            {
                if (page.intervalPreviewModel.intervalDataFields != null)
                {
                    fullPathname = await XmlDefaultPath.GetProjectPathAndFilename("DrillholeFields", DrillholeTableType.interval.ToString(), projectSession, projectLocation);
                    await CreateTableFieldForImportDialogPage(fullPathname, DrillholeTableType.interval, page.intervalPreviewModel.intervalDataFields, "DrillholeFields");

                    fullPathname = await XmlDefaultPath.GetProjectPathAndFilename("DrillholeData", DrillholeTableType.interval.ToString(), projectSession, projectLocation);
                    await CreateTableDataForImportDialogPage(fullPathname, page.intervalPreviewModel.intervalTableObject.xPreview, DrillholeTableType.interval, "DrillholeData");
                }
            }

            if (page.continuousPreviewModel != null)
            {
                if (page.continuousPreviewModel.continuousDataFields != null)
                {
                    fullPathname = await XmlDefaultPath.GetProjectPathAndFilename("DrillholeFields", DrillholeTableType.continuous.ToString(), projectSession, projectLocation);
                    await CreateTableFieldForImportDialogPage(fullPathname, DrillholeTableType.continuous, page.continuousPreviewModel.continuousDataFields, "DrillholeFields");

                    fullPathname = await XmlDefaultPath.GetProjectPathAndFilename("DrillholeData", DrillholeTableType.continuous.ToString(), projectSession, projectLocation);
                    await CreateTableDataForImportDialogPage(fullPathname, page.continuousPreviewModel.continuousTableObject.xPreview, DrillholeTableType.continuous, "DrillholeData");
                }
            }

            return true;
        }

        private async Task<bool> CreateTableFieldForImportDialogPage(string fullPathname, DrillholeTableType tableType, ImportTableFields importFields, string rootName)
        {
            await _xmlService.DrillholeFields(fullPathname, importFields, tableType, rootName);

            _xmlService.DrillholeFields(projectLocation + "\\" + projectSession + ".dh", fullPathname, DrillholeConstants.drillholeProject, tableType);

            return true;
        }

        private async Task<bool> CreateTableDataForImportDialogPage(string fullPathname, XElement xPreview, DrillholeTableType tableType, string rootName)
        {
            string xmlNodeName = "";
            if (tableType == DrillholeTableType.collar)
            {
                xmlNodeName = "Collars";
            }
            else if (tableType == DrillholeTableType.survey)
            {
                xmlNodeName = "Surveys";
            }
            else if (tableType == DrillholeTableType.assay)
            {
                xmlNodeName = "Assays";
            }
            else if (tableType == DrillholeTableType.interval)
            {
                xmlNodeName = "Intervals";
            }
            else if (tableType == DrillholeTableType.continuous)
            {
                xmlNodeName = "Continuous";
            }

            await _xmlService.DrillholeData(fullPathname, xPreview, tableType, xmlNodeName, rootName);

            _xmlService.DrillholeData(projectLocation + "\\" + projectSession + ".dh", fullPathname, DrillholeConstants.drillholeProject, tableType);

            return true;
        }

        #region Save Session
        private void Save_Session(object sender, RoutedEventArgs e)
        {
            // SaveDrillholeSession();
        }
        private async void Save_SessionAs(object sender, RoutedEventArgs e)
        {

            bool saved = await SaveDrillholeSession();

            if (!saved)
                return;

            await ManageXmlPreferences();

            var whichPage = frameMain.Content as Page;

                DrillholeProjectProperties properties = new DrillholeProjectProperties()
                {
                    ProjectName = projectSession,
                    ProjectParentFolder = projectLocation,
                    ProjectFolder = projectLocation + "\\" + projectSession,
                    ProjectFile = projectFile
                };

                dialogPage.savedSession = true;
                dialogPage.xmlProjectFile = properties.ProjectFile;
                dialogPage.projectSession = properties.ProjectName;
                dialogPage.projectLocation = properties.ProjectFolder;

                await ManageXmlProperties(properties);

                if (projectFile == "" || projectFile == null)
                    projectFile = await CheckProjectFile();

            if (whichPage.Title == "Import Drillhole Data")
            {
                var pageDialog = frameMain.Content as DrillholeDialogPage;
                pageDialog.savedSession = true;
                pageDialog.projectSession = projectSession;
                pageDialog.projectLocation = projectLocation;
                pageDialog.xmlProjectFile = projectFile;

            }

            else if (whichPage.Title == "Select Drillhole Fields")
            {
                //create Table properties
                var pageImport = frameMain.Content as DrillholeImportPage;

                pageImport.savedSession = true;

                await ManageXmlTableParameters(pageImport);

                //update settings
             //   await SynchroniseSettings(true, pageImport);

                await ManageXmlPreferences();
            }

        }

        private async Task<bool> SaveDrillholeSession()
        {
            string strHeader = "Save Drillhole Session";

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = strHeader;
            saveFileDialog.Filter = "Drillhole Session (*.dh)|*.dh|All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.CreatePrompt = false;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.AddExtension = false;

            if (saveFileDialog.ShowDialog() == true)
            {
                FileInfo info = new FileInfo(saveFileDialog.FileName);
                projectLocation = info.DirectoryName;
                string sessionName = info.Name;
                string extension = info.Extension;

                projectFile = projectLocation + "\\" + sessionName;

                //check if file exists
                if (info.Exists)
                {
                    MessageBoxResult fileExists = MessageBox.Show("File '" + sessionName + "' already exists. Overwrite existing file?", "Save File", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                    if (MessageBoxResult.Cancel == fileExists)
                        return false;
                    else if (MessageBoxResult.No == fileExists)
                        return false;
                }

                //strip off extension
                projectSession = sessionName.Substring(0, sessionName.Length - 3);

                //check if existing directory
                DirectoryInfo dirInfo = new DirectoryInfo(projectLocation);

                var directories = dirInfo.EnumerateDirectories();

                var query = directories.Where(d => d.Name == projectSession).Count();

                if (query > 0)
                {
                    MessageBox.Show("A project directory of the same name '" + projectSession + "' already exists. Please select a diffrent session name.", "Project Directory", MessageBoxButton.OK, MessageBoxImage.Information);

                    return false;
                }
                else //make project directory to store xml files
                {
                    dirInfo.CreateSubdirectory(projectSession);
                }

                savedProject = true;

                return true;

            };

            return false;
        }


        private void SaveProjectFile()
        {

        }
        #endregion

        #region Open Session
        private async void Open_Session(object sender, RoutedEventArgs e)
        {
            bool bOpen = await OpenDrillholeSession();

            if (!bOpen)
                return;

            projectFile = await CheckProjectFile();

            if (projectFile != "")
            {
                string filename = await CheckForDrillholeTableXml(projectFile);

                if (filename == "")
                {
                    dialogPage.savedSession = true;
                    dialogPage.xmlProjectFile = projectFile;
                    dialogPage.projectSession = projectSession;
                    dialogPage.projectLocation = projectLocation;

                    frameMain.Navigate(dialogPage);
                    return;
                }

                //if table properties stored then proceed
                List<DrillholeTable> _classes = new List<DrillholeTable>();

                _classes = await RetrieveDrillholeTableParameters(projectFile, filename);

                var checkPage = frameMain.Content as Page;
                if (checkPage.Title == "Import Drillhole Data")
                {
                    if (_classes != null)
                    {
                        DrillholeImportPage newPage = new DrillholeImportPage(_classes, true, projectFile, projectSession, projectLocation);
                        newPage.openSession = true;

                        frameMain.Navigate(newPage);
                    }
                    else
                    {
                        var dialogImport = frameMain.Content as DrillholeDialogPage;
                        dialogImport.savedSession = true;
                        dialogPage.xmlProjectFile = projectFile;
                        dialogPage.projectSession = projectSession;
                        dialogPage.projectLocation = projectLocation;
                    }
                }
                
            }
            else
            {
                var dialogImport = frameMain.Content as DrillholeDialogPage;
                dialogImport.savedSession = true;
                dialogPage.xmlProjectFile = projectFile;
                dialogPage.projectSession = projectSession;
                dialogPage.projectLocation = projectLocation;
            }

        }

        private async Task<string> CheckProjectFile()
        {
            //create XML temp table
            if (_xml == null)
                _xml = new Drillholes.XML.XmlController();

            if (_xmlService == null)
                _xmlService = new XmlService(_xml);

            if (File.Exists(projectLocation + "\\" + projectSession + ".dh"))
            {
                return projectLocation + "\\" + projectSession + ".dh";

            }

            return "";

        }

        private async Task<string> CheckForDrillholeTableXml(string projectFile)
        {
            DrillholePreferences preferences = null;

            XDocument xmlPreferences = await _xmlService.DrillholePreferences(projectFile, preferences, DrillholeConstants.drillholeProject);

            
            var elements = xmlPreferences.Descendants(DrillholeConstants.drillholeProject).Elements();

            
                var check = elements.Where(a => a.Element(DrillholeConstants.drillholeTable).Value != "").Count();

            string checkName = "";

            if (check > 0)
                {
                 checkName = elements.Select(a => a.Element(DrillholeConstants.drillholeTable)).SingleOrDefault().Value;
            }
            
            return checkName;
        }

        private async Task<List<DrillholeTable>> RetrieveDrillholeTableParameters(string projectFile, string drillholeTableFile)
        {
            List<DrillholeTable> tables = new List<DrillholeTable>();

            if (_xml == null)
                _xml = new Drillholes.XML.XmlController();

            if (_xmlService == null)
                _xmlService = new XmlService(_xml);

            tables = await _xmlService.TableParameters(projectFile, drillholeTableFile, DrillholeConstants.drillholeProject, DrillholeConstants.drillholeTable, DrillholeTableType.other);

            return tables;
        }

        private async Task<bool> OpenDrillholeSession()
        {
            string strHeader = "Open Drillhole Session";

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Drillhole Session (*.dh)|*.dh|All files (*.*)|*.*";
            openFileDialog.Title = strHeader;

            if (openFileDialog.ShowDialog() == true)
            {
                FileInfo info = new FileInfo(openFileDialog.FileName);
                projectLocation = info.DirectoryName;
                string sessionName = info.Name;
                string extension = info.Extension;

                projectSession = sessionName.Substring(0, sessionName.Length - 3);

                savedProject = true;

                SetEnvironmentFromXml(DrillholeConstants.drillholePref);
            }
            else
                return false;

            return true;

        }
        #endregion
        private void ExitForm(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void HelpDocumentation(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Not yet implemented");
        }

        private void New_Session(object sender, RoutedEventArgs e)
        {

            frameMain.Navigate(new DrillholeDialogPage(false, "", "", ""));
        }


        #region Ribbon 
        private async void Ribbon_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var ribbon = sender as Ribbon;

            if (frameMain == null)
                return;

            var whichPage = frameMain.Content as Page;

            DrillholeImportPage importPage = null;

            if (whichPage.Title == "Select Drillhole Fields" )
            {
                importPage = frameMain.Content as DrillholeImportPage;
            }

            if (ribbon.SelectedIndex == 0)
            {
                stkLabel.Visibility = Visibility.Visible;
                frameMain.Visibility = Visibility.Visible;

                stkCheckbox.Visibility = Visibility.Hidden;
                stkTolerance.Visibility = Visibility.Hidden;

                //check if import page then automatically reload preferences and refresh interface if required
                if (importPage != null)
                {
                   // await SynchroniseSettings(false, importPage);
                    await ManageXmlPreferences();
                }

            }
            else  //FIX
            {
                stkLabel.Visibility = Visibility.Hidden;
                frameMain.Visibility = Visibility.Hidden;

                stkCheckbox.Visibility = Visibility.Visible;
                stkTolerance.Visibility = Visibility.Visible;

                //if (importPage != null)
                //await SynchroniseSettings(true, importPage);

                await ManageXmlPreferences();

                //set interface from XML
                SetEnvironmentFromXml("DrillholePreferences");
            }
        }

        private async void RibbonWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await ManageXmlPreferences();
        }
        #endregion

        #region Preferences

        #region Preference controls


        private async void chkCollarColumns_Click(object sender, RoutedEventArgs e)
        {
            xmlName = "ImportCollarColumns";
            UpdateXmlPreferences((bool)chkCollarColumns.IsChecked);

            await ImportColumns(DrillholeTableType.collar, (bool)chkCollarColumns.IsChecked);
        }

        private async void chkSurveyColumns_Click(object sender, RoutedEventArgs e)
        {
            xmlName = "ImportSurveyColumns";
            UpdateXmlPreferences((bool)chkSurveyColumns.IsChecked);

            await ImportColumns(DrillholeTableType.survey, (bool)chkSurveyColumns.IsChecked);
        }

        private async void chkAssayColumns_Click(object sender, RoutedEventArgs e)
        {
            xmlName = "ImportAssayColumns";
            UpdateXmlPreferences((bool)chkAssayColumns.IsChecked);
            await ImportColumns(DrillholeTableType.assay, (bool)chkAssayColumns.IsChecked);

        }

        private async void chkIntervalColumns_Click(object sender, RoutedEventArgs e)
        {
            xmlName = "ImportIntervalColumns";
            UpdateXmlPreferences((bool)chkIntervalColumns.IsChecked);
            await ImportColumns(DrillholeTableType.interval, (bool)chkIntervalColumns.IsChecked);

        }

        private async void chkContinuousColumns_Click(object sender, RoutedEventArgs e)
        {
            xmlName = "ImportContinuousColumns";
            UpdateXmlPreferences((bool)chkContinuousColumns.IsChecked);
            await ImportColumns(DrillholeTableType.continuous, (bool)chkContinuousColumns.IsChecked);

        }

        private async Task<bool> ImportColumns(DrillholeTableType tableType, bool isChecked)
        {
            var whichpage = frameMain.Content;

            var test = whichpage.GetType();

            if (test.Name == "DrillholeImportPage")
            {
                DrillholeImportPage thisPage = whichpage as DrillholeImportPage;
                await thisPage.ImportColumns(tableType, isChecked);

                return true;
            }
            else
                return false;
        }

        private void chkDip_Click(object sender, RoutedEventArgs e)
        {
            xmlName = "NegativeDip";

            UpdateXmlPreferences((bool)chkDip.IsChecked);

        }

        private void chkIgnore_Click(object sender, RoutedEventArgs e)
        {
            xmlName = "IgnoreInvalidValues";

            UpdateXmlPreferences((bool)chkIgnore.IsChecked);

        }

        private void chkDetection_Click(object sender, RoutedEventArgs e)
        {
            xmlName = "LowerDetection";
            bool detection = (bool)chkDetection.IsChecked;

            UpdateXmlPreferences((bool)chkDetection.IsChecked);

        }

        private void SurveyTypeXML()
        {
            xmlName = "SurveyType";

            if ((bool)radDownhole.IsChecked)
            {
                survey[0] = true;
                survey[1] = false;
                survey[2] = false;
            }
            else if ((bool)radCollarSurvey.IsChecked)
            {
                survey[0] = false;
                survey[1] = true;
                survey[2] = false;
            }
            else if ((bool)radVertical.IsChecked)
            {
                survey[0] = false;
                survey[1] = false;
                survey[2] = true;
            }

            UpdateXmlSurveyPreferences(survey);

        }

        private void radDownhole_Click(object sender, RoutedEventArgs e)
        {
            SurveyTypeXML();
        }

        private void radCollarSurvey_Click(object sender, RoutedEventArgs e)
        {
            SurveyTypeXML();

        }

        private void radVertical_Click(object sender, RoutedEventArgs e)
        {
            SurveyTypeXML();

        }

        private void chkImportDeviation_Click(object sender, RoutedEventArgs e)
        {
            xmlName = "ImportSurveyOnly";

            UpdateXmlPreferences((bool)chkImportDeviation.IsChecked);

        }

        private void chkImportAssays_Click(object sender, RoutedEventArgs e)
        {
            xmlName = "ImportAssayOnly";

            UpdateXmlPreferences((bool)chkImportAssays.IsChecked);

        }

        private void chkImportGeology_Click(object sender, RoutedEventArgs e)
        {
            xmlName = "ImportGeologyOnly";

            UpdateXmlPreferences((bool)chkImportGeology.IsChecked);

        }

        private void chkImportContinuous_Click(object sender, RoutedEventArgs e)
        {
            xmlName = "ImportContinuousOnly";

            UpdateXmlPreferences((bool)chkImportContinuous.IsChecked);

        }

        private void chkZeroAssays_Click(object sender, RoutedEventArgs e)
        {
            xmlName = "NullifyZeroAssays";

            UpdateXmlPreferences((bool)chkZeroAssays.IsChecked);

        }

        private void SetGeologyTopOrBottom()
        {
            xmlName = "GeologyBase";

            if ((bool)radBottom.IsChecked)
            {
                geology[0] = true;
            }
            else if ((bool)radTop.IsChecked)
            {
                geology[1] = true;
            }

            UpdateXmlGeologyContactPreferences(geology);

        }

        private void radTop_Click(object sender, RoutedEventArgs e)
        {
            SetGeologyTopOrBottom();
        }

        private void radBottom_Click(object sender, RoutedEventArgs e)
        {
            SetGeologyTopOrBottom();
        }

        private void chkCollar_Click(object sender, RoutedEventArgs e)
        {
            xmlName = "CreateCollar";

            UpdateXmlPreferences((bool)chkCollar.IsChecked);

        }

        private void cboDesurvey_DropDownClosed(object sender, EventArgs e)
        {
            //var test = sender as RibbonComboBox;


            //if (test.Text != "")
            //{
            //    xmlName = "DesurveyMethod";
            //    UpdateXmlPreferences(test.Text);
            //}
        }

        private void chkToe_Click(object sender, RoutedEventArgs e)
        {
            xmlName = "CreateToe";

            UpdateXmlPreferences((bool)chkToe.IsChecked);

        }

        private void radTopCore_Click(object sender, RoutedEventArgs e)
        {
            xmlName = "TopCore";

            UpdateXmlPreferences((bool)radTopCore.IsChecked);
        }

        private void radBottomCore_Click(object sender, RoutedEventArgs e)
        {
            xmlName = "BottomCore";

            UpdateXmlPreferences((bool)radBotCore.IsChecked);
        }

        private void chkAlphaBeta_Click(object sender, RoutedEventArgs e)
        {
            xmlName = "CalculateStructures";

            UpdateXmlPreferences((bool)chkAlphaBeta.IsChecked);
        }

        private void txtDipTol_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Information.IsNothing(txtDipTol.Text))
            {
                MessageBox.Show("Please enter a valid number between 0 and 90", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (!Information.IsNumeric(txtDipTol.Text))
            {
                MessageBox.Show("Please enter a valid number between 0 and 90", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            double range = Convert.ToDouble(txtDipTol.Text);
            if (range < 0)
            {
                MessageBox.Show("Cannot be less than zero. Please enter a valid number between 0 and 90", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (range > 90)
            {
                MessageBox.Show("Cannot be greater than 90. Please enter a valid number between 0 and 90", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            xmlName = "DipTolerance";

            UpdateXmlPreferences(txtDipTol.Text);

        }

        private void txtAziTol_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Information.IsNothing(txtAziTol.Text))
            {
                MessageBox.Show("Please enter a valid number between 0 and 360", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (!Information.IsNumeric(txtAziTol.Text))
            {
                MessageBox.Show("Please enter a valid number between 0 and 360", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            double range = Convert.ToDouble(txtAziTol.Text);
            if (range < 0)
            {
                MessageBox.Show("Cannot be less than zero. Please enter a valid number between 0 and 360", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (range > 360)
            {
                MessageBox.Show("Cannot be greater than 360. Please enter a valid number between 0 and 360", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            xmlName = "AziTolerance";

            UpdateXmlPreferences(txtAziTol.Text);
        }

        private void txtDefault_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Information.IsNothing(txtDefault.Text))
            {
                MessageBox.Show("Please enter a valid number", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (!Information.IsNumeric(txtDefault.Text))
            {
                MessageBox.Show("Please enter a valid number", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            xmlName = "DefaultValue";

            UpdateXmlPreferences(txtDefault.Text);
        }
        #endregion

        #region XML 
        private async Task<DrillholePreferences> SetPreferencesToXml()
        {
            DrillholePreferences preferences = new DrillholePreferences()
            {
                NegativeDip = (bool)this.chkDip.IsChecked,
                IgnoreInvalidValues = (bool)this.chkIgnore.IsChecked,
                LowerDetection = (bool)this.chkDetection.IsChecked,
                ImportSurveyOnly = (bool)this.chkImportDeviation.IsChecked,
                ImportAssayOnly = (bool)this.chkImportAssays.IsChecked,
                ImportGeologyOnly = (bool)this.chkImportGeology.IsChecked,
                ImportContinuousOnly = (bool)this.chkImportContinuous.IsChecked,
                ImportCollarColumns = (bool) this.chkCollarColumns.IsChecked,
                ImportSurveyColumns = (bool) this.chkSurveyColumns.IsChecked,
                ImportAssayColumns = (bool) this.chkAssayColumns.IsChecked,
                ImportIntervalColumns = (bool) this.chkIntervalColumns.IsChecked,
                ImportContinuousColumns = (bool) this.chkContinuousColumns.IsChecked,
                NullifyZeroAssays = (bool)this.chkZeroAssays.IsChecked,
                CreateCollar = (bool)this.chkCollar.IsChecked,
                CreateToe = (bool)this.chkToe.IsChecked,
                TopCore = (bool)this.radTopCore.IsChecked,
                BottomCore = (bool)this.radBotCore.IsChecked,
                CalculateStructures = (bool)this.chkAlphaBeta.IsChecked
            };

            if ((bool)radDownhole.IsChecked)
            {
                preferences.surveyType = DrillholeSurveyType.downholesurvey;
            }
            else if ((bool)radVertical.IsChecked)
            {
                preferences.surveyType = DrillholeSurveyType.vertical;
            }
            else if ((bool)radCollarSurvey.IsChecked)
            {
                preferences.surveyType = DrillholeSurveyType.collarsurvey;
            }

            if ((bool)radBottom.IsChecked)
            {
                preferences.GeologyBase = true;
            }
            else if ((bool)radTop.IsChecked)
            {
                preferences.GeologyBase = false;
            }

            if (Information.IsNumeric(txtAziTol.Text))
                preferences.AzimuthTolerance = Convert.ToDouble(txtAziTol.Text);
            else
                preferences.AzimuthTolerance = 10.0; //default

            if (Information.IsNumeric(txtDipTol.Text))
                preferences.DipTolerance = Convert.ToDouble(txtDipTol.Text);
            else
                preferences.DipTolerance = 5.0; //default

            if (Information.IsNumeric(txtDefault.Text))
                preferences.DefaultValue = Convert.ToDouble(txtDefault.Text);
            else
                preferences.DefaultValue = -99.0; //default

            //ComboBox text value uses the previous value if the user selects different method. Therefore must use selected item instead
            var value = cboDesurvey.SelectedItem as ComboBoxItem;

            DrillholeDesurveyEnum surveyMethod = DrillholeDesurveyEnum.Tangential;

            if (value != null)
            {
                string selectedValue = value.Content.ToString();

                if (selectedValue == DrillholeDesurveyEnum.BalancedTangential.ToString())
                    surveyMethod = DrillholeDesurveyEnum.BalancedTangential;
                else if (selectedValue == DrillholeDesurveyEnum.MinimumCurvature.ToString())
                    surveyMethod = DrillholeDesurveyEnum.MinimumCurvature;
                else if (selectedValue == DrillholeDesurveyEnum.RadiusCurvature.ToString())
                    surveyMethod = DrillholeDesurveyEnum.RadiusCurvature;
                else if (selectedValue == DrillholeDesurveyEnum.Tangential.ToString())
                    surveyMethod = DrillholeDesurveyEnum.Tangential;
            }

            preferences.DesurveyMethod = surveyMethod; 

            return preferences;
        }

        private async void SetEnvironmentFromXml(string rootName)
        {
            if (savedProject)
                fullName = await XmlDefaultPath.GetProjectPathAndFilename(rootName, "alltables", projectSession, projectLocation);
            else
                fullName = await XmlDefaultPath.GetFullPathAndFilename(DrillholeConstants.drillholePref, "allTables");

            //create XML temp table
            if (_xml == null)
                _xml = new Drillholes.XML.XmlController();

            if (_xmlService == null)
                _xmlService = new XmlService(_xml);

            DrillholePreferences preferences = null;

            XDocument xmlPreferences = await _xmlService.DrillholePreferences(fullName, preferences, DrillholeConstants.drillholePref);

            var elements = xmlPreferences.Descendants(rootName).Elements();

            var updateValues = elements.Where(p => p.Attribute("Value").Value == "exists");

            foreach (var value in updateValues)
            {
                var dip = value.Element("NegativeDip").Value;

                chkDip.IsChecked = Convert.ToBoolean(dip);
                chkIgnore.IsChecked = Convert.ToBoolean(value.Element("IgnoreInvalidValues").Value);
                chkDetection.IsChecked = Convert.ToBoolean(value.Element("LowerDetection").Value);
                chkImportDeviation.IsChecked = Convert.ToBoolean(value.Element("ImportSurveyOnly").Value);
                chkImportAssays.IsChecked = Convert.ToBoolean(value.Element("ImportAssayOnly").Value);
                chkImportGeology.IsChecked = Convert.ToBoolean(value.Element("ImportGeologyOnly").Value);
                chkImportContinuous.IsChecked = Convert.ToBoolean(value.Element("ImportContinuousOnly").Value);
                chkZeroAssays.IsChecked = Convert.ToBoolean(value.Element("NullifyZeroAssays").Value);
                chkCollar.IsChecked = Convert.ToBoolean(value.Element("CreateCollar").Value);
                chkToe.IsChecked = Convert.ToBoolean(value.Element("CreateToe").Value);
                radTopCore.IsChecked = Convert.ToBoolean(value.Element("TopCore").Value);
                radBotCore.IsChecked = Convert.ToBoolean(value.Element("BottomCore").Value);
                chkAlphaBeta.IsChecked = Convert.ToBoolean(value.Element("CalculateStructures").Value);
                chkCollarColumns.IsChecked = Convert.ToBoolean(value.Element("ImportCollarColumns").Value);
                chkSurveyColumns.IsChecked = Convert.ToBoolean(value.Element("ImportSurveyColumns").Value);
                chkAssayColumns.IsChecked = Convert.ToBoolean(value.Element("ImportAssayColumns").Value);
                chkIntervalColumns.IsChecked = Convert.ToBoolean(value.Element("ImportIntervalColumns").Value);
                chkContinuousColumns.IsChecked = Convert.ToBoolean(value.Element("ImportContinuousColumns").Value);

                var geologybase = value.Element("GeologyBase").Value;

                if (Convert.ToBoolean(geologybase))
                    radBottom.IsChecked = true;
                else
                    radTop.IsChecked = true;

                var surveyType = value.Element("SurveyType").Value;

                if (surveyType == DrillholeSurveyType.collarsurvey.ToString())
                {
                    radCollarSurvey.IsChecked = true;
                }
                else if (surveyType == DrillholeSurveyType.downholesurvey.ToString())
                {
                    radDownhole.IsChecked = true;
                }
                else if (surveyType == DrillholeSurveyType.vertical.ToString())
                {
                    radVertical.IsChecked = true;
                }

                var defaultValue = value.Element("DefaultValue").Value;
                var dipTol = value.Element("DipTolerance").Value;
                var aziTol = value.Element("AziTolerance").Value;
                var desurv = value.Element("DesurveyMethod").Value;

                txtAziTol.Text = aziTol.ToString();
                txtDipTol.Text = dipTol.ToString();
                txtDefault.Text = defaultValue.ToString();

                cboDesurvey.Text = desurv.ToString();
            }

        }

        private async void UpdateXmlPreferences(bool bValue)
        {
            await _xmlService.DrillholePreferences(fullName, xmlName, bValue, DrillholeConstants.drillholePref);
        }

        private async void UpdateXmlSurveyPreferences(bool[] bValue)
        {
            
            if (bValue[0])
            {
                await _xmlService.DrillholePreferences(fullName, xmlName, DrillholeSurveyType.downholesurvey, DrillholeConstants.drillholePref);

            }
            else if (bValue[1])
            {
                await _xmlService.DrillholePreferences(fullName, xmlName, DrillholeSurveyType.collarsurvey, DrillholeConstants.drillholePref);

            }
            else
            {
                await _xmlService.DrillholePreferences(fullName, xmlName, DrillholeSurveyType.vertical, DrillholeConstants.drillholePref);

            }

        }

        private async void UpdateXmlGeologyContactPreferences(bool[] bValue)
        {

            if (bValue[0])
            {
                await _xmlService.DrillholePreferences(fullName, xmlName, true, DrillholeConstants.drillholePref);

            }
            else if (bValue[1])
            {
                await _xmlService.DrillholePreferences(fullName, xmlName, false, DrillholeConstants.drillholePref);

            }

        }
        private async void UpdateXmlPreferences(string xmlValue)
        {
            await _xmlService.DrillholePreferences(fullName, xmlName, xmlValue, DrillholeConstants.drillholePref);
        }

        #endregion

        #endregion

        private void Desurvey_Changed(object sender, SelectionChangedEventArgs e)
        {
            var test = sender as ComboBox;
            var value = test.SelectedItem as ComboBoxItem;

            if (value == null)
                return;

            string selectedValue = value.Content.ToString();

            if (test.Text != "")
            {
                xmlName = "DesurveyMethod";
                UpdateXmlPreferences(selectedValue);
            }
        }

        private void frameMain_Navigated(object sender, NavigationEventArgs e)
        {
            var whichPage = frameMain.Content as Page;

            DrillholeImportPage importPage = null;

            if (whichPage.Title == "Select Drillhole Fields")
            {
                importPage = frameMain.Content as DrillholeImportPage;
                if (importPage.collarPreviewModel.collarDataFields != null)
                    importPage.RefreshPreviewData();
            }
            else
                return;
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            string strFilter = "";

            if ((bool)radExcel.IsChecked)
            {
                MessageBox.Show("Sorry, but 'Export to Excel' has not yet been implemented!", "Export", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            else if ((bool) radText.IsChecked)
            {

                ExportToText("Drillhole Data (*.csv)| *.csv |Drillhole Data (*.txt)| *.txt |All files(*.*) | *.* ");
            }
            else if ((bool)radDatabase.IsChecked)
            {
                MessageBox.Show("Sorry, but 'Export to Database' has not yet been implemented!", "Export", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
        }

        private async void ExportToText(string filter)
        {
            string outputName = "C:\\Users\\mcunningham\\source\\Workspaces\\test.csv";
            //outputName = await ExportDataName(filter);

            if (outputName == "")
                return;

            //get table type
            var whichpage = frameMain.Content;

            var test = whichpage.GetType();

            DrillholeTableType tableType = DrillholeTableType.collar;

            if (test.Name == "DrillholeImportPage")
            {
                DrillholeImportPage thisPage = whichpage as DrillholeImportPage;

                int selected = thisPage._tabcontrol.SelectedIndex;

                if (selected == 1)
                {
                    tableType = DrillholeTableType.survey;
                }
                else if (selected == 2)
                {
                    tableType = DrillholeTableType.assay;

                }
                else if (selected == 3)
                {
                    tableType = DrillholeTableType.interval;

                }
                else if (selected == 4)
                {
                    tableType = DrillholeTableType.continuous;

                }

                //TODO export all desurveyed tables
            }
            else
                tableType = DrillholeTableType.other;

            string drillholeName = "", drillholeFields = "", drillholeInputData = "";

            if (savedProject)
            {
                drillholeName = await XmlDefaultPath.GetProjectPathAndFilename(DrillholeConstants.drillholeDesurv, tableType.ToString(), projectSession, projectLocation);
                drillholeFields = await XmlDefaultPath.GetProjectPathAndFilename(DrillholeConstants.drillholeFields, tableType.ToString(), projectSession, projectLocation);
                drillholeInputData = await XmlDefaultPath.GetProjectPathAndFilename(DrillholeConstants.drillholeData, tableType.ToString(), projectSession, projectLocation);
            }
            else
            {
                drillholeName = await XmlDefaultPath.GetFullPathAndFilename(DrillholeConstants.drillholeDesurv, tableType.ToString());
                drillholeFields = await XmlDefaultPath.GetFullPathAndFilename(DrillholeConstants.drillholeFields, tableType.ToString());
                drillholeInputData = await XmlDefaultPath.GetFullPathAndFilename(DrillholeConstants.drillholeData, tableType.ToString());

            }

            SetupExportService();

            await _exportService.ExportTextCsv(outputName, drillholeName, drillholeFields, drillholeInputData, exportFormat, true);

        }

        private async Task<string> ExportDataName(string filter)
        {
            string strHeader = "Export Results";

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = strHeader;
            saveFileDialog.Filter = filter;
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.CreatePrompt = false;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.AddExtension = false;

            string outputName = "";
            if (saveFileDialog.ShowDialog() == true)
            {
                outputName = saveFileDialog.FileName;

                FileInfo info = new FileInfo(outputName);
                string sessionName = info.Name;

                //check if file exists
                if (info.Exists)
                {
                    MessageBoxResult fileExists = MessageBox.Show("File '" + sessionName + "' already exists. Overwrite existing file?", "Save File", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                    if (MessageBoxResult.Cancel == fileExists)
                        return "";
                    else if (MessageBoxResult.No == fileExists)
                        return "";
                }
            }

            return outputName;
        }

        private async void SaveOutputFromDesurvey(string projectFile, string drillholeTableFile)
        {
            List<DrillholeTable> tables = new List<DrillholeTable>();

            if (_xml == null)
                _xml = new Drillholes.XML.XmlController();

            if (_xmlService == null)
                _xmlService = new XmlService(_xml);

            tables = await _xmlService.TableParameters(projectFile, drillholeTableFile, DrillholeConstants.drillholeProject, DrillholeConstants.drillholeTable, DrillholeTableType.other);

        }

        private void radDatabase_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)radText.IsChecked)
                exportFormat = DrillholeImportFormat.egdb_table;
        }

        private void radExcel_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)radText.IsChecked)
                exportFormat = DrillholeImportFormat.excel_table;
        }

        private void radText_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)radText.IsChecked) 
                exportFormat= DrillholeImportFormat.text_csv;
        }


    }


}

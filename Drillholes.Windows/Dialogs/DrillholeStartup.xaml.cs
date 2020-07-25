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

namespace Drillholes.Windows.Dialogs
{
    /// <summary>
    /// Interaction logic for DrillholeStartup.xaml
    /// </summary>
    public partial class DrillholeStartup : RibbonWindow
    {
        private XmlService _xmlService;
        private IDrillholeXML _xml;
       // private string rootName = "DrillholePreferences";
        private string fullName { get; set; }
        private string xmlName { get; set; }
        private bool[] survey { get; set; }
        private bool[] geology { get; set; }
        private bool savedProject { get; set; }
        private string projectSession { get; set; }
        private string projectLocation { get; set; }
        private string projectFile { get; set; }
        private DrillholeDialogPage dialogPage { get; set; }
        public DrillholeStartup()
        {
            InitializeComponent();

            survey = new bool[3];
            geology = new bool[2];
            savedProject = false;
            projectSession = "";
            dialogPage = new DrillholeDialogPage();
        }

        private async Task<bool> ManageXmlPreferences()
        {
            DrillholePreferences preferences = await SetPreferencesToXml();

            if (!savedProject)
            {
                //get pathname
                if (fullName == "" || fullName == null)
                    fullName = XmlDefaultPath.GetFullPathAndFilename(DrillholeConstants.drillholePref, "alltables");
            }
            else
            {
                fullName = XmlDefaultPath.GetProjectPathAndFilename(DrillholeConstants.drillholePref, "alltables", projectSession, projectLocation);
            }

            //create XML temp table
            if (_xml == null)
                _xml = new Drillholes.XML.XmlController();

            if (_xmlService == null)
                _xmlService = new XmlService(_xml);

            await _xmlService.DrillholePreferences(fullName, preferences, DrillholeConstants.drillholePref);

            return true;
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
            //get collar name for fields
            string fullPathname = XmlDefaultPath.GetProjectPathAndFilename("DrillholeFields", DrillholeTableType.collar.ToString(), projectSession, projectLocation);

            //Create table fields for collar and entry into .dh file
            await CreateTableFieldForImportDialogPage(fullPathname, DrillholeTableType.collar, page.collarPreviewModel.collarDataFields, "DrillholeFields");

            fullPathname = XmlDefaultPath.GetProjectPathAndFilename("DrillholeData", DrillholeTableType.collar.ToString(), projectSession, projectLocation);
            await CreateTableDataForImportDialogPage(fullPathname, page.collarPreviewModel.collarTableObject.xPreview, DrillholeTableType.collar, "DrillholeData");

            if (page.surveyPreviewModel != null)
            {
                if (page.surveyPreviewModel.surveyDataFields != null)
                {
                    fullPathname = XmlDefaultPath.GetProjectPathAndFilename("DrillholeFields", DrillholeTableType.survey.ToString(), projectSession, projectLocation);
                    await CreateTableFieldForImportDialogPage(fullPathname, DrillholeTableType.survey, page.surveyPreviewModel.surveyDataFields, "DrillholeFields");

                    fullPathname = XmlDefaultPath.GetProjectPathAndFilename("DrillholeData", DrillholeTableType.survey.ToString(), projectSession, projectLocation);
                    await CreateTableDataForImportDialogPage(fullPathname, page.surveyPreviewModel.surveyTableObject.xPreview, DrillholeTableType.survey, "DrillholeData");
                }
            }

            if (page.assayPreviewModel != null)
            {
                if (page.assayPreviewModel.assayDataFields != null)
                {
                    fullPathname = XmlDefaultPath.GetProjectPathAndFilename("DrillholeFields", DrillholeTableType.assay.ToString(), projectSession, projectLocation);
                    await CreateTableFieldForImportDialogPage(fullPathname, DrillholeTableType.assay, page.assayPreviewModel.assayDataFields, "DrillholeFields");

                    fullPathname = XmlDefaultPath.GetProjectPathAndFilename("DrillholeData", DrillholeTableType.assay.ToString(), projectSession, projectLocation);
                    await CreateTableDataForImportDialogPage(fullPathname, page.assayPreviewModel.assayTableObject.xPreview, DrillholeTableType.assay, "DrillholeData");
                }
            }
            if (page.intervalPreviewModel != null)
            {
                if (page.intervalPreviewModel.intervalDataFields != null)
                {
                    fullPathname = XmlDefaultPath.GetProjectPathAndFilename("DrillholeFields", DrillholeTableType.interval.ToString(), projectSession, projectLocation);
                    await CreateTableFieldForImportDialogPage(fullPathname, DrillholeTableType.interval, page.intervalPreviewModel.intervalDataFields, "DrillholeFields");

                    fullPathname = XmlDefaultPath.GetProjectPathAndFilename("DrillholeData", DrillholeTableType.interval.ToString(), projectSession, projectLocation);
                    await CreateTableDataForImportDialogPage(fullPathname, page.intervalPreviewModel.intervalTableObject.xPreview, DrillholeTableType.interval, "DrillholeData");
                }
            }

            if (page.continuousPreviewModel != null)
            {
                if (page.continuousPreviewModel.continuousDataFields != null)
                {
                    fullPathname = XmlDefaultPath.GetProjectPathAndFilename("DrillholeFields", DrillholeTableType.continuous.ToString(), projectSession, projectLocation);
                    await CreateTableFieldForImportDialogPage(fullPathname, DrillholeTableType.continuous, page.continuousPreviewModel.continuousDataFields, "DrillholeFields");

                    fullPathname = XmlDefaultPath.GetProjectPathAndFilename("DrillholeData", DrillholeTableType.continuous.ToString(), projectSession, projectLocation);
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

            }
            else
            {

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

                //Populate DrillholeDialog tables from XML
                frameMain.Navigate(new DrillholeImportPage(_classes, true, projectFile, projectSession, projectLocation));

            }
            else
            {
                frameMain.Navigate(new DrillholeDialogPage(true, projectFile, projectSession, projectLocation));

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

                if (check > 0)
                    return elements.Select(a => a.Element(DrillholeConstants.drillholeTable)).SingleOrDefault().Value;
            
            return "";
        }

        private async Task<List<DrillholeTable>> RetrieveDrillholeTableParameters(string projectFile, string drillholeTableFile)
        {
            List<DrillholeTable> tables = new List<DrillholeTable>();

            if (_xml == null)
                _xml = new Drillholes.XML.XmlController();

            if (_xmlService == null)
                _xmlService = new XmlService(_xml);

            tables = await _xmlService.DrillholeProjectProperties(projectFile, drillholeTableFile, DrillholeConstants.drillholeProject, DrillholeConstants.drillholeTable, DrillholeTableType.other);
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

      
        private void ribbonHome_GotFocus(object sender, RoutedEventArgs e)
        {
           
        }

        private void ribbonSettings_GotFocus(object sender, RoutedEventArgs e)
        {
            
        }

        private void Ribbon_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var ribbon = sender as Ribbon;

            if (ribbon.SelectedIndex == 0)
            {
                stkLabel.Visibility = Visibility.Visible;
                frameMain.Visibility = Visibility.Visible;

                stkCheckbox.Visibility = Visibility.Hidden;
                stkCollarToe.Visibility = Visibility.Hidden;
                stkGeology.Visibility = Visibility.Hidden;
                stkTolerance.Visibility = Visibility.Hidden;
            }
            else
            {
                stkLabel.Visibility = Visibility.Hidden;
                frameMain.Visibility = Visibility.Hidden;

                stkCheckbox.Visibility = Visibility.Visible;
                stkCollarToe.Visibility = Visibility.Visible;
                stkGeology.Visibility = Visibility.Visible;
                stkTolerance.Visibility = Visibility.Visible;
            }
        }




        private async void RibbonWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await ManageXmlPreferences();
        }






        #region Preferences
        private async Task<DrillholePreferences> SetPreferencesToXml()
        {
            DrillholePreferences preferences = new DrillholePreferences()
            {
                NegativeDip = (bool)this.chkDip.IsChecked,
                ImportAllColumns = (bool)this.chkImport.IsChecked,
                IgnoreInvalidValues = (bool)this.chkIgnore.IsChecked,
                LowerDetection = (bool)this.chkDetection.IsChecked,
                ImportSurveyOnly = (bool) this.chkImportDeviation.IsChecked,
                ImportAssayOnly = (bool) this.chkImportAssays.IsChecked,
                ImportGeologyOnly = (bool) this.chkImportGeology.IsChecked,
                ImportContinuousOnly = (bool) this.chkImportContinuous.IsChecked,
                NullifyZeroAssays = (bool) this.chkZeroAssays.IsChecked,
                CreateCollar = (bool) this.chkCollar.IsChecked,
                CreateToe = (bool) this.chkToe.IsChecked
            };

            if ((bool) radDownhole.IsChecked)
            {
                preferences.surveyType = DrillholeSurveyType.downholesurvey;
            }
            else if ((bool) radVertical.IsChecked)
            {
                preferences.surveyType = DrillholeSurveyType.vertical;
            }
            else if ((bool) radCollarSurvey.IsChecked)
            {
                preferences.surveyType = DrillholeSurveyType.collarsurvey;
            }

            if ((bool) radBottom.IsChecked)
            {
                preferences.GeologyBase = true;
            }
            else if ((bool) radTop.IsChecked)
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

            return preferences;
        }

        private async void SetEnvironmentFromXml(string rootName)
        {
            fullName = XmlDefaultPath.GetProjectPathAndFilename(rootName, "alltables", projectSession, projectLocation);
        
            //create XML temp table
            if (_xml == null)
                _xml = new Drillholes.XML.XmlController();

            if (_xmlService == null)
                _xmlService = new XmlService(_xml);

            DrillholePreferences preferences = null;

            XDocument xmlPreferences = await _xmlService.DrillholePreferences(fullName, preferences, DrillholeConstants.drillholePref);

            var elements = xmlPreferences.Descendants(rootName).Elements();

            var updateValues = elements.Where(p => p.Attribute("Value").Value == "exists");

            foreach(var value in updateValues)
            {
                var dip = value.Element("NegativeDip").Value;
                
                chkDip.IsChecked = Convert.ToBoolean(dip);
                chkImport.IsChecked = Convert.ToBoolean(value.Element("ImportAllColumns").Value);
                chkIgnore.IsChecked = Convert.ToBoolean(value.Element("IgnoreInvalidValues").Value);
                chkDetection.IsChecked = Convert.ToBoolean(value.Element("LowerDetection").Value);
                chkImportDeviation.IsChecked = Convert.ToBoolean(value.Element("ImportSurveyOnly").Value);
                chkImportAssays.IsChecked = Convert.ToBoolean(value.Element("ImportAssayOnly").Value);
                chkImportGeology.IsChecked = Convert.ToBoolean(value.Element("ImportGeologyOnly").Value);
                chkImportContinuous.IsChecked = Convert.ToBoolean(value.Element("ImportContinuousOnly").Value);
                chkZeroAssays.IsChecked = Convert.ToBoolean(value.Element("NullifyZeroAssays").Value);
                chkCollar.IsChecked = Convert.ToBoolean(value.Element("CreateCollar").Value);
                chkToe.IsChecked = Convert.ToBoolean(value.Element("CreateToe").Value);

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

                txtAziTol.Text = aziTol.ToString();
                txtDipTol.Text = dipTol.ToString();
                txtDefault.Text = defaultValue.ToString();

            }

        }
        private void chkDip_Click(object sender, RoutedEventArgs e)
        {
            xmlName = "NegativeDip";

            UpdateXmlPreferences((bool)chkDip.IsChecked);

        }

        private void chkImport_Click(object sender, RoutedEventArgs e)
        {
            xmlName = "ImportAllColumns";

            UpdateXmlPreferences((bool)chkImport.IsChecked);

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
            }
            else if ((bool)radCollarSurvey.IsChecked)
            {
                survey[1] = true;
            }
            else if ((bool)radVertical.IsChecked)
                survey[2] = true;

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

        private void chkToe_Click(object sender, RoutedEventArgs e)
        {
            xmlName = "CreateToe";

            UpdateXmlPreferences((bool)chkToe.IsChecked);

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



    }


}

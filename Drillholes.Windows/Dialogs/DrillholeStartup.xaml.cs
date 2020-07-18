﻿using System;
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

namespace Drillholes.Windows.Dialogs
{
    /// <summary>
    /// Interaction logic for DrillholeStartup.xaml
    /// </summary>
    public partial class DrillholeStartup : RibbonWindow
    {
        private XmlService _xmlService;
        private IDrillholeXML _xml;
        private string rootName = "DrillholePreferences";
        private string fullName { get; set; }
        private string xmlName { get; set; }
        private bool[] survey { get; set; }
        private bool[] geology { get; set; }
        private bool savedProject { get; set; }
        private string projectSession { get; set; }
        private string projectLocation { get; set; }
        public DrillholeStartup()
        {
            InitializeComponent();

            survey = new bool[3];
            geology = new bool[2];
            savedProject = false;
            projectSession = "";
        }

        private async Task<bool> ManageXmlPreferences()
        {
            DrillholePreferences preferences = await SetPreferencesToXml();

            if (!savedProject)
            {
                //get pathname
                if (fullName == "" || fullName == null)
                    fullName = XmlDefaultPath.GetFullPathAndFilename(rootName, "alltables");
            }
            else
            {

                    fullName = XmlDefaultPath.GetProjectPathAndFilename(rootName, "alltables", projectSession, projectLocation );
            }

            //create XML temp table
            if (_xml == null)
                _xml = new Drillholes.XML.XmlController();

            if (_xmlService == null)
                _xmlService = new XmlService(_xml);

            await _xmlService.DrillholePreferences(fullName, preferences, rootName);

            return true;
        }

        private async Task<bool> ManageXmlProperties(DrillholeProjectProperties properties)
        {
            await _xmlService.DrillholeProjectProperties(properties, "DrillholeProject");

            return true;
        }

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
           // string path = XmlDefaultPath.GetFullPathAndFilename();

            
                frameMain.Source = new Uri("DrillholeDialogPage.xaml", UriKind.Relative);
                txtStart.Visibility = Visibility.Collapsed;
            

            
        }

        private void Open_Session(object sender, RoutedEventArgs e)
        {
            OpenDrillholeSession();
            frameMain.Source = new Uri("DrillholeImportPage.xaml", UriKind.Relative);
            txtStart.Visibility = Visibility.Collapsed;

            var source = frameMain.Source;

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

        private void Save_Session(object sender, RoutedEventArgs e)
        {
           // SaveDrillholeSession();
        }

        private void Save_SessionAs(object sender, RoutedEventArgs e)
        {
            SaveDrillholeSession();
        }

        private async void RibbonWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await ManageXmlPreferences();
        }

        private async void OpenDrillholeSession()
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

                SetEnvironmentFromXml(rootName);
            }
            else
                return;

        }

        private async void SaveDrillholeSession()
        {
            string strHeader = "Save Drillhole Session";

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Drillhole Session (*.dh)|*.dh|All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.CreatePrompt = false;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.AddExtension = false;

            if(saveFileDialog.ShowDialog() == true)
            {
                FileInfo info = new FileInfo(saveFileDialog.FileName);
                projectLocation = info.DirectoryName;
                string sessionName = info.Name;
                string extension = info.Extension;

                //check if file exists
                if (info.Exists)
                {
                    MessageBoxResult fileExists = MessageBox.Show("File '" + sessionName + "' already exists. Overwrite existing file?", "Save File", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                    if (MessageBoxResult.Cancel == fileExists)
                        return;
                    else if (MessageBoxResult.No == fileExists)
                        return;
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

                    return;
                }
                else //make project directory to store xml files
                {
                    dirInfo.CreateSubdirectory(projectSession);
                }

                savedProject = true;

                await ManageXmlPreferences();

                DrillholeProjectProperties properties = new DrillholeProjectProperties()
                {
                    ProjectName = projectSession,
                    ProjectParentFolder = projectLocation,
                    ProjectFolder = projectLocation + "\\" + projectSession,
                    ProjectFile = projectLocation + "\\" + sessionName
                };

                await ManageXmlProperties(properties);

            };

            //Create directory
        }

        private void SaveProjectFile()
        {

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

            XDocument xmlPreferences = await _xmlService.DrillholePreferences(fullName, null, rootName);

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
            await _xmlService.DrillholePreferences(fullName, xmlName, bValue, rootName);
        }

        private async void UpdateXmlSurveyPreferences(bool[] bValue)
        {
            
            if (bValue[0])
            {
                await _xmlService.DrillholePreferences(fullName, xmlName, DrillholeSurveyType.downholesurvey, rootName);

            }
            else if (bValue[1])
            {
                await _xmlService.DrillholePreferences(fullName, xmlName, DrillholeSurveyType.collarsurvey, rootName);

            }
            else
            {
                await _xmlService.DrillholePreferences(fullName, xmlName, DrillholeSurveyType.vertical, rootName);

            }

        }

        private async void UpdateXmlGeologyContactPreferences(bool[] bValue)
        {

            if (bValue[0])
            {
                await _xmlService.DrillholePreferences(fullName, xmlName, true, rootName);

            }
            else if (bValue[1])
            {
                await _xmlService.DrillholePreferences(fullName, xmlName, false, rootName);

            }

        }
        private async void UpdateXmlPreferences(string xmlValue)
        {
            await _xmlService.DrillholePreferences(fullName, xmlName, xmlValue, rootName);
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

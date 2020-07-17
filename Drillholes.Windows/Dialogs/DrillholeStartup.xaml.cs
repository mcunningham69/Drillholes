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

        public DrillholeStartup()
        {
            InitializeComponent();

            survey = new bool[3];
            geology = new bool[2];
        }

        private async Task<bool> ManageXml()
        {
            DrillholePreferences preferences = await SetPreferences();

            //get pathname
            if (fullName == "" || fullName == null)
                fullName = XmlDefaultPath.GetFullPathAndFilename(rootName, "alltables");

            //create XML temp table
            if (_xml == null)
                _xml = new Drillholes.XML.XmlController();

            if (_xmlService == null)
                _xmlService = new XmlService(_xml);

            await _xmlService.DrillholePreferences(fullName, preferences, rootName);

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
            frameMain.Source = new Uri("DrillholeImportPage.xaml", UriKind.Relative);
            txtStart.Visibility = Visibility.Collapsed;

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

        }

        private async void RibbonWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await ManageXml();
        }

        private async Task<DrillholePreferences> SetPreferences()
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

            UpdateXmlPreferences(survey);

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

            UpdateXmlPreferences(geology);

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

        private void UpdateXmlPreferences(bool bValue)
        {
            TODO
        }

        private void UpdateXmlPreferences(bool[] bValue)
        {

        }

        private void UpdateXmlPreferences(string xmlValue)
        {

        }

        private void txtDipTol_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Information.IsNothing(txtDipTol.Text))
                MessageBox.Show("Please enter a valid number between 0 and 90", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            else if (Information.IsNumeric(txtDipTol.Text))
                MessageBox.Show("Please enter a valid number between 0 and 90", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            double range = Convert.ToDouble(txtDipTol.Text);
            if (range < 0)
                MessageBox.Show("Cannot be less than zero. Please enter a valid number between 0 and 90", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            else if (range > 90)
                MessageBox.Show("Cannot be greater than 90. Please enter a valid number between 0 and 90", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

        }

        private void txtAziTol_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Information.IsNothing(txtAziTol.Text))
                MessageBox.Show("Please enter a valid number between 0 and 360", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            else if (Information.IsNumeric(txtAziTol.Text))
                MessageBox.Show("Please enter a valid number between 0 and 360", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            double range = Convert.ToDouble(txtAziTol.Text);
            if (range < 0)
                MessageBox.Show("Cannot be less than zero. Please enter a valid number between 0 and 360", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            else if (range > 360)
                MessageBox.Show("Cannot be greater than 360. Please enter a valid number between 0 and 360", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

        }

        private void txtDefault_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Information.IsNothing(txtDefault.Text))
                MessageBox.Show("Please enter a valid number", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            else if (Information.IsNumeric(txtDefault.Text))
                MessageBox.Show("Please enter a valid number", "Error", MessageBoxButton.OK, MessageBoxImage.Error);


        }
    }


}

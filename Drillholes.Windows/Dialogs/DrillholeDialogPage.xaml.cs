using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Drillholes.Domain.Enum;
using Drillholes.Domain;
using AutoMapper;
using Drillholes.Domain.DataObject;
using Drillholes.Domain.DTO;
using Drillholes.Domain.Services;
using Drillholes.Domain.Interfaces;
using Drillholes.Domain.Exceptions;
using System.IO;

namespace Drillholes.Windows.Dialogs
{
    /// <summary>
    /// Interaction logic for DrillholeDialogPage.xaml
    /// </summary>
    public partial class DrillholeDialogPage : Page
    {
        private bool bArc { get; set; }

        bool bExcel = false;
        public List<DrillholeTable> importTables { get; set; }
        private DrillholeImportServices _importService;
        private IDrillholeTables _drillholeTables;
        private XmlService _xmlService;
        private IDrillholeXML _xml;
        // private string rootName = "DrillholeTables";
        public string xmlProjectFile { get; set; }

        public bool savedSession { get; set; }
        public string projectSession { get; set; }
        public string projectLocation { get; set; }
        private string fullName { get; set; }

        public string collarVal { get; set; }
        public string surveyVal { get; set; }
        public string assayVal { get; set; }
        public string intervalVal { get; set; }
        public string contVal { get; set; }

        IMapper mapper = null;
        bool bDebug = true;

        public DrillholeDialogPage()
        {
            InitializeComponent();
            bArc = false;
            Startup();
            savedSession = false;
            xmlProjectFile = "";

        }

        public DrillholeDialogPage(bool _savedSession, string _xmlProjectFile, string _projectSession, string _projectLocation)
        {
            InitializeComponent();
            bArc = false;
            Startup();
            savedSession = _savedSession;
            xmlProjectFile = _xmlProjectFile;
            projectLocation = _projectLocation;
            projectSession = _projectSession;

            collarVal = "";
            surveyVal = "";
            assayVal = "";
            intervalVal = "";
            contVal = "";
        }

        public DrillholeDialogPage(bool _savedSession, string _xmlProjectFile, string _projectSession, string _projectLocation, string _collar,
            string _survey, string _assay, string _interval, string _continuous)
        {
            InitializeComponent();
            bArc = false;
            Startup();
            savedSession = _savedSession;
            xmlProjectFile = _xmlProjectFile;
            projectLocation = _projectLocation;
            projectSession = _projectSession;

            //TODO set up event handler
            collarVal = _collar;
            surveyVal = _survey;
            assayVal = _assay;
            intervalVal = _interval;
            contVal = _continuous;

            txtCollar.Text = collarVal;
            txtSurvey.Text = surveyVal;
            txtAssay.Text = assayVal;
            txtLitho.Text = intervalVal;
            txtDistance.Text = contVal;
        }

        private void Startup()
        {

            if (bArc)
            {
                radArc.Visibility = Visibility.Visible;
                radArc.IsChecked = true;
            }

            importTables = new List<DrillholeTable>();

            //debug
            btnImport.IsEnabled = true;

            if (bDebug)
                txtCollar.Text = "Collar";

            fullName = "";

            _xml = new Drillholes.XML.XmlController();

            _xmlService = new XmlService(_xml);

            InitaliseDrillholeImportMap();
        }

        public void InitaliseDrillholeImportMap()
        {
            if (bArc)
            {
                //_drillholeTables = new Drillholes.ArcDialog.DrillholeImportEngine();
            }
            else if (bExcel)
            {
                // _drillholeTables = new Drillholes.ExcelDialog.DrillholeImportEngine();

            }
            else
            {
                //TODO - change to service so not need direct reference
                _drillholeTables = new Drillholes.FileDialog.DrillholeImportEngine();
            }

            var config = new MapperConfiguration(cfg =>
            { cfg.CreateMap<DrillholeImportDto, DrillholeTable>(); });

            mapper = config.CreateMapper();
            var source = new DrillholeImportDto();
            var dest = mapper.Map<DrillholeImportDto, DrillholeTable>(source);

            _importService = new DrillholeImportServices(_drillholeTables);
        }




        private async void btnCollar_Click(object sender, RoutedEventArgs e)
        {
            SelectTables(DrillholeTableType.collar, false);

            string collar = importTables.Where(o => o.tableType ==
            DrillholeTableType.collar).Select(s => s.tableName).FirstOrDefault();

            if (collar != "")
            {
                await ManageXml();

                txtCollar.Text = collar;
                collarVal = collar;
            }
        }

        private async void btnSurvey_Click(object sender, RoutedEventArgs e)
        {
            SelectTables(DrillholeTableType.survey, false);

            string survey = importTables.Where(o => o.tableType ==
            DrillholeTableType.survey).Select(s => s.tableName).FirstOrDefault();

            if (survey != "")
            {
                await ManageXml();

                txtSurvey.Text = survey;
                surveyVal = "";
            }
        }

        private async void btnAssay_Click(object sender, RoutedEventArgs e)
        {
            SelectTables(DrillholeTableType.assay, false);

            string assay = importTables.Where(o => o.tableType ==
            DrillholeTableType.assay).Select(s => s.tableName).FirstOrDefault();

            if (assay != "")
            {
                await ManageXml();

                txtAssay.Text = assay;
                assayVal = assay;
            }
        }

        private async void btnInterval_Click(object sender, RoutedEventArgs e)
        {
            SelectTables(DrillholeTableType.interval, false);

            string interval = importTables.Where(o => o.tableType ==
            DrillholeTableType.interval).Select(s => s.tableName).FirstOrDefault();

            if (interval != "")
            {
                await ManageXml();

                txtLitho.Text = interval;
                intervalVal = interval;
            }
        }

        private async void btnOther_Click(object sender, RoutedEventArgs e)
        {
            SelectTables(DrillholeTableType.continuous, false);

            string continuous = importTables.Where(o => o.tableType ==
            DrillholeTableType.continuous).Select(s => s.tableName).FirstOrDefault();

            if (continuous != "")
            {
                await ManageXml();

                txtDistance.Text = continuous;
                contVal = continuous;
            }
        }

        private void CloseApplication()
        {
            importTables.Clear();

            this.Visibility = Visibility.Collapsed;
        }
        private void radText_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)radText.IsChecked)
            {
                bArc = false;
                bExcel = false;
            }

            InitaliseDrillholeImportMap();
        }

        private void radExcel_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)radExcel.IsChecked)

                bExcel = true;
            else
                bExcel = false;

            InitaliseDrillholeImportMap();
        }

        private void radArc_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)radArc.IsChecked)

                bArc = true;
            else
                bArc = false;

            InitaliseDrillholeImportMap();
        }

        private void chkSystem_Click(object sender, RoutedEventArgs e)
        {
            if (chkSystem.IsChecked == true)
            {
                bArc = true;
            }
            else
                bArc = false;

            InitaliseDrillholeImportMap();
        }

        private async void btnImport_Click(object sender, RoutedEventArgs e)
        {
            if (bDebug)
                DebugMode();

            if (this.txtCollar.Text == "")
                importTables.Clear();

            if (importTables.Count == 0)
                return;

            try
            {
                await ManageXml();

                NavigationService.Navigate(new DrillholeImportPage(importTables,savedSession,xmlProjectFile, projectSession, projectLocation));

            }
            catch (ImportFormatException ex)
            {
                MessageBox.Show(ex.Message, "Format Issue", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Visibility = Visibility.Hidden;
            }
        }

        public bool SelectTables(DrillholeTableType value, bool bMulti)
        {

            DrillholeTable importService;

            if (bMulti == false)
            {
                importService = _importService.SelectTables(value, mapper);
            }
            else
            {
                importService = _importService.SelectIntervalTables(value, mapper);
            }

            if (importService.isCancelled)
            {
                return false;
            }


            return ManageImportClassesFromDialog(importService, value);
        }

        private async Task<bool>  ManageXml()
        {
            if (savedSession && xmlProjectFile != "")
            {
                //Read XML

                //Add new entry and save
                fullName = XmlDefaultPath.GetProjectPathAndFilename(DrillholeConstants.drillholeTable, "alltables", projectSession, projectLocation);

            }
            else
            {
                //get pathname
                if (fullName == "")
                    fullName = XmlDefaultPath.GetFullPathAndFilename(DrillholeConstants.drillholeTable, "alltables");
            }

            //create XML temp table
            if (_xml == null)
                _xml = new Drillholes.XML.XmlController();

            if (_xmlService == null)
                _xmlService = new XmlService(_xml);

            //Creates the alltables XML table
            await _xmlService.TableParameters(fullName, importTables, DrillholeConstants.drillholeTable);

            //updates the path to alltables in the .dh file
            // await _xmlService.DrillholeProjectProperties(projectLocation + "\\" + projectSession + ".dh", fullName, DrillholeConstants.drillholeProject, DrillholeConstants.drillholeTable, DrillholeTableType.other);
            _xmlService.TableParameters(projectLocation + "\\" + projectSession + ".dh", fullName, DrillholeConstants.drillholeProject, DrillholeTableType.other);

            return true;
        }

        private bool ManageImportClassesFromDialog(DrillholeTable _importClass, DrillholeTableType value)
        {

            if (importTables == null)
                return false;

            if (bExcel)
            {
                _importClass.tableLocation = _importClass.tableLocation + "\\" + _importClass.tableName;
                DrillholeExcelSheet sheet = new DrillholeExcelSheet(_importClass.tableType.ToString(), _importClass.tableLocation);

                sheet.ShowDialog();

                if (sheet.selectedSheet != "")
                {
                    _importClass.tableName = sheet.selectedSheet;
                }

                sheet.Close();
            }
            else if (bArc)
            {
                int nameLength = _importClass.tableName.Length;
                string toTrim = _importClass.tableName.Substring(nameLength - 1, 1); //check for $ sign

                if (toTrim == "$")
                    _importClass.tableName = _importClass.tableName.Substring(0, _importClass.tableName.Length - 1);
            }

            if (importTables.Count == 0)
            {
                importTables.Add(_importClass);
                return true;
            }

            var checkType = importTables.Where(i => i.tableType == value).Count();

            if (checkType == 0)
            {
                importTables.Add(_importClass);

                return true;
            }
            else //update values
            {
                var updateType = importTables.Where(i => i.tableType == value);

                int nCounter = 0;
                foreach (DrillholeTable _value in updateType)
                {

                    _value.tableLocation = _importClass.tableLocation;
                    _value.tableName = _importClass.tableName;
                    _value.tableFormat = _importClass.tableFormat;
                    _value.tableFormatName = _importClass.tableFormatName;

                    nCounter++;
                }
            }

            return true;

        }
        private List<DrillholeTable> DebugMode()
        {
            importTables = new List<DrillholeTable>();

            #region File GDB
            /*
            // string strFormat = "fgdb_table";
            string strFormatName = DrillholeImportFormat.fgdb_table.ToString();

            //setup collar


            //string strPath = @"C:\vscode\drillholes\drillholes.gdb\";

            string strPath = @"C:\Projects\SRK313 - Saudi(Bert)\SRK313\SRK313.gdb";

            string collarName = "T6_DD_collar";
            string surveyName = "Survey";
            string assayName = "Assays";
            string lithoName = "Geology";
            // string otherName = "other";

            //for debug
            this.txtCollar.Text = "Collar";

            string collarLoc = strPath + collarName;

            string surveyLoc = strPath + surveyName;

            string assayLoc = strPath + assayName;

            string lithoLoc = strPath + lithoName;

            importTables.Add(new DrillholeImportObject { tableFormat = DrillholeImportFormat.fgdb_table, tableFormatName = strFormatName, tableLocation = collarLoc, tableName = collarName, tableType = DrillholeTableType.collar, isCancelled = false, isValid = true });
            importTables.Add(new DrillholeImportObject { tableFormat = DrillholeImportFormat.fgdb_table, tableFormatName = strFormatName, tableLocation = surveyLoc, tableName = surveyName, tableType = DrillholeTableType.survey, isCancelled = false, isValid = true });
            importTables.Add(new DrillholeImportObject { tableFormat = DrillholeImportFormat.fgdb_table, tableFormatName = strFormatName, tableLocation = assayLoc, tableName = assayName, tableType = DrillholeTableType.assay, isCancelled = false, isValid = true });
            importTables.Add(new DrillholeImportObject { tableFormat = DrillholeImportFormat.fgdb_table, tableFormatName = strFormatName, tableLocation = lithoLoc, tableName = lithoName, tableType = DrillholeTableType.interval, isCancelled = false, isValid = true });
            */
            #endregion

            #region personal GDB
            /*
            string strFormat = "fgdb_table";
            string strFormatName = DrillholeConstants.pgdb.ToString(); 

            //setup collar
            List<DrillholeImportClass> _tables = new List<DrillholeImportClass>();

            string collarName = "collar_pani";
            string surveyName = "survey_pani";
            string assayName = "assay_pani";
            string lithoName = "litho_pani";
            string otherName = "other_pani";

            string collarLoc = @"C:\Projects\Software development\SRK.Drillholes\Project Data\testdata.mdb\collar_pani";

            string surveyLoc = @"C:\Projects\Software development\SRK.Drillholes\Project Data\testdata.mdb\survey_pani";

            string assayLoc = @"C:\Projects\Software development\SRK.Drillholes\Project Data\testdata.mdb\assay_pani";

            string lithoLoc = @"C:\Projects\Software development\SRK.Drillholes\Project Data\testdata.mdb\litho_pani";

            string otherLoc = @"C:\Projects\Software development\SRK.Drillholes\Project Data\testdata.mdb\density_pani";


            _tables.Add(new DrillholeImportClass { tableFormat = strFormat, tableFormatName = strFormatName, tableLocation = collarLoc, tableName = collarName, tableType = DrillholeTableMode.collar.ToString(), isCancelled = false, isValid = true });        
            _tables.Add(new DrillholeImportClass { tableFormat = strFormat, tableFormatName = strFormatName, tableLocation = surveyLoc, tableName = surveyName, tableType = DrillholeTableMode.survey.ToString(), isCancelled = false, isValid = true });
            _tables.Add(new DrillholeImportClass { tableFormat = strFormat, tableFormatName = strFormatName, tableLocation = assayLoc, tableName = assayName, tableType = DrillholeTableMode.assay.ToString(), isCancelled = false, isValid = true });
            _tables.Add(new DrillholeImportClass { tableFormat = strFormat, tableFormatName = strFormatName, tableLocation = lithoLoc, tableName = lithoName, tableType = DrillholeTableMode.litho.ToString(), isCancelled = false, isValid = true });
            _tables.Add(new DrillholeImportClass { tableFormat = strFormat, tableFormatName = strFormatName, tableLocation = otherLoc, tableName = otherName, tableType = DrillholeTableMode.other.ToString(), isCancelled = false, isValid = true });
           */
            #endregion

            #region CSV files

            string strFormatName = DrillholeConstants.text.ToString(); ;

            //setup collar


            //string collarName = "collar_pani.csv";
            //string surveyName = "survey_pani.csv";
            //string assayName = "assay_pani.csv";
            //string lithoName = "litho_pani.csv";

            string collarName = "collar_ten.csv";
            string surveyName = "survey_ten.csv";
            string assayName = "assay_ten.csv";
            string lithoName = "litho_ten.csv";
            string distName = "distance_ten.csv";

            //string path = @"C:\Projects\Source code\projectdata\panitest";

            string path = @"C:\Users\mcunningham\source\Workspaces\projectdata\Pani_test";
            //string path = @"C:\Users\Mike\source\Workspaces\projectdata\Pani_test";
            string collarLoc = path;

            string surveyLoc = path;

            string assayLoc = path;

            string lithoLoc = path;

            string otherLoc = path;


            importTables.Add(new DrillholeTable { tableFormat = DrillholeImportFormat.text_csv, tableFormatName = strFormatName, tableLocation = collarLoc, tableName = collarName, tableType = DrillholeTableType.collar, isCancelled = false, isValid = true });
            importTables.Add(new DrillholeTable { tableFormat = DrillholeImportFormat.text_csv, tableFormatName = strFormatName, tableLocation = surveyLoc, tableName = surveyName, tableType = DrillholeTableType.survey, isCancelled = false, isValid = true });
            importTables.Add(new DrillholeTable { tableFormat = DrillholeImportFormat.text_csv, tableFormatName = strFormatName, tableLocation = assayLoc, tableName = assayName, tableType = DrillholeTableType.assay, isCancelled = false, isValid = true });
            importTables.Add(new DrillholeTable { tableFormat = DrillholeImportFormat.text_csv, tableFormatName = strFormatName, tableLocation = lithoLoc, tableName = lithoName, tableType = DrillholeTableType.interval, isCancelled = false, isValid = true });
            importTables.Add(new DrillholeTable { tableFormat = DrillholeImportFormat.text_csv, tableFormatName = strFormatName, tableLocation = otherLoc, tableName = distName, tableType = DrillholeTableType.continuous, isCancelled = false, isValid = true });

            #endregion

            #region TXT files
            /*
            string strFormatName = DrillholeConstants.text.ToString(); ;

            //setup collar
            List<DrillholeImportObject> _tables = new List<DrillholeImportObject>();

            string collarName = "collar_pani.txt";
            string surveyName = "survey_pani.txt";
            string assayName = "assay_pani.txt";
            string lithoName = "litho_pani.txt";
            string otherName = "other_pani.txt";

            string collarLoc = @"C:\Users\mcunningham\source\Workspaces\projectdata";

            string surveyLoc = @"C:\Users\mcunningham\source\Workspaces\projectdata";

            string assayLoc = @"C:\Users\mcunningham\source\Workspaces\projectdata";

            string lithoLoc = @"C:\Users\mcunningham\source\Workspaces\projectdata";

            string otherLoc = @"C:\Users\mcunningham\source\Workspaces\projectdata";


            importTables.Add(new DrillholeImportObject { tableFormat = DrillholeImportFormat.text_txt, tableFormatName = strFormatName, tableLocation = collarLoc, tableName = collarName, tableType = DrillholeTableType.collar, isCancelled = false, isValid = true });
            //importTables.Add(new DrillholeImportObject { tableFormat = DrillholeImportFormat.text_txt, tableFormatName = strFormatName, tableLocation = surveyLoc, tableName = surveyName, tableType = DrillholeTableType.survey, isCancelled = false, isValid = true });
            //importTables.Add(new DrillholeImportObject { tableFormat = DrillholeImportFormat.text_txt, tableFormatName = strFormatName, tableLocation = assayLoc, tableName = assayName, tableType = DrillholeTableType.assay, isCancelled = false, isValid = true });
            //importTables.Add(new DrillholeImportObject { tableFormat = DrillholeImportFormat.text_txt, tableFormatName = strFormatName, tableLocation = lithoLoc, tableName = lithoName, tableType = DrillholeTableType.interval, isCancelled = false, isValid = true });
            //importTables.Add(new DrillholeImportObject { tableFormat = DrillholeImportFormat.text_txt, tableFormatName = strFormatName, tableLocation = otherLoc, tableName = otherName, tableType = DrillholeTableType.other, isCancelled = false, isValid = true });
            */
            #endregion

            #region Excel
            /*
            string strFormatName = DrillholeConstants.excel.ToString(); 



            string collarName = "collar$";
            string surveyName = "survey$";
            string assayName = "assay$";
            string lithoName = "litho$";
            string otherName = "density$";

            string collarLoc = @"C:\Users\mcunningham\source\Workspaces\projectdata\pani_holes.xlsx";

            string surveyLoc = @"C:\Users\mcunningham\source\Workspaces\projectdata\pani_holes.xlsx";

            string assayLoc = @"C:\Users\mcunningham\source\Workspaces\projectdata\pani_holes.xlsx";

            string lithoLoc = @"C:\Users\mcunningham\source\Workspaces\projectdata\pani_holes.xlsx";

            string otherLoc = @"C:\Users\mcunningham\source\Workspaces\projectdata\pani_holes.xlsx";


            importTables.Add(new DrillholeImportObject { tableFormat = DrillholeImportFormat.excel_table, tableFormatName = strFormatName, tableLocation = collarLoc, tableName = collarName, tableType = DrillholeTableType.collar, isCancelled = false, isValid = true });
            //importTables.Add(new DrillholeImportObject { tableFormat = DrillholeImportFormat.excel_table, tableFormatName = strFormatName, tableLocation = surveyLoc, tableName = surveyName, tableType = DrillholeTableType.survey, isCancelled = false, isValid = true });
            //importTables.Add(new DrillholeImportObject { tableFormat = DrillholeImportFormat.excel_table, tableFormatName = strFormatName, tableLocation = assayLoc, tableName = assayName, tableType = DrillholeTableType.assay, isCancelled = false, isValid = true });
            //importTables.Add(new DrillholeImportObject { tableFormat = DrillholeImportFormat.excel_table, tableFormatName = strFormatName, tableLocation = lithoLoc, tableName = lithoName, tableType = DrillholeTableType.interval, isCancelled = false, isValid = true });
            //importTables.Add(new DrillholeImportObject { tableFormat = DrillholeImportFormat.excel_table, tableFormatName = strFormatName, tableLocation = otherLoc, tableName = otherName, tableType = DrillholeTableType.other, isCancelled = false, isValid = true });
            */
            #endregion


            #region ArcSDE

            //string strFormatName = DrillholeConstants.egdb.ToString();

            //string collarName = "FIJV_2020.dbo.Collar";
            //string surveyName = "dbo.Drillhole_DhSurvey";
            //string assayName = "dbo.assays";
            //string lithoName = "dbo.lithology";
            // string otherName = "other";

            //string collarLoc = @"C:\..\fijv.sde";

            //string surveyLoc = @"C:\Users\mcunningham\OneDrive - SRK Consulting\Projects\FIJV\FIJV2020\FIJ004.sde\FIJ004.dbo.Drillhole_DhSurvey";

            //string assayLoc = @"C:\Users\mcunningham\OneDrive - SRK Consulting\Projects\FIJV\FIJV2020\FIJ004.sde\FIJ004.dbo.assays";

            //string lithoLoc = @"C:\Users\mcunningham\OneDrive - SRK Consulting\Projects\FIJV\FIJV2020\FIJ004.sde\FIJ004.dbo.lithology";

            //  string otherLoc = @"C:\Projects\Software development\SRK.Drillholes\Project Data\testdata.gdb\density_pani";


            // _tables.Add(new DrillholeImportObject { tableFormat = DrillholeImportFormat.egdb_table, tableFormatName = strFormatName, tableLocation = collarLoc, tableName = collarName, tableType = DrillholeTableType.collar, isCancelled = false, isValid = true });
            //_tables.Add(new DrillholeImportObject { tableFormat = DrillholeImportFormat.egdb_table, tableFormatName = strFormatName, tableLocation = surveyLoc, tableName = surveyName, tableType = DrillholeTableType.survey, isCancelled = false, isValid = true });
            //_tables.Add(new DrillholeImportObject { tableFormat = DrillholeImportFormat.egdb_table, tableFormatName = strFormatName, tableLocation = assayLoc, tableName = assayName, tableType = DrillholeTableType.assay, isCancelled = false, isValid = true });
            //_tables.Add(new DrillholeImportObject { tableFormat = DrillholeImportFormat.egdb_table, tableFormatName = strFormatName, tableLocation = lithoLoc, tableName = lithoName, tableType = DrillholeTableType.litho, isCancelled = false, isValid = true });

            #endregion


            return importTables;

        }


    }
}

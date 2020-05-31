using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Excel = Microsoft.Office.Interop.Excel;

namespace Drillholes.Windows.Dialogs
{
    /// <summary>
    /// Interaction logic for DrillholeExcelSheet.xaml
    /// </summary>
    public partial class DrillholeExcelSheet : Window
    {
        public string selectedSheet { get; set; }
        public ObservableCollection<string> availableSheets { get; set; }

        public DrillholeExcelSheet()
        {
            InitializeComponent();

            string _tableType = "Collar".ToUpper();
            selectedSheet = "";
            this.Title = "Import '" + _tableType + "' table";

            string filePath = @"C:\Users\mcunningham\source\Workspaces\projectdata\pani_holes.xlsx";





        }

        public DrillholeExcelSheet(string _tableType, string _filePath)
        {
            InitializeComponent();

            selectedSheet = "";
            this.Title = "Import " + _tableType.ToUpper() + " table";

            LoadListbox(_filePath);
        }

        private void LoadListbox(string filePath)
        {
            Excel.Application excelApp = new Excel.Application();

            Excel.Workbook excelWorkbook = excelApp.Workbooks.Open(filePath, 0, true, 5, "", "", true, Excel.XlPlatform.xlWindows, "", false, false, 0, true, false, false);
            Excel.Sheets excelSheets = excelWorkbook.Worksheets;

            availableSheets = new ObservableCollection<string>();

            foreach (Excel.Worksheet excelWorksheet in excelSheets)
            {
                availableSheets.Add(excelWorksheet.Name);
            }

            lstSheets.ItemsSource = availableSheets;

            excelWorkbook.Close(0);
            excelApp.Quit();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            selectedSheet = "";
            this.Close();
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            if (lstSheets.SelectedItem != null)
                selectedSheet = lstSheets.SelectedItem.ToString();

            this.Hide();
        }
    }
}

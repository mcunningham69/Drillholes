using System;
using System.IO;
using Drillholes.Domain.DTO;
using Drillholes.Domain.Enum;
using Drillholes.Domain.Interfaces;
using Microsoft.Win32;

namespace Drillholes.FileDialog
{
    public class DrillholeImportEngine : IDrillholeTables
    {
        private DrillholeImportDto drillholeTables;

        public DrillholeImportDto SelectIntervalTables(DrillholeTableType tableType)
        {
            throw new NotImplementedException();
        }

        public DrillholeImportDto SelectTables(DrillholeTableType tableType)
        {
            drillholeTables = new DrillholeImportDto();

            drillholeTables.tableType = tableType;
            drillholeTables.isValid = true;
            drillholeTables.isCancelled = false;

            string strHeader = "Select " + tableType.ToString() + " table";

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV files (*.csv)|*.csv|Text files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog.Title = strHeader;

            /*
             * 1 = CSV
             * 2 = TXT
             * 3 = Others
             */

            if (openFileDialog.ShowDialog() == true)
            {

                FileInfo info = new FileInfo(openFileDialog.FileName);
                drillholeTables.tableLocation = info.DirectoryName;
                drillholeTables.tableName = info.Name;
                string extension = info.Extension;

                switch (openFileDialog.FilterIndex)
                {
                    case 1: //CSV
                        drillholeTables.tableFormatName = "text";
                        drillholeTables.tableFormat = DrillholeImportFormat.text_csv;
                        break;
                    case 2:  //TEXT
                        drillholeTables.tableFormatName = "text";
                        drillholeTables.tableFormat = DrillholeImportFormat.text_txt;
                        break;
                    case 3: //All other
                        if (extension == "csv")
                        {
                            drillholeTables.tableFormatName = "text";
                            drillholeTables.tableFormat = DrillholeImportFormat.text_csv;

                        }
                        else if (extension == "txt")
                        {
                            drillholeTables.tableFormatName = "text";
                            drillholeTables.tableFormat = DrillholeImportFormat.text_txt;
                        }
                        else if (extension == "xlsx")
                        {
                            drillholeTables.tableFormatName = "excel";
                            drillholeTables.tableFormat = DrillholeImportFormat.excel_table;
                        }
                        else
                        {
                            drillholeTables.tableFormatName = "unknown";
                            drillholeTables.tableFormat = DrillholeImportFormat.other;

                        }
                            break;

                }
            }
            else
            {
                drillholeTables.isCancelled = true;

            }

  
            return drillholeTables;
        }

    
    }
}

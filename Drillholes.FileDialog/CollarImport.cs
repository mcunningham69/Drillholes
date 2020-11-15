using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using Drillholes.Domain;
using Drillholes.Domain.DTO;
using Drillholes.Domain.Enum;
using Drillholes.Domain.Exceptions;
using Drillholes.Domain.Interfaces;
using Drillholes.Domain.DataObject;

namespace Drillholes.FileDialog
{
    public class CollarImport : ICollarTable
    {
        private CollarTableDto collarTableDto;

        public async Task<CollarTableDto> ImportAllFieldsAsGeneric(bool bImport)
        {
            if (bImport)
            {
                var queryTo = collarTableDto.tableData.Where(v => v.columnImportAs == DrillholeConstants.notImported);

                foreach (var _val in queryTo)
                {
                    _val.columnImportAs = DrillholeConstants._genericName;
                    _val.columnImportName = _val.columnHeader;
                    _val.groupName = DrillholeConstants.GroupOtherFields;
                    _val.genericType = true;
                    _val.fieldType = "Text"; //TODO
                    _val.keys = new KeyValuePair<bool, bool>(false, false);
                }

            }

            return collarTableDto;
        }

        private async void PopulateXmlData(bool bCSV)
        {

            collarTableDto.xPreview = new XElement("Collars");

            char delimiter = '\t';

            if (bCSV)
                delimiter = ',';

            List<CsvRow> rows = new List<CsvRow>();

            XElement mFieldItems = null;

            int counter = 0;

            bool bExists = File.Exists(collarTableDto.tableLocation + "\\" + collarTableDto.tableName);

            if (!bExists)
                throw new ImportFormatException("File doesn't exist");

            try
            {
                using (var reader = new StreamReader(collarTableDto.tableLocation + "\\" + collarTableDto.tableName))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(delimiter);

                        if (counter > 0)
                        {
                            foreach (string row in values)
                            {
                                string strValue = row;
                                if (row == "")
                                    strValue = "-";
                                rows.Add(new CsvRow { results = strValue });
                            }

                            mFieldItems = new XElement("Collar", new XAttribute("ID", (counter - 1).ToString()), new XAttribute("Ignore", "false"));


                            for (int i = 0; i < collarTableDto.fields.Count; i++)
                            {
                                if (collarTableDto.fields[i] != "")
                                {
                                    string fieldName = collarTableDto.fields[i].Replace(" ", "_"); ;

                                    XElement mNode = new XElement(fieldName, rows[i].results);
                                    mFieldItems.Add(mNode);
                                }
                            }

                            collarTableDto.xPreview.Add(mFieldItems);
                            rows.Clear();
                        }

                        counter++;
                    }
                }
            }
            catch
            {
                throw new ImportFormatException("There is a problem with table format. Irrecoverable error at line: " + counter--.ToString());
            }

        }

        public async Task<CollarTableDto> RetrieveTableFieldnames(string tablePath, DrillholeTableType tableType, DrillholeImportFormat tableFormat, string tableName)
        {
            collarTableDto = new CollarTableDto();

            collarTableDto.tableLocation = tablePath;
            collarTableDto.tableType = tableType;
            collarTableDto.tableIsValid = true;
            collarTableDto.tableFormat = tableFormat;
            collarTableDto.tableName = tableName;

            if (collarTableDto.tableIsValid)
            {
                collarTableDto.tableData = new ImportTableFields();

                collarTableDto.fields = new List<string>();

                List<string> collarFields = new List<string>();

                bool bCSV = true;

                switch (tableFormat)
                {

                    case DrillholeImportFormat.text_csv:
                        {
                            collarFields = await RetrieveFieldnames(true, collarTableDto.tableLocation, collarTableDto.tableName);
                            break;
                        }

                    case DrillholeImportFormat.text_txt:
                        {
                            collarFields = await RetrieveFieldnames(false, collarTableDto.tableLocation, collarTableDto.tableName);

                            break;
                        }
                    case DrillholeImportFormat.other:
                        {
                            throw new CollarException("Table format currently not supported");
                        }

                    default:
                        throw new Exception("Generic error with previewing table");
                }


                if (collarFields.Count == 0)
                {
                    return RetrieveTemplateFieldnames().Result;
                }

                foreach (string field in collarFields)
                {
                    collarTableDto.fields.Add(field);
                }

                int index = 0;
                int fieldIncrement = 5;

                if (collarTableDto.fields[0].ToUpper() == "OBJECTID")
                {
                    index = index + 1;
                    fieldIncrement = fieldIncrement + 1;
                }

                if (collarTableDto.fields.Count < fieldIncrement)
                    throw new CollarException("Problem with fields in Collar table. Minimum of 5 fields required");

                //BHID
                collarTableDto.tableData.Add(new ImportTableField
                {
                    columnHeader = collarTableDto.fields[index],
                    columnImportAs = DrillholeConstants.holeID,
                    groupName = DrillholeConstants.GroupMapFields,
                    columnImportName = DrillholeConstants.holeIDName,
                    genericType = false,
                    fieldType = "Text",
                    keys = new KeyValuePair<bool, bool>(true, false) //Primary key
                });

                index++;
                //X
                collarTableDto.tableData.Add(new ImportTableField
                {
                    columnHeader = collarTableDto.fields[index],
                    columnImportAs = DrillholeConstants.x,
                    groupName = DrillholeConstants.GroupMapFields,
                    columnImportName = DrillholeConstants.xName,
                    genericType = false,
                    fieldType = "Double",
                    keys = new KeyValuePair<bool, bool>(false, false)
                });
                index++;
                //Y
                collarTableDto.tableData.Add(new ImportTableField
                {
                    columnHeader = collarTableDto.fields[index],
                    columnImportAs = DrillholeConstants.y,
                    groupName = DrillholeConstants.GroupMapFields,
                    columnImportName = DrillholeConstants.yName,
                    genericType = false,
                    fieldType = "Double",
                    keys = new KeyValuePair<bool, bool>(false, false)
                });
                index++;
                //Z
                collarTableDto.tableData.Add(new ImportTableField
                {
                    columnHeader = collarTableDto.fields[index],
                    columnImportAs = DrillholeConstants.z,
                    groupName = DrillholeConstants.GroupMapFields,
                    columnImportName = DrillholeConstants.zName,
                    genericType = false,
                    fieldType = "Double",
                    keys = new KeyValuePair<bool, bool>(false, false)
                });
                index++;
                //TD

                collarTableDto.tableData.Add(new ImportTableField
                {
                    columnHeader = collarTableDto.fields[index],
                    columnImportAs = DrillholeConstants.maxDepth,
                    groupName = DrillholeConstants.GroupMapFields,
                    columnImportName = DrillholeConstants.maxName,
                    genericType = false,
                    fieldType = "Double",
                    keys = new KeyValuePair<bool, bool>(false, false)
                });

                for (int i = fieldIncrement; i < collarTableDto.fields.Count; i++)
                {
                    collarTableDto.tableData.Add(new ImportTableField
                    {
                        columnHeader = collarTableDto.fields[i],
                        columnImportName = collarTableDto.fields[i],
                        columnImportAs = DrillholeConstants.notImported,
                        groupName = DrillholeConstants.GroupOtherFields,
                        genericType = true,
                        fieldType = "Text",
                        keys = new KeyValuePair<bool, bool>(false, false)
                    });
                }
            }

            return collarTableDto;
        }

        private async Task<List<string>> RetrieveFieldnames(bool bCSV, string tableLocation, string tableName)
        {
            List<string> collarFields = new List<string>();

            char delimiter = '\t';


            if (bCSV)
                delimiter = ',';

            string fileNameToCheck = tableLocation + "\\" + tableName;

            //E:\OneDrive - sonny-consulting.com\Projects\AusGold\Katanning\collar.csv

           // bool bExists = File.Exists(tableLocation + "\\" + tableName);
            bool bExists = File.Exists(fileNameToCheck);


            if (!bExists)
                throw new Exception("File does not exist");

            using (var reader = new StreamReader(tableLocation + "\\" + tableName))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(delimiter);

                    //first row only for columns
                    foreach (string column in values)
                    {
                        if (column != "")
                        {
                            string columnMod = column.Replace(" ", "_");

                            collarFields.Add(columnMod);
                        }

                    }
                    break;
                }
            }

            return collarFields;
        }

        public async Task<CollarTableDto> UpdateImportParameters(string previousSelection, string changeTo, string searchColumn, string strOldName, ImportTableFields collarTableFields)
        {
            if (collarTableDto == null)
                collarTableDto = new CollarTableDto()
                { 
                    tableData = collarTableFields
                };

            //Return row to be updated
            ImportTableField queryUpdate = (from column in collarTableDto.tableData
                                            where column.columnHeader == searchColumn
                                            select new ImportTableField
                                            {
                                                columnHeader = column.columnHeader,
                                                columnImportAs = column.columnImportAs,
                                                columnImportName = column.columnImportName,
                                                groupName = column.groupName,
                                                fieldType = column.fieldType,
                                                keys = column.keys
                                            }).SingleOrDefault();

            switch (changeTo)
            {
                case DrillholeConstants.holeID:
                    {
                        try
                        {
                            ReturnFormatAndUpdateFields.UpdateMandatoryFields(previousSelection, DrillholeConstants.holeID,
                                DrillholeConstants.holeIDName, queryUpdate, collarTableDto.tableData, DrillholeConstants.GroupMapFields,
                                false, "Text", new KeyValuePair<bool, bool>(true, false));
                        }
                        catch
                        {
                            ReturnFormatAndUpdateFields.UpdateOptionalFields(DrillholeConstants.holeIDName, DrillholeConstants.holeID, queryUpdate, collarTableDto.tableData, DrillholeConstants.GroupMapFields,
                                false, "Text", new KeyValuePair<bool, bool>(false, false));
                        }

                        break;
                    }
                case DrillholeConstants.x:
                    {
                        try
                        {
                            ReturnFormatAndUpdateFields.UpdateMandatoryFields(previousSelection, DrillholeConstants.x,
                            DrillholeConstants.xName, queryUpdate, collarTableDto.tableData, DrillholeConstants.GroupMapFields,
                            false, "Double", new KeyValuePair<bool, bool>(false, false));
                        }
                        catch
                        {
                            ReturnFormatAndUpdateFields.UpdateOptionalFields(DrillholeConstants.xName, DrillholeConstants.x, queryUpdate, collarTableDto.tableData, DrillholeConstants.GroupMapFields,
                                false, "Double", new KeyValuePair<bool, bool>(false, false));
                        }
                        break;
                    }
                case DrillholeConstants.y:
                    {
                        try
                        {
                            ReturnFormatAndUpdateFields.UpdateMandatoryFields(previousSelection, DrillholeConstants.y,
                                DrillholeConstants.yName, queryUpdate, collarTableDto.tableData, DrillholeConstants.GroupMapFields,
                                false, "Double", new KeyValuePair<bool, bool>(false, false));
                        }
                        catch
                        {
                            ReturnFormatAndUpdateFields.UpdateOptionalFields(DrillholeConstants.yName, DrillholeConstants.y, queryUpdate, collarTableDto.tableData, DrillholeConstants.GroupMapFields,
                                false, "Double", new KeyValuePair<bool, bool>(false, false));
                        }

                        break;

                    }
                case DrillholeConstants.z:
                    {
                        try
                        {
                            ReturnFormatAndUpdateFields.UpdateMandatoryFields(previousSelection, DrillholeConstants.z,
                                DrillholeConstants.zName, queryUpdate, collarTableDto.tableData, DrillholeConstants.GroupMapFields,
                                false, "Double", new KeyValuePair<bool, bool>(false, false));
                        }
                        catch
                        {
                            ReturnFormatAndUpdateFields.UpdateOptionalFields(DrillholeConstants.zName, DrillholeConstants.z, queryUpdate, collarTableDto.tableData, DrillholeConstants.GroupMapFields,
                                false, "Double", new KeyValuePair<bool, bool>(false, false));
                        }

                        break;
                    }
                case DrillholeConstants.maxDepth:
                    {
                        try
                        {
                            ReturnFormatAndUpdateFields.UpdateMandatoryFields(previousSelection, DrillholeConstants.maxDepth,
                                DrillholeConstants.maxName, queryUpdate, collarTableDto.tableData, DrillholeConstants.GroupMapFields,
                                false, "Double", new KeyValuePair<bool, bool>(false, false));
                        }
                        catch
                        {
                            ReturnFormatAndUpdateFields.UpdateOptionalFields("Dip", DrillholeConstants.maxDepth, queryUpdate, collarTableDto.tableData, DrillholeConstants.GroupOtherFields,
                                false, "Double", new KeyValuePair<bool, bool>(false, false));
                        }

                        break;


                    }
                case DrillholeConstants.azimuth:
                    {
                        try
                        {
                            ReturnFormatAndUpdateFields.UpdateMandatoryFields(previousSelection, DrillholeConstants.azimuth,
                                    DrillholeConstants.azimuthName, queryUpdate, collarTableDto.tableData, DrillholeConstants.GroupMapFields,
                                    false, "Double", new KeyValuePair<bool, bool>(false, false));
                        }
                        catch
                        {

                            ReturnFormatAndUpdateFields.UpdateOptionalFields("Azimuth", queryUpdate, collarTableDto.tableData, DrillholeConstants.GroupOtherFields,
                                false, "Double", new KeyValuePair<bool, bool>(false, false));
                        }

                        break;

                    }
                case DrillholeConstants.dip:
                    {
                        try
                        {
                            ReturnFormatAndUpdateFields.UpdateMandatoryFields(previousSelection, DrillholeConstants.dip,
                                    DrillholeConstants.dipName, queryUpdate, collarTableDto.tableData, DrillholeConstants.GroupMapFields,
                                    false, "Double", new KeyValuePair<bool, bool>(false, false));
                        }
                        catch
                        {

                            ReturnFormatAndUpdateFields.UpdateOptionalFields("Dip", queryUpdate, collarTableDto.tableData, DrillholeConstants.GroupOtherFields,
                            false, "Double", new KeyValuePair<bool, bool>(false, false));
                        }

                        break;
                    }
                case DrillholeConstants.notImported:
                    {
                        ReturnFormatAndUpdateFields.NotImportedFields(DrillholeConstants.notImported, queryUpdate, collarTableDto.tableData,
                            DrillholeConstants.GroupOtherFields, true);

                        break;
                    }

                case DrillholeConstants._numeric:
                    {
                        ReturnFormatAndUpdateFields.UpdateOptionalFields(DrillholeConstants._numeric, queryUpdate, collarTableDto.tableData,
                            DrillholeConstants.GroupOtherFields, true, "Double", new KeyValuePair<bool, bool>(false, false));
                        break;
                    }

                case DrillholeConstants._text:
                    ReturnFormatAndUpdateFields.UpdateOptionalFields(DrillholeConstants._text, queryUpdate, collarTableDto.tableData,
                        DrillholeConstants.GroupOtherFields, true, "Text", new KeyValuePair<bool, bool>(false, false));
                    break;

                case DrillholeConstants._generic:
                    ReturnFormatAndUpdateFields.UpdateOptionalFields(DrillholeConstants._genericName, queryUpdate, collarTableDto.tableData,
                        DrillholeConstants.GroupOtherFields, true, "Text", new KeyValuePair<bool, bool>(false, false));
                    break;

                default:
                    {
                        throw new TableTypeException("There is a problem with changing field type for Collar table");
                    }

            }

            collarTableDto.tableIsValid = true;

            return collarTableDto;
        }

        private async Task<CollarTableDto> RetrieveTemplateFieldnames()
        {
            collarTableDto = new CollarTableDto();

            collarTableDto.tableData = new ImportTableFields();

            collarTableDto.fields = new List<string>();

            //BHID
            collarTableDto.tableData.Add(new ImportTableField
            {
                columnHeader = DrillholeConstants.holeIDName,
                columnImportAs = DrillholeConstants.holeID,
                groupName = DrillholeConstants.GroupMapFields,
                columnImportName = DrillholeConstants.holeIDName,
                genericType = false,
                fieldType = "Text",
                keys = new KeyValuePair<bool, bool>(true, false) //Primary key
            });

            //X
            collarTableDto.tableData.Add(new ImportTableField
            {
                columnHeader = DrillholeConstants.xName,
                columnImportAs = DrillholeConstants.x,
                groupName = DrillholeConstants.GroupMapFields,
                columnImportName = DrillholeConstants.xName,
                genericType = false,
                fieldType = "Double",
                keys = new KeyValuePair<bool, bool>(false, false)
            });

            //Y
            collarTableDto.tableData.Add(new ImportTableField
            {
                columnHeader = DrillholeConstants.yName,
                columnImportAs = DrillholeConstants.y,
                groupName = DrillholeConstants.GroupMapFields,
                columnImportName = DrillholeConstants.yName,
                genericType = false,
                fieldType = "Double",
                keys = new KeyValuePair<bool, bool>(false, false)
            });

            //Z
            collarTableDto.tableData.Add(new ImportTableField
            {
                columnHeader = DrillholeConstants.zName,
                columnImportAs = DrillholeConstants.z,
                groupName = DrillholeConstants.GroupMapFields,
                columnImportName = DrillholeConstants.zName,
                genericType = false,
                fieldType = "Double",
                keys = new KeyValuePair<bool, bool>(false, false)
            });

            //TD
            collarTableDto.tableData.Add(new ImportTableField
            {
                columnHeader = DrillholeConstants.maxName,
                columnImportAs = DrillholeConstants.maxDepth,
                groupName = DrillholeConstants.GroupMapFields,
                columnImportName = DrillholeConstants.maxName,
                genericType = false,
                fieldType = "Double",
                keys = new KeyValuePair<bool, bool>(false, false)
            });

            collarTableDto.tableIsValid = true;
            collarTableDto.collarKey = DrillholeConstants.holeIDName; //set up primary key as 0 or 1 as default on startup

            return collarTableDto;
        }

        public async Task<CollarTableDto> RetrieveTableFieldnames(DrillholeImportFormat tableFormat, string tableLocation, string tableName)
        {
            collarTableDto = new CollarTableDto();

            collarTableDto.tableIsValid = true;

            collarTableDto.tableFormat = tableFormat;
            collarTableDto.tableLocation = tableLocation;
            collarTableDto.tableName = tableName;

            if (collarTableDto.tableIsValid)
            {
                collarTableDto.tableData = new ImportTableFields();

                collarTableDto.fields = new List<string>();

                List<string> collarFields = new List<string>();

                bool bCSV = true;

                switch (tableFormat)
                {

                    case DrillholeImportFormat.text_csv:
                        {
                            collarFields = await RetrieveFieldnames(true, collarTableDto.tableLocation, tableName);
                            break;
                        }

                    case DrillholeImportFormat.text_txt:
                        {
                            collarFields = await RetrieveFieldnames(false, collarTableDto.tableLocation, tableName);

                            break;
                        }
                    case DrillholeImportFormat.other:
                        {
                            throw new CollarException("Table format currently not supported");
                        }

                    default:
                        throw new Exception("Generic error with previewing table");
                }


                if (collarFields.Count == 0)
                {
                    return RetrieveTemplateFieldnames().Result;
                }

                foreach (string field in collarFields)
                {
                    collarTableDto.fields.Add(field);
                }

                int index = 0;
                int fieldIncrement = 5;

                if (collarTableDto.fields[0].ToUpper() == "OBJECTID")
                {
                    index = index + 1;
                    fieldIncrement = fieldIncrement + 1;
                }

                if (collarTableDto.fields.Count < fieldIncrement)
                    throw new CollarException("Problem with fields in Collar table. Minimum of 5 fields required");

                //BHID
                collarTableDto.tableData.Add(new ImportTableField
                {
                    columnHeader = collarTableDto.fields[index],
                    columnImportAs = DrillholeConstants.holeID,
                    groupName = DrillholeConstants.GroupMapFields,
                    columnImportName = DrillholeConstants.holeIDName,
                    genericType = false,
                    fieldType = "Text",
                    keys = new KeyValuePair<bool, bool>(true, false) //Primary key
                });

                index++;
                //X
                collarTableDto.tableData.Add(new ImportTableField
                {
                    columnHeader = collarTableDto.fields[index],
                    columnImportAs = DrillholeConstants.x,
                    groupName = DrillholeConstants.GroupMapFields,
                    columnImportName = DrillholeConstants.xName,
                    genericType = false,
                    fieldType = "Double",
                    keys = new KeyValuePair<bool, bool>(false, false)
                });
                index++;
                //Y
                collarTableDto.tableData.Add(new ImportTableField
                {
                    columnHeader = collarTableDto.fields[index],
                    columnImportAs = DrillholeConstants.y,
                    groupName = DrillholeConstants.GroupMapFields,
                    columnImportName = DrillholeConstants.yName,
                    genericType = false,
                    fieldType = "Double",
                    keys = new KeyValuePair<bool, bool>(false, false)
                });
                index++;
                //Z
                collarTableDto.tableData.Add(new ImportTableField
                {
                    columnHeader = collarTableDto.fields[index],
                    columnImportAs = DrillholeConstants.z,
                    groupName = DrillholeConstants.GroupMapFields,
                    columnImportName = DrillholeConstants.zName,
                    genericType = false,
                    fieldType = "Double",
                    keys = new KeyValuePair<bool, bool>(false, false)
                });
                index++;
                //TD

                collarTableDto.tableData.Add(new ImportTableField
                {
                    columnHeader = collarTableDto.fields[index],
                    columnImportAs = DrillholeConstants.maxDepth,
                    groupName = DrillholeConstants.GroupMapFields,
                    columnImportName = DrillholeConstants.maxName,
                    genericType = false,
                    fieldType = "Double",
                    keys = new KeyValuePair<bool, bool>(false, false)
                });

                for (int i = fieldIncrement; i < collarTableDto.fields.Count; i++)
                {
                    if (collarTableDto.fields[i] != "")
                    {
                        collarTableDto.tableData.Add(new ImportTableField
                        {
                            columnHeader = collarTableDto.fields[i],
                            columnImportName = collarTableDto.fields[i],
                            columnImportAs = DrillholeConstants.notImported,
                            groupName = DrillholeConstants.GroupOtherFields,
                            genericType = true,
                            fieldType = "Text",
                            keys = new KeyValuePair<bool, bool>(false, false)
                        });
                    }
                }
            }

            return collarTableDto;
        }

        public async Task<CollarTableDto> PreviewAndImportFields(DrillholeTableType tableType, int limit)
        {
            if (collarTableDto == null)
                collarTableDto = new CollarTableDto();

            if (collarTableDto.tableIsValid)
            {
                collarTableDto.tableType = tableType;

                switch (collarTableDto.tableFormat)
                {

                    case DrillholeImportFormat.text_csv:
                        {
                            PopulateXmlData(true);
                            break;
                        }

                    case DrillholeImportFormat.other:
                        {
                            throw new CollarException("Table format currently not supported");

                        }

                    case DrillholeImportFormat.text_txt:
                        {
                            PopulateXmlData(false);
                            break;

                        }

                    default:
                        throw new Exception("Generic error with previewing table");
                }
            }


            return collarTableDto;
        }
    }

    public class CsvRow
    {
        public string results { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drillholes.Domain.DTO;
using Drillholes.Domain.Enum;
using Drillholes.Domain.Interfaces;
using Drillholes.Domain;
using Drillholes.Domain.Exceptions;
using System.IO;
using System.Xml.Linq;

namespace Drillholes.FileDialog
{
    public class ContinuousImport : IContinuousTable
    {
        private static readonly object _collectionLock = new object();
        private ContinuousTableDto continuousTableDto;

        public async Task<ContinuousTableDto> ImportAllFieldsAsGeneric(bool bImport)
        {
            if (bImport)
            {
                var queryTo = continuousTableDto.tableData.Where(v => v.columnImportAs == DrillholeConstants.notImported);

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

            return continuousTableDto;
        }

        public async Task<ContinuousTableDto> PreviewAndImportFields(DrillholeTableType tableType, int limit)
        {
            if (continuousTableDto.tableIsValid)
            {
                switch (continuousTableDto.tableFormat)
                {

                    case DrillholeImportFormat.text_csv:
                        {
                            PopulateXmlData(true);
                            break;
                        }

                    case DrillholeImportFormat.other:
                        {
                            throw new ContinuousException("Table format currently not supported");

                        }

                    case DrillholeImportFormat.text_txt:
                        {
                            PopulateXmlData(false);
                            break;

                        }

                    default:
                        throw new ContinuousException("Generic error with previewing Interval table");
                }


            }

            return continuousTableDto;
        }
        private async void PopulateXmlData(bool bCSV)
        {
            string rootElement = "Continuous";
            continuousTableDto.xPreview = new XElement(rootElement);

            char delimiter = '\t';

            if (bCSV)
                delimiter = ',';

            List<CsvRow> rows = new List<CsvRow>();

            XElement mFieldItems = null;

            int counter = 0;

            try
            {
                using (var reader = new StreamReader(continuousTableDto.tableLocation + "\\" + continuousTableDto.tableName))
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

                            mFieldItems = new XElement(rootElement, new XAttribute("ID", (counter - 1).ToString()), new XAttribute("Ignore", "false"));

                            for (int i = 0; i < continuousTableDto.fields.Count; i++)
                            {
                                XElement mNode = new XElement(continuousTableDto.fields[i], rows[i].results);
                                mFieldItems.Add(mNode);
                            }

                            continuousTableDto.xPreview.Add(mFieldItems);
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

        public async Task<ContinuousTableDto> RetrieveTableFieldnames(DrillholeImportFormat tableFormat, string tableLocation, string tableName)
        {
            continuousTableDto = new ContinuousTableDto();

            continuousTableDto.tableLocation = tableLocation;
            continuousTableDto.tableIsValid = true;
            continuousTableDto.tableFormat = tableFormat;
            continuousTableDto.tableName = tableName;

            if (continuousTableDto.tableIsValid)
            {
                continuousTableDto.tableData = new ImportTableFields();

                continuousTableDto.fields = new List<string>();

                List<string> continuousFields = new List<string>();

                bool bCSV = true;

                switch (tableFormat)
                {
                    case DrillholeImportFormat.text_csv:
                        {
                            continuousFields = await RetrieveFieldnames(true);
                            break;
                        }

                    case DrillholeImportFormat.text_txt:
                        {
                            continuousFields = await RetrieveFieldnames(false);

                            break;
                        }
                    case DrillholeImportFormat.other:
                        {
                            throw new ContinuousException("Table format currently not supported");
                        }

                    default:
                        throw new ContinuousException("Generic error with previewing Interval table");
                }


                if (continuousFields.Count == 0)
                {
                    return RetrieveTemplateFieldnames().Result;
                }

                foreach (string field in continuousFields)
                {
                    continuousTableDto.fields.Add(field);
                }

                int index = 0;
                int fieldIncrement = 2;

                if (continuousTableDto.fields[0].ToUpper() == "OBJECTID")
                {
                    index = index + 1;
                    fieldIncrement = fieldIncrement + 1;
                }

                if (continuousTableDto.fields.Count < fieldIncrement)
                    throw new Exception("Problem with fields in Continuous Fields table. Minimum of 2 fields required");

                //HoleID
                continuousTableDto.tableData.Add(new ImportTableField
                {
                    columnHeader = continuousTableDto.fields[index],
                    columnImportAs = DrillholeConstants.holeID,
                    groupName = DrillholeConstants.GroupMapFields,
                    columnImportName = DrillholeConstants.holeIDName,
                    genericType = false,
                    fieldType = "Text",
                    keys = new KeyValuePair<bool, bool>(true, false) //true for primary key (holeID)
                });

                index++;

                //Distance From
                continuousTableDto.tableData.Add(new ImportTableField
                {
                    columnHeader = continuousTableDto.fields[index],
                    columnImportAs = DrillholeConstants.survDistance,
                    groupName = DrillholeConstants.GroupMapFields,
                    columnImportName = DrillholeConstants.distName,
                    genericType = false,
                    fieldType = "Double",
                    keys = new KeyValuePair<bool, bool>(false, true) //true for secondary key (holeID)
                });

                index++;

                if (continuousTableDto.fields.Count > fieldIncrement)
                {
                    for (int i = fieldIncrement; i < continuousTableDto.fields.Count; i++)
                    {
                        //bool bExists = false;

                        //bExists = CheckIfFieldExists(assayTableDto.fields[i]);

                        if (continuousTableDto.fields[i].ToUpper() != "OBJECTID")
                        {

                            continuousTableDto.tableData.Add(new ImportTableField
                            {
                                columnHeader = continuousTableDto.fields[i],
                                columnImportName = continuousTableDto.fields[i],
                                columnImportAs = DrillholeConstants.notImported,
                                groupName = DrillholeConstants.GroupOtherFields,
                                genericType = true,
                                fieldType = "Text",
                                keys = new KeyValuePair<bool, bool>(false, false) //not constraint field
                            });
                        }
                    }
                }

            }

            continuousTableDto.intervalKey = DrillholeConstants.holeIDName; //set up primary key as 0 or 1 as default on startup

            return continuousTableDto;
        }

        private async Task<List<string>> RetrieveFieldnames(bool bCSV)
        {
            List<string> continuousFields = new List<string>();

            char delimiter = '\t';


            if (bCSV)
                delimiter = ',';

            using (var reader = new StreamReader(continuousTableDto.tableLocation + "\\" + continuousTableDto.tableName))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(delimiter);

                    //first row only for columns
                    foreach (string column in values)
                    {
                        continuousFields.Add(column);

                    }
                    break;
                }
            }

            return continuousFields;
        }

        private async Task<ContinuousTableDto> RetrieveTemplateFieldnames()
        {
            continuousTableDto = new ContinuousTableDto();

            continuousTableDto.tableData = new ImportTableFields();

            continuousTableDto.fields = new List<string>();

            //BHID
            continuousTableDto.tableData.Add(new ImportTableField
            {
                columnHeader = DrillholeConstants.holeIDName,
                columnImportAs = DrillholeConstants.holeID,
                groupName = DrillholeConstants.GroupMapFields,
                columnImportName = DrillholeConstants.holeIDName,
                genericType = false,
                fieldType = "Text",
                keys = new KeyValuePair<bool, bool>(true, false) //Primary key
            });


            continuousTableDto.tableData.Add(new ImportTableField
            {
                columnHeader = DrillholeConstants.distName,
                columnImportAs = DrillholeConstants.survDistance,
                groupName = DrillholeConstants.GroupMapFields,
                columnImportName = DrillholeConstants.distName,
                genericType = false,
                fieldType = "Double",
                keys = new KeyValuePair<bool, bool>(false, true)
            }); //Distance field

            continuousTableDto.tableIsValid = true;
            continuousTableDto.assayKey = DrillholeConstants.holeIDName; //set up primary key as 0 or 1 as default on startup

            return continuousTableDto;
        }

        public async Task<ContinuousTableDto> UpdateImportParameters(string previousSelection, string changeTo, string searchColumn, string strOldName, ImportTableFields continuousTableFields)
        {
            if (continuousTableDto == null)
                continuousTableDto = new ContinuousTableDto()
                {
                    tableData = continuousTableFields
                };

            //Return row to be updated
            ImportTableField queryUpdate = (from column in continuousTableDto.tableData
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
                                DrillholeConstants.holeIDName, queryUpdate, continuousTableDto.tableData, DrillholeConstants.GroupMapFields,
                                false, "Text", new KeyValuePair<bool, bool>(true, false));
                        }
                        catch
                        {
                            ReturnFormatAndUpdateFields.UpdateOptionalFields(DrillholeConstants.holeIDName, DrillholeConstants.holeID, queryUpdate, continuousTableDto.tableData, DrillholeConstants.GroupMapFields,
                                false, "Text", new KeyValuePair<bool, bool>(false, false));
                        }
                        break;
                    }
                case DrillholeConstants.survDistance:
                    {
                        try
                        {
                            ReturnFormatAndUpdateFields.UpdateMandatoryFields(previousSelection, DrillholeConstants.survDistance,
                                    DrillholeConstants.distName, queryUpdate, continuousTableDto.tableData, DrillholeConstants.GroupMapFields,
                                    false, "Double", new KeyValuePair<bool, bool>(true, false));
                        }
                        catch
                        {

                            ReturnFormatAndUpdateFields.UpdateOptionalFields("Distance", queryUpdate, continuousTableDto.tableData, DrillholeConstants.GroupOtherFields,
                            false, "Double", new KeyValuePair<bool, bool>(false, false));
                        }

                        break;
                    }

                case DrillholeConstants.notImported:
                    {
                        ReturnFormatAndUpdateFields.NotImportedFields(DrillholeConstants.notImported, queryUpdate, continuousTableDto.tableData,
                            DrillholeConstants.GroupOtherFields, true);

                        break;
                    }

                case DrillholeConstants._numeric:
                    {
                        ReturnFormatAndUpdateFields.UpdateOptionalFields(DrillholeConstants._numeric, queryUpdate, continuousTableDto.tableData,
                            DrillholeConstants.GroupOtherFields, true, "Double", new KeyValuePair<bool, bool>(false, false));
                        break;
                    }

                case DrillholeConstants._alpha:
                    {
                        ReturnFormatAndUpdateFields.UpdateOptionalFields("Alpha", queryUpdate, continuousTableDto.tableData,
                            DrillholeConstants.GroupOtherFields, true, "Double", new KeyValuePair<bool, bool>(false, false));
                        break;
                    }
                case DrillholeConstants._beta:
                    {
                        ReturnFormatAndUpdateFields.UpdateOptionalFields("Beta", queryUpdate, continuousTableDto.tableData,
                            DrillholeConstants.GroupOtherFields, true, "Double", new KeyValuePair<bool, bool>(false, false));
                        break;
                    }
                case DrillholeConstants._gamma:
                    {
                        ReturnFormatAndUpdateFields.UpdateOptionalFields("Gamma", queryUpdate, continuousTableDto.tableData,
                            DrillholeConstants.GroupOtherFields, true, "Double", new KeyValuePair<bool, bool>(false, false));
                        break;
                    }
                case DrillholeConstants._density:
                    {
                        ReturnFormatAndUpdateFields.UpdateOptionalFields(DrillholeConstants._density, queryUpdate, continuousTableDto.tableData,
                            DrillholeConstants.GroupOtherFields, true, "Double", new KeyValuePair<bool, bool>(false, false));
                        break;
                    }

                case DrillholeConstants._text:
                    ReturnFormatAndUpdateFields.UpdateOptionalFields(DrillholeConstants._text, queryUpdate, continuousTableDto.tableData,
                        DrillholeConstants.GroupOtherFields, true, "Text", new KeyValuePair<bool, bool>(false, false));
                    break;

                case DrillholeConstants._generic: //TODO
                    ReturnFormatAndUpdateFields.UpdateOptionalFields(DrillholeConstants._genericName, queryUpdate, continuousTableDto.tableData,
                        DrillholeConstants.GroupOtherFields, true, "Text", new KeyValuePair<bool, bool>(false, false));
                    break;

                default:
                    {
                        throw new TableTypeException("There is a problem with changing field type for Continuous table");
                    }

            }

            continuousTableDto.tableIsValid = true;

            return continuousTableDto;
        }
    }
}

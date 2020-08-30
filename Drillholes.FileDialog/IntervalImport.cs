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
    public class IntervalImport : IIntervalTable
    {
        private static readonly object _collectionLock = new object();
        private IntervalTableDto intervalTableDto;

        public async Task<IntervalTableDto> ImportAllFieldsAsGeneric(bool bImport)
        {
            if (bImport)
            {
                var queryTo = intervalTableDto.tableData.Where(v => v.columnImportAs == DrillholeConstants.notImported);

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

            return intervalTableDto;
        }

        public async Task<IntervalTableDto> PreviewAndImportFields(DrillholeTableType tableType, int limit)
        {
            if (intervalTableDto.tableIsValid)
            {
                switch (intervalTableDto.tableFormat)
                {

                    case DrillholeImportFormat.text_csv:
                        {
                            PopulateXmlData(true);
                            break;
                        }

                    case DrillholeImportFormat.other:
                        {
                            throw new IntervalException("Table format currently not supported");

                        }

                    case DrillholeImportFormat.text_txt:
                        {
                            PopulateXmlData(false);
                            break;

                        }

                    default:
                        throw new IntervalException("Generic error with previewing Interval table");
                }


            }

            return intervalTableDto;
        }
        private async void PopulateXmlData(bool bCSV)
        {
            string rootElement = "Interval";
            intervalTableDto.xPreview = new XElement(rootElement + "s");

            char delimiter = '\t';

            if (bCSV)
                delimiter = ',';

            List<CsvRow> rows = new List<CsvRow>();

            XElement mFieldItems = null;

            int counter = 0;

            try
            {
                using (var reader = new StreamReader(intervalTableDto.tableLocation + "\\" + intervalTableDto.tableName))
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

                            for (int i = 0; i < intervalTableDto.fields.Count; i++)
                            {
                                string fieldName = intervalTableDto.fields[i].Replace(" ", "_");
                                XElement mNode = new XElement(fieldName, rows[i].results);
                                mFieldItems.Add(mNode);
                            }

                            intervalTableDto.xPreview.Add(mFieldItems);
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

        public async Task<IntervalTableDto> RetrieveTableFieldnames(DrillholeImportFormat tableFormat, string tableLocation, string tableName)
        {
            intervalTableDto = new IntervalTableDto();

            intervalTableDto.tableLocation = tableLocation;
            intervalTableDto.tableIsValid = true;
            intervalTableDto.tableFormat = tableFormat;
            intervalTableDto.tableName = tableName;

            if (intervalTableDto.tableIsValid)
            {
                intervalTableDto.tableData = new ImportTableFields();

                intervalTableDto.fields = new List<string>();

                List<string> intervalFields = new List<string>();

                bool bCSV = true;

                switch (tableFormat)
                {
                    case DrillholeImportFormat.text_csv:
                        {
                            intervalFields = await RetrieveFieldnames(true);
                            break;
                        }

                    case DrillholeImportFormat.text_txt:
                        {
                            intervalFields = await RetrieveFieldnames(false);

                            break;
                        }
                    case DrillholeImportFormat.other:
                        {
                            throw new IntervalException("Table format currently not supported");
                        }

                    default:
                        throw new IntervalException("Generic error with previewing Interval table");
                }


                if (intervalFields.Count == 0)
                {
                    return RetrieveTemplateFieldnames().Result;
                }

                foreach (string field in intervalFields)
                {
                    intervalTableDto.fields.Add(field);
                }

                int index = 0;
                int fieldIncrement = 3;

                if (intervalTableDto.fields[0].ToUpper() == "OBJECTID")
                {
                    index = index + 1;
                    fieldIncrement = fieldIncrement + 1;
                }

                if (intervalTableDto.fields.Count < fieldIncrement)
                    throw new Exception("Problem with fields in Interval table. Minimum of 3 fields required");

                //HoleID
                intervalTableDto.tableData.Add(new ImportTableField
                {
                    columnHeader = intervalTableDto.fields[index],
                    columnImportAs = DrillholeConstants.holeID,
                    groupName = DrillholeConstants.GroupMapFields,
                    columnImportName = DrillholeConstants.holeIDName,
                    genericType = false,
                    fieldType = "Text",
                    keys = new KeyValuePair<bool, bool>(true, false) //true for primary key (holeID)
                });

                index++;

                //Distance From
                intervalTableDto.tableData.Add(new ImportTableField
                {
                    columnHeader = intervalTableDto.fields[index],
                    columnImportAs = DrillholeConstants.distFrom,
                    groupName = DrillholeConstants.GroupMapFields,
                    columnImportName = DrillholeConstants.distFromName,
                    genericType = false,
                    fieldType = "Double",
                    keys = new KeyValuePair<bool, bool>(false, true) //true for secondary key (holeID)
                });

                index++;

                //Distance To
                intervalTableDto.tableData.Add(new ImportTableField
                {
                    columnHeader = intervalTableDto.fields[index],
                    columnImportAs = DrillholeConstants.distTo,
                    groupName = DrillholeConstants.GroupMapFields,
                    columnImportName = DrillholeConstants.distToName,
                    genericType = false,
                    fieldType = "Double",
                    keys = new KeyValuePair<bool, bool>(false, false) //true for secondary key (holeID)
                });


                if (intervalTableDto.fields.Count > fieldIncrement)
                {
                    for (int i = fieldIncrement; i < intervalTableDto.fields.Count; i++)
                    {
                        //bool bExists = false;

                        //bExists = CheckIfFieldExists(assayTableDto.fields[i]);

                        if (intervalTableDto.fields[i].ToUpper() != "OBJECTID")
                        {

                            intervalTableDto.tableData.Add(new ImportTableField
                            {
                                columnHeader = intervalTableDto.fields[i],
                                columnImportName = intervalTableDto.fields[i],
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

            intervalTableDto.intervalKey = DrillholeConstants.holeIDName; //set up primary key as 0 or 1 as default on startup

            return intervalTableDto;
        }

        private async Task<List<string>> RetrieveFieldnames(bool bCSV)
        {
            List<string> intervalFields = new List<string>();

            char delimiter = '\t';


            if (bCSV)
                delimiter = ',';

            using (var reader = new StreamReader(intervalTableDto.tableLocation + "\\" + intervalTableDto.tableName))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(delimiter);

                    //first row only for columns
                    foreach (string column in values)
                    {
                        string columnMod = column.Replace(" ", "_");
                        intervalFields.Add(columnMod);

                    }
                    break;
                }
            }

            return intervalFields;
        }

        private async Task<IntervalTableDto> RetrieveTemplateFieldnames()
        {
            intervalTableDto = new IntervalTableDto();

            intervalTableDto.tableData = new ImportTableFields();

            intervalTableDto.fields = new List<string>();

            //BHID
            intervalTableDto.tableData.Add(new ImportTableField
            {
                columnHeader = DrillholeConstants.holeIDName,
                columnImportAs = DrillholeConstants.holeID,
                groupName = DrillholeConstants.GroupMapFields,
                columnImportName = DrillholeConstants.holeIDName,
                genericType = false,
                fieldType = "Text",
                keys = new KeyValuePair<bool, bool>(true, false) //Primary key
            });


            intervalTableDto.tableData.Add(new ImportTableField
            {
                columnHeader = DrillholeConstants.distFromName,
                columnImportAs = DrillholeConstants.distFrom,
                groupName = DrillholeConstants.GroupMapFields,
                columnImportName = DrillholeConstants.distFromName,
                genericType = false,
                fieldType = "Double",
                keys = new KeyValuePair<bool, bool>(false, true)
            }); //Distance field

            intervalTableDto.tableData.Add(new ImportTableField
            {
                columnHeader = DrillholeConstants.distToName,
                columnImportAs = DrillholeConstants.distTo,
                groupName = DrillholeConstants.GroupMapFields,
                columnImportName = DrillholeConstants.distToName,
                genericType = false,
                fieldType = "Double",
                keys = new KeyValuePair<bool, bool>(false, false)
            }); //Azimuth field



            intervalTableDto.tableIsValid = true;
            intervalTableDto.assayKey = DrillholeConstants.holeIDName; //set up primary key as 0 or 1 as default on startup

            return intervalTableDto;
        }

        public async Task<IntervalTableDto> UpdateImportParameters(string previousSelection, string changeTo, string searchColumn, string strOldName, ImportTableFields intervalTableFields)
        {
            if (intervalTableDto == null)
                intervalTableDto = new IntervalTableDto()
                {
                    tableData = intervalTableFields
                };

            //Return row to be updated
            ImportTableField queryUpdate = (from column in intervalTableDto.tableData
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
                        ReturnFormatAndUpdateFields.UpdateMandatoryFields(previousSelection, DrillholeConstants.holeID,
                            DrillholeConstants.holeIDName, queryUpdate, intervalTableDto.tableData, DrillholeConstants.GroupMapFields,
                            false, "Text", new KeyValuePair<bool, bool>(true, false));
                        break;
                    }
                case DrillholeConstants.distFrom:
                    {
                        ReturnFormatAndUpdateFields.UpdateMandatoryFields(previousSelection, DrillholeConstants.distFrom,
                            DrillholeConstants.distFromName, queryUpdate, intervalTableDto.tableData, DrillholeConstants.GroupMapFields,
                            false, "Double", new KeyValuePair<bool, bool>(false, true));
                        break;
                    }
                case DrillholeConstants.distTo:
                    {
                        ReturnFormatAndUpdateFields.UpdateMandatoryFields(previousSelection, DrillholeConstants.distTo,
                            DrillholeConstants.distToName, queryUpdate, intervalTableDto.tableData, DrillholeConstants.GroupMapFields,
                            false, "Double", new KeyValuePair<bool, bool>(false, false));

                        break;

                    }

                case DrillholeConstants.notImported:
                    {
                        ReturnFormatAndUpdateFields.NotImportedFields(DrillholeConstants.notImported, queryUpdate, intervalTableDto.tableData,
                            DrillholeConstants.GroupOtherFields, true);

                        break;
                    }

                case DrillholeConstants._numeric:
                    {
                        ReturnFormatAndUpdateFields.UpdateOptionalFields(DrillholeConstants._numeric, queryUpdate, intervalTableDto.tableData,
                            DrillholeConstants.GroupOtherFields, true, "Double", new KeyValuePair<bool, bool>(false, false));
                        break;
                    }
                case DrillholeConstants._grade:
                    {
                        ReturnFormatAndUpdateFields.UpdateOptionalFields(DrillholeConstants._grade, queryUpdate, intervalTableDto.tableData,
                            DrillholeConstants.GroupOtherFields, true, "Double", new KeyValuePair<bool, bool>(false, false));
                        break;
                    }

                case DrillholeConstants._text:
                    ReturnFormatAndUpdateFields.UpdateOptionalFields(DrillholeConstants._text, queryUpdate, intervalTableDto.tableData,
                        DrillholeConstants.GroupOtherFields, true, "Text", new KeyValuePair<bool, bool>(false, false));
                    break;

                case DrillholeConstants._generic: //TODO
                    ReturnFormatAndUpdateFields.UpdateOptionalFields(DrillholeConstants._genericName, queryUpdate, intervalTableDto.tableData,
                        DrillholeConstants.GroupOtherFields, true, "Text", new KeyValuePair<bool, bool>(false, false));
                    break;
                case DrillholeConstants._alpha:
                    {
                        ReturnFormatAndUpdateFields.UpdateOptionalFields("Alpha", queryUpdate, intervalTableDto.tableData,
                            DrillholeConstants.GroupOtherFields, true, "Double", new KeyValuePair<bool, bool>(false, false));
                        break;
                    }
                case DrillholeConstants._beta:
                    {
                        ReturnFormatAndUpdateFields.UpdateOptionalFields("Beta", queryUpdate, intervalTableDto.tableData,
                            DrillholeConstants.GroupOtherFields, true, "Double", new KeyValuePair<bool, bool>(false, false));
                        break;
                    }
                case DrillholeConstants._gamma:
                    {
                        ReturnFormatAndUpdateFields.UpdateOptionalFields("Gamma", queryUpdate, intervalTableDto.tableData,
                            DrillholeConstants.GroupOtherFields, true, "Double", new KeyValuePair<bool, bool>(false, false));
                        break;
                    }
                case DrillholeConstants._density:
                    {
                        ReturnFormatAndUpdateFields.UpdateOptionalFields(DrillholeConstants._density, queryUpdate, intervalTableDto.tableData,
                            DrillholeConstants.GroupOtherFields, true, "Double", new KeyValuePair<bool, bool>(false, false));
                        break;
                    }


                    //default:
                    //    {
                    //        throw new TableTypeException("There is a problem with changing field type for Interval table");
                    //    }

            }

            intervalTableDto.tableIsValid = true;


            return intervalTableDto;
        }
    }
}

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
using System.Windows.Documents;

namespace Drillholes.FileDialog 
{
    public class AssayImport : IAssayTable
    {
        private static readonly object _collectionLock = new object();
        private AssayTableDto assayTableDto;

        public async Task<AssayTableDto> ImportAllFieldsAsGeneric(bool bImport)
        {
            if (bImport)
            {
                var queryTo = assayTableDto.tableData.Where(v => v.columnImportAs == DrillholeConstants.notImported);

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

            return assayTableDto;
        }

        public async Task<AssayTableDto> PreviewAndImportFields(DrillholeTableType tableType, int limit)
        {
            if (assayTableDto.tableIsValid)
            {
                switch (assayTableDto.tableFormat)
                {

                    case DrillholeImportFormat.text_csv:
                        {
                            PopulateXmlData(true);
                            break;
                        }

                    case DrillholeImportFormat.other:
                        {
                            throw new AssayException("Table format currently not supported");

                        }

                    case DrillholeImportFormat.text_txt:
                        {
                            PopulateXmlData(false);
                            break;

                        }

                    default:
                        throw new AssayException("Generic error with previewing Assay table");
                }


            }

            return assayTableDto;
        }
        private async void PopulateXmlData(bool bCSV)
        {
            string rootElement = "Assay";
            assayTableDto.xPreview = new XElement(rootElement + "s");

            char delimiter = '\t';

            if (bCSV)
                delimiter = ',';

            List<CsvRow> rows = new List<CsvRow>();

            XElement mFieldItems = null;

            int counter = 0;

            try
            {
                using (var reader = new StreamReader(assayTableDto.tableLocation + "\\" + assayTableDto.tableName))
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

                            for (int i = 0; i < assayTableDto.fields.Count; i++)
                            {
                                XElement mNode = new XElement(assayTableDto.fields[i], rows[i].results);
                                mFieldItems.Add(mNode);
                            }

                            assayTableDto.xPreview.Add(mFieldItems);
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

        public async Task<AssayTableDto> RetrieveTableFieldnames(DrillholeImportFormat tableFormat, string tableLocation, string tableName)
        {
            assayTableDto = new AssayTableDto();

            assayTableDto.tableLocation = tableLocation;
            assayTableDto.tableIsValid = true;
            assayTableDto.tableFormat = tableFormat;
            assayTableDto.tableName = tableName;

            if (assayTableDto.tableIsValid)
            {
                assayTableDto.tableData = new ImportTableFields();

                assayTableDto.fields = new List<string>();

                List<string> assayFields = new List<string>();

                bool bCSV = true;

                switch (tableFormat)
                {
                    case DrillholeImportFormat.text_csv:
                        {
                            assayFields = await RetrieveFieldnames(true);
                            break;
                        }

                    case DrillholeImportFormat.text_txt:
                        {
                            assayFields = await RetrieveFieldnames(false);

                            break;
                        }
                    case DrillholeImportFormat.other:
                        {
                            throw new AssayException("Table format currently not supported");
                        }

                    default:
                        throw new AssayException("Generic error with previewing Assay table");
                }


                if (assayFields.Count == 0)
                {
                    return RetrieveTemplateFieldnames().Result;
                }

                foreach (string field in assayFields)
                {
                    assayTableDto.fields.Add(field);
                }

                int index = 0;
                int fieldIncrement = 3;

                if (assayTableDto.fields[0].ToUpper() == "OBJECTID")
                {
                    index = index + 1;
                    fieldIncrement = fieldIncrement + 1;
                }

                if (assayTableDto.fields.Count < fieldIncrement)
                    throw new Exception("Problem with fields in Assay table. Minimum of 3 fields required");

                //HoleID
                assayTableDto.tableData.Add(new ImportTableField
                {
                    columnHeader = assayTableDto.fields[index],
                    columnImportAs = DrillholeConstants.holeID,
                    groupName = DrillholeConstants.GroupMapFields,
                    columnImportName = DrillholeConstants.holeIDName,
                    genericType = false,
                    fieldType = "Text",
                    keys = new KeyValuePair<bool, bool>(true, false) //true for primary key (holeID)
                });

                index++;

                //Distance From
                assayTableDto.tableData.Add(new ImportTableField
                {
                    columnHeader = assayTableDto.fields[index],
                    columnImportAs = DrillholeConstants.distFrom,
                    groupName = DrillholeConstants.GroupMapFields,
                    columnImportName = DrillholeConstants.distFromName,
                    genericType = false,
                    fieldType = "Double",
                    keys = new KeyValuePair<bool, bool>(false, true) //true for secondary key (holeID)
                });

                index++;

                //Distance To
                assayTableDto.tableData.Add(new ImportTableField
                {
                    columnHeader = assayTableDto.fields[index],
                    columnImportAs = DrillholeConstants.distTo,
                    groupName = DrillholeConstants.GroupMapFields,
                    columnImportName = DrillholeConstants.distToName,
                    genericType = false,
                    fieldType = "Double",
                    keys = new KeyValuePair<bool, bool>(false, false) //true for secondary key (holeID)
                });


                if (assayTableDto.fields.Count > fieldIncrement)
                {
                    for (int i = fieldIncrement; i < assayTableDto.fields.Count; i++)
                    {
                        //bool bExists = false;

                        //bExists = CheckIfFieldExists(assayTableDto.fields[i]);

                        if (assayTableDto.fields[i].ToUpper() != "OBJECTID")
                        {

                            assayTableDto.tableData.Add(new ImportTableField
                            {
                                columnHeader = assayTableDto.fields[i],
                                columnImportName = assayTableDto.fields[i],
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

            assayTableDto.assayKey = DrillholeConstants.holeIDName; //set up primary key as 0 or 1 as default on startup

            return assayTableDto;
        }

        private async Task<List<string>> RetrieveFieldnames(bool bCSV)
        {
            List<string> assayFields = new List<string>();

            char delimiter = '\t';


            if (bCSV)
                delimiter = ',';

            using (var reader = new StreamReader(assayTableDto.tableLocation + "\\" + assayTableDto.tableName))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(delimiter);

                    //first row only for columns
                    foreach (string column in values)
                    {
                        assayFields.Add(column);

                    }
                    break;
                }
            }

            return assayFields;
        }

        private async Task<AssayTableDto> RetrieveTemplateFieldnames()
        {
            assayTableDto = new AssayTableDto();

            assayTableDto.tableData = new ImportTableFields();

            assayTableDto.fields = new List<string>();

            //BHID
            assayTableDto.tableData.Add(new ImportTableField
            {
                columnHeader = DrillholeConstants.holeIDName,
                columnImportAs = DrillholeConstants.holeID,
                groupName = DrillholeConstants.GroupMapFields,
                columnImportName = DrillholeConstants.holeIDName,
                genericType = false,
                fieldType = "Text",
                keys = new KeyValuePair<bool, bool>(true, false) //Primary key
            });


            assayTableDto.tableData.Add(new ImportTableField
            {
                columnHeader = DrillholeConstants.distFromName,
                columnImportAs = DrillholeConstants.distFrom,
                groupName = DrillholeConstants.GroupMapFields,
                columnImportName = DrillholeConstants.distFromName,
                genericType = false,
                fieldType = "Double",
                keys = new KeyValuePair<bool, bool>(false, true)
            }); //Distance field

            assayTableDto.tableData.Add(new ImportTableField
            {
                columnHeader = DrillholeConstants.distToName,
                columnImportAs = DrillholeConstants.distTo,
                groupName = DrillholeConstants.GroupMapFields,
                columnImportName = DrillholeConstants.distToName,
                genericType = false,
                fieldType = "Double",
                keys = new KeyValuePair<bool, bool>(false, false)
            }); //Azimuth field



            assayTableDto.tableIsValid = true;
            assayTableDto.assayKey = DrillholeConstants.holeIDName; //set up primary key as 0 or 1 as default on startup

            return assayTableDto;
        }

        public async Task<AssayTableDto> UpdateImportParameters(string previousSelection, string changeTo, string searchColumn, string strOldName, ImportTableFields assayTableFields)
        {
            if (assayTableDto == null)
                assayTableDto = new AssayTableDto()
                {
                    tableData = assayTableFields
                };

            //Return row to be updated
            ImportTableField queryUpdate = (from column in assayTableDto.tableData
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
                            DrillholeConstants.holeIDName, queryUpdate, assayTableDto.tableData, DrillholeConstants.GroupMapFields,
                            false, "Text", new KeyValuePair<bool, bool>(true, false));
                        break;
                    }
                case DrillholeConstants.distFrom:
                    {
                        ReturnFormatAndUpdateFields.UpdateMandatoryFields(previousSelection, DrillholeConstants.distFrom,
                            DrillholeConstants.distFromName, queryUpdate, assayTableDto.tableData, DrillholeConstants.GroupMapFields,
                            false, "Double", new KeyValuePair<bool, bool>(false, true));
                        break;
                    }
                case DrillholeConstants.distTo:
                    {
                        ReturnFormatAndUpdateFields.UpdateMandatoryFields(previousSelection, DrillholeConstants.distTo,
                            DrillholeConstants.distToName, queryUpdate, assayTableDto.tableData, DrillholeConstants.GroupMapFields,
                            false, "Double", new KeyValuePair<bool, bool>(false, false));

                        break;

                    }

                case DrillholeConstants.notImported:
                    {
                        ReturnFormatAndUpdateFields.NotImportedFields(DrillholeConstants.notImported, queryUpdate, assayTableDto.tableData,
                            DrillholeConstants.GroupOtherFields, true);

                        break;
                    }

                case DrillholeConstants._numeric:
                    {
                        ReturnFormatAndUpdateFields.UpdateOptionalFields(DrillholeConstants._numeric, queryUpdate, assayTableDto.tableData,
                            DrillholeConstants.GroupOtherFields, true, "Double", new KeyValuePair<bool, bool>(false, false));
                        break;
                    }
                case DrillholeConstants._grade:
                    {
                        ReturnFormatAndUpdateFields.UpdateOptionalFields(DrillholeConstants._grade, queryUpdate, assayTableDto.tableData,
                            DrillholeConstants.GroupOtherFields, true, "Double", new KeyValuePair<bool, bool>(false, false));
                        break;
                    }
                case DrillholeConstants._sample:
                    {
                        ReturnFormatAndUpdateFields.UpdateOptionalFields(DrillholeConstants._sample, queryUpdate, assayTableDto.tableData,
                            DrillholeConstants.GroupOtherFields, true, "Text", new KeyValuePair<bool, bool>(false, false));
                        break;
                    }

                case DrillholeConstants._text:
                    ReturnFormatAndUpdateFields.UpdateOptionalFields(DrillholeConstants._text, queryUpdate, assayTableDto.tableData,
                        DrillholeConstants.GroupOtherFields, true, "Text", new KeyValuePair<bool, bool>(false, false));
                    break;

                case DrillholeConstants._generic: //TODO
                    ReturnFormatAndUpdateFields.UpdateOptionalFields(DrillholeConstants._genericName, queryUpdate, assayTableDto.tableData,
                        DrillholeConstants.GroupOtherFields, true, "Text", new KeyValuePair<bool, bool>(false, false));
                    break;

                    //default:
                    //    {
                    //        throw new TableTypeException("There is a problem with changing field type for Assay table");
                    //    }

            }

            assayTableDto.tableIsValid = true;


            return assayTableDto;
        }
    }
}

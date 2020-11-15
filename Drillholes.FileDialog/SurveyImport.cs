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
using System.Xml.Linq;
using System.IO;

namespace Drillholes.FileDialog
{
    public class SurveyImport : ISurveyTable
    {
        private static readonly object _collectionLock = new object();
        private SurveyTableDto surveyTableDto;

        public async Task<SurveyTableDto> ImportAllFieldsAsGeneric(bool bImport)
        {
            if (bImport)
            {
                var queryTo = surveyTableDto.tableData.Where(v => v.columnImportAs == DrillholeConstants.notImported);

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

            return surveyTableDto;
        }

        public async Task<SurveyTableDto> PreviewAndImportFields(DrillholeTableType tableType, int limit)
        {
            if (surveyTableDto == null)
                surveyTableDto = new SurveyTableDto();

      
                switch (surveyTableDto.tableFormat)
                {

                    case DrillholeImportFormat.text_csv:
                        {
                            PopulateXmlData(true);
                            break;
                        }

                    case DrillholeImportFormat.other:
                        {
                            throw new SurveyException("Table format currently not supported");

                        }

                    case DrillholeImportFormat.text_txt:
                        {
                            PopulateXmlData(false);
                            break;

                        }

                    default:
                        throw new SurveyException("Generic error with previewing Survey table");
                }
           

            return surveyTableDto;
        }

        private async void PopulateXmlData(bool bCSV)
        {
            surveyTableDto.xPreview = new XElement("Surveys");

            char delimiter = '\t';

            if (bCSV)
                delimiter = ',';

            List<CsvRow> rows = new List<CsvRow>();

            XElement mFieldItems = null;

            int counter = 0;

            bool bExists = File.Exists(surveyTableDto.tableLocation + "\\" + surveyTableDto.tableName);

            if (!bExists)
                throw new ImportFormatException("File doesn't exist");

            try
            {
                using (var reader = new StreamReader(surveyTableDto.tableLocation + "\\" + surveyTableDto.tableName))
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

                            mFieldItems = new XElement("Survey", new XAttribute("ID", (counter - 1).ToString()), new XAttribute("Ignore", "false"));

                            for (int i = 0; i < surveyTableDto.fields.Count; i++)
                            {
                                if (surveyTableDto.fields[i] != "")
                                {
                                    string fieldName = surveyTableDto.fields[i].Replace(" ", "_");

                                    XElement mNode = new XElement(fieldName, rows[i].results);
                                    mFieldItems.Add(mNode);
                                }
                            }

                            surveyTableDto.xPreview.Add(mFieldItems);
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

        public async Task<SurveyTableDto> RetrieveTableFieldnames(DrillholeImportFormat tableFormat, string tableLocation, string tableName)
        {
            surveyTableDto = new SurveyTableDto();

            surveyTableDto.tableLocation = tableLocation;
            surveyTableDto.tableIsValid = true;
            surveyTableDto.tableFormat = tableFormat;
            surveyTableDto.tableName = tableName;

            if (surveyTableDto.tableIsValid)
            {
                surveyTableDto.tableData = new ImportTableFields();

                surveyTableDto.fields = new List<string>();

                List<string> surveyFields = new List<string>();

                bool bCSV = true;

                switch (tableFormat)
                {

                    case DrillholeImportFormat.text_csv:
                        {
                            surveyFields = await RetrieveFieldnames(true);
                            break;
                        }

                    case DrillholeImportFormat.text_txt:
                        {
                            surveyFields = await RetrieveFieldnames(false);

                            break;
                        }
                    case DrillholeImportFormat.other:
                        {
                            throw new SurveyException("Table format currently not supported");
                        }

                    default:
                        throw new SurveyException("Generic error with previewing Survey table");
                }

                if (surveyFields.Count == 0)
                {
                    return RetrieveTemplateFieldnames().Result;
                }

                foreach (string field in surveyFields)
                {
                    surveyTableDto.fields.Add(field);
                }

                int index = 0;
                int fieldIncrement = 4;

                if (surveyTableDto.fields[0].ToUpper() == "OBJECTID")
                {
                    index = index + 1;
                    fieldIncrement = fieldIncrement + 1;
                }

                if (surveyTableDto.fields.Count < fieldIncrement)
                    throw new SurveyException("Problem with fields in Survey table. Minimum of 4 fields required");

                surveyTableDto.tableData.Add(new ImportTableField
                {
                    columnHeader = surveyTableDto.fields[index],
                    columnImportAs = DrillholeConstants.holeID,
                    groupName = DrillholeConstants.GroupMapFields,
                    columnImportName = DrillholeConstants.holeIDName,
                    genericType = false,
                    fieldType = "Text",
                    keys = new KeyValuePair<bool, bool>(true, false)
                }); //BHID

                index++;

                surveyTableDto.tableData.Add(new ImportTableField
                {
                    columnHeader = surveyTableDto.fields[index],
                    columnImportAs = DrillholeConstants.survDistance,
                    groupName = DrillholeConstants.GroupMapFields,
                    columnImportName = DrillholeConstants.distName,
                    genericType = false,
                    fieldType = "Double",
                    keys = new KeyValuePair<bool, bool>(false, true)
                }); //Distance field

                index++;

                surveyTableDto.tableData.Add(new ImportTableField
                {
                    columnHeader = surveyTableDto.fields[index],
                    columnImportAs = DrillholeConstants.azimuth,
                    groupName = DrillholeConstants.GroupMapFields,
                    columnImportName = DrillholeConstants.azimuthName,
                    genericType = false,
                    fieldType = "Double",
                    keys = new KeyValuePair<bool, bool>(false, false)
                }); //Azimuth field

                index++;

                surveyTableDto.tableData.Add(new ImportTableField
                {
                    columnHeader = surveyTableDto.fields[index],
                    columnImportAs = DrillholeConstants.dip,
                    groupName = DrillholeConstants.GroupMapFields,
                    columnImportName = DrillholeConstants.dipName,
                    genericType = false,
                    fieldType = "Double",
                    keys = new KeyValuePair<bool, bool>(false, false)
                }); //Dip field

                if (surveyTableDto.fields.Count > fieldIncrement)
                {
                    for (int i = fieldIncrement; i < surveyTableDto.fields.Count; i++)
                    {
                        if (surveyTableDto.fields[i].ToUpper() != "OBJECTID")
                        {
                            surveyTableDto.tableData.Add(new ImportTableField
                            {
                                columnHeader = surveyTableDto.fields[i],
                                columnImportName = surveyTableDto.fields[i],
                                columnImportAs = DrillholeConstants.notImported,
                                groupName = DrillholeConstants.GroupOtherFields,
                                genericType = true,
                                fieldType = "Text",
                                keys = new KeyValuePair<bool, bool>(false, false)
                            });
                        }
                    }
                }

                surveyTableDto.surveyKey = DrillholeConstants.holeIDName; //set up primary key as 0 or 1 as default on startup
            }
            return surveyTableDto;
        }

        private async Task<List<string>> RetrieveFieldnames(bool bCSV)
        {
            List<string> surveyFields = new List<string>();

            char delimiter = '\t';


            if (bCSV)
                delimiter = ',';

            using (var reader = new StreamReader(surveyTableDto.tableLocation + "\\" + surveyTableDto.tableName))
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
                            surveyFields.Add(columnMod);
                        }
                    }
                    break;
                }
            }

            return surveyFields;
        }

        public async Task<SurveyTableDto> UpdateImportParameters(string previousSelection, string changeTo, string searchColumn, string strOldName, ImportTableFields surveyTableFields)
        {
            if (surveyTableDto == null)
                surveyTableDto = new SurveyTableDto()
                {
                    tableData = surveyTableFields
                };

            //Return row to be updated
            ImportTableField queryUpdate = (from column in surveyTableDto.tableData
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
                                DrillholeConstants.holeIDName, queryUpdate, surveyTableDto.tableData, DrillholeConstants.GroupMapFields,
                                false, "Text", new KeyValuePair<bool, bool>(true, false));
                        }
                        catch
                        {
                            ReturnFormatAndUpdateFields.UpdateOptionalFields(DrillholeConstants.holeIDName, DrillholeConstants.holeID, queryUpdate, surveyTableDto.tableData, DrillholeConstants.GroupMapFields,
                                false, "Text", new KeyValuePair<bool, bool>(false, false));
                        }

                        break;
                    }
                case DrillholeConstants.survDistance:
                    {
                        try
                        {
                            ReturnFormatAndUpdateFields.UpdateMandatoryFields(previousSelection, DrillholeConstants.survDistance,
                                    DrillholeConstants.distName, queryUpdate, surveyTableDto.tableData, DrillholeConstants.GroupMapFields,
                                    false, "Double", new KeyValuePair<bool, bool>(true, false));
                        }
                        catch
                        {

                            ReturnFormatAndUpdateFields.UpdateOptionalFields("Distance", queryUpdate, surveyTableDto.tableData, DrillholeConstants.GroupOtherFields,
                            false, "Double", new KeyValuePair<bool, bool>(false, false));
                        }

                        break;
                    }
                case DrillholeConstants.azimuth:
                    {
                        try
                        {
                            ReturnFormatAndUpdateFields.UpdateMandatoryFields(previousSelection, DrillholeConstants.azimuth,
                                    DrillholeConstants.azimuthName, queryUpdate, surveyTableDto.tableData, DrillholeConstants.GroupMapFields,
                                    false, "Double", new KeyValuePair<bool, bool>(true, false));
                        }
                        catch
                        {

                            ReturnFormatAndUpdateFields.UpdateOptionalFields("Azimuth", queryUpdate, surveyTableDto.tableData, DrillholeConstants.GroupOtherFields,
                                false, "Double", new KeyValuePair<bool, bool>(false, false));
                        }

                        break;

                    }
                case DrillholeConstants.dip:
                    {
                        try
                        {
                            ReturnFormatAndUpdateFields.UpdateMandatoryFields(previousSelection, DrillholeConstants.dip,
                                    DrillholeConstants.dipName, queryUpdate, surveyTableDto.tableData, DrillholeConstants.GroupMapFields,
                                    false, "Double", new KeyValuePair<bool, bool>(false, false));
                        }
                        catch
                        {

                            ReturnFormatAndUpdateFields.UpdateOptionalFields("Dip", queryUpdate, surveyTableDto.tableData, DrillholeConstants.GroupOtherFields,
                            false, "Double", new KeyValuePair<bool, bool>(false, false));
                        }

                        break;
                    }
                case DrillholeConstants.notImported:
                    {
                        ReturnFormatAndUpdateFields.NotImportedFields(DrillholeConstants.notImported, queryUpdate, surveyTableDto.tableData,
                            DrillholeConstants.GroupOtherFields, true);

                        break;
                    }

                case DrillholeConstants._numeric:
                    {
                        ReturnFormatAndUpdateFields.UpdateOptionalFields(DrillholeConstants._numeric, queryUpdate, surveyTableDto.tableData,
                            DrillholeConstants.GroupOtherFields, true, "Double", new KeyValuePair<bool, bool>(false, false));
                        break;
                    }

                case DrillholeConstants._text:
                    ReturnFormatAndUpdateFields.UpdateOptionalFields(DrillholeConstants._text, queryUpdate, surveyTableDto.tableData,
                        DrillholeConstants.GroupOtherFields, true, "Text", new KeyValuePair<bool, bool>(false, false));
                    break;

                case DrillholeConstants._generic:
                    ReturnFormatAndUpdateFields.UpdateOptionalFields(DrillholeConstants._genericName, queryUpdate, surveyTableDto.tableData,
                        DrillholeConstants.GroupOtherFields, true, "Text", new KeyValuePair<bool, bool>(false, false));
                    break;

                default:
                    {
                        throw new TableTypeException("There is a problem with changing field type for Survey table");
                    }
            }

            surveyTableDto.tableIsValid = true;

            return surveyTableDto;
        }

        public async Task<SurveyTableDto> RetrieveTemplateFieldnames()
        {
            surveyTableDto = new SurveyTableDto();

            surveyTableDto.tableData = new ImportTableFields();

            surveyTableDto.fields = new List<string>();

            //BHID
            surveyTableDto.tableData.Add(new ImportTableField
            {
                columnHeader = DrillholeConstants.holeIDName,
                columnImportAs = DrillholeConstants.holeID,
                groupName = DrillholeConstants.GroupMapFields,
                columnImportName = DrillholeConstants.holeIDName,
                genericType = false,
                fieldType = "Text",
                keys = new KeyValuePair<bool, bool>(true, false) //Primary key
            });


            surveyTableDto.tableData.Add(new ImportTableField
            {
                columnHeader = DrillholeConstants.distName,
                columnImportAs = DrillholeConstants.survDistance,
                groupName = DrillholeConstants.GroupMapFields,
                columnImportName = DrillholeConstants.distName,
                genericType = false,
                fieldType = "Double",
                keys = new KeyValuePair<bool, bool>(false, true)
            }); //Distance field

            surveyTableDto.tableData.Add(new ImportTableField
            {
                columnHeader = DrillholeConstants.azimuthName,
                columnImportAs = DrillholeConstants.azimuth,
                groupName = DrillholeConstants.GroupMapFields,
                columnImportName = DrillholeConstants.azimuthName,
                genericType = false,
                fieldType = "Double",
                keys = new KeyValuePair<bool, bool>(false, false)
            }); //Azimuth field

            surveyTableDto.tableData.Add(new ImportTableField
            {
                columnHeader = DrillholeConstants.dipName,
                columnImportAs = DrillholeConstants.dip,
                groupName = DrillholeConstants.GroupMapFields,
                columnImportName = DrillholeConstants.dipName,
                genericType = false,
                fieldType = "Double",
                keys = new KeyValuePair<bool, bool>(false, false)
            }); //Dip field

            surveyTableDto.tableIsValid = true;
            surveyTableDto.surveyKey = DrillholeConstants.distName; //set up primary key as 0 or 1 as default on startup

            return surveyTableDto;
        }

        
    }
}

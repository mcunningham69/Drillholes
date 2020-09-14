using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drillholes.Domain.Enum;
using Drillholes.Domain.Exceptions;

namespace Drillholes.Domain
{
    public static class ReturnFormatAndUpdateFields
    {
        //when assigning key fields - from combobox
        public static void UpdateMandatoryFields(string previousSelection, string changeTo, string changeToName,
            ImportTableField queryUpdate, ImportTableFields tableData, string groupFields, bool genericType,
            string fieldType, KeyValuePair<bool, bool> keys)
        {

            if (queryUpdate.columnImportAs == changeTo) //if selected field not changed, then just update values below
            {
                var queryTo = tableData.Where(v => v.columnImportAs == changeTo).First();

                queryTo.genericType = genericType;
                queryTo.groupName = groupFields;
            }
            else
            {
                //queryUpdate takes the existing field with the value that are being used to change the selected field. queryUpdate then 
                //takes the selected field's original values

                //this is the field that is being changed
                ImportTableField queryPrevious = (from column in tableData
                                                  where column.columnImportName == changeToName
                                                  select new ImportTableField
                                                  {
                                                      columnHeader = column.columnHeader,
                                                      columnImportAs = column.columnImportAs,
                                                      columnImportName = column.columnImportName,
                                                      groupName = column.groupName,
                                                      fieldType = column.fieldType,
                                                      keys = column.keys
                                                  }).FirstOrDefault();

               

                //take one to change and give to row that was previously assinged (i.e. reverse values)
                ImportTableField queryFrom = tableData.Where(v => v.columnImportAs == changeTo).FirstOrDefault();

                if (queryFrom == null)
                    queryFrom = new ImportTableField();

                string previousColumnImportAs = queryUpdate.columnImportAs;

                //if field being changed from is NotImported then use original column name for import as
                string previousColumnImportName = "";
                if (previousSelection == DrillholeConstants.notImported)
                {
                    if (queryPrevious != null)
                        previousColumnImportName = queryPrevious.columnHeader;
                    else
                        previousColumnImportName = queryUpdate.columnImportName;

                }
                else
                {
                    previousColumnImportName = queryUpdate.columnImportName;
                }

                string previousGroupName = queryUpdate.groupName;
                bool previousType = queryUpdate.genericType;
                string previousFieldType = queryUpdate.fieldType;
                KeyValuePair<bool, bool> previousKeys = queryUpdate.keys;

                //the one to change
                var queryTo = tableData.Where(v => v.columnHeader == queryUpdate.columnHeader).First();

                //update changed from values
                queryFrom.columnImportAs = previousColumnImportAs;
                queryFrom.columnImportName = previousColumnImportName;
                queryFrom.groupName = previousGroupName;
                queryFrom.genericType = previousType;
                queryFrom.fieldType = previousFieldType;
                queryFrom.keys = previousKeys;

                //update changed to values
                queryTo.columnHeader.ToUpper(); //Make upper case
                queryTo.columnImportAs = changeTo;
                queryTo.columnImportName = changeToName;
                queryTo.groupName = groupFields;
                queryTo.fieldType = fieldType;  //check
                queryTo.genericType = genericType;
                queryTo.keys = keys;
            }


        }

        public static void SetPrimaryKey(string changeTo, string changeToName, ImportTableField queryUpdate, ImportTableFields tableData,
            string groupFields, bool genericType, string fieldType, KeyValuePair<bool, bool> keys)
        {
            //retrieve values to update row being changed from
            ImportTableField queryPrevious = (from column in tableData
                                              where column.columnImportName == changeToName
                                              select new ImportTableField
                                              {
                                                  columnHeader = column.columnHeader,
                                                  columnImportAs = column.columnImportAs,
                                                  columnImportName = column.columnImportName,
                                                  groupName = column.groupName,
                                                  genericType = column.genericType,
                                                  fieldType = column.fieldType,
                                                  keys = column.keys
                                              }).SingleOrDefault();

            //take one to change and give to row that was previously assinged (i.e. reverse values)
            var queryFrom = tableData.Where(v => v.columnImportAs == changeTo).First();

            string previousColumnImportAs = queryUpdate.columnImportAs;
            string previousColumnImportName = queryUpdate.columnImportName;
            string previousGroupName = queryUpdate.groupName;
            bool previousType = queryUpdate.genericType;
            string previousFieldType = queryUpdate.fieldType;
            KeyValuePair<bool, bool> previousKeys = queryUpdate.keys;

            //the one to change
            var queryTo = tableData.Where(v => v.columnHeader == queryUpdate.columnHeader).First();

            //update changed from values
            queryFrom.columnImportAs = previousColumnImportAs;
            queryFrom.columnImportName = previousColumnImportName;
            queryFrom.groupName = previousGroupName;
            queryFrom.genericType = previousType;
            queryFrom.fieldType = previousFieldType;
            queryFrom.keys = previousKeys;

            //update changed to values
            queryTo.columnImportAs = changeTo;
            queryTo.columnImportName = changeToName;
            queryTo.groupName = groupFields;
            queryTo.genericType = genericType;
            queryTo.fieldType = fieldType;  //check
            queryTo.keys = keys;


        }

        //when changing a "not imported" field to  a key type
        public static void NotImportedFields(string changeTo, ImportTableField queryUpdate, ImportTableFields tableData,
            string groupFields, bool genericType)
        {

            if (queryUpdate.columnImportAs == changeTo)
            {
                var queryTo = tableData.Where(v => v.columnImportAs == changeTo).First();

                queryTo.genericType = genericType;
                queryTo.groupName = groupFields;
                queryTo.fieldType = queryUpdate.fieldType;
                queryTo.keys = new KeyValuePair<bool, bool>(false, false);
            }
            else if (queryUpdate.columnImportAs == DrillholeConstants._genericName)
            {
                var queryTo = tableData.Where(v => v.columnImportAs == DrillholeConstants._genericName && v.columnHeader == queryUpdate.columnHeader).First();

                queryTo.columnImportAs = DrillholeConstants.notImported;
                queryTo.columnImportName = queryTo.columnHeader;
                queryTo.groupName = groupFields;
                queryTo.genericType = true;
                queryTo.fieldType = "Text";  //NEED TODO 
                queryTo.keys = new KeyValuePair<bool, bool>(false, false);

            }
            else if (changeTo == DrillholeConstants.notImported && queryUpdate.columnImportAs == DrillholeConstants._numeric)
            {
                var queryTo = tableData.Where(v => v.columnImportAs == DrillholeConstants._numeric && v.columnHeader == queryUpdate.columnHeader).First();

                queryTo.columnImportAs = DrillholeConstants.notImported;
                queryTo.columnImportName = queryTo.columnHeader;
                queryTo.groupName = groupFields;
                queryTo.genericType = true;
                queryTo.fieldType = "Double";
                queryTo.keys = new KeyValuePair<bool, bool>(false, false);

            }
            else if (changeTo == DrillholeConstants.notImported && queryUpdate.columnImportAs == DrillholeConstants._text)
            {
                var queryTo = tableData.Where(v => v.columnImportAs == DrillholeConstants._text && v.columnHeader == queryUpdate.columnHeader).First();

                queryTo.columnImportAs = DrillholeConstants.notImported;
                queryTo.columnImportName = queryTo.columnHeader;
                queryTo.groupName = groupFields;
                queryTo.genericType = true;
                queryTo.fieldType = "Text";
                queryTo.keys = new KeyValuePair<bool, bool>(false, false);

            }
            else
            {
                //retrieve values to update row being changed from
                ImportTableField queryPrevious = (from column in tableData
                                                  where column.columnImportName == changeTo
                                                  select new ImportTableField
                                                  {
                                                      columnHeader = column.columnHeader,
                                                      columnImportAs = column.columnImportAs,
                                                      columnImportName = column.columnImportName,
                                                      groupName = column.groupName,
                                                      genericType = column.genericType,
                                                      fieldType = column.fieldType,
                                                      keys = column.keys
                                                  }).SingleOrDefault();

                //take one to change and give to row that was previously assinged (i.e. reverse values)
                var queryFrom = tableData.Where(v => v.columnImportAs == changeTo).First();

                string previousColumnImportAs = queryUpdate.columnImportAs;
                string previousColumnImportName = queryUpdate.columnImportName;
                string previousGroupName = queryUpdate.groupName;
                bool previousType = queryUpdate.genericType;

                string previousFieldType = queryUpdate.fieldType;
                KeyValuePair<bool, bool> previousKeys = queryUpdate.keys;

                //the one to change
                var queryTo = tableData.Where(v => v.columnHeader == queryUpdate.columnHeader).First();

                //update changed from values
                queryFrom.columnImportAs = previousColumnImportAs;
                queryFrom.columnImportName = previousColumnImportName;
                queryFrom.groupName = previousGroupName;
                queryFrom.genericType = previousType;
                queryFrom.fieldType = previousFieldType;
                queryFrom.keys = previousKeys;

                //update changed to values
                queryTo.columnImportAs = changeTo;
                queryTo.columnImportName = queryUpdate.columnHeader;
                queryTo.groupName = groupFields;
                queryTo.genericType = genericType;
                queryTo.fieldType = queryUpdate.fieldType;  //check
                queryTo.keys = new KeyValuePair<bool, bool>(false, false);



            }

        }

        public static void UpdateOptionalFields(string changeTo, ImportTableField queryUpdate, ImportTableFields tableData,
            string groupFields, bool genericType, string fieldType, KeyValuePair<bool, bool> keys)
        {
            //the one to change
            var queryTo = tableData.Where(v => v.columnHeader == queryUpdate.columnHeader).First();

            string columnImportName = queryUpdate.columnHeader;

            if (changeTo == "Azimuth" || changeTo == "Dip")
            {
                if (changeTo == "Azimuth")
                    columnImportName = DrillholeConstants.azimuthName;
                else
                    columnImportName = DrillholeConstants.dipName;
            }


            //update changed to values
            queryTo.columnImportAs = changeTo;
            queryTo.columnImportName = columnImportName;
            queryTo.groupName = groupFields;
            queryTo.keys = keys;
            queryTo.fieldType = fieldType;

            queryTo.genericType = genericType;

        }

        public static void UpdateOptionalFields(string changeTo, string importAs, ImportTableField queryUpdate, ImportTableFields tableData,
    string groupFields, bool genericType, string fieldType, KeyValuePair<bool, bool> keys)
        {
            //the one to change
            var queryTo = tableData.Where(v => v.columnHeader == queryUpdate.columnHeader).First();

            if (changeTo == "Azimuth" || changeTo == "Dip")
            {

            }

            //update changed to values
            queryTo.columnImportAs = importAs;
            queryTo.columnImportName = queryUpdate.columnHeader;
            queryTo.groupName = groupFields;
            queryTo.keys = keys;
            queryTo.fieldType = fieldType;

            queryTo.genericType = genericType;

        }

        public static void GenericFields(ImportTableFields tableData, string strNotImported, string strGeneric,
            string groupFields, bool bImport)
        {

            if (bImport)
            {
                var queryTo = tableData.Where(v => v.columnImportAs == strNotImported);

                foreach (var _val in queryTo)
                {
                    _val.columnImportAs = strGeneric;
                    _val.columnImportName = _val.columnHeader;
                    _val.groupName = groupFields;
                    _val.genericType = true;
                    _val.fieldType = "Text"; //TODO
                    _val.keys = new KeyValuePair<bool, bool>(false, false);
                }

            }
            else
            {
                // throw new TableTypeException("Problem with Import of Fields");
            }

        }

        //method redundant
        public static DrillholeImportFormat PreviewTableParameter(string tableTInputType)
        {

            switch (tableTInputType)
            {
                case "excel_table":
                    return DrillholeImportFormat.excel_table;
                case "egdb_table":
                    return DrillholeImportFormat.egdb_table;
                case "fgdb_table":
                    return DrillholeImportFormat.fgdb_table;

                case "pgdb_table":
                    throw new TableTypeException("Personal geodatabase tables not supported");

                case "text_csv":
                    return DrillholeImportFormat.text_csv;

                case "text_txt":
                    return DrillholeImportFormat.text_txt;

                case "other":
                    return DrillholeImportFormat.other;

                default:
                    throw new TableTypeException("Input format problem");
            }


        }


    }

}

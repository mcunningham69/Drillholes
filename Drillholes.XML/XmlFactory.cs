using Drillholes.Domain;
using Drillholes.Domain.DataObject;
using Drillholes.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.VisualBasic;
using System.Text.RegularExpressions;

namespace Drillholes.XML
{
    public class XmlFactory
    {
        XmlManagement _xml;

        public XmlFactory(DrillholesXmlEnum xmlMode)
        {
            SetXmlType(xmlMode);
        }

        public void SetXmlType(DrillholesXmlEnum xmlMode)
        {
            _xml = XmlManagement.xmlType(xmlMode);
        }

        public async Task<XElement> CreateXML(string fullXmlName, object xmlValues, DrillholeTableType tableType, string rootName)
        {
            return _xml.CreateXML(fullXmlName, xmlValues, tableType, rootName).Result;
        }


        public async Task<object> OpenXML(string fullXmlName)
        {
            return _xml.OpenXML(fullXmlName).Result;
        }
        public async Task<object> ReplaceXmlNode(string fullXmlName, object xmlValues, XDocument xmlData, DrillholeTableType tableType, string xmlNodeTableNam, string rootName)
        {
            return _xml.ReplaceXmlNode(fullXmlName, xmlValues, xmlData, tableType, xmlNodeTableNam, rootName).Result;
        }
        public async Task<XElement> UpdateXmlNode(string fullXmlName, string xmlName, object xmlChange, XDocument xmlData, DrillholeTableType tableType, string rootName)
        {
            return _xml.UpdateXmlNodes(fullXmlName, xmlName, xmlChange, xmlData, tableType, rootName).Result;
        }

        public async void UpdateProjectFile(string projectFile, string drillholeFile, string drillholeRoot, DrillholeTableType tableType)
        {
            _xml.UpdateProjectFile(projectFile, drillholeFile, drillholeRoot, tableType);
        }

        public async Task<object> ReturnValuesFromXML(string projectFile, string drillholeFile, string drillholeProjectRoot, string drillholeRoot, DrillholeTableType tableType)
        {
            return await _xml.ReturnValuesFromXML(projectFile, drillholeFile, drillholeProjectRoot, drillholeRoot, tableType);
        }


    }

    public abstract class XmlManagement
    {
        public static XmlManagement xmlType(DrillholesXmlEnum xmlMode)
        {
            switch (xmlMode)
            {
                case DrillholesXmlEnum.DrillholeTableParameters:
                    return new XmlTableParameters();
                case DrillholesXmlEnum.DrillholeFields:
                    return new XmlTableFields();
                case DrillholesXmlEnum.DrillholeInputData:
                    return new XmlTableInputdata();
                case DrillholesXmlEnum.DrillholeDesurveyData:
                    return null;
                case DrillholesXmlEnum.DrillholePreferences:
                    return new XmlDrillholePreferences();
                case DrillholesXmlEnum.DrillholeProject:
                    return new XmlSavedSession();
            }

            return null;
        }

        public abstract Task<XElement> CreateXML(string fullXmlName, object xmlValues, DrillholeTableType tableType, string rootName);
        public abstract Task<object> OpenXML(string fullXmlName);
        public abstract void SaveXML(XDocument xmlFile, string fullXmlName);
        public abstract Task<object> ReplaceXmlNode(string fullXmlName, object xmlValues, XDocument xmlData, DrillholeTableType tableType, string xmlNodeTableNam, string rootName);

        public abstract Task<XElement> UpdateXmlNodes(string fullXmlName, string xmlName, object xmlChange, XDocument xmlData, DrillholeTableType tableType, string rootName);

        public abstract void UpdateProjectFile(string projectFile, string drillholeFile, string drillholeRoot, DrillholeTableType tableType);
        public abstract Task<object> ReturnValuesFromXML(string projectFile, string drillholeFile, string drillholeProjectRoot, string drillholeRoot, DrillholeTableType tableType);


    }

    public class XmlTableParameters : XmlManagement
    {
        public override async Task<XElement> CreateXML(string fullXmlName, object xmlValues, DrillholeTableType tableType, string rootName)
        {
            var tableValues = (List<DrillholeTable>)xmlValues;

            XElement tableParameters = null;

            XDocument xmlFile = new XDocument(
               new XDeclaration("1.0", null, null),
               new XProcessingInstruction("order", "alpha ascending"),
               new XElement(rootName, new XAttribute("Modified", DateTime.Now)));

            foreach (var table in tableValues)
            {
                List<XElement> nodes = new List<XElement>();
                nodes.Add(new XElement("TableName", table.tableName));
                nodes.Add(new XElement("TableFormat", table.tableFormat.ToString()));
                nodes.Add(new XElement("TableLocation", table.tableLocation));
                nodes.Add(new XElement("Cancelled", table.isCancelled.ToString()));
                nodes.Add(new XElement("Valid", table.isValid.ToString()));

                tableParameters = new XElement("TableType", new XAttribute("Value", table.tableType.ToString()));
                tableParameters.Add(nodes);

                xmlFile.Root.Add(tableParameters);

            }

            SaveXML(xmlFile, fullXmlName);

            return xmlFile.Element(rootName);
        }

        public override async Task<object> OpenXML(string fullXmlName)
        {
            return XDocument.Load(fullXmlName);
        }

        public override async void SaveXML(XDocument xmlFile, string fullXmlName)
        {
            xmlFile.Save(fullXmlName);
        }

        public override async Task<object> ReplaceXmlNode(string fullXmlName, object xmlValues, XDocument xmlData, DrillholeTableType tableType, string xmlNodeTableNam, string rootName)
        {
            var tableValues = (List<DrillholeTable>)xmlValues;

            XElement tableParameters = null;

            var elements = xmlData.Descendants(rootName).Elements();

            foreach (var table in tableValues)
            {
                var updateValues = elements.Where(e => e.Attribute("Value").Value == table.tableType.ToString());

                List<XElement> nodes = new List<XElement>();
                nodes.Add(new XElement("TableName", table.tableName));
                nodes.Add(new XElement("TableFormat", table.tableFormat.ToString()));
                nodes.Add(new XElement("TableLocation", table.tableLocation));
                nodes.Add(new XElement("Cancelled", table.isCancelled.ToString()));
                nodes.Add(new XElement("Valid", table.isValid.ToString()));

                if (updateValues.Any())
                    updateValues.Remove();

                tableParameters = new XElement("TableType", new XAttribute("Value", table.tableType.ToString()));
                tableParameters.Add(nodes);

                xmlData.Root.Add(tableParameters);

            }

            //change modified time
            var modified = xmlData.Descendants(rootName).Select(m => m.Attribute("Modified")).FirstOrDefault();
            if (modified != null)
                modified.Value = DateTime.Now.ToString();

            SaveXML(xmlData, fullXmlName);

            return xmlData.Element(rootName);
        }

        public override Task<XElement> UpdateXmlNodes(string fullXmlName, string xmlName, object xmlChange, XDocument xmlData, DrillholeTableType tableType, string rootName)
        {
            //TODO
            throw new NotImplementedException();
        }

        public override async void UpdateProjectFile(string projectFile, string drillholeFile, string drillholeProjectRoot, DrillholeTableType tableType)
        {
            XDocument xmlFile = await OpenXML(projectFile) as XDocument;
            var elements = xmlFile.Descendants(drillholeProjectRoot).Elements();

            var check = elements.Select(e => e.Element(DrillholeConstants.drillholeTable).Value).SingleOrDefault();

            if (check == "")  //saved session at dialog page
            {
                var updateValues = elements.Select(e => e.Element(DrillholeConstants.drillholeTable)).SingleOrDefault();
                updateValues.Value = drillholeFile;
            }

            SaveXML(xmlFile, projectFile);
        }

        public override async Task<object> ReturnValuesFromXML(string projectFile, string drillholeFile, string drillholeProjectRoot, string drillholeRoot, DrillholeTableType tableType)
        {
            XDocument xmlFile = await OpenXML(projectFile) as XDocument;
            var elements = xmlFile.Descendants(drillholeProjectRoot).Elements();

            List<DrillholeTable> tables = new List<DrillholeTable>();

            string drillholeTables = elements.Select(e => e.Element(DrillholeConstants.drillholeTable).Value).SingleOrDefault(); //check for drillhole table

            if (drillholeTables == "")
                return null;

            XDocument tableProperties = await OpenXML(drillholeTables) as XDocument;

                var tableElements = tableProperties.Descendants(drillholeRoot).Elements();

                var properties = tableElements.Select(f => f.Attribute("Value"));

                foreach (var property in properties)
                {
                    switch (property.Value)
                    {
                        case "collar":
                            var collar = await ReturnTableProperties(property.Parent, DrillholeTableType.collar);
                            tables.Add(collar);
                            break;
                        case "survey":
                            var survey = await ReturnTableProperties(property.Parent, DrillholeTableType.survey);
                            tables.Add(survey);
                            break;
                        case "assay":
                            var assay = await ReturnTableProperties(property.Parent, DrillholeTableType.assay);
                            tables.Add(assay);
                            break;
                        case "interval":
                            var interval = await ReturnTableProperties(property.Parent, DrillholeTableType.interval);
                            tables.Add(interval);
                            break;
                        case "continuous":
                            var continuous = await ReturnTableProperties(property.Parent, DrillholeTableType.continuous);
                            tables.Add(continuous);
                            break;
                    }
                }

            return tables;
        }

        private async Task<DrillholeTable> ReturnTableProperties(XElement elements, DrillholeTableType tableType)
        {
            var query = elements.Element("TableFormat").Value;

            DrillholeImportFormat importFormat = DrillholeImportFormat.other;

            if (query == "egdb_table")
                importFormat = DrillholeImportFormat.egdb_table;
            else if (query == "excel_table")
                importFormat = DrillholeImportFormat.excel_table;
            else if (query == "fgdb_table")
                importFormat = DrillholeImportFormat.fgdb_table;
            else if (query == "pgdb_table")
                importFormat = DrillholeImportFormat.pgdb_table;
            else if (query == "text_csv")
                importFormat = DrillholeImportFormat.text_csv;
            else if (query == "text_txt")
                importFormat = DrillholeImportFormat.text_txt;

            var table = new DrillholeTable(true)
            {
                tableFormat = importFormat,
                tableLocation = elements.Element("TableLocation").Value,
                tableName = elements.Element("TableName").Value,
                tableFormatName = elements.Element("TableFormat").Value,
                tableType = tableType
            };


            return table;
        }
    }

    public class XmlTableInputdata : XmlManagement
    {

        public override async Task<XElement> CreateXML(string fullXmlName, object xmlValues, DrillholeTableType tableType, string rootName)
        {
            var tableValues = (XElement)xmlValues;

            XElement tableParameters = null;

            XDocument xmlFile = new XDocument(
               new XDeclaration("1.0", null, null),
               new XProcessingInstruction("order", "alpha ascending"),
               new XElement(rootName, new XAttribute("Modified", DateTime.Now)));

            tableParameters = new XElement("TableType", new XAttribute("Value", tableType.ToString()));
            tableParameters.Add(tableValues);

            xmlFile.Root.Add(tableParameters);

            SaveXML(xmlFile, fullXmlName);

            return xmlFile.Element(rootName);
        }

        public override async Task<object> OpenXML(string fullXmlName)
        {
            return XDocument.Load(fullXmlName);
        }

        public override void SaveXML(XDocument xmlFile, string fullXmlName)
        {
            xmlFile.Save(fullXmlName);
        }



        public override async Task<object> ReplaceXmlNode(string fullXmlName, object xmlValues, XDocument xmlData, DrillholeTableType tableType, string xmlNodeTableNam, string rootName)
        {
            var tableValues = (XElement)xmlValues;

            XElement tableParameters = null;

            var elements = xmlData.Descendants(rootName).Elements();

            var updateValues = elements.Where(e => e.Attribute("Value").Value == tableType.ToString());


            if (updateValues.Any())
            {
                updateValues.Remove();

            }


            //insert if null

            tableParameters = new XElement("TableType", new XAttribute("Value", tableType.ToString()));
            tableParameters.Add(tableValues);

            xmlData.Root.Add(tableParameters);

            //change modified time
            var modified = xmlData.Descendants(rootName).Select(m => m.Attribute("Modified")).FirstOrDefault();
            if (modified != null)
                modified.Value = DateTime.Now.ToString();

            SaveXML(xmlData, fullXmlName);

            return xmlData.Element(rootName);
        }

        public override Task<XElement> UpdateXmlNodes(string fullXmlName, string xmlName, object xmlChange, XDocument xmlData, DrillholeTableType tableType, string rootName)
        {
            //TODO
            throw new NotImplementedException();
        }

        public override async void UpdateProjectFile(string projectFile, string drillholeFile, string drillholeRoot, DrillholeTableType tableType)
        {
            XDocument xmlFile = await OpenXML(projectFile) as XDocument;
            var elements = xmlFile.Descendants(drillholeRoot).Elements();

            //var check = elements.Select(e => e.Element(DrillholeConstants.drillholeData).Value).SingleOrDefault();

            //if (check == "")  //saved session at dialog page
            //{
                var updateValues = elements.Select(e => e.Element(DrillholeConstants.drillholeData)).Select(f => f.Element(tableType.ToString())).FirstOrDefault();

                updateValues.Value = drillholeFile;

            //}

            SaveXML(xmlFile, projectFile);
        }

        public override async Task<object> ReturnValuesFromXML(string projectFile, string drillholeFile, string drillholeProjectRoot, string drillholeRoot, DrillholeTableType tableType)
        {
            XDocument xmlFile = await OpenXML(projectFile) as XDocument;
            var elements = xmlFile.Descendants(drillholeProjectRoot).Elements();

            var drillholeFieldValues = elements.Select(e => e.Element(drillholeRoot)).Nodes().ToList(); //return all the tables and check below for the table type to then open the table fields table.

            string drillholeData = "";


            foreach (XElement drillholeValue in drillholeFieldValues)
            {
                if (drillholeValue.Name == tableType.ToString())
                {
                    drillholeData = drillholeValue.Value;
                    break;
                }
            }


            if (drillholeData == "")
                return null;

            XDocument tableProperties = await OpenXML(drillholeData) as XDocument;

            var tableElements = tableProperties.Descendants("TableType").Elements().FirstOrDefault();



            return tableElements;
        }
    }

    public class XmlTableFields : XmlManagement
    {
        public override async Task<XElement> CreateXML(string fullXmlName, object xmlValues, DrillholeTableType tableType, string rootName)
        {
            var tableFields = (ImportTableFields)xmlValues;

            XElement tableParameters = null;
            XElement fieldName = null;

            XDocument xmlFile = new XDocument(
                new XDeclaration("1.0", null, null),
                new XProcessingInstruction("order", "alpha ascending"),
                new XElement(rootName, new XAttribute("Modified", DateTime.Now)));

            tableParameters = new XElement("TableType", new XAttribute("Value", tableType.ToString()));

            foreach (var field in tableFields)
            {
                List<XElement> nodes = new List<XElement>();
                //   nodes.Add(new XElement("ColumnHeader", field.columnHeader));
                nodes.Add(new XElement("ColumnImportAs", field.columnImportAs));
                nodes.Add(new XElement("ColumnImportName", field.columnImportName));
                nodes.Add(new XElement("FieldType", field.fieldType));
                nodes.Add(new XElement("GenericType", field.genericType));
                nodes.Add(new XElement("GroupName", field.groupName));
                nodes.Add(new XElement("FieldKeys", field.keys)); //?

                fieldName = new XElement("ColumnHeader", new XAttribute("Name", field.columnHeader));
                fieldName.Add(nodes);

                tableParameters.Add(fieldName);

            }

            xmlFile.Root.Add(tableParameters);

            SaveXML(xmlFile, fullXmlName);

            return xmlFile.Element(rootName);
        }

        public override async Task<object> OpenXML(string fullXmlName)
        {
            return XDocument.Load(fullXmlName);
        }

        public override async void SaveXML(XDocument xmlFile, string fullXmlName)
        {
            xmlFile.Save(fullXmlName);
        }

        public override async Task<object> ReplaceXmlNode(string fullXmlName, object xmlValues, XDocument xmlData, DrillholeTableType tableType, string xmlNodeTableNam, string rootName)
        {
            var tableFields = (ImportTableFields)xmlValues;

            XElement tableParameters = null;
            XElement fieldName = null;

            tableParameters = new XElement("TableType", new XAttribute("Value", tableType.ToString()));

            var elements = xmlData.Descendants(rootName).Elements();

            foreach (var field in tableFields)
            {
                var updateValues = elements.Where(e => e.Attribute("Value").Value == tableType.ToString());

                List<XElement> nodes = new List<XElement>();
                nodes.Add(new XElement("ColumnImportAs", field.columnImportAs));
                nodes.Add(new XElement("ColumnImportName", field.columnImportName));
                nodes.Add(new XElement("FieldType", field.fieldType));
                nodes.Add(new XElement("GenericType", field.genericType));
                nodes.Add(new XElement("GroupName", field.groupName));
                nodes.Add(new XElement("FieldType", field.keys)); //?

                fieldName = new XElement("ColumnHeader", new XAttribute("Name", field.columnHeader));
                fieldName.Add(nodes);

                if (updateValues.Any())
                    updateValues.Remove();

                tableParameters.Add(fieldName);

            }

            xmlData.Root.Add(tableParameters);

            //change modified time
            var modified = xmlData.Descendants(rootName).Select(m => m.Attribute("Modified")).FirstOrDefault();
            if (modified != null)
                modified.Value = DateTime.Now.ToString();

            SaveXML(xmlData, fullXmlName);

            return xmlData.Element(rootName);
        }

        public override async Task<XElement> UpdateXmlNodes(string fullXmlName, string xmlName, object xmlChange, XDocument xmlData, DrillholeTableType tableType, string rootName)
        {
            throw new NotImplementedException();

        }

        public override async void UpdateProjectFile(string projectFile, string drillholeFile, string drillholeProjectRoot, DrillholeTableType tableType)
        {
            XDocument xmlFile = await OpenXML(projectFile) as XDocument;
            var elements = xmlFile.Descendants(drillholeProjectRoot).Elements();

            var updateValues = elements.Select(e => e.Element(DrillholeConstants.drillholeFields)).Select(f => f.Element(tableType.ToString())).FirstOrDefault();

            updateValues.Value = drillholeFile;

            SaveXML(xmlFile, projectFile);
        }

        public override async Task<object> ReturnValuesFromXML(string projectFile, string drillholeFile, string drillholeProjectRoot, string drillholeRoot, DrillholeTableType tableType)
        {
            XDocument xmlFile = await OpenXML(projectFile) as XDocument;
            var elements = xmlFile.Descendants(drillholeProjectRoot).Elements();

            ImportTableFields fields = new ImportTableFields();

            var drillholeFieldValues = elements.Select(e => e.Element(drillholeRoot)).Nodes().ToList(); //return all the tables and check below for the table type to then open the table fields table.

            string drillholeFields = "";

            
            foreach(XElement drillholeValue in drillholeFieldValues)
            {
                if (drillholeValue.Name == tableType.ToString())
                {
                    drillholeFields = drillholeValue.Value;

                    
                    break;
                }
            }

            if (drillholeFields == "")
                return null;

            XDocument tableProperties = await OpenXML(drillholeFields) as XDocument;

            var tableElements = tableProperties.Descendants(drillholeRoot).Elements().Nodes();

            ImportTableField field = null;
            foreach(XElement query in tableElements)
            {

                field = new ImportTableField();

                field.columnHeader = query.FirstAttribute.Value;

                var columnImportAs = query.FirstNode as XElement;
                field.columnImportAs = columnImportAs.Value;

                var columnImportName = columnImportAs.NextNode as XElement;
                field.columnImportName = columnImportName.Value;

                var fieldType = columnImportName.NextNode as XElement;
                field.fieldType = fieldType.Value;

                var genericType = fieldType.NextNode as XElement;
                field.genericType = Convert.ToBoolean(genericType.Value);

                var groupName = genericType.NextNode as XElement;
                field.groupName = groupName.Value;

                var fieldKeys = groupName.NextNode as XElement;

                var keystring = fieldKeys.Value.ToString();

                if (keystring.Contains("True"))
                {
                    string leftKey = "";
                    string rightKey = "";

                    if (keystring.Substring(1, 1) == "T")
                    {
                        leftKey = keystring.Substring(1, 4);
                        rightKey = keystring.Substring(7, 5);
                    }
                    else if (keystring.Substring(8, 1) == "T")
                    {
                        leftKey = keystring.Substring(1, 5);
                        rightKey = keystring.Substring(8, 4);
                    }
                    else
                    {
                        leftKey = keystring.Substring(1, 5);
                        rightKey = keystring.Substring(8, 5);
                    }

                    bool bLeft = Convert.ToBoolean(leftKey);
                    bool bRight = Convert.ToBoolean(rightKey);

                    field.keys = new KeyValuePair<bool, bool>(bLeft, bRight);
                }
                else
                {
                    //default
                    field.keys = new KeyValuePair<bool, bool>(false, false);
                }

                fields.Add(field);
            }

            return fields;
        }
    }

    public class XmlDrillholePreferences : XmlManagement
    {
        public override async Task<XElement> CreateXML(string fullXmlName, object xmlValues, DrillholeTableType tableType, string rootName)
        {
            var preferences = (DrillholePreferences)xmlValues;

            XElement tableParameters = null;

            XDocument xmlFile = new XDocument(
                new XDeclaration("1.0", null, null),
                new XProcessingInstruction("order", "alpha ascending"),
                new XElement(rootName, new XAttribute("Modified", DateTime.Now)));

            tableParameters = new XElement("Preferences", new XAttribute("Value", "exists"));

            List<XElement> nodes = new List<XElement>();
            nodes.Add(new XElement("NegativeDip", preferences.NegativeDip));
            nodes.Add(new XElement("ImportAllColumns", preferences.ImportAllColumns));
            nodes.Add(new XElement("IgnoreInvalidValues", preferences.IgnoreInvalidValues));
            nodes.Add(new XElement("LowerDetection", preferences.LowerDetection));
            nodes.Add(new XElement("SurveyType", preferences.surveyType.ToString()));
            nodes.Add(new XElement("DipTolerance", preferences.DipTolerance.ToString())); //?
            nodes.Add(new XElement("AziTolerance", preferences.AzimuthTolerance.ToString())); //?
            nodes.Add(new XElement("DefaultValue", preferences.DefaultValue.ToString())); //?
            nodes.Add(new XElement("ImportAllColumns", preferences.ImportAllColumns));
            nodes.Add(new XElement("ImportSurveyOnly", preferences.ImportSurveyOnly));
            nodes.Add(new XElement("ImportAssayOnly", preferences.ImportAssayOnly));
            nodes.Add(new XElement("ImportGeologyOnly", preferences.ImportGeologyOnly));
            nodes.Add(new XElement("ImportContinuousOnly", preferences.ImportContinuousOnly));
            nodes.Add(new XElement("NullifyZeroAssays", preferences.NullifyZeroAssays));
            nodes.Add(new XElement("GeologyBase", preferences.GeologyBase));
            nodes.Add(new XElement("CreateCollar", preferences.CreateCollar));
            nodes.Add(new XElement("CreateToe", preferences.CreateToe));
            nodes.Add(new XElement("TopCore", preferences.TopCore));
            nodes.Add(new XElement("BottomCore", preferences.BottomCore));
            nodes.Add(new XElement("CalculateStructures", preferences.CalculateStructures));
            nodes.Add(new XElement("DesurveyMethod", preferences.DesurveyMethod));

            

            tableParameters.Add(nodes);

            xmlFile.Root.Add(tableParameters);

            SaveXML(xmlFile, fullXmlName);

            return xmlFile.Element(rootName);
        }

        public override async Task<object> OpenXML(string fullXmlName)
        {
            return XDocument.Load(fullXmlName);
        }

        public override async Task<object> ReplaceXmlNode(string fullXmlName, object xmlValues, XDocument xmlData, DrillholeTableType tableType, string xmlNodeTableNam, string rootName)
        {
            var preferences = (DrillholePreferences)xmlValues;

            XElement tableParameters = null;

            tableParameters = new XElement("Preferences", new XAttribute("Value", "exists"));

            var elements = xmlData.Descendants(rootName).Elements();

            var updateValues = elements.Where(e => e.Attribute("Value").Value == "exists");

            List<XElement> nodes = new List<XElement>();
            nodes.Add(new XElement("NegativeDip", preferences.NegativeDip));
            nodes.Add(new XElement("ImportAllColumns", preferences.ImportAllColumns));
            nodes.Add(new XElement("IgnoreInvalidValues", preferences.IgnoreInvalidValues));
            nodes.Add(new XElement("LowerDetection", preferences.LowerDetection));
            nodes.Add(new XElement("SurveyType", preferences.surveyType.ToString()));
            nodes.Add(new XElement("DipTolerance", preferences.DipTolerance.ToString())); //?
            nodes.Add(new XElement("AziTolerance", preferences.AzimuthTolerance.ToString())); //?
            nodes.Add(new XElement("DefaultValue", preferences.DefaultValue.ToString())); //?
            nodes.Add(new XElement("ImportAllColumns", preferences.ImportAllColumns));
            nodes.Add(new XElement("ImportSurveyOnly", preferences.ImportSurveyOnly));
            nodes.Add(new XElement("ImportAssayOnly", preferences.ImportAssayOnly));
            nodes.Add(new XElement("ImportGeologyOnly", preferences.ImportGeologyOnly));
            nodes.Add(new XElement("ImportContinuousOnly", preferences.ImportContinuousOnly));
            nodes.Add(new XElement("NullifyZeroAssays", preferences.NullifyZeroAssays));
            nodes.Add(new XElement("GeologyBase", preferences.GeologyBase));
            nodes.Add(new XElement("CreateCollar", preferences.CreateCollar));
            nodes.Add(new XElement("CreateToe", preferences.CreateToe));
            nodes.Add(new XElement("TopCore", preferences.TopCore));
            nodes.Add(new XElement("BottomCore", preferences.BottomCore));
            nodes.Add(new XElement("CalculateStructures", preferences.CalculateStructures));
            nodes.Add(new XElement("DesurveyMethod", preferences.DesurveyMethod));


            if (updateValues.Any())
                updateValues.Remove();

            tableParameters.Add(nodes);

            xmlData.Root.Add(tableParameters);

            //change modified time
            var modified = xmlData.Descendants(rootName).Select(m => m.Attribute("Modified")).FirstOrDefault();
            if (modified != null)
                modified.Value = DateTime.Now.ToString();

            SaveXML(xmlData, fullXmlName);

            return xmlData.Element(rootName);
        }

        public override async Task<object> ReturnValuesFromXML(string projectFile, string drillholeFile, string drillholeRoot, string drillholeProjectRoot, DrillholeTableType tableType)
        {
            XDocument xmlFile = await OpenXML(drillholeFile) as XDocument;

            var elements = xmlFile.Descendants(drillholeProjectRoot).Elements().ToList();

            var importColumns = elements.Select(n => n.Element("ImportAllColumns").Value).SingleOrDefault();
            var surveyType = elements.Select(n => n.Element("SurveyType").Value).SingleOrDefault();
            var surveyMethod = elements.Select(n => n.Element("DesurveyMethod").Value).SingleOrDefault();

            bool bImport = false;

            if (importColumns != null)
            {
                if (importColumns.ToString() != "")
                    bImport = Convert.ToBoolean(importColumns);

            }

            DrillholeSurveyType survType = DrillholeSurveyType.downholesurvey; //default

            if (surveyType == DrillholeSurveyType.vertical.ToString())
            {
                survType = DrillholeSurveyType.vertical;
            }
            else if (surveyType == DrillholeSurveyType.collarsurvey.ToString())
            {
                survType = DrillholeSurveyType.collarsurvey;
            }

            DrillholeDesurveyEnum desurveyMethod = DrillholeDesurveyEnum.AverageAngle;

            if (surveyMethod == DrillholeDesurveyEnum.BalancedTangential.ToString())
                desurveyMethod = DrillholeDesurveyEnum.BalancedTangential;
            else if (surveyMethod == DrillholeDesurveyEnum.MinimumCurvature.ToString())
                desurveyMethod = DrillholeDesurveyEnum.MinimumCurvature;
            else if (surveyMethod == DrillholeDesurveyEnum.RadiusCurvature.ToString())
                desurveyMethod = DrillholeDesurveyEnum.RadiusCurvature;
            else if (surveyMethod == DrillholeDesurveyEnum.Tangential.ToString())
                desurveyMethod = DrillholeDesurveyEnum.Tangential;

            DrillholePreferences readPreferences = new DrillholePreferences()
            {
                surveyType = survType,
                ImportAllColumns = bImport,
                DesurveyMethod = desurveyMethod
            };


            List<object> myObjects = new List<object>();
            myObjects.Add(readPreferences);

            return myObjects;
        }

        public override void SaveXML(XDocument xmlFile, string fullXmlName)
        {
            xmlFile.Save(fullXmlName);
        }

        public override async void UpdateProjectFile(string projectFile, string drillholePreferencesFile, string drillholeProjectRoot, DrillholeTableType tableType)
        {
            XDocument xmlFile = await OpenXML(projectFile) as XDocument;
            var elements = xmlFile.Descendants(drillholeProjectRoot).Elements();

            var check = elements.Select(e => e.Element(DrillholeConstants.drillholePref).Value).SingleOrDefault();

            if (check == "")  //saved session at dialog page
            {
                var updateValues = elements.Select(e => e.Element(DrillholeConstants.drillholePref)).SingleOrDefault();
                updateValues.Value = drillholePreferencesFile;
            }

            SaveXML(xmlFile, projectFile);
        }

        public override async Task<XElement> UpdateXmlNodes(string fullXmlName, string xmlName, object xmlChange, XDocument xmlData, DrillholeTableType tableType, string rootName)
        {
            string strValue = "";
            bool bValue = true;

            var elements = xmlData.Descendants(rootName).Elements();
            var updateValues = elements.Select(e => e.Element(xmlName)).SingleOrDefault();

            if (Information.IsNumeric(xmlChange))
            {
                updateValues.Value = xmlChange.ToString();
            }
            else
            {
                updateValues.Value = xmlChange.ToString();
            }

            //change modified time
            var modified = xmlData.Descendants(rootName).Select(m => m.Attribute("Modified")).FirstOrDefault();
            if (modified != null)
                modified.Value = DateTime.Now.ToString();

            SaveXML(xmlData, fullXmlName);

            return xmlData.Element(rootName);
        }

    }

    public class XmlSavedSession : XmlManagement
    {
        public override async Task<XElement> CreateXML(string fullXmlName, object xmlValues, DrillholeTableType tableType, string rootName)
        {
            //xmlValues will be project name
            var projectProp = (DrillholeProjectProperties)xmlValues;

            XDocument xmlFile = new XDocument(
            new XDeclaration("1.0", null, null),
            new XProcessingInstruction("order", "alpha ascending"),
            new XElement(rootName, new XAttribute("Modified", DateTime.Now)));

            List<XElement> tableTypeNodes = new List<XElement>();

            tableTypeNodes.Add(new XElement(DrillholeTableType.collar.ToString(), ""));
            tableTypeNodes.Add(new XElement(DrillholeTableType.survey.ToString(), ""));
            tableTypeNodes.Add(new XElement(DrillholeTableType.assay.ToString(), ""));
            tableTypeNodes.Add(new XElement(DrillholeTableType.interval.ToString(), ""));
            tableTypeNodes.Add(new XElement(DrillholeTableType.continuous.ToString(), ""));

            XElement tableParameters = null;
            tableParameters = new XElement("Project", new XAttribute("Name", projectProp.ProjectName));

            List<XElement> nodes = new List<XElement>();
            nodes.Add(new XElement("ProjectParent", projectProp.ProjectParentFolder));
            nodes.Add(new XElement("ProjectFolder", projectProp.ProjectFolder));
            nodes.Add(new XElement("ProjectFile", projectProp.ProjectFile));
            nodes.Add(new XElement(DrillholeConstants.drillholeData, projectProp.DrillholeData));
            nodes.Add(new XElement(DrillholeConstants.drillholeFields, projectProp.DrillholeFields));
            nodes.Add(new XElement(DrillholeConstants.drillholePref, projectProp.DrillholePreferences));
            nodes.Add(new XElement(DrillholeConstants.drillholeTable, projectProp.DrillholeTables));
            nodes.Add(new XElement(DrillholeConstants.drillholeDesurv, projectProp.DrillholeDesurvey));


            var tableDataNode = nodes.Select(a => a.Element(DrillholeConstants.drillholeData)).FirstOrDefault();
            nodes[3].Add(tableTypeNodes);
            nodes[4].Add(tableTypeNodes);



            tableParameters.Add(nodes);


            xmlFile.Root.Add(tableParameters);

            SaveXML(xmlFile, fullXmlName);

            return xmlFile.Element(rootName);

        }

        public override async Task<object> OpenXML(string fullXmlName)
        {
               return XDocument.Load(fullXmlName);
        }

     

        public override Task<object> ReplaceXmlNode(string fullXmlName, object xmlValues, XDocument xmlData, DrillholeTableType tableType, string xmlNodeTableNam, string rootName)
        {
            throw new NotImplementedException();
        }

        public override Task<object> ReturnValuesFromXML(string projectFile, string drillholeFile, string drillholeRoot, string drillholeProjectRoot, DrillholeTableType tableType)
        {
            throw new NotImplementedException();
        }

        public override void SaveXML(XDocument xmlFile, string fullXmlName)
        {
            xmlFile.Save(fullXmlName);
        }

        public override void UpdateProjectFile(string projectFile, string drillholeFile, string drillholeRoot, DrillholeTableType tableType)
        {
            throw new NotImplementedException();
        }

        public override Task<XElement> UpdateXmlNodes(string fullXmlName, string xmlName, object xmlChange, XDocument xmlData, DrillholeTableType tableType, string rootName)
        {
            throw new NotImplementedException();
        }
    }


}

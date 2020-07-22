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
                nodes.Add(new XElement("FieldType", field.keys)); //?

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

        public override void SaveXML(XDocument xmlFile, string fullXmlName)
        {
            xmlFile.Save(fullXmlName);
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

        public override void SaveXML(XDocument xmlFile, string fullXmlName)
        {
            xmlFile.Save(fullXmlName);
        }

        public override Task<XElement> UpdateXmlNodes(string fullXmlName, string xmlName, object xmlChange, XDocument xmlData, DrillholeTableType tableType, string rootName)
        {
            throw new NotImplementedException();
        }
    }


}

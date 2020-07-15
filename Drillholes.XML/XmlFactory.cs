﻿using Drillholes.Domain;
using Drillholes.Domain.DataObject;
using Drillholes.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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
            _xml =  XmlManagement.xmlType(xmlMode);
        }

        public async Task<XElement> CreateXML(string fullXmlName, object xmlValues, DrillholeTableType tableType, string rootName)
        {
            return _xml.CreateXML(fullXmlName, xmlValues, tableType, rootName).Result;
        }


        public async Task<XDocument> OpenXML(string fullXmlName)
        {
            return _xml.OpenXML(fullXmlName).Result;
        }
        public async Task<XElement> UpdateXML(string fullXmlName, object xmlValues, XDocument xmlData, DrillholeTableType tableType, string xmlNodeTableNam, string rootName)
        {
            return _xml.UpdateXML(fullXmlName, xmlValues, xmlData, tableType, xmlNodeTableNam, rootName).Result;
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
                    return null;
            }

            return null;
        }

        public abstract Task<XElement> CreateXML(string fullXmlName, object xmlValues, DrillholeTableType tableType, string rootName);
        public abstract Task<XDocument> OpenXML(string fullXmlName);
        public abstract void SaveXML(XDocument xmlFile, string fullXmlName);
        public abstract Task<XElement> UpdateXML(string fullXmlName, object xmlValues, XDocument xmlData, DrillholeTableType tableType, string xmlNodeTableNam, string rootName);

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

        public override async Task<XDocument> OpenXML(string fullXmlName)
        {
            return XDocument.Load(fullXmlName);
        }

        public override async void SaveXML(XDocument xmlFile, string fullXmlName)
        {
            xmlFile.Save(fullXmlName);
        }

        public override async Task<XElement> UpdateXML(string fullXmlName, object xmlValues, XDocument xmlData, DrillholeTableType tableType, string xmlNodeTableNam, string rootName)
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

                if (updateValues != null)
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

        public override async Task<XDocument> OpenXML(string fullXmlName)
        {
            return XDocument.Load(fullXmlName);
        }

        public override void SaveXML(XDocument xmlFile, string fullXmlName)
        {
            xmlFile.Save(fullXmlName);
        }



        public override async Task<XElement> UpdateXML(string fullXmlName, object xmlValues, XDocument xmlData, DrillholeTableType tableType, string xmlNodeTableNam, string rootName)
        {
            var tableValues = (XElement)xmlValues;

            XElement tableParameters = null;

            var elements = xmlData.Descendants(rootName).Elements();

           // var updateValues = elements.Select(e => e.Element(xmlNodeTableNam)).FirstOrDefault();
            var updateValues = elements.Where(e => e.Attribute("Value").Value == tableType.ToString());


            if (updateValues.Any())
            {
               // var updateValues = elements.Select(e => e.Element(xmlNodeTableNam));
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
    }

    public class XmlTableFields : XmlManagement
    {
        public override async Task<XElement> CreateXML(string fullXmlName, object xmlValues, DrillholeTableType tableType, string rootName)
        {
            var tableFields = (ImportTableFields)xmlValues;

            XElement tableParameters = null;

            XDocument xmlFile = new XDocument(
                new XDeclaration("1.0", null, null),
                new XProcessingInstruction("order", "alpha ascending"),
                new XElement(rootName, new XAttribute("Modified", DateTime.Now)));

            foreach (var field in tableFields)
            {
                List<XElement> nodes = new List<XElement>();
                nodes.Add(new XElement("ColumnHeader", field.columnHeader));
                nodes.Add(new XElement("ColumnImportAs", field.columnImportAs));
                nodes.Add(new XElement("ColumnImportName", field.columnImportName));
                nodes.Add(new XElement("FieldType", field.fieldType));
                nodes.Add(new XElement("GenericType", field.genericType));
                nodes.Add(new XElement("GroupName", field.groupName));
                nodes.Add(new XElement("FieldType", field.keys)); //?

                tableParameters = new XElement("TableType", new XAttribute("Value", tableType.ToString()));
                tableParameters.Add(nodes);

            }

            xmlFile.Root.Add(tableParameters);

            SaveXML(xmlFile, fullXmlName);

            return xmlFile.Element(rootName);
        }

        public override async Task<XDocument> OpenXML(string fullXmlName)
        {
            return XDocument.Load(fullXmlName);
        }

        public override async void SaveXML(XDocument xmlFile, string fullXmlName)
        {
            xmlFile.Save(fullXmlName);
        }

        public override async Task<XElement> UpdateXML(string fullXmlName, object xmlValues, XDocument xmlData, DrillholeTableType tableType, string xmlNodeTableNam, string rootName)
        {
            var tableValues = (ImportTableFields)xmlValues;

            XElement tableParameters = null;

            var elements = xmlData.Descendants(rootName).Elements();

            foreach (var field in tableValues)
            {
                var updateValues = elements.Where(e => e.Attribute("Value").Value == tableType.ToString());

                List<XElement> nodes = new List<XElement>();
                nodes.Add(new XElement("ColumnHeader", field.columnHeader));
                nodes.Add(new XElement("ColumnImportAs", field.columnImportAs));
                nodes.Add(new XElement("ColumnImportName", field.columnImportName));
                nodes.Add(new XElement("FieldType", field.fieldType));
                nodes.Add(new XElement("GenericType", field.genericType));
                nodes.Add(new XElement("GroupName", field.groupName));
                nodes.Add(new XElement("FieldType", field.keys)); //?

                if (updateValues != null)
                    updateValues.Remove();

                tableParameters = new XElement("TableType", new XAttribute("Value", tableType.ToString()));
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
    }
}

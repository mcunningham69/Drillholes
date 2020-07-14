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

        public async Task<XElement> CreateXML(string fullXmlName, object xmlValues, DrillholeTableType tableType)
        {
            return _xml.CreateXML(fullXmlName, xmlValues, tableType).Result;
        }

        public async Task<XElement> CreateXML(string fullXmlName, object xmlValues)
        {
            return _xml.CreateXML(fullXmlName, xmlValues).Result;
        }

        public async Task<XElement> OpenXML(string fullXmlName)
        {
            return _xml.OpenXML(fullXmlName).Result;
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
                    return null;
                case DrillholesXmlEnum.DrillholeInputData:
                    return new XmlTableInputdata();
                case DrillholesXmlEnum.DrillholeDesurveyData:
                    return null;
                case DrillholesXmlEnum.DrillholePreferences:
                    return null;
            }

            return null;
        }

        public abstract Task<XElement> CreateXML(string fullXmlName, object xmlValues);
        public abstract Task<XElement> CreateXML(string fullXmlName, object xmlValues, DrillholeTableType tableType);
        public abstract Task<XElement> OpenXML(string fullXmlName);
        public abstract void SaveXML(XDocument xmlFile, string fullXmlName);
        public abstract Task<XElement> UpdateXML(string fullXmlName, object xmlValues);

    }

    public class XmlTableParameters : XmlManagement
    {
        public override Task<XElement> CreateXML(string fullXmlName, object xmlValues, DrillholeTableType tableType)
        {
            throw new NotImplementedException();
        }
        public override async Task<XElement> CreateXML(string fullXmlName, object xmlValues)
        {
            string rootName = "DrillholeTable";

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

        public override async Task<XElement> OpenXML(string fullXmlName)
        {
            return XDocument.Load(fullXmlName).Element("DrillholeTable");
        }

        public override async void SaveXML(XDocument xmlFile, string fullXmlName)
        {
            xmlFile.Save(fullXmlName);
        }

        public override async Task<XElement> UpdateXML(string fullXmlName, object xmlValues)
        {
            throw new NotImplementedException();
        }

       
    }

    public class XmlTableInputdata : XmlManagement
    {
        public override Task<XElement> CreateXML(string fullXmlName, object xmlValues)
        {
            throw new NotImplementedException();
        }

        public override async Task<XElement> CreateXML(string fullXmlName, object xmlValues, DrillholeTableType tableType)
        {
            string rootName = "DrillholeData";

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

        public override async Task<XElement> OpenXML(string fullXmlName)
        {
            return XDocument.Load(fullXmlName).Element("DrillholeData");
        }

        public override void SaveXML(XDocument xmlFile, string fullXmlName)
        {
            xmlFile.Save(fullXmlName);
        }

        public override Task<XElement> UpdateXML(string fullXmlName, object xmlValues)
        {
            throw new NotImplementedException();
        }
    }
}

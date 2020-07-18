using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using Drillholes.Domain.DataObject;
using Drillholes.Domain.Enum;
using Drillholes.Domain.Interfaces;
using Drillholes.Domain;
using System.Net.NetworkInformation;

namespace Drillholes.XML
{
    public class XmlController : IDrillholeXML
    {
        XmlFactory factory = null;

        public async Task<XElement> DrillholeData(string fileName, XElement xPreview, DrillholeTableType tableType, string xmlNodeName, string rootName)
        {
            XDocument xmlFile = null;
            XElement elements = null;

            if (factory == null)
                factory = new XmlFactory(DrillholesXmlEnum.DrillholeInputData);
            else
                factory.SetXmlType(DrillholesXmlEnum.DrillholeInputData);

            if (!File.Exists(fileName))
                elements = await factory.CreateXML(fileName, xPreview, tableType, rootName);
            else
            {
                xmlFile = await factory.OpenXML(fileName);

                if (xmlFile == null)
                    elements = await factory.CreateXML(fileName, xPreview, tableType, rootName);
                else
                    elements = await factory.ReplaceXmlNode(fileName, xPreview, xmlFile, tableType, xmlNodeName, rootName);
            }

            return await factory.CreateXML(fileName, xPreview, tableType, rootName);
        }

        public Task<XElement> DrillholeDesurvey()
        {
            throw new NotImplementedException();
        }

        public async Task<XDocument> DrillholePreferences(string fileName, DrillholePreferences preferences, string rootName)
        {
            XDocument xmlFile = null;
            XElement elements = null;

            if (factory == null)
                factory = new XmlFactory(DrillholesXmlEnum.DrillholePreferences);
            else
                factory.SetXmlType(DrillholesXmlEnum.DrillholePreferences);

            if (!File.Exists(fileName))
                elements = await factory.CreateXML(fileName, preferences, DrillholeTableType.other, rootName);
            else
            {
                xmlFile = await factory.OpenXML(fileName);

                if (xmlFile == null)
                    elements = await factory.CreateXML(fileName, preferences, DrillholeTableType.other, rootName);
                else
                {
                    if (preferences != null)
                        elements = await factory.ReplaceXmlNode(fileName, preferences, xmlFile, DrillholeTableType.other, "", rootName);
                }

            }

            if (elements != null)
                xmlFile = await factory.OpenXML(fileName);

            return xmlFile;
        }

        public async Task<XDocument> DrillholePreferences(string fileName, string xmlName, object xmlValue, string rootName)
        {
            XDocument xmlFile = null;
            XElement elements = null;

            if (factory == null)
                factory = new XmlFactory(DrillholesXmlEnum.DrillholePreferences);
            else
                factory.SetXmlType(DrillholesXmlEnum.DrillholePreferences);

            xmlFile = await factory.OpenXML(fileName);

            if (xmlFile != null)
            {

                elements = await factory.UpdateXmlNode(fileName, xmlName, xmlValue, xmlFile, DrillholeTableType.other, rootName);
            }

            if (elements != null)
                xmlFile = await factory.OpenXML(fileName);

            return xmlFile;

        }

        public async Task<XDocument> DrillholeFieldParameters(string fileName, ImportTableFields fields, DrillholeTableType tableType, string rootName)
        {
            XDocument xmlFile = null;
            XElement elements = null;

            if (factory == null)
                factory = new XmlFactory(DrillholesXmlEnum.DrillholeFields);
            else
                factory.SetXmlType(DrillholesXmlEnum.DrillholeFields);

            if (!File.Exists(fileName))
                elements = await factory.CreateXML(fileName, fields, tableType, rootName);
            else
            {
                xmlFile = await factory.OpenXML(fileName);

                if (xmlFile == null)
                    elements = await factory.CreateXML(fileName, fields, tableType, rootName);
                else
                    elements = await factory.ReplaceXmlNode(fileName, fields, xmlFile, tableType, "", rootName);
            }

            //todo check if XElement null and throw exception
            elements = await factory.CreateXML(fileName, fields, tableType, rootName);

            if (elements != null)
                xmlFile = await factory.OpenXML(fileName);

            return xmlFile;
        }



        public async Task<XElement> TableParameters(string fileName, List<DrillholeTable> importTables, string rootName)
        {
            XDocument xmlFile = null;
            XElement elements = null;

            DrillholeTableType tableType = DrillholeTableType.other;

            if (factory == null)
                factory = new XmlFactory(DrillholesXmlEnum.DrillholeTableParameters);
            else
                factory.SetXmlType(DrillholesXmlEnum.DrillholeTableParameters);

            //  File.Delete(fileName);

            if (!File.Exists(fileName))
                elements = await factory.CreateXML(fileName, importTables, tableType, rootName);
            else
            {
                xmlFile = await factory.OpenXML(fileName);

                if (xmlFile == null)
                    elements = await factory.CreateXML(fileName, importTables, tableType, rootName);
                else
                    elements = await factory.ReplaceXmlNode(fileName, importTables, xmlFile, tableType, "", rootName);
            }

            return elements;
        }

        public async Task<XDocument> DrillholeProjectProperties(DrillholeProjectProperties xmlValues, string rootName)
        {
            XDocument xmlFile = null;
            XElement elements = null;

            if (factory == null)
                factory = new XmlFactory(DrillholesXmlEnum.DrillholeProject);
            else
                factory.SetXmlType(DrillholesXmlEnum.DrillholeProject);

            if (!File.Exists(xmlValues.ProjectFile))
            {
                elements = await factory.CreateXML(xmlValues.ProjectFile, xmlValues, DrillholeTableType.other, rootName);
            }
            else
            {
                xmlFile = await factory.OpenXML(xmlValues.ProjectName);


                if (xmlFile == null)
                {
                    elements = await factory.CreateXML(xmlValues.ProjectFile, xmlValues, DrillholeTableType.other, rootName);
                }
            }

            if (elements != null)
                xmlFile = await factory.OpenXML(xmlValues.ProjectFile);

            return xmlFile;
        }
    }
}

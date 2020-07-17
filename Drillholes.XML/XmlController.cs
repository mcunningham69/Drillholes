﻿using System;
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

        public async Task<bool> DrillholePreferences(string fileName, DrillholePreferences preferences, string rootName)
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
                    elements = await factory.ReplaceXmlNode(fileName, preferences, xmlFile, DrillholeTableType.other, "", rootName);
                }

            }

            if (elements != null)
                return true;
            else
                return false;

        }

        public async Task<bool> DrillholePreferences(string fileName, string xmlName, object xmlValue, string rootName)
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
                return true;
            else
                return false;

        }

        public async Task<bool> DrillholeFieldParameters(string fileName, ImportTableFields fields, DrillholeTableType tableType, string rootName)
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
                return true;
            else
                return false;
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

       
    }
}

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
using System.Windows.Markup;

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
                xmlFile = await factory.OpenXML(fileName) as XDocument;

                if (xmlFile == null)
                    elements = await factory.CreateXML(fileName, xPreview, tableType, rootName);
                else
                    elements = await factory.ReplaceXmlNode(fileName, xPreview, xmlFile, tableType, xmlNodeName, rootName) as XElement;
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
                xmlFile = await factory.OpenXML(fileName) as XDocument;

                if (xmlFile == null)
                    elements = await factory.CreateXML(fileName, preferences, DrillholeTableType.other, rootName);
                else
                {
                    if (preferences != null)
                        elements = await factory.ReplaceXmlNode(fileName, preferences, xmlFile, DrillholeTableType.other, "", rootName) as XElement;
                }

            }

            if (elements != null)
                xmlFile = await factory.OpenXML(fileName) as XDocument;

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

            xmlFile = await factory.OpenXML(fileName) as XDocument;

            if (xmlFile != null)
            {

                elements = await factory.UpdateXmlNode(fileName, xmlName, xmlValue, xmlFile, DrillholeTableType.other, rootName);
            }

            if (elements != null)
                xmlFile = await factory.OpenXML(fileName) as XDocument;

            return xmlFile;

        }

        public async void DrillholePreferences(string projectFile, string drillholePreferencesFile, string drillholeProjectRoot)
        {
            XDocument xmlFile = null;

            if (factory == null)
                factory = new XmlFactory(DrillholesXmlEnum.DrillholePreferences);
            else
                factory.SetXmlType(DrillholesXmlEnum.DrillholePreferences);

            if (File.Exists(projectFile))
            {
                xmlFile = await factory.OpenXML(projectFile) as XDocument;
                var elements = xmlFile.Descendants(drillholeProjectRoot).Elements();

                var check = elements.Select(e => e.Element(DrillholeConstants.drillholePref).Value).SingleOrDefault();

                if (check == "")  //saved session at dialog page
                {
                    var updateValues = elements.Select(e => e.Element(DrillholeConstants.drillholePref)).SingleOrDefault();
                    updateValues.Value = drillholePreferencesFile;
                }

                xmlFile.Save(projectFile);

            }

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
                xmlFile = await factory.OpenXML(fileName) as XDocument;

                if (xmlFile == null)
                    elements = await factory.CreateXML(fileName, fields, tableType, rootName);
                else
                    elements = await factory.ReplaceXmlNode(fileName, fields, xmlFile, tableType, "", rootName) as XElement;
            }

            //todo check if XElement null and throw exception
            elements = await factory.CreateXML(fileName, fields, tableType, rootName);

            if (elements != null)
                xmlFile = await factory.OpenXML(fileName) as XDocument;

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
                xmlFile = await factory.OpenXML(fileName) as XDocument;

                if (xmlFile == null)
                    elements = await factory.CreateXML(fileName, importTables, tableType, rootName);
                else
                    elements = await factory.ReplaceXmlNode(fileName, importTables, xmlFile, tableType, "", rootName) as XElement;
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
                xmlFile = await factory.OpenXML(xmlValues.ProjectName) as XDocument;


                if (xmlFile == null)
                {
                    elements = await factory.CreateXML(xmlValues.ProjectFile, xmlValues, DrillholeTableType.other, rootName);
                }
            }

            if (elements != null)
                // xmlFile = await factory.OpenXML(xmlValues.ProjectFile) as XDocument;
                return XDocument.Load(xmlValues.ProjectFile);

            return xmlFile;
        }

        public async Task<object> DrillholeProjectProperties(string projectFile, string drillholeTableFile, string drillholeProjectRoot, string drillholeRootname)
        {
            XDocument xmlFile = null;
            List<DrillholeTable> tables = new List<DrillholeTable>();

            if (factory == null)
                factory = new XmlFactory(DrillholesXmlEnum.DrillholeProject);
            else
                factory.SetXmlType(DrillholesXmlEnum.DrillholeProject);

            if (File.Exists(projectFile))
            {
                xmlFile = await factory.OpenXML(projectFile) as XDocument;
                var elements = xmlFile.Descendants(drillholeProjectRoot).Elements();

                string[] values = new string[3];

                values[0] = elements.Select(e => e.Element(DrillholeConstants.drillholeTable).Value).SingleOrDefault(); //check for drillhole table
                values[1] = elements.Select(e => e.Element(DrillholeConstants.drillholeFields).Value).SingleOrDefault(); //check for drillhole fields
                values[2] = elements.Select(e => e.Element(DrillholeConstants.drillholeData).Value).SingleOrDefault(); //check for drillhole table

                //TODO





                //var check = elements.Select(e => e.Element(DrillholeConstants.drillholeTable).Value).SingleOrDefault();

                if (values[1] != "" && values[2] != "")  //saved session at dialog page
                {
                    var updateValues = elements.Select(e => e.Element(DrillholeConstants.drillholeTable)).SingleOrDefault();
                    updateValues.Value = drillholeTableFile;

                    xmlFile.Save(projectFile);
                }
                else if (values[0] != "") //saved session where tables already selected for import
                {
                    factory.SetXmlType(DrillholesXmlEnum.DrillholeTableParameters);
                    XDocument drillholeTableParameters = await factory.OpenXML(values[0]) as XDocument;

                    var tableElements = drillholeTableParameters.Descendants(drillholeRootname).Elements();

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
                }
                else  //saved session at dialog page
                {
                    var updateValues = elements.Select(e => e.Element(DrillholeConstants.drillholeTable)).SingleOrDefault();
                    updateValues.Value = drillholeTableFile;

                    xmlFile.Save(projectFile);
                }
            }
            else
                return null;


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
}

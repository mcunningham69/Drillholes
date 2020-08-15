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
using System.Net;

namespace Drillholes.XML
{
    public class XmlController : IDrillholeXML
    {
        XmlFactory factory = null;

        #region Data
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

        public async void DrillholeData(string projectFile, string drillholePreferencesFile, string drillholeProjectRoot, DrillholeTableType tableType)
        {
            if (factory == null)
                factory = new XmlFactory(DrillholesXmlEnum.DrillholeInputData);
            else
                factory.SetXmlType(DrillholesXmlEnum.DrillholeInputData);

            if (File.Exists(projectFile))
            {
                factory.UpdateProjectFile(projectFile, drillholePreferencesFile, drillholeProjectRoot, tableType);

            }

        }

        public async Task<XElement> DrillholeData(string projectFile, string drillholeTableFile, string drillholeProjectRoot, string drillholeRootname, DrillholeTableType tableType)
        {
            XElement xPreview = null;

            if (factory == null)
                factory = new XmlFactory(DrillholesXmlEnum.DrillholeInputData);
            else
                factory.SetXmlType(DrillholesXmlEnum.DrillholeInputData);

            if (File.Exists(projectFile))
            {
                var query = await factory.ReturnValuesFromXML(projectFile, drillholeTableFile, drillholeProjectRoot, drillholeRootname, tableType);

                if (query != null)
                    xPreview = query as XElement;
            }

            return xPreview;
        }
        #endregion

        public async Task<XElement> DrillholeDesurvey(string fileName, object desurveyObject, DrillholeTableType tableType, string xmlNodeName, string rootName)
        {
            XDocument xmlFile = null;
            XElement elements = null;

            if (factory == null)
                factory = new XmlFactory(DrillholesXmlEnum.DrillholeDesurveyData);
            else
                factory.SetXmlType(DrillholesXmlEnum.DrillholeDesurveyData);

            if (!File.Exists(fileName))
                elements = await factory.CreateXML(fileName, desurveyObject, tableType, rootName);
            else
            {
                xmlFile = await factory.OpenXML(fileName) as XDocument;

                if (xmlFile == null)
                    elements = await factory.CreateXML(fileName, desurveyObject, tableType, rootName);
                else
                    elements = await factory.ReplaceXmlNode(fileName, desurveyObject, xmlFile, tableType, xmlNodeName, rootName) as XElement;
            }

            return elements;
        }

        public async void DrillholeDesurvey(string projectFile, string drillholePreferencesFile, string drillholeProjectRoot, DrillholeTableType tableType)
        {
            if (factory == null)
                factory = new XmlFactory(DrillholesXmlEnum.DrillholeDesurveyData);
            else
                factory.SetXmlType(DrillholesXmlEnum.DrillholeDesurveyData);

            if (File.Exists(projectFile))
            {
                factory.UpdateProjectFile(projectFile, drillholePreferencesFile, drillholeProjectRoot, tableType);

            }

        }

        public async Task<XElement> DrillholeDesurvey(string projectFile, string drillholeTableFile, string drillholeProjectRoot, string drillholeRootname, DrillholeTableType tableType)
        {
            XElement xPreview = null;

            if (factory == null)
                factory = new XmlFactory(DrillholesXmlEnum.DrillholeDesurveyData);
            else
                factory.SetXmlType(DrillholesXmlEnum.DrillholeDesurveyData);

            if (File.Exists(projectFile))
            {
                var query = await factory.ReturnValuesFromXML(projectFile, drillholeTableFile, drillholeProjectRoot, drillholeRootname, tableType);

                if (query != null)
                    xPreview = query as XElement;
            }

            return xPreview;
        }

        #region Preferences
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

        public async void DrillholePreferences(string projectFile, string drillholePreferencesFile, string drillholeProjectRoot, DrillholeTableType tableType)
        {
            if (factory == null)
                factory = new XmlFactory(DrillholesXmlEnum.DrillholePreferences);
            else
                factory.SetXmlType(DrillholesXmlEnum.DrillholePreferences);

            if (File.Exists(projectFile))
            {
                factory.UpdateProjectFile(projectFile, drillholePreferencesFile, drillholeProjectRoot, tableType);

            }

        }

        public async Task<DrillholePreferences> DrillholePreferences(string drillholeTableFile, string drillholeRootname)
        {
            DrillholePreferences readPreferences = new DrillholePreferences();

            if (factory == null)
                factory = new XmlFactory(DrillholesXmlEnum.DrillholePreferences);
            else
                factory.SetXmlType(DrillholesXmlEnum.DrillholePreferences);

            if (File.Exists(drillholeTableFile))
            {
                object query = await factory.ReturnValuesFromXML("", drillholeTableFile, "", drillholeRootname, DrillholeTableType.other);

                var type = query.GetType();

                List<object> queryObj = query as List<object>;

                readPreferences = queryObj[0] as DrillholePreferences;

                
            }

            return readPreferences;
        }


        #endregion

        #region Fields
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
            if (elements == null)
                elements = await factory.CreateXML(fileName, fields, tableType, rootName);
            else
                xmlFile = await factory.OpenXML(fileName) as XDocument;

            return xmlFile;
        }


        //CHANGE TO OBJECT AND DO SWITCH FOR TABLETYPE
        public async Task<ImportTableFields> DrillholeFieldParameters(string projectFile, string drillholeTableFile, string drillholeProjectRoot, string drillholeRootname, DrillholeTableType tableType)
        {
            ImportTableFields fields = new ImportTableFields();

            if (factory == null)
                factory = new XmlFactory(DrillholesXmlEnum.DrillholeFields);
            else
                factory.SetXmlType(DrillholesXmlEnum.DrillholeFields);

            if (File.Exists(projectFile))
            {
                var query = await factory.ReturnValuesFromXML(projectFile, drillholeTableFile, drillholeProjectRoot, drillholeRootname, tableType);

                if (query != null)
                    fields = query as ImportTableFields;
                //else 
                //{
                //    XElement elements = await factory.CreateXML(drillholeTableFile, fields, tableType, drillholeRootname);

                //    query = await factory.ReturnValuesFromXML(projectFile, drillholeTableFile, drillholeProjectRoot, drillholeRootname, tableType);
                //    if (query != null)
                //        fields = query as ImportTableFields;
                //}
            }

            return fields;
        }
        #endregion


        public async void TableParameters(string projectFile, string drillholePreferencesFile, string drillholeProjectRoot, DrillholeTableType tableType)
        {
            if (factory == null)
                factory = new XmlFactory(DrillholesXmlEnum.DrillholeTableParameters);
            else
                factory.SetXmlType(DrillholesXmlEnum.DrillholeTableParameters);

            if (File.Exists(projectFile))
            {
                factory.UpdateProjectFile(projectFile, drillholePreferencesFile, drillholeProjectRoot, tableType);

            }
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

        public async Task<object> DrillholeProjectProperties(string projectFile, string drillholeTableFile, string drillholeProjectRoot, string drillholeRootname, DrillholeTableType tableType)
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

                if (values[1] != "")  //saved session at dialog page
                {
                    // var updateValues = elements.Select(e => e.Element(DrillholeConstants.drillholeTable)).SingleOrDefault();

                    var updateValues = elements.Select(e => e.Element(DrillholeConstants.drillholeFields));

                    var tableValueToUpdate = updateValues.Select(e => e.Element(tableType.ToString())).FirstOrDefault();

                    tableValueToUpdate.Value = drillholeTableFile;
                    // updateValues.Value = drillholeTableFile;

                    xmlFile.Save(projectFile);
                }
                else if (values[2] != "")  //saved session at dialog page
                {
                    // var updateValues = elements.Select(e => e.Element(DrillholeConstants.drillholeTable)).SingleOrDefault();

                    var updateValues = elements.Select(e => e.Element(DrillholeConstants.drillholeData));

                    var tableValueToUpdate = updateValues.Select(e => e.Element(tableType.ToString())).FirstOrDefault();

                    tableValueToUpdate.Value = drillholeTableFile;

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
                    updateValues.Value = drillholeTableFile;  //ADD TO XMLFACTORY

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

        public void DrillholeFieldParameters(string projectFile, string drillholeFieldsFile, string drillholeProjectRoot, DrillholeTableType tableType)
        {
            if (factory == null)
                factory = new XmlFactory(DrillholesXmlEnum.DrillholeFields);
            else
                factory.SetXmlType(DrillholesXmlEnum.DrillholeFields);

            if (File.Exists(projectFile))
            {
                factory.UpdateProjectFile(projectFile, drillholeFieldsFile, drillholeProjectRoot, tableType);

            }
        }

        public async Task<List<DrillholeTable>> TableParameters(string projectFile, string drillholeTableFile, string drillholeProjectRoot, string drillholeRootname, DrillholeTableType tableType)
        {
            List<DrillholeTable> tables = new List<DrillholeTable>();

            if (factory == null)
                factory = new XmlFactory(DrillholesXmlEnum.DrillholeTableParameters);
            else
                factory.SetXmlType(DrillholesXmlEnum.DrillholeTableParameters);

            if (File.Exists(projectFile))
            {
                var query = await factory.ReturnValuesFromXML(projectFile, drillholeTableFile, drillholeProjectRoot, drillholeRootname, tableType);

                if (query != null)
                    tables = query as List<DrillholeTable>;
            }

            return tables;
        }


    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Drillholes.Domain.DataObject;
using Drillholes.Domain.Enum;
using Drillholes.Domain.Interfaces;

namespace Drillholes.Domain.Services
{
    public class XmlService
    {
        private readonly IDrillholeXML _xml;

        public XmlService(IDrillholeXML xml)
        {
            this._xml = xml;
        }

        public async Task<XElement> TableParameters(string fileName, List<DrillholeTable> importTables, string rootName)
        {
            var xml = await _xml.TableParameters(fileName, importTables, rootName);

            return xml;
        }

        public async Task<XElement> DrillholeData(string fileName, XElement xPreview, DrillholeTableType tableType, string xmlNodeName, string rootName)
        {
            var xml = await _xml.DrillholeData(fileName, xPreview, tableType, xmlNodeName, rootName);

            return xml;
        }

        public async Task<XDocument> DrillholeFields(string fileName, ImportTableFields fields, DrillholeTableType tableType, string rootName)
        {
            var xml =  await _xml.DrillholeFieldParameters(fileName, fields, tableType, rootName);

            return xml;
        }



        public async Task<XDocument> DrillholeProjectProperties(DrillholeProjectProperties prop, string rootName)
        {
            var xml = await _xml.DrillholeProjectProperties(prop, rootName);

            return xml;
        }

        public async Task<List<DrillholeTable>> DrillholeProjectProperties(string projectFile, string drillholeTableFile, string projectRoot,  string rootName, DrillholeTableType tableType)
        {
            var xml = await _xml.DrillholeProjectProperties(projectFile, drillholeTableFile, projectRoot, rootName, tableType) as List<DrillholeTable>;

            return xml;
        }

        public async Task<XDocument> DrillholePreferences(string fileName, DrillholePreferences preferences, string rootName)
        {
            var xml = await _xml.DrillholePreferences(fileName, preferences, rootName);

            return xml;
        }

        public async Task<XDocument> DrillholePreferences(string fileName, string xmlName, object xmlValue, string rootName)
        {
            var xml = await _xml.DrillholePreferences(fileName, xmlName, xmlValue, rootName);

            return xml;
        }

        public async void DrillholePreferences(string projectFile, string drillholePreferencesFile, string drillholeRootName)
        {
             _xml.DrillholePreferences(projectFile, drillholePreferencesFile, drillholeRootName);

        }
    }
}

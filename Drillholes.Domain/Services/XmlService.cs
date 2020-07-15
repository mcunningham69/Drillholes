using System;
using System.Collections.Generic;
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

        public async Task<bool> DrillholeFields(string fileName, ImportTableFields fields, DrillholeTableType tableType, string rootName)
        {
            var xml =  await _xml.DrillholeFieldParameters(fileName, fields, tableType, rootName);

            return xml;
        }
    }
}

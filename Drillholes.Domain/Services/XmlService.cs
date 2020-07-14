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

        public async Task<XElement> TableParameters(string fileName, List<DrillholeTable> importTables)
        {
            var xml = await _xml.TableParameters(fileName, importTables);

            return xml;
        }

        public async Task<XElement> DrillholeData(string fileName, XElement xPreview, DrillholeTableType tableType)
        {
            var xml = await _xml.DrillholeData(fileName, xPreview, tableType);

            return xml;
        }
    }
}

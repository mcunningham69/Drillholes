using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Drillholes.Domain.DataObject;
using Drillholes.Domain.Enum;
using Drillholes.Domain.Interfaces;

namespace Drillholes.XML
{
    public class XmlController : IDrillholeXML
    {
        public async Task<XElement> DrillholeData(string fileName, XElement xPreview, DrillholeTableType tableType)
        {
            XmlFactory factory = new XmlFactory(DrillholesXmlEnum.DrillholeInputData);

            return await factory.CreateXML(fileName, xPreview, tableType);
        }

        public Task<XElement> DrillholeDesurvey()
        {
            throw new NotImplementedException();
        }

        public Task<XElement> DrillholePreferences()
        {
            throw new NotImplementedException();
        }

        public Task<XElement> FieldParameters()
        {
            throw new NotImplementedException();
        }

 

        public async Task<XElement> TableParameters(string fileName, List<DrillholeTable> importTables)
        {
            XmlFactory factory = new XmlFactory(DrillholesXmlEnum.DrillholeTableParameters);
            XElement elements = await factory.CreateXML(fileName, importTables);

            return elements;
        }
    }
}

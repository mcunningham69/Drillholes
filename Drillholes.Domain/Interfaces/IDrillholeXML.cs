using Drillholes.Domain.DataObject;
using Drillholes.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Drillholes.Domain.Interfaces
{
    public interface IDrillholeXML
    {
        Task<XElement> TableParameters(string fileName, List<DrillholeTable> importTables, string rootName);

        Task<XDocument> DrillholeFieldParameters(string fileName, ImportTableFields fields, DrillholeTableType tableType, string rootName);

        Task<XElement> DrillholeData(string fileName, XElement xPreview, DrillholeTableType tableType, string xmlNodeName, string rootName);

        Task<XDocument> DrillholePreferences(string fileName, DrillholePreferences preferences, string rootName);
        Task<XDocument> DrillholePreferences(string fileName, string xmlName, object xmlValue, string rootName);

        Task<XDocument> DrillholeProjectProperties(DrillholeProjectProperties xmlValues, string rootName);

        Task<List<DrillholeTable>> DrillholeProjectProperties(string projectFile, string drillholeTableFile, string drillholeProject, string drillholeRootname);


        Task<XElement> DrillholeDesurvey();
    }
}

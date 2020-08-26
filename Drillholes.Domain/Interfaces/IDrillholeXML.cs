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
        void TableParameters(string projectFile, string drillholePreferencesFile, string drillholeProjectRoot, DrillholeTableType tableType);
        Task<List<DrillholeTable>> TableParameters(string projectFile, string drillholeTableFile, string drillholeProjectRoot, string drillholeRootname, DrillholeTableType tableType);
        Task<ImportTableFields> DrillholeFieldParameters(string projectFile, string drillholeTableFile, string drillholeProjectRoot, string drillholeRootname, DrillholeTableType tableType);
        Task<XDocument> DrillholeFieldParameters(string fileName, ImportTableFields fields, DrillholeTableType tableType, string rootName);
        void DrillholeFieldParameters(string projectFile, string drillholeFieldsFile, string drillholeProjectRoot, DrillholeTableType tableType);


        Task<XElement> DrillholeData(string fileName, XElement xPreview, DrillholeTableType tableType, string xmlNodeName, string rootName);
        void DrillholeData(string projectFile, string drillholePreferencesFile, string drillholeProjectRoot, DrillholeTableType tableType);
        Task<XElement> DrillholeData(string projectFile, string drillholeTableFile, string drillholeProjectRoot, string drillholeRootname, DrillholeTableType tableType);
        Task<XDocument> DrillholePreferences(string fileName, DrillholePreferences preferences, string rootName);
        Task<XDocument> DrillholePreferences(string fileName, string xmlName, object xmlValue, string rootName);

        Task<DrillholePreferences> DrillholePreferences(string drillholeTableFile, string drillholeRootname);

        void DrillholePreferences(string projectFile, string drillholePreferencesFile, string drillholeProjectRoot, DrillholeTableType tableType);

        Task<XDocument> DrillholeProjectProperties(DrillholeProjectProperties xmlValues, string rootName);

        Task<object> DrillholeProjectProperties(string projectFile, string drillholeTableFile, string drillholeProjectRoot, string drillholeRootname, DrillholeTableType tableType);


        Task<XElement> DrillholeDesurvey(string fileName, object xPreview, DrillholeTableType tableType, string xmlNodeName, string rootName, bool bDownhole);
        void DrillholeDesurvey(string projectFile, string drillholePreferencesFile, string drillholeProjectRoot, DrillholeTableType tableType);
        Task<XElement> DrillholeDesurvey(string projectFile, string drillholeTableFile, string drillholeProjectRoot, string drillholeRootname, DrillholeTableType tableType, bool bDownhole);
    }
}

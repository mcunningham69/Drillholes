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
        Task<XElement> TableParameters(string fileName, List<DrillholeTable> importTables);

        Task<XElement> FieldParameters();

        Task<XElement> DrillholeData(string fileName, XElement xPreview, DrillholeTableType tableType);

        Task<XElement> DrillholePreferences();
        Task<XElement> DrillholeDesurvey();
    }
}

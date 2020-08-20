using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drillholes.Domain.Enum;

namespace Drillholes.Domain.Interfaces
{
    public interface IDrillholeExport
    {
        Task<bool> ExportResultsToCsv(string outputName, string drillholeTableFile, string drillholeFields, string drillholeOtherFields, string drillholeInputData, DrillholeImportFormat exportMode,
            bool bAttributes, DrillholeTableType tableType);
       

    }
}

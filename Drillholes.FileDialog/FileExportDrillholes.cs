using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drillholes.Domain.Interfaces;
using Drillholes.Domain.Enum;

namespace Drillholes.FileDialog
{
    public class FileExportDrillholes : IDrillholeExport
    {
        FileExportFactory factory = null;

        public async Task<bool>ExportResultsToCsv(string outputName, string drillholeTableFile, string drillholeFields, string drillholeInputData, DrillholeImportFormat exportMode, bool bAttributes)
        {
            if (factory == null)
                factory = new FileExportFactory(exportMode);
            else
                factory.SetExportType(exportMode);

            await factory.ExportCollarTable(outputName, drillholeTableFile, drillholeFields, drillholeInputData, bAttributes);

            return true;

        }
    }
}

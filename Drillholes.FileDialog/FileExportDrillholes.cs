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

        public async Task<bool> ExportResultsToCsv(string outputName, string drillholeTableFile, string drillholeFields, string drillholeOtherFields, string drillholeInputData,
            DrillholeImportFormat exportMode, bool bAttributes, DrillholeTableType tableType, string defaultValue, bool bVertical)
        {
            if (factory == null)
                factory = new FileExportFactory(exportMode);
            else
                factory.SetExportType(exportMode);

            if (tableType == DrillholeTableType.collar)
                await factory.ExportCollarTable(outputName, drillholeTableFile, drillholeFields, drillholeInputData, bAttributes);
            else if (tableType == DrillholeTableType.survey)
                await factory.ExportSurveyTable(outputName, drillholeTableFile, drillholeFields, drillholeOtherFields, drillholeInputData, bAttributes);
            else if (tableType == DrillholeTableType.assay)
                await factory.ExportAssayTable(outputName, drillholeTableFile, drillholeFields, drillholeOtherFields, drillholeInputData, bAttributes);
            else if (tableType == DrillholeTableType.interval)
                await factory.ExportIntervalTable(outputName, drillholeTableFile, drillholeFields, drillholeOtherFields, drillholeInputData, bAttributes);
            else if (tableType == DrillholeTableType.continuous)
                await factory.ExportContinuousTable(outputName, drillholeTableFile, drillholeFields, drillholeOtherFields, drillholeInputData, bAttributes, defaultValue, bVertical);

            return true;

        }

       
    }
}

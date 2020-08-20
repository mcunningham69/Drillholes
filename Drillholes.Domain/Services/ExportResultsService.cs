using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drillholes.Domain.Interfaces;
using AutoMapper;
using Drillholes.Domain.Enum;

namespace Drillholes.Domain.Services
{
    public class ExportResultsService
    {
        private readonly IDrillholeExport _drillhole;


        public ExportResultsService(IDrillholeExport drillhole)
        {
            this._drillhole = drillhole;
        }

        public async Task<bool> ExportTextCsv(string outputName, string drillholeTableFile, string drillholeFields, string drillholeOtherFields, string drillholeInputData, 
            DrillholeImportFormat exportMode, bool bAttributes, DrillholeTableType tableType)
        {
            bool saveToCsv = await _drillhole.ExportResultsToCsv(outputName, drillholeTableFile, drillholeFields, drillholeOtherFields, drillholeInputData, exportMode, bAttributes, tableType);
            
            if (saveToCsv)
                return true;
            else
                return false;
        }
    }
}

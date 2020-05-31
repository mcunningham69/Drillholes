using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drillholes.Domain.DTO;
using Drillholes.Domain.Enum;

namespace Drillholes.Domain.Interfaces
{
    public interface IDrillholeTables
    {
        DrillholeImportDto SelectIntervalTables(DrillholeTableType tableType);
        DrillholeImportDto SelectTables(DrillholeTableType tableType);
    }
}

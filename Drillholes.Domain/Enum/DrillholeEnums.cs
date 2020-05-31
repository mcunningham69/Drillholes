using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drillholes.Domain.Enum
{
    public enum DrillholeImportFormat
    {
        excel_table,
        egdb_table,
        fgdb_table,
        pgdb_table,
        text_csv,
        text_txt,
        other
    }
    public enum DrillholeMessageStatus
    {
        Valid,
        Error,
        Warning
    }
    public enum DrillholeSurveyType
    {
        collarsurvey,
        downholesurvey,
        vertical
    }
    public enum DrillholeTableType
    {
        collar,
        survey,
        assay,
        interval,
        other
    }

    public enum DrillholeInterfaceType
    {
        WindowsFileDialog,
        WindowsExcelDialog,
        WindowsArcGIS,
        MacFileDialog,
        MacExcelDialog
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drillholes.Domain.Enum;

namespace Drillholes.Domain.DTO
{
    public class CollarTableDto
    {
        public DrillholeTableType tableType { get; set; }
        public string tableName { get; set; }
        public bool tableIsValid { get; set; }
        public List<string> fields { get; set; }
         public string tableField { get; set; }
        public ImportTableFields tableData { get; set; }
        public DrillholeImportFormat tableFormat { get; set; }
        public System.Xml.Linq.XElement xPreview { get; set; }
        public string collarKey { get; set; }
        public bool isValid { get; set; }
        public bool isCancelled { get; set; }
       // public SummaryCollarStatistics SummaryStats { get; set; }

        //  public TestMessages testMessages { get; set; }
        public bool hasEdits { get; set; }
        public string tableLocation { get; set; }
        public DrillholeSurveyType surveyType { get; set; }
        public List<string> collarIDs { get; set; }

        public CollarTableDto()
        {
            //tableField = "";
        }
    }
}

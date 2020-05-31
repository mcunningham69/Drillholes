using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drillholes.Domain.Enum;

namespace Drillholes.Domain.DataObject
{
    public class DrillholeTable
    {
        public string tableLocation { get; set; }

        public string tableName { get; set; }

        public DrillholeTableType tableType { get; set; }

        public bool isValid { get; set; }

        public bool isCancelled { get; set; }

        public DrillholeImportFormat tableFormat { get; set; }

        public string tableFormatName { get; set; }

        public DrillholeTable()
        {
            isCancelled = true;
            isValid = true;
        }

        public DrillholeTable(bool _isValid)
        {
            isValid = _isValid;
        }

        public virtual void ReturnTableDetail()
        {


        }
    }

}

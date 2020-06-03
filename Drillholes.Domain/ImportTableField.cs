using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drillholes.Domain
{
    public class ImportTableField
    {
        public string columnHeader { get; set; }
        public string columnImportAs { get; set; }
        public string columnImportName { get; set; }
        public string groupName { get; set; }
        public bool genericType { get; set; }
        public KeyValuePair<bool, bool> keys { get; set; }
        public string fieldType { get; set; }

    }


    public class ImportTableFields : List<ImportTableField>
    {
       // public event EventHandler ItemsUpdated;

        public ImportTableFields()
        {

           // ItemsUpdated?.Invoke(this, new EventArgs());
        }
    }
}

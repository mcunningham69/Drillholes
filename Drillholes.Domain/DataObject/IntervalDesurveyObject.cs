using Drillholes.Domain.DTO;
using Drillholes.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drillholes.Domain.DataObject
{
    public class IntervalDesurveyObject : AssayDesurveyObject
    {
        public ImportTableFields intervalTableFields { get; set; }
        public List<bool> isInterval { get; set; }
        public List<int> intId { get; set; }


    }
}

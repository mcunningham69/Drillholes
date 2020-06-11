using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drillholes.Domain.DataObject
{
    public class ValidationCollar
    {
        public ValidationMessages testMessages { get; set; }
        public bool hasEdits { get; set; }
    }
}

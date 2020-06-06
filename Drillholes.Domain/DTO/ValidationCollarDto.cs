using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drillholes.Domain.DTO
{
    public class ValidationCollarDto
    {
        public ValidationMessages testMessages { get; set; }
        public bool isValid { get; set; }

    }
}

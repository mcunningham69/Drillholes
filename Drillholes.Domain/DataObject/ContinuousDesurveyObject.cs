using Drillholes.Domain.DTO;
using Drillholes.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drillholes.Domain.DataObject
{
    public class ContinuousDesurveyObject : IntervalDesurveyObject
    {
        public ImportTableFields continuousTableFields { get; set; }
        public List<bool> isContinuous { get; set; }
        public List<int> contId { get; set; }



    }
}

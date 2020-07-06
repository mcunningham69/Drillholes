using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drillholes.Domain.Enum;

namespace Drillholes.Domain
{
    public class EditEnvironment
    {
        public DrillholeEditSession editSession;

        public bool hasEdits;

        public EditEnvironment()
        {
            hasEdits = false;
        }
        public void StartEditSession()
        {
            editSession = DrillholeEditSession.Started;

        }

        public void StopEditSession()
        {
            editSession = DrillholeEditSession.Stopped;

            hasEdits = false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drillholes.Domain
{
    public class ValidationMessages
    {
        public string testType { get; set; }
        public List<ValidationMessage> testMessage { get; set; }

        public ValidationMessages()
        {
            testMessage = new List<ValidationMessage>();
        }
        public override string ToString()
        {
            return testType.ToString();
        }
    }

    public class ValidationMessage
    {
        public int count { get; set; }
        public List<ImportTableField> tableFields { get; set; }
        public string validationTest { get; set; }
        public List<string> validationMessages { get; set; }
        public List<DrillholeValidationStatus> ValidationStatus { get; set; }
        public bool verified { get; set; }

        public ValidationMessage()
        {
            ValidationStatus = new List<DrillholeValidationStatus>();
        }

    }

    public class DrillholeValidationStatus
    {
        public string Description { get; set; }
        public string ErrorColour { get; set; }
        public Enum.DrillholeMessageStatus ErrorType { get; set; }
        public int id { get; set; }

        public DrillholeValidationStatus()
        {
            // Description = new List<string>();
        }
        public override string ToString()
        {
            return ErrorType.ToString();
        }
    }


}

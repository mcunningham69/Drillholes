using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace Drillholes.Domain.Exceptions
{
    public class SurveyException : Exception
    {
        public SurveyException() : base()
        {
        }

        public SurveyException(string message) : base(message)
        {
        }

        public SurveyException(string message, Exception inner) : base(message, inner)
        {
        }

        protected SurveyException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }
    }
}

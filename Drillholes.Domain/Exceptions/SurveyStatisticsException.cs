using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace Drillholes.Domain.Exceptions
{
    public class SurveyStatisticsException : Exception
    {
        public SurveyStatisticsException() : base()
        {
        }

        public SurveyStatisticsException(string message) : base(message)
        {
        }

        public SurveyStatisticsException(string message, Exception inner) : base(message, inner)
        {
        }

        protected SurveyStatisticsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }
    }
}

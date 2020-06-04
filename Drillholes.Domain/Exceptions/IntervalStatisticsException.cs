using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace Drillholes.Domain.Exceptions
{
    public class IntervalStatisticsException : Exception
    {
        public IntervalStatisticsException() : base()
        {
        }

        public IntervalStatisticsException(string message) : base(message)
        {
        }

        public IntervalStatisticsException(string message, Exception inner) : base(message, inner)
        {
        }

        protected IntervalStatisticsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }
    }
}

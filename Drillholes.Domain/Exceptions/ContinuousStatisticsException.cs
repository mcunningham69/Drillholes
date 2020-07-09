using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace Drillholes.Domain.Exceptions
{
    public class ContinuousStatisticsException : Exception
    {
        public ContinuousStatisticsException() : base()
        {
        }

        public ContinuousStatisticsException(string message) : base(message)
        {
        }

        public ContinuousStatisticsException(string message, Exception inner) : base(message, inner)
        {
        }

        protected ContinuousStatisticsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }
    }
}

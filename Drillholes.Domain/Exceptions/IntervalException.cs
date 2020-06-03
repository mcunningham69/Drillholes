using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace Drillholes.Domain.Exceptions
{
    public class IntervalException : Exception
    {
        public IntervalException() : base()
        {
        }

        public IntervalException(string message) : base(message)
        {
        }

        public IntervalException(string message, Exception inner) : base(message, inner)
        {
        }

        protected IntervalException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }
    }
}

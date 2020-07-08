using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace Drillholes.Domain.Exceptions
{
    public class ContinuousException : Exception
    {
        public ContinuousException() : base()
        {
        }

        public ContinuousException(string message) : base(message)
        {
        }

        public ContinuousException(string message, Exception inner) : base(message, inner)
        {
        }

        protected ContinuousException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }
    }
}

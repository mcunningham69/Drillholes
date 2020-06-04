using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace Drillholes.Domain.Exceptions
{
    public class AssayStatisticsException : Exception
    {
        public AssayStatisticsException() : base()
        {
        }

        public AssayStatisticsException(string message) : base(message)
        {
        }

        public AssayStatisticsException(string message, Exception inner) : base(message, inner)
        {
        }

        protected AssayStatisticsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }
    }
}

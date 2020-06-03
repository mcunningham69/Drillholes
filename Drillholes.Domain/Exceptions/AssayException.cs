using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace Drillholes.Domain.Exceptions
{
    public class AssayException : Exception
    {
        public AssayException() : base()
        {
        }

        public AssayException(string message) : base(message)
        {
        }

        public AssayException(string message, Exception inner) : base(message, inner)
        {
        }

        protected AssayException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }
    }
}

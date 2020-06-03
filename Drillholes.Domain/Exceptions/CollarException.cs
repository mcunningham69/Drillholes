using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace Drillholes.Domain.Exceptions
{
    public class CollarException : Exception
    {
        public CollarException() : base()
        {
        }

        public CollarException(string message) : base(message)
        {
        }

        public CollarException(string message, Exception inner) : base(message, inner)
        {
        }

        protected CollarException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }
    }
}

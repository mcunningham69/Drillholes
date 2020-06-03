using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace Drillholes.Domain.Exceptions
{
    public class TableTypeException : Exception
    {
        public TableTypeException() : base()
        {
        }

        public TableTypeException(string message) : base(message)
        {
        }

        public TableTypeException(string message, Exception inner) : base(message, inner)
        {
        }

        protected TableTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace Drillholes.Domain.Exceptions
{
    public class ImportFormatException : Exception
    {
        public ImportFormatException() : base()
        {
        }

        public ImportFormatException(string message) : base(message)
        {
        }

        public ImportFormatException(string message, Exception inner) : base(message, inner)
        {
        }

        protected ImportFormatException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace Drillholes.Domain.Exceptions
{
    public class ImportClassException : Exception
    {
        public ImportClassException() : base()
        {
        }

        public ImportClassException(string message) : base(message)
        {
        }

        public ImportClassException(string message, Exception inner) : base(message, inner)
        {
        }

        protected ImportClassException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace Drillholes.Domain.Exceptions
{
    public class CollarStatisticsException : Exception
    {
        public CollarStatisticsException() : base()
        {
        }

        public CollarStatisticsException(string message) : base(message)
        {
        }

        public CollarStatisticsException(string message, Exception inner) : base(message, inner)
        {
        }

        protected CollarStatisticsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }
    }
}

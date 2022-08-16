using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Exceptions
{

    [Serializable]
    public class YtdlpException : Exception
    {
        public YtdlpException() { }
        public YtdlpException(string message) : base(message) { }
        public YtdlpException(string message, Exception inner) : base(message, inner) { }
        public YtdlpException(string message, Exception? inner, List<string> errorOutput) : base(message, inner)
        {
            ErrorOutputMessages = errorOutput.AsReadOnly();
        }
        protected YtdlpException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        public IReadOnlyList<string>? ErrorOutputMessages { get;}
    }
}

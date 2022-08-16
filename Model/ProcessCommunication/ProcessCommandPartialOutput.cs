using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ProcessCommunication
{
    public class ProcessCommandPartialOutput
    {
        public ProcessCommandPartialOutput(string message) : this(message, false) { }

        public ProcessCommandPartialOutput(string message, bool isError)
        {
            Message = message;
            IsError = isError;
            Timestamp = DateTimeOffset.UtcNow;
        }

        public string Message { get; }

        public bool IsError { get; }

        public DateTimeOffset Timestamp { get; }
    }
}

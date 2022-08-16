using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ProcessCommunication
{
    internal class ProcessCommandOutputBuilder
    {
        private readonly IList<ProcessCommandPartialOutput> _partialOutputs = new List<ProcessCommandPartialOutput>();

        public void AddOutput(ProcessCommandPartialOutput partialOutput)
        {
            _partialOutputs.Add(partialOutput);
        }

        public ProcessCommandOutput BuildOutput()
        {
            StringBuilder resultBuilder = new();
            List<ProcessCommandPartialOutput> partialOutputs = new List<ProcessCommandPartialOutput>();
            List<ProcessCommandPartialOutput> errorPartialOutputs = new List<ProcessCommandPartialOutput>();

            foreach (ProcessCommandPartialOutput partialOutput in _partialOutputs)
            {
                resultBuilder.AppendLine(partialOutput.Message);
                if (partialOutput.IsError)
                {
                    errorPartialOutputs.Add(partialOutput);
                }
                else
                {
                    partialOutputs.Add(partialOutput);
                }
            }

            string? resultString = _partialOutputs.Any() ? resultBuilder.ToString() : null;
            return new ProcessCommandOutput(resultString, partialOutputs.AsReadOnly(), errorPartialOutputs.AsReadOnly());
        }
    }
}

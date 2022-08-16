namespace Model.ProcessCommunication
{
    /// <summary>
    /// Represents the output from interaction with an external process.
    /// </summary>
    public class ProcessCommandOutput
    {
        /// <summary>
        /// Initializes a new instacne of the <see cref="ProcessCommandOutput"/> class.
        /// </summary>
        /// <param name="outputString"></param>
        /// <param name="partialResults"></param>
        /// <param name="errorPartialResults"></param>
        public ProcessCommandOutput(
            string? outputString,
            IReadOnlyList<ProcessCommandPartialOutput> partialResults,
            IReadOnlyList<ProcessCommandPartialOutput> errorPartialResults)
        {
            Result = outputString;
            PartialResults = partialResults;
            ErrorPartialResults = errorPartialResults;
        }

        /// <summary>
        /// A result of interaction with an external process as a single <see cref="string"/>.
        /// </summary>
        public string? Result { get; }

        /// <summary>
        /// A collection of <see cref="ProcessCommandPartialOutput"/> objects, which contribute to final output result.
        /// Only results that are NOT marked as an error are included within this collection.
        /// </summary>
        public IReadOnlyList<ProcessCommandPartialOutput> PartialResults { get; }

        /// <summary>
        /// A collection of <see cref="ProcessCommandPartialOutput"/> objects, which contribute to final output result.
        /// This collection provides only results that ARE marked as an ERROR.
        /// </summary>
        public IReadOnlyList<ProcessCommandPartialOutput> ErrorPartialResults { get; }
    }
}
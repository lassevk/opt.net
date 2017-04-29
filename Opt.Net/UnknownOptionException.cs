namespace Opt
{
    /// <summary>
    /// This exception is thrown in response to an unknown option being used on the command line.
    /// </summary>
    public class UnknownOptionException : OptionException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownOptionException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">
        /// The message detailing the exceptional condition.
        /// </param>
        public UnknownOptionException(string message)
            : base(message)
        {
            // Do nothing here
        }
    }
}
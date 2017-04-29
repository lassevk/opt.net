using System;

namespace Opt
{
    /// <summary>
    /// This exception is thrown in response to invalid syntax being used with an option, either too many minus signs,
    /// incorrect handling of values, etc.
    /// </summary>
    public class OptionSyntaxException : OptionException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionSyntaxException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">
        /// The message detailing the exceptional condition.
        /// </param>
        public OptionSyntaxException(string message)
            : base(message)
        {
            // Do nothing here
        }
    }
}
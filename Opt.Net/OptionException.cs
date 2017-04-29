using System;

namespace Opt
{
    /// <summary>
    /// This is the base class for exceptions thrown to indicate something wrong with
    /// the command line arguments and options.
    /// </summary>
    public abstract class OptionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionException"/> class.
        /// </summary>
        protected OptionException()
        {
            // Do nothing here
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">
        /// The message detailing the exceptional condition.
        /// </param>
        protected OptionException(string message)
            : base(message)
        {
            // Do nothing here
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionException"/> class with a specified error message and
        /// a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The message detailing the exceptional condition.</param>
        /// <param name="innerException">The inner exception.</param>
        protected OptionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
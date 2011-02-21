using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;

namespace Opt
{
    /// <summary>
    /// This is the base class for exceptions thrown to indicate something wrong with
    /// the command line arguments and options.
    /// </summary>
    [Serializable]
    public class OptionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionException"/> class.
        /// </summary>
        public OptionException()
        {
            // Do nothing here
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">
        /// The message detailing the exceptional condition.
        /// </param>
        public OptionException(string message)
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
        public OptionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionException"/> class with serialized data.
        /// </summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para>The <paramref name="info"/> parameter is null.</para>
        /// </exception>
        /// <exception cref="SerializationException">
        /// <para>The class name is null or <see cref="Exception.HResult"/> is zero (0).</para>
        /// </exception>
        protected OptionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            // Do nothing here
        }
    }
}
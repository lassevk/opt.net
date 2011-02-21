using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;

namespace Opt
{
    /// <summary>
    /// This exception is thrown in response to invalid syntax being used with an option, either too many minus signs,
    /// incorrect handling of values, etc.
    /// </summary>
    [Serializable]
    public class OptionSyntaxException : OptionException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionSyntaxException"/> class.
        /// </summary>
        public OptionSyntaxException()
        {
            // Do nothing here
        }

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

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionSyntaxException"/> class with a specified error message and
        /// a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The message detailing the exceptional condition.</param>
        /// <param name="innerException">The inner exception.</param>
        public OptionSyntaxException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionSyntaxException"/> class with serialized data.
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
        protected OptionSyntaxException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            // Do nothing here
        }
    }
}
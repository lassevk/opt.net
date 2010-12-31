using System;
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

        /// <summary>
        /// Sets the <see cref="SerializationInfo"/> with information about the exception.
        /// </summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="info"/> parameter is a null reference (Nothing in Visual Basic).
        /// </exception>
        /// <filterpriority>
        /// 2
        /// </filterpriority>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Read="*AllFiles*" PathDiscovery="*AllFiles*"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="SerializationFormatter"/>
        /// </PermissionSet>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            // Do nothing here
        }
    }
}
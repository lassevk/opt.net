using System;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Opt
{
    /// <summary>
    /// This <see cref="BaseOptionAttribute"/> descendant can be applied to <see cref="int"/> and other
    /// integer-type properties in order to specify how to map command line arguments to that property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public sealed class IntegerOptionAttribute : BaseOptionAttribute
    {
        private readonly Type[] _SupportedTypes = new[] { typeof(byte), typeof(byte?), typeof(sbyte), typeof(sbyte?), typeof(short), typeof(short?), typeof(ushort), typeof(ushort?), typeof(int), typeof(int?), typeof(uint), typeof(uint?), typeof(long), typeof(long?), typeof(ulong), typeof(ulong?), };

        /// <summary>
        /// Initializes a new instance of the <see cref="IntegerOptionAttribute"/> class.
        /// </summary>
        /// <param name="option">The option this <see cref="BaseOptionAttribute"/> will handle. Must be prefixed with either two minus signs (for long
        /// options), or one minus sign (for short options.)</param>
        /// <exception cref="ArgumentException">
        /// 	<para><paramref name="option"/> specifies a long option that is too short, must be at least 2 characters long (+ the two minus sign, totalling 4 characters in the string.)</para>
        /// 	<para>- or -</para>
        /// 	<para><paramref name="option"/> specifies a short option that is too long, must be only 1 character long (+ the one minus sign, totalling 2 characters in the string.)</para>
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// 	<para><paramref name="option"/> is <c>null</c> or empty.</para>
        /// </exception>
        public IntegerOptionAttribute(string option)
            : this(option, "VALUE")
        {
            // Do nothing here
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IntegerOptionAttribute"/> class.
        /// </summary>
        /// <param name="option">The option this <see cref="BaseOptionAttribute"/> will handle. Must be prefixed with either two minus signs (for long
        /// options), or one minus sign (for short options.)</param>
        /// <param name="parameterName">The name of the parameter, this is mostly used for help texts. This should be <see cref="String.Empty"/>
        /// if no parameter is used.</param>
        /// <exception cref="ArgumentException">
        /// 	<para><paramref name="option"/> specifies a long option that is too short, must be at least 2 characters long (+ the two minus sign, totalling 4 characters in the string.)</para>
        /// 	<para>- or -</para>
        /// 	<para><paramref name="option"/> specifies a short option that is too long, must be only 1 character long (+ the one minus sign, totalling 2 characters in the string.)</para>
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// 	<para><paramref name="option"/> is <c>null</c> or empty.</para>
        /// 	<para>- or -</para>
        /// 	<para><paramref name="parameterName"/> is <c>null</c>.</para>
        /// </exception>
        public IntegerOptionAttribute(string option, string parameterName)
            : base(option, parameterName)
        {
            // Do nothing here
        }

        /// <summary>
        /// Validates the usage of the attribute on the container type.
        /// </summary>
        /// <param name="containerType">
        /// The <see cref="Type"/> of the container.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="containerType"/> is <c>null</c>.</para>
        /// </exception>
        public override void ValidateUsage(Type containerType)
        {
            if (containerType == null)
                throw new ArgumentNullException("containerType");

            // Do nothing here
        }

        /// <summary>
        /// Validates the usage of the attribute on the property in a container.
        /// </summary>
        /// <param name="propertyInfo">
        /// The <see cref="PropertyInfo"/> of the property to validate against.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="propertyInfo"/> is <c>null</c>.</para>
        /// </exception>
        public override void ValidateUsage(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            if (!_SupportedTypes.Contains(propertyInfo.PropertyType))
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "The IntegerOptionAttribute can only be applied to properties of any of the integer types, including nullable variants of them, was applied to a property of type {0}", propertyInfo.PropertyType));
        }

        /// <summary>
        /// Interprets the given value and assign it to the given property.
        /// </summary>
        /// <param name="container">
        /// The container object to assign the property value into.
        /// </param>
        /// <param name="propertyInfo">
        /// The <see cref="PropertyInfo"/> of the property to assign the value to.
        /// </param>
        /// <param name="value">
        /// The value specified on the command line, if any, to assign to the value. If given
        /// <see cref="String.Empty"/>, assign the default value to the property, if possible.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="container"/> is <c>null</c>.</para>
        /// <para>- or -</para>
        /// <para><paramref name="propertyInfo"/> is <c>null</c>.</para>
        /// <para>- or -</para>
        /// <para><paramref name="value"/> is <c>null</c>.</para>
        /// </exception>
        public override void AssignValueToProperty(object container, PropertyInfo propertyInfo, string value)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");
            if (value == null)
                throw new ArgumentNullException("value");

            Type integerType = propertyInfo.PropertyType;
            if (integerType.IsGenericType && integerType.GetGenericTypeDefinition() == typeof(Nullable<>))
                integerType = integerType.GetGenericArguments()[0];

            object integerValue = Convert.ChangeType(value, integerType, CultureInfo.InvariantCulture);
            propertyInfo.SetValue(container, integerValue, null);
        }
    }
}
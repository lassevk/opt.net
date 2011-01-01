using System;
using System.Globalization;
using System.Reflection;

namespace Opt
{
    /// <summary>
    /// This <see cref="BaseOptionAttribute"/> descendant can be applied to <see cref="bool"/>
    /// properties in order to specify how to map command line arguments to that property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public sealed class BooleanOptionAttribute : BaseOptionAttribute
    {
        private readonly bool _PropertyValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="BooleanOptionAttribute"/> class.
        /// </summary>
        /// <param name="option">The option this <see cref="BooleanOptionAttribute"/> will handle. Must be prefixed with either two minus signs (for long
        /// options), or one minus sign (for short options.)</param>
        /// <exception cref="ArgumentException">
        /// 	<para><paramref name="option"/> specifies a long option that is too short, must be at least 2 characters long (+ the two minus sign, totalling 4 characters in the string.)</para>
        /// 	<para>- or -</para>
        /// 	<para><paramref name="option"/> specifies a short option that is too long, must be only 1 character long (+ the one minus sign, totalling 2 characters in the string.)</para>
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="option"/> is <c>null</c> or empty.</exception>
        public BooleanOptionAttribute(string option)
            : this(option, true)
        {
            // Do nothing here
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BooleanOptionAttribute"/> class.
        /// </summary>
        /// <param name="option">The option this <see cref="BooleanOptionAttribute"/> will handle. Must be prefixed with either two minus signs (for long
        /// options), or one minus sign (for short options.)</param>
        /// <param name="propertyValue">
        /// The value to set the target property to when this option is discovered, unless a specific value is specified along with the
        /// property.
        /// </param>
        /// <exception cref="ArgumentException">
        /// 	<para><paramref name="option"/> specifies a long option that is too short, must be at least 2 characters long (+ the two minus sign, totalling 4 characters in the string.)</para>
        /// 	<para>- or -</para>
        /// 	<para><paramref name="option"/> specifies a short option that is too long, must be only 1 character long (+ the one minus sign, totalling 2 characters in the string.)</para>
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="option"/> is <c>null</c> or empty.</exception>
        public BooleanOptionAttribute(string option, bool propertyValue)
            : base(option)
        {
            _PropertyValue = propertyValue;
        }

        /// <summary>
        /// The value to set the target property to.
        /// </summary>
        public bool PropertyValue
        {
            get
            {
                return _PropertyValue;
            }
        }

        /// <summary>
        /// When implemented in a descendant class, will validate the usage of the attribute
        /// on the container type.
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
        /// When implemented in a descendant class, will validate the usage of the attribute
        /// on the property in a container.
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

            if (propertyInfo.PropertyType != typeof(bool) && propertyInfo.PropertyType != typeof(bool?))
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "The BooleanOptionAttribute can only be applied to properties of type Boolean and Nullable<Boolean> (Boolean?), was applied to a property of type {0}", propertyInfo.PropertyType));
        }

        /// <summary>
        /// When implemented in descendant types, will interpret the given value and assign it to
        /// the given property.
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

            switch (value.ToUpperInvariant())
            {
                case "":
                    propertyInfo.SetValue(container, _PropertyValue, null);
                    break;

                case "+":
                case "ON":
                case "TRUE":
                case "1":
                case "YES":
                case "Y":
                    propertyInfo.SetValue(container, true, null);
                    break;

                case "-":
                case "OFF":
                case "FALSE":
                case "0":
                case "NO":
                case "N":
                    propertyInfo.SetValue(container, false, null);
                    break;

                default:
                    throw new InvalidOperationException("Unable to assign boolean value to property for option, unknown property value");
            }
        }
    }
}
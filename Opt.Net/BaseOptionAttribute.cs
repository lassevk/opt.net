﻿using System;
using System.Reflection;

namespace Opt
{
    /// <summary>
    /// This is the base class for option attributes for mapping command
    /// line arguments to properties on the container object.
    /// </summary>
    public abstract class BaseOptionAttribute : BasePropertyAttribute
    {
        private readonly string _Option;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseOptionAttribute"/> class.
        /// </summary>
        /// <param name="option">
        /// The option this <see cref="BaseOptionAttribute"/> will handle. Must be prefixed with either two minus signs (for long
        /// options), or one minus sign (for short options.)
        /// </param>
        /// <exception cref="ArgumentException">
        /// <para><paramref name="option"/> specifies a long option that is too short, must be at least 2 characters long (+ the two minus sign, totalling 4 characters in the string.)</para>
        /// <para>- or -</para>
        /// <para><paramref name="option"/> specifies a short option that is too long, must be only 1 character long (+ the one minus sign, totalling 2 characters in the string.)</para>
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="option"/> is <c>null</c> or empty.</para>
        /// </exception>
        protected BaseOptionAttribute(string option)
        {
            if (StringEx.IsNullOrWhiteSpace(option))
                throw new ArgumentNullException("option");
            if (!option.StartsWith("-", StringComparison.Ordinal))
                throw new ArgumentException("An option must start with a minus sign");
            if (option.StartsWith("--", StringComparison.Ordinal) && option.Length < 4)
                throw new ArgumentException("Long options, starting with --, must be at least 2 characters long");
            if (option.StartsWith("-", StringComparison.Ordinal) && !option.StartsWith("--", StringComparison.Ordinal) && option.Length != 2)
                throw new ArgumentException("Short options, starting with -, must be only 1 character long");

            _Option = option;
        }

        /// <summary>
        /// The option this <see cref="BaseOptionAttribute"/> handles.
        /// </summary>
        public string Option
        {
            get
            {
                return _Option;
            }
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
        public abstract void AssignValueToProperty(object container, PropertyInfo propertyInfo, string value);
    }
}
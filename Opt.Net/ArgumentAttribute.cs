using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Opt
{
    /// <summary>
    /// This attribute can be applied to a property of type <see cref="string"/>, with
    /// a specific unique order index. Standalone arguments will be assigned to properties with
    /// this attribute in the order of this order index, ascending order.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ArgumentAttribute : BasePropertyAttribute
    {
        /// <summary>
        /// This is the backing field for the <see cref="Name"/> property.
        /// </summary>
        private string _Name = string.Empty;

        /// <summary>
        /// Gets or sets the short name for the argument, used for help texts.
        /// </summary>
        public string Name
        {
            get
            {
                return _Name;
            }

            set
            {
                _Name = (value ?? string.Empty).Trim();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the argument is optional or not. Default is
        /// <c>false</c>.
        /// </summary>
        public bool Optional
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the order index of this <see cref="ArgumentAttribute"/>, used to assign values from
        /// the command line into properties in the correct order.
        /// </summary>
        public int OrderIndex
        {
            get;

            set;
        }

        /// <summary>
        /// Gets a value indicating whether the property type requires an argument, either as part
        /// of the option, or following the option.
        /// </summary>
        public override bool RequiresArgument
        {
            get
            {
                return false;
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
        /// <exception cref="InvalidOperationException">
        /// <para>The <see cref="ArgumentsAttribute"/> attribute has been applied to a property that is not of type <see cref="String"/>.</para>
        /// </exception>
        public override void ValidateUsage(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            if (!propertyInfo.IsDefined(typeof(ArgumentAttribute), true))
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "The ArgumentAttribute attribute is validated against a property that does not have the attribute, this is an internal error (aka bug), the property validated against was {0}", propertyInfo));
            if (propertyInfo.PropertyType != typeof(string))
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "The ArgumentAttribute attribute is applied to a property that is not of type String, but of {0}", propertyInfo.PropertyType));
        }
    }
}
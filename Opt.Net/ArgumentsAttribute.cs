using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Opt
{
    /// <summary>
    /// This attribute can be applied to a property of type <see cref="Collection{T}"/> where the <c>T</c>
    /// is of type <see cref="string"/>. Any leftover arguments that cannot be explicitly assigned to its
    /// own property will be added to the target property. Only one property in a container can have this
    /// attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ArgumentsAttribute : BasePropertyAttribute
    {
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

            int propertiesWithThisAttribute = (from property in containerType.GetProperties()
                                               where property.IsDefined(typeof (ArgumentsAttribute), true)
                                               select property).Count();

            if (propertiesWithThisAttribute == 0)
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture,
                    "The ArgumentsAttribute attribute is validated against a different container type than originally passed in; this is an internal error (aka bug), type validated against was {0}",
                    containerType));

            if (propertiesWithThisAttribute > 1)
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture,
                    "The ArgumentsAttribute attribute is applied to more than one property in the type {0}",
                    containerType));
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

            if (!propertyInfo.IsDefined(typeof (ArgumentsAttribute), true))
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture,
                    "The ArgumentsAttribute attribute is validated against a property that does not have the attribute, this is an internal error (aka bug), the property validated against was {0}",
                    propertyInfo));
            if (propertyInfo.PropertyType != typeof (Collection<string>))
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture,
                    "The ArgumentsAttribute attribute is applied to a property that is not of type Collection<string>, but of {0}",
                    propertyInfo.PropertyType));
        }
    }
}
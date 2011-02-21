using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Opt
{
    /// <summary>
    /// This class is the base class for all other attribute classes in Opt.Net
    /// that is applied to properties, to control how command line arguments are
    /// assigned to properties on the container object.
    /// </summary>
    public abstract class BasePropertyAttribute : Attribute
    {
        /// <summary>
        /// Gets a value indicating whether the property type requires an argument, either as part
        /// of the option, or following the option.
        /// </summary>
        public abstract bool RequiresArgument
        {
            get;
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
        public abstract void ValidateUsage(Type containerType);

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
        public abstract void ValidateUsage(PropertyInfo propertyInfo);
    }
}
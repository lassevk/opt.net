using System;
using System.Collections.Generic;

namespace Opt
{
    /// <summary>
    /// This class handles discovering properties of a type that can be mapped to command line arguments,
    /// and how to map them.
    /// </summary>
    public class PropertyMap
    {
        private readonly Type _Type;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyMap"/> class.
        /// </summary>
        /// <param name="type">
        /// The type to find all mappable properties for.
        /// </param>
        public PropertyMap(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (!type.IsClass)
                throw new ArgumentException("Type given must be a non-abstract class, was not a class", "type");
            if (type.IsAbstract)
                throw new ArgumentException("Type given must be a non-abstract class, was abstract", "type");

            _Type = type;
        }

        /// <summary>
        /// The type of object this <see cref="PropertyMap"/> handles.
        /// </summary>
        public Type Type
        {
            get
            {
                return _Type;
            }
        }

        /// <summary>
        /// Maps the arguments onto properties of the container.
        /// </summary>
        /// <param name="arguments">
        /// The collection of arguments to map onto properties on the <paramref name="container"/>.
        /// </param>
        /// <param name="container">
        /// The container object to map the <paramref name="arguments"/> onto.
        /// </param>
        /// <returns>
        /// Any leftover non-option arguments that couldn't be mapped. Note that if the container has
        /// a string collection property tagged with the <see cref="ArgumentsAttribute"/> attribute, the
        /// leftover arguments will be added to that property, and none will be returned from this method.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="arguments"/> is <c>null</c>.</para>
        /// <para>- or -</para>
        /// <para><paramref name="container"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <para><paramref name="container"/> is not the same <see cref="Type"/> as the original type given to this <see cref="PropertyMap"/>.</para>
        /// </exception>
        public string[] Map(IEnumerable<string> arguments, object container)
        {
            if (arguments == null)
                throw new ArgumentNullException("arguments");
            if (container == null)
                throw new ArgumentNullException("container");
            if (container.GetType() != _Type)
                throw new InvalidOperationException(
                    string.Format(
                        "The given container was not the same type as the original type given to PropertyMap, this is an internal error (aka bug), original type was {0}, container type was {1}",
                        _Type, container.GetType()));

            return new string[0];
        }
    }
}
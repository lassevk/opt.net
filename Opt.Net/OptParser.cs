using System;
using System.Collections.Generic;

namespace Opt
{
    /// <summary>
    /// This class implements the core argument parser.
    /// </summary>
    public static class OptParser
    {
        /// <summary>
        /// Parse the given arguments, and convert them into property values on a new object of
        /// type <typeparamref name="T"/>, returning the object afterwards.
        /// </summary>
        /// <typeparam name="T">
        /// The type of object to store all the arguments in.
        /// </typeparam>
        /// <param name="arguments">
        /// The arguments to parse.
        /// </param>
        /// <returns>
        /// The object containing all the parsed arguments.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="arguments"/> is <c>null</c>.</para>
        /// </exception>
        public static T Parse<T>(IEnumerable<string> arguments) where T : class, new()
        {
            if (arguments == null)
                throw new ArgumentNullException("arguments");

            var container = new T();
            Parse(container, arguments);
            return container;
        }

        /// <summary>
        /// Parse the given arguments, and convert them into property values on a new object of
        /// type <paramref name="containerType"/>, returning the object afterwards.
        /// </summary>
        /// <param name="containerType">
        /// The <see cref="Type"/> of object to store all the arguments in.
        /// </param>
        /// <param name="arguments">
        /// The arguments to parse.
        /// </param>
        /// <returns>
        /// The object containing all the parsed arguments.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="containerType"/> is <c>null</c>.</para>
        /// <para>- or -</para>
        /// <para><paramref name="arguments"/> is <c>null</c>.</para>
        /// </exception>
        public static object Parse(Type containerType, IEnumerable<string> arguments)
        {
            if (containerType == null)
                throw new ArgumentNullException("containerType");
            if (arguments == null)
                throw new ArgumentNullException("arguments");

            object container = Activator.CreateInstance(containerType);
            Parse(container, arguments);
            return container;
        }

        /// <summary>
        /// Parse the given arguments, and convert them into property values on the given
        /// <paramref name="container"/> object.
        /// </summary>
        /// <param name="container">
        /// The object to store all the arguments in.
        /// </param>
        /// <param name="arguments">
        /// The arguments to parse.
        /// </param>
        /// <returns>
        /// A collection of all the leftover arguments after parsing the given options/arguments,
        /// or an empty collection if there are none or they were put into a collection in the
        /// container object itself.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="container"/> is <c>null</c>.</para>
        /// <para>- or -</para>
        /// <para><paramref name="arguments"/> is <c>null</c>.</para>
        /// </exception>
        public static string[] Parse(object container, IEnumerable<string> arguments)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            if (arguments == null)
                throw new ArgumentNullException("arguments");

            var map = new PropertyMap(container.GetType());
            string[] leftovers = map.Map(arguments, container);
            return leftovers;
        }
    }
}
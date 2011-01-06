using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;

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

        /// <summary>
        /// Retrieves the help text for the given options container type.
        /// </summary>
        /// <typeparam name="T">
        /// The type of options container to retrieve the help text for.
        /// </typeparam>
        /// <returns>
        /// A collection of text lines, the help text.
        /// </returns>
        public static IEnumerable<string> GetHelp<T>() where T : class
        {
            return GetHelp(typeof(T));
        }

        /// <summary>
        /// Retrieves the help text for the given options container type.
        /// </summary>
        /// <param name="containerType">
        /// The type of options container to retrieve the help text for.
        /// </param>
        /// <returns>
        /// A collection of text lines, the help text.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="containerType"/> is <c>null</c>.</para>
        /// </exception>
        public static IEnumerable<string> GetHelp(Type containerType)
        {
            if (containerType == null)
                throw new ArgumentNullException("containerType");

            var map = new PropertyMap(containerType);
            if (map.MappedProperties.Any())
            {
                yield return Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location).ToLower() + " [OPTION]...";
                yield return string.Empty;
            }

            string[] containerHelp = GetHelpTextFor(containerType).ToArray();
            if (containerHelp.Length > 0)
            {
                foreach (string line in containerHelp)
                    yield return line;
                yield return string.Empty;
            }

            var propertiesWithHelpText = (from propMap in map.MappedProperties
                                          let text = GetHelpTextFor(propMap.Key).ToArray()
                                          where text.Length > 0
                                          select new
                                          {
                                              prop = propMap.Key, option = propMap.Value.Option, parameter = propMap.Value.ParameterName, text
                                          }
                                          into entry
                                          group entry by entry.prop into g
                                          select new
                                          {
                                              options = g.Select(e => e.option).OrderBy(o => o.Length).ToArray(), g.First().text, g.First().parameter
                                          }
                                          into entry2
                                          orderby (entry2.options.First() == "-h" || entry2.options.First() == "--help") ? 0 : 1, entry2.options.First()
                                          let shortOption = entry2.options.Where(o => o.Length == 2).FirstOrDefault() ?? string.Empty
                                          let longOption = entry2.options.Where(o => o.Length > 2).FirstOrDefault() ?? string.Empty
                                          select new
                                          {
                                              shortOption, longOption, entry2.options, entry2.parameter, entry2.text
                                          }).ToArray();

            if (propertiesWithHelpText.Length > 0)
            {
                yield return "options:";
                yield return string.Empty;

                int maxLongLength = propertiesWithHelpText.Max(p => p.longOption.Length + p.parameter.Length);

                foreach (var prop in propertiesWithHelpText)
                {
                    int lines = prop.text.Length;
                    if (prop.longOption.Length > 20)
                    {
                        yield return " " + (prop.shortOption.PadRight(2, ' ') + " " + prop.longOption + " " + prop.parameter).Trim();
                        var indent = new string(' ', 27);
                        foreach (string line in prop.text)
                            yield return indent + line;
                    }
                    else
                    {
                        yield return " " + prop.shortOption.PadRight(2, ' ') + " " + (prop.longOption + " " + prop.parameter).PadRight(20, ' ') + "  " + prop.text[0];
                        var indent = new string(' ', 27);
                        foreach (string line in prop.text.Skip(1))
                            yield return indent + line;
                    }
                }
            }
        }

        /// <summary>
        /// Retrieves the help text for the given reflection item.
        /// </summary>
        /// <param name="obj">
        /// The reflection object to retrieve the help text for.
        /// </param>
        /// <returns>
        /// The help text as a collection of text lines.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="obj"/> is <c>null</c>.</para>
        /// </exception>
        private static IEnumerable<string> GetHelpTextFor(MemberInfo obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            var attrs = obj.GetCustomAttributes(typeof(DescriptionAttribute), true) as DescriptionAttribute[];
            if (attrs.Length == 0)
                yield break;

            string text = attrs[0].Description;
           
            // TODO: Handle resource-based text
            using (var reader = new StringReader(text))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                    yield return line;
            }
        }
    }
}
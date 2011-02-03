using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

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
            
            // Command line help
            var parts = new List<string>();
            int argumentIndex = 1;
            foreach (var prop in map.ArgumentProperties)
            {
                var attr = (ArgumentAttribute)prop.GetCustomAttributes(typeof(ArgumentAttribute), true)[0];
                var name = attr.Name;
                if (StringEx.IsNullOrWhiteSpace(name))
                    name = "ARG" + argumentIndex;
                if (attr.Optional)
                    parts.Add("[" + name + "]");
                else
                    parts.Add(name);
                argumentIndex++;
            }

            if (map.ArgumentsProperty != null)
                parts.Add("[ARG]...");

            if (map.MappedProperties.Any())
                parts.Add("[OPTIONS]...");

            yield return Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location).ToLower() + " " + string.Join(" ", parts.ToArray());
            yield return string.Empty;

            string[] containerHelp = GetHelpTextFor(containerType).ToArray();
            if (containerHelp.Length > 0)
            {
                foreach (string line in containerHelp)
                    yield return line;
                yield return string.Empty;
            }

            var argumentsWithDescription =
                (from entry in map.ArgumentProperties.Select((property, index) => new { property, index })
                 let descriptionAttr = entry.property.GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault() as DescriptionAttribute
                 where descriptionAttr != null
                 let attr = (ArgumentAttribute)entry.property.GetCustomAttributes(typeof(ArgumentAttribute), true)[0]
                 let argName = !StringEx.IsNullOrWhiteSpace(attr.Name) ? attr.Name : ("ARG" + (entry.index + 1))
                 select new { argName, text = StringEx.SplitLines(descriptionAttr.Description).ToArray() }).ToArray();
            if (argumentsWithDescription.Length > 0)
            {
                yield return "arguments:";
                yield return string.Empty;

                int maxLongLength = argumentsWithDescription.Max(p => p.argName.Length);

                foreach (var arg in argumentsWithDescription)
                {
                    int lines = arg.text.Length;
                    if (arg.argName.Length > 20)
                    {
                        yield return " " + arg.argName;
                        var indent = new string(' ', maxLongLength + 3);
                        foreach (string line in arg.text)
                            yield return indent + line;
                    }
                    else
                    {
                        yield return " " + arg.argName.PadRight(maxLongLength, ' ') + "  " + arg.text[0];
                        var indent = new string(' ', maxLongLength + 3);
                        foreach (string line in arg.text.Skip(1))
                            yield return indent + line;
                    }
                }

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
            foreach (var line in StringEx.SplitLines(text))
                yield return line;
        }

        /// <summary>
        /// Extracts the command name from the arguments, finds the command handler class, applies all
        /// the options onto the command, and executes it.
        /// </summary>
        /// <param name="arguments">
        /// The arguments to parse.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="arguments"/> is <c>null</c>.</para>
        /// </exception>
        public static void ExecuteCommand(IEnumerable<string> arguments)
        {
            ExecuteCommand(null, arguments);
        }

        /// <summary>
        /// Extracts the command name from the arguments, finds the command handler class, applies all
        /// the options onto the command, and executes it.
        /// </summary>
        /// <param name="commonType">
        /// The <see cref="Type"/> object of the base class for the commands, used to extract
        /// common options in order to find the command, or <c>null</c> if there is
        /// no such base class, in which case the command must be the very first argument.
        /// </param>
        /// <param name="arguments">
        /// The arguments to parse.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="arguments"/> is <c>null</c>.</para>
        /// </exception>
        public static void ExecuteCommand(Type commonType, IEnumerable<string> arguments)
        {
            if (arguments == null)
                throw new ArgumentNullException("arguments");

            string command;
            int index;

            if (commonType != null)
            {
                var commonMap = new PropertyMap(commonType);
                command = commonMap.ExtractCommand(arguments, out index);
                if (command == string.Empty)
                    throw new InvalidOperationException("No command specified"); // TODO: Replace with better exception
            }
            else
            {
                command = arguments.FirstOrDefault();
                index = 0;
                if (command.StartsWith("-"))
                    throw new InvalidOperationException("No command specified"); // TODO: Replace with better exception
            }

            var commandType = CommandAttribute.LocateCommand(command);
            if (commandType == null)
                throw new InvalidOperationException("No command with the name '" + command + "'"); // TODO: Replace with better exception

            var commandInstance = Activator.CreateInstance(commandType);
            IEnumerable<string> argumentsExceptCommand;
            if (index > 0)
                argumentsExceptCommand = arguments.Take(index).Concat(arguments.Skip(index + 1)).ToArray();
            else
                argumentsExceptCommand = arguments.Skip(1).ToArray();

            var leftOvers = Parse(commandInstance, argumentsExceptCommand);

            ((ICommand)commandInstance).Execute(leftOvers);
        }

        /// <summary>
        /// Extracts the command name from the arguments, finds the command handler class, applies all
        /// the options onto the command, and executes it.
        /// </summary>
        /// <typeparam name="TCommon">
        /// The type of the common options class.
        /// </typeparam>
        /// <param name="arguments">
        /// The arguments to parse.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="arguments"/> is <c>null</c>.</para>
        /// </exception>
        public static void ExecuteCommand<TCommon>(IEnumerable<string> arguments)
            where TCommon : ICommand
        {
            ExecuteCommand(typeof(TCommon), arguments);
        }
    }
}
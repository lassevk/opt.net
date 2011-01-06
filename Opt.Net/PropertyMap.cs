using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Opt
{
    /// <summary>
    /// This class handles discovering properties of a type that can be mapped to command line arguments,
    /// and how to map them.
    /// </summary>
    public class PropertyMap
    {
        /// <summary>
        /// This is the backing field for the <see cref="ContainerType"/> property.
        /// </summary>
        private readonly Type _ContainerType;

        /// <summary>
        /// This field holds a map of all the options that the map holds, along with
        /// information about which property and attribute declared them.
        /// </summary>
        private readonly Dictionary<string, KeyValuePair<PropertyInfo, BaseOptionAttribute>> _Properties = new Dictionary<string, KeyValuePair<PropertyInfo, BaseOptionAttribute>>();

        /// <summary>
        /// This field holds the <see cref="PropertyInfo"/> object for the property on
        /// the container object that will get all the leftover arguments, or <c>null</c>
        /// if no such property has been declared.
        /// </summary>
        private PropertyInfo _ArgumentsProperty;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyMap"/> class.
        /// </summary>
        /// <param name="containerType">
        /// The type to find all mappable properties for.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="containerType"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><paramref name="containerType"/> is not a class (<see cref="Type.IsClass"/> returns <c>false</c>.)</para>
        /// <para>- or -</para>
        /// <para><paramref name="containerType"/> is an abstract class (<see cref="Type.IsAbstract"/> returns <c>true</c>.)</para>
        /// </exception>
        public PropertyMap(Type containerType)
        {
            if (containerType == null)
                throw new ArgumentNullException("containerType");
            if (!containerType.IsClass)
                throw new ArgumentException("Type given must be a non-abstract class, was not a class", "containerType");
            if (containerType.IsAbstract)
                throw new ArgumentException("Type given must be a non-abstract class, was abstract", "containerType");

            _ContainerType = containerType;
            DiscoverMappableProperties();
        }

        /// <summary>
        /// Gets the type of object this <see cref="PropertyMap"/> handles.
        /// </summary>
        public Type ContainerType
        {
            get
            {
                return _ContainerType;
            }
        }

        /// <summary>
        /// Discover all the mappable properties from the container type.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// An unknown descendant type of <see cref="BasePropertyAttribute"/> was detected.
        /// </exception>
        private void DiscoverMappableProperties()
        {
            // List<Tuple<PropertyInfo, ArgumentAttribute>> argumentProperties = new List<Tuple<PropertyInfo, ArgumentAttribute>>();
            foreach (PropertyInfo property in _ContainerType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (!property.IsDefined(typeof(BasePropertyAttribute), true))
                    continue;

                BasePropertyAttribute[] attributes = property.GetCustomAttributes(typeof(BasePropertyAttribute), true).Cast<BasePropertyAttribute>().ToArray();
                foreach (BasePropertyAttribute attr in attributes)
                {
                    attr.ValidateUsage(_ContainerType);
                    attr.ValidateUsage(property);

                    if (attr.GetType() == typeof(ArgumentsAttribute))
                        _ArgumentsProperty = property;
                        /*
                    else if (attr == typeof(ArgumentAttribute))
                        argumentProperties.Add(property);
                    */
                    else
                    {
                        var option = attr as BaseOptionAttribute;
                        if (option != null)
                            _Properties.Add(option.Option, new KeyValuePair<PropertyInfo, BaseOptionAttribute>(property, option));
                        else
                            throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Internal error, unknown type of property attribute discovered, type {0}", attr.GetType()));
                    }
                }
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
        /// <para><paramref name="container"/> is not the same <see cref="ContainerType"/> as the original type given to this <see cref="PropertyMap"/>.</para>
        /// </exception>
        /// <exception cref="UnknownOptionException">
        /// An unknown option was specified on the command line, one that was not declared in the container type.
        /// </exception>
        /// <exception cref="OptionSyntaxException">
        /// Argument starts with three or more minus signs, this is not legal.
        /// </exception>
        public string[] Map(IEnumerable<string> arguments, object container)
        {
            if (arguments == null)
                throw new ArgumentNullException("arguments");
            if (container == null)
                throw new ArgumentNullException("container");
            if (container.GetType() != _ContainerType)
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "The given container was not the same type as the original type given to PropertyMap, this is an internal error (aka bug), original type was {0}, container type was {1}", _ContainerType, container.GetType()));

            var leftovers = new List<string>();
            using (IEnumerator<string> argumentEnumerable = arguments.GetEnumerator())
            {
                while (argumentEnumerable.MoveNext())
                {
                    string argument = argumentEnumerable.Current;

                    if ((argument.StartsWith("--", StringComparison.Ordinal) || argument.StartsWith("-", StringComparison.Ordinal)) && !argument.StartsWith("---", StringComparison.Ordinal))
                    {
                        string option;
                        string value;

                        if (argument.StartsWith("--", StringComparison.Ordinal))
                        {
                            SplitOptionAndArgument(argument, out option, out value);
                        }
                        else
                        {
                            option = argument.Substring(0, 2);
                            value = argument.Substring(2);
                        }

                        KeyValuePair<PropertyInfo, BaseOptionAttribute> entry;
                        if (_Properties.TryGetValue(option, out entry))
                        {
                            if (entry.Value.RequiresArgument)
                            {
                                if (StringEx.IsNullOrWhiteSpace(value))
                                {
                                    if (!argumentEnumerable.MoveNext())
                                        throw new OptionSyntaxException(string.Format(CultureInfo.InvariantCulture, "option {0} requires an argument but none was provided", option));

                                    if (argumentEnumerable.Current.StartsWith("-"))
                                    {
                                        string possibleOption;
                                        string possibleArgument;
                                        SplitOptionAndArgument(argumentEnumerable.Current, out possibleOption, out possibleArgument);
                                        if (_Properties.ContainsKey(possibleOption))
                                            throw new OptionSyntaxException(string.Format(CultureInfo.InvariantCulture, "option {0} requires an argument but none was provided", option));
                                    }

                                    value = argumentEnumerable.Current;
                                }
                            }
                            entry.Value.AssignValueToProperty(container, entry.Key, value);
                        }
                        else
                            throw new UnknownOptionException(string.Format(CultureInfo.InvariantCulture, "Unknown option {0}", argument));
                    }
                    else if (argument.StartsWith("-", StringComparison.Ordinal))
                        throw new OptionSyntaxException("Argument starts with three or more minus signs, this is not legal");
                    else
                        leftovers.Add(argument);
                }
            }

            if (_ArgumentsProperty != null)
            {
                var argumentsProperty = _ArgumentsProperty.GetValue(container, null) as Collection<string>;
                if (argumentsProperty == null)
                {
                    if (_ArgumentsProperty.CanWrite)
                    {
                        argumentsProperty = new Collection<string>(new List<string>());
                        _ArgumentsProperty.SetValue(container, argumentsProperty, null);
                    }
                    else
                        throw new InvalidOperationException("The container has a property that has the ArgumentsAttribute attribute, but this property returns a null collection reference, and is not writeable");
                }

                foreach (string leftover in leftovers)
                    argumentsProperty.Add(leftover);
                return new string[0];
            }

            return leftovers.ToArray();
        }

        private void SplitOptionAndArgument(string argument, out string option, out string value)
        {
            int valueIndex = argument.IndexOf('=');
            if (valueIndex < 0)
            {
                option = argument;
                value = string.Empty;
            }
            else
            {
                option = argument.Substring(0, valueIndex).Trim();
                value = argument.Substring(valueIndex + 1);
            }
        }
    }
}
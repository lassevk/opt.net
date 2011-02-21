using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Opt
{
    /// <summary>
    /// This attribute can be applied to classes that implement <see cref="ICommand"/>
    /// to specify the name of the command they implement.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class CommandAttribute : Attribute
    {
        /// <summary>
        /// This is the backing field for the <see cref="Name"/> property.
        /// </summary>
        private string _Name;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandAttribute"/> class.
        /// </summary>
        /// <param name="name">
        /// The name of the command the target class implements.
        /// </param>
        public CommandAttribute(string name)
        {
            _Name = (name ?? string.Empty).Trim();
        }

        /// <summary>
        /// Gets or sets the name of the command the target class implements.
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
        /// Returns a collection of all command classes that can be found.
        /// </summary>
        /// <returns>
        /// A collection of all command classes that can be found.
        /// </returns>
        public static IEnumerable<Type> LocateAllCommands()
        {
            Type requiredInterface = typeof(ICommand);
            return
                from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from type in assembly.GetTypes()
                where type.IsClass
                      && !type.IsAbstract
                      && requiredInterface.IsAssignableFrom(type)
                      && type.IsDefined(typeof(CommandAttribute), true)
                select type;
        }

        /// <summary>
        /// Locates a specific command and returns its <see cref="Type"/>.
        /// </summary>
        /// <param name="name">
        /// The name of the command to locate.
        /// </param>
        /// <returns>
        /// The <see cref="Type"/> of the command located, or <c>null</c> if there is
        /// no such command.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="name"/> is <c>null</c> or empty.</para>
        /// </exception>
        public static Type LocateCommand(string name)
        {
            if (StringEx.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");

            return
                (from type in LocateAllCommands()
                 from CommandAttribute attr in type.GetCustomAttributes(typeof(CommandAttribute), true)
                 where attr.Name == name
                 select type).FirstOrDefault();
        }
    }
}
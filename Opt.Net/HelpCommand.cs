using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Opt
{
    /// <summary>
    /// This <see cref="ICommand"/> command handler implements the "help" command.
    /// </summary>
    [Command("help")]
    [Description("shows help for built-in commands")]
    public sealed class HelpCommand : ICommand
    {
        #region ICommand Members

        /// <summary>
        /// Execute the command.
        /// </summary>
        /// <param name="arguments">
        /// Any leftover arguments that wasn't parsed into any properties on the command.
        /// </param>
        public void Execute(string[] arguments)
        {
            string topic = arguments.FirstOrDefault();
            if (topic == null)
                ShowGeneralHelp();
            else
                ShowSpecificHelp(topic);
        }

        #endregion

        /// <summary>
        /// Shows help for a specific topic.
        /// </summary>
        /// <param name="topic">
        /// The name of the topic or command to show help for.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="topic"/> is <c>null</c> or empty.</para>
        /// </exception>
        private void ShowSpecificHelp(string topic)
        {
            Type commandType = CommandAttribute.LocateCommand(topic);
            if (commandType == null)
            {
                // TODO: Replace with better exception
                throw new InvalidOperationException("No command with the name '" + topic + "'");
            }

            foreach (string line in OptParser.GetHelp(commandType))
                Console.Out.WriteLine(line);
        }

        /// <summary>
        /// Show general help.
        /// </summary>
        private void ShowGeneralHelp()
        {
            var attr = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false).FirstOrDefault() as AssemblyDescriptionAttribute;
            if (attr != null)
            {
                Console.Out.WriteLine(attr.Description);
                Console.Out.WriteLine();
            }

            Console.Out.WriteLine("list of commands:");
            Console.Out.WriteLine();

            var commands =
                (from commandType in CommandAttribute.LocateAllCommands()
                 from CommandAttribute commandAttribute in commandType.GetCustomAttributes(typeof(CommandAttribute), true)
                 let descriptionAttribute = commandType.GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault() as DescriptionAttribute
                 select new
                 {
                     name = commandAttribute.Name,
                     description = (descriptionAttribute != null) ? descriptionAttribute.Description : string.Empty
                 }
                 into e
                 orderby e.name
                 select e).ToArray();

            int maxCommandLength = commands.Max(e => e.name.Length);

            foreach (var command in commands)
                Console.Out.WriteLine(" " + (command.name.PadRight(maxCommandLength, ' ') + "  " + command.description).Trim());
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Opt
{
    /// <summary>
    /// This <see cref="ICommand"/> command handler implements the "help" command.
    /// </summary>
    [Command("help")]
    [Description("shows help for built-in commands")]
    public sealed class HelpCommand : ICommand
    {
        /// <summary>
        /// Execute the command.
        /// </summary>
        /// <param name="arguments">
        /// Any leftover arguments that wasn't parsed into any properties on the command.
        /// </param>
        public void Execute(string[] arguments)
        {
            var topic = arguments.FirstOrDefault();
            if (topic == null)
                ShowGeneralHelp();
            else
                ShowSpecificHelp(topic);
        }

        private void ShowSpecificHelp(string topic)
        {
            var commandType = CommandAttribute.LocateCommand(topic);
            if (commandType == null)
                throw new InvalidOperationException("No command with the name '" + topic + "'"); // TODO: Replace with better exception

            foreach (string line in OptParser.GetHelp(commandType))
                Console.Out.WriteLine(line);
        }

        private void ShowGeneralHelp()
        {
            var attr =
                Assembly.GetEntryAssembly().GetCustomAttributes(typeof (AssemblyDescriptionAttribute), false).
                    FirstOrDefault() as AssemblyDescriptionAttribute;
            if (attr != null)
            {
                Console.Out.WriteLine(attr.Description);
                Console.Out.WriteLine();
            }
            Console.Out.WriteLine("list of commands:");
            Console.Out.WriteLine();

            var commands = (from commandType in CommandAttribute.LocateAllCommands()
                            from CommandAttribute commandAttribute in
                                commandType.GetCustomAttributes(typeof (CommandAttribute), true)
                            let descriptionAttribute =
                                commandType.GetCustomAttributes(typeof (DescriptionAttribute), true).FirstOrDefault() as
                                DescriptionAttribute
                            select
                                new
                                    {
                                        name = commandAttribute.Name,
                                        description =
                                (descriptionAttribute != null) ? descriptionAttribute.Description : string.Empty
                                    }
                            into e
                            orderby e.name
                            select e).
                ToArray();

            var maxCommandLength = commands.Max(e => e.name.Length);

            foreach (var command in commands)
            {
                Console.Out.WriteLine(" " + (command.name.PadRight(maxCommandLength, ' ') + "  " + command.description).Trim());
            }
        }
    }
}

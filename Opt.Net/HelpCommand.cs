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

        /// <inheritdoc />
        public void Execute(string[] arguments, Action<string> standardOutputAction, Action<string> errorOutputAction)
        {
            string topic = arguments.FirstOrDefault();
            if (topic == null)
                ShowGeneralHelp(standardOutputAction, errorOutputAction);
            else
                ShowSpecificHelp(topic, standardOutputAction, errorOutputAction);
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
        private void ShowSpecificHelp(string topic, Action<string> standardOutputAction, Action<string> errorOutputAction)
        {
            Type commandType = CommandAttribute.LocateCommand(topic);
            if (commandType == null)
            {
                // TODO: Replace with better exception
                throw new InvalidOperationException("No command with the name '" + topic + "'");
            }

            foreach (string line in OptParser.GetHelp(commandType))
                standardOutputAction?.Invoke(line);
        }

        /// <summary>
        /// Show general help.
        /// </summary>
        private void ShowGeneralHelp(Action<string> standardOutputAction, Action<string> errorOutputAction)
        {
            var attr = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false).FirstOrDefault() as AssemblyDescriptionAttribute;
            if (attr != null)
            {
                standardOutputAction?.Invoke(attr.Description);
                standardOutputAction?.Invoke(string.Empty);
            }

            standardOutputAction?.Invoke("list of commands:");
            standardOutputAction?.Invoke(string.Empty);

            var commands =
                (from commandType in CommandAttribute.LocateAllCommands()
                 let commandTypeInfo = commandType.GetTypeInfo()
                 where commandTypeInfo != null
                 from CommandAttribute commandAttribute in commandTypeInfo.GetCustomAttributes(typeof(CommandAttribute), true)
                 let descriptionAttribute = commandTypeInfo.GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault() as DescriptionAttribute
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
                standardOutputAction?.Invoke(" " + (command.name.PadRight(maxCommandLength, ' ') + "  " + command.description).Trim());
        }
    }
}
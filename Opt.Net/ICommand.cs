using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Opt
{
    /// <summary>
    /// This interface must be implemented by classes that will function as commands in the
    /// command-pattern.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Execute the command.
        /// </summary>
        /// <param name="arguments">
        /// Any leftover arguments that wasn't parsed into any properties on the command.
        /// </param>
        void Execute(string[] arguments);
    }
}
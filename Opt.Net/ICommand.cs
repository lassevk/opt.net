using System;

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
        /// <param name="standardOutputAction">
        /// This action, if non-<c>null</c>, will be called with standard output ("standard" means non-error text).
        /// </param>
        /// <param name="errorOutputAction">
        /// This action, if non-<c>null</c>, will be called with error output. If this parameter is <c>null</c>
        /// but <paramref name="standardOutputAction"/> is non-<c>null</c> then error output will go to standard
        /// output instead.
        /// </param>
        void Execute(string[] arguments, Action<string> standardOutputAction, Action<string> errorOutputAction);
    }
}
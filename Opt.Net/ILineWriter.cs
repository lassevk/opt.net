using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Opt
{
    /// <summary>
    /// This interface is used by the help part of Opt.Net, and the standard console command line interface
    /// helper classes of Opt.Net, to have a place to write help text, error messages, output, etc.
    /// This can be the console (see <see cref="ConsoleLineWriter"/>), or a custom class to catch the
    /// text, for instance to incorporate it into a GUI-based application.
    /// </summary>
    public interface ILineWriter
    {
        /// <summary>
        /// When implemented in a class, will write out a single line of text to the text target.
        /// </summary>
        /// <param name="line">
        /// The line of text to write out. Empty lines are given as <see cref="string.Empty"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="line"/> is <c>null</c>.</para>
        /// </exception>
        void WriteLine(string line);

        /// <summary>
        /// When implemented in a class, will write out a single line of error text to the text target.
        /// </summary>
        /// <param name="line">
        /// The line of error text to write out. Empty lines are given as <see cref="String.Empty"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="line"/> is <c>null</c>.</para>
        /// </exception>
        void WriteErrorLine(string line);
    }
}
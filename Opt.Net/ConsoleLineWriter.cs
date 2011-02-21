using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Opt
{
    /// <summary>
    /// Implements <see cref="ILineWriter"/> by writing out all the text to the console.
    /// </summary>
    public class ConsoleLineWriter : ILineWriter
    {
        #region ILineWriter Members

        /// <summary>
        /// When implemented in a class, will write out a single line of text to the text target.
        /// </summary>
        /// <param name="line">
        /// The line of text to write out. Empty lines are given as <see cref="string.Empty"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="line"/> is <c>null</c>.</para>
        /// </exception>
        public void WriteLine(string line)
        {
            if (line == null)
                throw new ArgumentNullException("line");

            Console.Out.WriteLine(line);
        }

        /// <summary>
        /// When implemented in a class, will write out a single line of error text to the text target.
        /// </summary>
        /// <param name="line">
        /// The line of error text to write out. Empty lines are given as <see cref="String.Empty"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="line"/> is <c>null</c>.</para>
        /// </exception>
        public void WriteErrorLine(string line)
        {
            if (line == null)
                throw new ArgumentNullException("line");

            Console.Error.WriteLine(line);
        }

        #endregion
    }
}
﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Opt
{
    /// <summary>
    /// This class will correctly recurse response files and produce all the arguments, in order. Any
    /// <c>null</c>-elements will be ignored along the way.
    /// </summary>
    public class ArgumentsCollection : IEnumerable<string>
    {
        private readonly IEnumerable<string> _Arguments;
        private readonly HashSet<string> _ProcessedResponseFiles = new HashSet<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentsCollection"/> class.
        /// </summary>
        /// <param name="arguments">
        /// The arguments to produce from.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="arguments"/> is <c>null</c>.</para>
        /// </exception>
        public ArgumentsCollection(IEnumerable<string> arguments)
        {
            if (arguments == null)
                throw new ArgumentNullException("arguments");

            _Arguments = arguments;
        }

        #region IEnumerable<string> Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<string> GetEnumerator()
        {
            foreach (string argument in _Arguments)
            {
                if (argument == null)
                    continue;

                if (argument.StartsWith("@"))
                {
                    foreach (string argumentFromResponseFile in ProcessResponseFile(argument.Substring(1)))
                        yield return argumentFromResponseFile;
                }
                else
                    yield return argument;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        private IEnumerable<string> ProcessResponseFile(string fileName)
        {
            Debug.Assert(!StringEx.IsNullOrWhiteSpace(fileName));

            fileName = Path.GetFullPath(fileName);
            if (_ProcessedResponseFiles.Contains(fileName))
                throw new InvalidOperationException(string.Format("Response file '{0}' has been processed more than once, this is not allowed", fileName));

            _ProcessedResponseFiles.Add(fileName);
            using (var reader = new StreamReader(fileName))
            {
                string argument;
                while ((argument = reader.ReadLine()) != null)
                {
                    if (argument.StartsWith("@"))
                    {
                        foreach (string nestedResponseFileArgument in ProcessResponseFile(argument.Substring(1)))
                            yield return nestedResponseFileArgument;
                    }
                    else
                        yield return argument;
                }
            }
        }
    }
}
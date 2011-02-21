using System.Collections.Generic;
using System.IO;

namespace Opt
{
    /// <summary>
    /// This class implements IsNullOrWhiteSpace for .NET 3.5.
    /// </summary>
    internal static class StringEx
    {
        /// <summary>
        /// Compares the <paramref name="value"/> against <c>null</c> and checks if the
        /// string contains only whitespace.
        /// </summary>
        /// <param name="value">
        /// The string value to check.
        /// </param>
        /// <returns>
        /// <c>true</c> if the string <paramref name="value"/> is <c>null</c>, <see cref="string.Empty"/>,
        /// or contains only whitespace; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrWhiteSpace(string value)
        {
            return value == null || value.Trim().Length == 0;
        }

        /// <summary>
        /// Splits the text into separate lines and returns them as a collection of strings.
        /// </summary>
        /// <param name="text">
        /// The text to split into lines.
        /// </param>
        /// <returns>
        /// A collection of lines from the text.
        /// </returns>
        public static IEnumerable<string> SplitLines(string text)
        {
            if (IsNullOrWhiteSpace(text))
                yield break;

            using (var reader = new StringReader(text))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                    yield return line;
            }
        }
    }
}
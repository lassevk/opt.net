using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Opt.Tests
{
    [TestFixture]
    public class ArgumentsCollectionTests : TestBase
    {
        [Test]
        public void Constructor_NullArguments_ThrowsArgumentNullException()
        {
            IEnumerable<string> arguments = null;

            Assert.Throws<ArgumentNullException>(() => new ArgumentsCollection(arguments));
        }

        [Test]
        public void Enumerate_CyclicResponseFiles_ThrowsInvalidOperationException()
        {
            string tempFileName1 = GetTempFileName();
            string tempFileName2 = GetTempFileName();

            File.WriteAllLines(tempFileName1, new[]
            {
                "2", "@" + tempFileName2, "3"
            });
            File.WriteAllLines(tempFileName2, new[]
            {
                "2", "@" + tempFileName1, "3"
            });

            IEnumerable<string> input = new[]
            {
                "1", "@" + tempFileName1, "4"
            };

            Assert.Throws<InvalidOperationException>(() => new ArgumentsCollection(input).ToArray());
        }

        [Test]
        public void Enumerate_ResponseFileListedTwice_ThrowsInvalidOperationException()
        {
            string tempFileName = GetTempFileName();

            File.WriteAllLines(tempFileName, new[]
            {
                "2", "3"
            });

            IEnumerable<string> input = new[]
            {
                "1", "@" + tempFileName, "@" + tempFileName, "4"
            };

            Assert.Throws<InvalidOperationException>(() => new ArgumentsCollection(input).ToArray());
        }

        [Test]
        public void Enumerate_WithEmptyResponseFile_ReturnsOriginalInputMinusResponseFileElement()
        {
            string tempFileName = GetTempFileName();
            File.WriteAllText(tempFileName, String.Empty);

            IEnumerable<string> input = new[]
            {
                "1", "@" + tempFileName, "2"
            };
            IEnumerable<string> expected = new[]
            {
                "1", "2"
            };

            string[] output = new ArgumentsCollection(input).ToArray();

            CollectionAssert.AreEqual(expected, output);
        }

        [Test]
        public void Enumerate_WithNestedResponseFiles_ReturnsOriginalInputWithContentsOfResponseFilesInline()
        {
            string tempFileName1 = GetTempFileName();
            string tempFileName2 = GetTempFileName();

            File.WriteAllLines(tempFileName1, new[]
            {
                "2", "@" + tempFileName2, "5"
            });
            File.WriteAllLines(tempFileName2, new[]
            {
                "3", "4"
            });

            IEnumerable<string> input = new[]
            {
                "1", "@" + tempFileName1, "6"
            };
            IEnumerable<string> expected = new[]
            {
                "1", "2", "3", "4", "5", "6"
            };

            string[] output = new ArgumentsCollection(input).ToArray();

            CollectionAssert.AreEqual(expected, output);
        }

        [Test]
        public void Enumerate_WithNullElements_ReturnsOriginalInputMinusNullElements()
        {
            IEnumerable<string> input = new[]
            {
                "1", "2", null, "3", "A", null, "B", "C", string.Empty, null
            };
            IEnumerable<string> expected = new[]
            {
                "1", "2", "3", "A", "B", "C", string.Empty
            };

            string[] output = new ArgumentsCollection(input).ToArray();

            CollectionAssert.AreEqual(expected, output);
        }

        [Test]
        public void Enumerate_WithResponseFile_ReturnsOriginalInputWithContentsOfResponseFileInline()
        {
            string tempFileName = GetTempFileName();
            File.WriteAllLines(tempFileName, new[]
            {
                "2", "3"
            });

            IEnumerable<string> input = new[]
            {
                "1", "@" + tempFileName, "4"
            };
            IEnumerable<string> expected = new[]
            {
                "1", "2", "3", "4"
            };

            string[] output = new ArgumentsCollection(input).ToArray();

            CollectionAssert.AreEqual(expected, output);
        }

        [Test]
        public void Enumerate_WithSimpleArgumentsInput_ReturnsOriginalInput()
        {
            IEnumerable<string> arguments = new[]
            {
                "1", "2", "3", "A", "B", "C", string.Empty
            };

            string[] output = new ArgumentsCollection(arguments).ToArray();

            CollectionAssert.AreEqual(arguments, output);
        }

        [Test]
        public void NonGenericEnumerate_WithSimpleArgumentsInput_ReturnsOriginalInput()
        {
            IEnumerable<string> arguments = new[]
            {
                "1", "2", "3", "A", "B", "C", string.Empty
            };

            var collection = new ArgumentsCollection(arguments);
            var enumerable = collection as IEnumerable;

            var output = new List<string>();
            foreach (string element in enumerable)
                output.Add(element);

            CollectionAssert.AreEqual(arguments, output);
        }
    }
}
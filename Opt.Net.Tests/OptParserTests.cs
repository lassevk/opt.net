using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Opt.Tests
{
    [TestFixture]
    public class OptParserTests
    {
        [Test]
        public void ParseGeneric_NullArguments_ThrowsArgumentNullException()
        {
            IEnumerable<string> arguments = null;

            Assert.Throws<ArgumentNullException>(() => OptParser.Parse<OptParserTests>(arguments));
        }

        [Test]
        public void ParseGeneric_WithEmptyArguments_ReturnsObjectOfRightType()
        {
            IEnumerable<string> arguments = Enumerable.Empty<string>();

            var container = OptParser.Parse<OptParserTests>(arguments);

            Assert.That(container, Is.Not.Null);
            Assert.That(container, Is.InstanceOf(typeof(OptParserTests)));
        }

        [Test]
        public void ParseWithContainerType_WithEmptyArguments_ReturnsObjectOfRightType()
        {
            object container = OptParser.Parse(typeof(OptParserTests), Enumerable.Empty<string>());

            Assert.That(container, Is.Not.Null);
            Assert.That(container, Is.InstanceOf(typeof(OptParserTests)));
        }

        [Test]
        public void ParseWithContainer_NullArguments_ThrowsArgumentNullException()
        {
            var container = new object();
            IEnumerable<string> arguments = null;

            Assert.Throws<ArgumentNullException>(() => OptParser.Parse(container, arguments));
        }

        [Test]
        public void ParseWithContainer_NullContainer_ThrowsArgumentNullException()
        {
            object container = null;
            IEnumerable<string> arguments = Enumerable.Empty<string>();

            Assert.Throws<ArgumentNullException>(() => OptParser.Parse(container, arguments));
        }

        [Test]
        public void ParseWithType_NullArguments_ThrowsArgumentNullException()
        {
            Type containerType = typeof(OptParserTests);
            IEnumerable<string> arguments = null;

            Assert.Throws<ArgumentNullException>(() => OptParser.Parse(containerType, arguments));
        }

        [Test]
        public void ParseWithType_NullContainerType_ThrowsArgumentNullException()
        {
            Type containerType = null;
            IEnumerable<string> arguments = Enumerable.Empty<string>();

            Assert.Throws<ArgumentNullException>(() => OptParser.Parse(containerType, arguments));
        }
    }
}
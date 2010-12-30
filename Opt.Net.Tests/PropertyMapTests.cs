using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace Opt.Tests
{
    [TestFixture]
    public class PropertyMapTests
    {
        [Test]
        public void Constructor_NullType_ThrowsArgumentNullException()
        {
            Type type = null;

            Assert.Throws<ArgumentNullException>(() => new PropertyMap(type));
        }

        [Test]
        public void Constructor_TypeGivenIsAbstract_ThrowsArgumentException()
        {
            Type type = typeof (TextReader);

            Assert.Throws<ArgumentException>(() => new PropertyMap(type));
        }

        [Test]
        public void Constructor_TypeGivenIsNotAClass_ThrowsArgumentException()
        {
            Type type = typeof (ICloneable);

            Assert.Throws<ArgumentException>(() => new PropertyMap(type));
        }

        [Test]
        public void Map_ContainerTypeMismatch_ThrowsInvalidOperationException()
        {
            IEnumerable<string> arguments = new string[0];
            var container = new object();
            var map = new PropertyMap(typeof (FileStream));

            Assert.Throws<InvalidOperationException>(() => map.Map(arguments, container));
        }

        [Test]
        public void Map_NullArguments_ThrowsArgumentNullException()
        {
            IEnumerable<string> arguments = null;
            var container = new object();
            var map = new PropertyMap(typeof (object));

            Assert.Throws<ArgumentNullException>(() => map.Map(arguments, container));
        }

        [Test]
        public void Map_NullContainer_ThrowsArgumentNullException()
        {
            IEnumerable<string> arguments = new string[0];
            object container = null;
            var map = new PropertyMap(typeof (object));

            Assert.Throws<ArgumentNullException>(() => map.Map(arguments, container));
        }

        [Test]
        public void Type_WhenProperTypeGivenToConstructor_ContainsTypeGivenToConstructor()
        {
            Type type = typeof (FileStream);
            var map = new PropertyMap(type);

            Assert.That(map.Type, Is.EqualTo(type));
        }
    }
}
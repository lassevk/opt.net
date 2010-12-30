using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using NUnit.Framework;

namespace Opt.Tests
{
    [TestFixture]
    public class PropertyMapTests
    {
        [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
        public class UnknownAttribute : BasePropertyAttribute
        {
            public override void ValidateUsage(Type containerType)
            {
            }

            public override void ValidateUsage(PropertyInfo propertyInfo)
            {
            }
        }

        public class ContainerWithUnknownAttribute
        {
            [UnknownAttribute]
            public string SomeProperty
            {
                get;
                set;
            }
        }

        public class ContainerWithArguments
        {
            private readonly Collection<string> _Args = new Collection<string>(new List<string>());

            [Arguments]
            public Collection<string> Args
            {
                get
                {
                    return _Args;
                }
            }
        }

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
        public void IncorrectAttributeClass_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => new PropertyMap(typeof (ContainerWithUnknownAttribute)));
        }

        [Test]
        public void LeftOverArguments_AreStoredIntoTheArgumentsProperty()
        {
            var container = new ContainerWithArguments();
            var arguments = new[] { "a", "b" };
            var map = new PropertyMap(container.GetType());

            string[] leftovers = map.Map(arguments, container);

            CollectionAssert.AreEqual(leftovers, new string[0]);
            CollectionAssert.AreEqual(container.Args, arguments);
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

            Assert.That(map.ContainerType, Is.EqualTo(type));
        }
    }
}